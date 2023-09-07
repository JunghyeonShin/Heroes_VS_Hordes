using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MainScene : UI_Scene
{
    private enum EButtons
    {
        PlayButton,
        NextButton,
        PrevButton
    }

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));

        _BindEvent(_GetButton((int)EButtons.PlayButton).gameObject, _PlayChapter);
        _BindEvent(_GetButton((int)EButtons.NextButton).gameObject, _ShowNextChapter);
        _BindEvent(_GetButton((int)EButtons.PrevButton).gameObject, _ShowPrevChapter);
    }

    private void _PlayChapter()
    {

    }

    private void _ShowNextChapter()
    {

    }

    private void _ShowPrevChapter()
    {

    }
}
