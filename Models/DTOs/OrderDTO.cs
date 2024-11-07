namespace CarBuilder.Models.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int WheelId { get; set; }
        public WheelsDTO Wheels { get; set; }
        public int TechnologyId { get; set; }
        public TechnologyDTO Technology { get; set; }
        public int PaintId { get; set; }
        public PaintColorDTO PaintColor { get; set; }
        public int InteriorId { get; set; }
        public InteriorDTO Interior { get; set; }
        public bool Complete { get; set; }
    }
}
