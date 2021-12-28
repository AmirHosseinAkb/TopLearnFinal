using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TopLearn.Data.Entities.User;

namespace TopLearn.Data.Context
{
    public class TopLearnContext:DbContext
    {
        public TopLearnContext(DbContextOptions<TopLearnContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<Role>()
                .HasQueryFilter(r => !r.IsDeleted);
            base.OnModelCreating(modelBuilder);
        }
    }
}
