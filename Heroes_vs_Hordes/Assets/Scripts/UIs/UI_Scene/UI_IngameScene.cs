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
        ExpPanel,
        ChapterCheckPanel,
        TimeCheckPanel,
        MonsterCheckPanel,
        BossMonsterHealthPanel,
        GoldPanel,
        WavePanel,
        AnnihilationModePanel,
        FinishWavePanel
    }

    private enum ESliders
    {
        ExpSlider,
        BossMonsterHealthSlider
    }

    private enum ETexts
    {
        LevelText,
        TimeText,
        MonsterText,
        GoldText,
        WaveText,
    }

    private GameObject _expPanel;
    private GameObject _chapterCheckPanel;
    private GameObject _timeCheckPanel;
    private GameObject _monsterCheckPanel;
    private GameObject _bossMonsterhealthPanel;
    private GameObject _goldPanel;

    private Animator _wavePanelAnimator;
    private Animator _annihilationModePanelAnimator;
    private Animator _finishWavePanelAnimator;
    private Slider _expSlider;
    private Slider _bossMonsterHealthSlider;
    private TextMeshProUGUI _levelText;
    private TextMeshProUGUI _timeText;
    private TextMeshProUGUI _monsterText;
    private TextMeshProUGUI _goldText;
    private TextMeshProUGUI _waveText;

    private const float DELAY_SHOWING_WAVE_PANEL = 0.2f;
    private const float DELAY_FINISHED_WAVE_PANEL = 1.2f;
    private const float DELAY_SPAWN_MONSTER = 0.2f;
    private const float SIXTY_SECONDS = 60f;
    private const int TEN_SECONDS = 10;
    private const int NON_REMAINING_MONSTER_COUNT = 0;
    private const string ANIMATOR_TRIGGER_MOVE_WAVE_PANEL = "MoveWavePanel";

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindGameObject(typeof(EGameObjects));
        _BindSlider(typeof(ESliders));
        _BindText(typeof(ETexts));

        _expPanel = _GetGameObject((int)EGameObjects.ExpPanel);
        _chapterCheckPanel = _GetGameObject((int)EGameObjects.ChapterCheckPanel);
        _timeCheckPanel = _GetGameObject((int)EGameObjects.TimeCheckPanel);
        _monsterCheckPanel = _GetGameObject((int)EGameObjects.MonsterCheckPanel);
        _bossMonsterhealthPanel = _GetGameObject((int)EGameObjects.BossMonsterHealthPanel);
        _goldPanel = _GetGameObject((int)EGameObjects.GoldPanel);

        _wavePanelAnimator = _GetGameObject((int)EGameObjects.WavePanel).GetComponent<Animator>();
        _annihilationModePanelAnimator = _GetGameObject((int)EGameObjects.AnnihilationModePanel).GetComponent<Animator>();
        _finishWavePanelAnimator = _GetGameObject((int)EGameObjects.FinishWavePanel).GetComponent<Animator>();

        _BindEvent(_GetButton((int)EButtons.PauseButton).gameObject, _PauseIngame);

        _expSlider = _GetSlider((int)ESliders.ExpSlider);
        _bossMonsterHealthSlider = _GetSlider((int)ESliders.BossMonsterHealthSlider);

        _levelText = _GetText((int)ETexts.LevelText);
        _timeText = _GetText((int)ETexts.TimeText);
        _monsterText = _GetText((int)ETexts.MonsterText);
        _goldText = _GetText((int)ETexts.GoldText);
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
        ingame.LevelUpHeroAbilityHandler -= _LevelUpHeroAbility;
        ingame.LevelUpHeroAbilityHandler += _LevelUpHeroAbility;

        ingame.RemainingMonsterHandler -= _SetMonsterText;
        ingame.RemainingMonsterHandler += _SetMonsterText;
        ingame.ChangeBossMonsterHealthHandler -= _SetBossMonsterHealth;
        ingame.ChangeBossMonsterHealthHandler += _SetBossMonsterHealth;

        ingame.ChangeGoldHandler -= _SetGoldText;
        ingame.ChangeGoldHandler += _SetGoldText;
    }

    #region Event
    private void _PauseIngame()
    {
        Manager.Instance.UI.ShowPopupUI<UI_PauseIngame>(Define.RESOURCE_UI_PAUSE_INGAME, (pauseIngameUI) =>
        {
            pauseIngameUI.UpdateWavePanel();
            pauseIngameUI.UpdateWeaponAbilityUI();
            pauseIngameUI.UpdateBookAbilityUI();
        });
        Manager.Instance.Ingame.ControlIngame(false);
    }
    #endregion

    public void FinishIngame()
    {
        _ShowFinishWavePanel().Forget();
    }

    private void _SetWaveIndex(string wavePanelText)
    {
        var waveIndex = Manager.Instance.Data.ChapterInfoDataList[Manager.Instance.Ingame.CurrentChapterIndex].WaveIndex[Manager.Instance.Ingame.CurrentWaveIndex];
        if (Define.INDEX_NORMAL_BATTLE_WAVE == waveIndex)
        {
            Utils.SetActive(_expPanel, true);
            Utils.SetActive(_chapterCheckPanel, true);
            Utils.SetActive(_monsterCheckPanel, false);
            Utils.SetActive(_bossMonsterhealthPanel, false);
            Utils.SetActive(_goldPanel, false);
        }
        else if (Define.INDEX_GOLD_RUSH_WAVE == waveIndex)
        {
            Utils.SetActive(_expPanel, false);
            Utils.SetActive(_chapterCheckPanel, false);
            Utils.SetActive(_monsterCheckPanel, false);
            Utils.SetActive(_bossMonsterhealthPanel, false);
            Utils.SetActive(_goldPanel, true);
        }
        else if (Define.INDEX_BOSS_BATTLE_WAVE == waveIndex)
        {
            Utils.SetActive(_expPanel, true);
            Utils.SetActive(_chapterCheckPanel, false);
            Utils.SetActive(_monsterCheckPanel, false);
            Utils.SetActive(_bossMonsterhealthPanel, true);
            Utils.SetActive(_goldPanel, false);
        }

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

        var waveIndex = Manager.Instance.Data.ChapterInfoDataList[Manager.Instance.Ingame.CurrentChapterIndex].WaveIndex[Manager.Instance.Ingame.CurrentWaveIndex];
        if (Define.INDEX_NORMAL_BATTLE_WAVE == waveIndex || Define.INDEX_GOLD_RUSH_WAVE == waveIndex)
            Manager.Instance.Ingame.StartSpawnMonster();
    }

    private void _SetTimeText(float time)
    {
        var minute = Mathf.FloorToInt(time / SIXTY_SECONDS);
        var second = Mathf.FloorToInt(time % SIXTY_SECONDS);
        if (second < TEN_SECONDS)
            _timeText.text = $"{minute}:0{second}";
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

    private void _LevelUpHeroAbility()
    {
        Manager.Instance.UI.ShowPopupUI<UI_LevelUpHero>(Define.RESOURCE_UI_LEVEL_UP_HERO, (levelUpHeroUI) =>
        {
            levelUpHeroUI.UpdateSelectAbilityPanel();
            levelUpHeroUI.UpdateWeaponAbilityUI();
            levelUpHeroUI.UpdateBookAbilityUI();
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

    private void _SetBossMonsterHealth(float value)
    {
        _bossMonsterHealthSlider.value = value;
    }

    private async UniTaskVoid _ShowFinishWavePanel()
    {
        _finishWavePanelAnimator.SetTrigger(ANIMATOR_TRIGGER_MOVE_WAVE_PANEL);
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_FINISHED_WAVE_PANEL));

        Manager.Instance.Ingame.ClearIngame();
    }

    private void _SetGoldText()
    {
        if (false == _goldPanel.activeSelf)
            return;

        _goldText.text = Manager.Instance.Ingame.AcquiredGold.ToString();
    }
}
