using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileHelpers;

namespace appCore.Toolbox.Notifications
{
    public class NotificationsManager : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;
        private ListBox listBox1;
        private TextBox textBox1;
        private Label label1;
        private Label label2;
        private UI.AMTRichTextBox amtRichTextBox1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private CheckBox checkBox1;
        private Label label3;
        private NumericUpDown numericUpDown1;
        private ComboBox comboBox1;

        public List<Notification> _notifications;

        public NotificationsManager(List<Notification> notifications)
        {
            InitializeComponent();

            _notifications = notifications;

            PopulateNotifications();
        }

        private void PopulateNotifications()
        {
            listBox1.Items.Clear();
            foreach (Notification notification in _notifications)
            {
                listBox1.Items.Add(notification.Title);
            }
        }

        private void ModifyNotifications(object sender, EventArgs e)
        {
            Control bt = sender as Control;
            Notification newNotification = null;
            switch (bt.Name)
            {
                case "button1": // Add
                    if (bt.Text == "Add")
                    {
                        SetReadWrite();
                        ClearForm();

                        bt.Text = "Save";
                    }
                    else
                    {
                        SetReadOnly();

                        newNotification = GenerateNotification();

                        _notifications.Add(newNotification);

                        bt.Text = "Add";
                    }
                    break;
                case "button2": // Remove
                    if(listBox1.SelectedIndex > -1)
                    {
                        UI.LoadingPanel loading = new UI.LoadingPanel();
                        loading.Show(false, this);

                        DialogResult ans = UI.FlexibleMessageBox.Show("Are you sure you want to remove the selected notification?" + Environment.NewLine + "This action can't be undone.", "Remove notification", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (ans == DialogResult.Yes)
                            _notifications.Remove(_notifications[listBox1.SelectedIndex]);

                        loading.Close();
                    }
                    break;
                case "button3": // Edit
                    if(bt.Text == "Edit")
                    {
                        SetReadWrite();

                        bt.Text = "Save";
                    }
                    else
                    {
                        SetReadOnly();

                        newNotification = GenerateNotification();

                        _notifications[listBox1.SelectedIndex] = newNotification;

                        bt.Text = "Edit";
                    }
                    break;
            }

            if (bt.Text != "Save")
            {
                WriteNotifications();

                PopulateNotifications();

                if (bt.Text == "Remove")
                    ClearForm();
            }
        }

        private void ClearForm()
        {
            textBox1.Text =
                amtRichTextBox1.Text = string.Empty;
            checkBox1.Checked = false;
        }

        private void SetReadOnly()
        {
            textBox1.ReadOnly =
            amtRichTextBox1.ReadOnly =
            numericUpDown1.ReadOnly = true;
            checkBox1.Enabled =
            comboBox1.Enabled = false;
        }

        private void SetReadWrite()
        {
            textBox1.ReadOnly =
            amtRichTextBox1.ReadOnly =
            numericUpDown1.ReadOnly = false;
            checkBox1.Enabled =
            comboBox1.Enabled = true;
        }

        Notification GenerateNotification()
        {
            if (checkBox1.Checked)
                return new Notification(textBox1.Text, amtRichTextBox1.Text, true, (int)numericUpDown1.Value, EnumExtensions.Parse(typeof(RecurrencyType), comboBox1.Text));
            else
                return new Notification(textBox1.Text, amtRichTextBox1.Text);
        }

        private void WriteNotifications()
        {
            string notificationsBlock = string.Empty;
            try
            {
                var engine = new FileHelperEngine<Notification>();

                notificationsBlock = engine.WriteString(_notifications);
            }
            catch (FileHelpersException e)
            {
                var m = e.Message;
            }


            BinaryWriter bw;

            //open or create the file
            try
            {
                bw = new BinaryWriter(new FileStream(NotificationsCenter.NotificationsFile, FileMode.OpenOrCreate));

            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot open file.");
                return;
            }

            try
            {
                bw.Write(notificationsBlock);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot write to file.");
                return;
            }
            bw.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex > -1)
            {
                int index = listBox1.SelectedIndex;
                textBox1.Text = _notifications[index].Title;
                amtRichTextBox1.Text = _notifications[index].Body;
                checkBox1.Checked = _notifications[index].Recurrent;
                if(checkBox1.Checked)
                {
                    numericUpDown1.Value = _notifications[index].Recurrency;
                    comboBox1.Text = _notifications[index].Recurrency_Type.GetDescription();
                }
            }

            button2.Enabled =
                button3.Enabled = listBox1.SelectedIndex > -1;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            label3.Visible =
                numericUpDown1.Visible =
                comboBox1.Visible = checkBox1.Checked;
        }

        private void RecurrencyItems_VisibilityChanged(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;
            if (!ctrl.Visible)
            { 
                switch (ctrl.Name)
                {
                    case "numericUpDown1":
                        numericUpDown1.Value = 0;
                        break;
                    case "comboBox1":
                        comboBox1.Text = string.Empty;
                        break;
                }
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.amtRichTextBox1 = new appCore.UI.AMTRichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 9);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(99, 290);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(207, 9);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(229, 20);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(118, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Notification Title";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(118, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Notification Body";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(117, 276);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Add";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ModifyNotifications);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(198, 276);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(77, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Remove";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.ModifyNotifications);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(281, 276);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 7;
            this.button3.Text = "Edit";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.ModifyNotifications);
            // 
            // button4
            // 
            this.button4.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button4.Location = new System.Drawing.Point(362, 276);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(74, 23);
            this.button4.TabIndex = 8;
            this.button4.Text = "Close";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(117, 250);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(127, 17);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "Recurrent notification";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(250, 251);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Notify every";
            this.label3.Visible = false;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(319, 249);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.ReadOnly = true;
            this.numericUpDown1.Size = new System.Drawing.Size(41, 20);
            this.numericUpDown1.TabIndex = 11;
            this.numericUpDown1.Visible = false;
            this.numericUpDown1.VisibleChanged += new System.EventHandler(this.RecurrencyItems_VisibilityChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.Enabled = false;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Minutes",
            "Hours",
            "StartUp"});
            this.comboBox1.Location = new System.Drawing.Point(366, 249);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(70, 21);
            this.comboBox1.TabIndex = 12;
            this.comboBox1.Visible = false;
            this.comboBox1.VisibleChanged += new System.EventHandler(this.RecurrencyItems_VisibilityChanged);
            // 
            // amtRichTextBox1
            // 
            this.amtRichTextBox1.Location = new System.Drawing.Point(117, 57);
            this.amtRichTextBox1.Name = "amtRichTextBox1";
            this.amtRichTextBox1.ReadOnly = true;
            this.amtRichTextBox1.Size = new System.Drawing.Size(319, 186);
            this.amtRichTextBox1.TabIndex = 4;
            this.amtRichTextBox1.Text = "";
            // 
            // NotificationsManager
            // 
            this.AcceptButton = this.button4;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 309);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.amtRichTextBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listBox1);
            this.MaximizeBox = false;
            this.Name = "NotificationsManager";
            this.Text = "Notifications Manager";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
