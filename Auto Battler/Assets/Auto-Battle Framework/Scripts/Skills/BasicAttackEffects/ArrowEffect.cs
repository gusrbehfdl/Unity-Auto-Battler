using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Formulas;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// Simple projectile attack. The projectile moves in straight line until it reaches the target.
    /// </summary>
    [CreateAssetMenu(fileName = "ArrowEffect", menuName = "Auto-Battle Framework/Effects/RangedEffect/ArrowEffect", order = 4)]
    public class ArrowEffect : RangedEffect
    {
        /// <summary>
        /// The transform from which the projectile will be launched.
        /// </summary>
        Transform shootingPoint;

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
        /// /// <param name="target">The target of the projectile.</param>
        public override void OnHit(GameCharacter target)
        {
            BattleFormulas.BasicAttackDamage(DamageType, ai.TargetEnemy, ai);
        }

        /// <summary>
        /// Instantiate the Projectile and sets its properties.
        /// </summary>
        /// <returns>The Instantiated projectile</returns>
        protected override Projectile SpawnProjectile()
        {
            Projectile proj = Instantiate(projectile) as Projectile;
            proj.transform.position = shootingPoint.transform.position;
            proj.transform.LookAt(ai.TargetEnemy.EnemyShootingPoint);
            proj.SetTarget(ai, ai.TargetEnemy, speed, this);
            
            return proj;
        }
    }
}