namespace ReftLabsTask.Internal.Infrastructure.Models;

public class ApiUserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string First_Name { get; set; } = string.Empty;
    public string Last_Name { get; set; } = string.Empty;
}
