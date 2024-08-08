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
            TeamColour teamColour = Enum.Parse<TeamColour>(root.GetProperty("TeamColour").GetString());
            bool hasMoved = root.GetProperty("HasMoved").GetBoolean();
            Square square = JsonSerializer.Deserialize<Square>(root.GetProperty("Square").GetRawText(), options);

            Console.WriteLine($"Deserializing piece: {name}");

            Piece piece = name switch
            {
                "Pawn" => new Pawn(teamColour, square.ToString(), hasMoved),
                "Knight" => new Knight(teamColour, square.ToString(), hasMoved),
                "Bishop" => new Bishop(teamColour, square.ToString(), hasMoved),
                "Rook" => new Rook(teamColour, square.ToString(), hasMoved),
                "King" => new King(teamColour, square.ToString(), hasMoved),
                "Queen" => new Queen(teamColour, square.ToString(), hasMoved),
                _ => throw new NotSupportedException($"Piece type {name} is not supported.")
            };

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
