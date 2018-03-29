using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuse.Models
{
    internal class Message
    {
        private DateTime _TimeStamp;
        private string _Content;

        internal Message()
        {
            this._Content = "???";
            this._TimeStamp = DateTime.Now;
        }

        internal Message(string content)
        {
            this._Content = content;
            this._TimeStamp = DateTime.Now;
        }

        internal DateTime TimeStamp { get => this._TimeStamp; }
        internal string   Content   { get => this._Content;   }
    }
}
