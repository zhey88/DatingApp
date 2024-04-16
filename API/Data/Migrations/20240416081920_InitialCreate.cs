using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

//When you modify your domain classes (entities), 
//you need to run migration commands to update the database schema
namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Create table inside the database
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    //AppUser entities
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true), //Id+1 each time we add a new record
                    UserName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        /// When we are removing the database, it will drop the table
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
