using AutoBattleFramework.Movement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using AutoBattleFramework.BattleBehaviour;

namespace AutoBattleFramework.Battlefield
{
    /// <summary>
    /// It allows the movement of characters through the cells that compose it. The Custom Inspector allows to create a grid quickly.
    /// </summary>
    public class BattleGrid : MonoBehaviour
    {
        /// <summary>
        /// Grid generation type
        /// </summary>
        public enum GridType
        {
            Squared,
            Hex
        }

        /// <summary>
        /// Type of grid that will be generated.
        /// </summary>
        [Tooltip("Type of grid that will be generated.")]
        public GridType GridShape;

        /// <summary>
        /// Prefab of the cell used if the grid type is <see cref="GridType.Squared"/>.
        /// </summary>
        [Tooltip("Prefab of the cell used if the grid type is Squared.")]
        public GridCell GridSquared;

        /// <summary>
        /// Prefab of the cell used if the grid type is <see cref="GridType.Hex"/>.
        /// </summary>
        [Tooltip("Prefab of the cell used if the grid type is Squared.")]
        public GridCell GridHex;

        /// <summary>
        /// Separation distance between cells.
        /// </summary>
        [Tooltip("Separation distance between cells.")]
        public float separation;

        /// <summary>
        /// If true, rotate the cells 15 degrees if hex, 45 if squared.
        /// </summary>
        [Tooltip("Rotate the cells.")]
        public bool RotateCells = false;

        /// <summary>
        /// When <see cref="GridSquared"/> or <see cref="GridHex"/> are instantiated, its X and Z local scale will be multiplied by this value.
        /// </summary>
        [Tooltip("When cells are instantiated, its X and Z scale will be multiplied by this value.")]
        public float CellScale = 1f;

        /// <summary>
        /// Number of cells across the width of the grid.
        /// </summary>
        [Tooltip("Number of cells across the width of the grid.")]
        public int GridWidth;

        /// <summary>
        /// Number of cells across the width of the grid.
        /// </summary>
        [Tooltip("Number of cells across the height of the grid.")]
        public int GridHeight;

        /// <summary>
        /// Number of rows assigned to team 1. The rest will be assigned to group 2.
        /// </summary>
        [Tooltip("Number of rows assigned to team 1. The rest will be assigned to group 2.")]
        public int Team1RowNumber;

        /// <summary>
        /// Color of the cell of Team 1 if it is empty.
        /// </summary>
        [Tooltip("Color of the cell of Team 1 if it is empty.")]
        public Color Team1EmptyCell;

        /// <summary>
        /// Color of the cell of Team 1 if a character is on it.
        /// </summary>
        [Tooltip("Color of the cell of Team 1 if a character is on it.")]
        public Color Team1OccupiedCell;

        /// <summary>
        /// Color of the cell of Team 1 if a character is being dragged over it.
        /// </summary>
        [Tooltip("Color of the cell of Team 1 if a character is being dragged over it.")]
        public Color Team1Drag;


        /// <summary>
        /// Color of the cell of Team 2 if it is empty.
        /// </summary>
        [Tooltip("Color of the cell of Team 2 if it is empty.")]
        public Color Team2EmptyCell;
        /// <summary>
        /// Color of the cell of Team 2 if a character is on it.
        /// </summary>
        [Tooltip("Color of the cell of Team 2 if a character is on it.")]
        public Color Team2OccupiedCell;
        /// <summary>
        /// Colo of the cell of Team 2 if a character is being dragged over it.
        /// </summary>
        [Tooltip("Color of the cell of Team 2 if a character is being dragged over it.")]
        public Color Team2Drag;

        /// <summary>
        /// Cells that compose the grid.
        /// </summary>
        [Tooltip("Cells that compose the grid.")]
        [HideInInspector]
        public GridCell[] GridCells;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Method used by the custom inspector to create a grid quickly.
        /// </summary>
        public void SpawnGridEditor()
        {
            DeleteGrid();
            SpawnGrid();
        }

        /// <summary>
        /// Removes the previous grid.
        /// </summary>
        void DeleteGrid()
        {
            foreach (GridCell cell in GridCells)
            {
                DestroyImmediate(cell.gameObject);
            }
        }

        /// <summary>
        /// Creates a new grid based on <see cref="GridShape"/>.
        /// </summary>
        void SpawnGrid()
        {
            GridCells = new GridCell[GridWidth * GridHeight];
            if (GridShape == GridType.Squared)
            {
                SpawnSquaredGrid(separation);
            }
            else if (GridShape == GridType.Hex)
            {
                SpawnHexGrid(separation);
            }
        }

        /// <summary>
        /// Spawns a squared grid.
        /// </summary>
        /// <param name="separation">Separation between cells.</param>
        void SpawnSquaredGrid(float separation)
        {
            int index = 0;
            Vector3 degrees30 = new Vector3(0f, 30f, 0f);
            for (int i = 0; i < GridWidth; i++)
            {
                for (int j = 0; j < GridHeight; j++)
                {
                    Vector3 position = transform.position;
                    if (!RotateCells)
                    {
                        SpawnSquaredNoRotation(position, i, j, 0, index);
                    }
                    else
                    {
                        float offset = 0f;
                        if (i % 2 != 0)
                        {
                            offset = (Mathf.Sqrt(Mathf.Pow(CellScale, 2) + Mathf.Pow(CellScale, 2)) + separation) /2;
                        }
                        SpawnSquaredRotation(position, i, j, offset, index);
                    }
                    ScaleCell(GridCells[index]);
                    GridCells[index].transform.name = "Cell " + i + "-" + j;
                    if (j < Team1RowNumber)
                    {
                        GridCells[index].CanPlaceCharacter = 0;
                        GridCellEffect effect = GridCells[index].GetComponent<GridCellEffect>();
                        if (effect)
                        {
                            effect.Empty = Team1EmptyCell;
                            effect.NotEmpty = Team1OccupiedCell;
                            effect.DragOver = Team1Drag;
                            effect.SetColor(effect.Empty);
                        }
                    }
                    else
                    {
                        GridCellEffect effect = GridCells[index].GetComponent<GridCellEffect>();
                        if (effect)
                        {
                            effect.Empty = Team2EmptyCell;
                            effect.NotEmpty = Team2OccupiedCell;
                            effect.DragOver = Team2Drag;
                            effect.SetColor(effect.Empty);
                        }
                    }
                    index++;
                }
            }
        }

        /// <summary>
        /// Spawns an hex grid.
        /// </summary>
        /// <param name="separation">Separation between cells.</param>
        void SpawnHexGrid(float separation)
        {
            int index = 0;
            for (int i = 0; i < GridWidth; i++)
            {
                for (int j = 0; j < GridHeight; j++)
                {
                    if (!RotateCells)
                    {
                        Vector3 position = transform.position;
                        float offset = 0;
                        if (j % 2 != 0)
                        {
                            offset = (CellScale + separation) / 2;
                        }

                        SpawnNoRotationHex(position, i, j, offset, index);
                    }
                    else {
                        Vector3 position = transform.position;
                        float offset = 0;
                        if (i % 2 != 0)
                        {
                            offset = (CellScale + separation) / 2;
                        }
                        SpawnRotationHex(position,i,j,offset,index);
                    }
                    ScaleCell(GridCells[index]);
                    GridCells[index].transform.name = "Cell " + i + "-" + j;
                    if (j < Team1RowNumber)
                    {
                        GridCells[index].CanPlaceCharacter = 0;
                        GridCellEffect effect = GridCells[index].GetComponent<GridCellEffect>();
                        if (effect)
                        {
                            effect.Empty = Team1EmptyCell;
                            effect.NotEmpty = Team1OccupiedCell;
                            effect.DragOver = Team1Drag;
                            effect.SetColor(effect.Empty);
                        }
                    }
                    else
                    {
                        GridCellEffect effect = GridCells[index].GetComponent<GridCellEffect>();
                        if (effect)
                        {
                            effect.Empty = Team2EmptyCell;
                            effect.NotEmpty = Team2OccupiedCell;
                            effect.DragOver = Team2Drag;
                            effect.SetColor(effect.Empty);
                        }
                    }
                    index++;
                }
            }
        }

        /// <summary>
        /// Used to spawn an hex grid cell without rotation
        /// </summary>
        /// <param name="position">Position of grid</param>
        /// <param name="i">i of cell</param>
        /// <param name="j">j of cell</param>
        /// <param name="offset">Offset of the cell</param>
        /// <param name="index">Current index of the cell.</param>
        void SpawnNoRotationHex(Vector3 position, int i, int j, float offset, int index)
        {
            position.x += i * CellScale + CellScale / 2 + offset;
            position.x += separation * i;
            position.z += j * CellScale + CellScale / 2;
            position.z += separation * j - CellScale * 0.125f * j;
            GridCells[index] = Instantiate(GridHex.gameObject, position, transform.rotation, transform).GetComponent<GridCell>();
        }

        /// <summary>
        /// Used to spawn a, hex grid cell rotated
        /// </summary>
        /// <param name="position">Position of grid</param>
        /// <param name="i">i of cell</param>
        /// <param name="j">j of cell</param>
        /// <param name="offset">Offset of the cell</param>
        /// <param name="index">Current index of the cell.</param>
        void SpawnRotationHex(Vector3 position, int i, int j, float offset, int index)
        {
            position.x += i * CellScale + CellScale / 2;
            position.x += separation * i - CellScale * 0.125f * i;
            position.z += j * CellScale + CellScale / 2 + offset;
            position.z += separation * j;
            GridCells[index] = Instantiate(GridHex.gameObject, position, transform.rotation, transform).GetComponent<GridCell>();
            GridCells[index].transform.rotation = Quaternion.Euler(GridCells[index].transform.rotation.eulerAngles + new Vector3(0f, 30f, 0f));
        }

        /// <summary>
        /// Used to spawn an squared grid cell without rotation
        /// </summary>
        /// <param name="position">Position of grid</param>
        /// <param name="i">i of cell</param>
        /// <param name="j">j of cell</param>
        /// <param name="offset">Offset of the cell</param>
        /// <param name="index">Current index of the cell.</param>
        void SpawnSquaredNoRotation(Vector3 position, int i, int j, float offset, int index)
        {
            position.x += i * CellScale + CellScale / 2;
            position.x += separation * i;
            position.z += j * CellScale + CellScale / 2;
            position.z += separation * j;
            GridCells[index] = Instantiate(GridSquared.gameObject, position, transform.rotation, transform).GetComponent<GridCell>();
        }

        /// <summary>
        /// Used to spawn a, squared grid cell rotated
        /// </summary>
        /// <param name="position">Position of grid</param>
        /// <param name="i">i of cell</param>
        /// <param name="j">j of cell</param>
        /// <param name="offset">Offset of the cellin X axis</param>
        /// <param name="index">Current index of the cell.</param>
        void SpawnSquaredRotation(Vector3 position, int i, int j, float offset, int index)
        {
            float diagonal = Mathf.Sqrt(Mathf.Pow(CellScale, 2) + Mathf.Pow(CellScale, 2));
            position.x += i * diagonal + diagonal / 2;
            position.x += separation * i - diagonal * 0.5f * i;
            position.z += j * diagonal + diagonal *0.5f + offset;
            position.z += separation * j;
            GridCells[index] = Instantiate(GridSquared.gameObject, position, transform.rotation, transform).GetComponent<GridCell>();
            GridCells[index].transform.rotation = Quaternion.Euler(GridCells[index].transform.rotation.eulerAngles + new Vector3(0f, 45f, 0f));
        }

        /// <summary>
        /// Update the colors of the grid
        /// </summary>
        public void UpdateColors()
        {
            int index = 0;
            for (int i = 0; i < GridWidth; i++)
            {
                for (int j = 0; j < GridHeight; j++)
                {
                    if (j < Team1RowNumber)
                    {
                        GridCellEffect effect = GridCells[index].GetComponent<GridCellEffect>();
                        if (effect)
                        {
                            effect.Empty = Team1EmptyCell;
                            effect.NotEmpty = Team1OccupiedCell;
                            effect.DragOver = Team1Drag;
                            effect.SetColor(effect.Empty);
                        }
                    }
                    else
                    {
                        GridCellEffect effect = GridCells[index].GetComponent<GridCellEffect>();
                        if (effect)
                        {
                            effect.Empty = Team2EmptyCell;
                            effect.NotEmpty = Team2OccupiedCell;
                            effect.DragOver = Team2Drag;
                            effect.SetColor(effect.Empty);
                        }
                    }
                    index++;
                }
            }
        }

        /// <summary>
        /// Multiply the X and Z local scale of a cell by <see cref="CellScale"/>.
        /// </summary>
        /// <param name="cell">Cell to be scaled.</param>
        void ScaleCell(GridCell cell)
        {
            Vector3 lscale = cell.gameObject.transform.localScale;
            lscale.x *= CellScale;
            lscale.z *= CellScale;
            cell.gameObject.transform.localScale = lscale;
        }

        /// <summary>
        /// Pre-calculates the distances of all cells from each other.
        /// </summary>
        public void CalculateDistances()
        {
            for (int i = 0; i < GridCells.Length; i++)
            {
                for(int j = 0; j < GridCells.Length; j++)
                {
                    if (GridCells[i].distancesToOtherCells.Length == 0)
                    {
                        GridCells[i].distancesToOtherCells = Enumerable.Repeat(-1, GridCells.Length).ToArray();
                    }
                    if (GridCells[j].distancesToOtherCells.Length == 0)
                    {
                        GridCells[j].distancesToOtherCells = Enumerable.Repeat(-1, GridCells.Length).ToArray();
                    }
                    if (GridCells[i].distancesToOtherCells[j] == -1)
                    {
                        GridCells[i].distancesToOtherCells[j] = PathFinding2D.find(GridCells[i], GridCells[j], Battle.Instance, true).Count - 1;
                        GridCells[j].distancesToOtherCells[i] = GridCells[i].distancesToOtherCells[j];
                        GridCells[i].grid = this;
                        GridCells[j].grid = this;
                    }
                }
            }
        }

        /// <summary>
        /// Find the neighbors of each cell in a hexagonal grid.
        /// </summary>
        public void FindHexNeighbors()
        {
            foreach (GridCell cell in GridCells)
            {
                cell.Neighbors = PathFinding2D.FindNeighbors6x(cell, this);
            }
        }

        /// <summary>
        /// Find the neighbors of each cell in a squared grid.
        /// </summary>
        public void FindSquaredNeighbors()
        {
            foreach (GridCell cell in GridCells)
            {
                cell.Neighbors = PathFinding2D.FindNeighbors4x(cell, this);
            }
        }


        /// <summary>
        /// Show or hide the sprites of the cells.
        /// </summary>
        /// <param name="show">Show or hide the cells.</param>
        public void ShowCells(bool show)
        {
            foreach(GridCell cell in GridCells)
            {
                cell.GetComponentInChildren<SpriteRenderer>().enabled = show;
            }
        }
    }
}