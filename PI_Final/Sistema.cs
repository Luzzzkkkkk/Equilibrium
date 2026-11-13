
using System;
using System.Collections.Generic;

public class Sistema
{
    private List<Paciente> pacientes = new List<Paciente>();
    private List<Medico> medicos = new List<Medico>();

    public void CadastrarPaciente()
    {
        Console.WriteLine("\nCADASTRO DE PACIENTE");

        Console.Write("Nome: ");
        string nome = Console.ReadLine()!;

        Console.Write("CPF: ");
        string cpf = Console.ReadLine()!;

        Console.Write("Email: ");
        string email = Console.ReadLine()!;

        Console.Write("Senha: ");
        string senha = Console.ReadLine()!;

        Console.Write("Idade: ");
        int idade = int.Parse(Console.ReadLine()!);

        Console.Write("Telefone: ");
        string telefone = Console.ReadLine()!;

        Console.Write("Telefone: ");
        string endereco = Console.ReadLine()!;

        Paciente paciente = new Paciente(
            nome, cpf, email, senha, idade, telefone, endereco
        );

        pacientes.Add(paciente);

        Console.WriteLine("Paciente cadastrado!");
    }

    public void CadastrarMedico()
    {
        Console.WriteLine("\nCADASTRO DE MÉDICO");

        Console.Write("Nome: ");
        string nome = Console.ReadLine()!;

        Console.Write("CPF: ");
        string cpf = Console.ReadLine()!;

        Console.Write("Email: ");
        string email = Console.ReadLine()!;

        Console.Write("Senha: ");
        string senha = Console.ReadLine()!;

        Console.Write("CRM: ");
        string crm = Console.ReadLine()!;

        Console.Write("Especialidade: ");
        string especialidade = Console.ReadLine()!;

        Medico medico = new Medico(
            nome, cpf, email, senha, crm, especialidade
        );

        medicos.Add(medico);

        Console.WriteLine("Médico cadastrado!");
    }
}
