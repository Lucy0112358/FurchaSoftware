-- Create the schema if it does not exist
CREATE SCHEMA IF NOT EXISTS furcha;

CREATE TYPE furcha.locker_status AS ENUM ('free', 'occupied');
CREATE TYPE furcha.locker_type AS ENUM ('Personal', 'Common', 'Hand Over', 'Parcel');
CREATE TYPE furcha.user_action_enum AS ENUM ('success', 'failure');

-- Create tables with schema furcha
-- Create BranchAddress table
CREATE TABLE IF NOT EXISTS furcha."BranchAddress"
(
    "Id" SERIAL PRIMARY KEY,
    "Street" VARCHAR(255) NOT NULL,
    "City" VARCHAR(100) NOT NULL,
    "PostalCode" VARCHAR(20),
    "Country" VARCHAR(100) NOT NULL
);
-- Create the Country table with manual PK insertion
CREATE TABLE IF NOT EXISTS furcha."Country"
(
    "Id" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    CONSTRAINT "Country_pkey" PRIMARY KEY ("Id")
);

-- Create the Company table
CREATE TABLE IF NOT EXISTS furcha."Company"
(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "Name" TEXT NOT NULL,
    "City" TEXT NOT NULL,
    "Street" TEXT NOT NULL,
    "Country" INTEGER NOT NULL,
    "Email" TEXT,
    "Phone" TEXT,
    "CountryCode" TEXT,
    CONSTRAINT "Company_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "Company_country_id" FOREIGN KEY ("Country") REFERENCES furcha."Country" ("Id")
);
-- Create Branch table with foreign key constraints
CREATE TABLE IF NOT EXISTS furcha."Branch"
(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "Name" VARCHAR(255) NOT NULL,
    "CompanyId" INTEGER,
    "AddressId" INTEGER UNIQUE NOT NULL,
    CONSTRAINT "Branch_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "fk_Company_Id" FOREIGN KEY ("CompanyId")
    REFERENCES furcha."Company" ("Id")
    ON UPDATE NO ACTION
    ON DELETE SET NULL,
    CONSTRAINT "fk_BranchAddress_Id" FOREIGN KEY ("AddressId")
    REFERENCES furcha."BranchAddress" ("Id")
    ON UPDATE NO ACTION
    ON DELETE SET NULL
);
-- Create LockerGroup table with foreign key constraint to Branch
CREATE TABLE IF NOT EXISTS furcha."LockerGroup"
(
    "Id" SERIAL NOT NULL,
    "BranchId" INTEGER NOT NULL,
    "Name" CHARACTER VARYING(100) NOT NULL,
    "Description" TEXT,
    CONSTRAINT "lockergroup_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "fk_lockergroup_branch" FOREIGN KEY ("BranchId")
    REFERENCES furcha."Branch" ("Id")
    ON UPDATE NO ACTION
    ON DELETE CASCADE
);
-- Create BrainModule table with foreign key constraint to Branch
CREATE TABLE IF NOT EXISTS furcha."BrainModule"
(
    "Description" TEXT NULL,
    "BranchId" INTEGER,
    "GroupId" INTEGER,
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "IpAddress" TEXT,
    "MacAddress" VARCHAR(20),
    CONSTRAINT "BrainModule_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "fk_Branch_Id" FOREIGN KEY ("BranchId")
    REFERENCES furcha."Branch" ("Id")
    ON UPDATE NO ACTION
    ON DELETE CASCADE,
    CONSTRAINT "fk_LockerGroup_Id" FOREIGN KEY ("GroupId")
    REFERENCES furcha."LockerGroup" ("Id") 
    ON UPDATE NO ACTION
    ON DELETE CASCADE
);
-- Create User table
CREATE TABLE IF NOT EXISTS furcha."User"
(
    "Id" INTEGER NOT NULL,
    "Email" TEXT,
    "Phone" TEXT,
    "CountryCode" TEXT,
    "Name" TEXT,
    "Surname" TEXT,
    "CreatedDate" DATE,
    CONSTRAINT "User_pkey" PRIMARY KEY ("Id")
);
-- Create Card table
CREATE TABLE IF NOT EXISTS furcha."Card"
(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "UserId" INTEGER NULL,
    "CardNumber" BIGINT UNIQUE NOT NULL,
    CONSTRAINT "Card_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "fk_User_Id" FOREIGN KEY ("UserId") REFERENCES furcha."User" ("Id") 
    ON UPDATE NO ACTION
    ON DELETE SET NULL
);



-- Create UserGroup table with foreign key constraint to Branch
CREATE TABLE IF NOT EXISTS furcha."UserGroup"
(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "BranchId" INTEGER NOT NULL,
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    CONSTRAINT "UserGroup_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "fk_Branch_Id" FOREIGN KEY ("BranchId")
    REFERENCES furcha."Branch" ("Id")
    ON UPDATE NO ACTION
    ON DELETE CASCADE
);

-- Create User_UserGroup table
CREATE TABLE IF NOT EXISTS furcha."User_UserGroup"
(
    "UserId" INTEGER NOT NULL,
    "UserGroupId" INTEGER NOT NULL,
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    CONSTRAINT "fk_User_Id" FOREIGN KEY ("UserId")
    REFERENCES furcha."User" ("Id")
    ON UPDATE NO ACTION
    ON DELETE CASCADE,
    CONSTRAINT "fk_Group_Id" FOREIGN KEY ("UserGroupId")
    REFERENCES furcha."UserGroup" ("Id")
    ON UPDATE NO ACTION
    ON DELETE CASCADE
);

-- Create Locker table
CREATE TABLE IF NOT EXISTS furcha."Locker"
(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "LockerType" furcha.locker_type NOT NULL,
    "PasswordHash" TEXT,
    "IsActive" INTEGER CHECK ("IsActive" IN (0, 1)),
    "IsOpen" INTEGER CHECK ("IsOpen" IN (0, 1)),
    "LockerStatus" furcha.locker_status,
    CONSTRAINT "Locker_pkey" PRIMARY KEY ("Id")
);





-- Create UserEventLog table
CREATE TABLE IF NOT EXISTS furcha."UserEventLog"
(
    "Id" SERIAL PRIMARY KEY,
    "LockerId" INTEGER NOT NULL,
    "CardIdId" INTEGER,
    "Action" furcha.user_action_enum NOT NULL,
    "LockerStateAfter" TEXT,
    "CreatedDate" DATE NOT NULL,
    "UserId" INTEGER,
    CONSTRAINT "UserEventLog_locker_id" FOREIGN KEY ("LockerId") REFERENCES furcha."Locker" ("Id"),
    CONSTRAINT "UserEventLog_card_id" FOREIGN KEY ("CardIdId") REFERENCES furcha."Card" ("Id"),
    CONSTRAINT "UserEventLog_user_id" FOREIGN KEY ("UserId") REFERENCES furcha."User" ("Id")
);

-- Create UserLocker table
CREATE TABLE IF NOT EXISTS furcha."UserLocker"
(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "LockerId" INTEGER NOT NULL,
    "UserId" INTEGER NOT NULL,
    CONSTRAINT "UserLocker_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "UserLocker_locker_Id" FOREIGN KEY ("LockerId") REFERENCES furcha."Locker" ("Id"),
    CONSTRAINT "UserLocker_user_Id" FOREIGN KEY ("UserId") REFERENCES furcha."User" ("Id")
);

-- Create UserCard table
CREATE TABLE IF NOT EXISTS furcha."UserCard"
(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "UserId" INTEGER NOT NULL,
    "CardId" INTEGER NOT NULL,
    CONSTRAINT "UserCard_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "UserCard_user_Id" FOREIGN KEY ("UserId") REFERENCES furcha."User" ("Id"),
    CONSTRAINT "UserCard_card_Id" FOREIGN KEY ("CardId") REFERENCES furcha."Card" ("Id"),
    CONSTRAINT "UserCard_unique_user_card" UNIQUE ("UserId", "CardId")
);

-- Create UserBranch table
CREATE TABLE IF NOT EXISTS furcha."UserBranch"
(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "UserId" INTEGER NOT NULL,
    "BranchId" INTEGER NOT NULL,
    CONSTRAINT "UserBranch_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "UserBranch_user_Id" FOREIGN KEY ("UserId") REFERENCES furcha."User" ("Id"),
    CONSTRAINT "UserBranch_branch_Id" FOREIGN KEY ("BranchId") REFERENCES furcha."Branch" ("Id"),
    CONSTRAINT "UserBranch_unique_user_branch" UNIQUE ("UserId", "BranchId")
);



-- Create Roles table
CREATE TABLE IF NOT EXISTS furcha."Roles"
(
    "Id" INTEGER NOT NULL,
    "Name" TEXT NOT NULL UNIQUE,
    "Description" TEXT,
    "OpenName" TEXT NOT NULL UNIQUE,
    CONSTRAINT "Roles_pkey" PRIMARY KEY ("Id")
);

-- Create Administrators table
CREATE TABLE IF NOT EXISTS furcha."Administrators"
(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "Name" TEXT NOT NULL,
    "Surname" TEXT NOT NULL,
    "Email" TEXT NOT NULL UNIQUE,
    "PasswordHash" TEXT NOT NULL,
    "Salt" TEXT NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "ModifiedBy" INTEGER,
    "LastPasswordChangeDate" TIMESTAMPTZ,
    "ForcePasswordReset" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedDate" TIMESTAMPTZ,
    "ModifiedDate" TIMESTAMPTZ NOT NULL,
    CONSTRAINT "Administrators_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "Administrators_modifiedBy_fkey" FOREIGN KEY ("ModifiedBy") REFERENCES furcha."Administrators" ("Id")
);

-- Create AdminRoles table
CREATE TABLE IF NOT EXISTS furcha."AdminRoles"
(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "AdministratorId" INTEGER NOT NULL,
    "RoleId" INTEGER NOT NULL,
    CONSTRAINT "AdminRoles_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "AdminRoles_administratorId_fkey" FOREIGN KEY ("AdministratorId") REFERENCES furcha."Administrators" ("Id"),
    CONSTRAINT "AdminRoles_roleId_fkey" FOREIGN KEY ("RoleId") REFERENCES furcha."Roles" ("Id"),
    CONSTRAINT "AdminRoles_unique_administrator_role" UNIQUE ("AdministratorId", "RoleId")
);

-- Create AdminBranch table
CREATE TABLE IF NOT EXISTS furcha."AdminBranch"
(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "AdministratorId" INTEGER NOT NULL,
    "BranchId" INTEGER NOT NULL,
    CONSTRAINT "AdminBranch_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "AdminBranch_administrator_Id" FOREIGN KEY ("AdministratorId") REFERENCES furcha."Administrators" ("Id"),
    CONSTRAINT "AdminBranch_branch_Id" FOREIGN KEY ("BranchId") REFERENCES furcha."Branch" ("Id"),
    CONSTRAINT "AdminBranch_unique_administrator_branch" UNIQUE ("AdministratorId", "BranchId")
);

-- Create AdminSessions table
CREATE TABLE IF NOT EXISTS furcha."AdminSessions"
(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "AdministratorId" INTEGER NOT NULL,
    "LoginTimestamp" TIMESTAMPTZ NOT NULL,
    "LogoutTimestamp" TIMESTAMPTZ,
    "IpAddress" TEXT,
    CONSTRAINT "AdminSessions_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "AdminSessions_administrator_Id" FOREIGN KEY ("AdministratorId") REFERENCES furcha."Administrators" ("Id")
);
