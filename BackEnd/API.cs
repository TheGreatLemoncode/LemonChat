using BackEnd.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BackEnd.Connection;
using BackEnd.User;

namespace BackEnd.API
{
    /// <summary>
    /// Serve as entry point to the backend. Handle communication and security protocole
    /// </summary>
    public static class API
    {

        // private instance of the connector class use to handle all communication with the server
        private static Connector Connection;
        // a private instance of the current connected user
        public static Person _user;
        // public delegate for the notifier
        public static event EventHandler NotificationEvent;

        /// <summary>
        /// Initialize all component in the API class (connector etc...) 
        /// </summary>
        public static void Initialisation()
        {
            Connection = new Connector();
            Connection.NewMessage += NewMessageEvent;
        }

        /// <summary>
        /// API method that handle the user account creation. It take the user information and send them
        /// to the server using the connector.
        /// </summary>
        /// <param name="pMail">string that represent the user's mail</param>
        /// <param name="pDisplay">string that represent the user's display name to the other users</param>
        /// <param name="pPassword">string that represent the user's hashed password</param>
        /// <returns>An empty task object</returns>
        public async static Task Registration(string pMail, string pDisplay, string pPassword)
        {
            // Wrap the informations in an instance of the credential class  
            Credential UserCredential = new Credential(pMail, pDisplay, pPassword);
            // Send the information to the server using the connector and wait for his response
            await Connection.Authentification(UserCredential, HandShake.REGISTRATION);
        }

        public async static Task Connexion(string pMail, string pPassword)
        {
            // We wrap the information in an instance of the credential class
            Credential UserCredential = new Credential(pMail, pPassword);
            // We send the information to the server using the connector
            await Connection.Authentification(UserCredential, HandShake.CONNEXION);
        }

        /// <summary>
        /// public static method that return the latest message from
        /// the server and shows it as notification
        /// </summary>
        /// <returns>A string message from the server</returns>
        public static string GetMessageFromServer()
        {
            return Connection.message;
        }

        public static bool ClientStatus()
        {
            return (Connection.IsAuthentified && !(_user is null));
        }

        /// <summary>
        /// A public static event type method that is raised when a new message 
        /// arrives from the server. Serves as bridge for the front and backend
        /// </summary>
        /// <param name="sender">The instance that raised this event</param>
        /// <param name="args">The argument that was use to raise the event </param>
        public static void NewMessageEvent(object sender, EventArgs args)
        {
            NotificationEvent?.Invoke(sender, args);
        }
    }
}
