namespace Aviant.DDD.Domain
{
    using ValueObject;

    public interface IValueObject
    {
        public bool Equals(Base obj);

        public int GetHashCode();

        public int HashValue(int seed, object value);
    }
}