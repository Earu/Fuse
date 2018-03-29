using Fuse.Models;
using SteamKit2;
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
    /// Interaction logic for MessageControl.xaml
    /// </summary>
    public partial class MessageControl : UserControl
    {
        public MessageControl()
        {
            this.InitializeComponent();
        }

        internal void Update(Friend friend,Message msg)
        {
            string name = friend.Name;
            BitmapImage source = friend.Avatar;
            this.IMAvatar.Source = source;

            name = name.Length >= 15 ? name.Substring(0, 15) + "..." : name;
            this.TBName.Text = name;

            this.TBContent.Text = msg.Content;
            this.TBTimeStamp.Text = msg.TimeStamp.ToString();
        }
    }
}
