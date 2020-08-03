namespace Aviant.DDD.Application
{
    using System.Collections.Generic;
    using System.Linq;

    public class Result
    {
        internal Result(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
        }

        public bool Succeeded { get; }

        public string[] Errors { get; }

        /// <summary>
        ///     In case you need to override the default behaviour, you can use in your derived class something like this:
        ///     public new static Result Success() { ... }
        /// </summary>
        /// <returns></returns>
        public static Result Success()
        {
            return new Result(true, new string[] { });
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }
    }
}