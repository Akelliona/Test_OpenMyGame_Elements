using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    LevelSettings settings;

    [SerializeField]
    List<List<string>> levels = new List<List<string>>();

    int minLevel = 0;
    int maxLevel = 0;

    public List<string> GetLevel(int i)
    {
        if (i >= minLevel && i <= maxLevel) {
            return GetLoadedLevel(i);
        } else {
            var info = settings.levels.Find(l => l.minLevel <= i && l.maxLevel >= i);
            if (info == null) {
                return GetNonExistLevel(i);
            } else {
                return GetUnloadedLevel(i, info);
            }
        }
    }
    void LoadLevels(string levelPath)
    {
        var text = Resources.Load<TextAsset>(levelPath);

        JObject obj = JObject.Parse(text.text);
        if (obj.ContainsKey("levels") && obj["levels"] as JArray != null) {
            JArray levelsJson = obj["levels"] as JArray;
            foreach (JArray levelJson in levelsJson) {
                List<string> row = new List<string>();
                foreach (string level in levelJson) {
                    row.Add(level);
                }
                levels.Add(row);
            }

        }
    }

    List<string> GetNonExistLevel(int i)
    {
        var max = settings.maxLevel();
        switch (settings.behaviour) {
            case EndLevelsBehaviour.Random:
                return GetLevel(Random.Range(1, max + 1));
            default:
                return GetLevel((i - 1) % max + 1);
        }
    }

    List<string> GetLoadedLevel(int i)
    {
        if (levels.Count == 0 || i - minLevel >= levels.Count)
        {
            return settings.defaultLevel;
        }

        return levels[i - minLevel];
    }

    List<string> GetUnloadedLevel(int i, LevelInfo info)
    {
        levels.Clear();

        minLevel = info.minLevel;
        maxLevel = info.maxLevel;
        LoadLevels(info.levelPath);

        return GetLoadedLevel(i);
    }
}
