using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutoBattleFramework.Formulas;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// Damages the owner of the buff with a fixed amount of damage.
    /// </summary>
    [CreateAssetMenu(fileName = "SimpleFixedDamageOverTimeEffect", menuName = "Auto-Battle Framework/Effects/DebuffEffect/SimpleFixedDamageOverTimeEffect", order = 4)]
    public class FixedDamageOverTimeEffect : BuffEffect
{
        /// <summary>
        /// How often damage is applied, in seconds.
        /// </summary>
        public float Tick = 1f;

        /// <summary>
        /// Amount of damage to be applied.
        /// </summary>
        public float Damage;

        /// <summary>
        /// When the las <see cref="Tick"/> whas applied.
        /// </summary>
        float lastTick = 0f;

        /// <summary>
        /// Displayed color of the damage. Will override <see cref="BattleBehaviour.Battle.EffectColor"/>.
        /// </summary>
        public Color color;

        /// <summary>
        /// On buff end, does nothing.
        /// </summary>
        protected override void OnBuffEnd(BuffEffectInfo info)
        {
            //
        }

        /// <summary>
        /// Set <see cref="lastTick"/> to zero.
        /// </summary>
        protected override void OnBuffStart(BuffEffectInfo info)
        {
            lastTick = 0f;
        }

        /// <summary>
        /// If the elapsed time since the last tick is greater, it damages the owner of the buff.
        /// </summary>
        protected override void OnBuffUpdate(BuffEffectInfo info)
        {
            if (lastTick + Tick < info.elapsedTime)
            {
                //If character is still alive.
                if (ai)
                {
                    lastTick = lastTick + Tick;
                    BattleFormulas.RecieveDamage(ai, Damage * info.stacks, DamageType, color);
                }
            }
        }

        /// <summary>
        /// Does nothing when the buff is applied again.
        /// </summary>
        protected override void OnRepeatedBuff(BuffEffectInfo info)
        {
            //
        }

    }
}
