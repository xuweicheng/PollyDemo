namespace MyClaims.Web
{
    public interface ITokenService
    {
        string GetToken();
        string RefreshToken();
    }
}