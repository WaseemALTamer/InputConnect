





namespace InputConnect.Network.Constants
{
    // this file will contain all the constant values for the network
    public static class MessageTypes{

        public static string Advertisement = "Advertisement"; // addvertisement is simply used to braodcast that you are looking for incoming connections
                                                              // it also helps other devices discover you on there application if they are lessining

        public static string Connect = "Connect"; // this message type is to intaillise a connections and connected both devices togather
                                                  // it is only used when connecting to another device

        public static string Disconnect = "Disconnect"; // used to disconnect a connection

        public static string Accept = "Accept"; // this will be used to reply to incoming device that try to connect

        public static string Decline = "Decline";  // this will be used to reply to incoming device that try to connect

        public static string Command = "Command"; // this message is a command messge which tells the other devices to excuate a mouse momvent, keyboard
                                                  // keypress and more, this need to be encreapted and each message needs  a special  pattern it is only
                                                  // used after astaplishing a secur UDP protocol


        public static string IntialDataRequest = "IntialDataRequest"; // this will request the intial data
        public static string IntialData = "IntialData"; // this message will be used to grap data like screens and other specific data
    }
}
