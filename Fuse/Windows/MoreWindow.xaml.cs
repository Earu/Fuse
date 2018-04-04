using System.Windows;
using System.Windows.Input;

namespace Fuse.Windows
{
    /// <summary>
    /// Interaction logic for MoreWindow.xaml
    /// </summary>
    public partial class MoreWindow : Window
    {
        public MoreWindow()
        {
            this.InitializeComponent();
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            this.Close();
        }
    }
}
