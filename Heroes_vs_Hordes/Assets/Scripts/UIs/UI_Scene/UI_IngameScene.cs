using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
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
        AnnihilationModePanel,
        FinishWavePanel
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

    private Animator _wavePanelAnimator;
    private Animator _annihilationModePanelAnimator;
    private Animator _finishWavePanelAnimator;
    private Slider _expSlider;
    private TextMeshProUGUI _levelText;
    private TextMeshProUGUI _timeText;
    private TextMeshProUGUI _monsterText;
    private TextMeshProUGUI _waveText;

    private const float DELAY_SHOWING_WAVE_PANEL = 0.2f;
    private const float DELAY_FINISHED_WAVE_PANEL = 1.2f;
    private const float DELAY_SPAWN_MONSTER = 0.2f;
    private const float SIXTY_SECONDS = 60f;
    private const int ZERO_SECOND = 0;
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

        _wavePanelAnimator = _GetGameObject((int)EGameObjects.WavePanel).GetComponent<Animator>();
        _annihilationModePanelAnimator = _GetGameObject((int)EGameObjects.AnnihilationModePanel).GetComponent<Animator>();
        _finishWavePanelAnimator = _GetGameObject((int)EGameObjects.FinishWavePanel).GetComponent<Animator>();

        _BindEvent(_GetButton((int)EButtons.PauseButton).gameObject, _PauseIngame);

        _expSlider = _GetSlider((int)ESliders.ExpSlider);

        _levelText = _GetText((int)ETexts.LevelText);
        _timeText = _GetText((int)ETexts.TimeText);
        _monsterText = _GetText((int)ETexts.MonsterText);
        _waveText = _GetText((int)ETexts.WaveText);

        var ingame = Manager.Instance.Ingame;
        ingame.ShowWavePanelHandler -= _SetWaveIndex;
        ingame.ShowWavePanelHandler += _SetWaveIndex;
        ingame.ChangeProgressTimeHandler -= _SetTimeText;
        ingame.ChangeProgressTimeHandler += _SetTimeText;
        ingame.ChangeModeHandler -= _ChangeMode;
        ingame.ChangeModeHandler += _ChangeMode;

        ingame.ChangeHeroExpHandler -= _SetExpSlider;
        ingame.ChangeHeroExpHandler += _SetExpSlider;
        ingame.ChangeHeroLevelHandler -= _SetLevel;
        ingame.ChangeHeroLevelHandler += _SetLevel;
        ingame.EnhanceHeroAbilityHandler -= _EnhanceHeroAbility;
        ingame.EnhanceHeroAbilityHandler += _EnhanceHeroAbility;

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

    private void _SetWaveIndex(string wavePanelText)
    {
        _waveText.text = wavePanelText;
        _ShowWavePanel().Forget();
    }

    private async UniTaskVoid _ShowWavePanel()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_SHOWING_WAVE_PANEL));

        _wavePanelAnimator.SetTrigger(ANIMATOR_TRIGGER_MOVE_WAVE_PANEL);
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_FINISHED_WAVE_PANEL));

        Manager.Instance.Ingame.CurrentWave.ProgressWave = true;
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_SPAWN_MONSTER));

        Manager.Instance.Ingame.StartSpawnMonster();
    }

    private void _SetTimeText(float time)
    {
        var minute = Mathf.FloorToInt(time / SIXTY_SECONDS);
        var second = Mathf.FloorToInt(time % SIXTY_SECONDS);
        if (ZERO_SECOND == second)
            _timeText.text = $"{minute}:00";
        else
            _timeText.text = $"{minute}:{second}";
    }

    private void _ChangeMode(int mode)
    {
        Utils.SetActive(_timeCheckPanel, Define.INDEX_TIME_ATTACK_MODE == mode);
        Utils.SetActive(_monsterCheckPanel, Define.INDEX_ANNIHILATION_MODE == mode);
        if (_monsterCheckPanel.activeSelf)
        {
            _annihilationModePanelAnimator.SetTrigger(ANIMATOR_TRIGGER_MOVE_WAVE_PANEL);
            Manager.Instance.Ingame.StopSpawnMonster();
        }
    }

    private void _SetExpSlider(float value)
    {
        _expSlider.value = value;
    }

    private void _SetLevel(int level)
    {
        _levelText.text = level.ToString();
    }

    private void _EnhanceHeroAbility()
    {
        Manager.Instance.UI.ShowPopupUI<UI_EnhanceHeroAbility>(Define.RESOURCE_UI_ENHANCE_HERO_ABILITY, (enhanceHeroAbilityUI) =>
        {
            Manager.Instance.Ingame.ControlIngame(false);
        });
    }

    private void _SetMonsterText()
    {
        if (false == _monsterCheckPanel.activeSelf)
            return;

        var ingame = Manager.Instance.Ingame;
        _monsterText.text = ingame.RemainingMonsterCount.ToString();
        if (ingame.RemainingMonsterCount <= NON_REMAINING_MONSTER_COUNT)
            _ShowFinishWavePanel().Forget();
    }

    private async UniTaskVoid _ShowFinishWavePanel()
    {
        _finishWavePanelAnimator.SetTrigger(ANIMATOR_TRIGGER_MOVE_WAVE_PANEL);
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_FINISHED_WAVE_PANEL));

        Manager.Instance.Ingame.ClearIngame();
    }
}
