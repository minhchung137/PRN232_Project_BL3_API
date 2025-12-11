using AutoMapper;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Services.BusinessModel;
namespace PRN232_GradingSystem_Services.Helpers
{
    public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            CreateMap<Exam, ExamBM>()
                .ForMember(dest => dest.Examid, opt => opt.MapFrom(src => src.Examid))
                .ForMember(dest => dest.Semesterid, opt => opt.MapFrom(src => src.Semesterid))
                .ForMember(dest => dest.Subjectid, opt => opt.MapFrom(src => src.Subjectid))
                .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
                //.ForMember(dest => dest.Submissions, opt => opt.MapFrom(src => src.Submissions))
                .ReverseMap()
                .ForMember(dest => dest.Semester, opt => opt.Ignore()) // tránh vòng lặp navigation
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.Submissions, opt => opt.Ignore());

            CreateMap<Grade, GradeBM>().ReverseMap()
        .ForMember(dest => dest.Submission, opt => opt.Ignore())
        .ForMember(dest => dest.MarkerNavigation, opt => opt.Ignore())
        .ForMember(dest => dest.Gradedetails, opt => opt.Ignore());

            CreateMap<Gradedetail, GradedetailBM>().ReverseMap()
        .ForMember(dest => dest.Grade, opt => opt.Ignore());

            CreateMap<Subject, SubjectBM>().ReverseMap()
        .ForMember(dest => dest.Exams, opt => opt.Ignore())
        .ForMember(dest => dest.SemesterSubjects, opt => opt.Ignore());


            CreateMap<Submission, SubmissionBM>().ReverseMap()
        .ForMember(dest => dest.Exam, opt => opt.Ignore())
        .ForMember(dest => dest.Student, opt => opt.Ignore())
        .ForMember(dest => dest.Grades, opt => opt.Ignore());

            CreateMap<Group, GroupBM>().ReverseMap()
        .ForMember(dest => dest.Semester, opt => opt.Ignore())
        .ForMember(dest => dest.GroupStudents, opt => opt.Ignore())
        .ForMember(dest => dest.CreatebyNavigation, opt => opt.Ignore())
        .ForMember(dest => dest.UpdatebyNavigation, opt => opt.Ignore());


            CreateMap<GroupStudent, GroupStudentBM>().ReverseMap()
        .ForMember(dest => dest.Group, opt => opt.Ignore())
        .ForMember(dest => dest.Student, opt => opt.Ignore());

            CreateMap<SemesterBM, Semester>()
      .ForMember(dest => dest.Semesterid, opt => opt.Ignore()) // KHÓA CHÍNH
      .ForMember(dest => dest.Exams, opt => opt.Ignore())
      .ForMember(dest => dest.Groups, opt => opt.Ignore())
      .ForMember(dest => dest.SemesterSubjects, opt => opt.Ignore());

            CreateMap<Semester, SemesterBM>()
                .ForMember(dest => dest.Exams, opt => opt.Ignore())
      .ForMember(dest => dest.Groups, opt => opt.Ignore())
      .ForMember(dest => dest.SemesterSubjects, opt => opt.Ignore());


            CreateMap<SemesterSubject, SemesterSubjectBM>().ReverseMap()
        .ForMember(dest => dest.Semester, opt => opt.Ignore())
        .ForMember(dest => dest.Subject, opt => opt.Ignore());

            CreateMap<User, UserBM>().ReverseMap()
        .ForMember(dest => dest.Grades, opt => opt.Ignore())
        .ForMember(dest => dest.GroupCreatebyNavigations, opt => opt.Ignore())
        .ForMember(dest => dest.GroupUpdatebyNavigations, opt => opt.Ignore());

            CreateMap<Student, StudentBM>().ReverseMap()
                .ForMember(dest => dest.Submissions, opt => opt.Ignore())
                .ForMember(dest => dest.GroupStudents, opt => opt.Ignore());


        }
    }
}
