using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemoveSeededAllCategory : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Clearing the reference explicitly rather than leaving it to the FK's SetNull, which only
        // fires when PRAGMA foreign_keys is on for the connection the migration happens to run on.
        // The category dropdown never offered the seeded row, so only a hand-edited or imported
        // database can have a blueprint pointing at it.
        migrationBuilder.Sql("UPDATE Blueprints SET CategoryId = NULL WHERE CategoryId = 1;");
        migrationBuilder.Sql("DELETE FROM Categories WHERE Id = 1;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("INSERT INTO Categories (Id, Name, Description) VALUES (1, 'All', '');");
    }
}
