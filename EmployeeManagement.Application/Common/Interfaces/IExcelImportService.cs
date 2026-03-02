using System.Threading.Tasks;

namespace EmployeeManagement.Application.Common.Interfaces
{
    public interface IExcelImportService
    {
        /// <summary>
        /// Process Excel file and import employees to database
        /// </summary>
        /// <param name="filePath">Server path to uploaded file</param>
        /// <param name="jobId">Unique job identifier for tracking</param>
        Task<ImportResult> ImportEmployeesAsync(string filePath, string jobId);
    }

    public class ImportResult
    {
        public bool Success { get; set; }
        public int RecordsProcessed { get; set; }
        public int RecordsFailed { get; set; }
        public string? ErrorMessage { get; set; }
        public string JobId { get; set; } = string.Empty;

       
    }
}