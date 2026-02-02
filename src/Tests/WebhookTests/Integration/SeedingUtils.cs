using Domain.Webhooks;
using Infrastructure.Database;
using System.Security.Cryptography;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Authentication;
using System.Net.Http;
using System.Net.Http.Json;

namespace WebhookTests.Integration;

public static class SeedingUtils
{
    public static async Task InitializeDbForTests(ApplicationDbContext db, Guid userId)
    {
        db.WebhookSubscriptions.AddRange(GetSeedingWebhookSubscriptions(userId));
        await db.SaveChangesAsync();
    }

    public static async Task ReinitializeDbForTests(ApplicationDbContext db, Guid userId)
    {
        db.WebhookSubscriptions.RemoveRange(db.WebhookSubscriptions);
        await db.SaveChangesAsync();
        await InitializeDbForTests(db, userId);
    }

    public static async Task AddUserToDb(ApplicationDbContext db, User user)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }

    public static List<WebhookSubscription> GetSeedingWebhookSubscriptions(Guid userId)
    {
        return new List<WebhookSubscription>()
        {
            new WebhookSubscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Url = "https://example.com/webhook",
                EventType = "test",
                IsActive = true,
                Secret = GenerateSecret(),
                CreatedAt = DateTime.UtcNow
            },
            new WebhookSubscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Url = "https://example.com/webhook2",
                EventType = "test2",
                IsActive = true,
                Secret = GenerateSecret(),
                CreatedAt = DateTime.UtcNow
            },
            new WebhookSubscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Url = "https://example.com/webhook3",
                EventType = "test3",
                IsActive = true,
                Secret = GenerateSecret(),
                CreatedAt = DateTime.UtcNow
            }
        };
    }

    public static User SeedUser(ApplicationDbContext context, IPasswordHasher passwordHasher, string Email, 
    string Firstname, string LastName, string Password, UserRole Role)
    {

        var user = context.Users.Add(new User
        {
            Email = Email,
            FirstName = Firstname,
            LastName = LastName,
            PasswordHash = passwordHasher.Hash(Password),
            Role = Role
        });

        context.SaveChanges();
        return user.Entity;
    }

    public static async Task<WebhookSubscription> SeedSubscription(ApplicationDbContext context, WebhookSubscription subscription)
    {
        await context.AddAsync(subscription);
        await context.SaveChangesAsync();
        return subscription;
    }

    public static async Task<string> LoginUser(HttpClient client, string email, string password)
    {
        var loginRequest = new
        {
            Email = email,
            Password = password
        };
        HttpResponseMessage response = await client.PostAsJsonAsync("users/login", loginRequest);
        response.EnsureSuccessStatusCode();

        // Deserializar el token como JSON string (ASP.NET Core serializa strings con comillas)
        string? token = await response.Content.ReadFromJsonAsync<string>();
        
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Failed to retrieve token from login response");
        }

        return token;
    }

    private static string GenerateSecret()
    {
        byte[] bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}