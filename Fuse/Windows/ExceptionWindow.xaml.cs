using System;
using System.Windows;
using System.Windows.Input;

namespace Fuse.Windows
{
    /// <summary>
    /// Interaction logic for ExceptionWindow.xaml
    /// </summary>
    public partial class ExceptionWindow : Window
    {
        public ExceptionWindow(string msg="???")
        {
            this.InitializeComponent();
            this.TBException.Text = msg;
            this.Topmost = true;
        }

        private void OnClose(object sender,EventArgs e)
        {
            this.Close();
        }

        private void OnMouseDownDrag(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
