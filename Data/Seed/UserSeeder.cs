using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TankR.Data.Enums;
using TankR.Data.Models;
using TankR.Data.Models.Identity;
using TankR.Repos.Interfaces;

namespace TankR.Data.Seed;

public static class UserSeeder
{
    public static async Task SeedDefaultUsersAsync(
        AppDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IUserRepo userRepo,
        IConfiguration config)
    {
        foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
        {
            var roleName = role.ToString();
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        var usersToSeed = new[]
        {
            new SeedUser(
                Role: UserRole.Admin,
                Email: config["Seed:Users:Admin:Email"] ?? "admin@test.com",
                Password: config["Seed:Users:Admin:Password"] ?? "Admintest123.",
                FirstName: config["Seed:Users:Admin:FirstName"] ?? "Admin",
                LastName: config["Seed:Users:Admin:LastName"] ?? "User",
                Phone: config["Seed:Users:Admin:Phone"] ?? "000000000"
            ),
            new SeedUser(
                Role: UserRole.Cashier,
                Email: config["Seed:Users:Cashier:Email"] ?? "cashier@test.com",
                Password: config["Seed:Users:Cashier:Password"] ?? "Cashiertest123.",
                FirstName: config["Seed:Users:Cashier:FirstName"] ?? "Cashier",
                LastName: config["Seed:Users:Cashier:LastName"] ?? "User",
                Phone: config["Seed:Users:Cashier:Phone"] ?? "000000001"
            ),
            new SeedUser(
                Role: UserRole.Cashier,
                Email: config["Seed:Users:Cashier2:Email"] ?? "cashier2@test.com",
                Password: config["Seed:Users:Cashier2:Password"] ?? "Cashiertest123.",
                FirstName: config["Seed:Users:Cashier2:FirstName"] ?? "Cashier2",
                LastName: config["Seed:Users:Cashier2:LastName"] ?? "User",
                Phone: config["Seed:Users:Cashier2:Phone"] ?? "000000010"
            ),
            new SeedUser(
                Role: UserRole.Cashier,
                Email: config["Seed:Users:Cashier3:Email"] ?? "cashier3@test.com",
                Password: config["Seed:Users:Cashier3:Password"] ?? "Cashiertest123.",
                FirstName: config["Seed:Users:Cashier3:FirstName"] ?? "Cashier3",
                LastName: config["Seed:Users:Cashier3:LastName"] ?? "User",
                Phone: config["Seed:Users:Cashier3:Phone"] ?? "000000011"
            ),
            new SeedUser(
                Role: UserRole.Customer,
                Email: config["Seed:Users:Customer:Email"] ?? "customer@test.com",
                Password: config["Seed:Users:Customer:Password"] ?? "Customertest123.",
                FirstName: config["Seed:Users:Customer:FirstName"] ?? "Customer",
                LastName: config["Seed:Users:Customer:LastName"] ?? "User",
                Phone: config["Seed:Users:Customer:Phone"] ?? "000000002"
            ),
            new SeedUser(
                Role: UserRole.Customer,
                Email: "john.doe@test.com",
                Password: "Customertest123.",
                FirstName: "John",
                LastName: "Doe",
                Phone: "555-0001"
            ),
            new SeedUser(
                Role: UserRole.Customer,
                Email: "jane.smith@test.com",
                Password: "Customertest123.",
                FirstName: "Jane",
                LastName: "Smith",
                Phone: "555-0002"
            ),
            new SeedUser(
                Role: UserRole.Customer,
                Email: "bob.wilson@test.com",
                Password: "Customertest123.",
                FirstName: "Bob",
                LastName: "Wilson",
                Phone: "555-0003"
            ),
            new SeedUser(
                Role: UserRole.Customer,
                Email: "alice.johnson@test.com",
                Password: "Customertest123.",
                FirstName: "Alice",
                LastName: "Johnson",
                Phone: "555-0004"
            ),
        };

        foreach (var su in usersToSeed)
        {
            await EnsureUserAsync(db, userManager, userRepo, su);
            await EnsureRoleAsync(userManager, su.Email, su.Role.ToString());
        }
    }

    private static async Task EnsureUserAsync(
        AppDbContext db,
        UserManager<ApplicationUser> userManager,
        IUserRepo userRepo,
        SeedUser su)
    {
        var identityUser = await userManager.FindByEmailAsync(su.Email);
        if (identityUser == null)
        {
            identityUser = new ApplicationUser
            {
                UserName = su.Email,
                Email = su.Email
            };

            var result = await userManager.CreateAsync(identityUser, su.Password);
            if (!result.Succeeded)
                throw new Exception($"Seed identity user failed ({su.Email}): " +
                                    string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var domainExists = await db.Users.AnyAsync(u => u.IdentityUserId == identityUser.Id);
        if (domainExists) return;

        var domainUser = new User
        {
            IdentityUserId = identityUser.Id,
            IdentityUser = identityUser,
            FirstName = su.FirstName,
            LastName = su.LastName,
            Email = su.Email,
            PhoneNumber = su.Phone,
            Role = su.Role
        };

        await userRepo.Add(domainUser);
    }

    private static async Task EnsureRoleAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string roleName)
    {
        var identityUser = await userManager.FindByEmailAsync(email);
        if (identityUser == null) return;

        if (!await userManager.IsInRoleAsync(identityUser, roleName))
            await userManager.AddToRoleAsync(identityUser, roleName);
    }

    private readonly record struct SeedUser(
        UserRole Role,
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string Phone
    );
}
