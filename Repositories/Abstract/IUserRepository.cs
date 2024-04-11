using AdminGateway.Entities;

namespace AdminGateway.Repositories.Abstract
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(string email);
        Task CreateUserAsync(User user);
        Task<bool> IsUserExistsAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
        Task UpdateUser();
        Task<RefreshToken> ValidateRefreshToken(string token);
        Task UpdateRefreshToken(RefreshToken refreshTokens);
        Task AddRefreshToken(RefreshToken refresh);
        Task RevokeExpiredRefreshTokens(int userId);
    }
}
