using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace LenusBookStore.Models
{
    /// <summary>
    /// Class representation of a Book with attributes for Title, Author and Price
    /// </summary>
    public class Book
    {
        public long Id { get; set; }

        public string Author { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        [Column(TypeName = "decimal(9, 2)")]
        public decimal Price { get; set; }
    }
}
