using BackEnd.API;
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

namespace LemonChat.View
{
    /// <summary>
    /// Logique d'interaction pour EchoView.xaml
    /// </summary>
    public partial class EchoView : UserControl
    {
        public EchoView()
        {
            InitializeComponent();
        }

        private async void connect_Click(object sender, RoutedEventArgs e)
        {
            bool status =  await API.socket_connection();
            ((Button)sender).IsEnabled = !status;
        }

        private async void sender_Click(object sender, RoutedEventArgs e)
        {
            string content = messager.Text;
            bool response = await API.send_message(content);

            if (response)
                MessageBox.Show("Message reussi");
            else
                MessageBox.Show("Echec");
        }
    }
}
