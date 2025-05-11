



namespace InputConnect.Structures
{
    public class Connection{
        // connections only need to be made for commands to be sent over all the other message types
        // doesnt not need encreaption, there is more than one type of command message



        public string? DeviceName { get; set; }

        public string? MacAdress { get; set; } // this will be used to identify the Connection and the  messages they send
                                               // simply  mimicking  someone  MacAdress  isn't  going  to cut  it  as  the
                                               // application has many layers of identification and filtring attackers for
                                               // replay attack, though if they decreapt the message then change the token

        public ulong SequenceNumber { get; set; } = 0; // this is used to track command messages we using long  data type which
                                                       // gives us 19 quintillion messages before  our application  will  break
                                                       // if anyone breaks the application that way, cheers bro  never  thought
                                                       // my application will run on a quantum computer



        public string? Token { get; set; } // token to dechyper the message

    }
}