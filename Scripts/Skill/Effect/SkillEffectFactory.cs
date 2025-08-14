using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillEffectFactory
{
    public static readonly Dictionary<string, ISkillEffect> EffectMap = new()
    {
        { "damage", new DamageEffect() },
        { "heal", new HealEffect() },
        { "stun", new StunEffect() },
        { "bleed", new BleedEffect() },
        { "shotgun", new ShotgunEffect() },
        { "bullet", new BulletEffect() },
        { "guard", new GuardEffect() },
        { "light", new LightEffect() },
        // { "knockback", new KnockbackEffect() }, ...
    };
}
