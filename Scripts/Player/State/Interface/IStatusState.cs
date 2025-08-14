using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusState
{
    void Enter();
    void Update(InputData input);
    void Exit();
}
