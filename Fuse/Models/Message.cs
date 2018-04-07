using System;

namespace Fuse.Models
{
    internal class Message
    {
        private User _Author;
        private DateTime _TimeStamp;
        private string _Content;
        private bool _IsNotification;

        internal Message(User author)
        {
            this._Author = author;
            this._Content = "???";
            this._TimeStamp = DateTime.Now;
            this._IsNotification = false;
        }

        internal Message(User author,string content,DateTime? timestamp=null,bool isnotif=false)
        {
            this._Author = author;
            this._Content = content;
            this._TimeStamp = timestamp ?? DateTime.Now;
            this._IsNotification = isnotif;
        }

        internal User     Author         { get => this._Author;         }
        internal DateTime TimeStamp      { get => this._TimeStamp;      }
        internal string   Content        { get => this._Content;        }
        internal bool     IsNotification { get => this._IsNotification; }
    }
}
