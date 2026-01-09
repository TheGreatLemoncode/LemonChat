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
using BackEnd.API;

namespace LemonChat
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            API.Initialisation();
        }

        private async void btn_sumit_Click(object sender, RoutedEventArgs e)
        {
            string mail = tbx_Mail.Text;
            string password = tbx_pword.Text;
            string display = "admin test";
            bool ServerResponse = await API.Register(mail, display, password);
            chk_test.IsChecked = ServerResponse;
        }
    }
}
