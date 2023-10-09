using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ClearWave : UI_Popup
{
    private enum EButtons
    {
        StartNextWaveButton
    }

    private enum EGameObjects
    {
        TotalWaves
    }

    private enum ETexts
    {
        ClearWaveText,
        RewardGoldText
    }

    private TextMeshProUGUI _clearWaveText;

    private GameObject _totalWaves;
    private List<UI_Wave> _wavePanelList = new List<UI_Wave>();

    private const int PREV_WAVE_INDEX = 1;

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindGameObject(typeof(EGameObjects));
        _BindText(typeof(ETexts));

        _totalWaves = _GetGameObject((int)EGameObjects.TotalWaves);

        _BindEvent(_GetButton((int)EButtons.StartNextWaveButton).gameObject, _StartNextWave);

        _clearWaveText = _GetText((int)ETexts.ClearWaveText);
    }

    #region Event
    private void _StartNextWave()
    {
        ClosePopupUI();
        Manager.Instance.Ingame.StartIngame();
    }
    #endregion

    public void SetClearWaveText()
    {
        _clearWaveText.text = $"웨이브 {Manager.Instance.Ingame.CurrentWaveIndex} 클리어";
    }

    public void InitWavePanel()
    {
        // 사용한 WaveUI를 반납 후 리스트 정리
        for (int ii = 0; ii < _wavePanelList.Count; ++ii)
            _wavePanelList[ii].ReturnWaveUI();
        _wavePanelList.Clear();

        // 사용할 WaveUI를 가져옴
        var waveIndices = Manager.Instance.Data.ChapterInfoDataList[Manager.Instance.Ingame.CurrentChapterIndex].WaveIndex;
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
        _UpdateWavePanel().Forget();
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

    private async UniTaskVoid _UpdateWavePanel()
    {
        var currentWaveIndex = Manager.Instance.Ingame.CurrentWaveIndex;
        for (int ii = 0; ii < currentWaveIndex - PREV_WAVE_INDEX; ++ii)
            _wavePanelList[ii].UpdateWaveUI();
        if (currentWaveIndex - PREV_WAVE_INDEX >= 0)
        {
            var complete = false;
            _wavePanelList[currentWaveIndex - PREV_WAVE_INDEX].UpdateWaveUIAnimation(() =>
            {
                complete = true;
            });
            while (false == complete)
                await UniTask.Yield();
        }
        _wavePanelList[currentWaveIndex].UpdateWaveUI();
    }
}
