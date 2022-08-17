using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System;
using System.Text;

namespace RandomDuelsAndStuffBot.Players
{
    public sealed class PlayerData
    {
        private const string _playerDataLocation = "player.dat";

        public PlayerData()
        {
            if (File.Exists(_playerDataLocation))
            {
                using FileStream fileData = File.OpenRead(_playerDataLocation);
                Data = JsonSerializer.Deserialize<Dictionary<ulong, Player>>(fileData, _serializationOptions);
            }
            else
                Data = new();
        }

        // todo: string compression

        public async Task SaveAsync()
        {
            await using FileStream saveData = File.Create(_playerDataLocation);
            await JsonSerializer.SerializeAsync(saveData, Data, _serializationOptions);
        }

        public Dictionary<ulong, Player> Data { get; }

        private readonly JsonSerializerOptions _serializationOptions = new()
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            AllowTrailingCommas = true,
            IncludeFields = false,
            WriteIndented = false
        };
    }
}
