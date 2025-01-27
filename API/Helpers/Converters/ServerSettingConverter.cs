using API.DTOs;
using API.Entities;
using API.Entities.Enums;
using AutoMapper;

namespace API.Helpers.Converters;

public class ServerSettingConverter: ITypeConverter<IEnumerable<ServerSettings>, ServerSettingDto>
{
    public ServerSettingDto Convert(IEnumerable<ServerSettings> source, ServerSettingDto destination, ResolutionContext context)
    {
        destination ??= new ServerSettingDto();
        foreach (var row in source)
        {
            switch (row.Key)
            {
                default:
                    break;
            }
        }

        return destination;
    }
}