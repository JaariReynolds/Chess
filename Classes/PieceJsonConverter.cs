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

            string name = root.GetProperty("name").GetString();
            TeamColour teamColour = Enum.Parse<TeamColour>(root.GetProperty("teamColour").GetString());
            bool hasMoved = root.GetProperty("hasMoved").GetBoolean();
            Square square = JsonSerializer.Deserialize<Square>(root.GetProperty("square").GetRawText(), options);

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
            writer.WriteStartObject();
            writer.WriteString("name", value.Name);
            writer.WritePropertyName("square");
            JsonSerializer.Serialize(writer, value.Square, options);
            writer.WriteString("teamColour", value.TeamColour.ToString());
            writer.WriteNumber("pieceValue", value.PieceValue);
            writer.WriteBoolean("hasMoved", value.HasMoved);
            writer.WriteEndObject();
        }
    }
}
