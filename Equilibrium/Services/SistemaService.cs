
using System;
using System.Collections.Generic;
using Equilibrium.Models;

public class SistemaService
{
    private List<Paciente> pacientes = new List<Paciente>();
    private List<Medico> medicos = new List<Medico>();

    public SistemaService()
    {
        // Paciente de exemplo
        Paciente paciente = new Paciente(
            "Maria",
            "12345678900",
            "maria@email.com",
            "senha123",
            30,
            "1234567890",
            "Rua Exemplo, 123"
        );

        pacientes.Add(paciente);
    }


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

    public void login()
    {
        Console.WriteLine("\nLOGIN");

        Console.Write("Email: ");
        string email = Console.ReadLine()!;

        Console.Write("Senha: ");
        string senha = Console.ReadLine()!;

        //Verifica se é paciente
        Paciente? paciente = pacientes.Find(p => p.Email == email && p.Senha == senha);

        if (paciente != null)
        {
            Console.WriteLine("\nBem-vindo(a), " + paciente.Nome);
            MenuPaciente(paciente);
            return;
        }

        //Verifica se é médico
        Medico? medico = medicos.Find(m => m.Email == email && m.Senha == senha);

        if (medico != null)
        {
            Console.WriteLine("\nBem-vindo(a), " + medico.Nome);
            MenuMedico(medico);
            return;
        }

        Console.WriteLine("Email ou senha incorretos!");
    }

    public void MenuPaciente(Paciente paciente)
    {
        int opcao;

        do
        {
            Console.WriteLine("\n========= MENU PACIENTE =========");
            Console.WriteLine("1 - Meu perfil");
            Console.WriteLine("2 - Minhas consultas");
            Console.WriteLine("3 - Meu historico");
            Console.WriteLine("4 - Aendar consulta");
            Console.WriteLine("5 - Sair");

            Console.Write("\nEscolha uma opção: ");

            if (!int.TryParse(Console.ReadLine(), out opcao))
                opcao = 0;

            switch (opcao)
            {
                case 1:
                    Console.WriteLine("Nome: " + paciente.Nome);
                    Console.WriteLine("CPF: " + paciente.CPF);
                    Console.WriteLine("Email: " + paciente.Email);
                    Console.WriteLine("Idade: " + paciente.Idade);
                    Console.WriteLine("Telefone: " + paciente.Telefone);
                    Console.WriteLine("Endereço: " + paciente.Endereco);
                    break;

                case 2:
                    Console.WriteLine("Área de consultas.");
                    break;

                case 3:
                    Console.WriteLine("Área de histórico.");
                    break;

                case 4:
                    Console.WriteLine("Área de agendamento de consultas.");
                    break;

                case 5:
                    Console.WriteLine("Saindo da conta...");
                    break;

                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }

        } while (opcao != 5);
    }

    public void MenuMedico(Medico medico)
    {
        int opcao;

        do
        {
            Console.WriteLine("\n========= MENU MÉDICO =========");
            Console.WriteLine("1 - Meu perfil");
            Console.WriteLine("2 - Listar pacientes");
            Console.WriteLine("3 - Consultar pacientes");
            Console.WriteLine("4 - Registrar atendimento");
            Console.WriteLine("5 - Sair");

            Console.Write("\nEscolha uma opção: ");

            if (!int.TryParse(Console.ReadLine(), out opcao))
                opcao = 0;

            switch (opcao)
            {
                case 1:
                    Console.WriteLine("Nome: " + medico.Nome);
                    Console.WriteLine("CPF: " + medico.CPF);
                    Console.WriteLine("Email: " + medico.Email);
                    Console.WriteLine("CRM: " + medico.CRM);
                    Console.WriteLine("Especialidade: " + medico.Especialidade);
                    break;

                case 2:
                    ListarPacientes();
                    break;

                case 3:
                    Console.WriteLine("Área de consulta");
                    break;

                case 4:
                    Console.WriteLine("Área de atendimento");
                    break;

                case 5:
                    Console.WriteLine("Saindo da conta...");
                    break;

                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }

        } while (opcao != 5);
    }

    public void ListarPacientes()
    {
        Console.WriteLine("\n=========== PACIENTES CADASTRADOS ===========");

        if (pacientes.Count == 0)
        {
            Console.WriteLine("Nenhum paciente cadastrado!");
            return;
        }

        foreach (Paciente paciente in pacientes)
        {
            Console.WriteLine("Nome: " + paciente.Nome);
            Console.WriteLine("CPF: " + paciente.CPF);
            Console.WriteLine("Email: " + paciente.Email);
            Console.WriteLine("Idade: " + paciente.Idade);
            Console.WriteLine("Telefone: " + paciente.Telefone);
            Console.WriteLine("Endereço: " + paciente.Endereco);
            Console.WriteLine("------------------------------");
        }
    }
}



