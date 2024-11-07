namespace CarBuilder.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int WheelId { get; set; }
        public Wheels Wheel { get; set; }
        public int TechnologyId { get; set; }
        public Technology Technology { get; set; }
        public int PaintId { get; set; }
        public PaintColor PaintColor { get; set; }
        public int InteriorId { get; set; }
        public Interior Interior { get; set; }
        public bool Complete { get; set; }
    }
}
