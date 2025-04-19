using AutoBattleFramework.BattleBehaviour;
using AutoBattleFramework.Formulas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutoBattleFramework.BattleBehaviour.GameActors;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// The projectile spawns above the target and moves in straight line until it reaches the target.
    /// </summary>
    [CreateAssetMenu(fileName = "HealthMeteoriteAllEffect", menuName = "Auto-Battle Framework/Effects/RangedEffect/HealthMeteoriteAllEffect", order = 4)]
    public class HealthMeteoriteAllEffect : RangedEffect
    {
        /// <summary>
        /// The transform from which the projectile will be launched.
        /// </summary>
        Transform shootingPoint;

        /// <summary>
        /// Percentage of life that each meteorite heals.
        /// </summary>
        public float HealthHealed = 0.2f;

        public Color healColor = Color.cyan;

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
        public override void OnHit(GameCharacter target)
        {
            if (target.State != GameCharacter.AIState.Dead)
            {
                BattleFormulas.RecieveDamage(target, -RecoverHealthCalc(HealthHealed, target), DamageType, healColor);
            }
        }

        /// <summary>
        /// Instantiate all Projectiles and sets its properties.
        /// </summary>
        /// <returns>The Instantiated projectile</returns>
        protected override Projectile SpawnProjectile()
        {
            List<GameCharacter> team = Battle.Instance.teams[0].team;
            if (!team.Contains(ai))
            {
                team = Battle.Instance.teams[1].team;
            }
            foreach(GameCharacter mate in team)
            {
                Projectile proj = Instantiate(projectile) as Projectile;
                Vector3 pos = mate.transform.position;
                pos.y = shootingPoint.position.y;
                proj.transform.position = pos;
                proj.transform.LookAt(mate.EnemyShootingPoint);
                proj.SetTarget(ai, mate, speed, this);
            }
            return null;
        }

        int RecoverHealthCalc(float perc, GameCharacter target)
        {
            return (int)(HealthHealed * (float)target.InitialStats.Health);
        }
    }
}
