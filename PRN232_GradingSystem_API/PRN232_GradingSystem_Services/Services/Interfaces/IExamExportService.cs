using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Interfaces
{
    public interface IExamExportService
    {
        Task<byte[]> ExportExamScoresToExcelAsync(int examId);
    }
}
