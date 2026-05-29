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
using ToastNotifications.Messages;

namespace LemonChat.View
{
    /// <summary>
    /// Logique d'interaction pour Connexion.xaml
    /// </summary>
    public partial class Connexion : UserControl
    {
        public Connexion()
        {
            InitializeComponent();
        }

        public void Reset_click(object sender, RoutedEventArgs e)
        {
            txtBox_mail.Text = string.Empty;
            txtBox_Password.Text = string.Empty;
            ((MainWindow)Application.Current.MainWindow)._notifier.ShowInformation("All field got reset");
        }

        public async void Connexion_click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBox_mail.Text) || string.IsNullOrEmpty(txtBox_Password.Text))
            {
                MessageBox.Show("Un des champs est vide");
            }
            else
            {
                await API.Connexion(txtBox_mail.Text, txtBox_Password.Text);
                //if (API.ClientStatus())
                //{
                //    ((MainWindow)Application.Current.MainWindow)._notifier.ShowSuccess($"Connexion réussi, Bienvenue {API._user}");
                //}
                //else
                //{
                //    ((MainWindow)Application.Current.MainWindow)._notifier.ShowError(API.GetMessageFromServer());
                //}
            }
         }
    }
}
