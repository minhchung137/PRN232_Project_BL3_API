using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_Repositories.DBContext;

public partial class PRN232_GradingSystem_APIContext : DbContext
{
    public PRN232_GradingSystem_APIContext()
    {
    }

    public PRN232_GradingSystem_APIContext(DbContextOptions<PRN232_GradingSystem_APIContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<ClassGroup> ClassGroups { get; set; }

    public virtual DbSet<Criterion> Criteria { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<GradeDetail> GradeDetails { get; set; }

    public virtual DbSet<GroupStudent> GroupStudents { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<SemesterSubject> SemesterSubjects { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Submission> Submissions { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=grading_db;Username=postgres;Password=12345");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("app_user_pkey");

            entity.ToTable("app_user");

            entity.HasIndex(e => e.Email, "app_user_email_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.RefreshToken).HasColumnName("refresh_token");
            entity.Property(e => e.RefreshTokenExpiryTime).HasColumnName("refresh_token_expiry_time");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
            entity.Property(e => e.Salt).HasColumnName("salt");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<ClassGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("class_group_pkey");

            entity.ToTable("class_group");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .HasColumnName("group_name");
            entity.Property(e => e.SemesterId).HasColumnName("semester_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ClassGroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("class_group_created_by_fkey");

            entity.HasOne(d => d.Semester).WithMany(p => p.ClassGroups)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("class_group_semester_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ClassGroupUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("class_group_updated_by_fkey");
        });

        modelBuilder.Entity<Criterion>(entity =>
        {
            entity.HasKey(e => e.CriteriaId).HasName("criteria_pkey");

            entity.ToTable("criteria");

            entity.Property(e => e.CriteriaId).HasColumnName("criteria_id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.IsManual)
                .HasDefaultValue(true)
                .HasColumnName("is_manual");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.Weight)
                .HasPrecision(5, 2)
                .HasColumnName("weight");

            entity.HasOne(d => d.Question).WithMany(p => p.Criteria)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("criteria_question_id_fkey");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("exam_pkey");

            entity.ToTable("exam");

            entity.Property(e => e.ExamId).HasColumnName("exam_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.ExamDate).HasColumnName("exam_date");
            entity.Property(e => e.ExamName)
                .HasMaxLength(255)
                .HasColumnName("exam_name");
            entity.Property(e => e.SemesterId).HasColumnName("semester_id");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.Semester).WithMany(p => p.Exams)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("exam_semester_id_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.Exams)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("exam_subject_id_fkey");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("grade_pkey");

            entity.ToTable("grade");

            entity.Property(e => e.GradeId).HasColumnName("grade_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.GradeCount)
                .HasDefaultValue(1)
                .HasColumnName("grade_count");
            entity.Property(e => e.MarkerId).HasColumnName("marker_id");
            entity.Property(e => e.PlagiarismScore)
                .HasPrecision(5, 2)
                .HasColumnName("plagiarism_score");
            entity.Property(e => e.Q1)
                .HasPrecision(5, 2)
                .HasColumnName("q1");
            entity.Property(e => e.Q2)
                .HasPrecision(5, 2)
                .HasColumnName("q2");
            entity.Property(e => e.Q3)
                .HasPrecision(5, 2)
                .HasColumnName("q3");
            entity.Property(e => e.Q4)
                .HasPrecision(5, 2)
                .HasColumnName("q4");
            entity.Property(e => e.Q5)
                .HasPrecision(5, 2)
                .HasColumnName("q5");
            entity.Property(e => e.Q6)
                .HasPrecision(5, 2)
                .HasColumnName("q6");
            entity.Property(e => e.Q7)
                .HasPrecision(5, 2)
                .HasColumnName("q7");
            entity.Property(e => e.Q8)
                .HasPrecision(5, 2)
                .HasColumnName("q8");
            entity.Property(e => e.Q9)
                .HasPrecision(5, 2)
                .HasColumnName("q9");
            entity.Property(e => e.Q10)
                .HasPrecision(5, 2)
                .HasColumnName("q10");
            entity.Property(e => e.Q11)
                .HasPrecision(5, 2)
                .HasColumnName("q11");
            entity.Property(e => e.Q12)
                .HasPrecision(5, 2)
                .HasColumnName("q12");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.TotalScore)
                .HasPrecision(5, 2)
                .HasColumnName("total_score");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Marker).WithMany(p => p.Grades)
                .HasForeignKey(d => d.MarkerId)
                .HasConstraintName("grade_marker_id_fkey");

            entity.HasOne(d => d.Submission).WithMany(p => p.Grades)
                .HasForeignKey(d => d.SubmissionId)
                .HasConstraintName("grade_submission_id_fkey");
        });

        modelBuilder.Entity<GradeDetail>(entity =>
        {
            entity.HasKey(e => e.GradeDetailId).HasName("grade_detail_pkey");

            entity.ToTable("grade_detail");

            entity.Property(e => e.GradeDetailId).HasColumnName("grade_detail_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CriteriaId).HasColumnName("criteria_id");
            entity.Property(e => e.GradeId).HasColumnName("grade_id");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.Point)
                .HasPrecision(5, 2)
                .HasColumnName("point");
            entity.Property(e => e.QCode)
                .HasMaxLength(10)
                .HasColumnName("q_code");
            entity.Property(e => e.SubCode)
                .HasMaxLength(50)
                .HasColumnName("sub_code");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Criteria).WithMany(p => p.GradeDetails)
                .HasForeignKey(d => d.CriteriaId)
                .HasConstraintName("grade_detail_criteria_id_fkey");

            entity.HasOne(d => d.Grade).WithMany(p => p.GradeDetails)
                .HasForeignKey(d => d.GradeId)
                .HasConstraintName("grade_detail_grade_id_fkey");
        });

        modelBuilder.Entity<GroupStudent>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.StudentId }).HasName("group_student_pkey");

            entity.ToTable("group_student");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupStudents)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("group_student_group_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.GroupStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("group_student_student_id_fkey");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("question_pkey");

            entity.ToTable("question");

            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ExamId).HasColumnName("exam_id");
            entity.Property(e => e.MaxScore)
                .HasPrecision(5, 2)
                .HasColumnName("max_score");
            entity.Property(e => e.QCode)
                .HasMaxLength(10)
                .HasColumnName("q_code");

            entity.HasOne(d => d.Exam).WithMany(p => p.Questions)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("question_exam_id_fkey");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.SemesterId).HasName("semester_pkey");

            entity.ToTable("semester");

            entity.HasIndex(e => e.SemesterCode, "semester_semester_code_key").IsUnique();

            entity.Property(e => e.SemesterId).HasColumnName("semester_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.SemesterCode)
                .HasMaxLength(50)
                .HasColumnName("semester_code");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SemesterCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("semester_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SemesterUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("semester_updated_by_fkey");
        });

        modelBuilder.Entity<SemesterSubject>(entity =>
        {
            entity.HasKey(e => new { e.SemesterId, e.SubjectId }).HasName("semester_subject_pkey");

            entity.ToTable("semester_subject");

            entity.Property(e => e.SemesterId).HasColumnName("semester_id");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Semester).WithMany(p => p.SemesterSubjects)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("semester_subject_semester_id_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.SemesterSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("semester_subject_subject_id_fkey");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("student_pkey");

            entity.ToTable("student");

            entity.HasIndex(e => e.StudentRoll, "student_student_roll_key").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.StudentFullname)
                .HasMaxLength(100)
                .HasColumnName("student_fullname");
            entity.Property(e => e.StudentRoll)
                .HasMaxLength(20)
                .HasColumnName("student_roll");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("subject_pkey");

            entity.ToTable("subject");

            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(100)
                .HasColumnName("subject_name");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("submission_pkey");

            entity.ToTable("submission");

            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.ExamId).HasColumnName("exam_id");
            entity.Property(e => e.FileUrl).HasColumnName("file_url");
            entity.Property(e => e.Solution).HasColumnName("solution");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Exam).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("submission_exam_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("submission_student_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
