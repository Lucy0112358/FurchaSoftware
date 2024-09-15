-- Create the schema if it does not exist
CREATE SCHEMA IF NOT EXISTS furcha;

CREATE TYPE furcha.locker_status AS ENUM ('free', 'occupied');
CREATE TYPE furcha.locker_type AS ENUM ('Personal', 'Common', 'Hand Over', 'Parcel');

-- Create tables with schema furcha

-- Create Branch table with foreign key constraints
CREATE TABLE IF NOT EXISTS furcha."Branch"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    name varchar(255) NOT NULL,
    "companyId" integer,
    "addressId" integer UNIQUE NOT NULL,
    CONSTRAINT "Branch_pkey" PRIMARY KEY (id),
    CONSTRAINT "fk_Company_Id" FOREIGN KEY ("companyId")
    REFERENCES furcha."Company" (id)
    ON UPDATE NO ACTION
    ON DELETE SET NULL,
    CONSTRAINT "fk_BranchAddress_Id" FOREIGN KEY ("addressId")
    REFERENCES furcha."BranchAddress" (id)
    ON UPDATE NO ACTION
    ON DELETE SET NULL
);

-- Create BrainModule table with foreign key constraint to Branch
CREATE TABLE IF NOT EXISTS furcha."BrainModule"
(
    "description" text NULL,
    "branchId" integer,
    "groupId" integer,
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "ipAddress" text,
    "macAddress" varchar(20), 
    CONSTRAINT "BrainModule_pkey" PRIMARY KEY (id),
    CONSTRAINT "fk_Branch_Id" FOREIGN KEY ("branchId")
    REFERENCES furcha."Branch" (id)
    ON UPDATE NO ACTION
    ON DELETE CASCADE,
    CONSTRAINT "fk_LockerGroup_Id" FOREIGN KEY ("groupId")
    REFERENCES furcha."LockerGroup" (id) 
    ON UPDATE NO ACTION
    ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS furcha."Card"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "userId" integer NULL,
    "cardNumber" bigint UNIQUE NOT NULL,  
    CONSTRAINT "Card_pkey" PRIMARY KEY (id),
    CONSTRAINT "fk_User_Id" FOREIGN KEY ("userId") REFERENCES furcha."User"(id) 
    ON UPDATE NO ACTION
    ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS furcha."BranchAddress"
(
    id serial PRIMARY KEY,             
    "street" varchar(255) NOT NULL,     
    "city" varchar(100) NOT NULL,                      
    "postalCode" varchar(20),            
    "country" varchar(100) NOT NULL,             
);

CREATE TABLE IF NOT EXISTS furcha."UserGroup"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "branchId" integer NOT NULL,              
    "name" varchar(255) NOT NULL,         
    "description" text,     
    CONSTRAINT "UserGroup_pkey" PRIMARY KEY (id),
    CONSTRAINT "fk_Branch_Id" FOREIGN KEY ("branchId")
    REFERENCES furcha."Branch" (id)
    ON UPDATE NO ACTION
    ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS furcha."User_UserGroup"
(
    "userId" integer NOT NULL,
    "userGroupId" integer NOT NULL,
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    CONSTRAINT "fk_User_Id" FOREIGN KEY ("userId")
    REFERENCES furcha."User" (id)
    ON UPDATE NO ACTION
    ON DELETE CASCADE,
    CONSTRAINT "fk_Group_Id" FOREIGN KEY ("userGroupId")
    REFERENCES furcha."UserGroup" (id)
    ON UPDATE NO ACTION
    ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS furcha."Locker"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1),
    "lockerType" furcha.locker_type NOT NULL,  
    "passwordHash" text,                            
    "isActive" integer CHECK ("isActive" IN (0, 1)),                 
    "isOpen" integer CHECK ("isOpen" IN (0, 1)),                        
    "LockerStatus" furcha.locker_status,                                             
    CONSTRAINT "Locker_pkey" PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS furcha."LockerGroup"
(
    id serial NOT NULL,
    branchId integer NOT NULL,
    "name" character varying(100) NOT NULL,
    "description" text,
    CONSTRAINT "lockergroup_pkey" PRIMARY KEY (id)
    CONSTRAINT "fk_lockergroup_branch" FOREIGN KEY (branchid)
    REFERENCES furcha."Branch" (id)
    ON UPDATE NO ACTION
    ON DELETE CASCADE,
);

CREATE TABLE IF NOT EXISTS furcha."User"
(
    id integer NOT NULL,
    email text,
    phone text,
    name text,
    surname text,
    "lockerId" integer,
    CONSTRAINT "User_pkey" PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS furcha."UserEventLog"
(
    id integer NOT NULL,
    "lockerId" integer,
    action text,
    "modifiedDate" date,
    "userId" integer,
    CONSTRAINT "UserEventLog_pkey" PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS furcha."UserLocker"
(
    id integer NOT NULL,
    "lockerId" integer,
    "userId" integer,
    CONSTRAINT "UserLocker_pkey" PRIMARY KEY (id)
);

UserCard

Company

UserBranch

Roles

Administrators

AdminRoles