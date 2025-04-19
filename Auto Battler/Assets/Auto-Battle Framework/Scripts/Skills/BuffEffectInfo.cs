using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// It contains buff information that depends on each character, such as elapsed time or stacks.
    /// </summary>
    [System.Serializable]
    public class BuffEffectInfo
    {
        /// <summary>
        /// Buff that applies to the character.
        /// </summary>
        public BuffEffect buff;

        /// <summary>
        /// Number of current stacks.
        /// </summary>
        public int stacks;
        public GameCharacter character;

        /// <summary>
        /// Duration of the buff, in seconds.
        /// </summary>
        protected float duration;

        /// <summary>
        /// Time the buff has been applied.
        /// </summary>
        public float elapsedTime;

        /// <summary>
        /// Constructor of BuffEffectInfo.
        /// </summary>
        /// <param name="buff">Buff that affects the character.</param>
        /// <param name="character">Character affected by the buff.</param>
        /// <param name="stacks">Number of current stacks.</param>
        /// <param name="duration">Duration of the buff.</param>
        public BuffEffectInfo(BuffEffect buff, GameCharacter character, int stacks, float duration)
        {
            this.buff = buff;
            this.character = character;
            this.stacks = stacks;
            this.duration = duration;
            this.elapsedTime = 0;
        }

        /// <summary>
        /// Set the elapsed time to 0.
        /// </summary>
        public void RestartTime()
        {
            elapsedTime = 0f;
        }

        /// <summary>
        /// Adds the value to the elapsed time. Normally the value is equal to Time.deltatime.
        /// </summary>
        /// <param name="time">Delta time.</param>
        public void UpdateTime(float time)
        {
            elapsedTime += time;
        }

        /// <summary>
        /// Check if the elapsed time is greater than the <see cref="duration"/>. In that case, the buff should end.
        /// </summary>
        /// <returns>True if elapsed time is greater that <see cref="duration"/></returns>
        public bool CanBeRemoved()
        {
            return elapsedTime >= duration;
        }
    }
}