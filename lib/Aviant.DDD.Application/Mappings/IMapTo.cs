using AutoMapperProfile = AutoMapper.Profile;

namespace Aviant.DDD.Application.Mappings
{
    public interface IMapTo<T>
    {
        void Mapping(AutoMapperProfile profile) => profile.CreateMap(GetType(), typeof(T));
    }
}