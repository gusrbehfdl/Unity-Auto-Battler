using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Battlefield;
using AutoBattleFramework.Movement;
using AutoBattleFramework.Skills;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace AutoBattleFramework.BattleBehaviour.States
{
    /// <summary>
    /// It allows teams to play against each other, and obtain a winner based on a victory condition.
    /// </summary>
    [CreateAssetMenu(fileName = "FightState", menuName = "Auto-Battle Framework/BattleStates/FightState", order = 1)]
    public class FightState : BattleState
    {
        //Team references
        List<GameCharacter> Team1;
        List<GameCharacter> Team2;

        //If stage is finished.
        protected bool stageFinished = false;

        /// <summary>
        /// The default movement of the characters. <see cref="ApproximateAstarMovement"/> by default.
        /// </summary>
        IBattleMovement movement;

        /// <summary>
        /// Hide the grid cells during the fight.
        /// </summary>
        public bool HideCells = false;

        /// <summary>
        /// If the Team  wins, the player obtains one unit of currency.
        /// </summary>
        public int currencyPerWin = 1;

        /// <summary>
        /// The type of victory condition: <br />
        /// More Character Alive: Team 1 wins if the timer reaches zero and has more members alive than Team 2. <br />
        /// More Total HP: Team 1 wins if the timer reaches zero and the sum of the current <see cref="Stats.CharacterStats.Health"/> of all his alive members is greater than Team 2. <br />
        /// More Percentual HP: Team 1 wins If the timer reaches zero and has more percentage <see cref="Stats.CharacterStats.Health"/> than team 2. <br />
        /// Survive: Team 1 wins if the timer reaches zero and there is at least one alive character. <br />
        /// KillAll: Team 1 wins if the timer reaches zero and all characters in Team2 are dead.
        /// </summary> 
        public enum WinCondition
        {
            MoreCharactersAlive,
            MoreTotalHP,
            MorePercentualHP,
            Survive,
            KillAll
        }

        /// <summary>
        /// The type of lose condition: <br />
        /// Defeated All: Team 1 loses if all members are dead.
        /// </summary>
        public enum LoseCondition
        {
            DefeatedAll,
        }

        /// <summary>
        /// The win condition of the fight.
        /// </summary>
        public WinCondition winCondition;

        /// <summary>
        /// The lose condition of the fight.
        /// </summary>
        public LoseCondition loseCondition;

        /// <summary>
        /// Check for win and lose conditions. If one condition is fullfiled, change state.
        /// </summary>
        public override void Update()
        {
            if (!stageFinished)
            {
                bool WinCondition = CheckWinCondition();

                bool LoseCondition = CheckLoseCondition();

                //If win or lose, 
                if (WinCondition || LoseCondition)
                {
                    Battle.Instance.timer.ResetTimer(0);
                    stageFinished = true;
                }
            }
        }

        /// <summary>
        /// Do not allow any character drag. Item drag is allowed, so items can be equipped while fighting.
        /// </summary>
        /// <param name="actor">Actor to be dragged.</param>
        /// <returns>True if the actor is an Item.</returns>
        public override bool AllowFieldDrag(GameActor actor)
        {
            if(actor is GameItem)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Moves the character and allows the attack on an enemy.
        /// </summary>
        /// <param name="character"></param>
        public override void CharacterAIUpdate(GameCharacter character)
        {
            if (movement == null)
            {
                movement = new ApproximateAstarMovement();
            }

            if (character.State == GameCharacter.AIState.Dead)
            {
                character.agent.enabled = false;
                return;
            }
            else
            {
                character.agent.enabled = true;
            }

            if (character.State != GameCharacter.AIState.Benched && character.State != GameCharacter.AIState.Dead)
            {
                character.GetCurrentCell();
                int currentPathIndex = character.currentPath.IndexOf(character.CurrentGridCell);
                if (currentPathIndex + 1 < character.currentPath.Count)
                {
                    GridCell nextCell = character.currentPath[currentPathIndex + 1];
                    if (nextCell.shopItem)
                    {
                        character.ForceRepath = true;
                    }
                }
            }

            character.animator.SetFloat("AttackSpeed", character.CurrentStats.AttackSpeed);
            character.animator.SetFloat("MovementSpeed", character.CurrentStats.MovementSpeed / character.OriginalStats.MovementSpeed);
            character.agent.speed = character.CurrentStats.MovementSpeed;

            List<BuffEffectInfo> toBeRemoved = new List<BuffEffectInfo>();

            foreach (BuffEffectInfo info in character.BuffList)
            {
                info.buff.UpdateBuff(info);
                if (info.CanBeRemoved())
                {
                    toBeRemoved.Add(info);
                }
            }

            foreach (BuffEffectInfo info in toBeRemoved)
            {
                info.buff.RemoveBuff(info);
            }

            if (character.CurrentStats.Health <= 0 && character.State != GameCharacter.AIState.Dead)
            {
                movement.Dead(character);
            }

            switch (character.State)
            {
                case GameCharacter.AIState.NoTarget:
                    movement.CharacterMovement(character);
                    break;

                case GameCharacter.AIState.Moving:
                    if (character.TargetEnemy.State == GameCharacter.AIState.Moving || character.State == GameCharacter.AIState.Moving || character.State == GameCharacter.AIState.Dead || character.ForceRepath)
                    {
                        movement.CharacterMovement(character);
                    }
                    break;

                case GameCharacter.AIState.Attacking:
                    if (character.TargetEnemy.State == GameCharacter.AIState.Moving || character.TargetEnemy.State == GameCharacter.AIState.Dead)
                    {
                        movement.CharacterMovement(character);
                    }
                    else
                    {
                        character.FaceTarget();
                        if (character.CurrentStats.Energy == character.OriginalStats.Energy && character.SpecialAttackEffect != null)
                        {
                            character.ChangeState(GameCharacter.AIState.SpecialAttacking);
                        }
                    }
                    break;
                case GameCharacter.AIState.SpecialAttacking:
                    if (character.TargetEnemy.State == GameCharacter.AIState.Moving || character.TargetEnemy.State == GameCharacter.AIState.Dead)
                    {
                        movement.CharacterMovement(character);
                    }
                    else
                    {
                        character.FaceTarget();
                        if (character.CurrentStats.Energy != character.OriginalStats.Energy)
                        {
                            character.ChangeState(GameCharacter.AIState.Attacking);
                        }
                    }
                    break;
            }


            //If both Characters targets the same cell, the one closest to the cell keeps moving, and the other one stops.
            if (character.TargetEnemy)
            {
                if (character.TargetEnemy.TargetGridCell == character.TargetGridCell && character.TargetGridCell != null)
                {
                    if (Vector3.Distance(character.transform.position, character.TargetGridCell.transform.position) > Vector3.Distance(character.TargetEnemy.transform.position, character.TargetGridCell.transform.position))
                    {
                        movement.CharacterMovement(character);
                    }
                }
            }

            //If a different character is in target cell, set to no Target
            if (character.State != GameCharacter.AIState.NoTarget && character.State != GameCharacter.AIState.Dead)
            {
                if (character.TargetGridCell)
                {
                    if (character.TargetGridCell.shopItem && character.TargetGridCell.shopItem != character)
                    {
                        movement.ResolveTie(character);
                        character.ForceRepath = false;
                    }
                }
            }

        }

        /// <summary>
        /// When the battle ends, check for the win or lose condition.
        /// </summary>
        public override void OnTimerFinish()
        {
            bool WinCondition = CheckWinCondition();

            if (WinCondition)
            {
                Battle.Instance.StartCoroutine(Win(3f));
            }
            else
            {
                Battle.Instance.StartCoroutine(Lose(3f));
            }
        }

        /// <summary>
        /// Coroutine called when winning the round.
        /// </summary>
        /// <param name="waitTime">Waiting time until next round.</param>
        /// <returns>Waiting time.</returns>
        IEnumerator Win(float waitTime)
        {
            Debug.Log("You win");
            RemoveElementsAtFinish();
            Battle.Instance.shopManager.currency += currencyPerWin;
            yield return new WaitForSeconds(waitTime);
            if (HideCells)
            {
                Battle.Instance.grid.ShowCells(true);
            }
            RemoveElementsAtFinish();
            ResetTeams();
            stage.NextState();
        }

        /// <summary>
        /// Coroutine called when losing the round.
        /// </summary>
        /// <param name="waitTime">Waiting time until the scene is reset.</param>
        /// <returns>Waiting time.</returns>
        IEnumerator Lose(float waitTime)
        {
            Debug.Log("You lose");
            RemoveElementsAtFinish();
            Battle.Instance.losePanel.gameObject.SetActive(true);
            Battle.Instance.timer.StopTimer();
            RemoveElementsAtFinish();
            ResetTeams();
            yield break;
        }

        /// <summary>
        /// Remove elements such projectiles and buffs from the field when the battle is over.
        /// </summary>
        private void RemoveElementsAtFinish()
        {
            RemoveAllProjectiles();
            RemoveAllBuffs();
        }

        /// <summary>
        /// Reset Team1 positions and remove Team2
        /// </summary>
        private void ResetTeams()
        {
            ResetTeamPositions(Team1);
            RemoveTeam(Team2);
        }

        /// <summary>
        /// Reset all character and moves them to the initial position.
        /// </summary>
        /// <param name="team">List of team members.</param>
        void ResetTeamPositions(List<GameCharacter> team)
        {
            foreach (GameCharacter ai in team)
            {
                ai.GetComponent<NavMeshAgent>().Warp(ai.StartingFightPosition);
                ai.ChangeState(GameCharacter.AIState.NoTarget);
                ai.animator.Play("Idle", -1, 0f);
            }
        }

        /// <summary>
        /// Remove the members of a team.
        /// </summary>
        /// <param name="team">List of team members.</param>
        void RemoveTeam(List<GameCharacter> team)
        {
            for (int i = 0; i < team.Count; i++)
            {
                Destroy(team[i].gameObject);
            }
            team.Clear();
        }

        /// <summary>
        /// Saves the starting position of all characters.
        /// </summary>
        public override void OnStageStart()
        {
            Team1 = Battle.Instance.teams[0].team;
            Team2 = Battle.Instance.teams[1].team;

            SetStartingTeamFightPosition(Team1);
            SetStartingTeamFightPosition(Team2);

            stageFinished = false;

            Battle.Instance.timer.ResetTimer(time);
            Battle.Instance.timer.StartTimer();

            if (HideCells) {
                Battle.Instance.grid.ShowCells(false);
            }
        }

        /// <summary>
        /// Calcels any dragging and saves the starting position of each character.
        /// </summary>
        /// <param name="team">Members of a team.</param>
        void SetStartingTeamFightPosition(List<GameCharacter> team)
        {
            foreach (GameCharacter ai in team)
            {
                ai.CancelDrag();
                ai.StartingFightPosition = ai.transform.position;
            }
        }
        
        /// <summary>
        /// Destroy all projectiles in play.
        /// </summary>
        void RemoveAllProjectiles()
        {
            foreach(Projectile projectile in FindObjectsOfType<Projectile>())
            {
                Destroy(projectile.gameObject);
            }
        }

        /// <summary>
        /// Remove all buffs from characters in play.
        /// </summary>
        void RemoveAllBuffs()
        {
            List<GameCharacter> all = Battle.Instance.teams[0].team.ToList();
            all.AddRange(Battle.Instance.teams[1].team);
            foreach (GameCharacter character in all)
            {
                foreach (BuffEffectInfo info in character.BuffList.ToList())
                {
                    info.buff.RemoveBuff(info);
                }
                character.BuffList.Clear();
            }
        }

        /// <summary>
        /// Checks if the win condition has been fullfiled.
        /// </summary>
        /// <returns>True if the player wins.</returns>
        public bool CheckWinCondition()
        {
            switch (winCondition)
            {
                case WinCondition.MoreCharactersAlive:
                    return CheckMoreCharatersAliveWinCondition();

                case WinCondition.MoreTotalHP:
                    return CheckMoreTotalHPWinCondition();

                case WinCondition.MorePercentualHP:
                    return CheckMorePercentualHPWinCondition();

                case WinCondition.Survive:
                    return CheckSurviveWinCondition();

                case WinCondition.KillAll:
                    return CheckKillAllWinCondition();
            }
            return false;
        }


        /// <summary>
        /// Check that the first team has more characters alive than the second team.
        /// </summary>
        /// <returns>First team has more alive characters.</returns>
        bool CheckMoreCharatersAliveWinCondition()
        {
            if (CheckKillAllWinCondition())
            {
                return true;
            }
            if (Battle.Instance.timer.timeRemaining <= 0)
            {
                int team1Alive = Team1.Count(x => x.State != GameCharacter.AIState.Dead);
                int team2Alive = Team2.Count(x => x.State != GameCharacter.AIState.Dead);

                return team1Alive > team2Alive;
            }
            return false;
        }

        /// <summary>
        /// Check that the first team has more total life than the second team.
        /// </summary>
        /// <returns>First team has more total life than the second team.</returns>
        bool CheckMoreTotalHPWinCondition()
        {
            if (CheckKillAllWinCondition())
            {
                return true;
            }
            if (Battle.Instance.timer.timeRemaining <= 0)
            {
                int team1RemainingHP = Team1.Sum(x => x.CurrentStats.Health);
                int team2RemainingHP = Team2.Sum(x => x.CurrentStats.Health);

                return team1RemainingHP > team2RemainingHP;
            }
            return false;
        }

        /// <summary>
        /// Check that the first team has more life in percentage than the second team.
        /// </summary>
        /// <returns>First team has more life in percentage than the second team.</returns>
        bool CheckMorePercentualHPWinCondition()
        {
            if (CheckKillAllWinCondition())
            {
                return true;
            }
            if (Battle.Instance.timer.timeRemaining <= 0)
            {
                int team1RemainingHP = Team1.Sum(x => x.CurrentStats.Health);
                int team2RemainingHP = Team2.Sum(x => x.CurrentStats.Health);

                int team1InitialHP = Team1.Sum(x => x.InitialStats.Health);
                int team2InitialHP = Team2.Sum(x => x.InitialStats.Health);

                float team1Percentage = team1RemainingHP / (float)team1InitialHP;
                float team2Percentage = team2RemainingHP / (float)team2InitialHP;

                return team1Percentage > team2Percentage;
            }
            return false;
        }

        /// <summary>
        /// Check that at the end of the round, there is at least one character from the first team alive.
        /// </summary>
        /// <returns>There is at least one character from the first team alive</returns>
        bool CheckSurviveWinCondition()
        {
            if (CheckKillAllWinCondition())
            {
                return true;
            }
            if (Battle.Instance.timer.timeRemaining <= 0)
            {
                int team1Alive = Team1.Count(x => x.State != GameCharacter.AIState.Dead);
                return team1Alive > 0;
            }
            return false;
        }

        /// <summary>
        /// Check that there are no characters from the second team alive.
        /// </summary>
        /// <returns>No characters from the second team alive.</returns>
        bool CheckKillAllWinCondition()
        {
            int team2Alive = Team2.Count(x => x.State != GameCharacter.AIState.Dead);
            return team2Alive == 0;
        }

        /// <summary>
        /// Checks if the lose condition has been fullfiled.
        /// </summary>
        /// <returns>True if the player loses.</returns>
        public bool CheckLoseCondition()
        {
            switch (loseCondition)
            {
                case LoseCondition.DefeatedAll:
                    return CheckDefeatedAllLoseCondition();
            }
            return false;
        }

        /// <summary>
        /// Check that there are no characters from the first team alive.
        /// </summary>
        /// <returns>No characters from the first team alive.</returns>
        bool CheckDefeatedAllLoseCondition()
        {
            int team1Alive = Team1.Count(x => x.State != GameCharacter.AIState.Dead);
            return team1Alive == 0;
        }
    }
}