using Locadora_JEM_20.Models;
using Microsoft.EntityFrameworkCore;

namespace Locadora_JEM_20.Data;

public class AppDataContext : DbContext
{
      public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) 
      {
            Filmes = Set<Filme>();
            Clientes = Set<Cliente>();
            Locacoes = Set<Locacao>();
      }
      

      //Classes que vão virar tabelas no banco de dados
      public DbSet<Filme> Filmes { get; set; }
      public DbSet<Cliente> Clientes { get; set; }
      public DbSet<Locacao> Locacoes { get; set; }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
            modelBuilder.Entity<Locacao>()
                  .HasOne(l => l.Filme)
                  .WithMany()
                  .HasForeignKey(l => l.FilmeId);
      }
      
}
