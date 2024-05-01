using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Checkers.Models;

namespace Checkers.Services;

public class PieceConverter : JsonConverter<Piece>
{
    public override Piece Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            // Read properties from JSON
            var root = doc.RootElement;
            var type = (EPieceType)root.GetProperty("Type").GetInt32();
            var color = (EPieceColor)root.GetProperty("Color").GetInt32();

            // Create piece using factory method
            return Piece.Create(type, color);
        }
    }

    public override void Write(Utf8JsonWriter writer, Piece value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
}