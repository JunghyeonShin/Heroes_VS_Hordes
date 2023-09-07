using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PauseIngame : UI_Popup
{
    private enum EButtons
    {
        RestartButton,
        ExitButton
    }

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));

        _BindEvent(_GetButton((int)EButtons.RestartButton).gameObject, _RestartIngame);
        _BindEvent(_GetButton((int)EButtons.ExitButton).gameObject, _ExitIngame);
    }

    private void _RestartIngame()
    {

    }

    private void _ExitIngame()
    {

    }
}
