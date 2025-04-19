using AutoBattleFramework.BattleBehaviour;
using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Battlefield;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AutoBattleFramework.Movement
{
    /// <summary>
    /// Determine how the characters move and choose the target.
    /// </summary>
    public abstract class IBattleMovement
    {
        public IBattleMovement()
        {
        }

        /// <summary>
        /// Determines the next position to which the character will move.
        /// </summary>
        /// <param name="character">Character to be moved.</param>
        /// <param name="cell">Final destination to which you want to move the character.</param>
        /// <param name="forceToCenter">Move the character to the center of the cell. Used when the final destination has been reached and proceeds to attack.</param>
        protected virtual void MoveTo(GameCharacter character, GridCell cell, bool forceToCenter = false)
        {
            //Set the target grid cell and find a path to that cell.
            character.TargetGridCell = cell;
            character.currentPath = PathFinding2D.find(character.CurrentGridCell, cell, Battle.Instance);

            //Get the current cell index in path.
            int currentPathIndex = character.currentPath.IndexOf(character.CurrentGridCell);

            if (currentPathIndex >= 0)
            {
                if (currentPathIndex + 1 < character.currentPath.Count)
                {
                    //Move to the next cell in path.
                    character.agent.SetDestination(character.currentPath[currentPathIndex + 1].transform.position);
                    character.ChangeState(GameCharacter.AIState.Moving);
                }
            }
            else
            {
                if (character.CurrentGridCell.shopItem == character)
                {
                    //No target, set the target to the current gridCell
                    character.TargetGridCell = character.CurrentGridCell;
                }

            }

            if (forceToCenter)
            {
                //Force the character to move to the center of the cell.
                character.agent.velocity = Vector3.zero;
                character.agent.SetDestination(cell.transform.position);
            }
        }

        /// <summary>
        /// This method is invoked to solve the problem of two characters arriving at the same square. The second one that has arrived moves to the nearest unoccupied square.
        /// </summary>
        /// <param name="character">Second character that has arrived in the cell.</param>
        public virtual void ResolveTie(GameCharacter character)
        {
            GridCell target = character.GetClosestCellDifferentToCurrent();
            MoveTo(character, target);
            Debug.Log("Tie resolve: " + character.gameObject.name + " to " + target.name);
        }

        /// <summary>
        /// Main method of movement. It is responsible for setting a target and moving the character.
        /// </summary>
        /// <param name="ai"></param>
        public virtual void CharacterMovement(GameCharacter ai)
        {
            GameCharacter enemy = FindNearestEnemy(ai);

            if (enemy)
            {
                GridCell target = SetTargetAndMove(ai, enemy);

                if (target == ai.CurrentGridCell && ai.CurrentGridCell.shopItem == ai)
                {
                    if (ai.InAttackRange())
                    {
                        MoveTo(ai, target, true);
                        ai.ChangeState(GameCharacter.AIState.Attacking);
                    }
                    else
                    {
                        //If agent velocity is zero, it means that there is no path to the target and it�s stuck moving on the current tile, so set it to NoTarget
                        Vector3 velocityV = ai.agent.velocity;
                        float velocity = velocityV.magnitude / ai.agent.speed;
                        if (velocity == 0)
                        {
                            NoTarget(ai);
                        }
                    }
                }

            }
            else
            {
                NoTarget(ai);
            }
        }

        /// <summary>
        /// Set the enemy target, the target cell to move and start the movement to that cell.
        /// </summary>
        /// <param name="character">Character to set target.</param>
        /// <param name="enemy">Enemy character that os the current target.</param>
        /// <returns>Target cell where the character will move.</returns>
        protected virtual GridCell SetTargetAndMove(GameCharacter character, GameCharacter enemy)
        {
            GridCell target = null;
            if (character.CurrentGridCell == enemy.CurrentGridCell && character != character.CurrentGridCell.shopItem)
            {
                target = character.GetClosestCellDifferentToCurrent();
            }
            else if (character.TargetEnemy == enemy && enemy.TargetEnemy == character)
            {
                target = enemy.CurrentGridCell.FindNearestGridCell(character.CurrentStats.Range, character.CurrentGridCell, character);
                bool areEqual = character.currentPath.SequenceEqual(enemy.currentPath.AsEnumerable().Reverse());

                if (!areEqual)
                {
                    GameCharacter slowest = character.CurrentStats.MovementSpeed > enemy.CurrentStats.MovementSpeed ? enemy : character;
                    GameCharacter fastest = character.CurrentStats.MovementSpeed > enemy.CurrentStats.MovementSpeed ? character : enemy;
                    slowest.currentPath = fastest.currentPath;
                }
            }
            else
            {
                target = enemy.CurrentGridCell.FindNearestGridCell(character.CurrentStats.Range, character.CurrentGridCell, character);
            }
            MoveTo(character, target);
            character.TargetEnemy = enemy;


            return character.TargetGridCell;
        }

        /// <summary>
        /// Given a character, search for the nearest enemy character.
        /// </summary>
        /// <param name="character">Character looking for an enemy.</param>
        /// <returns>Nearest enemy character from the given character.</returns>
        protected virtual GameCharacter FindNearestEnemy(GameCharacter character)
        {
            TeamData data = Battle.Instance.teams.Where(x => x.team.Contains(character)).FirstOrDefault();

            List<GameCharacter> team = new List<GameCharacter>();

            foreach (TeamData teamData in Battle.Instance.teams)
            {
                if (teamData != data)
                {
                    team.AddRange(teamData.team);
                }
            }

            /*
            List<GameCharacter> team = Battle.Instance.teams[0].team;

            if (Battle.Instance.teams[0].team.Contains(character))
            {
                team = Battle.Instance.teams[1].team;
            }*/

            GameCharacter nearest = null;
            float distance = float.MaxValue;

            foreach (GameCharacter ai in team)
            {
                if (ai.State != GameCharacter.AIState.Dead)
                {
                    float dst = Vector3.Distance(character.transform.position, ai.transform.position);
                    if (dst < distance)
                    {
                        distance = dst;
                        nearest = ai;
                    }
                }
            }
            return nearest;
        }

        /// <summary>
        /// Set the character state as <see cref="BattleBehaviour.GameActors.GameCharacter.AIState.NoTarget"/>
        /// </summary>
        /// <param name="character">Character without target.</param>
        public void NoTarget(GameCharacter character)
        {
            MoveTo(character, character.CurrentGridCell);
            character.TargetGridCell = null;
            character.ChangeState(GameCharacter.AIState.NoTarget);
        }

        /// <summary>
        /// Set the caracter state as <see cref="BattleBehaviour.GameActors.GameCharacter.AIState.Dead"/>
        /// </summary>
        /// <param name="character"></param>
        public void Dead(GameCharacter character)
        {
            character.State = GameCharacter.AIState.Dead;
            character.ChangeState(GameCharacter.AIState.Dead);
            character.TargetEnemy = null;
            character.GetComponent<NavMeshAgent>().enabled = false;
            character.CurrentGridCell.shopItem = null;
            character.CurrentGridCell = null;
        }
    }
}