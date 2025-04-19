using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// When an attack with this effects hits a GameCharacter, applies a <see cref="BuffEffect"/> on that GameCharacter. Use it to apply negative effects such as stat reduction, or effect damage such as poison or burns.
    /// </summary>
    [CreateAssetMenu(fileName = "ApplyBuffOnHit", menuName = "Auto-Battle Framework/Effects/OnHitEffect/ApplyBuff", order = 4)]
    public class ApplyBuffOnHitEffect : OnHitEffect
    {
        /// <summary>
        /// Buff the will be applied to the defender.
        /// </summary>
        public BuffEffect effect;

        /// <summary>
        /// Applies the buff when the attack hits.
        /// </summary>
        /// <param name="defender">GameCharacter recieving the attack. The <see cref="BuffEffect"/> will be applied to this GameCharacter.</param>
        /// <param name="attacker">GameCharacter that is attacking.</param>
        /// <param name="damage">Fixed amount of damage. In this case it is not necessary.</param>
        public override void OnHit(GameCharacter defender, GameCharacter attacker, float damage)
        {
            effect.Attack(attacker, null);
        }
    }
}
