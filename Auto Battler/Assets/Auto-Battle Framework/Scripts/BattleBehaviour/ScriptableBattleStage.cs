using AutoBattleFramework.BattleBehaviour.States;
using AutoBattleFramework.BattleUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.BattleBehaviour
{
    /// <summary>
    /// Represents the set of sub-states that make up a complete stage.
    /// </summary>
    [CreateAssetMenu(fileName = "Stage", menuName = "Auto-Battle Framework/BattleStates/Stage", order = 2)]
    public class ScriptableBattleStage : ScriptableObject
    {
        /// <summary>
        /// List of sub-states that make up a stage.
        /// </summary>
        public List<BattleState> stage;

        StageUI ui;

        int currentStage = -1;

        /// <summary>
        /// Initialize the variables of the stage.
        /// </summary>
        /// <param name="currentStage">Index of the current stage. Set to -1 if starting the stage, then call <see cref="NextState"/> to start the first state.</param>
        public void InitializeBattleStage(int currentStage)
        {
            this.currentStage = currentStage;
            foreach (BattleState state in stage)
            {
                state.stage = this;
            }
            ui = FindObjectOfType<StageUI>();
            ui.InitializeUI();
        }

        /// <summary>
        /// Returns the current BattleState
        /// </summary>
        /// <returns>Current BattleState</returns>
        public BattleState GetCurrentState()
        {
            return stage[currentStage];
        }

        /// <summary>
        /// Returns the current BattleState index.
        /// </summary>
        /// <returns>Current BattleState index.</returns>
        public int GetCurrentIndex()
        {
            return currentStage;
        }

        /// <summary>
        /// Ends the current BattleState and starts the next one.
        /// </summary>
        public void NextState()
        {
            currentStage++;
            if (currentStage == stage.Count)
            {
                currentStage = 0;
            }
            if (Battle.Instance.stage)
                GetCurrentState().OnStageStart();

            if (ui)
            {
                ui.NextState(currentStage);
            }
        }

        /// <summary>
        /// Go to the previous Battle state.
        /// </summary>
        public void PreviousState()
        {
            currentStage--;
            if (currentStage < 0)
            {
                currentStage = 0;
            }
            if (Battle.Instance.stage)
                GetCurrentState().OnStageStart();

        }

        /// <summary>
        /// Go to the start of the current stage.
        /// </summary>
        public void ResetStage()
        {
            currentStage = 0;
            if (Battle.Instance.stage)
            {
                GetCurrentState().OnStageStart();
                Battle.Instance.stage.ui.InitializeUI();
            }
            if (ui)
            {
                ui.NextState(currentStage);
            }
        }
    }
}