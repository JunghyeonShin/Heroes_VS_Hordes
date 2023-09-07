using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_IngameScene : UI_Scene
{
    private enum EButtons
    {
        PauseButton
    }

    private enum EGameObjects
    {
        TimeCheck,
        MonsterCheck,
        WavePanel
    }

    private enum ESliders
    {
        LevelSlider
    }

    private enum ETexts
    {
        TimeText,
        MonsterText,
        WaveText
    }

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindGameObject(typeof(EGameObjects));
        _BindSlider(typeof(ESliders));
        _BindText(typeof(ETexts));

        _BindEvent(_GetButton((int)EButtons.PauseButton).gameObject, _PauseIngame);
    }

    private void _PauseIngame()
    {

    }
}
