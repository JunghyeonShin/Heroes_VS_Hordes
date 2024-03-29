using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;

    private Action _completeLoadingHandler;

    private const float DELAY_LOADING_TIME = 3f;

    private void Awake()
    {
        Manager.CreateInstance();
        Manager.Instance.CameraController = _cameraController;
    }

    private void Start()
    {
        Manager.Instance.UI.ShowSceneUI<UI_MainScene>(Define.RESOURCE_UI_MAIN_SCENE);
        Manager.Instance.UI.ShowPopupUI<UI_Loading>(Define.RESOURCE_UI_LOADING, (loadingUI) =>
        {
            _completeLoadingHandler -= loadingUI.CompleteLoading;
            _completeLoadingHandler += loadingUI.CompleteLoading;
            loadingUI.StartLoading();

            _CheckLoadComplete().Forget();
        });
    }

    private async UniTaskVoid _CheckLoadComplete()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_LOADING_TIME));
        while (false == Manager.Instance.LoadComplete())
            await UniTask.Yield();

        var mainSceneUI = Manager.Instance.UI.CurrentSceneUI as UI_MainScene;
        mainSceneUI.SetChapter(Manager.Instance.SaveData.ClearChapter + Define.ADJUSE_CHAPTER_INDEX);

        _completeLoadingHandler?.Invoke();
    }
}
