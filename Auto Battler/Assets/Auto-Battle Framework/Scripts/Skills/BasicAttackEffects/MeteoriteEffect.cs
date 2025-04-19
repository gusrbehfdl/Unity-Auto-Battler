using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Formulas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// The projectile spawns above the target and moves in straight line until it reaches the target.
    /// </summary>
    [CreateAssetMenu(fileName = "MeteoriteEffect", menuName = "Auto-Battle Framework/Effects/RangedEffect/MeteoriteEffect", order = 4)]
    public class MeteoriteEffect : RangedEffect
    {
        /// <summary>
        /// The transform from which the projectile will be launched.
        /// </summary>
        Transform shootingPoint;

        /// <summary>
        /// Percentage of magic damage applied to the attack.
        /// </summary>
        public float MagicDamage = 2.5f;

        public float SpawnFromHeight = 9f;

        /// <summary>
        /// On attack method. Spawn a <see cref="Projectile"/>.
        /// </summary>
        /// <param name="ai">Attacking GameCharacter</param>
        /// <param name="shootingPoint">The transform from which the projectile will be launched.</param>
        public override void Attack(GameCharacter ai, Transform shootingPoint)
        {
            this.ai = ai;
            this.shootingPoint = shootingPoint;
            SpawnProjectile();
        }

        /// <summary>
        /// Calls <see cref="Formulas.BattleFormulas.BasicAttackDamage"/>  when the projectile hits the target.
        /// </summary>
        /// <param name="target">The target of the character of projectile.</param>
        public override void OnHit(GameCharacter target)
        {
            BattleFormulas.SpecialAttackDamage(DamageType, MagicDamage*ai.CurrentStats.MagicDamage, target, ai);
        }

        /// <summary>
        /// Instantiate the Projectile and sets its properties.
        /// </summary>
        /// <returns>The Instantiated projectile</returns>
        protected override Projectile SpawnProjectile()
        {
            Projectile proj = Instantiate(projectile) as Projectile;
            Vector3 pos = ai.TargetEnemy.transform.position;
            pos.y = SpawnFromHeight;
            proj.transform.position = pos;
            proj.transform.LookAt(ai.TargetEnemy.EnemyShootingPoint);
            proj.SetTarget(ai, ai.TargetEnemy, speed, this);
            return proj;
        }
    }
}
