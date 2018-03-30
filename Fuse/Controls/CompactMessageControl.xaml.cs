using Fuse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fuse.Controls
{
    /// <summary>
    /// Interaction logic for CompactMessage.xaml
    /// </summary>
    public partial class CompactMessageControl : UserControl
    {
        private Message _Message;

        internal CompactMessageControl(Message msg)
        {
            this._Message = msg;
            this.InitializeComponent();
        }

        internal Message Message { get => this._Message; }

        internal void Update(Message msg = null)
        {
            if (msg != null) this._Message = msg;
            msg = this._Message;

            this.TBContent.Text = msg.Content;
        }
    }
}
