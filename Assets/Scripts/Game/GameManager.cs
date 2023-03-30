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

    [SerializeField]
    LevelManager levelManager;

    int curLevel = 1;
    int steps = 0;

    void Start()
    {
        field.OnEventRegister(OnGameEvent);
        var levelInfo = levelManager.GetLevel(curLevel);
        field.CreateLevelFrom(levelInfo.ToArray());
    }

    private void OnDestroy()
    {
        field.OnEventUnregister(OnGameEvent);
    }

    public void NextLevel()
    {
        ++curLevel;
        ResetSteps();

        var levelInfo = levelManager.GetLevel(curLevel);

        field.ClearLevel();
        field.CreateLevelFrom(levelInfo.ToArray());
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
