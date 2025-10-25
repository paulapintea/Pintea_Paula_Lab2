using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pintea_Paula_Lab2.Data;
using Pintea_Paula_Lab2.Models;

namespace Pintea_Paula_Lab2.Pages.Books
{
    public class EditModel : BookCategoriesPageModel
    {
        private readonly Pintea_Paula_Lab2Context _context;

        public EditModel(Pintea_Paula_Lab2Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Include Author, Publisher și BookCategories
            Book = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Book == null)
            {
                return NotFound();
            }

            // Populează datele pentru checkboxuri
            PopulateAssignedCategoryData(_context, Book);

            // Populează SelectList pentru Author și Publisher
            var authorList = _context.Author.Select(a => new
            {
                a.ID,
                FullName = a.LastName + " " + a.FirstName
            });

            ViewData["AuthorID"] = new SelectList(authorList, "ID", "FullName");
            ViewData["PublisherID"] = new SelectList(_context.Publisher, "ID", "PublisherName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedCategories)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookToUpdate = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(b => b.ID == id);

            if (bookToUpdate == null)
            {
                return NotFound();
            }

            // Actualizează proprietățile Book, inclusiv AuthorID
            if (await TryUpdateModelAsync<Book>(
                bookToUpdate,
                "Book",
                b => b.Title, b => b.AuthorID, b => b.Price, b => b.PublishingDate, b => b.PublisherID))
            {
                // Actualizează categoriile selectate
                UpdateBookCategories(_context, selectedCategories, bookToUpdate);

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            // Dacă modelul nu e valid, reapelăm metodele pentru checkboxuri și dropdownuri
            UpdateBookCategories(_context, selectedCategories, bookToUpdate);
            PopulateAssignedCategoryData(_context, bookToUpdate);

            var authorList = _context.Author.Select(a => new
            {
                a.ID,
                FullName = a.LastName + " " + a.FirstName
            });

            ViewData["AuthorID"] = new SelectList(authorList, "ID", "FullName");
            ViewData["PublisherID"] = new SelectList(_context.Publisher, "ID", "PublisherName");

            return Page();
        }
    }
}
