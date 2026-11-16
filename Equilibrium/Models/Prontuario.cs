public class Prontuario
{
    public int Id { get; set; }
    public Paciente Paciente { get; set; }
    public string Observacoes { get; set; }
    public string DataAtendimento { get; set; }

    public Prontuario(
        int id,
        Paciente paciente,
        string observacoes,
        string dataAtendimento)
    {
        Id = id;
        Paciente = paciente;
        Observacoes = observacoes;
        DataAtendimento = dataAtendimento;
    }
}
