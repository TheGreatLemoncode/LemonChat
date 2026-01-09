using BackEnd.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BackEnd.Connection;

namespace BackEnd.API
{
    /// <summary>
    /// Serve as entry point to the backend. Handle communication and security protocole
    /// </summary>
    public static class API
    {
        // private random number generator use to create salts
        private static RandomNumberGenerator rng = RandomNumberGenerator.Create();
        // private instance of the connector class use to handle all communication with the server
        private static Connector Connection;

        /// <summary>
        /// Initialize all component in the API class (connector etc...) 
        /// </summary>
        public static void Initialisation()
        {
            Connection = new Connector();
        }

        /// <summary>
        /// API method that handle the user account creation. It take the user info, hash the password and send
        /// to the server using the connector. Return true if the connection is successful and false otherwise 
        /// </summary>
        /// <param name="pMail">string that represent the user's mail</param>
        /// <param name="pDisplay">string that represent the user's display name to the other users</param>
        /// <param name="pPassword">string that represent the user's hashed password</param>
        /// <returns>Boolean that represent the connection status</returns>
        public async static Task<bool> Registration(string pMail, string pDisplay, string pPassword)
        {
            // Wrap the informations an instance of the credential class  
            Credential UserCredential = new Credential(pMail, pDisplay, pPassword, CreateSalt());
            // Send the information to the server using the connector and wait for his response
            bool response = await Connection.Authentification(UserCredential);
            // return the awaited response
            return response;
        }

        /// <summary>
        /// Private method that create a 16 bytes salt to hash password
        /// </summary>
        /// <returns>An array of bytes of size 16 </returns>
        private static byte[] CreateSalt()
        {
            byte[] buffer = new byte[16];
            rng.GetBytes(buffer);
            return buffer;
        }
    }
}
