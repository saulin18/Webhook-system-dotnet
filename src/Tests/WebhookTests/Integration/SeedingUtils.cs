using Domain.Webhooks;
using Infrastructure.Database;
using System.Security.Cryptography;
using Domain.Users;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Abstractions.Authentication;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WebhookTests.Integration;

public static class SeedingUtils
{

    public static List<WebhookSubscription> GetSeedingWebhookSubscriptions(Guid? userId)
    {

        Guid id = userId ?? Guid.NewGuid();

        return
        [
            new WebhookSubscription
            {
                Id = Guid.NewGuid(),
                UserId = id,
                Url = "https://example.com/webhook",
                EventType = "test",
                IsActive = true,
                Secret = GenerateSecret(),
                CreatedAt = DateTime.UtcNow
            },
            new WebhookSubscription
            {
                Id = Guid.NewGuid(),
                UserId = id,
                Url = "https://example.com/webhook2",
                EventType = "test2",
                IsActive = true,
                Secret = GenerateSecret(),
                CreatedAt = DateTime.UtcNow
            },
            new WebhookSubscription
            {
                Id = Guid.NewGuid(),
                UserId = id,
                Url = "https://example.com/webhook3",
                EventType = "test3",
                IsActive = true,
                Secret = GenerateSecret(),
                CreatedAt = DateTime.UtcNow
            }
        ];
    }

    public static User SeedUser(ApplicationDbContext context, IPasswordHasher passwordHasher, string Email,
    string FirstName, string LastName, string Password, UserRole Role)
    {

        EntityEntry<User> user = context.Users.Add(new User
        {
            Email = Email,
            FirstName = FirstName,
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


    public static async Task<Tuple<WebhookSubscription, List<WebhookDelivery>>> SeedWebhookDeliveries(ApplicationDbContext context, User user)
    {


    var subscription = new WebhookSubscription
    {
        Id = Guid.NewGuid(),
        UserId = user.Id,
        Url = "https://example.com/webhook",
        EventType = "test",
        IsActive = true,
        Secret = GenerateSecret(),
        CreatedAt = DateTime.UtcNow
    };

    await context.WebhookSubscriptions.AddAsync(subscription);
    await context.SaveChangesAsync();
    
      var deliveries = new List<WebhookDelivery>
            {
            new WebhookDelivery {
                Id = Guid.NewGuid(),
                SubscriptionId = subscription.Id,
                Payload = JsonSerializer.Serialize(new { UserId = Guid.NewGuid(), UserName = "testuser" }),
                Status = WebhookDeliveryStatus.Pending,
                CreatedAt = DateTime.UtcNow
            },
            new WebhookDelivery {
                Id = Guid.NewGuid(),
                SubscriptionId = subscription.Id,
                Payload = JsonSerializer.Serialize(new { UserId = Guid.NewGuid(), UserName = "testuser" }),
                Status = WebhookDeliveryStatus.Pending,
                CreatedAt = DateTime.UtcNow
            }
        };

    await context.WebhookDeliveries.AddRangeAsync(deliveries);
    await context.SaveChangesAsync();

        return Tuple.Create(subscription, deliveries);
    }
    private static string GenerateSecret()
    {
        byte[] bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}