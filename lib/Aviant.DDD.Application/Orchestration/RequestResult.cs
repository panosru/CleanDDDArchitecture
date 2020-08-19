namespace Aviant.DDD.Application.Orchestration
{
    using System;
    using System.Collections.Generic;

    public class RequestResult
    {
        private readonly object? _payload;

        public RequestResult()
        {
        }

        public RequestResult(object? payload)
        {
            _payload = payload;
            Success = true;
        }

        public RequestResult(object? payload, int? affectedRows)
            : this(payload)
        {
            AffectedRows = affectedRows;
        }

        public RequestResult(List<string> messages)
        {
            Messages = messages;
            Success = false;
        }

        public bool Success { get; set; }

        public List<string> Messages { get; set; } = new List<string>();

        public int? AffectedRows { get; set; }

        public object? Payload()
        {
            return _payload;
        }

        public T Payload<T>()
        {
            if (_payload is null)
                throw new Exception("Payload is null");

            if (typeof(T) != _payload.GetType())
                throw new Exception(
                    string.Format(
                        "Type \"{0}\" does not much payload type \"{1}\"",
                        typeof(T).FullName,
                        _payload.GetType().FullName));

            return (T) _payload;
        }
    }
}