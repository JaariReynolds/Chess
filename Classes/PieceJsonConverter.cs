using Chess.Classes.ConcretePieces;
using Chess.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chess.Classes
{
    public class PieceJsonConverter : JsonConverter<Piece>
    {
        public override Piece? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var root = jsonDoc.RootElement;

            string name = root.GetProperty("Name").GetString();

            Console.WriteLine($"Deserializing piece: {name}");

            Piece piece = name switch
            {
                "Pawn" => new Pawn(),
                "Knight" => new Knight(),
                "Bishop" => new Bishop(),
                "Rook" => new Rook(),
                "King" => new King(),
                "Queen" => new Queen(),
                _ => throw new NotSupportedException($"Piece type {name} is not supported.")
            };

            piece.Name = name;
            piece.TeamColour = Enum.Parse<TeamColour>(root.GetProperty("TeamColour").GetString());
            piece.HasMoved = root.GetProperty("HasMoved").GetBoolean();
            piece.Square = JsonSerializer.Deserialize<Square>(root.GetProperty("Square").GetRawText(), options);
            piece.PieceValue = root.GetProperty("PieceValue").GetInt16();

            return piece;
        }

        public override void Write(Utf8JsonWriter writer, Piece value, JsonSerializerOptions options)
        {
            Console.WriteLine($"Serializing piece: {value.Name}");

            writer.WriteStartObject();
            writer.WriteString("Name", value.Name);
            writer.WritePropertyName("Square");
            JsonSerializer.Serialize(writer, value.Square, options);
            writer.WriteString("TeamColour", value.TeamColour.ToString());
            writer.WriteNumber("PieceValue", value.PieceValue);
            writer.WriteBoolean("HasMoved", value.HasMoved);
            writer.WriteEndObject();
        }
    }
}
