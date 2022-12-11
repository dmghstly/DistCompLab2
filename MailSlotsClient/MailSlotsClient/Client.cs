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
using System.IO;
using MailSlotsClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Newtonsoft.Json;
using System.Threading;

namespace MailSlots
{
    public partial class frmMain : Form
    {
        private Int32 UserMailSlot;
        private string UserMailSlotName = "\\\\*\\mailslot\\UserMailslot";
        private Int32 MainHandleMailSlot;   // дескриптор мэйлслота
        private string MainHandleMailSlotdName = "\\\\*\\mailslot\\MainMailslot";

        private Int32 RecieverMailslot;   // дескриптор канала
        private string RecieverMailslotName = "";
        private Int32 AllMessagesMailslot;   // дескриптор канала
        private string AllMessagesMailslotName = "";

        private bool _retrieveMessagesContinue = true;
        private bool _messageRecieverContinue = true;

        private Thread t1;
        private Thread t2;

        // конструктор формы
        public frmMain()
        {
            InitializeComponent();
            this.Text += "     " + Dns.GetHostName();   // выводим имя текущей машины в заголовок формы

            tbMessage.Enabled = false;
            messageBox.Enabled = false;
            btnSend.Enabled = false;
        }

        private void InitializeMailslots(string name)
        {
            RecieverMailslotName = "\\\\.\\mailslot\\RecieverMailslot" + name;
            AllMessagesMailslotName = "\\\\.\\mailslot\\AllMessagesMailslot" + name;

            RecieverMailslot = DIS.Import.CreateMailslot(RecieverMailslotName, 0, DIS.Types.MAILSLOT_WAIT_FOREVER, 0);

            AllMessagesMailslot = DIS.Import.CreateMailslot(AllMessagesMailslotName, 0, DIS.Types.MAILSLOT_WAIT_FOREVER, 0);
        }

        private void RetrieveOldMessages()
        {
            string msg = "";            // прочитанное сообщение
            int MailslotSize = 0;       // максимальный размер сообщения
            int lpNextSize = 0;         // размер следующего сообщения
            int MessageCount = 0;       // количество сообщений в мэйлслоте
            uint realBytesReaded = 0;   // количество реально прочитанных из мэйлслота байтов

            // входим в бесконечный цикл работы с каналом
            while (_retrieveMessagesContinue)
            {
                DIS.Import.GetMailslotInfo(AllMessagesMailslot, MailslotSize, ref lpNextSize, ref MessageCount, 0);

                if (MessageCount > 0)
                {
                    for (int i = 0; i < MessageCount; i++)
                    {
                        if (i == 0)
                        {
                            byte[] buff = new byte[1024];                           // буфер прочитанных из мэйлслота байтов
                            DIS.Import.FlushFileBuffers(AllMessagesMailslot);      // "принудительная" запись данных, расположенные в буфере операционной системы, в файл мэйлслота
                            DIS.Import.ReadFile(AllMessagesMailslot, buff, 1024, ref realBytesReaded, 0);      // считываем последовательность байтов из мэйлслота в буфер buff
                            msg = Encoding.Unicode.GetString(buff);                 // выполняем преобразование байтов в последовательность символов

                            messageBox.Invoke((MethodInvoker)delegate
                            {
                                // выводим полученное сообщение на форму
                                if (msg != "")
                                    messageBox.Text += msg;
                            });

                            Thread.Sleep(500);

                            _retrieveMessagesContinue = false;
                        }
                    }
                }

                
            }
        }

        private void RecieveMessage()
        {
            string msg = "";            // прочитанное сообщение
            int MailslotSize = 0;       // максимальный размер сообщения
            int lpNextSize = 0;         // размер следующего сообщения
            int MessageCount = 0;       // количество сообщений в мэйлслоте
            uint realBytesReaded = 0;   // количество реально прочитанных из мэйлслота байтов

            // входим в бесконечный цикл работы с каналом
            while (_messageRecieverContinue)
            {
                DIS.Import.GetMailslotInfo(RecieverMailslot, MailslotSize, ref lpNextSize, ref MessageCount, 0);

                if (MessageCount > 0)
                {
                    for (int i = 0; i < MessageCount; i++)
                    {
                        byte[] buff = new byte[1024];                           // буфер прочитанных из мэйлслота байтов
                        DIS.Import.FlushFileBuffers(RecieverMailslot);      // "принудительная" запись данных, расположенные в буфере операционной системы, в файл мэйлслота
                        DIS.Import.ReadFile(RecieverMailslot, buff, 1024, ref realBytesReaded, 0);      // считываем последовательность байтов из мэйлслота в буфер buff
                        msg = Encoding.Unicode.GetString(buff);                 // выполняем преобразование байтов в последовательность символов

                        messageBox.Invoke((MethodInvoker)delegate
                        {
                            // выводим полученное сообщение на форму
                            if (msg != "")
                                messageBox.Text += msg;
                        });

                        Thread.Sleep(500);                                      // приостанавливаем работу потока перед тем, как приcтупить к обслуживанию очередного клиента
                    }
                }
            }
        }

        // присоединение к мэйлслоту
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                // открываем мэйлслот, имя которого указано в поле tbMailSlot
                MainHandleMailSlot = DIS.Import.CreateFile(MainHandleMailSlotdName, DIS.Types.EFileAccess.GenericWrite, DIS.Types.EFileShare.Read, 0, DIS.Types.ECreationDisposition.OpenExisting, 0, 0);
                UserMailSlot = DIS.Import.CreateFile(UserMailSlotName, DIS.Types.EFileAccess.GenericWrite, DIS.Types.EFileShare.Read, 0, DIS.Types.ECreationDisposition.OpenExisting, 0, 0);

                if (MainHandleMailSlot != -1 && UserMailSlot != -1)
                {
                    btnConnect.Enabled = false;
                    btnSend.Enabled = true;
                    tbMessage.Enabled = true;
                    tbName.Enabled = false;

                    uint BytesWritten = 0;
                    byte[] buff = Encoding.Unicode.GetBytes(tbName.Text);

                    DIS.Import.WriteFile(UserMailSlot, buff, Convert.ToUInt32(buff.Length), ref BytesWritten, 0);

                    InitializeMailslots(tbName.Text);

                    t1 = new Thread(RetrieveOldMessages);
                    t1.Start();

                    t2 = new Thread(RecieveMessage);
                    t2.Start();
                }
                else
                    MessageBox.Show("Не удалось подключиться к мейлслоту");
            }
            catch
            {
                MessageBox.Show("Не удалось подключиться к мейлслоту");
            }
        }

        // отправка сообщения
        private void btnSend_Click(object sender, EventArgs e)
        {
            uint BytesWritten = 0;  // количество реально записанных в мэйлслот байт
            var time = DateTime.Now.ToString("HH:mm:ss");

            JsonMessage message = new JsonMessage
            {
                Id = Guid.NewGuid(),
                ClientName = tbName.Text,
                Message = tbMessage.Text,
                Time = time
            };

            var jsonData = JsonConvert.SerializeObject(message);

            byte[] buff = Encoding.Unicode.GetBytes(jsonData);    // выполняем преобразование сообщения (вместе с идентификатором машины) в последовательность байт

            DIS.Import.WriteFile(MainHandleMailSlot, buff, Convert.ToUInt32(buff.Length), ref BytesWritten, 0);     // выполняем запись последовательности байт в мэйлслот
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DIS.Import.CloseHandle(MainHandleMailSlot);     // закрываем дескриптор мэйлслота
            DIS.Import.CloseHandle(UserMailSlot);

            _messageRecieverContinue = false;

            if (t1 != null)
                t1.Abort();          // завершаем поток

            if (t2 != null)
                t2.Abort();


            if (RecieverMailslot != -1)
                DIS.Import.CloseHandle(RecieverMailslot);     // закрываем дескриптор канала

            if (AllMessagesMailslot != -1)
                DIS.Import.CloseHandle(AllMessagesMailslot);
        }
    }
}