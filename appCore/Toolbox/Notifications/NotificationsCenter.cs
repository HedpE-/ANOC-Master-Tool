using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using FileHelpers;

namespace appCore.Toolbox.Notifications
{
    public class NotificationsCenter
    {
        public static string NotificationsFile
        {
            get;
            private set;
        } = Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + "\\notifications.bin";

        public static System.Drawing.Size GuiSize
        {
            get
            {
                return new System.Drawing.Size(316, 306);
            }
        }

        public int TotalNotificationsCount
        {
            get
            {
                return _notifications.Count();
            }
        }

        public int UnreadNotificationsCount
        {
            get
            {
                return _notifications.Where(n => !n.isRead).Count();
            }
        }

        public bool HasUnreadNotifications
        {
            get
            {
                return UnreadNotificationsCount > 0;
            }
        }

        private List<Notification> _notifications;

        private Timer NewNotificationsListener = new Timer(10 * 1000); // 10 seconds in milliseconds
        private bool ListenerActive;

        private string previousNotificationsFileMD5;

        public NotificationsCenter()
        {
            try
            {
                ReadNotificationsFile();
            }
            catch(Exception e)
            {
                throw e;
            }

            NewNotificationsListener.Elapsed += NewNotificationsListener_Elapsed;

            NewNotificationsListener.Enabled = true;
        }

        private void NewNotificationsListener_Elapsed(object sender, ElapsedEventArgs e)
        {
            int notificationFormsCount = System.Windows.Forms.Application.OpenForms.Cast<System.Windows.Forms.Form>().Count(f => f.Name.StartsWith("Notifications"));
            if(!ListenerActive && notificationFormsCount == 0)
            {
                ListenerActive = true;
                if(File.Exists(NotificationsFile))
                {
                    string md5 = Tools.CalculateMD5Hash(new FileInfo(NotificationsFile));
                    if (previousNotificationsFileMD5 != md5)
                    {
                        ReadNotificationsFile();
                        if (HasUnreadNotifications)
                            MainForm.OpenNotificationsGUI_Delegate();
                    }
                }
                ListenerActive = false;
            }
        }

        public void OpenGUI(System.Windows.Forms.FormStartPosition formStartPosition)
        {
            if(_notifications != null)
            {
                NotificationsForm notificationsForm = new NotificationsForm(_notifications);
                notificationsForm.StartPosition = formStartPosition;
                notificationsForm.ShowDialog();

                _notifications = notificationsForm._notifications;
            }
        }

        public void OpenGUI(System.Drawing.Point manualLocation)
        {
            if (_notifications != null)
            {
                NotificationsForm notificationsForm = new NotificationsForm(_notifications);
                notificationsForm.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                notificationsForm.Location = manualLocation;
                notificationsForm.ShowDialog();

                _notifications = notificationsForm._notifications;
            }
        }

        void ReadNotificationsFile()
        {
            if(File.Exists(NotificationsFile))
                previousNotificationsFileMD5 = Tools.CalculateMD5Hash(new FileInfo(NotificationsFile));
            
            List<Notification> newNotifications = new List<Notification>();
            
            BinaryReader br;

            try
            {
                br = new BinaryReader(new FileStream(NotificationsFile, FileMode.OpenOrCreate));
            }
            catch (IOException e)
            {
                throw new Exception(e.Message + "\n Cannot find path to notifications file.");
            }

            string notificationsBlock = string.Empty;
            try
            {
                notificationsBlock = br.ReadString();
            }
            catch { }
            br.Close();
            
            try
            {
                var engine = new FileHelperEngine<Notification>();
                engine.AfterReadRecord += (eng, e) => {
                    e.Record.rawData = e.RecordLine;
                };
                newNotifications = engine.ReadStringAsList(notificationsBlock);
            }
            catch (FileHelpersException e)
            {
                string f = e.Message;
            }

            if(_notifications != null)
            {
                for (int c = 0; c < newNotifications.Count; c++)
                {
                    var foundResult = _notifications.FirstOrDefault(n => n.Equal(newNotifications[c]));
                    if (foundResult != null)
                        newNotifications[c] = foundResult;
                }
            }

            _notifications = newNotifications;
        }
    }


}
