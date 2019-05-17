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

namespace SimpleChat.Gui.Connect
{
    /// <summary>
    /// Interaction logic for ConnectControl.xaml
    /// </summary>
    public partial class ConnectControl : UserControl
    {
        private IWindowController controller;
        public ConnectControl(IWindowController controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        private void ConnectToServer(object sender, RoutedEventArgs e)
        {
            controller.GotoLogin();
        }
    }
}