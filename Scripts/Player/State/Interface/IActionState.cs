using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionState
{
    void Enter(InputData input);
    void Update(InputData input);
    void Exit();
}
