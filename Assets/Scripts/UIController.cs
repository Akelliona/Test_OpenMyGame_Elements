using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    List<Button> buttons;
    [SerializeField]
    TMPro.TMP_Text stepLabel;
    
    public void SetSteps(int step)
    {
        stepLabel.text = string.Format("Ходов: {0}", step);
    }

    public void Block()
    {
        foreach(var button in buttons) {
            button.interactable = false;
        }
    }

    public void UnBlock()
    {
        foreach (var button in buttons) {
            button.interactable = true;
        }
    }
}
