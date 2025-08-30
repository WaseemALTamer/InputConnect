using InputConnect.Structures;


namespace InputConnect.SharedData
{
    public static class IncomingConnection{

        // this class is mode only for the communcation between the
        // Connection.Mangaer and the  ConnectionReplay UI class so
        // they can share the data among one another

        public static MessageUDP? Message;

        public static PasswordKey? PasswordKey;



        public static void Clear() {
            Message = null;
            PasswordKey = null;
        }
    }
}
