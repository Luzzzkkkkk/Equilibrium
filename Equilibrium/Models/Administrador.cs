
public class Administrador : Usuarios
{
    public string NivelAcesso { get; set; }

    public Administrador(
        string nome,
        string cpf,
        string email,
        string senha,
        string nivelAcesso
    ) : base(nome, cpf, email, senha)
    {
        NivelAcesso = nivelAcesso;
    }
}
