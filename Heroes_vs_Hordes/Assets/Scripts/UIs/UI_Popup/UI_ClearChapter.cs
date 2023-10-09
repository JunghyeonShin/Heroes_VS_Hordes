using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ClearChapter : UI_Popup
{
    private enum EButtons
    {
        ExitIngameButton,
        RewardChapterButton
    }

    private enum EGameObjects
    {
        ClearChapterIndex,
        ClosedBox,
        OpenedBox
    }

    private enum ETexts
    {
        ClearChapterIndexText
    }

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindGameObject(typeof(EGameObjects));
        _BindText(typeof(ETexts));

        _BindEvent(_GetButton((int)EButtons.ExitIngameButton).gameObject, _ExitIngame);
        _BindEvent(_GetButton((int)EButtons.RewardChapterButton).gameObject, _GetReward);
    }

    #region Event
    private void _ExitIngame()
    {
        var manager = Manager.Instance;
        if (manager.Ingame.CurrentChapterIndex > manager.SaveData.ClearChapter)
            manager.SaveData.ClearChapter = manager.Ingame.CurrentChapterIndex;
        manager.Ingame.ExitIngame();
    }

    private void _GetReward()
    {

    }
    #endregion
}
