using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aviant.DDD.Domain.ValueObject
{
    // Learn more: https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/implement-value-objects
    // source: https://github.com/jhewlett/ValueObject
    public abstract class Base : IValueObject
    {
        private List<PropertyInfo> _properties;
        private List<FieldInfo> _fields;
        
        public static bool operator ==(Base left, Base right)
        {
            if (left is null ^ right is null)
            {
                return false;
            }

            return left?.Equals(right) != false;
        }
        
        public static bool operator !=(Base left, Base right)
        {
            return !(left == right);
        }
        
        public bool Equals(Base obj)
        {
            return Equals(obj as object);
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType()) return false;

            return GetProperties().All(p => PropertiesAreEqual(obj, p))
                   && GetFields().All(f => FieldsAreEqual(obj, f));
        }

        private bool PropertiesAreEqual(object obj, PropertyInfo p)
        {
            return object.Equals(p.GetValue(this, null), p.GetValue(obj, null));
        }

        private bool FieldsAreEqual(object obj, FieldInfo f)
        {
            return object.Equals(f.GetValue(this), f.GetValue(obj));
        }

        private IEnumerable<PropertyInfo> GetProperties()
        {
            if (_properties is null)
            {
                _properties = GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.GetCustomAttribute(typeof(IgnoreMemberAttribute)) is null)
                    .ToList();
            }

            return _properties;
        }

        private IEnumerable<FieldInfo> GetFields()
        {
            if (_fields is null)
            {
                _fields = GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .Where(f => f.GetCustomAttribute(typeof(IgnoreMemberAttribute)) is null)
                    .ToList();
            }

            return _fields;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (var property in GetProperties())
                {
                    var value = property.GetValue(this, null);
                    hash = HashValue(hash, value);
                }

                foreach (var field in GetFields())
                {
                    var value = field.GetValue(this);
                    hash = HashValue(hash, value);
                }

                return hash;
            }
        }

        public int HashValue(int seed, object value)
        {
            var currentHash = value?.GetHashCode() ?? 0;

            return seed * 23 + currentHash;
        }
    }
}