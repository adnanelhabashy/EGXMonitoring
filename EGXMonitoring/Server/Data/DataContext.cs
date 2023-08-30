
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EGXMonitoring.Server.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<Widget> Widgets { get; set; }
    }
}
