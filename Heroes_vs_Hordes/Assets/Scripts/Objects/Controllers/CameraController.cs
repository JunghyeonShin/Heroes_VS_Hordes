using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private MainScene _mainScene;

    private CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        SetFollower();
    }

    public void SetFollower(Transform target = null)
    {
        if (null != target)
            _virtualCamera.Follow = target;
        else
            _virtualCamera.Follow = _mainScene.transform;
    }
}
