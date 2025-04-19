using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AutoBattleFramework.BattleUI
{
    /// <summary>
    /// Updates the fill amount of a character energy bar.
    /// </summary>
    public class CharacterEnergyUI : MonoBehaviour
    {
        GameCharacter character;

        /// <summary>
        /// Image of the energy bar that needs to be filled.
        /// </summary>
        [Tooltip("Reference to the fill of the bar.")]
        public Image EnergyBar;

        // Start is called before the first frame update
        void Start()
        {

        }

        /// <summary>
        /// Set the character reference.
        /// </summary>
        /// <param name="character">Character reference</param>
        public void Initialize(GameCharacter character)
        {
            this.character = character;
        }

        // Update is called once per frame
        void Update()
        {
            EnergyBar.fillAmount = character.CurrentStats.Energy / (float)character.InitialStats.Energy;
        }
    }
}