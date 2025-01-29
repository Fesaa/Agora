using System.ComponentModel.DataAnnotations;
using API.Entities.Enums;

namespace API.Entities;

public class ServerSetting
{
    [Key]
    public required ServerSettingKey Key { get; set; }

    public required string Value { get; set; }

    [ConcurrencyCheck]
    public uint RowVersion { get; private set; }

    public void OnSavingChanges()
    {
        RowVersion++;
    }
}