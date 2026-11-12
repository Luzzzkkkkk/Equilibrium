public class Paciente : Usuarios
{
    public int Idade { get; set; }

    public string Endereco { get; set; }

    public Paciente(string nome, string cpf, string email, string senha, string telefone, string endereco)
        : base(nome, cpf, email, senha)
    {
        Idade = idade;
        Endereco = endereco;
    }
}