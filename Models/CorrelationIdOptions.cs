namespace ApiGateway.Models
{
    public class CorrelationIdOptions
    {
        public string Header { get; set; } = "X-Correlation-Id";
        public bool InResponse { get; set; } = true;
    }
}
