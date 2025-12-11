using PRN232_GradingSystem_Services.Services.Implementations;
using System.ComponentModel.DataAnnotations;

namespace PRN232_GradingSystem_API.Models.RequestModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GradedetailValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Chuyển sang request class
            var request = value as GradedetailCreateRequest;
            if (request == null) return ValidationResult.Success;

            // ===== Validate Qcode =====
            var question = MaxScoresData.MaxScores.FirstOrDefault(q => q.Qcode == request.Qcode);
            if (question == null)
                return new ValidationResult($"Qcode '{request.Qcode}' không hợp lệ.");

            // ===== Validate Subcode =====
            if (!question.SubScores.ContainsKey(request.Subcode))
            {
                var allowed = string.Join(", ", question.SubScores.Keys);
                return new ValidationResult($"Subcode '{request.Subcode}' không hợp lệ cho Qcode '{request.Qcode}'. Cho phép: {allowed}");
            }

            // ===== Validate Point =====
            var maxPoint = question.SubScores[request.Subcode];
            if (request.Point.HasValue && request.Point > maxPoint)
                return new ValidationResult($"Point '{request.Point}' không hợp lệ cho {request.Qcode}.{request.Subcode}. Must be smaller or equal: {maxPoint}");

            return ValidationResult.Success;
        }
    }
}
