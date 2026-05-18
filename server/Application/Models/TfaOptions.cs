namespace Application.Models;

public sealed class TfaOptions
{
    public string Issuer { get; set; } = string.Empty;

    public int Digits { get; set; }

    public int Period { get; set; }
}