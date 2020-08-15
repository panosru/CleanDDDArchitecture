namespace Aviant.DDD.Application.Orchestration
{
    using System.Collections.Generic;

    public class RequestResult
    {
        public bool Success { get; set; }
        
        public List<string> Messages { get; set; } = new List<string>();
        
        public object? Payload { get; set; }
    }
}