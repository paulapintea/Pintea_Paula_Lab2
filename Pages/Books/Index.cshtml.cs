using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Pintea_Paula_Lab2.Models;
using Pintea_Paula_Lab2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pintea_Paula_Lab2.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly Pintea_Paula_Lab2.Data.Pintea_Paula_Lab2Context _context;

        public IndexModel(Pintea_Paula_Lab2.Data.Pintea_Paula_Lab2Context context)
        {
            _context = context;
        }

        public IList<Book> Book { get; set; }
        public BookData BookD { get; set; }
        public int BookID { get; set; }
        public int CategoryID { get; set; }

        // 🔹 Proprietăți pentru sortare și filtrare
        public string TitleSort { get; set; }
        public string AuthorSort { get; set; }
        public string CurrentFilter { get; set; }

        public async Task OnGetAsync(int? id, int? categoryID, string sortOrder, string searchString)
        {
            BookD = new BookData();

            // inițializare sortări
            TitleSort = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            AuthorSort = sortOrder == "author" ? "author_desc" : "author";
            CurrentFilter = searchString;

            // încărcăm toate cărțile
            BookD.Books = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.BookCategories)
                    .ThenInclude(b => b.Category)
                .AsNoTracking()
                .OrderBy(b => b.Title)
                .ToListAsync();

            // filtrare după searchString
            if (!String.IsNullOrEmpty(searchString))
            {
                BookD.Books = BookD.Books.Where(s =>
                    s.Author.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    s.Author.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    s.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            // selectare carte pentru afișarea categoriilor
            if (id != null)
            {
                BookID = id.Value;
                Book book = BookD.Books
                    .Where(i => i.ID == id.Value)
                    .Single();
                BookD.Categories = book.BookCategories.Select(s => s.Category);
            }

            // sortare după coloanele specificate
            switch (sortOrder)
            {
                case "title_desc":
                    BookD.Books = BookD.Books.OrderByDescending(s => s.Title);
                    break;
                case "author_desc":
                    BookD.Books = BookD.Books.OrderByDescending(s => s.Author.FullName);
                    break;
                case "author":
                    BookD.Books = BookD.Books.OrderBy(s => s.Author.FullName);
                    break;
                default:
                    BookD.Books = BookD.Books.OrderBy(s => s.Title);
                    break;
            }
        }
    }
}
