-- ================================================================
-- PHẦN 1: CLEANUP (XÓA BẢNG CŨ ĐỂ CHẠY LẠI TỪ ĐẦU)
-- ================================================================
DROP TABLE IF EXISTS grade_detail CASCADE;
DROP TABLE IF EXISTS grade CASCADE;
DROP TABLE IF EXISTS submission CASCADE;
DROP TABLE IF EXISTS exam CASCADE;
DROP TABLE IF EXISTS group_student CASCADE;
DROP TABLE IF EXISTS class_group CASCADE; -- Đổi tên từ 'Group' vì trùng keyword
DROP TABLE IF EXISTS semester_subject CASCADE;
DROP TABLE IF EXISTS student CASCADE;
DROP TABLE IF EXISTS semester CASCADE;
DROP TABLE IF EXISTS subject CASCADE;
DROP TABLE IF EXISTS app_user CASCADE; -- Đổi tên từ 'User' vì trùng keyword

-- ================================================================
-- PHẦN 2: TẠO BẢNG (SCHEMA)
-- ================================================================

-- 1. Users (Admin, Teacher, Moderator)
CREATE TABLE app_user (
    user_id SERIAL PRIMARY KEY,
    email VARCHAR(255) NOT NULL UNIQUE,
    username VARCHAR(50) NOT NULL,
    password BYTEA, -- Lưu binary password
    salt BYTEA,
    role VARCHAR(50), -- Admin, Teacher, Moderator, Student
    refresh_token TEXT,
    refresh_token_expiry_time TIMESTAMPTZ,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- 2. Semester
CREATE TABLE semester (
    semester_id SERIAL PRIMARY KEY,
    semester_code VARCHAR(50) NOT NULL UNIQUE, -- SU24, FA24
    start_date TIMESTAMPTZ,
    end_date TIMESTAMPTZ,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    created_by INT REFERENCES app_user(user_id),
    updated_by INT REFERENCES app_user(user_id)
);

-- 3. Subject
CREATE TABLE subject (
    subject_id SERIAL PRIMARY KEY,
    subject_name VARCHAR(100) NOT NULL,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- 4. Semester_Subject (Many-to-Many)
CREATE TABLE semester_subject (
    semester_id INT REFERENCES semester(semester_id),
    subject_id INT REFERENCES subject(subject_id),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    PRIMARY KEY (semester_id, subject_id)
);

-- 5. Class Group (Lớp học)
CREATE TABLE class_group (
    group_id SERIAL PRIMARY KEY,
    group_name VARCHAR(50) NOT NULL,
    semester_id INT REFERENCES semester(semester_id),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    created_by INT REFERENCES app_user(user_id),
    updated_by INT REFERENCES app_user(user_id)
);

-- 6. Student
CREATE TABLE student (
    student_id SERIAL PRIMARY KEY,
    student_fullname VARCHAR(100),
    student_roll VARCHAR(20) NOT NULL UNIQUE, -- HE150001
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- 7. Group_Student
CREATE TABLE group_student (
    group_id INT REFERENCES class_group(group_id),
    student_id INT REFERENCES student(student_id),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    PRIMARY KEY (group_id, student_id)
);

-- 8. Exam
CREATE TABLE exam (
    exam_id SERIAL PRIMARY KEY,
    semester_id INT REFERENCES semester(semester_id),
    subject_id INT REFERENCES subject(subject_id),
    exam_name VARCHAR(255),
    exam_date TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- 9. Submission
CREATE TABLE submission (
    submission_id SERIAL PRIMARY KEY,
    exam_id INT REFERENCES exam(exam_id),
    student_id INT REFERENCES student(student_id),
    solution TEXT,
    comment TEXT,
    file_url TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ
);

-- 10. Grade (QUAN TRỌNG NHẤT)
CREATE TABLE grade (
    grade_id SERIAL PRIMARY KEY,
    submission_id INT REFERENCES submission(submission_id),
    
    -- Các cột điểm cứng của hệ thống cũ
    q1 NUMERIC(5,2),
    q2 NUMERIC(5,2),
    q3 NUMERIC(5,2),
    q4 NUMERIC(5,2),
    q5 NUMERIC(5,2),
    q6 NUMERIC(5,2),
    total_score NUMERIC(5,2),
    
    status VARCHAR(50), -- TeacherVerified, ModeratorApproved
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    marker_id INT REFERENCES app_user(user_id),

    -- CỘT MỚI CHO MODERATOR
    grade_count INT DEFAULT 1,     -- 1: Teacher chấm, 2: Moderator chấm
    plagiarism_score NUMERIC(5,2)  -- 0 -> 100%
);

-- 11. GradeDetail
CREATE TABLE grade_detail (
    grade_detail_id SERIAL PRIMARY KEY,
    grade_id INT REFERENCES grade(grade_id),
    q_code VARCHAR(10), -- Q1, Q2
    sub_code VARCHAR(50), -- Login, CRUD
    point NUMERIC(5,2),
    note TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ
);

-- ================================================================
-- PHẦN 3: SEED DATA (DỮ LIỆU MẪU)
-- ================================================================

-- 1. Tạo Users
INSERT INTO app_user (email, username, role, is_active) VALUES
('admin@fpt.edu.vn', 'admin', 'Admin', true),
('teacher@fpt.edu.vn', 'teacher', 'Teacher', true),     -- ID 2
('moderator@fpt.edu.vn', 'mod', 'Moderator', true);      -- ID 3

-- 2. Tạo Semester & Subject
INSERT INTO semester (semester_code, start_date, end_date, created_by) 
VALUES ('SU24', '2024-05-01', '2024-08-31', 2);

INSERT INTO subject (subject_name) VALUES ('PRN232');

INSERT INTO semester_subject (semester_id, subject_id) VALUES (1, 1);

-- 3. Tạo Class Group
INSERT INTO class_group (group_name, semester_id, created_by) VALUES ('SE1601', 1, 2);

-- 4. Tạo Students
INSERT INTO student (student_fullname, student_roll) VALUES
('Nguyen Van A', 'HE150001'), -- Case 1: Bình thường
('Tran Van B', 'HE150002'),   -- Case 2: Đạo văn
('Le Thi C', 'HE150003');     -- Case 3: Đã Review

INSERT INTO group_student (group_id, student_id) VALUES (1, 1), (1, 2), (1, 3);

-- 5. Tạo Exam
INSERT INTO exam (semester_id, subject_id, exam_name, exam_date) 
VALUES (1, 1, 'PRN232 Final Exam', NOW());

-- 6. TẠO SUBMISSION & GRADE (3 TÌNH HUỐNG CHO MODERATOR)

-- CASE 1: Bài cần Review (TeacherVerified, GradeCount = 1)
INSERT INTO submission (exam_id, student_id, file_url) VALUES (1, 1, 'https://cloud.../sv1.zip');
-- Insert Grade
INSERT INTO grade (submission_id, q1, total_score, status, marker_id, grade_count, plagiarism_score)
VALUES (1, 1.0, 8.0, 'TeacherVerified', 2, 1, 0.0);
-- Insert Detail
INSERT INTO grade_detail (grade_id, q_code, sub_code, point, note) 
VALUES (1, 'Q1', 'Login', 1.0, 'Good');


-- CASE 2: Bài nghi vấn Đạo văn (PlagiarismScore = 90%)
INSERT INTO submission (exam_id, student_id, file_url) VALUES (1, 2, 'https://cloud.../sv2.zip');
-- Insert Grade
INSERT INTO grade (submission_id, q1, total_score, status, marker_id, grade_count, plagiarism_score)
VALUES (2, 1.0, 9.0, 'TeacherVerified', 2, 1, 90.5); -- 90.5% đạo văn
-- Insert Detail
INSERT INTO grade_detail (grade_id, q_code, sub_code, point, note) 
VALUES (2, 'Q1', 'Login', 1.0, 'Code looks suspicious');


-- CASE 3: Bài ĐÃ Review xong (ModeratorApproved, GradeCount = 2)
INSERT INTO submission (exam_id, student_id, file_url) VALUES (1, 3, 'https://cloud.../sv3.zip');
-- Insert Grade (ID = 3)
INSERT INTO grade (submission_id, q1, total_score, status, marker_id, grade_count, plagiarism_score)
VALUES (3, 0.5, 7.5, 'ModeratorApproved', 3, 2, 5.0); -- GradeCount = 2, Marker = Moderator (ID 3)
-- Insert Detail
INSERT INTO grade_detail (grade_id, q_code, sub_code, point, note) 
VALUES (3, 'Q1', 'Login', 0.5, 'Moderator: Logic sai phần validate');

-- Kiểm tra kết quả
SELECT g.grade_id, s.student_roll, g.status, g.grade_count, g.plagiarism_score 
FROM grade g 
JOIN submission sub ON g.submission_id = sub.submission_id
JOIN student s ON sub.student_id = s.student_id;