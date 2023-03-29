using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Field field;

    [SerializeField]
    InputPlayerController playerController;

    [SerializeField]
    UIController uiController;

    int curLevel = 1;
    int steps = 0;

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
        field.OnEventRegister(OnGameEvent);
        field.CreateLevelFrom(level1);
    }

    private void OnDestroy()
    {
        field.OnEventUnregister(OnGameEvent);
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
        ResetSteps();

        int realLevel = (curLevel - 1) % 3 + 1;

        field.ClearLevel();
        field.CreateLevelFrom(GetLevelInfo(realLevel));
    }

    void OnGameEvent(GameEventType type)
    {
        switch (type) {
            case GameEventType.MoveStart:
                UpdateSteps();
                playerController.Block();
                uiController.Block();
                break;
            case GameEventType.MoveEnd: 
                if(field.IsClear()) {
                    NextLevel();
                }
                playerController.Unblock();
                uiController.UnBlock();
                break;
        }
    }

    void UpdateSteps()
    {
        ++steps;
        uiController.SetSteps(steps);
    }

    private void ResetSteps()
    {
        steps = 0;
        uiController.SetSteps(steps);
    }
}
