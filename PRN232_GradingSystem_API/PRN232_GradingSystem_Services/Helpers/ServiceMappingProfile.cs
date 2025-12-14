using AutoMapper;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Services.BusinessModel;

namespace PRN232_GradingSystem_Services.Helpers
{
    public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            // 1. MAPPING EXAM
            CreateMap<Exam, ExamBM>()
                .ForMember(dest => dest.Examid, opt => opt.MapFrom(src => src.ExamId))
                .ForMember(dest => dest.Semesterid, opt => opt.MapFrom(src => src.SemesterId))
                .ForMember(dest => dest.Subjectid, opt => opt.MapFrom(src => src.SubjectId))
                .ReverseMap()
                .ForMember(dest => dest.Semester, opt => opt.Ignore())
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.Submissions, opt => opt.Ignore());

            // 2. MAPPING GRADE (SỬA LỖI CHÍNH Ở ĐÂY)
            CreateMap<Grade, GradeBM>()
                .ForMember(dest => dest.Gradeid, opt => opt.MapFrom(src => src.GradeId))
                .ForMember(dest => dest.Submissionid, opt => opt.MapFrom(src => src.SubmissionId))
                // Map ID: Grade.MarkerId (Entity) -> GradeBM.Marker (int)
                .ForMember(dest => dest.Marker, opt => opt.MapFrom(src => src.MarkerId))
                // Map Navigation: Grade.Marker (Entity) -> GradeBM.MarkerNavigation
                .ForMember(dest => dest.MarkerNavigation, opt => opt.MapFrom(src => src.Marker))
                .ReverseMap()
                .ForMember(dest => dest.Submission, opt => opt.Ignore())
                // Khi map ngược: GradeBM.Marker (int) -> Grade.MarkerId
                .ForMember(dest => dest.MarkerId, opt => opt.MapFrom(src => src.Marker))
                // Bỏ qua Navigation khi map ngược để tránh lỗi
                .ForMember(dest => dest.Marker, opt => opt.Ignore())
                .ForMember(dest => dest.GradeDetails, opt => opt.Ignore());

            // 3. MAPPING GRADEDETAIL
            CreateMap<GradeDetail, GradedetailBM>()
                .ForMember(dest => dest.Gradedetailid, opt => opt.MapFrom(src => src.GradeDetailId))
                .ForMember(dest => dest.Gradeid, opt => opt.MapFrom(src => src.GradeId))
                .ReverseMap()
                .ForMember(dest => dest.Grade, opt => opt.Ignore());

            // 4. MAPPING SUBJECT
            CreateMap<Subject, SubjectBM>()
                .ForMember(dest => dest.Subjectid, opt => opt.MapFrom(src => src.SubjectId))
                .ReverseMap()
                .ForMember(dest => dest.Exams, opt => opt.Ignore())
                .ForMember(dest => dest.SemesterSubjects, opt => opt.Ignore());

            // 5. MAPPING SUBMISSION
            CreateMap<Submission, SubmissionBM>()
                .ForMember(dest => dest.Submissionid, opt => opt.MapFrom(src => src.SubmissionId))
                .ForMember(dest => dest.Examid, opt => opt.MapFrom(src => src.ExamId))
                .ForMember(dest => dest.Studentid, opt => opt.MapFrom(src => src.StudentId))
                .ReverseMap()
                .ForMember(dest => dest.Exam, opt => opt.Ignore())
                .ForMember(dest => dest.Student, opt => opt.Ignore())
                .ForMember(dest => dest.Grades, opt => opt.Ignore());

            // 6. MAPPING CLASSGROUP -> GROUPBM (Sửa Group -> ClassGroup)
            CreateMap<ClassGroup, GroupBM>()
                .ForMember(dest => dest.Groupid, opt => opt.MapFrom(src => src.GroupId))
                .ForMember(dest => dest.Semesterid, opt => opt.MapFrom(src => src.SemesterId))
                // Map Navigation CreateBy/UpdateBy từ ClassGroup sang BM
                .ForMember(dest => dest.CreatebyNavigation, opt => opt.MapFrom(src => src.CreatedByNavigation))
                .ForMember(dest => dest.UpdatebyNavigation, opt => opt.MapFrom(src => src.UpdatedByNavigation))
                .ReverseMap()
                .ForMember(dest => dest.Semester, opt => opt.Ignore())
                .ForMember(dest => dest.GroupStudents, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedByNavigation, opt => opt.Ignore());

            // 7. MAPPING GROUPSTUDENT
            CreateMap<GroupStudent, GroupStudentBM>()
                .ForMember(dest => dest.Groupid, opt => opt.MapFrom(src => src.GroupId))
                .ForMember(dest => dest.Studentid, opt => opt.MapFrom(src => src.StudentId))
                .ReverseMap()
                .ForMember(dest => dest.Group, opt => opt.Ignore())
                .ForMember(dest => dest.Student, opt => opt.Ignore());

            // 8. MAPPING SEMESTER
            CreateMap<Semester, SemesterBM>()
                .ForMember(dest => dest.Semesterid, opt => opt.MapFrom(src => src.SemesterId))
                // Entity dùng ClassGroups, BM dùng Groups
                .ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.ClassGroups))
                .ReverseMap()
                .ForMember(dest => dest.Exams, opt => opt.Ignore())
                .ForMember(dest => dest.ClassGroups, opt => opt.Ignore())
                .ForMember(dest => dest.SemesterSubjects, opt => opt.Ignore());

            // 9. MAPPING SEMESTERSUBJECT
            CreateMap<SemesterSubject, SemesterSubjectBM>()
                .ForMember(dest => dest.Semesterid, opt => opt.MapFrom(src => src.SemesterId))
                .ForMember(dest => dest.Subjectid, opt => opt.MapFrom(src => src.SubjectId))
                .ReverseMap()
                .ForMember(dest => dest.Semester, opt => opt.Ignore())
                .ForMember(dest => dest.Subject, opt => opt.Ignore());

            // 10. MAPPING APPUSER -> USERBM (Sửa User -> AppUser)
            CreateMap<AppUser, UserBM>()
                .ForMember(dest => dest.Userid, opt => opt.MapFrom(src => src.UserId))
                .ReverseMap()
                .ForMember(dest => dest.Grades, opt => opt.Ignore())
                // Model mới dùng ClassGroupCreatedBy... thay vì GroupCreateby...
                .ForMember(dest => dest.ClassGroupCreatedByNavigations, opt => opt.Ignore())
                .ForMember(dest => dest.ClassGroupUpdatedByNavigations, opt => opt.Ignore())
                .ForMember(dest => dest.SemesterCreatedByNavigations, opt => opt.Ignore())
                .ForMember(dest => dest.SemesterUpdatedByNavigations, opt => opt.Ignore());

            // 11. MAPPING STUDENT
            CreateMap<Student, StudentBM>()
                .ForMember(dest => dest.Studentid, opt => opt.MapFrom(src => src.StudentId))
                .ReverseMap()
                .ForMember(dest => dest.Submissions, opt => opt.Ignore())
                .ForMember(dest => dest.GroupStudents, opt => opt.Ignore());
            
            // 12. MAPPING QUESTION
            CreateMap<Question, QuestionBM>()
                .ForMember(dest => dest.Questionid, opt => opt.MapFrom(src => src.QuestionId))
                .ForMember(dest => dest.Examid, opt => opt.MapFrom(src => src.ExamId))
                .ForMember(dest => dest.Qcode, opt => opt.MapFrom(src => src.QCode))
                .ForMember(dest => dest.Maxscore, opt => opt.MapFrom(src => src.MaxScore))
                .ReverseMap()
                .ForMember(dest => dest.Exam, opt => opt.Ignore())          // tránh vòng lặp (nếu có nav)
                .ForMember(dest => dest.Criteria, opt => opt.Ignore());     // nếu Question có ICollection<Criterion>

            // 13. MAPPING CRITERIA (Entity là Criterion)
            CreateMap<Criterion, CriteriaBM>()
                .ForMember(dest => dest.Criteriaid, opt => opt.MapFrom(src => src.CriteriaId))
                .ForMember(dest => dest.Questionid, opt => opt.MapFrom(src => src.QuestionId))
                .ForMember(dest => dest.Ismanual, opt => opt.MapFrom(src => src.IsManual))
                .ForMember(dest => dest.Orderindex, opt => opt.MapFrom(src => src.OrderIndex))
                .ReverseMap()
                .ForMember(dest => dest.Question, opt => opt.Ignore())      // tránh vòng lặp (nếu có nav)
                .ForMember(dest => dest.GradeDetails, opt => opt.Ignore());
            
        }
    }
}