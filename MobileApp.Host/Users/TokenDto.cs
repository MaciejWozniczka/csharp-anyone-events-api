namespace AnyOneApi.Host.Users;

public class TokenDto
{
    public string AccessToken { get; set; }
    public DateTimeOffset AccessTokenExpiry { get; set; }
    public string RefreshToken { get; set; }
    public string Id { get; set; }
}