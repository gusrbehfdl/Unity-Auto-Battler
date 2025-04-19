using AutoBattleFramework.BattleBehaviour;
using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Battlefield;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Movement
{
    /// <summary>
    /// The characters move through each of the squares towards the target square. If a more natural movement is desired, look at <see cref="ApproximateAstarMovement"/>
    /// </summary>
    public class ExactAstarMovement : IBattleMovement
    {
        public ExactAstarMovement(Battle battle) : base()
        {
        }
        
    }
}