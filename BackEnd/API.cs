using BackEnd.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.API
{
    public static class API
    {
        private static RandomNumberGenerator rng = RandomNumberGenerator.Create();
        private static Connector Connection;

        public static void Initialisation()
        {
            Connection = new Connector();
        }


        public async static Task<bool> Register(string pMail, string pDisplay, string pPassword)
        {
            Credential UserCredential = new Credential(pMail, pDisplay, pPassword, CreateSalt());
            UserCredential.RegisterEvent();
            bool response = await Connection.Authentification(UserCredential);
            return response;
        }

        private static byte[] CreateSalt()
        {
            byte[] buffer = new byte[16];
            rng.GetBytes(buffer);
            return buffer;
        }
    }
}
