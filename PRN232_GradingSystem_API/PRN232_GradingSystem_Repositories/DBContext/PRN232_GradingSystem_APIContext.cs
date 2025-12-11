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

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<GradeDetail> GradeDetails { get; set; }

    public virtual DbSet<GroupStudent> GroupStudents { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<SemesterSubject> SemesterSubjects { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Submission> Submissions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=prn232_management_db;Username=postgres;Password=12345");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("app_user_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<ClassGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("class_group_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ClassGroupCreatedByNavigations).HasConstraintName("class_group_created_by_fkey");

            entity.HasOne(d => d.Semester).WithMany(p => p.ClassGroups).HasConstraintName("class_group_semester_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ClassGroupUpdatedByNavigations).HasConstraintName("class_group_updated_by_fkey");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("exam_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Semester).WithMany(p => p.Exams).HasConstraintName("exam_semester_id_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.Exams).HasConstraintName("exam_subject_id_fkey");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("grade_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.GradeCount).HasDefaultValue(1);

            entity.HasOne(d => d.Marker).WithMany(p => p.Grades).HasConstraintName("grade_marker_id_fkey");

            entity.HasOne(d => d.Submission).WithMany(p => p.Grades).HasConstraintName("grade_submission_id_fkey");
        });

        modelBuilder.Entity<GradeDetail>(entity =>
        {
            entity.HasKey(e => e.GradeDetailId).HasName("grade_detail_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Grade).WithMany(p => p.GradeDetails).HasConstraintName("grade_detail_grade_id_fkey");
        });

        modelBuilder.Entity<GroupStudent>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.StudentId }).HasName("group_student_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupStudents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("group_student_group_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.GroupStudents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("group_student_student_id_fkey");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.SemesterId).HasName("semester_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SemesterCreatedByNavigations).HasConstraintName("semester_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SemesterUpdatedByNavigations).HasConstraintName("semester_updated_by_fkey");
        });

        modelBuilder.Entity<SemesterSubject>(entity =>
        {
            entity.HasKey(e => new { e.SemesterId, e.SubjectId }).HasName("semester_subject_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Semester).WithMany(p => p.SemesterSubjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("semester_subject_semester_id_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.SemesterSubjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("semester_subject_subject_id_fkey");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("student_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("subject_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("submission_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Exam).WithMany(p => p.Submissions).HasConstraintName("submission_exam_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.Submissions).HasConstraintName("submission_student_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
