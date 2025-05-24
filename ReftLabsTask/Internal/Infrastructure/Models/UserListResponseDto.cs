namespace ReftLabsTask.Internal.Infrastructure.Models;

public class UserListResponseDto
{
    public int Page { get; set; }
    public int Total_Pages { get; set; }
    public List<ApiUserDto> Data { get; set; } = new();
}
