using System.ComponentModel.DataAnnotations;

namespace Assignment4ChatApplication.Models

// I, Sami Nachwati, student number 000879289, certify that this material is my original work. No other person's
// work has been used without due acknowledgment and I have not made my work available to anyone else.
{
    public class User
    {

        /* unique identifier of user */
        [Key]
        public int Id { get; set; }
        
        /* username of user */
        [Required]
        public string Username { get; set; }

        /* message of user */
        public string? Message { get; set; }

        /* time user sent message */
        public DateTimeOffset? MessageAt { get; set; }

        /* time user connected a room */
        public DateTimeOffset? ConnectedAt { get; set; }
        
        /* time user disconnects from a room */
        public DateTimeOffset? DisconnectedAt { get; set; }


    }
}


