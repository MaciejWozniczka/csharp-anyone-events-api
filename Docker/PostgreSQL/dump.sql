-- Adminer 4.8.1 PostgreSQL 14.1 dump

DROP TABLE IF EXISTS "Addresses";
CREATE TABLE "public"."Addresses" (
    "Id" uuid NOT NULL,
    "Label" text,
    "CountryCode" text,
    "CountryName" text,
    "StateCode" text,
    "State" text,
    "CountyCode" text,
    "County" text,
    "City" text,
    "District" text,
    "Street" text,
    "PostalCode" text,
    "HouseNumber" text,
    "ApartmentNumber" text,
    "CreateDate" timestamptz NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletingDate" timestamptz,
    CONSTRAINT "PK_Addresses" PRIMARY KEY ("Id")
) WITH (oids = false);


DROP TABLE IF EXISTS "AspNetRoles";
CREATE TABLE "public"."AspNetRoles" (
    "Id" text NOT NULL,
    "Name" character varying(256),
    "NormalizedName" character varying(256),
    "ConcurrencyStamp" text,
    CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id"),
    CONSTRAINT "RoleNameIndex" UNIQUE ("NormalizedName")
) WITH (oids = false);


DROP TABLE IF EXISTS "Categories";
CREATE TABLE "public"."Categories" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Description" text,
    "Picture" text,
    "CreateDate" timestamptz NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletingDate" timestamptz,
    CONSTRAINT "PK_Categories" PRIMARY KEY ("Id")
) WITH (oids = false);


DROP TABLE IF EXISTS "AspNetUsers";
CREATE TABLE "public"."AspNetUsers" (
    "Id" text NOT NULL,
    "FirstName" text,
    "LastName" text,
    "Age" integer,
    "Country" integer,
    "CurrentLocationId" uuid,
    "Nationality" text,
    "Sex" integer,
    "Languages" text[],
    "Picture" text,
    "Desciption" text,
    "PhoneNumber" integer,
    "PhoneCountryCode" text,
    "UserType" integer,
    "CreateDate" timestamptz NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletingDate" timestamptz,
    "UserName" character varying(256),
    "NormalizedUserName" character varying(256),
    "Email" character varying(256),
    "NormalizedEmail" character varying(256),
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text,
    "SecurityStamp" text,
    "ConcurrencyStamp" text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamptz,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL,
    CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id"),
    CONSTRAINT "UserNameIndex" UNIQUE ("NormalizedUserName")
) WITH (oids = false);

CREATE INDEX "EmailIndex" ON "public"."AspNetUsers" USING btree ("NormalizedEmail");

CREATE INDEX "IX_AspNetUsers_CurrentLocationId" ON "public"."AspNetUsers" USING btree ("CurrentLocationId");


DROP TABLE IF EXISTS "Locations";
CREATE TABLE "public"."Locations" (
    "Id" uuid NOT NULL,
    "Latitude" double precision NOT NULL,
    "Longitude" double precision NOT NULL,
    "Distance" integer NOT NULL,
    "UserId" text NOT NULL,
    "CreateDate" timestamptz NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletingDate" timestamptz,
    CONSTRAINT "PK_Locations" PRIMARY KEY ("Id")
) WITH (oids = false);


DROP TABLE IF EXISTS "EventTypes";
CREATE TABLE "public"."EventTypes" (
    "Id" uuid NOT NULL,
    "CategoryId" uuid NOT NULL,
    "Name" text NOT NULL,
    "Type" text,
    "Picture" text,
    "CreateDate" timestamptz NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletingDate" timestamptz,
    CONSTRAINT "PK_EventTypes" PRIMARY KEY ("Id")
) WITH (oids = false);

CREATE INDEX "IX_EventTypes_CategoryId" ON "public"."EventTypes" USING btree ("CategoryId");


DROP TABLE IF EXISTS "Events";
CREATE TABLE "public"."Events" (
    "Id" uuid NOT NULL,
    "EventTypeId" uuid NOT NULL,
    "CategoryId" uuid NOT NULL,
    "CreatorId" text NOT NULL,
    "EventDateTime" timestamptz NOT NULL,
    "Duration" integer NOT NULL,
    "LocationId" uuid NOT NULL,
    "AddressId" uuid NOT NULL,
    "ShortDescription" text NOT NULL,
    "Description" text NOT NULL,
    "Picture" text,
    "PeopleLimit" integer NOT NULL,
    "AgeFrom" integer,
    "AgeTo" integer,
    "Cities" text[],
    "Countries" integer[],
    "Nationalities" text[],
    "SexTypes" integer[],
    "Languages" text[],
    "ExperienceLevels" integer[],
    "TripType" integer,
    "Budget" text,
    "Destination" text,
    "IsActive" boolean NOT NULL,
    "CreateDate" timestamptz NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletingDate" timestamptz,
    CONSTRAINT "PK_Events" PRIMARY KEY ("Id")
) WITH (oids = false);

CREATE INDEX "IX_Events_AddressId" ON "public"."Events" USING btree ("AddressId");

CREATE INDEX "IX_Events_CategoryId" ON "public"."Events" USING btree ("CategoryId");

CREATE INDEX "IX_Events_CreatorId" ON "public"."Events" USING btree ("CreatorId");

CREATE INDEX "IX_Events_EventTypeId" ON "public"."Events" USING btree ("EventTypeId");

CREATE INDEX "IX_Events_LocationId" ON "public"."Events" USING btree ("LocationId");


DROP TABLE IF EXISTS "Tenants";
CREATE TABLE "public"."Tenants" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Email" text NOT NULL,
    "Industry" text NOT NULL,
    "TaxNumber" text NOT NULL,
    "Logo" text NOT NULL,
    "AddressId" uuid NOT NULL,
    "UserId" text NOT NULL,
    "CreateDate" timestamptz NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletingDate" timestamptz,
    CONSTRAINT "PK_Tenants" PRIMARY KEY ("Id")
) WITH (oids = false);

CREATE INDEX "IX_Tenants_AddressId" ON "public"."Tenants" USING btree ("AddressId");

CREATE INDEX "IX_Tenants_UserId" ON "public"."Tenants" USING btree ("UserId");


DROP TABLE IF EXISTS "Communications";
CREATE TABLE "public"."Communications" (
    "Id" uuid NOT NULL,
    "EventId" uuid,
    "Message" text,
    "UserId" text,
    "CreateDate" timestamptz NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletingDate" timestamptz,
    CONSTRAINT "PK_Communications" PRIMARY KEY ("Id")
) WITH (oids = false);

CREATE INDEX "IX_Communications_EventId" ON "public"."Communications" USING btree ("EventId");

CREATE INDEX "IX_Communications_UserId" ON "public"."Communications" USING btree ("UserId");


DROP TABLE IF EXISTS "EventUser";
CREATE TABLE "public"."EventUser" (
    "EventsAssignedId" uuid NOT NULL,
    "UsersAssignedId" text NOT NULL,
    CONSTRAINT "PK_EventUser" PRIMARY KEY ("EventsAssignedId", "UsersAssignedId")
) WITH (oids = false);

CREATE INDEX "IX_EventUser_UsersAssignedId" ON "public"."EventUser" USING btree ("UsersAssignedId");


DROP TABLE IF EXISTS "Companies";
CREATE TABLE "public"."Companies" (
    "Id" uuid NOT NULL,
    "TenantId" uuid NOT NULL,
    "AddressId" uuid NOT NULL,
    "BankAccount" text NOT NULL,
    "Picture" text NOT NULL,
    "CreateDate" timestamptz NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletingDate" timestamptz,
    CONSTRAINT "PK_Companies" PRIMARY KEY ("Id")
) WITH (oids = false);

CREATE INDEX "IX_Companies_AddressId" ON "public"."Companies" USING btree ("AddressId");

CREATE INDEX "IX_Companies_TenantId" ON "public"."Companies" USING btree ("TenantId");


DROP TABLE IF EXISTS "Invoices";
CREATE TABLE "public"."Invoices" (
    "Id" uuid NOT NULL,
    "CompanyId" uuid NOT NULL,
    "Number" text NOT NULL,
    "Amount" numeric NOT NULL,
    "IsPaid" boolean NOT NULL,
    "CreateDate" timestamptz NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletingDate" timestamptz,
    CONSTRAINT "PK_Invoices" PRIMARY KEY ("Id")
) WITH (oids = false);

CREATE INDEX "IX_Invoices_CompanyId" ON "public"."Invoices" USING btree ("CompanyId");


DROP TABLE IF EXISTS "AspNetUserLogins";
CREATE TABLE "public"."AspNetUserLogins" (
    "LoginProvider" text NOT NULL,
    "ProviderKey" text NOT NULL,
    "ProviderDisplayName" text,
    "UserId" text NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey")
) WITH (oids = false);

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "public"."AspNetUserLogins" USING btree ("UserId");


DROP TABLE IF EXISTS "AspNetUserRoles";
CREATE TABLE "public"."AspNetUserRoles" (
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId")
) WITH (oids = false);

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "public"."AspNetUserRoles" USING btree ("RoleId");


DROP TABLE IF EXISTS "AspNetUserTokens";
CREATE TABLE "public"."AspNetUserTokens" (
    "UserId" text NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name" text NOT NULL,
    "Value" text,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name")
) WITH (oids = false);


DROP TABLE IF EXISTS "AspNetUserClaims";
CREATE TABLE "public"."AspNetUserClaims" (
    "Id" integer NOT NULL,
    "UserId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text,
    CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id")
) WITH (oids = false);

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "public"."AspNetUserClaims" USING btree ("UserId");


DROP TABLE IF EXISTS "AspNetRoleClaims";
CREATE TABLE "public"."AspNetRoleClaims" (
    "Id" integer NOT NULL,
    "RoleId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text,
    CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id")
) WITH (oids = false);

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "public"."AspNetRoleClaims" USING btree ("RoleId");


DROP TABLE IF EXISTS "__EFMigrationsHistory";
CREATE TABLE "public"."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
) WITH (oids = false);


INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp") VALUES
('4c9b71f5-b2b5-4724-8024-0f3fef7549a4',	'user',	'USER',	'344e9049-c626-4877-95c0-61e85554abde');

INSERT INTO "AspNetUsers" ("Id", "FirstName", "LastName", "Age", "Country", "CurrentLocationId", "Nationality", "Sex", "Languages", "Picture", "Desciption", "PhoneNumber", "PhoneCountryCode", "UserType", "CreateDate", "IsDeleted", "DeletingDate", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount") VALUES
('ad66205d-02aa-4e84-a3d9-ed9b5174a405',	'Maciej',	'Wozniczka',	29,	139,	'27f6b3ac-caba-473d-9aca-9237b50356d5',	'polish',	1,	'{polish,english}',	NULL,	'Half-time traveller',	123456789,	'+48',	0,	'2023-12-16 00:33:18.439552+00',	'f',	NULL,	'maciej.wozniczka@outlook.com',	'MACIEJ.WOZNICZKA@OUTLOOK.COM',	'maciej.wozniczka@outlook.com',	'MACIEJ.WOZNICZKA@OUTLOOK.COM',	'f',	'AQAAAAEAACcQAAAAEC4e+RFPsd4tyKl9V+sHgetM1cLKTDnqdvlou8KmjLnrvTmz5p4fYA/SSI87seTirQ==',	'N55O5ZP72D3FTT7CRF6O533324BE6J2G',	'9c546f2e-1059-47f7-8339-832441eca08a',	'f',	'f',	NULL,	't',	0),
('87f8a59e-a6d7-470e-8e0b-1e3d2bc778a2',	'Bartosz',	'Zasiadczyk',	29,	139,	'13f4cc9e-e62b-41e5-b430-5d27cba828cd',	'polish',	1,	'{polish,english}',	NULL,	'CEO Mordo!',	987654321,	'+48',	0,	'2023-12-16 01:19:40.992596+00',	'f',	NULL,	'bartosz.zasiadczyk@wp.pl',	'BARTOSZ.ZASIADCZYK@WP.PL',	'bartosz.zasiadczyk@wp.pl',	'BARTOSZ.ZASIADCZYK@WP.PL',	'f',	'AQAAAAEAACcQAAAAEMVdlpZkQNFvqPciEoLILGJ8FSRdScYoYr4g/XUo7whnmhUA34pnudGMCHgvc+vnnw==',	'Q6AL3I6HF3OGKD7J2RS7HSWN4LBUUG77',	'65713d54-bec5-4be0-9c12-63652e850108',	'f',	'f',	NULL,	't',	0);

INSERT INTO "AspNetUserRoles" ("UserId", "RoleId") VALUES
('ad66205d-02aa-4e84-a3d9-ed9b5174a405',	'4c9b71f5-b2b5-4724-8024-0f3fef7549a4'),
('87f8a59e-a6d7-470e-8e0b-1e3d2bc778a2',	'4c9b71f5-b2b5-4724-8024-0f3fef7549a4');

INSERT INTO "Locations" ("Id", "Latitude", "Longitude", "Distance", "UserId", "CreateDate", "IsDeleted", "DeletingDate") VALUES
('27f6b3ac-caba-473d-9aca-9237b50356d5',	52.39746,	16.96257,	0,	'ad66205d-02aa-4e84-a3d9-ed9b5174a405',	'2023-12-16 00:39:51.151259+00',	'f',	NULL),
('13f4cc9e-e62b-41e5-b430-5d27cba828cd',	52.46262,	16.92506,	0,	'87f8a59e-a6d7-470e-8e0b-1e3d2bc778a2',	'2023-12-16 01:21:16.422655+00',	'f',	NULL);

INSERT INTO "Categories" ("Id", "Name", "Description", "Picture", "CreateDate", "IsDeleted", "DeletingDate") VALUES
('deba8c96-6697-454f-bb48-6fe3c4105aa8',	'Ogólne',	NULL,	NULL,	'2023-12-16 01:34:02.521651+00',	'f',	NULL),
('9ee65f6a-14eb-4a9e-9db8-83fc767ba690',	'Kultura',	NULL,	NULL,	'2023-12-16 01:34:16.642973+00',	'f',	NULL),
('b7022fb7-688e-41cd-8b2f-c95e7947f90b',	'Sport',	NULL,	NULL,	'2023-12-16 01:34:20.897971+00',	'f',	NULL);

INSERT INTO "EventTypes" ("Id", "CategoryId", "Name", "Type", "Picture", "CreateDate", "IsDeleted", "DeletingDate") VALUES
('69fff64d-8324-4ee7-8f2b-99dffe88c62d',	'deba8c96-6697-454f-bb48-6fe3c4105aa8',	'Kawa',	NULL,	NULL,	'2023-12-16 01:37:37.479065+00',	'f',	NULL),
('6189aa9e-1f71-4c13-a3f4-a17b1c554e27',	'deba8c96-6697-454f-bb48-6fe3c4105aa8',	'Lunch',	NULL,	NULL,	'2023-12-16 01:37:51.138617+00',	'f',	NULL),
('26cbb6e1-6b89-4a26-a8a4-4b2597585ce3',	'deba8c96-6697-454f-bb48-6fe3c4105aa8',	'Spacer',	NULL,	NULL,	'2023-12-16 01:37:55.891051+00',	'f',	NULL),
('d187945f-c606-4a6e-aed6-2e3e5d8c8f77',	'9ee65f6a-14eb-4a9e-9db8-83fc767ba690',	'Kino',	NULL,	NULL,	'2023-12-16 01:38:24.950033+00',	'f',	NULL),
('680ea217-091a-4261-882c-35e1a3b49794',	'9ee65f6a-14eb-4a9e-9db8-83fc767ba690',	'Teatr',	NULL,	NULL,	'2023-12-16 01:38:30.297509+00',	'f',	NULL),
('1c614b25-aa7d-4d94-be8b-3d452f55f5c0',	'9ee65f6a-14eb-4a9e-9db8-83fc767ba690',	'Impreza',	NULL,	NULL,	'2023-12-16 01:38:35.674275+00',	'f',	NULL),
('b60f12c6-d414-4e0c-adbd-6b5a5905754c',	'b7022fb7-688e-41cd-8b2f-c95e7947f90b',	'Bieganie',	NULL,	NULL,	'2023-12-16 01:38:49.035103+00',	'f',	NULL),
('2c4a4cb2-5dad-48c1-a159-7628d74b672a',	'b7022fb7-688e-41cd-8b2f-c95e7947f90b',	'Siłownia',	NULL,	NULL,	'2023-12-16 01:38:53.135884+00',	'f',	NULL),
('4f3ebcc2-3cfc-448b-aebb-52f539b3fbb2',	'b7022fb7-688e-41cd-8b2f-c95e7947f90b',	'Squash',	NULL,	NULL,	'2023-12-16 01:38:59.344559+00',	'f',	NULL);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") VALUES
('20231216003151_DbInitialization',	'6.0.24'),
('20231216013346_CategoryEventTypeNullability',	'6.0.24');


ALTER TABLE ONLY "public"."AspNetRoleClaims" ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles"("Id") ON DELETE CASCADE NOT DEFERRABLE;

ALTER TABLE ONLY "public"."AspNetUserClaims" ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE NOT DEFERRABLE;

ALTER TABLE ONLY "public"."AspNetUserLogins" ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE NOT DEFERRABLE;

ALTER TABLE ONLY "public"."AspNetUserRoles" ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles"("Id") ON DELETE CASCADE NOT DEFERRABLE;
ALTER TABLE ONLY "public"."AspNetUserRoles" ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE NOT DEFERRABLE;

ALTER TABLE ONLY "public"."AspNetUserTokens" ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE NOT DEFERRABLE;

ALTER TABLE ONLY "public"."AspNetUsers" ADD CONSTRAINT "FK_AspNetUsers_Locations_CurrentLocationId" FOREIGN KEY ("CurrentLocationId") REFERENCES "Locations"("Id") NOT DEFERRABLE;

ALTER TABLE ONLY "public"."Communications" ADD CONSTRAINT "FK_Communications_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers"("Id") NOT DEFERRABLE;
ALTER TABLE ONLY "public"."Communications" ADD CONSTRAINT "FK_Communications_Events_EventId" FOREIGN KEY ("EventId") REFERENCES "Events"("Id") NOT DEFERRABLE;

ALTER TABLE ONLY "public"."Companies" ADD CONSTRAINT "FK_Companies_Addresses_AddressId" FOREIGN KEY ("AddressId") REFERENCES "Addresses"("Id") ON DELETE CASCADE NOT DEFERRABLE;
ALTER TABLE ONLY "public"."Companies" ADD CONSTRAINT "FK_Companies_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenants"("Id") ON DELETE CASCADE NOT DEFERRABLE;

ALTER TABLE ONLY "public"."EventTypes" ADD CONSTRAINT "FK_EventTypes_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories"("Id") ON DELETE CASCADE NOT DEFERRABLE;

ALTER TABLE ONLY "public"."EventUser" ADD CONSTRAINT "FK_EventUser_AspNetUsers_UsersAssignedId" FOREIGN KEY ("UsersAssignedId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE NOT DEFERRABLE;
ALTER TABLE ONLY "public"."EventUser" ADD CONSTRAINT "FK_EventUser_Events_EventsAssignedId" FOREIGN KEY ("EventsAssignedId") REFERENCES "Events"("Id") ON DELETE CASCADE NOT DEFERRABLE;

ALTER TABLE ONLY "public"."Events" ADD CONSTRAINT "FK_Events_Addresses_AddressId" FOREIGN KEY ("AddressId") REFERENCES "Addresses"("Id") ON DELETE CASCADE NOT DEFERRABLE;
ALTER TABLE ONLY "public"."Events" ADD CONSTRAINT "FK_Events_AspNetUsers_CreatorId" FOREIGN KEY ("CreatorId") REFERENCES "AspNetUsers"("Id") ON DELETE RESTRICT NOT DEFERRABLE;
ALTER TABLE ONLY "public"."Events" ADD CONSTRAINT "FK_Events_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories"("Id") ON DELETE CASCADE NOT DEFERRABLE;
ALTER TABLE ONLY "public"."Events" ADD CONSTRAINT "FK_Events_EventTypes_EventTypeId" FOREIGN KEY ("EventTypeId") REFERENCES "EventTypes"("Id") ON DELETE CASCADE NOT DEFERRABLE;
ALTER TABLE ONLY "public"."Events" ADD CONSTRAINT "FK_Events_Locations_LocationId" FOREIGN KEY ("LocationId") REFERENCES "Locations"("Id") ON DELETE CASCADE NOT DEFERRABLE;

ALTER TABLE ONLY "public"."Invoices" ADD CONSTRAINT "FK_Invoices_Companies_CompanyId" FOREIGN KEY ("CompanyId") REFERENCES "Companies"("Id") ON DELETE CASCADE NOT DEFERRABLE;

ALTER TABLE ONLY "public"."Tenants" ADD CONSTRAINT "FK_Tenants_Addresses_AddressId" FOREIGN KEY ("AddressId") REFERENCES "Addresses"("Id") ON DELETE CASCADE NOT DEFERRABLE;
ALTER TABLE ONLY "public"."Tenants" ADD CONSTRAINT "FK_Tenants_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE NOT DEFERRABLE;

-- 2023-12-16 00:42:50.599629+00
