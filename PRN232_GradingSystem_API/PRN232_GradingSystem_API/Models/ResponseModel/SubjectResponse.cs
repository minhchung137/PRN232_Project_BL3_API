using PRN232_GradingSystem_Repositories.Models;

namespace PRN232_GradingSystem_API.Models.ResponseModel
{
    public class SubjectResponse
    {
        public int Subjectid { get; set; }
        public string Subjectname { get; set; }
        public DateTime? Createat { get; set; }

        // Optional navigation
        //public IReadOnlyList<Exam> Exams { get; set; }
        public IReadOnlyList<SemesterSubject> SemesterSubjects { get; set; }

    }
}
