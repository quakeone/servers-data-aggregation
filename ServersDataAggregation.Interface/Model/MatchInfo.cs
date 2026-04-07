using ServersDataAggregation.Common.Enums;

namespace ServersDataAggregation.Common.Model
{
    public class TeamScore
    {
        public string Color { get; set; }
        public int Score { get; set; }
    }
    
    public enum FlagStatus
    {
        AtBase,
        Dropped,
        Carried
    }
    public class MatchInfo
    {
        public MatchStatus Status { get; set; }
        public int? MatchLengthMin { get; set; }
        public int? MatchTimeRemainingMin { get; set; }
        public bool IsSuddenDeath { get; set; }
        public int? Round { get; set; }
        public int? RoundTotal { get; set; }
        public FlagStatus? RedFlagStatus { get; set; }
        public FlagStatus? BlueFlagStatus { get; set; }
        public TeamScore Team1 { get; set; }
        public TeamScore Team2 { get; set; }
    }
}
