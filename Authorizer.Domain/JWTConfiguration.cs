namespace Authorizer.Domain;

public class JWTOptions
{
    public static string Section => "JWT";
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Admin { get; set; }
}
