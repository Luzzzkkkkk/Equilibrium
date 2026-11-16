public class Paciente : Usuarios
{
    public string Idade { get; set; }

    public string Endereco { get; set; }

    public string Telefone { get; set; }

    public Paciente(string nome, string cpf, string email, string senha, int idade, string telefone, string endereco)
        : base(nome, cpf, email, senha)
    {
        Idade = idade.ToString();
        Telefone = telefone;
        Endereco = endereco;
    }
}