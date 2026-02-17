using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationSystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            INSERT INTO "AspNetUsers" ("Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount")
            VALUES(1, 'bruno', 'BRUNO', 'bruno@gmail.com', 'BRUNO@GMAIL.COM', true, 'AQAAAAIAAYagAAAAEL+XL4+MwZfkzuagPZsTaujjqa63/6Cz5N5g8QydyfYOeaVVru7Lv//3Rwop0XluIw==', gen_random_uuid()::text, gen_random_uuid()::text, false, false, true, 0)
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "AspNetUsers"
                WHERE "Id" = 1;
            """);
        }
    }
}
