namespace Aviant.DDD.Domain.ValueObjects
{
    public interface IValueObject
    {
        public bool Equals(ValueObjectBase obj);

        public int GetHashCode();

        public int HashValue(int seed, object value);
    }
}