using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_MainScene : UI_Scene
{
    private enum EButtons
    {
        PlayButton,
        NextButton,
        PrevButton
    }

    private enum ETexts
    {
        ChapterText
    }

    private GameObject _nextButton;
    private GameObject _prevButton;
    private TextMeshProUGUI _chapterText;

    private const int FIRST_WAVE_INDEX = 0;

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindText(typeof(ETexts));

        _nextButton = _GetButton((int)EButtons.NextButton).gameObject;
        _prevButton = _GetButton((int)EButtons.PrevButton).gameObject;

        _BindEvent(_GetButton((int)EButtons.PlayButton).gameObject, _PlayChapter);
        _BindEvent(_nextButton, _ShowNextChapter);
        _BindEvent(_prevButton, _ShowPrevChapter);

        _chapterText = _GetText((int)ETexts.ChapterText);
    }

    #region Event
    private void _PlayChapter()
    {
        Manager.Instance.UI.ShowSceneUI<UI_IngameScene>(Define.RESOURCE_UI_INGAME_SCENE, (ingameSceneUI) =>
        {
            var pauseIngameUI = Manager.Instance.UI.FindUI<UI_PauseIngame>(Define.RESOURCE_UI_PAUSE_INGAME);
            pauseIngameUI.InitWavePanel();

            Manager.Instance.Ingame.StartIngame(FIRST_WAVE_INDEX);
        });
    }

    private void _ShowNextChapter()
    {

    }

    private void _ShowPrevChapter()
    {

    }
    #endregion
}
