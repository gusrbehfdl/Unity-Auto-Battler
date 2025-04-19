using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// It represents an attack that does needs to create a <see cref="Projectile"/>
    /// </summary>
    public abstract class RangedEffect : IAttackEffect
    {
        /// <summary>
        /// Object that will move until it hits the target.
        /// </summary>
        public Projectile projectile;

        /// <summary>
        /// Projectile moving speed.
        /// </summary>
        public float speed = 3f;

        /// <summary>
        /// Instantiate the Projectile and sets its properties.
        /// </summary>
        /// <returns>The Instantiated projectile</returns>
        protected abstract Projectile SpawnProjectile();
    }
}