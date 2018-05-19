using Fuse.Models;
using Fuse.Utils;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Fuse.Controls
{
    /// <summary>
    /// Interaction logic for MessageControl.xaml
    /// </summary>
    public partial class MessageControl : UserControl
    {
        private Message _Message;

        internal MessageControl(Message msg)
        {
            this._Message = msg;
            this.InitializeComponent();
        }

        internal Message Message { get => this._Message; }

        internal void Update(Message msg=null)
        {
            if (msg != null) this._Message = msg;
            msg = this._Message;
            User friend = this._Message.Author;

            string name = friend.Name;
            BitmapImage source = friend.Avatar;
            this.IMAvatar.Source = source;

            name = name.Length >= 15 ? name.Substring(0, 15) + "..." : name;
            this.TBName.Text = name;

            this.TBContent.Text = msg.Content;
            this.TBTimeStamp.Text = msg.TimeStamp.ToFriendlyFormat();
        }
    }
}
