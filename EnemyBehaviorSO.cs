using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehaviorSO : ScriptableObject, IEnemyBehavior
{
    protected EnemyController enemy;

    public void Init(EnemyController controller)
    {
        enemy = controller;
        OnInit();
    }

    public void Tick()
    {
        if (CanExecute())
            Execute();
    }

    protected virtual void OnInit() { }

    protected abstract bool CanExecute();

    protected abstract void Execute();
}
