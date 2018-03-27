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
using System.Windows.Shapes;

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
