﻿using Client;
using Newtonsoft.Json;
using Shared;
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

namespace SimpleChat.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainWindowViewModel)
            {
                (DataContext as MainWindowViewModel).Terminate();
            }
        }

        private void MessagesArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            MessagesAreaScroller.ScrollToEnd();
        }
    }
}
