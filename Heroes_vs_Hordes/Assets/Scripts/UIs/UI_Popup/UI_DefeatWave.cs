using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DefeatWave : UI_Popup
{
    private enum EButtons
    {
        ExitIngameButton
    }

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));

        _BindEvent(_GetButton((int)EButtons.ExitIngameButton).gameObject, _ExitIngame);
    }


    #region Event
    private void _ExitIngame()
    {
        var manager = Manager.Instance;
        manager.Ingame.DefeatIngame = true;
        manager.Ingame.ExitIngame();
    }
    #endregion
}
