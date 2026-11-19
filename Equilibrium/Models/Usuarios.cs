namespace Equilibrium.Models;

public abstract class Usuarios
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;

    protected Usuarios() { }

    protected Usuarios(string nome, string cpf, string email, string senha)
    {
        Nome = nome;
        CPF = cpf;
        Email = email;
        Senha = senha;
    }
}
