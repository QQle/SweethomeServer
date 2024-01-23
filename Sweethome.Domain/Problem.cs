namespace Sweethome.Domain
{
    public class Problem
    {
        public long Id { get; set; }
        public string Problems { get; set; }
        public string Description { get; set; }

        public DateTime DateOfsolution { get; set; }
        public string Rewiew { get; set; }

        public User User { get; set; }
        public long UserId { get; set; }
    }
}
