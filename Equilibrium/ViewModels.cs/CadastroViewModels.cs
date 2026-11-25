using System.ComponentModel.DataAnnotations;

namespace Equilibrium.ViewModels;

public class CadastroViewModel
{
    [Required]
    public string Nome { get; set; } = "";

    public string? CPF { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    [MinLength(8)]
    public string Senha { get; set; } = "";

    [Required]
    [Compare(nameof(Senha))]
    public string ConfirmarSenha { get; set; } = "";

    [Required]
    public int Idade { get; set; }

    public string Telefone { get; set; } = "";

    public string Endereco { get; set; } = "";
}