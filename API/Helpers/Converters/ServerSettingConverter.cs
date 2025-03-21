using System;
using System.Collections.Generic;
using System.Globalization;
using API.DTOs;
using API.Entities;
using API.Entities.Enums;
using AutoMapper;

namespace API.Helpers.Converters;

public class ServerSettingConverter: ITypeConverter<IEnumerable<ServerSetting>, ServerSettingDto>
{
    public ServerSettingDto Convert(IEnumerable<ServerSetting> source, ServerSettingDto? destination, ResolutionContext context)
    {
        destination ??= new ServerSettingDto();
        foreach (var row in source)
        {
            switch (row.Key)
            {
                case ServerSettingKey.OpenIdAuthority:
                    destination.OpenIdAuthority = row.Value;
                    break;
                case ServerSettingKey.LoggingLevel:
                    destination.LoggingLevel = row.Value;
                    break;
                case ServerSettingKey.OpenIdConnectProviders:
                    destination.OpenIdProvider = Enum.Parse<OpenIdProvider>(row.Value);
                    break;
                case ServerSettingKey.CalenderSyncProvider:
                    destination.CalenderSyncProvider = Enum.Parse<CalenderSyncProvider>(row.Value);
                    break;
                default:
                    break;
            }
        }

        return destination;
    }
}