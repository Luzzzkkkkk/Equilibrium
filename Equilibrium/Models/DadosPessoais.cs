namespace Equilibrium.Models;

public static class DadosPessoais
{
    public static string SomenteNumeros(string? valor) =>
        new((valor ?? string.Empty).Where(char.IsDigit).ToArray());

    public static string MascararCpf(string? cpf)
    {
        var numeros = SomenteNumeros(cpf);
        return numeros.Length == 11 ? $"{numeros[..3]}.***.***-**" : "Não informado";
    }

    public static string MascararTelefone(string? telefone)
    {
        var numeros = SomenteNumeros(telefone);
        return numeros.Length is 10 or 11
            ? $"({numeros[..2]}) *****-****"
            : "Não informado";
    }
}
