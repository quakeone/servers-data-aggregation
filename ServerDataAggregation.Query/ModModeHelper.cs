using ServersDataAggregation.Common.Model;

namespace ServersDataAggregation.Query;

public static class ModModeHelper
{
    public static bool SupportsTimedMatch (string mod, string mode) {
        if (mod == "CRMod")
        {
            return mode == "tdm" || mode == "ctf" || mode == "duel"
                || mode == "ca" || mode == "caw" || mode == "ra"
                || mode == "airshot";
        }
        if (mod == "KTX")
        {
            return mode == "tdm" || mode == "ctf" || mode == "duel"
                || mode == "ca" || mode == "caw" || mode == "ra";
        }
        return mode == "tdm" || mode == "ctf" || mode == "duel";
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
        else if (value.Contains("crmod++ duel"))
        {
            mod.Mode = "duel";
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
        else if (value.Contains("clanarenawipeout"))
        {
            mod.Mode = "caw";
        }
        else if (value.Contains("clanarena"))
        {
            mod.Mode = "ca";
        }
        else if (value.Contains("head hunters"))
        {
            mod.Mode = "hh";
        }
        else if (value.Contains("crctf pickup"))
        {
            mod.Mode = "ctf";
        }
        else if (value.Contains("practice"))
        {
            mod.Mode = "practice";
        }
        else if (value.Contains("team deathmatch"))
        {
            mod.Mode = "tdm";
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

    private static ModMode? KTXModMode(IEnumerable<ServerSetting> settings)
    {
        var modeVal = settings.FirstOrDefault(s => s.Setting.ToLower() == "mode");
        if (modeVal == null)
        {
            return new ModMode { Mod = "KTX", Mode = "ffa" };
        }

        // KTX mode format: "base[-submode]..." e.g. "4on4", "4on4-ca", "duel-instagib"
        var parts = modeVal.Value.ToLower().Split('-');
        var baseMode = parts[0];
        var mode = baseMode switch
        {
            "ctf" => "ctf",
            "duel" or "1on1" => "duel",
            "2on2" or "3on3" or "4on4" => "tdm",
            _ => "ffa"
        };

        // submodes override the base mode
        for (int i = 1; i < parts.Length; i++)
        {
            switch (parts[i])
            {
                case "ca":
                    mode = "ca";
                    break;
                case "wo":
                    mode = "caw";
                    break;
                case "ra":
                    mode = "ra";
                    break;
            }
        }

        return new ModMode
        {
            Mod = "KTX",
            Mode = mode
        };
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
        if (mod != null && mod.Value.StartsWith("nqCRx"))
        {
            var fraglimitSetting = settings.FirstOrDefault(s => s.Setting.ToLower() == "fraglimit");
            if (fraglimitSetting != null)
            {
                return CRModMode(fraglimitSetting);
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
            else if (teamplay.Value.Contains("CRx") || teamplay.Value.Contains("CRX"))
            {
                return CRxModMode(fraglimit);
            }
        }
        var ktxVer = settings.FirstOrDefault(s => s.Setting.ToLower() == "ktxver");
        if (ktxVer != null)
        {
            return KTXModMode(settings);
        }
        return null;
    }
}
