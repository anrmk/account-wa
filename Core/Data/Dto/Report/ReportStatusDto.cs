
using Core.Data.Enum;

namespace Core.Data.Dto {
    public class ReportStatusDto {
        public ReportCheckStatus Status { get; set; }
        public string Message { get; set; }

        public ReportStatusDto(ReportCheckStatus status, string message) {
            Status = status;
            Message = message;
        }
    }
}
