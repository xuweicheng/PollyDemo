namespace FunctionApi.Services
{
    public interface ITokenService
    {
        string GetToken();
        string RefreshToken();
    }
}