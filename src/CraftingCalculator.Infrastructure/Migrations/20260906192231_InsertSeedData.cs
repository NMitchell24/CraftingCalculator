using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InsertSeedData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        string ns = "CraftingCalculator.Infrastructure.Data.Seed";
        string recipeFiltersFile = "RecipeFiltersSeed.sql";
        Assembly assembly = Assembly.GetExecutingAssembly();

        using Stream stream = assembly.GetManifestResourceStream($"{ns}.{recipeFiltersFile}");
        using StreamReader reader = new(stream);
        string sql = reader.ReadToEnd();
        migrationBuilder.Sql(sql);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DELETE FROM RecipeFilters");
    }
}
