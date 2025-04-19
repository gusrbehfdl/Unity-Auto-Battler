using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// Behavior of projectiles such as arrows or spells. The default behavior is to go straight towards the target.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        /// <summary>
        /// GameCharacter that launches this projectile.
        /// </summary>
        public GameCharacter source;
        /// <summary>
        /// GameCharacter that is the target of this projectile.
        /// </summary>
        public GameCharacter target;

        /// <summary>
        /// Travelling speed of the projectile.
        /// </summary>
        public float speed;

        /// <summary>
        /// Effect contained in this projectile.
        /// </summary>
        IAttackEffect effect;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (target != null)
            {
                if (target.State != GameCharacter.AIState.Dead)
                {
                    Movement();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }

        }

        /// <summary>
        /// Default movement of the projectile. Travels in a straight line to the target. When the target is hit, it activates the default effects and the projectile is destroyed.
        /// </summary>
        public virtual void Movement()
        {
            var dist = Vector3.Distance(transform.position, target.EnemyShootingPoint.position);
            float i = speed * Time.deltaTime / dist;
            transform.position = Vector3.Lerp(transform.position, target.EnemyShootingPoint.position, i);
            if (i >= 1)
            {
                OnHit();
            }
        }

        /// <summary>
        /// Set the projectile properties.
        /// </summary>
        /// <param name="source">GameCharacter who launches this projectile.</param>
        /// <param name="target">GameCharacter target of this projectile.</param>
        /// <param name="speed">Travelling speed of the projectile.</param>
        /// <param name="effect">Attack effects attached to this projectile.</param>
        public virtual void SetTarget(GameCharacter source, GameCharacter target, float speed, IAttackEffect effect)
        {
            this.source = source;
            this.target = target;
            this.speed = speed;
            this.effect = effect;
        }

        /// <summary>
        /// Applies on hit effects and damage, and then destroy the projectile.
        /// </summary>
        public void OnHit()
        {
            if (!source.DamageVisualOnly)
            {
                effect.OnHit(target);
            }
            Destroy(gameObject);
            
        }
    }
}