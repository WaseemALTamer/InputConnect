

namespace InputConnect.Connections
{
    public static class Constants
    {

        public static string PassPhase = "UuU"; // this pass phase will be sent as a message with the connect message
                                                // if the other device decreaption matches this then we have the same
                                                // token and we can move forward in establishing a squre connenection
                                                // other wise a connection is responded to as invalid token


        public static string StatePending = "Pending"; // this state is when we are waiting for there acceptance
        public static string StateConnected = "Connected"; // this state is when they replay with with acceptance
        public static string StateRejection = "Rejection"; // this state is when you are rejected rarly used



        public static string Transmitter = "Transmitter";
        public static string Receiver = "Receiver";
        



    }
}
