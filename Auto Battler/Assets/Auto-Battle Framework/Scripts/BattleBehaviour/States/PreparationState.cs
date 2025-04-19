using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Shop;
using AutoBattleFramework.Stats;
using AutoBattleFramework.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AutoBattleFramework.BattleBehaviour.States
{
    /// <summary>
    /// Allows to instantiate enemies, as well as move and equip ally characters.
    /// </summary>
    [CreateAssetMenu(fileName = "PreparationStage", menuName = "Auto-Battle Framework/BattleStates/PreparationState", order = 2)]
    public class PreparationState : BattleState
    {
        // Team references
        List<GameCharacter> Team1;
        List<GameCharacter> Team2;

        bool appliedBackup = false;

        /// <summary>
        /// Positions where characters belonging to the enemy team, the second position of <see cref="BattleBehaviour.Battle.teams"/>, will be spawned.
        /// </summary>
        public List<ScriptableBattlePosition> battlePositions;

        // Traits has been checked when moving characters between grid and bench.
        bool TraitChecked = false;

        /// <summary>
        /// Gold that the player wins  at the start of this state.
        /// </summary>
        public int goldPerRound = 4;

        /// <summary>
        /// Percentage of interest that the player will earn by accumulating money. For example, if he has 50 units of currency and the interest percentage is 0.1, he will earn 5 additional units.
        /// </summary>
        public float interestRate = 0.1f;


        /// <summary>
        /// Experience that the player wins at the start of this state.
        /// </summary>
        public int expPerRound = 2;

        private void Reset()
        {
            ShowInUI = false;
            UIPrefab = AutoBattleSettings.GetOrCreateSettings().defaultStageEmptyUIPrefab;
        }

        /// <summary>
        /// All characters and items can be moved in this state.
        /// </summary>
        /// <param name="actor">Actor to be dragged.</param>
        /// <returns>True, all characters and items should be allowed to be moved.</returns>
        public override bool AllowFieldDrag(GameActor actor)
        {
            return true;
        }

        /// <summary>
        /// The characters will stand still in this state.
        /// </summary>
        /// <param name="character">Character to be updated.</param>
        public override void CharacterAIUpdate(GameCharacter character)
        {
            character.agent.enabled = true;
        }

        /// <summary>
        /// The player recieves the gold from <see cref="interestRate"/>, then recieves gold from <see cref="goldPerRound"/> and shop experience from <see cref="expPerRound"/>. <br />
        /// Spawns the enemy team members in the positions set by <see cref="battlePositions"/>.
        /// </summary>
        public override void OnStageStart()
        {
            Battle battle = Battle.Instance;
            int interest = (int)(interestRate * battle.shopManager.currency);
            battle.shopManager.currency += interest;
            battle.shopManager.currency += goldPerRound;
            
            battle.shopManager.shopLevelManager.AddExp(expPerRound);
            

            TraitChecked = false;

            appliedBackup = ApplyBackupPlayerTeam();            

            Team1 = battle.teams[0].team;
            Team2 = battle.teams[1].team;


            battle.timer.ResetTimer(time);
            battle.timer.StartTimer();                       

            InitializeTeam(Team1);            

            foreach(GameCharacter character in battle.TeamBenches[0].GetGameCharacterInBench())
            {
                battle.TryFusion(character.info as ShopCharacter);
            }

            // Make a backup of the player´s team.
            BackupStatePlayerTeam();

            // Make a buckup of the player´s team if it´s the first state in the stage.
            if(Battle.Instance.stage.GetCurrentIndex() == 0)
            {
                BackupStagePlayerTeam();
            }

            SpawnCharacters();
            InitializeTeam(Team2);
        }

        /// <summary>
        /// Resets characters stats that should be reset each round.
        /// </summary>
        /// <param name="team">Team to be reset.</param>
        void InitializeTeam(List<GameCharacter> team)
        {
            foreach (GameCharacter ai in team)
            {
                ai.ChangeState(GameCharacter.AIState.NoTarget);
                ai.GetCurrentCell();
                ai.currentPath.Clear();
                ai.CurrentStats = ai.InitialStats.Copy();
                ai.CurrentStats.Energy = 0;
                FaceRandomTarget(ai);
            }
        }

        /// <summary>
        /// When the timer reaches zero, go to the next state.
        /// </summary>
        public override void OnTimerFinish()
        {
            stage.NextState();
        }

        /// <summary>
        /// Check for trait changes when trades between grid and bench.
        /// </summary>
        public override void Update()
        {
            if (!TraitChecked)
            {                
                Battle.Instance.TraitCheck(Battle.Instance.teams[0].team, Battle.Instance.TeamBenches[0].GetGameCharacterInBench(),Battle.Instance.TraitsToCheck, Battle.Instance.TeamTraitListUI[0], !appliedBackup);
                Battle.Instance.shopManager.GetRandomItems();
                SetTraitsOnTeam2();
                TraitChecked = true;
                appliedBackup = false;
            }
        }

        void SetTraitsOnTeam2()
        {
            // Since it is possible that the trait option will not change, without applying the effects.
            Battle.Instance.TraitCheck(Battle.Instance.teams[1].team, null, Battle.Instance.TraitsToCheckTeam2, Battle.Instance.TeamTraitListUI[1], false);

            if (Battle.Instance.ApplyTeam2TraitModificators)
            {
                // Then apply the effects of the traits on the new characters.
                foreach (GameCharacter character in Battle.Instance.teams[1].team)
                {
                    character.ApplyTraitsToNewCharacter(Battle.Instance.TraitsToCheckTeam2);
                }
            }
        }

        /// <summary>
        /// Instantiate all <see cref="Team2"/> characters from <see cref="battlePositions"/>.
        /// </summary>
        void SpawnCharacters()
        {
            ScriptableBattlePosition battlePosition = battlePositions[Random.Range(0, battlePositions.Count)];
            for (int i = 0; i < battlePosition.battlePositions.Length; i++)
            {
                if (battlePosition.battlePositions[i] > 0)
                {
                    if (battlePosition.GetCharacterGroup(i).characterGroup.Count > 0)
                    {
                        SpawnCharacter(i, battlePosition.GetCharacterGroup(i));
                    }
                }
            }
        }

        /// <summary>
        /// Given an index that represents a cell from <see cref="AutoBattleFramework.Battlefield.BattleGrid.GridCells"/> and a group of characters, spawns a random character of that group in that cell.
        /// </summary>
        /// <param name="i">Index of cell in <see cref="AutoBattleFramework.Battlefield.BattleGrid.GridCells"/></param>
        /// <param name="group">Group of characters set int <see cref="ScriptableBattlePosition"/>.</param>
        void SpawnCharacter(int i, CharacterGroup group)
        {
            ShopCharacter randomCharacter = group.GetRandomCharacter();
            GameCharacter character = Instantiate(randomCharacter.shopItem) as GameCharacter;
            character.gameObject.name = character.gameObject.name + i;
            character.GetComponent<NavMeshAgent>().Warp(Battle.Instance.grid.GridCells[i].transform.position);
            character.transform.rotation = Quaternion.Euler(0, 180, 0);
            Battle.Instance.teams[1].team.Add(character);
            character.CanBeMoved = false;
        }

        /// <summary>
        /// Makes a character to face a random character in opposing team.
        /// </summary>
        /// <param name="character">Character who will face another character from the other team.</param>
        void FaceRandomTarget(GameCharacter character)
        {
            List<GameCharacter> team = Team1;
            if (team.Contains(character))
            {
                team = Team2;
            }
            if (team.Count > 0)
            {
                GameCharacter TargetEnemy = team[Random.Range(0, team.Count)];
                Vector3 direction = (TargetEnemy.transform.position - character.transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                character.transform.rotation = lookRotation;
            }
        }
    }
}