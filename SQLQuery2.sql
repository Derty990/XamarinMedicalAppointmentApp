-- Usuwanie tabel w odwrotnej kolejnoœci zale¿noœci
DROP TABLE IF EXISTS DoctorClinics;
DROP TABLE IF EXISTS Appointments;
DROP TABLE IF EXISTS Doctors;
DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS Clinics;
DROP TABLE IF EXISTS Addresses;
DROP TABLE IF EXISTS Specializations;
DROP TABLE IF EXISTS AppointmentStatuses;
DROP TABLE IF EXISTS MedicalRecords;
DROP TABLE IF EXISTS AppointmentStatuses;
DROP TABLE IF EXISTS Prescriptions;


-- 1. Tabele podstawowe

CREATE TABLE Addresses (
    AddressId INT PRIMARY KEY IDENTITY(1,1),
    Street NVARCHAR(150) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(10) NOT NULL
);

CREATE TABLE Specializations (
    SpecializationId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE AppointmentStatuses (
    StatusId INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL UNIQUE -- Np. 'Scheduled', 'Completed', 'Cancelled'
);

-- 2. Tabele odwo³uj¹ce siê do tabel podstawowych

CREATE TABLE Clinics (
    ClinicId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    AddressId INT NOT NULL,              -- Klucz obcy do tabeli Addresses
    FOREIGN KEY (AddressId) REFERENCES Addresses(AddressId)
);

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    AddressId INT NULL,                 -- Klucz obcy do Addresses (NULL jeœli u¿ytkownik nie musi mieæ adresu)
    RoleId INT NOT NULL                 -- Mapowane na Enum (1=Patient, 2=Doctor, 3=Admin)
    -- Usuniêto: DateOfBirth, Gender, Phone, IsActive, RegisteredDate
    FOREIGN KEY (AddressId) REFERENCES Addresses(AddressId)
);

-- 3. Tabele zale¿ne od Users, Clinics, Specializations

CREATE TABLE Doctors (
    DoctorId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL UNIQUE,             -- Odwo³anie do u¿ytkownika, który jest lekarzem
    SpecializationId INT NOT NULL,          -- Lekarz musi mieæ specjalizacjê
    -- Usuniêto: LicenseNumber, PictureUrl
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (SpecializationId) REFERENCES Specializations(SpecializationId)
);

-- 4. Tabele relacyjne i g³ówne tabele transakcyjne

-- Tabela ³¹cz¹ca dla relacji Wiele-do-Wielu: Doctors <-> Clinics
CREATE TABLE DoctorClinics (
    DoctorClinicId INT PRIMARY KEY IDENTITY(1,1),
    DoctorId INT NOT NULL,
    ClinicId INT NOT NULL,
    FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId) ON DELETE CASCADE,
    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId) ON DELETE CASCADE,
    UNIQUE (DoctorId, ClinicId)
);

-- Wizyty
CREATE TABLE Appointments (
    AppointmentId INT PRIMARY KEY IDENTITY(1,1),
    PatientId INT NOT NULL,             -- U¿ytkownik (pacjent)
    DoctorId INT NOT NULL,              -- Lekarz (z tabeli Doctors)
    ClinicId INT NOT NULL,              -- Klinika (z tabeli Clinics)
    AppointmentDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,              -- Pozostawione dla elastycznoœci, choæ mo¿na by obliczaæ
    StatusId INT NOT NULL,              -- Klucz obcy do AppointmentStatuses
    -- Usuniêto: Notes, CreatedDate
    FOREIGN KEY (PatientId) REFERENCES Users(UserId),
    FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId),
    FOREIGN KEY (StatusId) REFERENCES AppointmentStatuses(StatusId),
    CHECK (EndTime > StartTime)
);