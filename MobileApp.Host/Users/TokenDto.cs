namespace AnyOneApi.Host.Users;

public class TokenDto
{
    /// <summary>Token dostępu</summary>
    public string AccessToken { get; set; }
    /// <summary>Data wygaśnięcia tokenu dostępu</summary>
    public DateTimeOffset AccessTokenExpiry { get; set; }
    /// <summary>Token odświeżania</summary>
    public string RefreshToken { get; set; }
    /// <summary>ID użytkownika</summary>
    public string Id { get; set; }
}