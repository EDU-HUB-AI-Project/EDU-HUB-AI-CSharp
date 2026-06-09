namespace EDU_HUB_AI.Util
{
    public static class TransportTypes
    {
        public const string Ktx = "KTX";
        public const string Srt = "SRT";
        public const string Exbus = "EXBUS";
        public const string Airport = "AIRPORT";
        public const string Shuttle = "SHUTTLE";

        public static readonly string[] All = [Ktx, Srt, Exbus, Airport, Shuttle];

        public static string GetLabel(string? type) => type?.Trim().ToUpperInvariant() switch
        {
            Ktx => "KTX",
            Srt => "SRT",
            Exbus => "고속·시외버스",
            Airport => "공항",
            Shuttle => "셔틀버스",
            _ => type ?? "-"
        };

        public static bool UsesArriveTime(string? type) =>
            !string.Equals(type, Shuttle, StringComparison.OrdinalIgnoreCase);
    }
}
