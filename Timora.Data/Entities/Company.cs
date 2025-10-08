namespace Timora.Data.Entities
{
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
