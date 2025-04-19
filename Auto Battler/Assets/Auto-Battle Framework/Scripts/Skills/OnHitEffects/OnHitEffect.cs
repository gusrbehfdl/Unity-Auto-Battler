using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// Effect that is only applied when a GameCharacter attacks another.
    /// </summary>
    public abstract class OnHitEffect : ScriptableObject
    {
        /// <summary>
        /// When the defender receives an attack, the effects of this method are applied.
        /// </summary>
        /// <param name="defender">GameCharacter recieving the attack.</param>
        /// <param name="attacker">GameCharacter that is attacking.</param>
        /// <param name="damage">Fixed amount of damage.</param>
        public abstract void OnHit(GameCharacter defender, GameCharacter attacker, float damage);

        /// <summary>
        /// Instantiate the OnHitEffect
        /// </summary>
        public virtual OnHitEffect InstantiateOnHitEffect()
        {
            return Instantiate(this);
        }
    }
}