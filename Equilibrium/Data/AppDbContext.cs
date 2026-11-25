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
    public DbSet<Administrador> Administradores => Set<Administrador>();
    public DbSet<Prontuario> Prontuarios => Set<Prontuario>();
    public DbSet<Consulta> Consultas => Set<Consulta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // HERANÇA TPT
        modelBuilder.Entity<Usuarios>()
            .UseTptMappingStrategy()
            .ToTable("usuarios");

        modelBuilder.Entity<Paciente>().ToTable("pacientes");
        modelBuilder.Entity<Medico>().ToTable("medicos");
        modelBuilder.Entity<Administrador>().ToTable("administradores");

        // USUÁRIOS
        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Nome)
                .HasMaxLength(100).IsRequired();

            entity.Property(u => u.CPF)
                .HasMaxLength(14);

            entity.Property(u => u.Email)
                .HasMaxLength(100).IsRequired();

            entity.Property(u => u.Senha)
                .HasMaxLength(255).IsRequired();

            entity.Property(u => u.Tipo)
                .HasMaxLength(20).IsRequired();

            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.CPF).IsUnique();
        });

        // PACIENTES
        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.Property(p => p.Idade).IsRequired();
            entity.Property(p => p.Telefone).HasMaxLength(20);
            entity.Property(p => p.Endereco).HasMaxLength(200);
        });

        // MÉDICOS
        modelBuilder.Entity<Medico>(entity =>
        {
            entity.Property(m => m.CRM)
                .HasMaxLength(30).IsRequired();

            entity.Property(m => m.Especialidade)
                .HasMaxLength(100).IsRequired();

            entity.HasIndex(m => m.CRM).IsUnique();
        });

        // ADMINISTRADORES
        modelBuilder.Entity<Administrador>(entity =>
        {
            entity.Property(a => a.NivelAcesso)
                .HasMaxLength(50).IsRequired();
        });

        // PRONTUÁRIOS
        modelBuilder.Entity<Prontuario>(entity =>
        {
            entity.ToTable("prontuarios");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Observacoes)
                .HasColumnType("text").IsRequired();

            entity.Property(p => p.DataAtendimento)
                .HasColumnType("datetime").IsRequired();

            entity.HasOne(p => p.Paciente)
                .WithMany(p => p.Prontuarios)
                .HasForeignKey(p => p.PacienteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CONSULTAS
        modelBuilder.Entity<Consulta>(entity =>
        {
            entity.ToTable("consultas");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Status)
                .HasMaxLength(20).IsRequired();

            entity.Property(c => c.Observacoes)
                .HasMaxLength(500);

            entity.HasOne(c => c.Paciente)
                .WithMany()
                .HasForeignKey(c => c.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Medico)
                .WithMany()
                .HasForeignKey(c => c.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(c => new { c.MedicoId, c.DataHora })
                .IsUnique();
        });
    }
}