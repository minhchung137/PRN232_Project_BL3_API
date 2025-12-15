namespace PRN232_GradingSystem_API.Models.ResponseModel
{
    public class RubricResponse
    {
        public class ExamRubricResponse
        {
            public string ExamCode { get; set; } = "";
            public List<QuestionRubricResponse> Questions { get; set; } = new();
        }

        public class QuestionRubricResponse
        {
            public int QuestionId { get; set; }
            public string QCode { get; set; } = "";
            public string? Description { get; set; }
            public decimal MaxScore { get; set; }
            public List<CriteriaRubricResponse> Criteria { get; set; } = new();
        }

        public class CriteriaRubricResponse
        {
            public int CriteriaId { get; set; }
            public int OrderIndex { get; set; }
            public string Content { get; set; } = "";
            public decimal Weight { get; set; }
            public bool IsManual { get; set; }
        }
    }
}