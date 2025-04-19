using AutoBattleFramework.BattleBehaviour;
using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Battlefield;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoBattleFramework.Movement
{
    /// <summary>
    /// A* algorithm, used by <see cref="BattleBehaviour.GameActors.GameCharacter"/> to move through the <see cref="Battlefield.BattleGrid"/>.
    /// This script is a modification of UnityPathFinding2D, under MIT license. The original script can be found at: https://github.com/folospace/UnityPathFinding2D
    /// </summary>
    public class PathFinding2D
    {
        /// <summary>
        /// Get the path from an initial cell to a target cell.
        /// </summary>
        /// <param name="from">Initial cell</param>
        /// <param name="to">Target cell</param>
        /// <param name="battle">Current battle</param>
        /// <param name="GetPathWithoutObstacles">Optional parameter. Set it to true to ignore other GameCharacters. Useful to find the distance between cells.</param>
        /// <returns>List of cells that build a path from the initial to the final cell.</returns>
        public static List<GridCell> find(GridCell from, GridCell to, Battle battle, bool GetPathWithoutObstacles = false)
        {
            Func<GridCell, GridCell, float> getDistance = delegate (GridCell a, GridCell b)
            {
                return Vector3.Distance(a.transform.position, b.transform.position);
            };

            Func<GridCell, List<GridCell>> getNeighbors = delegate (GridCell cell)
            {
                return cell.Neighbors;
            };

            //If using from the inspector, find the Battle object
            if(!battle)
                battle = GameObject.FindObjectOfType<Battle>();

            return astar(from, to, battle.grid, getDistance, getNeighbors, battle, GetPathWithoutObstacles);
        }

        /// <summary>
        /// Find the neighbors cells of a given cell, used in Hex grid types.
        /// </summary>
        /// <param name="cell">Cell to look for neighbors.</param>
        /// <param name="grid">Current grid of cells.</param>
        /// <returns>List of neighbor cells.</returns>
        public static List<GridCell> FindNeighbors6x(GridCell cell, BattleGrid grid)
        {
            List<GridCell> neighbors = new List<GridCell>();

            RaycastHit hit1 = new RaycastHit();
            RaycastHit hit2 = new RaycastHit();
            RaycastHit hit3 = new RaycastHit();
            RaycastHit hit4 = new RaycastHit();
            RaycastHit hit5 = new RaycastHit();
            RaycastHit hit6 = new RaycastHit();

            Vector3 rayPosition = cell.transform.position;
            Vector3 RayRotation1 = Quaternion.AngleAxis(-30, cell.transform.up) * cell.transform.forward;
            Vector3 RayRotation2 = Quaternion.AngleAxis(30, cell.transform.up) * cell.transform.forward;
            Vector3 RayRotation3 = Quaternion.AngleAxis(-90, cell.transform.up) * cell.transform.forward;
            Vector3 RayRotation4 = Quaternion.AngleAxis(90, cell.transform.up) * cell.transform.forward;
            Vector3 RayRotation5 = Quaternion.AngleAxis(-150, cell.transform.up) * cell.transform.forward;
            Vector3 RayRotation6 = Quaternion.AngleAxis(150, cell.transform.up) * cell.transform.forward;

            Physics.Raycast(rayPosition, RayRotation1, out hit1, 10f, LayerMask.GetMask("GridCell"));
            Physics.Raycast(rayPosition, RayRotation2, out hit2, 10f, LayerMask.GetMask("GridCell"));
            Physics.Raycast(rayPosition, RayRotation3, out hit3, 10f, LayerMask.GetMask("GridCell"));
            Physics.Raycast(rayPosition, RayRotation4, out hit4, 10f, LayerMask.GetMask("GridCell"));
            Physics.Raycast(rayPosition, RayRotation5, out hit5, 10f, LayerMask.GetMask("GridCell"));
            Physics.Raycast(rayPosition, RayRotation6, out hit6, 10f, LayerMask.GetMask("GridCell"));

            List<RaycastHit> hits = new List<RaycastHit>();
            hits.Add(hit1);
            hits.Add(hit2);
            hits.Add(hit3);
            hits.Add(hit4);
            hits.Add(hit5);
            hits.Add(hit6);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider)
                {
                    if (hit.collider != cell.GetComponent<Collider>())
                    {
                        if (grid.GridCells.Contains(hit.collider.GetComponent<GridCell>()))
                        {
                            neighbors.Add(hit.collider.GetComponent<GridCell>());
                        }
                    }
                }
            }
            return neighbors;
        }

        /// <summary>
        /// Find the neighbors cells of a given cell, used in Squared grid types.
        /// </summary>
        /// <param name="cell">Cell to look for neighbors.</param>
        /// <param name="grid">Current grid of cells.</param>
        /// <returns>List of neighbor cells.</returns>
        public static List<GridCell> FindNeighbors4x(GridCell cell, BattleGrid grid)
        {
            List<GridCell> neighbors = new List<GridCell>();

            RaycastHit hit1 = new RaycastHit();
            RaycastHit hit2 = new RaycastHit();
            RaycastHit hit3 = new RaycastHit();
            RaycastHit hit4 = new RaycastHit();


            Vector3 rayPosition = cell.transform.position;
            Vector3 RayRotation1 = Quaternion.AngleAxis(0, cell.transform.up) * cell.transform.forward;
            Vector3 RayRotation2 = Quaternion.AngleAxis(180, cell.transform.up) * cell.transform.forward;
            Vector3 RayRotation3 = Quaternion.AngleAxis(-90, cell.transform.up) * cell.transform.forward;
            Vector3 RayRotation4 = Quaternion.AngleAxis(90, cell.transform.up) * cell.transform.forward;

            Physics.Raycast(rayPosition, RayRotation1, out hit1, 10f, LayerMask.GetMask("GridCell"));
            Physics.Raycast(rayPosition, RayRotation2, out hit2, 10f, LayerMask.GetMask("GridCell"));
            Physics.Raycast(rayPosition, RayRotation3, out hit3, 10f, LayerMask.GetMask("GridCell"));
            Physics.Raycast(rayPosition, RayRotation4, out hit4, 10f, LayerMask.GetMask("GridCell"));


            List<RaycastHit> hits = new List<RaycastHit>();
            hits.Add(hit1);
            hits.Add(hit2);
            hits.Add(hit3);
            hits.Add(hit4);


            foreach (RaycastHit hit in hits)
            {
                if (hit.collider)
                {
                    if (hit.collider != cell.GetComponent<Collider>())
                    {
                        if (grid.GridCells.Contains(hit.collider.GetComponent<GridCell>()))
                        {
                            neighbors.Add(hit.collider.GetComponent<GridCell>());
                        }
                    }
                }
            }
            return neighbors;
        }


        /// <summary>
        /// Applies the A* algorithm to find a path.
        /// </summary>
        static List<GridCell> astar(GridCell from, GridCell to, BattleGrid grid,
                          Func<GridCell, GridCell, float> getDistance, Func<GridCell, List<GridCell>> getNeighbors, Battle battle, bool GetPathWithoutObstacles)
        {
            var result = new List<GridCell>();
            if (from == to)
            {
                result.Add(from);
                return result;
            }
            Node finalNode;
            List<Node> open = new List<Node>();
            if (findDest(new Node(null, from, getDistance(from, to), 0), open, grid, to, out finalNode, getDistance, getNeighbors, battle, GetPathWithoutObstacles))
            {
                while (finalNode != null)
                {
                    result.Add(finalNode.pos);
                    finalNode = finalNode.preNode;
                }
            }
            result.Reverse();
            return result;
        }

        static bool findDest(Node currentNode, List<Node> openList,
                             BattleGrid grid, GridCell to, out Node finalNode,
                          Func<GridCell, GridCell, float> getDistance, Func<GridCell, List<GridCell>> getNeighbors, Battle battle, bool GetPathWithoutObstacles)
        {
            if (currentNode == null)
            {
                finalNode = null;
                return false;
            }
            else if (currentNode.pos == to)
            {
                finalNode = currentNode;
                return true;
            }
            currentNode.open = false;
            openList.Add(currentNode);

            foreach (GridCell cell in getNeighbors(currentNode.pos))
            {
                GameCharacter ai = cell.shopItem as GameCharacter;
                if (openList.Count > 0)
                {
                    ai = openList[0].pos.shopItem as GameCharacter;
                }
                
                bool dead = false;
                //bool nextMoveOfCharacter = battle.CellIsNextOtherCharacterMovement(ai, cell);
                if (ai)
                {
                    dead = ai.State == GameCharacter.AIState.Dead;
                }

                if (!GetPathWithoutObstacles)
                {
                    if (grid.GridCells.Contains(cell) &&  (!cell.shopItem || dead))
                    {
                        findTemp(openList, currentNode, cell, to, getDistance);
                    }
                }
                else
                {
                    if (grid.GridCells.Contains(cell))
                    {
                        findTemp(openList, currentNode, cell, to, getDistance);
                    }
                }
            }
            var next = openList.FindAll(obj => obj.open).Min();
            return findDest(next, openList, grid, to, out finalNode, getDistance, getNeighbors, battle, GetPathWithoutObstacles);
        }

        static void findTemp(List<Node> openList, Node currentNode, GridCell from, GridCell to, Func<GridCell, GridCell, float> getDistance)
        {

            Node temp = openList.Find(obj => obj.pos == from);
            if (temp == null)
            {
                temp = new Node(currentNode, from, getDistance(from, to), currentNode.gScore + 1);
                openList.Add(temp);
            }
            else if (temp.open && temp.gScore > currentNode.gScore + 1)
            {
                temp.gScore = currentNode.gScore + 1;
                temp.fScore = temp.hScore + temp.gScore;
                temp.preNode = currentNode;
            }
        }

        class Node : IComparable
        {
            public Node preNode;
            public GridCell pos;
            public float fScore;
            public float hScore;
            public float gScore;
            public bool open = true;

            public Node(Node prePos, GridCell pos, float hScore, float gScore)
            {
                preNode = prePos;
                this.pos = pos;
                this.hScore = hScore;
                this.gScore = gScore;
                fScore = hScore + gScore;
            }

            public int CompareTo(object obj)
            {
                Node temp = obj as Node;

                if (temp == null) return 1;

                if (Mathf.Abs(fScore - temp.fScore) < 0.01f)
                {
                    if(fScore > temp.fScore)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }

                if (Mathf.Abs(hScore - temp.hScore) < 0.01f)
                {
                    if(hScore > temp.hScore)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                return 0;
            }
        }
    }
}