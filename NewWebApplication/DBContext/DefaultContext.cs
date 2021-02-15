using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewWebApplication.Models;

namespace NewWebApplication.DBContext
{
    public class DefaultDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
        { }

        public DbSet<CustomerModel> Customer { get; set; }
    }
}
