using System;
using Aviant.DDD.Domain;

namespace Aviant.DDD.Domain.Validators
{
    public static class AssertionsConcern
    {
        public static bool HasNotifications()
        {
            return Notification.Facade.HasNotifications();
        }

        public static bool IsSatisfiedBy(params Func<bool>[] asserts)
        {
            bool isSatisfied = true;

            foreach (var assert in asserts)
            {
                if (!assert())
                    isSatisfied = false;
            }

            return isSatisfied;
        }

        public static Func<bool> IsEqual(string left, string right, INotification message)
        {
            return delegate
            {
                if (left == right) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }

        public static Func<bool> IsDateEqual(DateTime left, DateTime right, INotification message)
        {
            return delegate
            {
                if (left.Date != right.Date) return true;

                Notification.Facade.AddNotification(message);
                return false;
            };
        }

        public static Func<bool> IsLowerThan(int? left, int right, INotification message)
        {
            return delegate()
            {
                if (left > right) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }
        
        public static Func<bool> IsLowerThan(decimal? left, decimal right, INotification message)
        {
            return delegate()
            {
                if (left > right) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }
        
        public static Func<bool> IsLowerThan(DateTime? left, DateTime right, INotification message)
        {
            return delegate()
            {
                if (left > right) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }

        public static Func<bool> IsLowerThanOrEqual(int? left, int right, INotification message)
        {
            return delegate
            {
                if (left >= right) return true;

                Notification.Facade.AddNotification(message);
                return false;
            };
        }
        
        public static Func<bool> IsLowerThanOrEqual(decimal? left, decimal right, INotification message)
        {
            return delegate
            {
                if (left >= right) return true;

                Notification.Facade.AddNotification(message);
                return false;
            };
        }
        
        public static Func<bool> IsLowerThanOrEqual(DateTime? left, DateTime right, INotification message)
        {
            return delegate
            {
                if (left >= right) return true;

                Notification.Facade.AddNotification(message);
                return false;
            };
        }

        public static Func<bool> IsGreaterThan(int? left, int right, INotification message)
        {
            return delegate
            {
                if (left > right) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }
        
        public static Func<bool> IsGreaterThan(decimal? left, decimal right, INotification message)
        {
            return delegate
            {
                if (left > right) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }
        
        public static Func<bool> IsGreaterThan(DateTime? left, DateTime right, INotification message)
        {
            return delegate
            {
                if (left > right) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }

        public static Func<bool> IsGreaterThanOrEqual(int? left, int right, INotification message)
        {
            return delegate
            {
                if (left <= right) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }
        
        public static Func<bool> IsGreaterThanOrEqual(decimal? left, decimal right, INotification message)
        {
            return delegate
            {
                if (left <= right) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }
        
        public static Func<bool> IsGreaterThanOrEqual(DateTime? left, DateTime right, INotification message)
        {
            return delegate
            {
                if (left <= right) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }

        public static Func<bool> IsStringNotNullOrWhiteSpace(string value, INotification message)
        {
            return delegate
            {
                if (!string.IsNullOrWhiteSpace(value)) return true;

                Notification.Facade.AddNotification(message);
                return false;
            };
        }

        public static Func<bool> HasMinimumLength(string value, int minLength, INotification message)
        {
            return delegate
            {
                if (value.Length > minLength) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }

        public static Func<bool> HasLengthEqual(string value, int Length, INotification message)
        {
            return delegate
            {
                if (value?.Length == Length) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }

        public static Func<bool> IsGuidNotEmpty(Guid guid, INotification message)
        {
            return delegate
            {
                if (Guid.Empty != guid) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }

        public static Func<bool> IsGuidNotNull(Guid guid, INotification message)
        {
            return delegate
            {
                if (guid != null) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }

        public static Func<bool> ValidateGuidFromString(string guid, string stringIsEmptyMessage,
            string guidIsEmptyMessage, string guidIsInvalidMessage)
        {
            return delegate
            {
                bool isValid = false;

                isValid = IsSatisfiedBy(IsStringNotNullOrWhiteSpace(guid, 
                    new Notification.Create(stringIsEmptyMessage)));

                if (!isValid) return false;

                Guid.TryParse(guid, out Guid parsed);

                isValid = IsSatisfiedBy(
                    IsGuidNotNull(parsed, new Notification.Create(guidIsInvalidMessage)),
                    IsGuidNotEmpty(parsed, new Notification.Create(guidIsEmptyMessage)));

                return isValid;
            };
        }

        public static Func<bool> IsNotNull(object obj, INotification message)
        {
            return delegate
            {
                if (null != obj) return true;

                Notification.Facade.AddNotification(message);
                return false;
            };
        }

        public static Func<bool> IsNull(object obj, INotification message)
        {
            return delegate
            {
                if (obj is null) return true;
                
                Notification.Facade.AddNotification(message);
                return false;
            };
        }
    }
}