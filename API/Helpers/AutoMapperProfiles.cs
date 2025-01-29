using System.Collections.Generic;
using API.DTOs;
using API.Entities;
using API.Helpers.Converters;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        
        CreateMap<IList<ServerSetting>, ServerSettingDto>()
            .ConvertUsing<ServerSettingConverter>();

        CreateMap<IEnumerable<ServerSetting>, ServerSettingDto>()
            .ConvertUsing<ServerSettingConverter>();

    }
}