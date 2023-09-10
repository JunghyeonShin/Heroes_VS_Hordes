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
        manager.Ingame.ExitIngame();
        manager.Object.ReturnMap(Define.RESOURCE_MAP_00);
        manager.Object.ReturnHero(Define.RESOURCE_HERO_ARCANE_MAGE);
        Utils.SetActive(manager.Object.MapCollisionArea, false);
        Utils.SetActive(manager.Object.MonsterSpawner, false);
        manager.CameraController.SetFollower();
        manager.UI.ShowSceneUI<UI_MainScene>(Define.RESOURCE_UI_MAIN_SCENE);
    }

    private void _GetReward()
    {

    }
    #endregion
}
