using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RandomDuelsAndStuffBot.Players
{
    public sealed class PlayerData
    {
        private const string _playerDataLocation = "playerData.json";

        public PlayerData()
        {
            // if data file doesn't exist we just assume the bot is starting fresh
            // as im not stupid
            if (!File.Exists(_playerDataLocation))
            {
                Dictionary<ulong, Player> newData = new();
                using FileStream saveData = File.Create(_playerDataLocation);
                JsonSerializer.Serialize(saveData, newData, _serializationOptions);
                Data = newData;
                return;
            }

            using FileStream fileData = File.OpenRead(_playerDataLocation);
            Data = JsonSerializer.Deserialize<Dictionary<ulong, Player>>(fileData, _serializationOptions);
        }

        public async Task SaveAsync()
        {
            using FileStream saveData = File.Create(_playerDataLocation);
            await JsonSerializer.SerializeAsync(saveData, Data, _serializationOptions);
            await saveData.DisposeAsync();
        }

        public Dictionary<ulong, Player> Data { get; } = new();

        private readonly JsonSerializerOptions _serializationOptions = new()
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            AllowTrailingCommas = true,
            IncludeFields = false,
            WriteIndented = true
        };
    }
}
