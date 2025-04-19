using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// It represents an attack that does not need to create a <see cref="Projectile"/>
    /// </summary>
    public abstract class MeleeEffect : IAttackEffect
    {
    }
}