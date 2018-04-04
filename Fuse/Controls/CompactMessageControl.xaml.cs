using Fuse.Models;
using System.Windows.Controls;

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
