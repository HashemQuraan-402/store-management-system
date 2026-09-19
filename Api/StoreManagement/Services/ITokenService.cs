using StoreManagement.Models.Entities;

namespace StoreManagement.Services
{
    public interface ITokenService
    {
        (string Token, DateTime Expiration) GenerateToken(User user);
    }
}
