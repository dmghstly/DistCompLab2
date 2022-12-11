namespace MailSlots
{
    partial class frmMain
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSend = new System.Windows.Forms.Button();
            this.tbMessage = new System.Windows.Forms.TextBox();
            this.lblMailSlot = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.messageBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Enabled = false;
            this.btnSend.Location = new System.Drawing.Point(442, 294);
            this.btnSend.Margin = new System.Windows.Forms.Padding(4);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(121, 32);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Отправить";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // tbMessage
            // 
            this.tbMessage.Location = new System.Drawing.Point(156, 300);
            this.tbMessage.Margin = new System.Windows.Forms.Padding(4);
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.Size = new System.Drawing.Size(278, 22);
            this.tbMessage.TabIndex = 1;
            // 
            // lblMailSlot
            // 
            this.lblMailSlot.AutoSize = true;
            this.lblMailSlot.Location = new System.Drawing.Point(16, 16);
            this.lblMailSlot.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMailSlot.Name = "lblMailSlot";
            this.lblMailSlot.Size = new System.Drawing.Size(122, 16);
            this.lblMailSlot.TabIndex = 2;
            this.lblMailSlot.Text = "Введите никнейм";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(442, 7);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(121, 32);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Подключиться";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(10, 303);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(138, 16);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "Введите сообщение";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(156, 12);
            this.tbName.Margin = new System.Windows.Forms.Padding(4);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(278, 22);
            this.tbName.TabIndex = 0;
            this.tbName.Text = "SomeName";
            // 
            // messageBox
            // 
            this.messageBox.Enabled = false;
            this.messageBox.Location = new System.Drawing.Point(12, 46);
            this.messageBox.Name = "messageBox";
            this.messageBox.Size = new System.Drawing.Size(551, 241);
            this.messageBox.TabIndex = 4;
            this.messageBox.Text = "";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 339);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblMailSlot);
            this.Controls.Add(this.tbMessage);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnSend);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Клиент";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox tbMessage;
        private System.Windows.Forms.Label lblMailSlot;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.RichTextBox messageBox;
    }
}

