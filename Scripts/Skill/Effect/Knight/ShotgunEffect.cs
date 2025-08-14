using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunEffect : ISkillEffect
{
    public void Apply(ICombatEntity caster, ICombatEntity unused, SkillExecutionContext context)
    {
        var playerController = caster as PlayerController;
        if (playerController == null)
            return;

        // InputManager, Camera 참조 얻기 (필요시 PlayerController에 프로퍼티로 추가)
        InputManager inputManager = playerController.InputManager; // public 프로퍼티 필요
        Camera camera = Camera.main; // 또는 playerController.MainCamera 등

        var skill = context.Skill;

        if (skill.lightCostValue > 0f && PlayerSightController.Instance != null)
            PlayerSightController.Instance.AddBrightness(-skill.lightCostValue);

        float soundRadius = 25f;
        float soundPriority = 3f;
        playerController.SoundEmitter.EmitSound(soundRadius, soundPriority);

        Vector3 origin = playerController.ProjectilePoint.position;

        Vector3 mouseWorldPos = GetMouseWorldPosition(inputManager, camera);
        Vector3 dirToMouse = mouseWorldPos - origin; dirToMouse.y = 0;

        Vector3 baseDir = playerController.ProjectilePoint.forward;

        if (caster is PlayerController player)
        {
            Vector3 recoilDir = -baseDir;
            float recoilForce = 1.0f; // recoil force
            player.DoRecoil(recoilDir, recoilForce);
        }

        // 프리팹 발사
        if (ProjectilePrefabRegistry.PrefabMap.TryGetValue(skill.projectilePrefabName, out var prefab))
        {
            for (int i = 0; i < skill.projectileCount; i++)
            {
                float spread = Random.Range(-skill.spreadAngle / 2f, skill.spreadAngle / 2f);
                Quaternion spreadRot = Quaternion.AngleAxis(spread, Vector3.up);
                Vector3 dir = spreadRot * baseDir;

                Quaternion lookRot = Quaternion.LookRotation(dir, -camera.transform.forward);

                GameObject go = GameObject.Instantiate(prefab, origin, lookRot);
                go.transform.localEulerAngles = new Vector3(0, 0, 0);
                var proj = go.GetComponent<BulletProjectile>();
                proj?.Init(caster, dir, skill.projectileSpeed, skill.range, skill.damageParam);
            }
        }

        if (!string.IsNullOrEmpty(skill.soundPath))
        {
            if (skill.soundClip == null)
                skill.soundClip = Resources.Load<AudioClip>(skill.soundPath);
            if (skill.soundClip != null)
                SoundManager.Instance.PlaySFXAtPosition(skill.soundClip, playerController.ProjectilePoint.position, 1f, 16f);
        }

        if (!string.IsNullOrEmpty(skill.effectPath))
        {
            if (skill.effectPrefab == null)
                skill.effectPrefab = Resources.Load<GameObject>(skill.effectPath);

            Vector3 vfxOrigin = playerController.MuzzlePoint.position;

            Quaternion vfxRot = playerController.LeftWeaponController.WeaponVisual.rotation;

            if (skill.effectPrefab != null)
            {
                GameObject fx = GameObject.Instantiate(
                    skill.effectPrefab,
                    vfxOrigin,
                    vfxRot
                );
                Object.Destroy(fx, 1.0f);
            }
        }
    }

    private void FireShotgunProjectiles(ICombatEntity caster, SkillData skill, Vector3 origin, Vector3 forward)
    {
        if (!ProjectilePrefabRegistry.PrefabMap.TryGetValue(skill.projectilePrefabName, out var prefab))
        {
            Debug.LogError($"[ShotgunEffect] projectile prefab not found: {skill.projectilePrefabName}");
            return;
        }

        for (int i = 0; i < skill.projectileCount; i++)
        {
            float spread = Random.Range(-skill.spreadAngle / 2f, skill.spreadAngle / 2f);
            Quaternion rotation = Quaternion.AngleAxis(spread, Vector3.up);
            Vector3 dir = rotation * forward;

            GameObject go = GameObject.Instantiate(prefab, origin, Quaternion.LookRotation(dir));
            var proj = go.GetComponent<BulletProjectile>();
            proj?.Init(caster, dir, skill.projectileSpeed, skill.range, skill.damageParam);
        }
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

}
