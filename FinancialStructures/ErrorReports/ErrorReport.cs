using System;

namespace FinancialStructures.ReportingStructures
{
    public class ErrorReport : IComparable
    {
        /// <summary>
        /// Method of comparison
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj is ErrorReport value)
            {
                if (value.ErrorType == ErrorType)
                {
                    return Message.CompareTo(value.Message);
                }

                return ErrorType.CompareTo(value.ErrorType);
            }

            return 0;
        }

        private ReportType fErrorType;

        internal ReportType ErrorType
        {
            get { return fErrorType; }
            set { fErrorType = value; }
        }

        private string fMessage;
        internal string Message
        {
            get { return fMessage; }
            set { fMessage = value; }
        }

        private ErrorReport()
        {
        }

        internal ErrorReport(ReportType type, string msg)
        {
            ErrorType = type;
            Message = msg;
        }
    }
}
