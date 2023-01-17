using ServersDataAggregation.Common.Model;

namespace ServersDataAggregation.Query;

public static class ModModeHelper
{
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

    public static ModMode? DeriveModMode(List<ServerSetting> settings)
    {
        ModMode? mode;
        var fraglimit = settings.Find(s => s.Setting.ToLower() == "fraglimit");
        if (fraglimit != null)
        {
            mode = CRModMode(fraglimit);
            if (mode != null)
            {
                return mode;
            }
        }
        return null;
    }
}
