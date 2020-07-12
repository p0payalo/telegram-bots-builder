using Microsoft.EntityFrameworkCore.Migrations;

namespace WebTelegramBotsBuilder.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotResponses",
                columns: table => new
                {
                    BotResponseId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    responseType = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotResponses", x => x.BotResponseId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bots",
                columns: table => new
                {
                    TelegramBotId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    IsStarted = table.Column<bool>(nullable: false),
                    BotName = table.Column<string>(nullable: true),
                    BotToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bots", x => x.TelegramBotId);
                    table.ForeignKey(
                        name: "FK_Bots_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BotQueries",
                columns: table => new
                {
                    BotQueryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BotResponseId = table.Column<int>(nullable: false),
                    queryType = table.Column<int>(nullable: false),
                    TelegramBotId = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotQueries", x => x.BotQueryId);
                    table.ForeignKey(
                        name: "FK_BotQueries_BotResponses_BotResponseId",
                        column: x => x.BotResponseId,
                        principalTable: "BotResponses",
                        principalColumn: "BotResponseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BotQueries_Bots_TelegramBotId",
                        column: x => x.TelegramBotId,
                        principalTable: "Bots",
                        principalColumn: "TelegramBotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotQueries_BotResponseId",
                table: "BotQueries",
                column: "BotResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_BotQueries_TelegramBotId",
                table: "BotQueries",
                column: "TelegramBotId");

            migrationBuilder.CreateIndex(
                name: "IX_Bots_UserId",
                table: "Bots",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotQueries");

            migrationBuilder.DropTable(
                name: "BotResponses");

            migrationBuilder.DropTable(
                name: "Bots");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
