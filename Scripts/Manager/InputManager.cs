using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager
{
    private PlayerInput playerInput;
    private InputData currentInput;

    private bool _prevRightHeld = false;

    public PlayerInput GetInputActions() => playerInput;

    public InputManager()
    {
        playerInput = new PlayerInput();
    }

    public void Enable() => playerInput.Enable();
    public void Disable() => playerInput.Disable();

    public void UpdateInputData()
    {
        var gameplay = playerInput.Gameplay;

        Vector2 mouseScreenPos = playerInput.Gameplay.MousePosition.ReadValue<Vector2>();

        currentInput.MoveDirection = playerInput.Gameplay.Move.ReadValue<Vector2>();
        currentInput.isRun = playerInput.Gameplay.Run.ReadValue<float>() > 0.1f;
        currentInput.isSneak = playerInput.Gameplay.Sneak.ReadValue<float>() > 0.1f;

        currentInput.isAttack = playerInput.Gameplay.Attack.ReadValue<float>() > 0.1f;
        currentInput.isAttackDown = gameplay.Attack.triggered;

        bool isRightHeld = gameplay.RightSkill.ReadValue<float>() > 0.1f;
        currentInput.isRightClickDown = !_prevRightHeld && isRightHeld;
        currentInput.isRightClickUp = _prevRightHeld && !isRightHeld;
        currentInput.isRightClickHeld = isRightHeld;
        _prevRightHeld = isRightHeld;

        currentInput.isQSkill = playerInput.Gameplay.QSkill.ReadValue<float>() > 0.1f;

        currentInput.isESkill = playerInput.Gameplay.ESkill.ReadValue<float>() > 0.1f;
        currentInput.isESkillDown = gameplay.ESkill.triggered;

        currentInput.isUltimateSkill = playerInput.Gameplay.SkillUltimate.ReadValue<float>() > 0.1f;

        currentInput.isSpaceDown = gameplay.Dash.triggered;

        currentInput.isUseItem1Down = gameplay.UseItem1.triggered;
        currentInput.isUseItem2Down = gameplay.UseItem2.triggered;

        currentInput.isUseItem1 = playerInput.Gameplay.UseItem1.ReadValue<float>() > 0.1f;
        currentInput.isUseItem2 = playerInput.Gameplay.UseItem2.ReadValue<float>() > 0.1f;

        currentInput.isLightDown = gameplay.Light.triggered;

        currentInput.isInteract = playerInput.Gameplay.Interact.ReadValue<float>() > 0.1f;
        currentInput.isInteractDown = playerInput.Gameplay.Interact.triggered;
        currentInput.isInventory = playerInput.Gameplay.Inventory.triggered;
        currentInput.isOption = playerInput.Gameplay.Option.triggered;

        currentInput.isRootAll = playerInput.Gameplay.RootAll.triggered;

        currentInput.isMiniMap = playerInput.Gameplay.Minimap.triggered;
    }

    public InputData GetInput()
    {
        return currentInput;
    }

    Vector3 GetMouseWorldPosition(InputManager inputManager, Camera camera)
    {
        Vector2 mouseScreenPos = inputManager.GetMouseScreenPosition(); // 아래 참고
        Ray ray = camera.ScreenPointToRay(mouseScreenPos);
        Plane plane = new Plane(Vector3.up, Vector3.zero); // Y=0 평면
        if (plane.Raycast(ray, out float enter))
            return ray.GetPoint(enter);
        return Vector3.zero;
    }

    public Vector2 GetMouseScreenPosition()
    {
        return playerInput.Gameplay.MousePosition.ReadValue<Vector2>();
    }

}
