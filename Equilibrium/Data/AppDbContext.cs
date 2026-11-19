using Equilibrium.Models;
using Microsoft.EntityFrameworkCore;

namespace Equilibrium.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuarios> Usuarios => Set<Usuarios>();
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Medico> Medicos => Set<Medico>();
    public DbSet<Prontuario> Prontuarios => Set<Prontuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Usuarios, Paciente e Medico compartilham uma tabela com discriminador.
        modelBuilder.Entity<Usuarios>()
            .HasDiscriminator<string>("TipoUsuario")
            .HasValue<Paciente>("Paciente")
            .HasValue<Medico>("Medico");

        modelBuilder.Entity<Usuarios>().HasIndex(usuario => usuario.CPF).IsUnique();
        modelBuilder.Entity<Usuarios>().HasIndex(usuario => usuario.Email).IsUnique();

        modelBuilder.Entity<Prontuario>()
            .HasOne(prontuario => prontuario.Paciente)
            .WithMany(paciente => paciente.Prontuarios)
            .HasForeignKey(prontuario => prontuario.PacienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
