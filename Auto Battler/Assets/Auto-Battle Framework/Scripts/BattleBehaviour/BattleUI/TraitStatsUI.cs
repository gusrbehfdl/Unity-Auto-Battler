using AutoBattleFramework.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AutoBattleFramework.BattleUI
{
    /// <summary>
    /// UI that displays the image and text of a <see cref="Stats.Trait"/>.
    /// </summary>
    public class TraitStatsUI : MonoBehaviour
    {
        /// <summary>
        /// Trait image.
        /// </summary>
        [Tooltip("Reference to the Trait image.")]
        public Image TraitImage;
        /// <summary>
        /// Trait name.
        /// </summary>
        [Tooltip("Reference to the Trait name text.")]
        public TMPro.TextMeshProUGUI TraitText;

        /// <summary>
        /// Set the <see cref="TraitImage"/> and <see cref="TraitText"/>.
        /// </summary>
        /// <param name="trait">Trait to display.</param>
        public void SetUI(Trait trait)
        {
            TraitImage.sprite = trait.TraitImage;
            TraitText.SetText(trait.TraitName);
        }
    }
}