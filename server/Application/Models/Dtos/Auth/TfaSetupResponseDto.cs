namespace Application.Models.Dtos.Auth;

public class TfaSetupResponseDto
{
    public byte[] QrCodeImage { get; set; } = null!;
}