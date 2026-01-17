using System.Diagnostics.CodeAnalysis;

namespace Timora.Data.Entities
{
    /// <summary>
    /// Represents a company in the system that can have users and documents.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Company
    {
        /// <summary>
        /// Gets or sets the unique identifier for the company.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of users belonging to this company.
        /// </summary>
        public ICollection<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// Gets or sets the collection of documents belonging to this company.
        /// </summary>
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
