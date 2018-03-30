using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuse.Models
{
    internal class Message
    {
        private User _Author;
        private DateTime _TimeStamp;
        private string _Content;

        internal Message(User author)
        {
            this._Author = author;
            this._Content = "???";
            this._TimeStamp = DateTime.Now;
        }

        internal Message(User author,string content)
        {
            this._Author = author;
            this._Content = content;
            this._TimeStamp = DateTime.Now;
        }

        internal User   Author    { get => this._Author;    }
        internal DateTime TimeStamp { get => this._TimeStamp; }
        internal string   Content   { get => this._Content;   }
    }
}
