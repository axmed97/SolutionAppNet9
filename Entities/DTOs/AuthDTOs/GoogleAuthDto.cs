namespace Entities.DTOs.AuthDTOs;

public record GoogleAuthDto
{
    public string IdToken { get; init; }
    public string Provider { get; init; }
}