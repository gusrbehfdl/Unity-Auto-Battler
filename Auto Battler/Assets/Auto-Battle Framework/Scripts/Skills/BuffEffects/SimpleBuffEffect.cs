using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// A simple stat or effect application. Modify the values in the Inspector to customize the buff.
    /// </summary>
    [CreateAssetMenu(fileName = "SimpleBuffEffect", menuName = "Auto-Battle Framework/Effects/BuffEffect/SimpleBuffEffect", order = 4)]
    public class SimpleBuffEffect : BuffEffect
    {
        protected override void OnBuffEnd(BuffEffectInfo info)
        {
            //
        }

        protected override void OnBuffStart(BuffEffectInfo info)
        {
            //
        }

        protected override void OnBuffUpdate(BuffEffectInfo info)
        {
            //
        }

        protected override void OnRepeatedBuff(BuffEffectInfo info)
        {
            //
        }
    }
}