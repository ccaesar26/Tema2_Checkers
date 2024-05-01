using System;
using System.IO;
using System.Text.Json;
using Checkers.Models;

namespace Checkers.Services;

public static class JsonUtility
{
    public static void SerializeGame(GameConfiguration gameConfiguration, string filePath)
    {
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        
        var jsonString = JsonSerializer.Serialize(gameConfiguration, options);
        File.WriteAllText(filePath, jsonString);
    }
    
    public static GameConfiguration DeserializeGame(string filePath)
    {
        var jsonString = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions();
        options.Converters.Add(new PieceConverter());
        return JsonSerializer.Deserialize<GameConfiguration>(jsonString, options);
    }
    
    public static void SerializeLeaderboard(Leaderboard leaderboard)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        
        var directory = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = Path.Combine(directory, "../../Data/leaderboard.json");
        
        var jsonString = JsonSerializer.Serialize(leaderboard, options);
        File.WriteAllText(filePath, jsonString);
    }
    
    public static Leaderboard DeserializeLeaderboard()
    {
        var directory = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = Path.Combine(directory, "../../Data/leaderboard.json");
        var jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<Leaderboard>(jsonString);
    }
}