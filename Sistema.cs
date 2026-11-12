using System;

public class Sistema
{
    private List<Paciente> pacientes = new List<Paciente>();

    private List<Medico> medicos = new List<Medico>();

    public void CadastrarPaciente()
    {
        Console.WriteLine("Digite o nome do paciente:");
        string nome = Console.ReadLine();

        Console.WriteLine("Digite o CPF do paciente:");
        string cpf = Console.ReadLine();

        Console.WriteLine("Digite o email do paciente:");
        string email = Console.ReadLine();

        Console.WriteLine("Digite a senha do paciente:");
        string senha = Console.ReadLine();

        Console.WriteLine("Digite a idade do paciente:");
        int idade = int.Parse(Console.ReadLine());

        Console.WriteLine("Digite o endereço do paciente:");
        string endereco = Console.ReadLine();

        Paciente paciente = new Paciente(nome, cpf, email, senha, idade, endereco);
        pacientes.Add(paciente);

        Console.WriteLine("Paciente cadastrado com sucesso!");
    }
}