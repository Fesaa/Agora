using API.DTOs;
using API.Entities;
using API.Helpers.Converters;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        
        CreateMap<IList<ServerSettings>, ServerSettingDto>()
            .ConvertUsing<ServerSettingConverter>();

        CreateMap<IEnumerable<ServerSettings>, ServerSettingDto>()
            .ConvertUsing<ServerSettingConverter>();

    }
}