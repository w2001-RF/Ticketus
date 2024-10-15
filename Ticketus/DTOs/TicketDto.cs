namespace Ticketus.DTOs
{
    public class TicketDto
    {
        public int TicketId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
