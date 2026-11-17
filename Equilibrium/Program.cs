// var builder = WebApplication.CreateBuilder(args);

// // ADICIONA O MVC
// builder.Services.AddControllersWithViews();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseRouting();

// app.UseAuthorization();

// app.MapStaticAssets();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}")
//     .WithStaticAssets();


// app.Run();


using System;
class Program
{
    static void Main()
    {
        Sistema sistema = new Sistema();

        int opcao;

        do
        {
            Console.WriteLine("\n==============================");
            Console.WriteLine("     SISTEMA DE SAÚDE MENTAL");
            Console.WriteLine("==============================");

            Console.WriteLine("1 - Cadastrar paciente");
            Console.WriteLine("2 - Cadastrar médico");
            Console.WriteLine("3 - Fazer login");
            Console.WriteLine("4 - Sair");

            Console.Write("\nEscolha uma opção: ");

            if (!int.TryParse(Console.ReadLine(), out opcao))
                opcao = 0;

            switch (opcao)
            {
                case 1:
                    sistema.CadastrarPaciente();
                    break;

                case 2:
                    sistema.CadastrarMedico();
                    break;

                case 3:
                    // sistema.Login();
                    break;

                case 4:
                    Console.WriteLine("Encerrando o sistema...");
                    break;

                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }

        } while (opcao != 4);
    }
}


