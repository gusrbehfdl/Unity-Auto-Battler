using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Shop;
using AutoBattleFramework.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace AutoBattleFramework.BattleBehaviour.States
{
    /// <summary>
    /// When reached this state, change to another scene.
    /// </summary>
    [CreateAssetMenu(fileName = "ChangeSceneState", menuName = "Auto-Battle Framework/BattleStates/ChangeSceneState", order = 1)]
    public class ChangeSceneState : BattleState
    {
        /// <summary>
        /// Name of the scene to be changed to.
        /// </summary>
        public string ChangeSceneTo;


        /// <summary>
        /// Show the win panel
        /// </summary>
        public bool ShowWinButton = true;

        private void Reset()
        {
            ShowInUI = false;
            UIPrefab = AutoBattleSettings.GetOrCreateSettings().defaultStageEmptyUIPrefab;
        }

        /// <summary>
        /// Characters and items can not be moved in this state.
        /// </summary>
        /// <returns>True, Characters and items can not be moved in this state.</returns>
        public override bool AllowFieldDrag(GameActor actor)
        {
            return false;
        }

        /// <summary>
        /// The characters will stand still in this state.
        /// </summary>
        /// <param name="character">Character to be updated.</param>
        public override void CharacterAIUpdate(GameCharacter character)
        {
            //
        }

        /// <summary>
        /// Reset the timer.
        /// </summary>
        public override void OnStageStart()
        {
            Battle battle = Battle.Instance;
            if (ShowWinButton)
                battle.winPanel.gameObject.SetActive(true);

            battle.timer.ResetTimer(time);
            battle.timer.StartTimer();
        }

        /// <summary>
        /// Restart the scene when time reaches zero.
        /// </summary>
        public override void OnTimerFinish()
        {
            SceneManager.LoadScene(ChangeSceneTo);
        }

        /// <summary>
        /// Do nothing
        /// </summary>
        public override void Update()
        {
            //
        }



    }
}

