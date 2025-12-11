using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using PRN232_GradingSystem_Services.Services.Interfaces;
using PRN232_GradingSystem_Services.SignalR;

namespace PRN232_GradingSystem_Services.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext)
    {
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
    }

    public async Task SendGradeCreatedNotificationAsync(Grade grade)
    {
        if (!grade.SubmissionId.HasValue)
            return;

        var gradeWithDetails = await _unitOfWork.GradeRepository.GetByIdWithDetailsAsync(grade.GradeId);
        if (gradeWithDetails == null)
            return;

        var submission = await _unitOfWork.SubmissionRepository.GetByIdWithDetailsAsync(grade.SubmissionId.Value);
        if (submission == null || submission.Student == null)
            return;

        var studentRoll = submission.Student.StudentRoll ?? "unknown";

        var gradeDetails = gradeWithDetails.GradeDetails?
            .Select(gd => new GradeDetailNotification
            {
                GradedetailId = gd.GradeDetailId,
                Qcode = gd.QCode,
                Subcode = gd.SubCode,
                Point = gd.Point,
                Note = gd.Note,
                CreatedAt = gd.CreatedAt
            })
            .ToList()
            ?? new List<GradeDetailNotification>();

        var notification = new
        {
            Type = "GradeCreated",
            GradeId = grade.GradeId,
            SubmissionId = grade.SubmissionId,
            StudentId = studentRoll,
            TotalScore = grade.TotalScore,
            Status = grade.Status,
            CreatedAt = grade.CreatedAt,
            Q1 = grade.Q1,
            Q2 = grade.Q2,
            Q3 = grade.Q3,
            Q4 = grade.Q4,
            Q5 = grade.Q5,
            Q6 = grade.Q6,
            GradeDetails = gradeDetails,
            Message = $"Grade đã được tạo cho submission #{grade.SubmissionId} với tổng điểm: {grade.TotalScore}"
        };

        await _hubContext.Clients.Group($"student_{studentRoll}").SendAsync("GradeCreated", notification);
        await _hubContext.Clients.All.SendAsync("GradeCreated", notification);
    }

    private class GradeDetailNotification
    {
        public int GradedetailId { get; set; }
        public string Qcode { get; set; } = string.Empty;
        public string Subcode { get; set; } = string.Empty;
        public decimal? Point { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

