using System;
using Snebur.Core.Enums;
using Snebur.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Snebur.Persistence.Identity.Migrations;

/// <inheritdoc />
public partial class InitialMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        Guard.NotNull(migrationBuilder);    

        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:CollationDefinition:case_accent_insensitive", "und-u-ks-level1,und-u-ks-level1,icu,False")
            .Annotation("Npgsql:Enum:authentication_type", "anonymous,credentials,facebook,google,microsoft,sms,system,unknown,whats_app")
            .Annotation("Npgsql:Enum:business_type", "civil_registry_office,system,undefined")
            .Annotation("Npgsql:Enum:country", "argentina,bolivia,brazil,canada,chile,colombia,ecuador,france,germany,guyana,italy,mexico,paraguay,peru,portugal,spain,suriname,united_kingdom,united_states,unknown,uruguay,venezuela")
            .Annotation("Npgsql:Enum:culture", "de_de,en_ca,en_gb,en_gy,en_us,es_ar,es_bo,es_cl,es_co,es_ec,es_es,es_mx,es_pe,es_uy,es_ve,fr_ca,fr_fr,gn_py,it_it,nl_sr,pt_br,pt_pt,undefined")
            .Annotation("Npgsql:Enum:currency", "brl,eur,unknown,usd")
            .Annotation("Npgsql:Enum:language", "default,english,french,german,italian,latin_spanish,portuguese_brazil,portuguese_portugal,spanish")
            .Annotation("Npgsql:Enum:password_strength", "empty,medium,strong,weak")
            .Annotation("Npgsql:Enum:tenant_state", "cancelled,closed,new,onboarding,operational,system,trial,unknown")
            .Annotation("Npgsql:Enum:tenant_status", "active,archived,inactive,pending,suspended,unknown")
            .Annotation("Npgsql:Enum:tenant_type", "company,individual,system,undefined")
            .Annotation("Npgsql:Enum:user_role", "admin,anonymous,chat_agent,operator,owner,system_admin,undefined,viewer")
            .Annotation("Npgsql:Enum:user_state", "active,blocked,deleted,inactive,new,pending_verification,suspended,undefined")
            .Annotation("Npgsql:Enum:user_status", "anonymous,away,busy,do_not_disturb,new,offline,online,system,undefined")
            .Annotation("Npgsql:Enum:user_type", "admin_user,anonymous,system_user,tenant_user,undefined")
            .Annotation("Npgsql:Enum:verification_state", "not_verified,undefined,verified")
            .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

        migrationBuilder.CreateTable(
            name: "addresses",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                address_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, collation: "case_accent_insensitive"),
                street = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "case_accent_insensitive"),
                number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, collation: "case_accent_insensitive"),
                complement = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, collation: "case_accent_insensitive"),
                neighborhood = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, collation: "case_accent_insensitive"),
                city = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, collation: "case_accent_insensitive"),
                state = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false, collation: "case_accent_insensitive"),
                zip_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, collation: "case_accent_insensitive"),
                country = table.Column<Country>(type: "country", nullable: false),
                tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_default = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted_session_id = table.Column<Guid>(type: "uuid", nullable: true),
                sort_order = table.Column<double>(type: "double precision", nullable: true, defaultValueSql: "get_next_sort_order_asc('addresses', 'sort_order')"),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                last_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                created_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                last_updated_session_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_addresses", x => x.id);
                table.CheckConstraint("ck_addresses_created_session_id_not_empty", "created_session_id <> '00000000-0000-0000-0000-000000000000'::uuid");
                table.CheckConstraint("ck_addresses_id_not_empty", "id <> '00000000-0000-0000-0000-000000000000'::uuid");
                table.CheckConstraint("ck_addresses_last_updated_session_id_not_empty", "last_updated_session_id <> '00000000-0000-0000-0000-000000000000'::uuid");
                table.CheckConstraint("ck_addresses_tenant_id_not_empty", "tenant_id <> '00000000-0000-0000-0000-000000000000'::uuid");
            });

        migrationBuilder.CreateTable(
            name: "sessions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                application_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, collation: "case_accent_insensitive"),
                ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false, collation: "case_accent_insensitive"),
                user_agent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false, collation: "case_accent_insensitive"),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                is_persistent = table.Column<bool>(type: "boolean", nullable: false),
                last_activity = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                terminated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                language = table.Column<Language>(type: "language", nullable: false),
                authentication_type = table.Column<AuthenticationType>(type: "authentication_type", nullable: false),
                termination_reason = table.Column<int>(type: "integer", nullable: true),
                user_role = table.Column<UserRole>(type: "user_role", nullable: false),
                user_type = table.Column<UserType>(type: "user_type", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                GeoLocation_Latitude = table.Column<double>(type: "double precision", nullable: true),
                GeoLocation_Longitude = table.Column<double>(type: "double precision", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                last_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                created_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                last_updated_session_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_sessions", x => x.id);
                table.CheckConstraint("ck_sessions_created_session_id_not_empty", "created_session_id <> '00000000-0000-0000-0000-000000000000'::uuid");
                table.CheckConstraint("ck_sessions_id_not_empty", "id <> '00000000-0000-0000-0000-000000000000'::uuid");
                table.CheckConstraint("ck_sessions_last_updated_session_id_not_empty", "last_updated_session_id <> '00000000-0000-0000-0000-000000000000'::uuid");
                table.CheckConstraint("ck_sessions_user_id_not_empty", "user_id <> '00000000-0000-0000-0000-000000000000'::uuid");
            });

        migrationBuilder.CreateTable(
            name: "tenants",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, collation: "case_accent_insensitive"),
                email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "case_accent_insensitive"),
                business_type = table.Column<BusinessType>(type: "business_type", nullable: false),
                country = table.Column<Country>(type: "country", nullable: false),
                currency = table.Column<Currency>(type: "currency", nullable: false),
                language = table.Column<Language>(type: "language", nullable: false),
                tenant_state = table.Column<TenantState>(type: "tenant_state", nullable: false),
                tenant_status = table.Column<TenantStatus>(type: "tenant_status", nullable: false),
                tenant_type = table.Column<TenantType>(type: "tenant_type", nullable: false),
                fiscal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                owner_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                address_id = table.Column<Guid>(type: "uuid", nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted_session_id = table.Column<Guid>(type: "uuid", nullable: true),
                TimeZone_Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Offset = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                last_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                created_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                last_updated_session_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_tenants", x => x.id);
                table.CheckConstraint("ck_tenants_created_session_id_not_empty", "created_session_id <> '00000000-0000-0000-0000-000000000000'::uuid");
                table.CheckConstraint("ck_tenants_id_not_empty", "id <> '00000000-0000-0000-0000-000000000000'::uuid");
                table.CheckConstraint("ck_tenants_last_updated_session_id_not_empty", "last_updated_session_id <> '00000000-0000-0000-0000-000000000000'::uuid");
                table.ForeignKey(
                    name: "fk_tenants_addresses_address_id",
                    column: x => x.address_id,
                    principalTable: "addresses",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, collation: "case_accent_insensitive"),
                email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "case_accent_insensitive"),
                role = table.Column<UserRole>(type: "user_role", nullable: false),
                language = table.Column<Language>(type: "language", nullable: false),
                user_state = table.Column<UserState>(type: "user_state", nullable: false),
                user_status = table.Column<UserStatus>(type: "user_status", nullable: false),
                email_verification_state = table.Column<VerificationState>(type: "verification_state", nullable: false),
                phone_number_verification_state = table.Column<VerificationState>(type: "verification_state", nullable: false),
                phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted_session_id = table.Column<Guid>(type: "uuid", nullable: true),
                sort_order = table.Column<double>(type: "double precision", nullable: true, defaultValueSql: "get_next_sort_order_asc('users', 'sort_order')"),
                discriminator = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false, collation: "case_accent_insensitive"),
                Password_HashValue = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Password_Strength = table.Column<PasswordStrength>(type: "password_strength", nullable: false),
                tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                last_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                created_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                last_updated_session_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
                table.CheckConstraint("ck_users_created_session_id_not_empty", "created_session_id <> '00000000-0000-0000-0000-000000000000'::uuid");
                table.CheckConstraint("ck_users_id_not_empty", "id <> '00000000-0000-0000-0000-000000000000'::uuid");
                table.CheckConstraint("ck_users_last_updated_session_id_not_empty", "last_updated_session_id <> '00000000-0000-0000-0000-000000000000'::uuid");
                table.CheckConstraint("ck_users_tenant_id_not_empty", "tenant_id <> '00000000-0000-0000-0000-000000000000'::uuid");
                table.ForeignKey(
                    name: "fk_users_tenants_tenant_id",
                    column: x => x.tenant_id,
                    principalTable: "tenants",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "ix_addresses_sort_order",
            table: "addresses",
            column: "sort_order");

        migrationBuilder.CreateIndex(
            name: "ix_addresses_tenant_id",
            table: "addresses",
            column: "tenant_id");

        migrationBuilder.CreateIndex(
            name: "ix_sessions_tenant_id",
            table: "sessions",
            column: "tenant_id");

        migrationBuilder.CreateIndex(
            name: "ix_sessions_user_id",
            table: "sessions",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_tenants_address_id",
            table: "tenants",
            column: "address_id",
            unique: true,
            filter: "is_deleted = false");

        migrationBuilder.CreateIndex(
            name: "ix_tenants_email",
            table: "tenants",
            column: "email",
            unique: true,
            filter: "is_deleted = false");

        migrationBuilder.CreateIndex(
            name: "ix_tenants_fiscal_code",
            table: "tenants",
            column: "fiscal_code",
            unique: true,
            filter: "is_deleted = false");

        migrationBuilder.CreateIndex(
            name: "ix_tenants_owner_user_id",
            table: "tenants",
            column: "owner_user_id",
            unique: true,
            filter: "is_deleted = false");

        migrationBuilder.CreateIndex(
            name: "ix_users_email_tenant_id_discriminator",
            table: "users",
            columns: ["email", "tenant_id", "discriminator"],
            unique: true,
            filter: "is_deleted = false");

        migrationBuilder.CreateIndex(
            name: "ix_users_phone_number_tenant_id_discriminator",
            table: "users",
            columns: ["phone_number", "tenant_id", "discriminator"],
            unique: true,
            filter: "is_deleted = false");

        migrationBuilder.CreateIndex(
            name: "ix_users_sort_order",
            table: "users",
            column: "sort_order");

        migrationBuilder.CreateIndex(
            name: "ix_users_tenant_id",
            table: "users",
            column: "tenant_id");

        migrationBuilder.AddForeignKey(
            name: "fk_addresses_tenants_tenant_id",
            table: "addresses",
            column: "tenant_id",
            principalTable: "tenants",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_sessions_tenants_tenant_id",
            table: "sessions",
            column: "tenant_id",
            principalTable: "tenants",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_sessions_users_user_id",
            table: "sessions",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "fk_tenants_users_owner_user_id",
            table: "tenants",
            column: "owner_user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        Guard.NotNull(migrationBuilder);

        migrationBuilder.DropForeignKey(
            name: "fk_addresses_tenants_tenant_id",
            table: "addresses");

        migrationBuilder.DropForeignKey(
            name: "fk_users_tenants_tenant_id",
            table: "users");

        migrationBuilder.DropTable(
            name: "sessions");

        migrationBuilder.DropTable(
            name: "tenants");

        migrationBuilder.DropTable(
            name: "addresses");

        migrationBuilder.DropTable(
            name: "users");
    }
}
