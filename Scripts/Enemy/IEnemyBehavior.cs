using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyBehavior
{
    void Init(EnemyController controller);

    void Tick();
}
