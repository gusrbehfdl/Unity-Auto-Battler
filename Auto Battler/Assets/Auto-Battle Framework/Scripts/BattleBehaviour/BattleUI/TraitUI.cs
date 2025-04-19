using AutoBattleFramework.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AutoBattleFramework.BattleUI
{
    /// <summary>
    /// Handles the UI that controls the representation of the <see cref="Stats.Trait.ActivatedOption"/>, including the colors and the description of the trait.
    /// </summary>
    public class TraitUI : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// Reference to the list where it will be displayed.
        /// </summary>
        TraitListUI traitListUI;

        [Header("Images")]

        /// <summary>
        /// Image of the trait background
        /// </summary>
        [Tooltip("Reference to the background image.")]
        public Image TraitImageBackground;

        /// <summary>
        /// Image where the <see cref="Stats.Trait.TraitImage"/> will be displayed.
        /// </summary>
        [Tooltip("Reference to the trait image.")]
        public Image TraitImage;


        /// <summary>
        /// List of Images where each <see cref="Stats.TraitOption.NumberOfTraits"/> of <see cref="Stats.Trait.TraitOptions"/> will be displayed.
        /// </summary>
        [Tooltip("Reference to the list of Images where each Number of traits of each Trait option will be displayed.")]
        public List<Image> TraitNumbers;

        [Header("Texts")]
        /// <summary>
        /// Text where the <see cref="Stats.Trait.TraitName"/> will be displayed.
        /// </summary>
        [Tooltip("Reference to the Trait name text.")]
        public TMPro.TextMeshProUGUI TraitName;


        /// <summary>
        /// Text where the <see cref="Stats.Trait.TraitNumber"/> will be displayed.
        /// </summary>
        [Tooltip("Reference to the Trait number text.")]
        public TMPro.TextMeshProUGUI TraitNumber;


        [Header("Description panel.")]
        /// <summary>
        /// Panel that describes the <see cref="Stats.Trait"/>.
        /// </summary>
        [Tooltip("Reference to the panel that describes the Trait.")]
        public TraitDescriptionUI DescriptionUI;

        /// <summary>
        /// Trait reference
        /// </summary>
        Trait trait;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            HideDescriptionIfClicked();
        }

        /// <summary>
        /// Returns the displayed trait.
        /// </summary>
        /// <returns>Dìsplayed trait.</returns>
        public Trait GetTrait()
        {
            return trait;
        }

        /// <summary>
        /// Set the trait UI, including texts, descriptions and colors.
        /// </summary>
        /// <param name="trait"></param>
        /// <param name="traitListUI"></param>
        public void SetTrait(Trait trait, TraitListUI traitListUI)
        {
            this.trait = trait;
            this.traitListUI = traitListUI;
            if (trait.ActivatedOption != null)
            {
                TraitName.SetText(trait.TraitName);
                TraitName.color = Color.white;
                TraitImageBackground.color = trait.ActivatedOption.TraitOptionColor;
                TraitImage.sprite = trait.TraitImage;

                SetTraitNumbers();
                gameObject.SetActive(true);
            }
            else
            {
                if (trait.TraitNumber>0)
                {
                    TraitName.SetText(trait.TraitName);
                    if (trait.ActivatedOption != null)
                    {
                        TraitImageBackground.color = trait.ActivatedOption.TraitOptionColor;
                    }
                    else
                    {
                        TraitImageBackground.color = traitListUI.NoActivatedColor;
                    }
                    
                    TraitName.color = TraitImageBackground.color;
                    TraitImage.sprite = trait.TraitImage;

                    SetTraitNumbers();
                    gameObject.SetActive(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            DescriptionUI.SetTraitDescription(trait);
        }

        /// <summary>
        /// Displays or hide the <see cref="BattleUI.TraitDescriptionUI"/> of the trait.
        /// </summary>
        /// <param name="show">Show or hide the description panel.</param>
        public void ShowDescriptionUI(bool show)
        {
            DescriptionUI.gameObject.SetActive(show);
        }

        /// <summary>
        /// Set the possible <see cref="Stats.TraitOption.NumberOfTraits"/> of all <see cref="Stats.Trait.TraitOptions"/>.
        /// </summary>
        void SetTraitNumbers()
        {
            int optionIndex = trait.TraitOptions.IndexOf(trait.ActivatedOption);

            for (int i = 0; i < TraitNumbers.Count; i++)
            {
                TraitNumbers[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().faceColor = traitListUI.DeactivatedColorText;
                TraitNumbers[i].color = traitListUI.DeactivatedColorBackground;
                if (i < trait.TraitOptions.Count)
                {
                    TraitNumbers[i].enabled = true;
                    TraitNumbers[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText(trait.TraitOptions[i].NumberOfTraits.ToString());
                    if (optionIndex == i)
                    {
                        TraitNumbers[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().faceColor = traitListUI.ActivatedColorText;
                        TraitNumbers[i].color = traitListUI.ActivatedColorBackground;
                    }
                }
                else
                {
                    TraitNumbers[i].gameObject.SetActive(false);
                }
            }
            TraitNumber.SetText(trait.TraitNumber.ToString());
        }

        /// <summary>
        /// If clicked inside, show the <see cref="DescriptionUI"/>.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            DescriptionUI.gameObject.SetActive(true);
        }

        /// <summary>
        /// If clicked outside the panel, hide the <see cref="DescriptionUI"/>.
        /// </summary>
        private void HideDescriptionIfClicked()
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                DescriptionUI.gameObject.SetActive(false);
            }
        }
    }
}