using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AutoBattleFramework.Stats;

namespace AutoBattleFramework.BattleUI
{
    /// <summary>
    /// Panel describing the effects of a trait.
    /// </summary>
    public class TraitDescriptionUI : MonoBehaviour
    {
        /// <summary>
        /// Text displaying the trait name.
        /// </summary>
        public TMPro.TextMeshProUGUI TraitName;

        /// <summary>
        /// Text displaying the trait description.
        /// </summary>
        public TMPro.TextMeshProUGUI TraitDescription;

        /// <summary>
        /// Display the trait image.
        /// </summary>
        public Image TraitImage;

        /// <summary>
        /// Trait reference.
        /// </summary>
        Trait trait;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Sets the description of the trait
        /// </summary>
        /// <param name="trait">Trait to be described.</param>
        public void SetTraitDescription(Trait trait)
        {
            this.trait = trait;

            TraitImage.sprite = trait.TraitImage;
            TraitName.SetText(trait.TraitName);

            string richText = GetDescriptionRichText() +'\n' + '\n';
            richText += GetTraitOptionsRichText();

            TraitDescription.SetText(richText);
        }

        /// <summary>
        /// If the option is the <see cref="Trait.ActivatedOption"/>, set to bold.
        /// </summary>
        /// <returns></returns>
        string GetDescriptionRichText()
        {
            if (trait.ActivatedOption != null)
            {
                return "<b>" + trait.TraitDescription + "</b>";
            }
            else
            {
                return trait.TraitDescription;
            }
        }


        /// <summary>
        /// Set the list of options. The activated one goes in bold.
        /// </summary>
        /// <returns>String describing the trait options.</returns>
        string GetTraitOptionsRichText()
        {
            string rich = string.Empty;
            foreach(TraitOption option in trait.TraitOptions)
            {
                if(option == trait.ActivatedOption)
                {
                    rich += "<b>(" + option.NumberOfTraits +") " + option.OptionDescription + "</b>";
                }
                else
                {
                    rich += "(" + option.NumberOfTraits +") " + option.OptionDescription;
                }
                rich += '\n';
            }
            return rich;
        }
    }

}