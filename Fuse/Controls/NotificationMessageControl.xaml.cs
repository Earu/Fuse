using Fuse.Models;
using System.Windows.Controls;

namespace Fuse.Controls
{
    /// <summary>
    /// Interaction logic for NotificationMessageControl.xaml
    /// </summary>
    public partial class NotificationMessageControl : UserControl
    {
        private Message _Message;

        internal NotificationMessageControl(Message msg)
        {
            this._Message = msg;
            this.InitializeComponent();
        }

        internal Message Message { get => this._Message; }

        internal void Update(Message msg=null)
        {
            if (msg != null) this._Message = msg;
            msg = this._Message;

            this.TBContent.Text = msg.Content;
        }
    }
}
