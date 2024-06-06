using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace AntiFlood
{
    public class AntiFlood : BasePlugin, IPluginConfig<AntiFloodConfig>
    {
        public override string ModuleName => "AntiFlood";
        public override string ModuleDescription => "";
        public override string ModuleAuthor => "E!N";
        public override string ModuleVersion => "0.0.1";

        private float _floodTime;
        private readonly Dictionary<CCSPlayerController, PlayerInfo> _playerInfo = [];

        public AntiFloodConfig Config { get; set; } = new();

        public void OnConfigParsed(AntiFloodConfig config)
        {
            Config = config;
        }

        public override void Load(bool hotReload)
        {
            _floodTime = Config.FloodTime;
            AddCommandListener("say", OnSay);
            AddCommandListener("say_team", OnSay);
        }

        private HookResult OnSay(CCSPlayerController? player, CommandInfo info)
        {
            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;

            if (_floodTime <= 0.0f)
            {
                return HookResult.Continue;
            }

            float curTime = Server.CurrentTime;

            if (_playerInfo.TryGetValue(player, out PlayerInfo infod))
            {
                if (infod.LastTime >= curTime)
                {
                    if (infod.TokenCount >= 3)
                    {
                        player.PrintToChat($"{ChatColors.Red}You are flooding the server!");
                        return HookResult.Handled;
                    }

                    infod.LastTime += 3.0f;
                    if (infod.TokenCount < 3)
                    {
                        infod.TokenCount++;
                    }
                }
                else if (infod.TokenCount > 0)
                {
                    infod.TokenCount--;
                }

                infod.LastTime = curTime + _floodTime;
            }
            else
            {
                infod = new PlayerInfo { LastTime = curTime + _floodTime };
            }

            _playerInfo[player] = infod;

            return HookResult.Continue;
        }

        private struct PlayerInfo
        {
            public float LastTime { get; set; }
            public int TokenCount { get; set; }
        }
    }
}