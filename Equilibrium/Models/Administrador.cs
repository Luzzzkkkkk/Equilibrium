namespace Equilibrium.Models;

public class Administrador : Usuarios
{
    public string NivelAcesso { get; set; } = string.Empty;

    public Administrador() { }

    public Administrador(
        string nome,
        string? cpf,
        string email,
        string senha,
        string nivelAcesso
    ) : base(nome, cpf, email, senha)
    {
        Tipo = "Admin";
        NivelAcesso = nivelAcesso;
    }
}
