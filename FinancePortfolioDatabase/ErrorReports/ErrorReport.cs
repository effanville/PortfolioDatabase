
namespace ReportingStructures
{
    public class ErrorReport
    {
        private ReportType fErrorType;

        public ReportType ErrorType
        {
            get { return fErrorType; }
            set { fErrorType = value; }
        }

        private string fMessage;
        public string Message
        { 
            get { return fMessage; }
            set { fMessage = value; }
        }

        public ErrorReport()
        {
        }

        public ErrorReport(ReportType type, string msg)
        {
            ErrorType = type;
            Message = msg;
        }
    }
}
