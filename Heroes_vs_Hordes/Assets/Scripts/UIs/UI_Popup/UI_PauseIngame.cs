using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PauseIngame : UI_Popup
{
    private struct WavePanelTransform
    {
        public Vector2 PanelPosition { get; set; }
        public Vector2 PanelSize { get; set; }
        public Vector2 IconSize { get; set; }
    }

    private enum EButtons
    {
        RestartButton,
        GiveUpIngameButton
    }

    private enum EGameObjects
    {
        TotalWaves
    }

    private GameObject _totalWaves;
    private List<UI_Element> _wavePanelList = new List<UI_Element>();

    private readonly WavePanelTransform[] _fourWavePanelTranforms = new WavePanelTransform[]
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
        Manager.Instance.Ingame.GiveUpIngame();
        Manager.Instance.UI.ShowSceneUI<UI_MainScene>(Define.RESOURCE_UI_MAIN_SCENE);
    }
    #endregion

    public void InitWavePanel()
    {
        _wavePanelList.Clear();

        for (int ii = 0; ii < Define.MAX_WAVE_INDEX - 1; ++ii)
            _InitWavePanel<UI_NormalBattleWave>(Define.RESOURCE_UI_NORMAL_BATTLE_WAVE, ii);

        _InitWavePanel<UI_CoinRushWave>(Define.RESOURCE_UI_COIN_RUSH_WAVE, Define.MAX_WAVE_INDEX - 1);
    }

    public void UpdateWavePanel()
    {
        for (int ii = 0; ii < _wavePanelList.Count; ++ii)
            _wavePanelList[ii].UpdateUIElement();
    }

    private void _InitWavePanel<T>(string wavePanelName, int index) where T : UI_Element
    {
        var wave = Manager.Instance.UI.GetElementUI(wavePanelName);
        var waveUI = Utils.GetOrAddComponent<T>(wave);
        waveUI.InitUIElement(index, _totalWaves.transform, _fourWavePanelTranforms[index].PanelPosition, _fourWavePanelTranforms[index].PanelSize, _fourWavePanelTranforms[index].IconSize);
        Utils.SetActive(wave, true);
        _wavePanelList.Add(waveUI);
    }
}
