using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using BCrypt.Net;
using Poseidon.Data;


namespace Poseidon.Endpoints
{
    public static class PasswordHashMigration
    {
        public static void MapPasswordMigration(this IEndpointRouteBuilder app)
        {
            app.MapPost("/migrate-password", async ([FromServices] PoseidonDbContext db) =>
            {
                var users = db.Users.ToList();

                foreach (var user in users)
                {
                    if (user.Password.StartsWith("$2"))
                        continue;

                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                }

                await db.SaveChangesAsync();

                return Results.Ok("Password Migration Completed");
            })
            .WithDisplayName("One time password hash migration")
            .WithTags("Migration");
        }
    }
}
