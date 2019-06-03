
using Everest.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Everest.Identity
{
    public class PersistenceContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Authorization> Authorizations { get; set; }

        public PersistenceContext(DbContextOptions<PersistenceContext> options):base(options)
        {

        }
    }
}
