using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState : IFiniteState
{
    protected GameObject _owner;

    public BaseState(GameObject owner)
    {
        _owner = owner;
    }

    public abstract void InitState();

    public abstract void EnterState();

    public abstract void ExitState();

    public abstract void FixedUpdateState();

    public abstract void UpdateState();
}
