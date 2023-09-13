using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;

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
            loadingUI.StartLoading();
        });
    }
}
