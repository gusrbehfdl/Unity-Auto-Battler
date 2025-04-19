using AutoBattleFramework.Battlefield;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AutoBattleFramework.EditorScripts
{
    [CustomEditor(typeof(Bench))]
    public class BenchEditor : UnityEditor.Editor
    {
        // The function that makes the custom editor work
        public override void OnInspectorGUI()
        {
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

            DisplayColors();

            serializedObject.ApplyModifiedProperties();

            //Creates a button to create the grid based on the properties.
            if (GUILayout.Button("Create Grid"))
            {
                BattleGrid grid = (BattleGrid)target;
                grid.Team1RowNumber = int.MaxValue;
                grid.SpawnGridEditor();
            }

            if (GUILayout.Button("Update colors"))
            {
                BattleGrid grid = (BattleGrid)target;
                grid.UpdateColors();
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("GridCells"));
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
        /// Display the color properties.
        /// </summary>
        void DisplayColors()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Team1EmptyCell"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Team1OccupiedCell"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Team1Drag"));
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