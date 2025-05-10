



namespace InputConnect.Structures
{
    public class Connection{
        // connections only need to be made for commands to be sent over all the other message types
        // doesnt not need encreaption, there is more than one type of command message


        public string? MacAdress { get; set; } // this will be used to identify the Connection and the  messages they send
                                               // simply  mimicking  someone  MacAdress  isn't  going  to cut  it  as  the
                                               // application has many layers of identification and filtring attackers for
                                               // replay attack, though if they decreapt the message then change the token

        public int? SequenceNumber { get; set; } // this is used to track command messages



        public string? Token { get; set; } // token to dechyper the message

    }
}