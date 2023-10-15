using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ClearChapter : UI_Popup
{
    private enum EButtons
    {
        ExitIngameButton,
        RewardChapterButton
    }

    private enum EGameObjects
    {
        ClearChapterIndex,
        ClosedBox,
        OpenedBox,
        RewardGoldIcon
    }

    private enum ETexts
    {
        ClearChapterIndexText,
        RewardGoldText
    }

    private GameObject _exitIngameButtonGO;
    private GameObject _closeBoxGO;
    private GameObject _openBoxGO;
    private GameObject _rewardGoldIconGO;

    private TextMeshProUGUI _rewardGoldText;

    private RectTransform _closeBoxRectTransform;
    private RectTransform _rewardGoldIconRectTransform;

    private bool _rewardGold;

    private const float INIT_CLOSE_BOX_HEIGHT = 300;
    private const float CHANGE_CLOSE_BOX_HEIGHT = 250;
    private const float ZERO_SECOND = 0f;
    private const float ONE_SECOND = 1f;
    private const float READY_TO_OPEN_BOX_SPEED = 2f;
    private const float FLOAT_REWARD_GOLD_SPEED = 3f;
    private const float DELAY_ACTIVE_EXIT_BUTTON = 0.5f;
    private const int MIN_REWARD_GOLD = 1000;
    private const int MAX_REWARD_GOLD = 1750;

    private readonly Vector3 INIT_REWARD_GOLD_POSITION = new Vector3(0f, -365f, 0f);
    private readonly Vector3 CHANGE_REWARD_GOLD_POSITION = new Vector3(0f, 140f, 0f);

    protected override void _Init()
    {
        _BindButton(typeof(EButtons));
        _BindGameObject(typeof(EGameObjects));
        _BindText(typeof(ETexts));

        _exitIngameButtonGO = _GetButton((int)EButtons.ExitIngameButton).gameObject;
        _BindEvent(_exitIngameButtonGO, _ExitIngame);
        _BindEvent(_GetButton((int)EButtons.RewardChapterButton).gameObject, _GetReward);

        _closeBoxGO = _GetGameObject((int)EGameObjects.ClosedBox);
        _openBoxGO = _GetGameObject((int)EGameObjects.OpenedBox);
        _rewardGoldIconGO = _GetGameObject((int)EGameObjects.RewardGoldIcon);

        _rewardGoldText = _GetText((int)ETexts.RewardGoldText);

        _closeBoxRectTransform = Utils.GetOrAddComponent<RectTransform>(_closeBoxGO);
        _rewardGoldIconRectTransform = Utils.GetOrAddComponent<RectTransform>(_rewardGoldIconGO);
    }

    #region Event
    private void _ExitIngame()
    {
        var manager = Manager.Instance;
        if (manager.Ingame.CurrentChapterIndex > manager.SaveData.ClearChapter)
            manager.SaveData.ClearChapter = manager.Ingame.CurrentChapterIndex;
        manager.Ingame.ExitIngame();
    }

    private void _GetReward()
    {
        if (_rewardGold)
            return;

        _RewardGold().Forget();
    }
    #endregion

    public void SetClearChapterUI()
    {
        _rewardGold = false;

        Utils.SetActive(_exitIngameButtonGO, false);
        Utils.SetActive(_closeBoxGO, true);
        Utils.SetActive(_openBoxGO, false);
        Utils.SetActive(_rewardGoldIconGO, false);

        _closeBoxRectTransform.sizeDelta = new Vector2(_closeBoxRectTransform.sizeDelta.x, INIT_CLOSE_BOX_HEIGHT);
        _rewardGoldIconRectTransform.anchoredPosition = INIT_REWARD_GOLD_POSITION;
    }

    private async UniTaskVoid _RewardGold()
    {
        _rewardGold = true;

        var time = ZERO_SECOND;
        while (time < ONE_SECOND)
        {
            time += READY_TO_OPEN_BOX_SPEED * Time.deltaTime;
            if (time >= ONE_SECOND)
                time = ONE_SECOND;
            var height = Mathf.Lerp(INIT_CLOSE_BOX_HEIGHT, CHANGE_CLOSE_BOX_HEIGHT, time);
            _closeBoxRectTransform.sizeDelta = new Vector2(_closeBoxRectTransform.sizeDelta.x, height);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        }
        Utils.SetActive(_closeBoxGO, false);
        Utils.SetActive(_openBoxGO, true);

        Manager.Instance.Ingame.ClearChapterReward = UnityEngine.Random.Range(MIN_REWARD_GOLD, MAX_REWARD_GOLD);
        _FloatGold().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_ACTIVE_EXIT_BUTTON));

        Utils.SetActive(_exitIngameButtonGO, true);
    }

    private async UniTaskVoid _FloatGold()
    {
        _rewardGoldText.text = Manager.Instance.Ingame.ClearChapterReward.ToString();
        Utils.SetActive(_rewardGoldIconGO, true);

        var time = ZERO_SECOND;
        while (time < ONE_SECOND)
        {
            time += FLOAT_REWARD_GOLD_SPEED * Time.deltaTime;
            if (time >= ONE_SECOND)
                time = ONE_SECOND;
            _rewardGoldIconRectTransform.anchoredPosition = Vector3.Lerp(INIT_REWARD_GOLD_POSITION, CHANGE_REWARD_GOLD_POSITION, time);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        }
    }
}
