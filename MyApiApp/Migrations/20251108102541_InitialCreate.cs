using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyApiApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    customer_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    arabic_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    parent_customer_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    parent_customer_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    contact_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fax = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    address1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    address2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    address3 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    address4 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    contact_person_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    city_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    city_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    city_name_arabic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    region_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    region_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    price_list_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customer_group_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customer_group_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    credit_limit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    credit_days = table.Column<int>(type: "integer", nullable: true),
                    payment_term_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    payment_term_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customer_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    channel_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    channel_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    sub_channel_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    sub_channel_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_blocked = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    item_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    arabic_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sales_org_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    base_uom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    brand_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    brand_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    category_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    category_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    is_batch_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    business_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    business_type_description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "item_uoms",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_id = table.Column<int>(type: "integer", nullable: false),
                    uom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    conversion_factor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_uoms", x => x.id);
                    table.ForeignKey(
                        name: "FK_item_uoms_items_item_id",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customers_customer_code",
                table: "customers",
                column: "customer_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_uoms_item_id_uom",
                table: "item_uoms",
                columns: new[] { "item_id", "uom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_items_item_code",
                table: "items",
                column: "item_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "item_uoms");

            migrationBuilder.DropTable(
                name: "items");
        }
    }
}
