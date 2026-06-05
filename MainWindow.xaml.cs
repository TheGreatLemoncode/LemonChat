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
using ToastNotifications;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using ToastNotifications.Lifetime;

namespace LemonChat
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Notifier _notifier;
        public MainWindow()
        {
            InitializeComponent();
            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomLeft,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(2),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(2));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
            API.Initialisation();
            API.NotificationEvent += PopUpMessage;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _notifier.Dispose();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWelcome();
        }

        private void Show_connexion_click(object sender, RoutedEventArgs e)
        {
            Display.Content = new View.Connexion();
        }

        private void Show_registration_click(object sender, RoutedEventArgs e)
        {
            Display.Content = new View.AccountCreation();
        }

        private void LoadWelcome()
        {
            MessageBox.Show(Application.Current.MainWindow, "Welcome in LemonChat", "LemonChat");
        }

        public void load_messager()
        {
            Display.Content = new View.EchoView();
        }

        /// <summary>
        /// Private event type method that is called when a new 
        /// message hit the connector from the server side
        /// </summary>
        /// <param name="sender">object that called the event</param>
        /// <param name="args">Argument used to call the  event</param>
        private void PopUpMessage(object sender, EventArgs args)
        {
            MessageBox.Show(API.GetMessageFromServer());
        }

        private bool LoadRegistration()
        {
            return true;
        }

    }
}
