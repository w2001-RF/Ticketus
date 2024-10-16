using System;
using System.ComponentModel.DataAnnotations;

namespace Ticketus.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public DateTime DateJoined { get; set; } = DateTime.Now;

        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
