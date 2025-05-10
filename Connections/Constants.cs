using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputConnect.Connections
{
    public static class Constants
    {

        public static string PassPhase = "UuU"; // this pass phase will be sent as a message with the connect message
                                                // if the other device decreaption matches this then we have the same
                                                // token and we can move forward in establishing a squre connenection
                                                // other wise a connection is responded to as invalid token
        



    }
}
