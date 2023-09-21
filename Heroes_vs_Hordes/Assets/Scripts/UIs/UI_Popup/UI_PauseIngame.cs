using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PauseIngame : UI_Popup
{
    private enum EButtons
    {
        RestartButton,
        GiveUpIngameButton
    }

    private enum EGameObjects
    {
        TotalWaves,
        OwnedWeaponContents,
        OwnedBookContents
    }

    private GameObject _totalWaves;
    private GameObject _ownedWeaponContents;
    private GameObject _ownedBookContents;

    private List<UI_Wave> _wavePanelList = new List<UI_Wave>();
    private List<UI_Ability> _weaponAbilityList = new List<UI_Ability>();
    private List<UI_Ability> _bookAbilityList = new List<UI_Ability>();

    private const int CREATE_ABILITY_UI_COUNT = 5;

    private readonly WavePanelTransform[] FOUR_WAVE_PANEL_TRANSFORMS = new WavePanelTransform[]
    {
        new WavePanelTransform() { PanelPosition = new Vector2(-125f, 0f), PanelSize = new Vector2(125f, 40f), IconSize = new Vector2(50f, 55f) },
        new WavePanelTransform() { PanelPosition = Vector2.zero, PanelSize = new Vector2(125f, 40f), IconSize = new Vector2(50f, 55f) },
        new WavePanelTransform() { PanelPosition = new Vector2(125f, 0f), PanelSize = new Vector2(125f, 40f), IconSize = new Vector2(50f, 55f) },
        new WavePanelTransform() { PanelPosition = new Vector2(187.5f, 30f), PanelSize = new Vector2(100f, 100f), IconSize = new Vector2(50f, 55f) }
    };

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindGameObject(typeof(EGameObjects));

        _totalWaves = _GetGameObject((int)EGameObjects.TotalWaves);
        _ownedWeaponContents = _GetGameObject((int)EGameObjects.OwnedWeaponContents);
        _ownedBookContents = _GetGameObject((int)EGameObjects.OwnedBookContents);

        _BindEvent(_GetButton((int)EButtons.RestartButton).gameObject, _RestartIngame);
        _BindEvent(_GetButton((int)EButtons.GiveUpIngameButton).gameObject, _GiveUpIngame);
    }

    #region Event
    private void _RestartIngame()
    {
        Manager.Instance.Ingame.ControlIngame(true);
        _ClosePopupUI();
    }

    private void _GiveUpIngame()
    {
        var manager = Manager.Instance;
        manager.Ingame.ExitIngameForce = true;
        manager.Ingame.ExitIngame();
    }
    #endregion

    public void InitWavePanel()
    {
        // 사용한 WaveUI를 반납 후 리스트 정리
        for (int ii = 0; ii < _wavePanelList.Count; ++ii)
            _wavePanelList[ii].ReturnWaveUI();
        _wavePanelList.Clear();

        // 사용할 WaveUI를 가져옴
        for (int ii = 0; ii < Manager.Instance.Ingame.TotalWaveIndex - 1; ++ii)
            _InitWavePanel<UI_NormalBattleWave>(Define.RESOURCE_UI_NORMAL_BATTLE_WAVE, ii);
        _InitWavePanel<UI_GoldRushWave>(Define.RESOURCE_UI_GOLD_RUSH_WAVE, Manager.Instance.Ingame.TotalWaveIndex - 1);
    }

    public void UpdateWavePanel()
    {
        for (int ii = 0; ii < _wavePanelList.Count; ++ii)
            _wavePanelList[ii].UpdateWaveUI();
    }

    public void InitAbilityUI()
    {
        for (int ii = 0; ii < _weaponAbilityList.Count; ++ii)
            _weaponAbilityList[ii].ReturnAbilityUI();
        _weaponAbilityList.Clear();

        for (int ii = 0; ii < CREATE_ABILITY_UI_COUNT; ++ii)
        {
            var abilityGO = Manager.Instance.UI.GetElementUI(Define.RESOURCE_UI_ABILITY);
            var abilityUI = Utils.GetOrAddComponent<UI_Ability>(abilityGO);
            abilityUI.InitAbilityUI(_ownedWeaponContents.transform);
            _weaponAbilityList.Add(abilityUI);
        }

        for (int ii = 0; ii < _bookAbilityList.Count; ++ii)
            _bookAbilityList[ii].ReturnAbilityUI();
        _bookAbilityList.Clear();

        for (int ii = 0; ii < CREATE_ABILITY_UI_COUNT; ++ii)
        {
            var abilityGO = Manager.Instance.UI.GetElementUI(Define.RESOURCE_UI_ABILITY);
            var abilityUI = Utils.GetOrAddComponent<UI_Ability>(abilityGO);
            abilityUI.InitAbilityUI(_ownedBookContents.transform);
            _bookAbilityList.Add(abilityUI);
        }
    }

    public void UpdateWeaponAbilityUI()
    {
        for (int ii = 0; ii < _weaponAbilityList.Count; ++ii)
        {

        }
    }

    public void UpdateBookAbilityUI()
    {

    }

    private void _InitWavePanel<T>(string wavePanelName, int index) where T : UI_Wave
    {
        var wave = Manager.Instance.UI.GetElementUI(wavePanelName);
        var waveUI = Utils.GetOrAddComponent<T>(wave);
        waveUI.InitWaveUI(index, _totalWaves.transform, FOUR_WAVE_PANEL_TRANSFORMS[index].PanelPosition, FOUR_WAVE_PANEL_TRANSFORMS[index].PanelSize, FOUR_WAVE_PANEL_TRANSFORMS[index].IconSize);
        Utils.SetActive(wave, true);
        _wavePanelList.Add(waveUI);
    }
}
