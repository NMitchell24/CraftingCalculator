using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingCalculator.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddDatasets : Migration
{
    /// <summary>
    /// Id given to the dataset every pre-existing record is filed into. Hardcoded rather than read
    /// from DatasetConstants: a migration describes a schema that has already shipped, so it must not
    /// change when a constant does.
    /// </summary>
    private const int DefaultDatasetId = 1;

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Reordered from the scaffold, which emitted the AddColumn calls before CreateTable: the
        // foreign keys below need both the table and the row they point at to already exist.
        migrationBuilder.CreateTable(
            name: "Datasets",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Datasets", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Datasets_Name",
            table: "Datasets",
            column: "Name");

        migrationBuilder.Sql($"INSERT INTO Datasets (Id, Name) VALUES ({DefaultDatasetId}, 'Default');");

        // defaultValue is the whole point of these four, and the scaffold emitted 0: it is what fills
        // the column for every row written before datasets existed, and 0 matches no Datasets row, so
        // the scaffolded value would leave an upgraded install showing nothing and failing the foreign
        // key below. It is deliberately not declared as HasDefaultValue on the entity - new records get
        // their dataset from CraftingDataContext.StampDataset, not from a store default.
        foreach (string table in new[] { "Categories", "Components", "Blueprints", "Favorites" })
        {
            migrationBuilder.AddColumn<int>(
                name: "DatasetId",
                table: table,
                type: "INTEGER",
                nullable: false,
                defaultValue: DefaultDatasetId);

            migrationBuilder.CreateIndex(
                name: $"IX_{table}_DatasetId",
                table: table,
                column: "DatasetId");

            migrationBuilder.AddForeignKey(
                name: $"FK_{table}_Datasets_DatasetId",
                table: table,
                column: "DatasetId",
                principalTable: "Datasets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (string table in new[] { "Categories", "Components", "Blueprints", "Favorites" })
        {
            migrationBuilder.DropForeignKey(name: $"FK_{table}_Datasets_DatasetId", table: table);
            migrationBuilder.DropIndex(name: $"IX_{table}_DatasetId", table: table);
            migrationBuilder.DropColumn(name: "DatasetId", table: table);
        }

        migrationBuilder.DropTable(name: "Datasets");
    }
}
