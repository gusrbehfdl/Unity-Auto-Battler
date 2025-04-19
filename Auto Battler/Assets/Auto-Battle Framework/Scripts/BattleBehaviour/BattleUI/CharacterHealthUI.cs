using AutoBattleFramework.BattleBehaviour;
using AutoBattleFramework.BattleBehaviour.GameActors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AutoBattleFramework.BattleUI
{
    /// <summary>
    /// Updates position and the fill amount of a character health bar.
    /// </summary>
    public class CharacterHealthUI : MonoBehaviour
    {
        /// <summary>
        /// Reference to the character.
        /// </summary>
        GameCharacter character;

        [Tooltip("Reference to the fill of the bar.")]
        public Image HealthBar;

        /// <summary>
        /// Rect of the UI bar
        /// </summary>
        RectTransform rect;

        // Start is called before the first frame update
        void Start()
        {

        }

        /// <summary>
        /// Returns the character who owns this bar.
        /// </summary>
        /// <returns></returns>
        public GameCharacter GetCharacter()
        {
            return character;
        }

        /// <summary>
        /// Set the character reference and the transform position.
        /// </summary>
        /// <param name="character"></param>
        public void Initialize(GameCharacter character)
        {
            this.character = character;
            transform.SetParent(Battle.Instance.UIBars);
            rect = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            HealthBar.fillAmount = character.CurrentStats.Health / (float)character.InitialStats.Health;

            Vector2 coordinates = Camera.main.WorldToScreenPoint(character.UIBarPosition.position);
            rect.position = coordinates;
            rect.rotation = Quaternion.identity;
        }
    }
}