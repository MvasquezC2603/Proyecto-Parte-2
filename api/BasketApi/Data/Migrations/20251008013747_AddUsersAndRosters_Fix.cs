using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasketApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersAndRosters_Fix : Migration
    {
        /// <inheritdoc />  
        protected override void Up(MigrationBuilder migrationBuilder)
        {
migrationBuilder.CreateTable(
        name: "Users",
        columns: table => new
        {
            Id = table.Column<int>(type: "int", nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            Username = table.Column<string>(type: "nvarchar(256)", nullable: false),
            PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Role = table.Column<string>(type: "nvarchar(64)", nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Users", x => x.Id);
        });

    migrationBuilder.CreateIndex(
        name: "IX_Users_Username",
        table: "Users",
        column: "Username",
        unique: true);

    migrationBuilder.CreateTable(
        name: "MatchRosters",
        columns: table => new
        {
            Id = table.Column<int>(type: "int", nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            MatchId = table.Column<int>(type: "int", nullable: false),
            TeamId = table.Column<int>(type: "int", nullable: false),
            PlayerId = table.Column<int>(type: "int", nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_MatchRosters", x => x.Id);

            table.ForeignKey(
                name: "FK_MatchRosters_Matches_MatchId",
                column: x => x.MatchId,
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            table.ForeignKey(
                name: "FK_MatchRosters_Teams_TeamId",
                column: x => x.TeamId,
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            table.ForeignKey(
                name: "FK_MatchRosters_Players_PlayerId",
                column: x => x.PlayerId,
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        });

    migrationBuilder.CreateIndex(
        name: "IX_MatchRosters_MatchId",
        table: "MatchRosters",
        column: "MatchId");

    migrationBuilder.CreateIndex(
        name: "IX_MatchRosters_TeamId",
        table: "MatchRosters",
        column: "TeamId");

    migrationBuilder.CreateIndex(
        name: "IX_MatchRosters_PlayerId",
        table: "MatchRosters",
        column: "PlayerId");
}
        

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropTable(name: "MatchRosters");
    migrationBuilder.DropTable(name: "Users");
}
    }
}
