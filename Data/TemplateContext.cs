using _5._1_Desafio_do_curso.Entities;
using Microsoft.EntityFrameworkCore;


namespace _5._1_Desafio_do_curso.Data
{
    public class TemplateContext : DbContext
    {
        public TemplateContext(DbContextOptions<TemplateContext> options) : base(options)
        {

        }

        public DbSet<ContaPF> ContaPF { get; set; }

    }
}
