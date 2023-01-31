using ServersDataAggregation.Common.Model;

namespace ServersDataAggregation.Query;

public static class ModModeHelper
{
    public static bool SupportsTimedMatch (string mode) {
        return mode == "tdm" || mode == "ctf";
    }

    private static ModMode? CRxModMode(ServerSetting setting)
    {
        var value = setting.Value.ToLower();
        ModMode mod = new ModMode() { Mod = "CRx", Mode = "ffa" };
        if (value.Contains("crctf match"))
        {
            mod.Mode = "ctf";
        }
        else if (value.Contains("crmod++ match"))
        {
            mod.Mode = "tdm";
        }
        else if (value.Contains("airshot"))
        {
            mod.Mode = "airshot";
        }
        else if (value.Contains("povduel"))
        {
            mod.Mode = "duel";
        }
        else if (value.Contains("rocketarena"))
        {
            mod.Mode = "ra";
        }
        else if (value.Contains("practice"))
        {
            mod.Mode = "practice";
        }
        else if (value.Contains("clanarena"))
        {
            if (value.Contains("wipeout"))
                mod.Mode = "caw";
            else
                mod.Mode = "ca";
        }
        return mod;
    }

    private static ModMode CRModMode(ServerSetting setting)
    {
        var value = setting.Value.ToLower();
        ModMode mod = new ModMode() { Mod="CRMod", Mode="ffa" };
        if (value.Contains("crctf match"))
        {
            mod.Mode = "ctf";
        }
        else if (value.Contains("crmod++ match"))
        {
            mod.Mode = "tdm";
        }
        else if (value.Contains("airshot"))
        {
            mod.Mode = "airshot";
        }
        else if (value.Contains("povduel"))
        {
            mod.Mode = "duel";
        }
        else if (value.Contains("rocketarena"))
        {
            mod.Mode = "ra";
        }
        else if (value.Contains("practice"))
        {
            mod.Mode = "practice";
        }
        else if (value.Contains("clanarena"))
        {
            if (value.Contains("wipeout"))
                mod.Mode = "caw";
            else
                mod.Mode = "ca";
        }
        return mod;
    }
    private static ModMode? QECRxModMode(IEnumerable<ServerSetting> settings)
    {
        var modeVal = settings.FirstOrDefault(setting => setting.Setting == "mode");

        if (modeVal != null)
        {
            var mode = modeVal.Value;
            if (modeVal.Value == "dm")
            {
                mode = "tdm";
            }
            return new ModMode
            {
                Mod = "QECRx",
                Mode = mode
            };
        }
        return null;
    }

    public static ModMode? DeriveModMode(IEnumerable<ServerSetting> settings)
    {
        ModMode? mode;
        var mod = settings.FirstOrDefault(s => s.Setting.ToLower() == "mod");
        if (mod != null && mod.Value == "qecrx")
        {
            mode = QECRxModMode(settings);
            if (mode != null)
            {
                return mode;
            }
        }
        var teamplay = settings.FirstOrDefault(s => s.Setting.ToLower() == "teamplay");
        var fraglimit = settings.FirstOrDefault(s => s.Setting.ToLower() == "fraglimit");
        if (fraglimit != null && teamplay != null)
        {
            if (teamplay.Value.Contains("ClanRing"))
            {
                return CRModMode(fraglimit);
            }
            else if (teamplay.Value.Contains("CRx"))
            {
                return CRxModMode(fraglimit);
            }
        }
        return null;
    }
}
