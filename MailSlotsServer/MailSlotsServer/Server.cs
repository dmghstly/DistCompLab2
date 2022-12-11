using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using MailSlotsServer;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace MailSlots
{
    public partial class frmMain : Form
    {
        private readonly MessagesDBContext _dbContext;

        private int UserMailSlot;
        private string UserMailSlotName = "\\\\.\\mailslot\\UserMailslot";
        private int MainHandleMailSlot;       // дескриптор мэйлслота
        private string MainMailSlotName = "\\\\.\\mailslot\\MainMailslot";    // имя мэйлслота, Dns.GetHostName() - метод, возвращающий имя машины, на которой запущено приложение
        private Thread t1;                       // поток для обслуживания мэйлслота
        private Thread t2;                       // поток для обслуживания мэйлслота
        private bool _mainContinue = true;          // флаг, указывающий продолжается ли работа с мэйлслотом

        // конструктор формы
        public frmMain()
        {
            InitializeComponent();

            _dbContext = new MessagesDBContext();

            // создание мэйлслота
            MainHandleMailSlot = DIS.Import.CreateMailslot(MainMailSlotName, 0, DIS.Types.MAILSLOT_WAIT_FOREVER, 0);
            UserMailSlot = DIS.Import.CreateMailslot(UserMailSlotName, 0, DIS.Types.MAILSLOT_WAIT_FOREVER, 0);

            // вывод имени мэйлслота в заголовок формы, чтобы можно было его использовать для ввода имени в форме клиента, запущенного на другом вычислительном узле
            this.Text += "     " + MainMailSlotName;

            // создание потока, отвечающего за работу с мэйлслотом
            t1 = new Thread(ReceiveMessage);
            t1.Start();

            t2 = new Thread(RecieveUserName);
            t2.Start();
        }

        private Guid GetClientId(string name)
        {
            var entry = _dbContext.Users.Where(u => u.UserName == name).FirstOrDefault();

            return entry.Id;
        }

        private void CreateClient(string name)
        {
            if (!_dbContext.Users.Where(u => u.UserName == name).Any())
            {
                var clientId = Guid.NewGuid();

                _dbContext.Users.Add(new Client
                {
                    Id = Guid.NewGuid(),
                    UserName = name
                });
                _dbContext.SaveChanges();
            }
        }

        private void AddMessage(JsonMessage message)
        {
            _dbContext.ClientMessages.Add(new ClientMessage
            {
                MessageContent = message.Message,
                Time = message.Time,
                UserName = message.ClientName,
                UniqueField = message.Id
            });

            _dbContext.SaveChanges();
        }

        private string GetAllMessages()
        {
            string result = "";
            var entries = _dbContext.ClientMessages.ToList();

            foreach (var entry in entries)
            {
                result += $"\n {entry.UserName} >> {entry.MessageContent} >> {entry.Time}";
            }

            return result;
        }

        private bool CheckMessageUnique(Guid id)
        {
            return _dbContext.ClientMessages.Where(cm => cm.UniqueField == id).Any();
        }

        private void SendOldMessages(string name)
        {
            uint BytesWritten = 0;  // количество реально записанных в канал байт

            var result = GetAllMessages();

            byte[] buff = Encoding.Unicode.GetBytes(result);    // выполняем преобразование сообщения (вместе с идентификатором машины) в последовательность байт

            var retrieveOldMessages = DIS.Import.CreateFile("\\\\.\\mailslot\\AllMessagesMailslot" + name, DIS.Types.EFileAccess.GenericWrite, DIS.Types.EFileShare.Read, 0, DIS.Types.ECreationDisposition.OpenExisting, 0, 0);

            DIS.Import.WriteFile(retrieveOldMessages, buff, Convert.ToUInt32(buff.Length), ref BytesWritten, 0);         // выполняем запись последовательности байт в канал

            DIS.Import.CloseHandle(retrieveOldMessages);
        }

        private List<NewMessage> FormNewMessageList(JsonMessage message)
        {
            List<NewMessage> messages = new List<NewMessage>();
            var users = _dbContext.Users.ToList();

            foreach (var user in users)
            {
                messages.Add(new NewMessage
                {
                    Message = $"\n {message.ClientName} >> {message.Message} >> {message.Time}",
                    Reciever = user.UserName
                });
            }

            return messages;
        }

        private void SendNewMessages(NewMessage newMessage)
        {
            uint BytesWritten = 0;  // количество реально записанных в канал байт

            byte[] buff = Encoding.Unicode.GetBytes(newMessage.Message);    // выполняем преобразование сообщения (вместе с идентификатором машины) в последовательность байт

            var SendNewMessagePipeHandle = DIS.Import.CreateFile("\\\\.\\mailslot\\RecieverMailslot" + newMessage.Reciever, DIS.Types.EFileAccess.GenericWrite, DIS.Types.EFileShare.Read, 0, DIS.Types.ECreationDisposition.OpenExisting, 0, 0);
            DIS.Import.WriteFile(SendNewMessagePipeHandle, buff, Convert.ToUInt32(buff.Length), ref BytesWritten, 0);         // выполняем запись последовательности байт в канал

            DIS.Import.CloseHandle(SendNewMessagePipeHandle);
        }

        private void RecieveUserName()
        {
            string msg = "";            // прочитанное сообщение
            int MailslotSize = 0;       // максимальный размер сообщения
            int lpNextSize = 0;         // размер следующего сообщения
            int MessageCount = 0;       // количество сообщений в мэйлслоте
            uint realBytesReaded = 0;   // количество реально прочитанных из мэйлслота байтов

            // входим в бесконечный цикл работы с каналом
            while (_mainContinue)
            {
                DIS.Import.GetMailslotInfo(UserMailSlot, MailslotSize, ref lpNextSize, ref MessageCount, 0);

                if (MessageCount > 0)
                {
                    for (int i = 0; i < MessageCount; i++)
                    {
                        byte[] buff = new byte[1024];                           // буфер прочитанных из мэйлслота байтов
                        DIS.Import.FlushFileBuffers(UserMailSlot);      // "принудительная" запись данных, расположенные в буфере операционной системы, в файл мэйлслота
                        DIS.Import.ReadFile(UserMailSlot, buff, 1024, ref realBytesReaded, 0);      // считываем последовательность байтов из мэйлслота в буфер buff
                        msg = Encoding.Unicode.GetString(buff);                 // выполняем преобразование байтов в последовательность символов

                        CreateClient(msg);

                        SendOldMessages(msg);

                        Thread.Sleep(500);                                      // приостанавливаем работу потока перед тем, как приcтупить к обслуживанию очередного клиента
                    }
                }
            }
        }

        private void ReceiveMessage()
        {
            string msg = "";            // прочитанное сообщение
            int MailslotSize = 0;       // максимальный размер сообщения
            int lpNextSize = 0;         // размер следующего сообщения
            int MessageCount = 0;       // количество сообщений в мэйлслоте
            uint realBytesReaded = 0;   // количество реально прочитанных из мэйлслота байтов

            // входим в бесконечный цикл работы с мэйлслотом
            while (_mainContinue)
            {
                // получаем информацию о состоянии мэйлслота
                DIS.Import.GetMailslotInfo(MainHandleMailSlot, MailslotSize, ref lpNextSize, ref MessageCount, 0);

                // если есть сообщения в мэйлслоте, то обрабатываем каждое из них
                if (MessageCount > 0)
                    for (int i = 0; i < MessageCount; i++)
                    {
                        byte[] buff = new byte[1024];                           // буфер прочитанных из мэйлслота байтов
                        DIS.Import.FlushFileBuffers(MainHandleMailSlot);      // "принудительная" запись данных, расположенные в буфере операционной системы, в файл мэйлслота
                        DIS.Import.ReadFile(MainHandleMailSlot, buff, 1024, ref realBytesReaded, 0);      // считываем последовательность байтов из мэйлслота в буфер buff
                        msg = Encoding.Unicode.GetString(buff);                 // выполняем преобразование байтов в последовательность символов

                        JsonMessage message = JsonConvert.DeserializeObject<JsonMessage>(msg);

                        var clientId = GetClientId(message.ClientName);

                        if (!CheckMessageUnique(message.Id))
                        {
                            AddMessage(message);

                            rtbMessages.Invoke((MethodInvoker)delegate
                            {
                                if (msg != "")
                                    rtbMessages.Text += "\n >> " + message.ClientName + 
                                                        " >> " + message.Message + 
                                                        " >> " + message.Time + " \n";     // выводим полученное сообщение на форму
                            });

                            var messages = FormNewMessageList(message);
                            Parallel.ForEach(messages, SendNewMessages);
                        } 

                        Thread.Sleep(500);                                      // приостанавливаем работу потока перед тем, как приcтупить к обслуживанию очередного клиента
                    }
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _mainContinue = false;      // сообщаем, что работа с мэйлслотом завершена

            if (t1 != null)
                t1.Abort();          // завершаем поток

            if (t2 != null)
                t2.Abort();          // завершаем поток

            if (MainHandleMailSlot != -1)
                DIS.Import.CloseHandle(MainHandleMailSlot);            // закрываем дескриптор мэйлслота

            if (UserMailSlot != -1)
                DIS.Import.CloseHandle(UserMailSlot);
        }
    }
}