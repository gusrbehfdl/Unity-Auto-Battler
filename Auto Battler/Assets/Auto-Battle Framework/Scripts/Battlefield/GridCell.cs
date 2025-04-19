using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AutoBattleFramework.Utility;
using AutoBattleFramework.BattleBehaviour;
using AutoBattleFramework.BattleBehaviour.GameActors;

namespace AutoBattleFramework.Battlefield
{
    /// <summary>
    /// Cell that composes the <see cref="BattleGrid"/> and <see cref="Bench"/>, and that allows the movement and battle of characters.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class GridCell : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="BattleBehaviour.GameActors.GameCharacter"/> or <see cref="BattleBehaviour.GameActors.GameItem"/> that is occupying the cell.
        /// </summary>
        public GameActor shopItem;

        /// <summary>
        /// Cell neighbors. If the cell type is hexagonal, it will have up to 6 neighbors, if it is square, it will have up to 4 neighbors.
        /// </summary>
        public List<GridCell> Neighbors;

        /// <summary>
        /// Index of the player who can place characters on this cell.
        /// Used to prevent characters from different teams from occupying cells belonging to other teams.
        /// </summary>
        public int CanPlaceCharacter = 1;

        /// <summary>
        /// Reference to the grid where the cell is located.
        /// </summary>
        [ReadOnly] public BattleGrid grid;

        /// <summary>
        /// Distance from this cell to the rest of the cells in the grid.
        /// </summary>
        [ReadOnly] public int[] distancesToOtherCells;

        /// <summary>
        /// Given another cell and the character occupying it, find the nearest unoccupied cell within the radius. If the cell passed as a parameter is within the radius and is occupied by the character, it returns that one instead.
        /// </summary>
        /// <param name="radius">Radius where to look for the nearest cell.</param>
        /// <param name="other">Another cell occupied by the character.</param>
        /// <param name="character">Character that occupies the cell passed as parameter.</param>
        /// <returns>Cell closest to this one within the radius. If the parameter cell is within the radius and is occupied by the parameter character, it returns that one instead.</returns>
        public GridCell FindNearestGridCell(float radius, GridCell other, GameCharacter character)
        {
            List<GridCell> inRadius = FindGridCellsInRadius(radius);
            float distance = float.MaxValue;
            GridCell nearestCell = other;
            if (other)
            {
                if (inRadius.Contains(other))
                {
                    if (other.shopItem == character || other.shopItem == null)
                        return other;
                }
                foreach (GridCell cell in inRadius)
                {     
                    if (!cell.shopItem && cell != other)
                    {
                        float currentDistance = Vector3.Distance(cell.transform.position, other.transform.position);
                        if (currentDistance < distance)
                        {
                            distance = currentDistance;
                            nearestCell = cell;
                        }
                    }
                }
            }
            return nearestCell;
        }

        /// <summary>
        /// Find all cells in the radius.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns>List of cells within radius</returns>
        List<GridCell> FindGridCellsInRadius(float radius)
        {
            List<GridCell> inRadius = new List<GridCell>();
            for(int i = 0; i < distancesToOtherCells.Length; i++)
            {
                if (distancesToOtherCells[i] > 0 && distancesToOtherCells[i] <= radius)
                {
                    inRadius.Add(grid.GridCells[i]);
                }
            }
            return inRadius;
        }

        /// <summary>
        /// Sets the value of the drag effect.
        /// </summary>
        /// <param name="value">If should change the color of the cell to <see cref="GridCellEffect.DragOver"/>.</param>
        public void SetDragEffect(bool value)
        {
            GridCellEffect effect = GetComponent<GridCellEffect>();
            if (effect)
            {
                effect.CharacterDrag = value;
            }
        }

        /// <summary>
        /// Get the cell distance between this cell and other.
        /// </summary>
        /// <param name="cell">Cell in the grid</param>
        /// <returns>Distance between both cells.</returns>
        public int DistanceToOtherCell(GridCell cell)
        {
            int cellIndex = System.Array.IndexOf(cell.grid.GridCells, cell);

            return distancesToOtherCells[cellIndex];
        }
    }


}