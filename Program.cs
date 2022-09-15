using Microsoft.EntityFrameworkCore;
using System.Text;
using LenusBookStore.Models;

var builder = WebApplication.CreateBuilder(args);

// Add the DbContext and Controller services
builder.Services.AddDbContext<DataContext>(options => {options.UseSqlServer(builder.Configuration["ConnectionStrings:LenusBooks"]);});
builder.Services.AddControllers();

var app = builder.Build();

// Map controller endpoints
app.MapControllers();

// Set up a default landing page
StringBuilder landingPage = new StringBuilder("");

landingPage.AppendLine("Books API");
landingPage.AppendLine();
landingPage.AppendLine("API that manages a collection of books in a fictional store");
landingPage.AppendLine();
landingPage.AppendLine("Use http://localhost:5001/Books/ to get a list of books (Use POST to add a new book)");
landingPage.AppendLine("Use http://localhost:5001/Books/{id} to get a specific book (Use PUT to update a book, or DELETE to remove a book)");

app.MapGet("/", () => landingPage.ToString());

// Seed the database (which only adds the initial values if needed)
var dataContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedDatabase.Seed(dataContext);

app.Run();
