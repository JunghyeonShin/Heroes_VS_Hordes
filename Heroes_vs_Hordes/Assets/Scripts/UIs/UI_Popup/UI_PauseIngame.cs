using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PauseIngame : UI_Popup
{
    private enum EButtons
    {
        RestartButton,
        GiveUpIngameButton
    }

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));

        _BindEvent(_GetButton((int)EButtons.RestartButton).gameObject, _RestartIngame);
        _BindEvent(_GetButton((int)EButtons.GiveUpIngameButton).gameObject, _GiveUpIngame);
    }

    #region Event
    private void _RestartIngame()
    {
        Manager.Instance.Ingame.ControlIngame(true);
        _ClosePopupUI();
    }

    private void _GiveUpIngame()
    {
        Manager.Instance.Ingame.GiveUpIngame();
        Manager.Instance.UI.ShowSceneUI<UI_MainScene>(Define.RESOURCE_UI_MAIN_SCENE);
    }
    #endregion
}
