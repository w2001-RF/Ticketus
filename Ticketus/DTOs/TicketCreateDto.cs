namespace Ticketus.DTOs
{
    public class TicketCreateDto
    {
        public string Description { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
