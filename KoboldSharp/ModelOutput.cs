﻿using System.Text.Json.Serialization;

namespace KoboldSharp
{
    public class ModelOutput
    {
        [JsonPropertyName("results")]
        public List<Result> Results { get; set; }
    }

    public class Result
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
