using AutoBattleFramework.BattleBehaviour;
using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Battlefield;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Movement
{
    /// <summary>
    /// The characters will move, if possible, every two cells for a more natural movement.
    /// </summary>
    public class ApproximateAstarMovement : IBattleMovement
    {
        public ApproximateAstarMovement() : base()
        {
        }


        /// <summary>
        /// Determines the next position to which the character will move. Given a path, the character will move every two cells, for a more natural movement.
        /// </summary>
        /// <param name="character">Character to be moved.</param>
        /// <param name="cell">Final destination to which you want to move the character.</param>
        /// <param name="forceToCenter">Move the character to the center of the cell. Used when the final destination has been reached and proceeds to attack.</param>
        protected override void MoveTo(GameCharacter ai, GridCell cell, bool forceToCenter = false)
        {
            bool sameCell = cell == ai.CurrentGridCell;
            if (sameCell)
                forceToCenter = true;
            ai.TargetGridCell = cell;

            ai.currentPath = PathFinding2D.find(ai.CurrentGridCell, cell, Battle.Instance);
            int currentPathIndex = ai.currentPath.IndexOf(ai.CurrentGridCell);

            if (currentPathIndex >= 0)
            {
                if (currentPathIndex + 2 < ai.currentPath.Count)
                {
                    ai.agent.SetDestination(ai.currentPath[currentPathIndex + 2].transform.position);
                    ai.ChangeState(GameCharacter.AIState.Moving);
                }
                else if (currentPathIndex + 1 < ai.currentPath.Count)
                {
                    ai.agent.SetDestination(ai.currentPath[currentPathIndex + 1].transform.position);
                    ai.ChangeState(GameCharacter.AIState.Moving);
                }
            }
            else
            {
                if (ai.CurrentGridCell.shopItem == ai)
                {
                    //No target, set the target to the current gridCell
                    ai.TargetGridCell = ai.CurrentGridCell;
                }

            }
            if (forceToCenter)
            {
                ai.agent.velocity = Vector3.zero;
                ai.agent.SetDestination(cell.transform.position);
            }
        }      
    }
}