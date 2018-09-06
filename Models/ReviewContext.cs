using Microsoft.EntityFrameworkCore;
 
namespace BankAccount.Models
{
    public class ReviewContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public ReviewContext(DbContextOptions<ReviewContext> options) : base(options) { }
        public DbSet<sqlUser> usertable { get; set; } //this name "usertable" is the table name in SQL, also at homecontroller
        public DbSet<Transaction> transactiontable { get; set; } 
        public DbSet<Wedding> weddingtable { get; set; } 
        public DbSet<RSVP> RSVPtable { get; set; } 
    }
}
