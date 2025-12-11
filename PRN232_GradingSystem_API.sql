-- ===== RESET GRADING DATABASE - FULL SCRIPT (pgAdmin version, UTC version) =====

-- 1) Set Timezone (chỉ ảnh hưởng session, không ảnh hưởng dữ liệu UTC)
SET TIMEZONE = 'Asia/Ho_Chi_Minh';

-- 2) DROP TABLES in correct dependency order
DROP TABLE IF EXISTS GradeDetail CASCADE;
DROP TABLE IF EXISTS Grade CASCADE;
DROP TABLE IF EXISTS Submission CASCADE;
DROP TABLE IF EXISTS Exam CASCADE;
DROP TABLE IF EXISTS Group_Student CASCADE;
DROP TABLE IF EXISTS Student CASCADE;
DROP TABLE IF EXISTS "Group" CASCADE;
DROP TABLE IF EXISTS Semester_Subject CASCADE;
DROP TABLE IF EXISTS Subject CASCADE;
DROP TABLE IF EXISTS Semester CASCADE;
DROP TABLE IF EXISTS "User" CASCADE;

-- ===== RECREATE ALL TABLES =====

CREATE TABLE "User" (
    UserId SERIAL PRIMARY KEY,
    Email VARCHAR(255),
    Username VARCHAR(100) NOT NULL,
    Password BYTEA NOT NULL,
    Salt BYTEA NOT NULL,
    Role VARCHAR(50),
    RefreshToken VARCHAR(255) NULL,
    RefreshTokenExpiryTime TIMESTAMPTZ NULL, 
    IsActive BOOLEAN DEFAULT TRUE,
    CreateAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Semester (
    SemesterId SERIAL PRIMARY KEY,
    SemesterCode VARCHAR(50) NOT NULL,
    StartDate TIMESTAMPTZ,
    EndDate TIMESTAMPTZ,
    IsActive BOOLEAN DEFAULT TRUE,
    CreateAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    UpdateAt TIMESTAMPTZ,
    CreateBy INT REFERENCES "User"(UserId),
    UpdateBy INT REFERENCES "User"(UserId)
);

CREATE TABLE Subject (
    SubjectId SERIAL PRIMARY KEY,
    SubjectName VARCHAR(255) NOT NULL,
    CreateAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Semester_Subject (
    SemesterId INT REFERENCES Semester(SemesterId) ON DELETE CASCADE,
    SubjectId INT REFERENCES Subject(SubjectId) ON DELETE CASCADE,
    CreateAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (SemesterId, SubjectId)
);

CREATE TABLE "Group" (
    GroupId SERIAL PRIMARY KEY,
    GroupName VARCHAR(255) NOT NULL,
    SemesterId INT REFERENCES Semester(SemesterId) ON DELETE CASCADE,
    CreateAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    UpdateAt TIMESTAMPTZ,
    CreateBy INT REFERENCES "User"(UserId),
    UpdateBy INT REFERENCES "User"(UserId)
);

CREATE TABLE Student (
    StudentId SERIAL PRIMARY KEY,
    StudentFullName VARCHAR(255),
    StudentRoll VARCHAR(50),
    IsActive BOOLEAN DEFAULT TRUE,
    CreateAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Group_Student (
    GroupId INT REFERENCES "Group"(GroupId) ON DELETE CASCADE,
    StudentId INT REFERENCES Student(StudentId) ON DELETE CASCADE,
    CreateAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (GroupId, StudentId)
);

CREATE TABLE Exam (
    ExamId SERIAL PRIMARY KEY,
    SemesterId INT REFERENCES Semester(SemesterId) ON DELETE CASCADE,
    SubjectId INT REFERENCES Subject(SubjectId) ON DELETE CASCADE,
    ExamName VARCHAR(255),
    ExamDate TIMESTAMPTZ,
    CreateAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Submission (
    SubmissionId SERIAL PRIMARY KEY,
    ExamId INT REFERENCES Exam(ExamId) ON DELETE CASCADE,
    StudentId INT REFERENCES Student(StudentId) ON DELETE CASCADE,
    Solution TEXT,
    Comment TEXT,
    FileUrl VARCHAR(255),
    CreateAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    UpdateAt TIMESTAMPTZ
);

CREATE TABLE Grade (
    GradeId SERIAL PRIMARY KEY,
    SubmissionId INT REFERENCES Submission(SubmissionId) ON DELETE CASCADE,
    Q1 NUMERIC(5,2),
    Q2 NUMERIC(5,2),
    Q3 NUMERIC(5,2),
    Q4 NUMERIC(5,2),
    Q5 NUMERIC(5,2),
    Q6 NUMERIC(5,2),
    TotalScore NUMERIC(6,2),
    Status VARCHAR(50),
    CreateAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    UpdateAt TIMESTAMPTZ,
    Marker INT REFERENCES "User"(UserId)
);


CREATE TABLE GradeDetail (
    GradeDetailId SERIAL PRIMARY KEY,
    GradeId INT REFERENCES Grade(GradeId) ON DELETE CASCADE,
    QCode VARCHAR(50),
    SubCode VARCHAR(255),
    Point NUMERIC(5,2),
    Note TEXT,
    CreateAt TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    UpdateAt TIMESTAMPTZ
);


-- ===== SAMPLE DATA =====

-- 1) Users
INSERT INTO "User" (Email, Username, Password, Salt, Role)
VALUES
('admin@gmail.com', 'admin', 'c73e4eba5702ded60e366c5eef05f3639f0e67c51122b0c114ef9506d18c5a8600148ce8814745c3613705f705f4d7ec850650209cfed023b078188e5130b797', '47291abfc98bb8d7ec343f784b5c3201337f358a789d7f5c5636e825b3e7a8c47e6abce51e1d6bcb68347d4ffd270bb97fb9e58af9303a0f96aed7fd96ca7735dbebc40e7ceda806599fbe85689811d84d9147ca7f3e47c42837015cca5b2eba8853fdfb63136338c61b566ec8fcc913137bb4d4c51f05e6a1c59b1d6dba5e42', 'Admin'),
('manager@gmail.com', 'manager', 'dd41d0ea83f4e66f20c1dfb810b5d3381aacee098dd4aafb1a80b34287b166370587600aa372b4baafb70d61b23ce86083a12cf9e55e60a82fb6074f86a94123', 'c4f76cf3254c9f4cebc7bb08ff8abaf74bb1fe1b08df5fe5aee001103bbebcb750b2b4408430e16344a40b4517b60bc22be411aaf623cc797feac8563f5dc49127b1b8c14dee82fd18700ce4d91db7fbd5fb61757365c4788c620fe59c60da484bc96fa108da8b7fb8369d599b5346a671659d9874c065aaed23a8adf1aab583', 'Manager'),
('moderator@gmail.com', 'moderator', 'c73e4eba5702ded60e366c5eef05f3639f0e67c51122b0c114ef9506d18c5a8600148ce8814745c3613705f705f4d7ec850650209cfed023b078188e5130b797', '47291abfc98bb8d7ec343f784b5c3201337f358a789d7f5c5636e825b3e7a8c47e6abce51e1d6bcb68347d4ffd270bb97fb9e58af9303a0f96aed7fd96ca7735dbebc40e7ceda806599fbe85689811d84d9147ca7f3e47c42837015cca5b2eba8853fdfb63136338c61b566ec8fcc913137bb4d4c51f05e6a1c59b1d6dba5e42', 'Moderator'),
('vulns@gmail.com', 'vulns', '609a322daf0500f7bd4673ccda0635825e544c09ad10ac681645b7f8dacf398e5f17c61f95b2fe2e16172fb218e5d844f7533b3371e58999ebdd638419b481eb', '7301c8128b2f7eeb6f0ea1e224acb0d732ef49c896aac413440e4f16208135cb69e9939fc84230875b3035509bab78e8e2ef32076763c27429fa7e352f2f922ee8f7634b51c6a89d67f10cbf4f0f7058e3a5f14c4b8c221a45f287871bd7488990aa037207f34cee804640f59b12142a13c044b287eeac9d935304ad5d1ec48a', 'Examiner');

-- 2) Semesters
INSERT INTO Semester (SemesterCode, StartDate, EndDate, IsActive, CreateBy)
VALUES
('SU25','2025-05-12','2025-09-06', TRUE, 1);


-- 3) Subjects
INSERT INTO Subject (SubjectName)
VALUES
('PRN222');

-- 4) Semester_Subject
INSERT INTO Semester_Subject (SemesterId, SubjectId)
VALUES
(1, 1);

-- 5) Groups
INSERT INTO "Group" (GroupName, SemesterId, CreateBy)
VALUES
('SE1704', 1, 1),
('SE1707', 1, 1),
('SE1712', 1, 1);

-- 6) Students
-- Map Groups: SE1704=1, SE1707=2, SE1712=3
INSERT INTO Student (StudentFullName, StudentRoll)
VALUES
('Đỗ Long Ánh', 'SE181818'),
('Nguyễn Hoàng Mai Anh', 'SE181874'),
('Nguyễn Anh', 'SE171217'),
('Phạm Thị Hải Anh', 'SE171207'),
('Hoàng Quốc An', 'SE181520'),
('Trần Tuấn Anh', 'SE172616'),
('Lê Quang An', 'SE150619'),
('Trần Vĩnh An', 'SE184967'),
('Quách Gia Bảo', 'SE180449'),
('Nguyễn Phạm Thanh Bình', 'SE181532'),
('Nguyễn Thanh Bình', 'SE181790'),
('Nguyễn Phú Cường', 'SE173621'),
('Lại Vũ Hải Đăng', 'SE151369'),
('Phạm Thế Danh', 'SE184514'),
('Phạm Thị Anh Đào', 'SE181924'),
('Nguyễn Quốc Đạt', 'SE184502'),
('Vũ Minh Đạt', 'SE184665'),
('Phan Thanh Đức', 'SE150708'),
('Trịnh Hải Đức', 'SE184622'),
('Nguyễn Mạnh Dưỡng', 'SE181515'),
('Phan Khánh Dương', 'SE180524'),
('Nguyễn Thái Duy', 'SE170601'),
('Trần Hạ Khương Duy', 'QE180075'),
('Trần Thiện Duy', 'SE184596'),
('Phan Hoàng Ngọc Hân', 'SE184307'),
('Vũ Đậu Thành Hoàng', 'SE160556'),
('Nguyễn Huy Thiên Hòa', 'SE185071'),
('Nguyễn Hàng Nhật Huy', 'SE170046'),
('Nguyễn Quang Huy', 'SE181563'),
('Lê Tuấn Khanh', 'SE184638'),
('Trần Nam Khánh', 'SE184544'),
('Nguyễn Văn Duy Khiêm', 'SE180168'),
('Lê Trung Anh Khôi', 'SE180591'),
('Nguyễn Lâm Minh Khôi', 'SE182845'),
('Phạm Minh Khôi', 'SE171989'),
('Nguyễn Trung Kiên', 'SE170416'),
('Lu Tử Kiệt', 'SE184654'),
('Trần Gia Kiệt', 'SE180500'),
('Trương Tuấn Kiệt', 'SE185063'),
('Trần Văn Lâm', 'SE173173'),
('Nguyễn Trình Cát Linh', 'SE181858'),
('Võ Gia Linh', 'SE181523'),
('Lê Hồ Hoàng Long', 'SE181754'),
('Nguyễn Hoàng Long', 'SE172340'),
('Nguyễn Hoàng Long', 'SE172388'),
('Nguyễn Phi Long', 'SE181672'),
('Trương Nguyễn Hoàng Long', 'SE173174'),
('Lý Hải Luân', 'SE172597'),
('Dương Đức Mạnh', 'SE173244'),
('Nguyễn Đức Mạnh', 'SE184730'),
('Nguyễn Khánh Minh', 'SE180188'),
('Nguyễn Lê Minh', 'SE183360'),
('Nguyễn Quang Minh', 'SE184796'),
('Trần Hoàng Nhật Minh', 'SE180461'),
('Mai Hải Nam', 'SE184557'),
('Nguyễn Đặng Phương Nam', 'SE171442'),
('Nguyễn Hiền Trung Nam', 'SE183876'),
('Nguyễn Thành Nam', 'SE170239'),
('Phan Thành Nam', 'SE180525'),
('Trương Lê Minh Nghĩa', 'QE170244'),
('Trần Đình Nguyên', 'SE172654'),
('Trần Phạm Thảo Nguyên', 'SE180486'),
('Nguyễn Phúc Nhân', 'SE184696'),
('Cao Hoàng Nhật', 'SE184713'),
('Huỳnh Trung Nhiên', 'SE173168'),
('Hỷ Minh Phát', 'SE184629'),
('Tăng Phát', 'SE181923'),
('Đặng Nhật Phi', 'SE171156'),
('Hoàng Gia Phong', 'SE180543'),
('Nguyễn Thanh Phong', 'SE181770'),
('Nguyễn Anh Quân', 'SE180619'),
('Lê Quang Thái Sơn', 'SE184097'),
('Bùi Minh Thắng', 'SE180564'),
('Nguyễn Thanh Thắng', 'SE170073'),
('Nguyễn Xuân Thắng', 'SE184639'),
('Trần Nhật Thắng', 'QE180046'),
('Nguyễn Trí Hoàng Thân', 'SE183895'),
('Đinh Lê Thịnh', 'SE181755'),
('Đỗ Trường Thịnh', 'SE183642'),
('Nguyễn Đức Thịnh', 'SE184492'),
('Nguyễn Phong Thịnh', 'SE172990'),
('Trần Đình Thịnh', 'SE181531'),
('Phạm Nhất Thống', 'SE184826'),
('Nguyễn Thái Thuận', 'SE171716'),
('Trần Chí Thuận', 'SE184519'),
('Hoàng Ngọc Tiến', 'SE181783'),
('Nguyễn Ngọc Tiến', 'SE181773'),
('Lê Hữu Thành Tín', 'SE180481'),
('Nguyễn Ngọc Toàn', 'SE171130'),
('Trần Thị Thanh Trang', 'SE180491'),
('Nguyễn Cao Trí', 'SE181729'),
('Nguyễn Thanh Trí', 'SE182028'),
('Hoàng Chí Trung', 'SE181597'),
('Lê Trung', 'SE173223'),
('Mai Tiến Trung', 'SE172199'),
('Nguyễn Minh Trung', 'SE184811'),
('Nguyễn Nhật Trường', 'SE184834'),
('Phạm Minh Trường', 'SE182027'),
('Nguyễn Anh Tuấn', 'SE172510'),
('Nguyễn Văn Triệu Tuấn', 'SE161029'),
('Nguyễn Việt', 'SE180672'),
('Nguyễn Đức Hoàng Vũ', 'SE181551'),
('Trương Tuấn Vũ', 'SE184370'),
('Nguyễn Tuấn Lộc', 'SE160990'),
('Nguyễn Mai Thành Nam', 'SE170240'),
('Phạm Đình Quốc Thịnh', 'SE171589');



-- 7) Group_Student
-- SE1712 = 3
INSERT INTO Group_Student (GroupId, StudentId)
VALUES
(3, 1),
(3, 7),
(3, 8),
(3, 12),
(3, 13),
(3, 16),
(3, 17),
(3, 18),
(3, 19),
(3, 23),
(3, 27),
(3, 30),
(3, 33),
(3, 34),
(3, 37),
(3, 49),
(3, 50),
(3, 51),
(3, 53),
(3, 55),
(3, 57),
(3, 59),
(3, 60),
(3, 63),
(3, 64),
(3, 65),
(3, 66),
(3, 72),
(3, 75),
(3, 83),
(3, 97),
(3, 99),
(3, 100),
(3, 103),
(3, 106);

-- SE1707 = 2
INSERT INTO Group_Student (GroupId, StudentId)
VALUES
(2, 2),
(2, 5),
(2, 9),
(2, 10),
(2, 11),
(2, 20),
(2, 21),
(2, 29),
(2, 32),
(2, 38),
(2, 41),
(2, 42),
(2, 43),
(2, 46),
(2, 54),
(2, 67),
(2, 70),
(2, 71),
(2, 73),
(2, 76),
(2, 78),
(2, 79),
(2, 82),
(2, 86),
(2, 87),
(2, 88),
(2, 90),
(2, 91),
(2, 92),
(2, 93),
(2, 101),
(2, 102);

-- SE1704 = 1
INSERT INTO Group_Student (GroupId, StudentId)
VALUES
(1, 3),
(1, 4),
(1, 6),
(1, 14),
(1, 22),
(1, 24),
(1, 25),
(1, 26),
(1, 28),
(1, 31),
(1, 35),
(1, 36),
(1, 40),
(1, 44),
(1, 45),
(1, 47),
(1, 48),
(1, 52),
(1, 56),
(1, 58),
(1, 61),
(1, 64),
(1, 74),
(1, 77),
(1, 89),
(1, 94),
(1, 95),
(1, 96),
(1, 98),
(1, 104),
(1, 105);



-- 8) Exams
INSERT INTO Exam (SemesterId, SubjectId, ExamName, ExamDate)
VALUES
(1, 1, 'PE_PRN222_SU25_332278', '2025-08-30');

