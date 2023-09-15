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
        Manager.Instance.UI.ShowSceneUI<UI_IngameScene>(Define.RESOURCE_UI_INGAME_SCENE);
        Manager.Instance.UI.ShowPopupUI<UI_Loading>(Define.RESOURCE_UI_LOADING, (loadingUI) =>
        {
            loadingUI.OnLoadCompleteHandler -= Manager.Instance.Ingame.StartIngame;
            loadingUI.OnLoadCompleteHandler += Manager.Instance.Ingame.StartIngame;
            loadingUI.StartLoading();

            Manager.Instance.Ingame.InitIngame(loadingUI);
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
