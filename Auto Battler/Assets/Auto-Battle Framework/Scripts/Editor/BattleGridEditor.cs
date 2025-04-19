using AutoBattleFramework.Battlefield;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AutoBattleFramework.EditorScripts
{
    [CustomEditor(typeof(BattleGrid))]
    public class BattleGridEditor : UnityEditor.Editor
    {
        // The function that makes the custom editor work
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Grid shape settings", EditorStyles.boldLabel);
            // Display the enum popup in the inspector
            EditorGUILayout.PropertyField(serializedObject.FindProperty("GridShape"));

            int gridType = serializedObject.FindProperty("GridShape").enumValueIndex;


            // Create a space to separate this enum popup from other variables 
            EditorGUILayout.Space();

            // Switch statement to handle what happens for each category
            switch (gridType)
            {
                case 0:
                    serializedObject.FindProperty("GridShape").enumValueIndex = (int)BattleGrid.GridType.Squared;
                    DisplaySquared();
                    break;

                case 1:
                    serializedObject.FindProperty("GridShape").enumValueIndex = (int)BattleGrid.GridType.Hex;
                    DisplayHex();
                    break;

            }
            DisplayShape();

            //Display the number of rows dedicated to team 1.
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Team1RowNumber"));

            DisplayColors();

            //Display the cell list.
            EditorGUILayout.PropertyField(serializedObject.FindProperty("GridCells"));

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Create Grid"))
            {
                BattleGrid grid = (BattleGrid)target;
                grid.SpawnGridEditor();
                if(gridType == 0)
                {
                    grid.FindSquaredNeighbors();
                }
                else
                {
                    grid.FindHexNeighbors();
                }
                
                grid.CalculateDistances();
            }

            if (GUILayout.Button("Update colors"))
            {
                BattleGrid grid = (BattleGrid)target;
                grid.UpdateColors();
            }
        }

        /// <summary>
        /// Display the shape properties.
        /// </summary>
        void DisplayShape()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CellScale"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("separation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("GridWidth"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("GridHeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RotateCells"));
        }

        /// <summary>
        /// Display the colors properties.
        /// </summary>
        void DisplayColors()
        {
            EditorGUILayout.LabelField("Grid color settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Team1EmptyCell"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Team1OccupiedCell"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Team1Drag"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Team2EmptyCell"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Team2OccupiedCell"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Team2Drag"));
        }

        /// <summary>
        /// Display the squared grid properties.
        /// </summary>
        void DisplaySquared()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("GridSquared"));
        }

        /// <summary>
        /// Display the hexagonal grid properties.
        /// </summary>
        void DisplayHex()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("GridHex"));
        }
    }
}