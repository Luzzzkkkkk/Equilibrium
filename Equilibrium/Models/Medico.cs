namespace Equilibrium.Models;

public class Medico : Usuarios
{
    public string CRM { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;

    // Necessário para materialização pelo Entity Framework Core.
    public Medico() { }

    public Medico(string nome, string cpf, string email, string senha, string crm, string especialidade)
        : base(nome, cpf, email, senha)
    {
        CRM = crm;
        Especialidade = especialidade;
    }
}
