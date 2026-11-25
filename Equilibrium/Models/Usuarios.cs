namespace Equilibrium.Models;

public class Usuarios
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CPF { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Tipo { get; set; } = "Paciente";

    public Usuarios() { }

    public Usuarios(string nome, string? cpf, string email, string senha)
    {
        Nome = nome;
        CPF = cpf;
        Email = email;
        Senha = senha;
    }
}
