using AutoMapper;
using PRN232_GradingSystem_API.Models.RequestModel;
using PRN232_GradingSystem_API.Models.ResponseModel;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;

namespace PRN232_GradingSystem_API.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // ================== BM -> Response ==================
            CreateMap<UserBM, UserResponse>();
            CreateMap<GradeBM, GradeResponse>();
            CreateMap<GradedetailBM, GradedetailResponse>();
            CreateMap<GroupBM, GroupResponse>();
            CreateMap<GroupStudentBM, GroupStudentResponse>();
            CreateMap<SemesterBM, SemesterResponse>();
            CreateMap<SemesterSubjectBM, SemesterSubjectResponse>();
            CreateMap<StudentBM, StudentResponse>();
            CreateMap<SubjectBM, SubjectResponse>();
            CreateMap<SubmissionBM, SubmissionResponse>();
            CreateMap<ExamBM, ExamResponse>();

            // ================== Request -> BM ==================
            CreateMap<UserRequest, UserBM>();
            CreateMap<GradeFilterRequest, GradeBM>();
            CreateMap<GradeRequest, GradeBM>();

            CreateMap<GradedetailFilterRequest, GradedetailBM>();
            CreateMap<GradedetailCreateRequest, GradedetailBM>();
            CreateMap<GradedetailUpdateRequestList, GradedetailUpdateRequestListBM>();
            CreateMap<GradedetailUpdateItem, GradedetailUpdateItemBM>();

            CreateMap<GradeWithDetailsRequest, GradeWithDetailsRequestBM>()
                .ForMember(dest => dest.Gradedetails, opt => opt.MapFrom(src => src.Gradedetails));



            CreateMap<GroupFilterRequest, GroupBM>();
            CreateMap<GroupCreateRequest, GroupBM>()
            .ForMember(dest => dest.Createat, opt => opt.Ignore())
            .ForMember(dest => dest.Updateat, opt => opt.Ignore())
            .ForMember(dest => dest.Groupid, opt => opt.Ignore())
            .ForMember(dest => dest.Updateby, opt => opt.Ignore())
            .ForMember(dest => dest.Semester, opt => opt.Ignore())
            .ForMember(dest => dest.CreatebyNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatebyNavigation, opt => opt.Ignore());

            CreateMap<GroupUpdateRequest, GroupBM>()
                .ForMember(dest => dest.Updateat, opt => opt.Ignore())
                .ForMember(dest => dest.Groupid, opt => opt.Ignore())
                .ForMember(dest => dest.Createat, opt => opt.Ignore())
                .ForMember(dest => dest.Createby, opt => opt.Ignore())
                .ForMember(dest => dest.Semester, opt => opt.Ignore())
                .ForMember(dest => dest.CreatebyNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatebyNavigation, opt => opt.Ignore());

            CreateMap<GroupStudentFilterRequest, GroupStudentBM>();
            CreateMap<GroupStudentRequest, GroupStudentBM>()
    .ForMember(dest => dest.Group, opt => opt.Ignore())
    .ForMember(dest => dest.Student, opt => opt.Ignore());

            CreateMap<SemesterFilterRequest, SemesterBM>();
            // Map khi tạo mới Semester
            CreateMap<SemesterCreateRequest, SemesterBM>()
                .ForMember(dest => dest.Createat, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Updateat, opt => opt.Ignore()) // chưa có
                .ForMember(dest => dest.Semesterid, opt => opt.Ignore())
                .ForMember(dest => dest.Exams, opt => opt.Ignore())
                .ForMember(dest => dest.Groups, opt => opt.Ignore())
                .ForMember(dest => dest.SemesterSubjects, opt => opt.Ignore());

            // Map khi cập nhật Semester
            CreateMap<SemesterUpdateRequest, SemesterBM>()
                    .ForMember(dest => dest.Semesterid, opt => opt.Ignore())
                .ForMember(dest => dest.Updateat, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Createat, opt => opt.Ignore())
                .ForMember(dest => dest.Createby, opt => opt.Ignore()) // không cho update
                .ForMember(dest => dest.Exams, opt => opt.Ignore())
                .ForMember(dest => dest.Groups, opt => opt.Ignore())
                .ForMember(dest => dest.SemesterSubjects, opt => opt.Ignore());

            CreateMap<SemesterSubjectFilterRequest, SemesterSubjectBM>();
            CreateMap<SemesterSubjectRequest, SemesterSubjectBM>()
                .ForMember(dest => dest.Semesterid, opt => opt.MapFrom(src => src.Semesterid))
                .ForMember(dest => dest.Subjectid, opt => opt.MapFrom(src => src.Subjectid))
                .ForMember(dest => dest.Semester, opt => opt.Ignore())
                .ForMember(dest => dest.Subject, opt => opt.Ignore());

            CreateMap<StudentFilterRequest, StudentBM>();
            CreateMap<StudentRequest, StudentBM>()
           .ForMember(dest => dest.Studentid, opt => opt.Ignore()) // vì khi tạo mới chưa có ID
           .ForMember(dest => dest.Createat, opt => opt.Ignore())
           .ForMember(dest => dest.GroupStudents, opt => opt.Ignore())
           .ForMember(dest => dest.Submissions, opt => opt.Ignore());
            CreateMap<SubjectFilterRequest, SubjectBM>();
            // Map khi tạo mới và update Subject
            CreateMap<SubjectRequest, SubjectBM>()
                .ForMember(dest => dest.Subjectid, opt => opt.Ignore())
                .ForMember(dest => dest.Subjectname, opt => opt.MapFrom(src => src.SubjectName))
                .ForMember(dest => dest.Exams, opt => opt.Ignore())
                .ForMember(dest => dest.SemesterSubjects, opt => opt.Ignore());
            CreateMap<SubmissionFilterRequest, SubmissionBM>();
            CreateMap<SubmissionRequest, SubmissionBM>();

            CreateMap<ExamFilterRequest, ExamBM>();
            CreateMap<ExamRequest, ExamBM>()
                .ForMember(dest => dest.Semester, opt => opt.Ignore())
                .ForMember(dest => dest.Subject, opt => opt.Ignore());


            // ================== Paged Mapping ==================
            CreateMap(typeof(PagedResult<>), typeof(PagedResponse<>));


            CreateMap<Exam, ExamBM>()
                .ForMember(dest => dest.Examid, opt => opt.MapFrom(src => src.ExamId))
                .ForMember(dest => dest.Semesterid, opt => opt.MapFrom(src => src.SemesterId))
                .ForMember(dest => dest.Subjectid, opt => opt.MapFrom(src => src.SubjectId))
                .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
                //.ForMember(dest => dest.Submissions, opt => opt.MapFrom(src => src.Submissions))
                .ReverseMap()
                .ForMember(dest => dest.Semester, opt => opt.Ignore()) // tránh vòng lặp navigation
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.Submissions, opt => opt.Ignore());

            CreateMap<Grade, GradeBM>().ReverseMap()
        .ForMember(dest => dest.Submission, opt => opt.Ignore())
        .ForMember(dest => dest.Marker, opt => opt.Ignore())
        .ForMember(dest => dest.GradeDetails, opt => opt.Ignore());

            CreateMap<GradeDetail, GradedetailBM>().ReverseMap()
        .ForMember(dest => dest.Grade, opt => opt.Ignore());

            CreateMap<Subject, SubjectBM>().ReverseMap()
        .ForMember(dest => dest.Exams, opt => opt.Ignore())
        .ForMember(dest => dest.SemesterSubjects, opt => opt.Ignore());


            CreateMap<Submission, SubmissionBM>().ReverseMap()
        .ForMember(dest => dest.Exam, opt => opt.Ignore())
        .ForMember(dest => dest.Student, opt => opt.Ignore())
        .ForMember(dest => dest.Grades, opt => opt.Ignore());

            CreateMap<ClassGroup, GroupBM>().ReverseMap()
        .ForMember(dest => dest.Semester, opt => opt.Ignore())
        .ForMember(dest => dest.GroupStudents, opt => opt.Ignore())
        .ForMember(dest => dest.CreatedByNavigation, opt => opt.Ignore())
        .ForMember(dest => dest.UpdatedByNavigation, opt => opt.Ignore());


            CreateMap<GroupStudent, GroupStudentBM>().ReverseMap()
        .ForMember(dest => dest.Group, opt => opt.Ignore())
        .ForMember(dest => dest.Student, opt => opt.Ignore());

            CreateMap<Semester, SemesterBM>().ReverseMap()
        .ForMember(dest => dest.Exams, opt => opt.Ignore())
        .ForMember(dest => dest.ClassGroups, opt => opt.Ignore())
        .ForMember(dest => dest.SemesterSubjects, opt => opt.Ignore());

            CreateMap<SemesterSubject, SemesterSubjectBM>().ReverseMap()
        .ForMember(dest => dest.Semester, opt => opt.Ignore())
        .ForMember(dest => dest.Subject, opt => opt.Ignore());

            CreateMap<AppUser, UserBM>().ReverseMap()
        .ForMember(dest => dest.Grades, opt => opt.Ignore())
        .ForMember(dest => dest.ClassGroupCreatedByNavigations, opt => opt.Ignore())
        .ForMember(dest => dest.ClassGroupUpdatedByNavigations, opt => opt.Ignore());

            CreateMap<Student, StudentBM>().ReverseMap()
                .ForMember(dest => dest.Submissions, opt => opt.Ignore())
                .ForMember(dest => dest.GroupStudents, opt => opt.Ignore());

        }
    }
}
