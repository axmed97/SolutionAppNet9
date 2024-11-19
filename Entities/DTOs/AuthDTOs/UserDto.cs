namespace Entities.DTOs.AuthDTOs;

public record UserDto
{
    public string Id { get; init; }
    public string Email { get; init; }
    public string Username { get; init; }
}