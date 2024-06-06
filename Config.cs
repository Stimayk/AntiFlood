using CounterStrikeSharp.API.Core;

namespace AntiFlood
{
    public class AntiFloodConfig : BasePluginConfig
    {
        public float FloodTime { get; set; } = 0.75f;

    }
}
