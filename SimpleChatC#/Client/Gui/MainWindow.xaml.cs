using SimpleChat.Gui.Connect;
using SimpleChat.Gui.Login;
using SimpleChat.Gui.Register;
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

namespace SimpleChat.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWindowController
    {
        public MainWindow()
        {
            InitializeComponent();
            GotoConnect();
        }

        public void GotoLogin()
        {
            Content.Children.Clear();
            Content.Children.Add(new LoginControl(this));
        }

        public void GotoConnect()
        {
            Content.Children.Clear();
            Content.Children.Add(new ConnectControl(this));
        }

        public void GotoStartScreen()
        {

        }

        public void GotoRegister()
        {
            Content.Children.Clear();
            Content.Children.Add(new RegisterControl(this));
        }

        public void GotoChat()
        {
            
        }
    }
}