using AutoBattleFramework.BattleBehaviour.Backup;
using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.BattleBehaviour.States
{
    /// <summary>
    /// Change the current stage to another one.
    /// </summary>
    [CreateAssetMenu(fileName = "ChangeStageState", menuName = "Auto-Battle Framework/BattleStates/ChangeStageState", order = 1)]
    public class ChangeStageState : BattleState
    {
        /// <summary>
        /// If changed, lock the update.
        /// </summary>
        bool changed = false;

        /// <summary>
        /// Stage to change to.
        /// </summary>
        public ScriptableBattleStage NextStage;

        public override bool AllowFieldDrag(GameActor actor)
        {
            return false;
        }

        public override void CharacterAIUpdate(GameCharacter character)
        {
            //
        }

        public override void OnStageStart()
        {
            changed = false;
        }

        public override void OnTimerFinish()
        {
            //
        }

        public override void Update()
        {
            if (!changed)
            {
                Battle.Instance.stage = NextStage;

                // Remove the LastStageBackup, or it will mess up if a stage is repeated.
                BattleBackup.Instance.RemoveLastStageBackup();

                // Initialize the first battle state of the stage.
                Battle.Instance.stage.InitializeBattleStage(-1);
                Battle.Instance.stage.NextState();

                changed = true;
            }
        }
    }
}