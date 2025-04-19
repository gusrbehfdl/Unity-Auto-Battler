using AutoBattleFramework.BattleBehaviour;
using AutoBattleFramework.Battlefield;
using AutoBattleFramework.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AutoBattleFramework.EditorScripts
{
    [CustomEditor(typeof(ScriptableBattlePosition))]
    public class BattlePositionEditor : UnityEditor.Editor
    {

        // Variables to center the grid in the inspector.
        float separation = 5f;
        float size = 60f;
        float marginX = 30f;
        float marginY = 20f;

        //Target object.
        ScriptableBattlePosition sbp;

        // If the current selected cell is part of the enemy grid cells.
        bool enemyBoard;

        // The function that makes the custom editor work
        public override void OnInspectorGUI()
        {
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("GridCellTexture"));
            size = AutoBattleSettings.GetOrCreateSettings().BattlePositionEditorSize;

            Battle battle = FindObjectOfType<Battle>();
            BattleGrid grid = battle.grid;

            sbp = target as ScriptableBattlePosition;


            if (battle.grid.GridShape == BattleGrid.GridType.Hex)
            {
                sbp.GridCellTexture = AutoBattleSettings.GetOrCreateSettings().defaultHexTexture;
            }
            else
            {
                sbp.GridCellTexture = AutoBattleSettings.GetOrCreateSettings().defaultSquaredTexture;
            }

            if (sbp.battlePositions == null)
            {
                sbp.battlePositions = new int[grid.GridWidth * grid.GridHeight];
                for (int i = 0; i < grid.GridWidth * grid.GridHeight; i++)
                {
                    sbp.battlePositions[i] = 0;
                }
            }
            if (sbp.battlePositions.Length < grid.GridWidth * grid.GridHeight)
            {
                var characterGroups = sbp.battlePositions.ToList();
                int le = characterGroups.Count;
                int total = grid.GridWidth * grid.GridHeight;
                int fin = total - le;
                for (int i = 0; i < fin; i++)
                {
                    characterGroups.Add(0);
                }
                sbp.battlePositions = characterGroups.ToArray();
            }

            Color contentColor = GUI.contentColor;
            if (grid)
            {
                string[] buttonNames = grid.GridCells.Select(x => x.name).ToArray();

                // Create a space to separate this enum popup from other variables 
                EditorGUILayout.Space();

                // Switch statement to handle what happens for each category
                switch (grid.GridShape)
                {
                    case BattleGrid.GridType.Squared:
                        DisplaySquared(grid);
                        break;

                    case BattleGrid.GridType.Hex:
                        DisplayHex(grid);
                        break;

                }
            }
            GUI.contentColor = contentColor;
            DisplayPopup(enemyBoard);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("characterGroups"));
            serializedObject.ApplyModifiedProperties();

        }

        void DisplayHex(BattleGrid grid)
        {
            int count = 0;
            Texture texture = sbp.GridCellTexture;

            GUILayout.Box("", GUILayout.Width(size * (grid.GridWidth + 1) + separation * (grid.GridWidth + 1)), GUILayout.Height(size * grid.GridHeight + separation * grid.GridHeight));

            for (int i = 0; i < grid.GridWidth; i++)
            {
                for (int j = 0; j < grid.GridHeight; j++)
                {
                    int ii = i;
                    int jj = j;
                    //To draw the grid to match the scene (the player below and enemies above)
                    float offset = 0;
                    ii = i;
                    jj = grid.GridHeight - j;


                    if (jj % 2 != 0)
                    {
                        offset = (size + separation) / 2;
                    }


                    float xPos = ii * size + offset;
                    xPos += separation * ii;
                    xPos += marginX;

                    float yPos = jj * size;
                    yPos += separation * jj - size * 0.125f * jj;
                    yPos += marginY;

                    GUI.Box(new Rect(xPos, yPos, size, size), "");

                    GUI.contentColor = Color.white;

                    if (sbp.GetCharacterGroup(count) != null)
                    {
                        GUI.contentColor = Color.white;
                        Texture groupTexture = sbp.GetCharacterGroup(count).GetGroupTexture();
                        if (groupTexture)
                        {
                            GUI.Box(new Rect(xPos, yPos, size, size), groupTexture, GUIStyle.none);
                        }
                    }

                    if (j < grid.Team1RowNumber)
                    {
                        if (count != sbp.selected)
                        {
                            if (sbp.GetCharacterGroupList(count) == null)
                            {
                                GUI.contentColor = grid.Team1EmptyCell;
                            }
                            else
                            {
                                if (sbp.GetCharacterGroupList(count).Count == 0)
                                {
                                    GUI.contentColor = grid.Team1EmptyCell;
                                }
                                else
                                {
                                    GUI.contentColor = grid.Team1OccupiedCell;
                                }

                            }
                        }
                        else
                        {
                            GUI.contentColor = Color.white;
                            enemyBoard = false;
                        }
                    }
                    else
                    {
                        if (count != sbp.selected)
                        {
                            if (sbp.GetCharacterGroupList(count) == null)
                            {
                                GUI.contentColor = grid.Team2EmptyCell;
                            }
                            else
                            {
                                if (sbp.GetCharacterGroupList(count).Count == 0)
                                {
                                    GUI.contentColor = grid.Team2EmptyCell;
                                }
                                else
                                {
                                    GUI.contentColor = grid.Team2OccupiedCell;
                                }
                            }
                        }
                        else
                        {
                            GUI.contentColor = Color.white;
                            enemyBoard = true;
                        }
                    }

                    if (GUI.Button(new Rect(xPos, yPos, size, size), texture, GUIStyle.none))
                    {
                        sbp.selected = count;
                        Debug.Log(count);
                    }


                    count++;
                }
            }
        }

        void DisplaySquared(BattleGrid grid)
        {
            int count = 0;
            Texture texture = sbp.GridCellTexture;

            GUILayout.Box("", GUILayout.Width(size * (grid.GridWidth + 1) + separation * (grid.GridWidth + 1)), GUILayout.Height(marginY*2.5f + size * grid.GridHeight + separation * grid.GridHeight));

            for (int i = 0; i < grid.GridWidth; i++)
            {
                for (int j = 0; j < grid.GridHeight; j++)
                {
                    int ii = i;
                    int jj = j;
                    //To draw the grid to match the scene (the player below and enemies above)
                    float offset = 0;
                    ii = i;
                    jj = grid.GridHeight - j;
                    
                    offset = (size + separation) / 2;


                    float xPos = ii * size + offset;
                    xPos += separation * ii;
                    xPos += marginX;

                    float yPos = jj * size;
                    yPos += separation * jj;
                    yPos += marginY;

                    GUI.Box(new Rect(xPos, yPos, size, size), "");

                    GUI.contentColor = Color.white;

                    if (sbp.GetCharacterGroup(count) != null)
                    {
                        GUI.contentColor = Color.white;
                        Texture groupTexture = sbp.GetCharacterGroup(count).GetGroupTexture();
                        if (groupTexture)
                        {
                            GUI.Box(new Rect(xPos, yPos, size, size), groupTexture, GUIStyle.none);
                        }
                    }

                    if (j < grid.Team1RowNumber)
                    {
                        if (count != sbp.selected)
                        {
                            if (sbp.GetCharacterGroupList(count) == null)
                            {
                                GUI.contentColor = grid.Team1EmptyCell;
                            }
                            else
                            {
                                if (sbp.GetCharacterGroupList(count).Count == 0)
                                {
                                    GUI.contentColor = grid.Team1EmptyCell;
                                }
                                else
                                {
                                    GUI.contentColor = grid.Team1OccupiedCell;
                                }

                            }
                        }
                        else
                        {
                            GUI.contentColor = Color.white;
                            enemyBoard = false;
                        }
                    }
                    else
                    {
                        if (count != sbp.selected)
                        {
                            if (sbp.GetCharacterGroupList(count) == null)
                            {
                                GUI.contentColor = grid.Team2EmptyCell;
                            }
                            else
                            {
                                if (sbp.GetCharacterGroupList(count).Count == 0)
                                {
                                    GUI.contentColor = grid.Team2EmptyCell;
                                }
                                else
                                {
                                    GUI.contentColor = grid.Team2OccupiedCell;
                                }
                            }
                        }
                        else
                        {
                            GUI.contentColor = Color.white;
                            enemyBoard = true;
                        }
                    }

                    if (GUI.Button(new Rect(xPos, yPos, size, size), texture, GUIStyle.none))
                    {
                        sbp.selected = count;
                        Debug.Log(count);
                    }


                    count++;
                }
            }
        }

        void DisplayPopup(bool enemyBoard)
        {
            if (enemyBoard)
            {
                List<string> itemNames = new List<string>();
                for (int i = 0; i < sbp.characterGroups.Count; i++)
                {
                    itemNames.Add("Group " + i);
                }

                //Add none option
                itemNames.Insert(0, "None");
                string[] itemNamesArray = itemNames.ToArray();

                int selection = sbp.battlePositions[sbp.selected];

                selection = EditorGUILayout.Popup(selection, itemNamesArray);

                sbp.battlePositions[sbp.selected] = selection;



            }
        }
    }
}