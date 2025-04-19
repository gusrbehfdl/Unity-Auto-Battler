using AutoBattleFramework.BattleBehaviour.Backup;
using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.BattleBehaviour.States
{
    /// <summary>
    /// It represents a state of the battle. It determines how the characters behave within the state and sets the conditions for moving to the next state. <br />
    /// For example, <see cref="PreparationState"/> allows to instantiate enemies, as well as move and equip characters, while state <see cref="FightState"/> allows both teams to battle each other.
    /// </summary>
    public abstract class BattleState : ScriptableObject
    {
        /// <summary>
        /// The <see cref="ScriptableBattleStage"/> to which the state belongs.
        /// </summary>
        [HideInInspector]
        public ScriptableBattleStage stage;

        /// <summary>
        /// Duration of the stage.
        /// </summary>
        public float time = 10f;

        /// <summary>
        /// If true, show the state in <see cref="AutoBattleFramework.BattleUI.StageUI"/>
        /// </summary>
        public bool ShowInUI = true;

        /// <summary>
        /// Prefab that represents the image of the stage.
        /// </summary>
        public GameObject UIPrefab;

        /// <summary>
        /// Method that is invoked once the state starts.
        /// </summary>
        public abstract void OnStageStart();

        /// <summary>
        /// Method that is once every frame.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Sets the condition in which a <see cref="GameActors.GameActor"/> can be moved.
        /// </summary>
        /// <param name="actor">Actor to be dragged.</param>
        /// <returns>If a <see cref="GameActors.GameActor"/> can be moved.</returns>
        public abstract bool AllowFieldDrag(GameActor actor);

        /// <summary>
        /// Every GameCharacter delegates the <see cref="GameActors.GameCharacter.Update"/> method to this state.
        /// </summary>
        /// <param name="character">Character to be updated.</param>
        public abstract void CharacterAIUpdate(GameCharacter character);

        /// <summary>
        /// Method that is called once when the <see cref="AutoBattleFramework.Shop.ShopGUI.Timer"/> marks zero.
        /// </summary>
        public abstract void OnTimerFinish();

        /// <summary>
        /// Backup copy of the status of characters and items. Can be used to return to the previous state of a round, before making changes.
        /// </summary>
        protected void BackupStatePlayerTeam(int PlayerIndex = 0)
        {
            if (!BattleBackup.Instance)
            {
                new GameObject().AddComponent<BattleBackup>();
            }
            BattleBackup.Instance.BackupLastState(this, Battle.Instance.teams[PlayerIndex].team, Battle.Instance.ItemBenches[PlayerIndex], Battle.Instance.TeamBenches[PlayerIndex]);
        }

        /// <summary>
        /// Backup copy of the status of characters and items at the start of a stage. Can be used to return to the initial state of the stage, before making changes.
        /// </summary>
        protected void BackupStagePlayerTeam(int PlayerIndex = 0)
        {
            if (!BattleBackup.Instance)
            {
                new GameObject().AddComponent<BattleBackup>();
            }
            BattleBackup.Instance.BackupLastStage(this, Battle.Instance.teams[PlayerIndex].team, Battle.Instance.ItemBenches[PlayerIndex], Battle.Instance.TeamBenches[PlayerIndex]);
        }

        /// <summary>
        /// Apply the Backup copy of the status of characters and items. Can be used to return to the previous state of a round, before making changes.
        /// </summary>
        protected bool ApplyBackupPlayerTeam(int PlayerIndex = 0)
        {
            if (!BattleBackup.Instance)
            {
                new GameObject().AddComponent<BattleBackup>();
            }
            if (BattleBackup.Instance.LastStateBackup?.BattleStateBackup == this)
            {
                return BattleBackup.Instance.ApplyLastStateBackup(this, PlayerIndex);
            }
            else if(BattleBackup.Instance.LastStageBackup?.BattleStateBackup == this)
            {
                return BattleBackup.Instance.ApplyLastStageBackup(this, PlayerIndex);
            }
            return false;
        }
    }
}