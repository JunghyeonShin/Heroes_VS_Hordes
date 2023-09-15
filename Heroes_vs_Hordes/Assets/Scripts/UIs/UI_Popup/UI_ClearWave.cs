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

    private readonly WavePanelTransform[] FOUR_WAVE_PANEL_TRANSFORMS = new WavePanelTransform[]
    {
        new WavePanelTransform() { PanelPosition = new Vector2(-150f, 0f), PanelSize = new Vector2(150f, 50f), IconSize = new Vector2(60f, 65f) },
        new WavePanelTransform() { PanelPosition = Vector2.zero, PanelSize = new Vector2(150f, 50f), IconSize = new Vector2(60f, 65f) },
        new WavePanelTransform() { PanelPosition = new Vector2(150f, 0f), PanelSize = new Vector2(150f, 50f), IconSize = new Vector2(60f, 65f) },
        new WavePanelTransform() { PanelPosition = new Vector2(225f, 25f), PanelSize = new Vector2(100f, 100f), IconSize = new Vector2(60f, 65f) }
    };

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
        Manager.Instance.Ingame.StartIngame();
        _ClosePopupUI();
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
        for (int ii = 0; ii < Manager.Instance.Ingame.TotalWaveIndex - 1; ++ii)
            _InitWavePanel<UI_NormalBattleWave>(Define.RESOURCE_UI_NORMAL_BATTLE_WAVE, ii);
        _InitWavePanel<UI_CoinRushWave>(Define.RESOURCE_UI_COIN_RUSH_WAVE, Manager.Instance.Ingame.TotalWaveIndex - 1);
    }

    public void UpdateWavePanel()
    {
        _UpdateWavePanel().Forget();
    }

    private void _InitWavePanel<T>(string wavePanelName, int index) where T : UI_Wave
    {
        var wave = Manager.Instance.UI.GetElementUI(wavePanelName);
        var waveUI = Utils.GetOrAddComponent<T>(wave);
        waveUI.InitWaveUI(index, _totalWaves.transform, FOUR_WAVE_PANEL_TRANSFORMS[index].PanelPosition, FOUR_WAVE_PANEL_TRANSFORMS[index].PanelSize, FOUR_WAVE_PANEL_TRANSFORMS[index].IconSize);
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
