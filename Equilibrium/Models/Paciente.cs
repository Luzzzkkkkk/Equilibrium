namespace Equilibrium.Models;

public class Paciente : Usuarios
{
    public int Idade { get; set; }
    public string Endereco { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public ICollection<Prontuario> Prontuarios { get; set; } = new List<Prontuario>();

    // Necessário para materialização pelo Entity Framework Core.
    public Paciente() { }

    public Paciente(string nome, string cpf, string email, string senha, int idade, string telefone, string endereco)
        : base(nome, cpf, email, senha)
    {
        Tipo = "Paciente";
        Idade = idade;
        Telefone = telefone;
        Endereco = endereco;
    }
}
