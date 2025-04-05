namespace Assignment4ChatApplication.Models

// I, Sami Nachwati, student number 000879289, certify that this material is my original work. No other person's
// work has been used without due acknowledgment and I have not made my work available to anyone else.

{
    public class Room
    {
        /* name of room */
        public string RoomName { get; set; }
        
        /* list of connection ids */
        public List<string> ConnectionIds { get; set; }
    }
}
