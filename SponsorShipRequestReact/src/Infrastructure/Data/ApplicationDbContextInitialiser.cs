using TechAssessment.Domain.Constants;
using TechAssessment.Domain.Entities;
using TechAssessment.Domain.ValueObjects;
using TechAssessment.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TechAssessment.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole(Roles.Administrator);
        var managerRole = new IdentityRole(Roles.Manager);
        var financeRole = new IdentityRole(Roles.FinanceAdmin);
        var requestorRole = new IdentityRole(Roles.Requestor);
        var systemAdminRole = new IdentityRole(Roles.SystemAdmin);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }
        if (_roleManager.Roles.All(r => r.Name != managerRole.Name))
        {
            await _roleManager.CreateAsync(managerRole);
        }
        if (_roleManager.Roles.All(r => r.Name != financeRole.Name))
        {
            await _roleManager.CreateAsync(financeRole);
        }
        if (_roleManager.Roles.All(r => r.Name != requestorRole.Name))
        {
            await _roleManager.CreateAsync(requestorRole);
        }
        if (_roleManager.Roles.All(r => r.Name != systemAdminRole.Name))
        {
            await _roleManager.CreateAsync(systemAdminRole);
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "admin@localhost", Email = "admin@localhost" };
        var manager = new ApplicationUser { UserName = "manager@localhost", Email = "manager@localhost" };
        var finance = new ApplicationUser { UserName = "finance@localhost", Email = "finance@localhost" };
        var requestor = new ApplicationUser { UserName = "requestor@localhost", Email = "requestor@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            await _userManager.AddToRolesAsync(administrator, new[] { Roles.Administrator, Roles.SystemAdmin });
        }

        if (_userManager.Users.All(u => u.UserName != manager.UserName))
        {
            await _userManager.CreateAsync(manager, "Manager1!");
            await _userManager.AddToRoleAsync(manager, Roles.Manager);
        }

        if (_userManager.Users.All(u => u.UserName != finance.UserName))
        {
            await _userManager.CreateAsync(finance, "Finance1!");
            await _userManager.AddToRoleAsync(finance, Roles.FinanceAdmin);
        }

        if (_userManager.Users.All(u => u.UserName != requestor.UserName))
        {
            await _userManager.CreateAsync(requestor, "Requestor1!");
            await _userManager.AddToRoleAsync(requestor, Roles.Requestor);
        }
    }
}
