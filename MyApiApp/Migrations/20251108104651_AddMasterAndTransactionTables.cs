using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyApiApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMasterAndTransactionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "m_brand",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    brand_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_brand", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "m_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    category_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "m_channel",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    channel_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    channel_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    sub_channel_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    sub_channel_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_channel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "m_city",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    city_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    city_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    city_name_arabic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_city", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "m_payment_term",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    payment_term_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    payment_term_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    credit_days = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_payment_term", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "m_region",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    region_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    region_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_region", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "m_uom",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uom_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    uom_description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_uom", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    item_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    arabic_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sales_org_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    base_uom = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    brand_id = table.Column<int>(type: "integer", nullable: true),
                    category_id = table.Column<int>(type: "integer", nullable: true),
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
                    table.PrimaryKey("PK_t_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_item_m_brand_brand_id",
                        column: x => x.brand_id,
                        principalTable: "m_brand",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_t_item_m_category_category_id",
                        column: x => x.category_id,
                        principalTable: "m_category",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "t_customer",
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
                    region_id = table.Column<int>(type: "integer", nullable: true),
                    city_id = table.Column<int>(type: "integer", nullable: true),
                    payment_term_id = table.Column<int>(type: "integer", nullable: true),
                    channel_id = table.Column<int>(type: "integer", nullable: true),
                    price_list_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customer_group_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customer_group_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    credit_limit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    credit_days = table.Column<int>(type: "integer", nullable: true),
                    customer_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_blocked = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_customer", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_customer_m_channel_channel_id",
                        column: x => x.channel_id,
                        principalTable: "m_channel",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_t_customer_m_city_city_id",
                        column: x => x.city_id,
                        principalTable: "m_city",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_t_customer_m_payment_term_payment_term_id",
                        column: x => x.payment_term_id,
                        principalTable: "m_payment_term",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_t_customer_m_region_region_id",
                        column: x => x.region_id,
                        principalTable: "m_region",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "t_item_uom_conversion",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_id = table.Column<int>(type: "integer", nullable: false),
                    uom_id = table.Column<int>(type: "integer", nullable: false),
                    conversion_factor = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_item_uom_conversion", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_item_uom_conversion_m_uom_uom_id",
                        column: x => x.uom_id,
                        principalTable: "m_uom",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_item_uom_conversion_t_item_item_id",
                        column: x => x.item_id,
                        principalTable: "t_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_m_brand_brand_code",
                table: "m_brand",
                column: "brand_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_m_category_category_code",
                table: "m_category",
                column: "category_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_m_channel_channel_code",
                table: "m_channel",
                column: "channel_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_m_city_city_code",
                table: "m_city",
                column: "city_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_m_payment_term_payment_term_code",
                table: "m_payment_term",
                column: "payment_term_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_m_region_region_code",
                table: "m_region",
                column: "region_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_m_uom_uom_code",
                table: "m_uom",
                column: "uom_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_customer_channel_id",
                table: "t_customer",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_customer_city_id",
                table: "t_customer",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_customer_customer_code",
                table: "t_customer",
                column: "customer_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_customer_payment_term_id",
                table: "t_customer",
                column: "payment_term_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_customer_region_id",
                table: "t_customer",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_item_brand_id",
                table: "t_item",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_item_category_id",
                table: "t_item",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_item_item_code",
                table: "t_item",
                column: "item_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_item_uom_conversion_item_id_uom_id",
                table: "t_item_uom_conversion",
                columns: new[] { "item_id", "uom_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_item_uom_conversion_uom_id",
                table: "t_item_uom_conversion",
                column: "uom_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_customer");

            migrationBuilder.DropTable(
                name: "t_item_uom_conversion");

            migrationBuilder.DropTable(
                name: "m_channel");

            migrationBuilder.DropTable(
                name: "m_city");

            migrationBuilder.DropTable(
                name: "m_payment_term");

            migrationBuilder.DropTable(
                name: "m_region");

            migrationBuilder.DropTable(
                name: "m_uom");

            migrationBuilder.DropTable(
                name: "t_item");

            migrationBuilder.DropTable(
                name: "m_brand");

            migrationBuilder.DropTable(
                name: "m_category");
        }
    }
}
