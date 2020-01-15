using System.Collections.Generic;

namespace ProductMgt.ApplicationService
{
    public abstract class BaseCommandResult
    {
        public HashSet<ResultStatus> ResultStatuses { get; set; } = new HashSet<ResultStatus>(ResultStatus.Instance);
        public class ResultStatus : IEqualityComparer<ResultStatus>
        {
            public bool IsSuccess { get; set; }
            public string ErrorCode { get; set; }
            public string ErrorMessage { get; set; }
            public static ResultStatus Instance => new ResultStatus();
            public bool Equals(ResultStatus x, ResultStatus y)
            {
                if ((x == null) && (y == null))
                    return true;
                if (x == null || y == null)
                    return false;
                return x.ErrorCode == y.ErrorCode;
            }

            public int GetHashCode(ResultStatus obj)
            {
                return obj.ErrorCode.GetHashCode();
            }
        }

        public void AddStatus(bool isSuccess, string errorCode, string errorMessage)
        {
            ResultStatuses.Add(new ResultStatus()
            {
                IsSuccess = isSuccess,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            });
        }

        public void RemoveStatus(string errorCode)
        {
            ResultStatuses.RemoveWhere(r => r.ErrorCode == errorCode);
        }
    }
}