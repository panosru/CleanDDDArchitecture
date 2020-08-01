namespace Aviant.DDD.Domain
{
    public interface IValueObject
    {
        public bool Equals(ValueObject.Base obj);

        public int GetHashCode();

        public int HashValue(int seed, object value);
    }
}