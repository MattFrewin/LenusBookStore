using Microsoft.EntityFrameworkCore;

namespace LenusBookStore.Models
{
    /// <summary>
    /// Contains seed method for initial database population
    /// </summary>
    public static class SeedDatabase
    {
        /// <summary>
        /// Populates the database with initial book data.
        /// </summary>
        /// <param name="dataContext">The DbContext to use for data population</param>
        public static void Seed(DataContext dataContext)
        {
            // Ensure the database exists and is up-to-date with migrations
            dataContext.Database.Migrate();

            // If there are not books in the database...
            if(dataContext.Books.Count() == 0)
            {
                // ... create the initial book list
                dataContext.Books.AddRange(
                    new Book {
                        Author = "A. A. Milne",
                        Title = "Winnie-the-Pooh",
                        Price = 19.25M
                    },
                    new Book {
                        Author = "Jane Austen",
                        Title = "Pride and Prejudice",
                        Price = 5.49M
                    },
                    new Book {
                        Author = "William Shakespeare",
                        Title = "Romeo and Juliet",
                        Price = 6.95M
                    });

                // Save the books to the database
                dataContext.SaveChanges();
            }
        }

    }
}
