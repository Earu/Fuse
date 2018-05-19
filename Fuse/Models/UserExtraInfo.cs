using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuse.Models
{
    internal struct UserExtraInfo
    {
        private string _Country;
        private string _RealName;
        private List<string> _PreviousNames;

        internal UserExtraInfo(string country,string realname,List<string> previousnames)
        {
            this._Country = country;
            this._RealName = realname;
            this._PreviousNames = previousnames;
        }

        internal string       Country       { get => this._Country;       set => this._Country       = value; }
        internal string       RealName      { get => this._RealName;      set => this._RealName      = value; }
        internal List<string> PreviousNames { get => this._PreviousNames; set => this._PreviousNames = value; }
    }
}
