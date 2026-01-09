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

        public Credential()
        {
            Salt = new byte[16];
        }

        /// <summary>
        /// Initialize an instance of the credential class with all parameters except the Guid
        /// </summary>
        /// <param name="pMail"></param>
        /// <param name="pDisplayName"></param>
        /// <param name="pPassword"></param>
        /// <param name="pSalt"></param>
        public Credential(string pMail,  string pDisplayName, string pPassword, byte[] pSalt)
        {
            Mail = pMail;
            DisplayName = pDisplayName;
            Password = pPassword;
            Salt = pSalt;
        }

        public void RegisterEvent()
        {
            UserId = Guid.NewGuid().ToString();
        }
    }
}
