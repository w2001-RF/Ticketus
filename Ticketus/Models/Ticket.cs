using System;
using System.ComponentModel.DataAnnotations;

namespace Ticketus.Models
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }
    }
}
