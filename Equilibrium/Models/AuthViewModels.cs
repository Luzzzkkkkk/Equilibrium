using System.ComponentModel.DataAnnotations;

namespace Equilibrium.Models;

public class CadastroViewModel
{
    [Required, StringLength(100)] public string Nome { get; set; } = string.Empty;
    [Required, RegularExpression(@"^(?:\d{11}|\d{3}\.\d{3}\.\d{3}-\d{2})$", ErrorMessage = "Informe um CPF com 11 números.")]
    public string CPF { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(100)] public string Email { get; set; } = string.Empty;
    [Required, StringLength(100, MinimumLength = 8), DataType(DataType.Password)] public string Senha { get; set; } = string.Empty;
    [Required, Compare(nameof(Senha)), DataType(DataType.Password), Display(Name = "Confirmar senha")] public string ConfirmarSenha { get; set; } = string.Empty;
    [Required, Range(1, 120, ErrorMessage = "A idade deve estar entre 1 e 120 anos.")] public int Idade { get; set; }
    [RegularExpression(@"^(?:\d{10,11}|\(\d{2}\)\s?\d{4,5}-?\d{4})$", ErrorMessage = "Informe um telefone com DDD e 10 ou 11 números.")]
    public string? Telefone { get; set; }
    [StringLength(200)] public string? Endereco { get; set; }
}

public class CadastroProfissionalViewModel
{
    [Required, StringLength(100)] public string Nome { get; set; } = string.Empty;
    [Required, RegularExpression(@"^(?:\d{11}|\d{3}\.\d{3}\.\d{3}-\d{2})$", ErrorMessage = "Informe um CPF com 11 números.")]
    public string CPF { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(100)] public string Email { get; set; } = string.Empty;
    [Required, StringLength(100, MinimumLength = 8), DataType(DataType.Password)] public string Senha { get; set; } = string.Empty;
    [Required, StringLength(30)] public string CRM { get; set; } = string.Empty;
    [Required, StringLength(100)] public string Especialidade { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, DataType(DataType.Password)] public string Senha { get; set; } = string.Empty;
}

public class SolicitarRedefinicaoViewModel
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
}

public class RedefinirSenhaViewModel
{
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string Token { get; set; } = string.Empty;
    [Required, StringLength(100, MinimumLength = 8), DataType(DataType.Password)] public string NovaSenha { get; set; } = string.Empty;
    [Required, Compare(nameof(NovaSenha)), DataType(DataType.Password), Display(Name = "Confirmar senha")] public string ConfirmarSenha { get; set; } = string.Empty;
}
