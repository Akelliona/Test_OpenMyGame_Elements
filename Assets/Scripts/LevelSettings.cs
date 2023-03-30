using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EndLevelsBehaviour {
    Random,
    Repeated
}


[Serializable]
public class LevelInfo {
    public int minLevel;
    public int maxLevel;
    public string levelPath;
}

[CreateAssetMenu(fileName = "Data", menuName = "Settings/Create Level Settings", order = 1)]
public class LevelSettings : ScriptableObject
{
    public EndLevelsBehaviour behaviour;
    public List<LevelInfo> levels;
    public List<string> defaultLevel;

    public int maxLevel()
    {
        int max = 0;
        foreach(var level in levels) {
            max = Math.Max(max, level.maxLevel);
        }
        return max;
    }
}
