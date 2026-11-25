using Equilibrium.Models;
using Microsoft.EntityFrameworkCore;

namespace Equilibrium.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuarios> Usuarios => Set<Usuarios>();
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Medico> Medicos => Set<Medico>();
    public DbSet<Prontuario> Prontuarios => Set<Prontuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("id");
            entity.Property(u => u.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();
            entity.Property(u => u.CPF).HasColumnName("cpf").HasMaxLength(14);
            entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
            entity.Property(u => u.Senha).HasColumnName("senha").HasMaxLength(255).IsRequired();
            entity.Property(u => u.Tipo).HasColumnName("tipo").HasMaxLength(10).IsRequired();
            entity.HasIndex(u => u.CPF).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Herança TPT: os dados comuns ficam em usuarios; os específicos,
        // em pacientes/medicos, ligados pela coluna usuario_id.
        modelBuilder.Entity<Paciente>().ToTable("pacientes");
        modelBuilder.Entity<Paciente>().Property(p => p.Id).HasColumnName("usuario_id");
        modelBuilder.Entity<Paciente>().Property(p => p.Idade).HasColumnName("idade");
        modelBuilder.Entity<Paciente>().Property(p => p.Telefone).HasColumnName("telefone").HasMaxLength(20);
        modelBuilder.Entity<Paciente>().Property(p => p.Endereco).HasColumnName("endereco").HasMaxLength(200);

        modelBuilder.Entity<Medico>().ToTable("medicos");
        modelBuilder.Entity<Medico>().Property(m => m.Id).HasColumnName("usuario_id");
        modelBuilder.Entity<Medico>().Property(m => m.CRM).HasColumnName("crm").HasMaxLength(30).IsRequired();
        modelBuilder.Entity<Medico>().Property(m => m.Especialidade).HasColumnName("especialidade").HasMaxLength(100).IsRequired();
        modelBuilder.Entity<Medico>().HasIndex(m => m.CRM).IsUnique();

        modelBuilder.Entity<Prontuario>(entity =>
        {
            entity.ToTable("prontuarios");
            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.PacienteId).HasColumnName("paciente_id");
            entity.Property(p => p.Observacoes).HasColumnName("observacoes").HasColumnType("text").IsRequired();
            entity.Property(p => p.DataAtendimento).HasColumnName("data_atendimento").HasColumnType("date");
            entity.HasOne(p => p.Paciente).WithMany(p => p.Prontuarios)
                .HasForeignKey(p => p.PacienteId).HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
