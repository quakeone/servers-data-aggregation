using ServersDataAggregation.Common.Model;

namespace ServersDataAggregation.Query;

public static class ModModeHelper
{
    public static bool SupportsTimedMatch (string mode) {
        return mode == "match" || mode == "ctf";
    }

    private static ModMode? CRModMode(ServerSetting setting)
    {
        var value = setting.Value.ToLower();
        string? mod = null;
        if (value.Contains("crmod"))
        {
            mod = "CRMod";
        }
        else if (value.Contains("crctf"))
        {
            mod = "CRCTF";
        }

        if (!string.IsNullOrEmpty(mod))
        {
            var split = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return new ModMode
            {
                Mod = mod,
                Mode = split[split.Length - 1].ToLower()
            };
        }
        return null;
    }
    private static ModMode? CRxModMode(IEnumerable<ServerSetting> settings)
    {
        var playMode = settings.FirstOrDefault(setting => setting.Setting == "playmode");
        if (playMode != null)
        {
            return new ModMode
            {
                Mod = "CRx",
                Mode = playMode.Value
            };
        }
        return null;
    }

    public static ModMode? DeriveModMode(IEnumerable<ServerSetting> settings)
    {
        ModMode? mode;
        var fraglimit = settings.FirstOrDefault(s => s.Setting.ToLower() == "fraglimit");
        if (fraglimit != null)
        {
            mode = CRModMode(fraglimit);
            if (mode != null)
            {
                return mode;
            }
        }
        var mod = settings.FirstOrDefault(s => s.Setting.ToLower() == "mod");
        if (mod != null && mod.Value == "qecrx")
        {
            mode = CRxModMode(settings);
            if (mode != null)
            {
                return mode;
            }
        }
        return null;
    }
}
