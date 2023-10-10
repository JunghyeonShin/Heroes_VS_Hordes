using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SelectHero : UI_Popup
{
    private enum EButtons
    {
        ExitButton
    }

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));

        _BindEvent(_GetButton((int)EButtons.ExitButton).gameObject, _ExitButton);
    }

    #region Event
    private void _ExitButton()
    {
        ClosePopupUI();
    }
    #endregion
}
