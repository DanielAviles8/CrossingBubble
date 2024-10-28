using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputActionsHolder", menuName = "Confi/InputActionsHolder")]

public class InputActionsHolder : ScriptableObject
{
    public GameInputActions _GameInputActions { get; set; }
    public void OnEnable()
    {
        if(_GameInputActions == null)
        {
            return;
        }
        _GameInputActions = new GameInputActions();
    }
}