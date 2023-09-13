using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_IngameScene : UI_Scene
{
    private enum EButtons
    {
        PauseButton
    }

    private enum EGameObjects
    {
        TimeCheckPanel,
        MonsterCheckPanel,
        WavePanel,
        WaveImage
    }

    private enum ESliders
    {
        ExpSlider
    }

    private enum ETexts
    {
        LevelText,
        TimeText,
        MonsterText,
        WaveText
    }

    private GameObject _timeCheckPanel;
    private GameObject _monsterCheckPanel;
    private GameObject _waveImage;

    private Animator _wavePanelAnimator;
    private Slider _expSlider;
    private TextMeshProUGUI _levelText;
    private TextMeshProUGUI _timeText;
    private TextMeshProUGUI _monsterText;
    private TextMeshProUGUI _waveText;

    private const float DELAY_TIME_SHOWING_WAVE_PANEL = 1f;
    private const float FINISHED_TIME_SHOWING_WAVE_PANEL = 1f;
    private const float SIXTY_SECONDS = 60f;
    private const float DELAY_ENHANCE_HERO_ABILITY = 1.2f;
    private const int INIT_LEVEL = 1;
    private const int ADJUST_WAVE_INDEX = 1;
    private const int NON_REMAINING_MONSTER_COUNT = 0;
    private const string ANIMATOR_TRIGGER_MOVE_WAVE_PANEL = "MoveWavePanel";

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindGameObject(typeof(EGameObjects));
        _BindSlider(typeof(ESliders));
        _BindText(typeof(ETexts));

        _timeCheckPanel = _GetGameObject((int)EGameObjects.TimeCheckPanel);
        _monsterCheckPanel = _GetGameObject((int)EGameObjects.MonsterCheckPanel);
        _waveImage = _GetGameObject((int)EGameObjects.WaveImage);

        _wavePanelAnimator = _GetGameObject((int)EGameObjects.WavePanel).GetComponent<Animator>();
        _BindEvent(_GetButton((int)EButtons.PauseButton).gameObject, _PauseIngame);

        _expSlider = _GetSlider((int)ESliders.ExpSlider);

        _levelText = _GetText((int)ETexts.LevelText);
        _timeText = _GetText((int)ETexts.TimeText);
        _monsterText = _GetText((int)ETexts.MonsterText);
        _waveText = _GetText((int)ETexts.WaveText);

        var ingame = Manager.Instance.Ingame;
        ingame.WavePanelHandler -= _SetWaveIndex;
        ingame.WavePanelHandler += _SetWaveIndex;
        ingame.TimePassHandler -= _SetTimeText;
        ingame.TimePassHandler += _SetTimeText;
        ingame.ChangeHeroExpHandler -= _SetExpSlider;
        ingame.ChangeHeroExpHandler += _SetExpSlider;
        ingame.ChangeHeroLevelHandler -= _SetLevel;
        ingame.ChangeHeroLevelHandler += _SetLevel;
        ingame.ChangeModeHandler -= _ChangeMode;
        ingame.ChangeModeHandler += _ChangeMode;
        ingame.RemainingMonsterHandler -= _SetMonsterText;
        ingame.RemainingMonsterHandler += _SetMonsterText;
    }

    #region Event
    private void _PauseIngame()
    {
        Manager.Instance.UI.ShowPopupUI<UI_PauseIngame>(Define.RESOURCE_UI_PAUSE_INGAME, (pauseIngameUI) =>
        {
            pauseIngameUI.UpdateWavePanel();
        });
        Manager.Instance.Ingame.ControlIngame(false);
    }
    #endregion 

    private void _SetWaveIndex()
    {
        _waveText.text = $"¿þÀÌºê {Manager.Instance.Ingame.CurrentWaveIndex + ADJUST_WAVE_INDEX}";
        _ShowWavePanel().Forget();
    }

    private async UniTaskVoid _ShowWavePanel()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_TIME_SHOWING_WAVE_PANEL));

        _wavePanelAnimator.SetTrigger(ANIMATOR_TRIGGER_MOVE_WAVE_PANEL);
        await UniTask.Delay(TimeSpan.FromSeconds(FINISHED_TIME_SHOWING_WAVE_PANEL));

        Manager.Instance.Ingame.ProgressWave = true;
    }

    private void _SetTimeText(float time)
    {
        var minute = Mathf.FloorToInt(time / SIXTY_SECONDS);
        var second = Mathf.FloorToInt(time % SIXTY_SECONDS);
        _timeText.text = $"{minute}:{second}";
    }

    private void _SetExpSlider(float value)
    {
        _expSlider.value = value;
    }

    private void _SetLevel(int level)
    {
        _levelText.text = level.ToString();
        if (level > INIT_LEVEL)
            _EnhanceHeroAbility().Forget();
    }

    private async UniTaskVoid _EnhanceHeroAbility()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_ENHANCE_HERO_ABILITY));

        Manager.Instance.UI.ShowPopupUI<UI_EnhanceHeroAbility>(Define.RESOURCE_UI_ENHANCE_HERO_ABILITY, (enhanceHeroAbilityUI) =>
        {
            Manager.Instance.Ingame.ControlIngame(false);
        });
    }

    private void _ChangeMode(int mode)
    {
        Utils.SetActive(_timeCheckPanel, Define.TIME_ATTACK_MODE == mode);
        Utils.SetActive(_monsterCheckPanel, Define.TIME_ATTACK_MODE != mode);
        if (_monsterCheckPanel.activeSelf)
            _monsterText.text = mode.ToString();
    }

    private void _SetMonsterText(int remainingCount)
    {
        _monsterText.text = remainingCount.ToString();
        if (remainingCount <= NON_REMAINING_MONSTER_COUNT)
            Manager.Instance.Ingame.ClearIngame();
    }
}
