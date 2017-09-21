using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace appCore.Toolbox.Notifications
{
    /// <summary>
    /// Summary description for TipOfTheDayDialog.
    /// </summary>
    public class NotificationsForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private Label lblTitle;
        private Label lblNotificationBody;
        private Label lblNotificationCount;
        private Panel panel1;
        private Button btnNextNotification;
        private Button btnClose;
        private Button btnManage;

        public List<Notification> _notifications = new List<Notification>();
        private Label lblNotificationTitle;
        private int _currentNotificationIndex;

        public NotificationsForm(List<Notification> notificationsList)
        {
            InitializeComponent();

            if(Settings.CurrentUser.UserName == "GONCARJ3" || Settings.CurrentUser.UserName == "PANCHOPJ")
                btnManage.Visible = true;
            
            _notifications = notificationsList;

            //try
            //{
            //    if(_notifications.Count > 0)
                    UpdateNotification();
            //    else
            //    {
            //        btnNextNotification.Enabled = false;
            //        btnClose.Enabled = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(String.Format("Trouble Creating Notifications Dialog {0}", ex.ToString()));
            //    btnNextNotification.Enabled = false;
            //    btnClose.Enabled = true;
            //}
        }

        private void UpdateNotification()
        {
            if (_notifications.Count > 0)
            {
                lblNotificationTitle.Text = _notifications[_currentNotificationIndex].Title;
                lblNotificationBody.Text = _notifications[_currentNotificationIndex].Body;
                lblNotificationCount.Text = string.Format("({0}/{1})", _currentNotificationIndex + 1, _notifications.Count());
                
                btnClose.Enabled = _notifications.Where(n => n.isRead).Count() == _notifications.Count;
                btnNextNotification.Enabled = true;
            }
            else
            {
                lblNotificationTitle.Text = string.Empty;
                lblNotificationBody.Text = "No notifications to be shown";
                lblNotificationCount.Text = "(0/0)";
                btnClose.Enabled = true;
                btnNextNotification.Enabled = false;
            }
        }

        private void btnNextNotification_Click(object sender, EventArgs e)
        {
            _notifications[_currentNotificationIndex].isRead = true;

            if (_currentNotificationIndex == _notifications.Count - 1)
                _currentNotificationIndex = 0;
            else
                _currentNotificationIndex++;

            UpdateNotification();
        }

        private void btnManage_Click(object sender, EventArgs e)
        {
            UI.LoadingPanel loading = new UI.LoadingPanel();
            loading.Show(false, this);
            try
            {
                NotificationsManager manager = new NotificationsManager(_notifications);
                manager.StartPosition = FormStartPosition.CenterParent;
                manager.ShowDialog();

                _notifications = manager._notifications;

                if (_currentNotificationIndex > _notifications.Count - 1)
                    _currentNotificationIndex = 0;

                UpdateNotification();
            }
            catch (Exception ex)
            {
                var m = ex.Message;
            }

            loading.Close();
        }

        private void NotificationsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!btnClose.Enabled)
                e.Cancel = true;
            else
                MainForm.UpdateNotificationsIcon();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotificationsForm));
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblNotificationBody = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblNotificationTitle = new System.Windows.Forms.Label();
            this.btnNextNotification = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblNotificationCount = new System.Windows.Forms.Label();
            this.btnManage = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Trebuchet MS", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.DarkRed;
            this.lblTitle.Location = new System.Drawing.Point(3, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(291, 37);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Notifications Center";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblNotificationBody
            // 
            this.lblNotificationBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNotificationBody.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNotificationBody.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotificationBody.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lblNotificationBody.Location = new System.Drawing.Point(15, 87);
            this.lblNotificationBody.Name = "lblNotificationBody";
            this.lblNotificationBody.Size = new System.Drawing.Size(270, 156);
            this.lblNotificationBody.TabIndex = 2;
            this.lblNotificationBody.Text = "Could not read notifications file...";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.lblNotificationTitle);
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Controls.Add(this.lblNotificationBody);
            this.panel1.Location = new System.Drawing.Point(8, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 258);
            this.panel1.TabIndex = 3;
            // 
            // lblNotificationTitle
            // 
            this.lblNotificationTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNotificationTitle.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotificationTitle.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblNotificationTitle.Location = new System.Drawing.Point(15, 55);
            this.lblNotificationTitle.Name = "lblNotificationTitle";
            this.lblNotificationTitle.Size = new System.Drawing.Size(270, 22);
            this.lblNotificationTitle.TabIndex = 3;
            this.lblNotificationTitle.Text = "Title";
            this.lblNotificationTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnNextNotification
            // 
            this.btnNextNotification.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextNotification.BackColor = System.Drawing.SystemColors.Control;
            this.btnNextNotification.Image = ((System.Drawing.Image)(resources.GetObject("btnNextNotification.Image")));
            this.btnNextNotification.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNextNotification.Location = new System.Drawing.Point(128, 274);
            this.btnNextNotification.Name = "btnNextNotification";
            this.btnNextNotification.Size = new System.Drawing.Size(116, 24);
            this.btnNextNotification.TabIndex = 5;
            this.btnNextNotification.Text = "&Next Notification";
            this.btnNextNotification.UseVisualStyleBackColor = false;
            this.btnNextNotification.Click += new System.EventHandler(this.btnNextNotification_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.SystemColors.Control;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Enabled = false;
            this.btnClose.Location = new System.Drawing.Point(250, 274);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(58, 24);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // lblNotificationCount
            // 
            this.lblNotificationCount.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNotificationCount.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotificationCount.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblNotificationCount.Location = new System.Drawing.Point(5, 271);
            this.lblNotificationCount.Name = "lblNotificationCount";
            this.lblNotificationCount.Size = new System.Drawing.Size(45, 16);
            this.lblNotificationCount.TabIndex = 3;
            this.lblNotificationCount.Text = "(0/0)";
            // 
            // btnManage
            // 
            this.btnManage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnManage.BackColor = System.Drawing.SystemColors.Control;
            this.btnManage.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnManage.Location = new System.Drawing.Point(47, 274);
            this.btnManage.Name = "btnManage";
            this.btnManage.Size = new System.Drawing.Size(75, 24);
            this.btnManage.TabIndex = 7;
            this.btnManage.Text = "&Manage...";
            this.btnManage.UseVisualStyleBackColor = false;
            this.btnManage.Visible = false;
            this.btnManage.Click += new System.EventHandler(this.btnManage_Click);
            // 
            // NotificationsForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(316, 306);
            this.Controls.Add(this.btnManage);
            this.Controls.Add(this.lblNotificationCount);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnNextNotification);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NotificationsForm";
            this.Text = "Notifications Center";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NotificationsForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
    }
}
