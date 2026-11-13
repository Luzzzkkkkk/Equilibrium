public abstract class Usuarios
{
    public string Nome { get; set; }

    public string CPF { get; set; }

    public string Email { get; set; }

    public string Senha { get; set; }

    public Usuarios(string nome, string cpf, string email, string senha)
    {
        Nome = nome;
        CPF = cpf;
        Email = email;
        Senha = senha;
    }
}