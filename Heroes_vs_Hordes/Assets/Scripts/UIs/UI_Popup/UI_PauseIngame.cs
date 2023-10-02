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
        var waveIndices = Manager.Instance.Data.ChapterInfoDataList[Define.CURRENT_CHAPTER_INDEX].WaveIndex;
        for (int ii = 0; ii < Manager.Instance.Ingame.TotalWaveIndex; ++ii)
        {
            switch (waveIndices[ii])
            {
                case Define.INDEX_NORMAL_BATTLE_WAVE:
                    _InitWavePanel<UI_BattleWave>(Define.RESOURCE_UI_NORMAL_BATTLE_WAVE, waveIndices.Length, ii);
                    break;
                case Define.INDEX_GOLD_RUSH_WAVE:
                    _InitWavePanel<UI_GoldRushWave>(Define.RESOURCE_UI_GOLD_RUSH_WAVE, waveIndices.Length, ii);
                    break;
                case Define.INDEX_BOSS_BATTLE_WAVE:
                    _InitWavePanel<UI_BattleWave>(Define.RESOURCE_UI_BOSS_BATTLE_WAVE, waveIndices.Length, ii);
                    break;
                default:
                    Debug.LogError($"Invalid Wave Index : {waveIndices[ii]}");
                    return;
            }
        }
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
            var abilityUIGO = Manager.Instance.UI.GetElementUI(Define.RESOURCE_UI_ABILITY);
            var abilityUI = Utils.GetOrAddComponent<UI_Ability>(abilityUIGO);
            abilityUI.InitAbilityUI(_ownedWeaponContents.transform);
            _weaponAbilityList.Add(abilityUI);
        }

        for (int ii = 0; ii < _bookAbilityList.Count; ++ii)
            _bookAbilityList[ii].ReturnAbilityUI();
        _bookAbilityList.Clear();

        for (int ii = 0; ii < CREATE_ABILITY_UI_COUNT; ++ii)
        {
            var abilityUIGO = Manager.Instance.UI.GetElementUI(Define.RESOURCE_UI_ABILITY);
            var abilityUI = Utils.GetOrAddComponent<UI_Ability>(abilityUIGO);
            abilityUI.InitAbilityUI(_ownedBookContents.transform);
            _bookAbilityList.Add(abilityUI);
        }
    }

    public void UpdateWeaponAbilityUI()
    {
        var ownedAllWeaponList = Manager.Instance.Ingame.OwnedWeaponList;
        for (int ii = 0; ii < ownedAllWeaponList.Count; ++ii)
            _weaponAbilityList[ii].UpdateAbilityUI(ownedAllWeaponList[ii]);
    }

    public void UpdateBookAbilityUI()
    {
        var ownedAllBookList = Manager.Instance.Ingame.OwnedBookList;
        for (int ii = 0; ii < ownedAllBookList.Count; ++ii)
            _bookAbilityList[ii].UpdateAbilityUI(ownedAllBookList[ii]);
    }

    private void _InitWavePanel<T>(string wavePanelName, int totalWaveIndex, int index) where T : UI_Wave
    {
        var wave = Manager.Instance.UI.GetElementUI(wavePanelName);
        var waveUI = Utils.GetOrAddComponent<T>(wave);
        if (Define.FOUR_WAVE == totalWaveIndex)
            waveUI.InitWaveUI(index, _totalWaves.transform, Define.FOUR_WAVE_PANEL_TRANSFORMS[index].PanelPosition, Define.FOUR_WAVE_PANEL_TRANSFORMS[index].PanelSize, Define.FOUR_WAVE_PANEL_TRANSFORMS[index].IconSize);
        else if (Define.FIVE_WAVE == totalWaveIndex)
            waveUI.InitWaveUI(index, _totalWaves.transform, Define.FIVE_WAVE_PANEL_TRANSFORMS[index].PanelPosition, Define.FIVE_WAVE_PANEL_TRANSFORMS[index].PanelSize, Define.FIVE_WAVE_PANEL_TRANSFORMS[index].IconSize);
        Utils.SetActive(wave, true);
        _wavePanelList.Add(waveUI);
    }
}
