using System.Text.Json.Serialization;

namespace CortexFilterTestAPI.Other
{
    public class ValuesResponse
    {
        public const string JSON_SCHEMA = """
                {
                    "type": "object",
                    "properties": {
                        "values": {
                            "type": "array",
                            "items": {
                                "type": "number"
                            }
                        }
                    },
                    "required": [ "values" ]
                }
                """;

        [JsonPropertyName("values")]
        public int[] Values { get; init; }
    }
}