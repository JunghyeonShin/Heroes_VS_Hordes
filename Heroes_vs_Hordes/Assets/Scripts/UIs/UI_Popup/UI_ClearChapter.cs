using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ClearChapter : UI_Popup
{
    private enum EButtons
    {
        CloseButton,
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

        _BindEvent(_GetButton((int)EButtons.CloseButton).gameObject, _MoveToMainScene);
        _BindEvent(_GetButton((int)EButtons.RewardChapterButton).gameObject, _GetReward);
    }

    private void _MoveToMainScene()
    {

    }

    private void _GetReward()
    {

    }
}
