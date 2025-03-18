using System.Text.Json.Serialization;

namespace KoboldSharp
{
    public class PerformanceData
    {
        /// <summary>
        /// Last Processing time in seconds
        /// </summary>
        [JsonPropertyName("last_process")]
        public float LastProcessSeconds { get; set; }

        [JsonIgnore]
        public TimeSpan LastProcessTime
        {
            get
            {
                float seconds = MathF.Floor(LastProcessSeconds);
                float milliseconds = (LastProcessSeconds - seconds) / 1000;
                return new TimeSpan(0, 0, 0, (int)seconds, (int)milliseconds);
            }
        }

        /// <summary>
        /// Last evalution time in seconds
        /// </summary>
        [JsonPropertyName("last_eval")]
        public float LastEvalSeconds { get; set; }

        [JsonIgnore]
        public TimeSpan LastEvalTime
        {
            get
            {
                float seconds = MathF.Floor(LastEvalSeconds);
                float milliseconds = (LastEvalSeconds - seconds) / 1000;
                return new TimeSpan(0, 0, 0, (int)seconds, (int)milliseconds);
            }
        }

        /// <summary>
        /// Last token count.
        /// </summary>
        [JsonPropertyName("last_token_count")]
        public int LastTokenCount { get; set; }

        /// <summary>
        /// Last generation seed used.
        /// </summary>
        [JsonPropertyName("last_seed")]
        public int LastGenerationSeed { get; set; }

        /// <summary>
        /// Total requests generated since startup.
        /// </summary>
        [JsonPropertyName("total_gens")]
        public int TotalGenerations { get; set; }

        /// <summary>
        /// Reason the generation stopped. See <see cref="KoboldSharp.StopReason"/>
        /// </summary>
        [JsonPropertyName("stop_reason")]
        public StopReason StopReason { get; set; }

        /// <summary>
        /// Queue length
        /// </summary>
        [JsonPropertyName("queue")]
        public int Queue { get; set; }

        /// <summary>
        /// Current backend status. See <see cref="KoboldSharp.BackendState"/>
        /// </summary>
        [JsonPropertyName("idle")]
        public BackendState Status { get; set; }

        /// <summary>
        /// Horde exit counter, if applicable, if this value is very high, may have crashed.
        /// </summary>
        [JsonPropertyName("hordeexitcounter")]
        public int HordeExitCounter { get; set; }

        /// <summary>
        /// Uptime in seconds. see <see cref="Uptime"/> for <see cref="TimeSpan"/> version
        /// </summary>
        [JsonPropertyName("uptime")]
        public int UptimeSeconds { get; set; }

        [JsonIgnore]
        public TimeSpan Uptime
        {
            get
            {
                return new TimeSpan(0, 0, seconds: UptimeSeconds);
            }
        }
    }

    public enum BackendState : byte
    {
        Busy = 0,
        Idle = 1
    }

    public enum StopReason : int
    {
        Invalid = -1,
        OutOfTokens = 0,
        EOSTokenHit = 1,
        CustomStopped = 2,
    }
}
