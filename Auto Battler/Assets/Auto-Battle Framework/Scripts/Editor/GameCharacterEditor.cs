using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AutoBattleFramework.EditorScripts
{
    /// <summary>
    /// Custom inspector for Game Character.Keep the copy the original stats to current and initial stats when changed.
    /// </summary>
    [CustomEditor(typeof(GameCharacter))]
    public class GameCharacterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                GameCharacter character = (GameCharacter)target;
                character.InitialStats = character.OriginalStats.Copy();
                character.CurrentStats = character.OriginalStats.Copy();
            }
        }
    }
}
