namespace Aviant.DDD.Application.Mappings
{
    using AutoMapper;

    public interface IMapTo<T>
    {
        void Mapping(Profile profile) => profile.CreateMap(GetType(), typeof(T));
    }
}