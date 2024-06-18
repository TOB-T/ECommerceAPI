using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProductNavigationToCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSavedForLater",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "IsSavedForLater",
                table: "Carts");
        }
    }
}
