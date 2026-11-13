public class Medico : Usuarios
{
    public string CRM { get; set; }
    public string Especialidade { get; set; }

    public Medico(string nome, string cpf, string email, string senha, string crm, string especialidade)
        : base(nome, cpf, email, senha)
    {
        CRM = crm;
        Especialidade = especialidade;
    }
}

