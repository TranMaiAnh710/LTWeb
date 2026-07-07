using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace BTL.Models
{
    /// <summary>
    /// Code-first DbContext for tables that are NOT in Model1.edmx.
    /// Points to the same DataWeb database using a raw SQL connection string
    /// so it never touches the EDMX metadata schema.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext()
            : base("name=DataWebRaw")
        {
            Database.SetInitializer<AppDbContext>(null);
        }

        public DbSet<HopDong> HopDongs { get; set; }
        public DbSet<CaLamViec> CaLamViecs { get; set; }
        public DbSet<TinTuc> TinTucs { get; set; }
        public DbSet<LienHe> LienHes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
