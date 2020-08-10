namespace Aviant.DDD.Application.Identity
{
    using System.Collections.Generic;
    using System.Linq;

    public class IdentityResult
    {
        internal IdentityResult(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
        }

        public bool Succeeded { get; }

        public string[] Errors { get; }

        /// <summary>
        ///     In case you need to override the default behaviour, you can use in your derived class something like this:
        ///     public new static IdentityResult Success() { ... }
        /// </summary>
        /// <returns></returns>
        public static IdentityResult Success()
        {
            return new IdentityResult(true, new string[] { });
        }

        public static IdentityResult Failure(IEnumerable<string> errors)
        {
            return new IdentityResult(false, errors);
        }
    }
}