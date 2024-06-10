using System.Collections.Generic;
using System.Text;

namespace Core.CrossCuttingConcerns.Logging
{
    public class LogDetail
    {
        public string MethodName { get; set; }
        public List<LogParameter> LogParameters { get; set; } = new List<LogParameter>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Method: {MethodName}");
            sb.AppendLine("Parameters:");
            foreach (var param in LogParameters)
            {
                sb.AppendLine($"- {param.Name}: {param.Value} ({param.Type})");
            }
            return sb.ToString();
        }
    }
}