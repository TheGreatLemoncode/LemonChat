using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd
{
    /// <summary>
    /// Enum that represent the http request type
    /// </summary>
    public enum Methods
    {
        POST,
        GET
    }

    public enum Data
    {
        FRIENDS,
        MESSAGES
    }

    public enum HandShake
    {
        CONNEXION,
        REGISTRATION
    }
}
