using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_MainScene : UI_Scene
{
    private enum EButtons
    {
        PlayButton,
        NextChapterButton,
        PrevChapterButton
    }

    private enum ETexts
    {
        ChapterText
    }

    private GameObject _nextChapterButton;
    private GameObject _prevChapterButton;
    private TextMeshProUGUI _chapterText;

    private int _selectChapter;

    private const int INIT_CHAPTER_INDEX = 0;

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindText(typeof(ETexts));

        _nextChapterButton = _GetButton((int)EButtons.NextChapterButton).gameObject;
        _prevChapterButton = _GetButton((int)EButtons.PrevChapterButton).gameObject;

        _BindEvent(_GetButton((int)EButtons.PlayButton).gameObject, _PlayChapter);
        _BindEvent(_nextChapterButton, _ClickNextChapterButton);
        _BindEvent(_prevChapterButton, _ClickPrevChapterButton);

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

            Manager.Instance.Ingame.InitIngame(_selectChapter, loadingUI);
        });
    }

    private void _ClickNextChapterButton()
    {
        SetChapter(++_selectChapter);
    }

    private void _ClickPrevChapterButton()
    {
        SetChapter(--_selectChapter);
    }
    #endregion

    public void SetChapter(int chapterIndex)
    {
        _selectChapter = chapterIndex;
        var lastChapter = Manager.Instance.Data.ChapterInfoDataList.Count - Define.ADJUSE_CHAPTER_INDEX;
        if (_selectChapter <= INIT_CHAPTER_INDEX)
        {
            _selectChapter = INIT_CHAPTER_INDEX;
            if (_selectChapter > Manager.Instance.SaveData.ClearChapter)
                _ActiveChpaterButton(false, false);
            else
                _ActiveChpaterButton(true, false);
        }
        else if (_selectChapter >= lastChapter)
        {
            _selectChapter = lastChapter;
            _ActiveChpaterButton(false, true);
        }
        else
        {
            if (_selectChapter > Manager.Instance.SaveData.ClearChapter)
                _ActiveChpaterButton(false, true);
            else
                _ActiveChpaterButton(true, true);
        }

        _chapterText.text = Manager.Instance.Data.ChapterInfoDataList[_selectChapter].ChapterName;
    }

    private void _ActiveChpaterButton(bool activeNextChapter, bool activePrevChapter)
    {
        Utils.SetActive(_nextChapterButton, activeNextChapter);
        Utils.SetActive(_prevChapterButton, activePrevChapter);
    }
}
