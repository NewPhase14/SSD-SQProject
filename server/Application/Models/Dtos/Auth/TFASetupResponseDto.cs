namespace Application.Models.Dtos.Auth;

public class TFASetupResponseDto
{
    public byte[] QrCodeImage { get; set; } = null!;
}