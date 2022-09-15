using Microsoft.AspNetCore.Mvc;

using LenusBookStore.Models;

namespace LenusBookStore.Controllers
{
    /// <summary>
    /// Handles API operations for the book collection
    /// </summary>
    [Route("[controller]")]
    public class BooksController : Controller
    {
        // Local reference to the DbContext
        private DataContext dataContext;

        public BooksController(DataContext context) => dataContext = context;

        /// <summary>
        /// Returns a list of books. Sorted by title by default.
        /// </summary>
        /// <param name="sortBy">Sort the book list by 'title', 'author' or 'price'</param>
        /// <returns>The book list, sorted by the specified sortBy parameter. The list is sorted by 'title' by default.</returns>
        [HttpGet]
        public IActionResult GetBooks(string sortBy)
            => Ok(GetSortedBookList(sortBy ?? ""));

        /// <summary>
        /// Gets a book by id
        /// </summary>
        /// <param name="id">The id of the book to get</param>
        /// <returns>The requested book, if found, otherwise a 404 error.</returns>
        [HttpGet("{id}")]
        public IActionResult GetBook(long id)
        {
            // Attempt to get the book with the specified id
            var book = from b in dataContext.Books where b.Id == id select b;

            // If a book was found, return it, otherwise return a 404 error
            return book.Count() == 1
                ? Ok(book.FirstOrDefault())
                : NotFound("Book not found");
        }

        /// <summary>
        /// Creates a new book
        /// </summary>
        /// <param name="book">The new book to create</param>
        /// <returns>If successful, returns HTTP status 201 and the id of the newly created book.
        /// If unsuccessful, returns a 400 Bad Request error with a list of error details</returns>
        [HttpPost]
        public async Task<IActionResult> SaveBook([FromBody] Book book)
        {
            // Validate the input object
            List<string> errors = ValidateBookObject(book);

            // If any of the fields are not validated, return a Bad Request with the list of errors
            if (errors.Count > 0)
                return BadRequest(errors);

            // Ensure book id is set to zero to avoid identity insert errors (rather than potentially throwing an error which would reveal the book id field to the user)
            book.Id = 0;

            // Add the new book and update the database
            await dataContext.Books.AddAsync(book);
            await dataContext.SaveChangesAsync();

            // Return the new book's id
            return Created($"{Request.Path}/{book.Id}", book.Id);
        }

        /// <summary>
        /// Update an existing book
        /// </summary>
        /// <param name="book">Details of the book to be updated and the new values for that book</param>
        /// <returns>If successful, returns HTTP status 200 and a success message.
        /// If unsuccessful, returns a 400 Bad Request error with a list of error details</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(long id, [FromBody] Book book)
        {
            // Validate the input object
            List<string> errors = new List<string>();

            // If there are no values to update...
            if(String.IsNullOrEmpty(book.Title) && String.IsNullOrEmpty(book.Author) && book.Price == 0)
            {
                errors.Add($"Please specify the values to be updated");
            }    

            // Also check the a valid book id was specified
            var currentBookEntry = dataContext.Books.Find(id);
            if (currentBookEntry == null)
            {
                errors.Add($"Invalid book id specified for update ({id})");
            }

            // If any of the fields are not validated, return a Bad Request with the list of errors
            if (errors.Count > 0)
                return BadRequest(errors);

            // Update the fields with any new values
            if(!String.IsNullOrEmpty(book.Title))
                currentBookEntry.Title = book.Title;

            if (!String.IsNullOrEmpty(book.Author))
                currentBookEntry.Author = book.Author;

            if (book.Price != 0)
                currentBookEntry.Price = book.Price;

            // Update the book and save the changes to the database
            dataContext.Update(currentBookEntry);
            await dataContext.SaveChangesAsync();

            // Return success
            return Ok("Success");
        }

        /// <summary>
        /// Deletes a book by id
        /// </summary>
        /// <param name="id">The id of the book to delete</param>
        /// <returns>HTTP status 200 if the book is found, HTTP status 404 if the book is not found.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(long id)
        {
            // Attempt to get the book from the supplied id
            if (dataContext.Books.Find(id) == null)
                return NotFound("Book not found");

            // Remove the book and return success
            dataContext.Books.Remove(new Book() { Id = id });
            await dataContext.SaveChangesAsync();

            return Ok("Success");
        }


        /// <summary>
        /// Helper class for sort options when getting a list of books
        /// </summary>
        internal class BookSortOptions
        {
            public const string Title = "title";
            public const string Author = "author";
            public const string Price = "price";
        }

        /// <summary>
        /// Get all books in the database.
        /// </summary>
        /// <param name="sortBy">Sort the book list by 'title', 'author' or 'price'</param>
        /// <returns>The book list, sorted by the specified sortBy parameter. The list is sorted by 'title' by default.</returns>
        private IEnumerable<Book> GetSortedBookList(string sortBy) => sortBy.ToLower() switch {
            BookSortOptions.Title => from b in dataContext.Books orderby b.Title select b,
            BookSortOptions.Author => from b in dataContext.Books orderby b.Author select b,
            BookSortOptions.Price => from b in dataContext.Books orderby b.Price select b,
            _ => from b in dataContext.Books orderby b.Title select b // Sort by title by default
        };

        /// <summary>
        /// Provides basic validation for a book object to ensure all values are present
        /// </summary>
        /// <param name="book">The book obejct to be validated</param>
        /// <returns>If validation passes, then an empty list is returned.
        /// If validation fails, a list of errors is ret</returns>
        private List<string> ValidateBookObject(Book book)
        {
            List<string> errors = new List<string>();

            // Check that the 'title', 'author' and 'price' fields have values
            if (String.IsNullOrEmpty(book.Title))
                errors.Add("Title is required");

            if (String.IsNullOrEmpty(book.Author))
                errors.Add("Author is required");

            if (book.Price == 0)
                errors.Add("Price is required");

            // Return any errors that were found
            return (errors);
        }
    }
}
