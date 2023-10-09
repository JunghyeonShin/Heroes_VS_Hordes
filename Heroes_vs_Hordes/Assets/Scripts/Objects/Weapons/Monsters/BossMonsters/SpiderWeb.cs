using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWeb : MonoBehaviour
{
    public event Action<GameObject> ReturnHandler;

    [SerializeField] private GameObject _webSpit;
    [SerializeField] private GameObject _webContainer;
    [SerializeField] private GameObject[] _webs;

    private const float CHECK_DIRECTION = 0f;
    private const float ANGLE_360 = 360f;
    private const float ZERO_SECOND = 0f;
    private const float ONE_SECOND = 1f;
    private const float MOVE_SPEED = 2f;
    private const float DELAY_RETURN = 5f;

    public void InitTransform(Vector3 startPos, Vector3 targetPos)
    {
        _webSpit.transform.position = startPos;
        Utils.SetActive(_webSpit, true);

        _webContainer.transform.position = targetPos;
        foreach (var web in _webs)
            Utils.SetActive(web, false);
        
        _RotateWebSpit(targetPos);
        _MoveWebSpit(targetPos).Forget();
    }

    private void _RotateWebSpit(Vector3 targetPos)
    {
        var targetVec = targetPos - _webSpit.transform.position;
        var angle = Vector3.Angle(Vector3.up, targetVec);
        if (_IsRightSide(targetVec.x))
            angle = ANGLE_360 - angle;
        _webSpit.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    private bool _IsRightSide(float value)
    {
        return value >= CHECK_DIRECTION;
    }

    private async UniTaskVoid _MoveWebSpit(Vector3 targetPos)
    {
        var startPos = _webSpit.transform.position;
        var time = ZERO_SECOND;
        while (time < ONE_SECOND)
        {
            time += MOVE_SPEED * Time.fixedDeltaTime;
            if (time >= ONE_SECOND)
                time = ONE_SECOND;
            _webSpit.transform.position = BezierCurve.LinearCurve(startPos, targetPos, time);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.fixedDeltaTime));
        }
        Utils.SetActive(_webSpit, false);
        _ControlWeb().Forget();
    }

    private async UniTaskVoid _ControlWeb()
    {
        foreach (var web in _webs)
        {
            Utils.SetActive(web, true);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.fixedDeltaTime));
        }
        await UniTask.Delay(TimeSpan.FromSeconds(DELAY_RETURN));

        ReturnHandler?.Invoke(gameObject);
    }
}
