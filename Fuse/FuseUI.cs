using Fuse.Windows;
using System.Media;
using System.IO;
using Fuse.Properties;

namespace Fuse
{
    internal class FuseUI
    {
        private ClientWindow _Window;
        private FuseClient _Client;
        private SoundPlayer _Player;

        internal FuseUI(FuseClient client)
        {
            this._Client = client;
            this._Window = new ClientWindow(client);
            this._Player = new SoundPlayer();
        }

        internal void ShowLogin(string user=null,string pass=null)
        {
            LoginWindow win = new LoginWindow(this._Client);
            win.TBUserName.Text = user;
            win.PBPassword.Password = pass;
            win.Show();
        }

        internal void PlayStream(UnmanagedMemoryStream stream)
        {
            this._Player.Stream = stream;
            this._Player.Play();
        }

        internal void ShowException(string msg)
        {
            this.PlayStream(Resources.Error);
            ExceptionWindow ewin = new ExceptionWindow(msg);
            ewin.ShowDialog();
        }

        internal ClientWindow ClientWindow { get => _Window; }
    }
}
