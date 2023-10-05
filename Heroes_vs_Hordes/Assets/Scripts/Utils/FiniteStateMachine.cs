using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EStateTypes
{
    None,
    Idle,
    Move,
    Attack,
    Dead,
}

public interface IFiniteState
{
    public void InitState();
    public void EnterState();
    public void ExitState();
    public void FixedUpdateState();
    public void UpdateState();
}

public class FiniteStateMachine : MonoBehaviour
{
    private Dictionary<EStateTypes, IFiniteState> _stateDic = new Dictionary<EStateTypes, IFiniteState>();
    private IFiniteState _currentState;

    private void FixedUpdate()
    {
        _currentState?.FixedUpdateState();
    }

    private void Update()
    {
        _currentState?.UpdateState();
    }

    public void AddState(EStateTypes type, IFiniteState state)
    {
        if (_stateDic.ContainsKey(type))
            return;

        _stateDic.Add(type, state);
        _stateDic[type].InitState();
    }

    public void ChangeState(EStateTypes type)
    {
        _currentState?.ExitState();

        if (_stateDic.TryGetValue(type, out var state))
        {
            _currentState = state;
            _currentState.EnterState();
        }
        else
            _currentState = null;
    }
}
