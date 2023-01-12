using IdentityModel;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.IdentityProvider;
using Imanys.SolenLms.IdentityProvider.Core.Domain.Entities;
using Imanys.SolenLms.IdentityProvider.Core.UseCases;
using Imanys.SolenLms.IdentityProvider.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Imanys.SolenLms.Application.Shared.Core.Enums.UserRole;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.Services;

internal sealed class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IIntegratedEventsSender _eventsSender;
    private readonly IdentityDbContext _dbContext;
    private readonly IServer _server;
    private readonly IEmailService _emailService;

    public AccountService(UserManager<User> userManager, SignInManager<User> signInManager,
        IIntegratedEventsSender eventsSender,
        IdentityDbContext dbContext, IServer server, IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _eventsSender = eventsSender;
        _dbContext = dbContext;
        _server = server;
        _emailService = emailService;
    }


    public async Task<(bool isSuccess, User? user)> UserPasswordSignInAsync(string email, string password,
        bool isPersistent)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return (false, null);

        if (!user.Active)
            return (false, user);

        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent, false);

        if (result.Succeeded)
            return (true, user);

        return (false, null);
    }

    public async Task<(bool isSucess, User? admin, string? error)> RegisterOrganization(string organizationName,
        string adminGivenName, string adminFamilyName, string adminEmail, string adminPassword)
    {
        if (await EmailAlreadyExist(adminEmail))
            return (false, null, $"The email address {adminEmail} is already taken.");

        var organization = new Organization(Guid.NewGuid().ToString(), organizationName, DateTime.Now);
        var admin = new User(organization, adminGivenName, adminFamilyName)
        {
            UserName = adminEmail, Email = adminEmail
        };

        var result = await _userManager.CreateAsync(admin, adminPassword);
        if (!result.Succeeded)
            return (false, null, result.Errors.First().Description);

        result = await _userManager.AddClaimsAsync(admin,
            new[]
            {
                new("organizationId", organization.Id), new Claim(JwtClaimTypes.Email, adminEmail),
                new(ClaimTypes.Role, Admin)
            });

        if (!result.Succeeded)
            return (false, null, result.Errors.First().Description);

        return (true, admin, null);
    }

    public async Task<bool> ActivateUser(string userEmail, string securityCode)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return false;

        if (user.SecurityCode != securityCode)
            return false;

        if (user.SecurityCodeExpirationDate < DateTime.UtcNow)
            return false;

        user.Activate();

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return false;

        var userClaims = await _userManager.GetClaimsAsync(user);

        var userAddedEvent = new UserAdded(userClaims.First(x => x.Type == "organizationId").Value, user.Id)
        {
            GivenFamily = user.GivenName!,
            FamilyFamily = user.FamilyName!,
            Email = user.Email!,
            Roles = new[] { userClaims.First(x => x.Type == ClaimTypes.Role).Value }
        };

        await _eventsSender.SendEvent(userAddedEvent);

        return true;
    }

    public async Task<string?> GenerateSecurityCode(string userEmail)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user == null)
            return null;

        var securityCode = Convert.ToBase64String(await _userManager.CreateSecurityTokenAsync(user));
        user.SetSecurityCode(securityCode);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return null;

        return securityCode;
    }

    public async Task<(bool isSuccess, string? error)> RestUserPassword(string userEmail, string resetToken,
        string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return (false, "invalid user email");

        var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
        if (!result.Succeeded)
            return (false, result.Errors.First().Description);

        return (true, null);
    }

    public async Task<(bool isSuccess, string? token)> GeneratePasswordResetToken(string userEmail)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user == null)
            return (false, null);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        return (true, token);
    }

    public async Task<IEnumerable<IdentityUserClaim<string>>> GetUsersClaims(List<string> usersId)
    {
        return await _dbContext.UserClaims.AsNoTracking().Where(x => usersId.Contains(x.UserId)).ToListAsync();
    }


    public async Task<(bool isSuccess, string? error)> AddUser(string inviterId, string? givenName, string? familyName,
        string email, List<string> roles)
    {
        if (await EmailAlreadyExist(email))
            return (false, $"The email address {email} is already taken.");

        var inviter = await _dbContext.Users.Include(x => x.Organization).FirstOrDefaultAsync(x => x.Id == inviterId);
        if (inviter == null)
            return (false, $"user not found");

        var user = new User(inviter.Organization, givenName, familyName) { Email = email, UserName = email };

        var securityCode = Convert.ToBase64String(await _userManager.CreateSecurityTokenAsync(user));
        user.SetSecurityCode(securityCode);

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
            return (false, result.Errors.First().Description);

        var userClaims = new List<Claim>
        {
            new("organizationId", inviter.Organization.Id), new(JwtClaimTypes.Email, email)
        };

        if (roles.Contains(Admin))
            userClaims.Add(new Claim(ClaimTypes.Role, Admin));
        else
        {
            if (roles.Contains(Instructor))
                userClaims.Add(new Claim(ClaimTypes.Role, Instructor));
            else
                userClaims.Add(new Claim(ClaimTypes.Role, Learner));
        }

        await _userManager.AddClaimsAsync(user, userClaims);

        var hostAddress = _server.Features.Get<IServerAddressesFeature>()!.Addresses.First();

        var linkToRegisterUser =
            $"{hostAddress}/Account/UserRegistration?userEmail={email}&securityCode={securityCode}";

        await _emailService.SendUserInvitationToJoinOrganization(inviter.FullName!, email, inviter.Organization.Name,
            linkToRegisterUser);

        return (true, null);
    }

    public async Task<bool> CheckUserSecurityCode(string userEmail, string securityCode)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return false;

        if (user.SecurityCode != securityCode)
            return false;

        if (user.SecurityCodeExpirationDate < DateTime.UtcNow)
            return false;

        return true;
    }

    public async Task<(bool isSuccess, string? error)> RegisterUser(string userEmail, string securityCode,
        string password, string givenName, string familyName)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return (false, "Invalid user");

        if (user.SecurityCode != securityCode)
            return (false, "Invalid token");

        if (user.SecurityCodeExpirationDate < DateTime.UtcNow)
            return (false, "Invalid token");


        var result = await _userManager.AddPasswordAsync(user, password);
        if (!result.Succeeded)
            return (false, result.Errors.First().Description);

        user.Activate();
        user.UpdateFamilyName(familyName);
        user.UpdateGivenName(givenName);

        result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return (false, "Error occured while activating the user");

        var userClaims = await _userManager.GetClaimsAsync(user);

        var userAddedEvent = new UserAdded(userClaims.First(x => x.Type == "organizationId").Value, user.Id)
        {
            GivenFamily = user.GivenName!,
            FamilyFamily = user.FamilyName!,
            Email = user.Email!,
            Roles = userClaims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList()
        };

        await _eventsSender.SendEvent(userAddedEvent);

        return (true, null);
    }


    public async Task<(bool isSuccess, string? error)> RegenerateUserRegistrationToken(string userEmail)
    {
        var user = await _dbContext.Users.Include(x => x.Organization).FirstOrDefaultAsync(x => x.Email == userEmail);
        if (user == null)
            return (false, "Unknown email address");

        if (user.Active)
            return (false, "User already active");

        var securityCode = Convert.ToBase64String(await _userManager.CreateSecurityTokenAsync(user));
        user.SetSecurityCode(securityCode);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return (false, "Error occured when regenerating the code");

        var hostAddress = _server.Features.Get<IServerAddressesFeature>()!.Addresses.First();

        var linkToRegisterUser =
            $"{hostAddress}/Account/UserRegistration?userEmail={userEmail}&securityCode={securityCode}";

        await _emailService.SendUserInvitationToJoinOrganization(null, userEmail, user.Organization.Name,
            linkToRegisterUser);

        return (true, null);
    }

    public Task<User?> GetUser(string userEmail)
    {
        return _userManager.FindByEmailAsync(userEmail);
    }

    private Task<bool> EmailAlreadyExist(string userEmail)
    {
        return _dbContext.Users.IgnoreQueryFilters().AnyAsync(x => x.Email == userEmail);
    }

    public async Task<(bool isSuccess, string? error)> DeleteUser(User user)
    {
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return (false, result.Errors.First().Description);

        var userDeletedEvent = new UserDeleted(user.Id);

        await _eventsSender.SendEvent(userDeletedEvent);

        return (true, null);
    }
}