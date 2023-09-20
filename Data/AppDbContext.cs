using _5._1_Desafio_do_curso.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;

namespace _5._1_Desafio_do_curso.Data
{
    public class AppDbContext : DbContext
    {
       // public DbSet<Clientes> Clientes { get; set; }
        public DbSet<ContaPF> ContaPFs { get; set; }
        public DbSet<ContaPJ> ContaPJs { get; set; }
        public DbSet<ExtratoPF> ExtratoPFs { get; set; }
        public DbSet<ExtratoPJ> ExtratoPJs { get; set; }

        public AppDbContext()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("SqlConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<ContaPF>().HasNoKey();
            modelBuilder.Entity<ContaPJ>().HasNoKey();
            //modelBuilder.Entity<ExtratoPF>().HasOne(x => x.);
            //modelBuilder.Entity<ExtratoPJ>().HasKey();
        }
    }

}
