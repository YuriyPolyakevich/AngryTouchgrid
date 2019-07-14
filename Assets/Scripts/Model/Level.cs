namespace Model
{
    public class Level
    {
        public int Id { get; set; }
        public string VisibleName { get; set; }
        public string LoadName { get; set; }
        public int Star { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; }
    }
}