using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Fuse.Controls
{
    internal class DropDownButtonControl : ToggleButton
    {
        internal DropDownButtonControl()
        {
            Binding binding = new Binding("Menu.IsOpen")
            {
                Source = this
            };
            this.SetBinding(IsCheckedProperty, binding);
            this.DataContextChanged += (sender, args) =>
            {
                if (this.Menu != null)
                    this.Menu.DataContext = this.DataContext;
            };
        }

        internal ContextMenu Menu
        {
            get { return (ContextMenu)this.GetValue(MenuProperty); }
            set { this.SetValue(MenuProperty, value); }
        }

        internal static readonly DependencyProperty MenuProperty = DependencyProperty.Register("Menu",
            typeof(ContextMenu), typeof(DropDownButtonControl), new UIPropertyMetadata(null, OnMenuChanged));

        private static void OnMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DropDownButtonControl ddbtn = (DropDownButtonControl)d;
            ContextMenu ctxmenu = (ContextMenu)e.NewValue;
            ctxmenu.DataContext = ddbtn.DataContext;
        }

        protected override void OnClick()
        {
            if (this.Menu != null)
            {
                this.Menu.PlacementTarget = this;
                this.Menu.Placement = PlacementMode.Bottom;
                this.Menu.IsOpen = true;
            }
        }
    }
}
