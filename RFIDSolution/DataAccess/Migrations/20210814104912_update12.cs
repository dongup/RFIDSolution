using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RFIDSolution.DataAccess.Migrations
{
    public partial class update12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUserId = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUserId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CONFIG",
                columns: table => new
                {
                    CONFIG_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KEY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VALUE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IS_DELETED = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONFIG", x => x.CONFIG_ID);
                });

            migrationBuilder.CreateTable(
                name: "INVENTORY",
                columns: table => new
                {
                    IVENTORY_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IVENTORY_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IVENTORY_SEQ = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    REF_DOC_NO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    INVENTORY_NAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    INVENTORY_STATUS = table.Column<int>(type: "int", nullable: false, comment: "Trạng thái xử lý: 1 - chờ xử lý; 2 - đã hoàn thành"),
                    COMPLETE_USER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CREATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    UPDATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATETED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    DELETED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IS_DELETED = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INVENTORY", x => x.IVENTORY_ID);
                });

            migrationBuilder.CreateTable(
                name: "INVENTORY_DTL",
                columns: table => new
                {
                    DTL_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    INVENTORY_ID = table.Column<int>(type: "int", nullable: false),
                    PRODUCT_ID = table.Column<int>(type: "int", nullable: false),
                    COMPLETE_USER = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    STATUS = table.Column<int>(type: "int", nullable: false, comment: "Trạng thái xử lý: 1 - Tìm thấy; 2 - Không tìm thấy"),
                    CREATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    UPDATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATETED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    DELETED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IS_DELETED = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INVENTORY_DTL", x => x.DTL_ID);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Method = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RequestUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OperationSystemVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestIpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    RequestUserId = table.Column<int>(type: "int", nullable: true),
                    ExceptionMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptionDetail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CREATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    UPDATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATETED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    DELETED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IS_DELETED = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MODEL",
                columns: table => new
                {
                    MODEL_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MODEL_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CREATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    UPDATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATETED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    DELETED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IS_DELETED = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MODEL", x => x.MODEL_ID);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCT_ALTER",
                columns: table => new
                {
                    ALERT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PRODUCT_ID = table.Column<int>(type: "int", nullable: false),
                    EPC = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ALERT_IP = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ALERT_TIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ALERT_FREQ = table.Column<int>(type: "int", nullable: false),
                    ALERT_CONF_STATUS = table.Column<int>(type: "int", nullable: false, comment: "Trạng thái xử lý: 1 - chưa xử lý; 2 - đã xử lý"),
                    ALERT_CONF_REASON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ALERT_CONF_USER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ALERT_CONF_TIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    UPDATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATETED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    DELETED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IS_DELETED = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCT_ALTER", x => x.ALERT_ID);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCT_IO",
                columns: table => new
                {
                    IO_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IO_STATUS = table.Column<int>(type: "int", nullable: false, comment: "Trạng thái của inout: 1 - chưa trả; 2 - đã trả"),
                    IO_REASON = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    IO_DEPART = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    REF_DOC_NO = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    REF_DOC_DATE = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCT_IO", x => x.IO_ID);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCT_IO_DTL",
                columns: table => new
                {
                    IO_DTL_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IO_ID = table.Column<int>(type: "int", nullable: false),
                    IO_DTL_EXT = table.Column<int>(type: "int", nullable: false),
                    PRODUCT_ID = table.Column<int>(type: "int", nullable: false),
                    IO_GET_TIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IO_RET_TIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IO_GET_USER = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IO_RET_USER = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IO_GET_NOTE = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    IO_RET_NOTE = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    IO_GET_STATUS = table.Column<int>(type: "int", nullable: false, comment: "Trạng thái giầy lấy: 1 - Ok; 2 - không ok"),
                    IO_RET_STATUS = table.Column<int>(type: "int", nullable: false, comment: "Trạng thái giầy trả: 1 - Ok; 2 - không ok"),
                    CREATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    UPDATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATETED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    DELETED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IS_DELETED = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCT_IO_DTL", x => x.IO_DTL_ID);
                });

            migrationBuilder.CreateTable(
                name: "RFID_TAG",
                columns: table => new
                {
                    RFID_TAG_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EPC = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TAG_STATUS = table.Column<int>(type: "int", nullable: false, comment: "Trạng thái xử lý: 1 - Sẵn sàng; 2 - Bị hủy"),
                    CREATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    UPDATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATETED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    DELETED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IS_DELETED = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RFID_TAG", x => x.RFID_TAG_ID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCT",
                columns: table => new
                {
                    PRODUCT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PRODUCT_NAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PRODUCT_CODE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EPC = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MODEL_ID = table.Column<int>(type: "int", nullable: false),
                    PRODUCT_ARTICLE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PRODUCT_SIZE = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PRODUCT_CATEGORY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PRODUCT_STATUS = table.Column<int>(type: "int", nullable: false, comment: "Trạng thái của giầy: 1 - Available; 2 - NotAvailable; 3 - OnHold"),
                    SAMPLE_NO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SAMPLE_REQUEST = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SAMPLE_SIZE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PRODUCT_POC = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LR = table.Column<int>(type: "int", nullable: false, comment: "Bên của giầy: 1 - Left; 2 - Right"),
                    PRODUCT_LOCATION = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PRODUCT_REMARKS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DEV_TEAM = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DEV_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PRODUCT_SEASON = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PRODUCT_STAGE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    COLOR_NAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PRODUCT_GENDER = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    COMPLETED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PRODUCT_WHQDEVELOPER = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UPPER_MATERIAL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PRODUCT_MSMATERIAL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OUTSOLE_MATERIAL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LAST_ID = table.Column<int>(type: "int", nullable: false),
                    PRODUCT_PATTERN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PRODUCT_STLFILE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    INPUT_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    REF_DOC_NO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    REF_DOC_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PAIR_CODE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CREATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    UPDATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATETED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    DELETED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IS_DELETED = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCT", x => x.PRODUCT_ID);
                    table.ForeignKey(
                        name: "FK_PRODUCT_MODEL_MODEL_ID",
                        column: x => x.MODEL_ID,
                        principalTable: "MODEL",
                        principalColumn: "MODEL_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDeleted", "Name", "NormalizedName" },
                values: new object[] { 1, "db042179-39f0-48cc-bf74-272e9937e7a0", false, "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDeleted", "Name", "NormalizedName" },
                values: new object[] { 2, "bc60b423-498f-4ac4-ac86-5de26a369c92", false, "User", "USER" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCT_MODEL_ID",
                table: "PRODUCT",
                column: "MODEL_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CONFIG");

            migrationBuilder.DropTable(
                name: "INVENTORY");

            migrationBuilder.DropTable(
                name: "INVENTORY_DTL");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "PRODUCT");

            migrationBuilder.DropTable(
                name: "PRODUCT_ALTER");

            migrationBuilder.DropTable(
                name: "PRODUCT_IO");

            migrationBuilder.DropTable(
                name: "PRODUCT_IO_DTL");

            migrationBuilder.DropTable(
                name: "RFID_TAG");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "MODEL");
        }
    }
}
