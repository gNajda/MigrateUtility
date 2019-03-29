using System.Collections.Generic;
using System.Linq;

namespace MigrateUtility
{
    public class OutputWrapper<T>
    {
        public bool ISuccess => !FailedRecords.Any();
        public IEnumerable<(T Record, string Message)> FailedRecords { get; internal set; } = new List<(T Record, string Message)>();

        public static OutputWrapper<T> operator +(OutputWrapper<T> left, OutputWrapper<T> right)
        {
            return new OutputWrapper<T>
            {
                FailedRecords = left.FailedRecords.Concat(right.FailedRecords)
            };
        }
    }
}
