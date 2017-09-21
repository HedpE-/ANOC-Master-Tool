using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace appCore.Toolbox.Notifications
{
    [DelimitedRecord("||")]
    public class Notification
    {
        [FieldOrder(1)]
        public string Title { get; private set; }
        [FieldOrder(2)]
        public string Body { get; private set; }
        [FieldOrder(3)]
        public bool Recurrent { get; private set; }
        [FieldOrder(4)]
        public int Recurrency { get; private set; }
        [FieldOrder(5)]
        string _recurrencyType;
        public RecurrencyType Recurrency_Type
        {
            get
            {
                return EnumExtensions.Parse(typeof(RecurrencyType), _recurrencyType);
            }
            private set
            {
                _recurrencyType = value.GetDescription();
            }
        }

        [FieldHidden]
        DateTime expiryDateTime;
        public DateTime ExpiryDateTime { get { return expiryDateTime; } private set { expiryDateTime = value; } }
        [FieldHidden]
        public bool isRead;
        //public bool isRead { get { return read; } set { read = value; } }
        [FieldHidden]
        public string rawData;

        public Notification() { }

        public Notification(string title, string body)
        {
            Title = title;
            Body = body;
            //try
            //{
            //    ExpiryDateTime = Convert.ToDateTime(expiryDate);
            //}
            //catch { }
            
            //if (ExpiryDateTime > DateTime.Now)
                isRead = false;
            //else
            //    isRead = true;
        }

        public Notification(string title, string body, bool recurrent, int recurrency, RecurrencyType recurrencyType)
        {
            Title = title;
            Body = body;
            Recurrent = recurrent;
            Recurrency = recurrency;
            Recurrency_Type = recurrencyType;

            //if (ExpiryDateTime > DateTime.Now)
                isRead = false;
            //else
            //    isRead = true;
        }

        public bool Equal(Notification compareTo)
        {
            if (rawData != compareTo.rawData)
                return false;

            return true;
        }
    }
}
