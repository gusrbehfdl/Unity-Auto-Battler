using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Stats;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// It represents an effect that modifies statistics or adds new effects, temporarily.
    /// </summary>
    public abstract class BuffEffect : IAttackEffect
    {
        /// <summary>
        /// Modification of the stats and/or aggregation of new effects.
        /// </summary>
        public StatsModificator modificator;

        /// <summary>
        /// If the buff is applied multiple times, the max number of the same effect that can be applied.
        /// </summary>
        public int maxStacks = 1;

        /// <summary>
        /// Duration of the buff, in seconds.
        /// </summary>
        public float duration;

        /// <summary>
        /// If the buff is applied when it already exists, it should reset the time.
        /// </summary>
        public bool RestartTimeWhenRepeated = true;

        /// <summary>
        /// Buff has ended.
        /// </summary>
        bool end = false;

        /// <summary>
        /// It checks that the buff is still active, updates the elapsed time and calls <see cref="OnBuffEnd"/> and <see cref="OnBuffUpdate"/> methods when the time is right.
        /// </summary>
        /// <param name="info">Buff information.</param>
        public void UpdateBuff(BuffEffectInfo info)
        {
            info.UpdateTime(Time.deltaTime);

            if (info.CanBeRemoved() && !end)
            {
                end = true;
                OnBuffEnd(info);
            }
            else
            {
                OnBuffUpdate(info);
            }
        }

        /// <summary>
        /// Applies the buff to the GameCharacter.
        /// </summary>
        /// <param name="ai">GameCharacter that will recieve the buff.</param>
        /// <param name="shootingPoint">It should be null.</param>
        public override void Attack(GameCharacter ai, Transform shootingPoint)
        {
            BuffEffectInfo info = ai.BuffList.Where(x => x.buff == this).FirstOrDefault();
            if (info == null)
            {
                info = new BuffEffectInfo(this, ai, 1, duration);

                for (int i = 0; i < modificator.attackEffects.Count; i++)
                {
                    modificator.attackEffects[i] = Instantiate(modificator.attackEffects[i]);
                }

                AddModificator(info,modificator);

                OnHit(ai);
            }
            else
            {
                if (RestartTimeWhenRepeated)
                    info.RestartTime();
                if (info.stacks < maxStacks)
                {
                    info.stacks++;
                    modificator.AddStats(ai, true);
                }
                OnRepeatedBuff(info);
            }
        }

        /// <summary>
        /// Add the buff to the GameCharacter and applies its effects.
        /// </summary>
        /// <param name="info">Buff information.</param>
        /// <param name="modificator">Temporal stats modificator.</param>
        protected void AddModificator(BuffEffectInfo info, StatsModificator modificator)
        {
            info.character.BuffList.Add(info);
            modificator.AddStats(info.character, true);
        }

        /// <summary>
        /// Remove the buff from the GameCharacter and removes its effects.
        /// </summary>
        /// <param name="info">Buff information.</param>
        /// <param name="modificator">Temporal stats modificator to be removed.</param>
        protected void RemoveModificator(BuffEffectInfo info, StatsModificator modificator)
        {
            info.character.BuffList.Remove(info);
            for (int i = 0; i < info.stacks; i++)
            {
                modificator.AddStats(info.character, false);
            }
        }

        /// <summary>
        /// When the buff hits, calls <see cref="OnBuffStart"/>.
        /// </summary>
        /// <param name="target">The target of the character or projectile.</param>
        public override void OnHit(GameCharacter target)
        {
            BuffEffectInfo info = target.BuffList.Where(x => x.buff == this).FirstOrDefault();
            OnBuffStart(info);
        }

        /// <summary>
        /// Method that is called once when the buff will be applied.
        /// </summary>
        /// <param name="info">Buff information.</param>
        protected abstract void OnBuffStart(BuffEffectInfo info);

        /// <summary>
        /// Remove the buff from the GameCharacter and removes its effects.
        /// </summary>
        /// <param name="info">Buff information.</param>
        public void RemoveBuff(BuffEffectInfo info)
        {
            RemoveModificator(info,modificator);
        }

        /// <summary>
        /// Method that is called once when the buff is applied and there is another instance of the buff previously applied.
        /// </summary>
        /// <param name="info">Buff information.</param>
        protected abstract void OnRepeatedBuff(BuffEffectInfo info);

        /// <summary>
        /// Method that is called once when the buff will end.
        /// </summary>
        /// <param name="info">Buff information.</param>
        protected abstract void OnBuffEnd(BuffEffectInfo info);

        /// <summary>
        /// Method that is called while the buff is applied.
        /// </summary>
        /// <param name="info">Buff information.</param>
        protected abstract void OnBuffUpdate(BuffEffectInfo info);

    }
}