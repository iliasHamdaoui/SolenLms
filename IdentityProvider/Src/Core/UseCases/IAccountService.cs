using Imanys.SolenLms.IdentityProvider.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases;

public interface IAccountService
{
    Task<(bool isSuccess, User? user)> UserPasswordSignInAsync(string email, string password, bool isPersistent);
    Task<(bool isSucess, User? admin, string? error)> RegisterOrganization(string organizationName, string adminGivenName, string adminFamilyName, string adminEmail, string adminPassword);
    Task<string?> GenerateSecurityCode(string userEmail);
    Task<bool> ActivateUser(string userEmail, string securityCode);
    Task<(bool isSuccess, string? token)> GeneratePasswordResetToken(string userEmail);
    Task<(bool isSuccess, string? error)> RestUserPassword(string userEmail, string resetToken, string password);
    Task<IEnumerable<IdentityUserClaim<string>>> GetUsersClaims(List<string> usersId);
    Task<(bool isSuccess, string? error)> AddUser(string inviterId, string? givenName, string? familyName, string email, List<string> roles);
    Task<bool> CheckUserSecurityCode(string userEmail, string securityCode);
    Task<(bool isSuccess, string? error)> RegisterUser(string userEmail, string securityCode, string password, string givenName, string familyName);
    Task<(bool isSuccess, string? error)> RegenerateUserRegistrationToken(string userEmail);
    Task<User?> GetUser(string userEmail);
    Task<(bool isSuccess, string? error)> DeleteUser(User user);
}
