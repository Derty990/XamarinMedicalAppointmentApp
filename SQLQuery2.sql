
DROP TABLE IF EXISTS Prescriptions
DROP TABLE IF EXISTS MedicalRecords
DROP TABLE IF EXISTS Appointments
DROP TABLE IF EXISTS DoctorClinics
DROP TABLE IF EXISTS Doctors
DROP TABLE IF EXISTS Users
DROP TABLE IF EXISTS Clinics
DROP TABLE IF EXISTS Specializations
-- 1. Tabele bez kluczy obcych (lub odwo�uj�ce si� tylko do siebie)

CREATE TABLE Specializations (
    SpecializationId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500)
);

CREATE TABLE Clinics (
    ClinicId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Address NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    IsActive BIT DEFAULT 1 NOT NULL
);

-- 2. Tabele odwo�uj�ce si� do tabel z kroku 1.


CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL, -- Pami�taj o bezpiecznym hashowaniu!
    DateOfBirth DATE,
    Gender NVARCHAR(10),
    Phone NVARCHAR(20),
    Address NVARCHAR(200),
    RoleId INT NOT NULL, -- Przechowuje ID roli (np. 1=Patient, 2=Doctor z Enuma)
    IsActive BIT DEFAULT 1 NOT NULL,
    RegisteredDate DATETIME DEFAULT GETDATE() NOT NULL
);

-- Lekarze (odwo�uje si� do Users i Specializations)
CREATE TABLE Doctors (
    DoctorId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL UNIQUE,
    LicenseNumber NVARCHAR(50) NOT NULL,
    SpecializationId INT,
    PictureUrl NVARCHAR(255),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (SpecializationId) REFERENCES Specializations(SpecializationId)
);

-- 3. Tabele odwo�uj�ce si� do tabel z krok�w 1 i 2.

-- Tabela ��cz�ca dla relacji wiele-do-wielu mi�dzy Doctors a Clinics
CREATE TABLE DoctorClinics (
    DoctorClinicId INT PRIMARY KEY IDENTITY(1,1),
    DoctorId INT NOT NULL,
    ClinicId INT NOT NULL,
    FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId) ON DELETE CASCADE,
    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId) ON DELETE CASCADE,
    UNIQUE (DoctorId, ClinicId)
);

-- Wizyty (odwo�uje si� do Users, Doctors i Clinics)
CREATE TABLE Appointments (
    AppointmentId INT PRIMARY KEY IDENTITY(1,1),
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    ClinicId INT NOT NULL,
    AppointmentDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Scheduled' CHECK (Status IN ('Scheduled', 'Completed', 'Cancelled', 'NoShow')),
    Notes NVARCHAR(500),
    CreatedDate DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (PatientId) REFERENCES Users(UserId),
    FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId),
    CHECK (EndTime > StartTime)
);

-- 4. Tabele odwo�uj�ce si� do wcze�niejszych tabel

-- Historia medyczna (odwo�uje si� do Users i Appointments)
CREATE TABLE MedicalRecords (
    RecordId INT PRIMARY KEY IDENTITY(1,1),
    PatientId INT NOT NULL,
    AppointmentId INT UNIQUE,
    Diagnosis NVARCHAR(500),
    RecordDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (PatientId) REFERENCES Users(UserId),
    FOREIGN KEY (AppointmentId) REFERENCES Appointments(AppointmentId)
);

-- Recepty (odwo�uje si� do Users, Doctors i Appointments)

CREATE TABLE Prescriptions (
    PrescriptionId INT PRIMARY KEY IDENTITY(1,1),
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    AppointmentId INT, -- Usuni�to UNIQUE st�d
    Medication NVARCHAR(500) NOT NULL,
    Dosage NVARCHAR(200) NOT NULL,
    IssuedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ExpiryDate DATE,
    FOREIGN KEY (PatientId) REFERENCES Users(UserId),
    FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    FOREIGN KEY (AppointmentId) REFERENCES Appointments(AppointmentId) -- Relacja nadal istnieje
);