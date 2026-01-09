using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Security
{
    /// <summary>
    /// Create once when the user try to login or signup. Serve as payload for the http authentification
    /// </summary>
    public class Credential
    {
        public string Mail;
        public string DisplayName;
        public string UserId;
        public string Password;
        public byte[] Salt;

        /// <summary>
        /// Initialize an empty instance of the credential class to be used as a wrapper
        /// </summary>
        public Credential()
        {
            Salt = new byte[16];
        }

        /// <summary>
        /// Initialize an instance of the credential class with all parameters except the Guid. Use for
        /// registration only.
        /// </summary>
        /// <param name="pMail">the user mail</param>
        /// <param name="pDisplayName">the user display name</param>
        /// <param name="pPassword">the user hashed password</param>
        /// <param name="pSalt">the salt used to hash the user password</param>
        public Credential(string pMail,  string pDisplayName, string pPassword, byte[] pSalt)
        {
            Mail = pMail.Trim();
            DisplayName = pDisplayName;
            Password = pPassword;
            Salt = pSalt;
            UserId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initialize an instance of the credential class with the user's mail and password. Use for
        /// authentification only.
        /// </summary>
        /// <param name="pMail"></param>
        /// <param name="pPassword"></param>
        public Credential(string pMail, string pPassword)
        {
            Mail = pMail.Trim();
            Password = pPassword;
        }
    }
}
