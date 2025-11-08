using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApiApp.Migrations
{
    /// <inheritdoc />
    public partial class AddStagingAndTransactionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "log_customer_ingestion",
                columns: table => new
                {
                    log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    request_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    raw_payload = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    http_status = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    validation_details = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    process_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_log_customer_ingestion", x => x.log_id);
                });

            migrationBuilder.CreateTable(
                name: "log_item_ingestion",
                columns: table => new
                {
                    log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    request_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    raw_payload = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    http_status = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    validation_details = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    process_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_log_item_ingestion", x => x.log_id);
                });

            migrationBuilder.CreateTable(
                name: "customer_transactions",
                columns: table => new
                {
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    customer_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    transaction_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    transaction_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    transaction_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_transactions", x => x.transaction_id);
                    table.ForeignKey(
                        name: "FK_customer_transactions_log_customer_ingestion_log_id",
                        column: x => x.log_id,
                        principalTable: "log_customer_ingestion",
                        principalColumn: "log_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_transactions",
                columns: table => new
                {
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    item_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    transaction_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    transaction_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    transaction_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_transactions", x => x.transaction_id);
                    table.ForeignKey(
                        name: "FK_item_transactions_log_item_ingestion_log_id",
                        column: x => x.log_id,
                        principalTable: "log_item_ingestion",
                        principalColumn: "log_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customer_transactions_customer_code",
                table: "customer_transactions",
                column: "customer_code");

            migrationBuilder.CreateIndex(
                name: "IX_customer_transactions_log_id",
                table: "customer_transactions",
                column: "log_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_transactions_transaction_status",
                table: "customer_transactions",
                column: "transaction_status");

            migrationBuilder.CreateIndex(
                name: "IX_item_transactions_item_code",
                table: "item_transactions",
                column: "item_code");

            migrationBuilder.CreateIndex(
                name: "IX_item_transactions_log_id",
                table: "item_transactions",
                column: "log_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_transactions_transaction_status",
                table: "item_transactions",
                column: "transaction_status");

            migrationBuilder.CreateIndex(
                name: "IX_log_customer_ingestion_process_status",
                table: "log_customer_ingestion",
                column: "process_status");

            migrationBuilder.CreateIndex(
                name: "IX_log_customer_ingestion_reference_id",
                table: "log_customer_ingestion",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "IX_log_customer_ingestion_request_time",
                table: "log_customer_ingestion",
                column: "request_time");

            migrationBuilder.CreateIndex(
                name: "IX_log_customer_ingestion_status",
                table: "log_customer_ingestion",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_log_item_ingestion_process_status",
                table: "log_item_ingestion",
                column: "process_status");

            migrationBuilder.CreateIndex(
                name: "IX_log_item_ingestion_reference_id",
                table: "log_item_ingestion",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "IX_log_item_ingestion_request_time",
                table: "log_item_ingestion",
                column: "request_time");

            migrationBuilder.CreateIndex(
                name: "IX_log_item_ingestion_status",
                table: "log_item_ingestion",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_transactions");

            migrationBuilder.DropTable(
                name: "item_transactions");

            migrationBuilder.DropTable(
                name: "log_customer_ingestion");

            migrationBuilder.DropTable(
                name: "log_item_ingestion");
        }
    }
}
