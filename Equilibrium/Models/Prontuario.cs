namespace Equilibrium.Models;

public class Prontuario
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public Paciente Paciente { get; set; } = null!;
    public string Observacoes { get; set; } = string.Empty;
    public DateTime DataAtendimento { get; set; }

    public Prontuario() { }

    public Prontuario(
        Paciente paciente, 
        string observacoes, 
        DateTime dataAtendimento
    )

    {
        Paciente = paciente ?? throw new ArgumentNullException(nameof(paciente));
        PacienteId = paciente.Id;
        Observacoes = observacoes;
        DataAtendimento = dataAtendimento;
    }
}
