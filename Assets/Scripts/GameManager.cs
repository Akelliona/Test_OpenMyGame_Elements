using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Field field;

    int curLevel = 1;

    string[] level1 = new string[] {
        "2 0 2 1 1",
        "2 0 1 0 0"
    };

    string[] level2 = new string[]
    {
        "2 1 2 2",
        "2 1 2 2",
        "1 2 1 1",
        "2 1 2 2",
        "2 2"
    };

    string[] level3 = new string[]
    {
        "2 1 2 2",
        "2 1 2 2",
        "1 2 1 1",
        "2 2 0 2",
        "2 1",
        "0 2"
    };

    void Start()
    {
        field.CreateLevelFrom(level1);
    }

    string[] GetLevelInfo(int i)
    {
        if ( i == 1) {
            return level1;
        } else if (i == 2) {
            return level2;
        } else {
            return level3;
        }
    }
    public void NextLevel()
    {
        ++curLevel;
        int realLevel = (curLevel - 1) % 3 + 1;

        field.ClearLevel();
        field.CreateLevelFrom(GetLevelInfo(realLevel));
    }
}
