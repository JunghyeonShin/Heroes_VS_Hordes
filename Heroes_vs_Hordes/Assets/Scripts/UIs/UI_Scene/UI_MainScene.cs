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
            // UI_PauseIngame의 웨이브 진행도 초기 세팅
            var pauseIngameUI = Manager.Instance.UI.FindUI<UI_PauseIngame>(Define.RESOURCE_UI_PAUSE_INGAME);
            pauseIngameUI.InitWavePanel();

            // UI_ClearWave의 웨이브 진행도 초기 세팅
            var clearWaveUI = Manager.Instance.UI.FindUI<UI_ClearWave>(Define.RESOURCE_UI_CLEAR_WAVE);
            clearWaveUI.InitWavePanel();

            // 맵 생성
            Manager.Instance.Object.GetMap(Define.RESOURCE_MAP_00, (mapGO) =>
            {
                var mapController = Utils.GetOrAddComponent<MapController>(mapGO);
                Utils.SetActive(mapGO, true);

                // 영웅 생성
                Manager.Instance.Object.GetHero(Define.RESOURCE_HERO_ARCANE_MAGE, (hero) =>
                {
                    var heroController = Utils.GetOrAddComponent<HeroController>(hero);
                    mapController.SetHeroController(heroController);
                    Utils.SetActive(hero, true);

                    {
                        var chaseHero = Utils.GetOrAddComponent<ChaseHero>(Manager.Instance.Object.MapCollisionArea);
                        chaseHero.HeroTransform = hero.transform;
                        Utils.SetActive(Manager.Instance.Object.MapCollisionArea, true);
                    }

                    // 카메라 팔로워 세팅
                    Manager.Instance.CameraController.SetFollower(hero.transform);
                });
            });

            // 인게임 시작
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
