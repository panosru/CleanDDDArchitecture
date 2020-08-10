namespace Aviant.DDD.Domain.Validators
{
    using System;
    using Events;

    public static class AssertionsConcernValidator
    {
        public static bool HasEvents()
        {
            return EventsFacade.HasEvents();
        }

        public static bool IsSatisfiedBy(params Func<bool>[] asserts)
        {
            var isSatisfied = true;

            foreach (var assert in asserts)
                if (!assert())
                    isSatisfied = false;

            return isSatisfied;
        }

        public static Func<bool> IsEqual(string left, string right, IEvent @event)
        {
            return delegate
            {
                if (left == right) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsDateEqual(DateTime left, DateTime right, IEvent @event)
        {
            return delegate
            {
                if (left.Date != right.Date) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsLowerThan(int? left, int right, IEvent @event)
        {
            return delegate
            {
                if (left > right) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsLowerThan(decimal? left, decimal right, IEvent @event)
        {
            return delegate
            {
                if (left > right) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsLowerThan(DateTime? left, DateTime right, IEvent @event)
        {
            return delegate
            {
                if (left > right) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsLowerThanOrEqual(int? left, int right, IEvent @event)
        {
            return delegate
            {
                if (left >= right) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsLowerThanOrEqual(decimal? left, decimal right, IEvent @event)
        {
            return delegate
            {
                if (left >= right) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsLowerThanOrEqual(DateTime? left, DateTime right, IEvent @event)
        {
            return delegate
            {
                if (left >= right) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsGreaterThan(int? left, int right, IEvent @event)
        {
            return delegate
            {
                if (left > right) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsGreaterThan(decimal? left, decimal right, IEvent @event)
        {
            return delegate
            {
                if (left > right) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsGreaterThan(DateTime? left, DateTime right, IEvent @event)
        {
            return delegate
            {
                if (left > right) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsGreaterThanOrEqual(int? left, int right, IEvent @event)
        {
            return delegate
            {
                if (left <= right) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsGreaterThanOrEqual(decimal? left, decimal right, IEvent @event)
        {
            return delegate
            {
                if (left <= right) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsGreaterThanOrEqual(DateTime? left, DateTime right, IEvent @event)
        {
            return delegate
            {
                if (left <= right) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsStringNotNullOrWhiteSpace(string value, IEvent @event)
        {
            return delegate
            {
                if (!string.IsNullOrWhiteSpace(value)) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> HasMinimumLength(string value, int minLength, IEvent @event)
        {
            return delegate
            {
                if (value.Length > minLength) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> HasLengthEqual(string value, int Length, IEvent @event)
        {
            return delegate
            {
                if (value?.Length == Length) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsGuidNotEmpty(Guid guid, IEvent @event)
        {
            return delegate
            {
                if (Guid.Empty != guid) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsGuidNotNull(Guid guid, IEvent @event)
        {
            return delegate
            {
                if (guid != null) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> ValidateGuidFromString(string guid, string stringIsEmptyMessage,
            string guidIsEmptyMessage, string guidIsInvalidMessage)
        {
            return delegate
            {
                var isValid = false;

                isValid = IsSatisfiedBy(IsStringNotNullOrWhiteSpace(guid,
                    new DomainEvent(stringIsEmptyMessage)));

                if (!isValid) return false;

                Guid.TryParse(guid, out var parsed);

                isValid = IsSatisfiedBy(
                    IsGuidNotNull(parsed, new DomainEvent(guidIsInvalidMessage)),
                    IsGuidNotEmpty(parsed, new DomainEvent(guidIsEmptyMessage)));

                return isValid;
            };
        }

        public static Func<bool> IsNotNull(object obj, IEvent @event)
        {
            return delegate
            {
                if (null != obj) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }

        public static Func<bool> IsNull(object obj, IEvent @event)
        {
            return delegate
            {
                if (obj is null) return true;

                EventsFacade.AddEvent(@event);
                return false;
            };
        }
    }
}