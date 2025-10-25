using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pintea_Paula_Lab2.Models;

namespace Pintea_Paula_Lab2.Data
{
    public class Pintea_Paula_Lab2Context : DbContext
    {
        public Pintea_Paula_Lab2Context (DbContextOptions<Pintea_Paula_Lab2Context> options)
            : base(options)
        {
        }

        public DbSet<Pintea_Paula_Lab2.Models.Book> Book { get; set; } = default!;
        public DbSet<Pintea_Paula_Lab2.Models.Publisher> Publisher { get; set; } = default!;
        public DbSet<Pintea_Paula_Lab2.Models.Author> Author { get; set; } = default!;
        public DbSet<Pintea_Paula_Lab2.Models.Category> Category { get; set; } = default!;
    }
}
