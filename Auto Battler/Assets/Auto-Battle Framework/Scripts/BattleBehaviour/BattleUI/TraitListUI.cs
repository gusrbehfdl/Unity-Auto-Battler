using AutoBattleFramework.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AutoBattleFramework.BattleBehaviour;

namespace AutoBattleFramework.BattleUI
{
    /// <summary>
    /// List of active features. It colors the traits according to their level.
    /// </summary>
    public class TraitListUI : MonoBehaviour
    {
        /// <summary>
        /// Color of a non activated <see cref="Stats.Trait"/>.
        /// </summary>
        [Tooltip("Color of a non activated Trait.")]
        public Color NoActivatedColor;

        /// <summary>
        /// Color of the background of an activated trait.
        /// </summary>
        [Tooltip("Color of the background of an activated trait.")]
        public Color ActivatedColorBackground;

        /// <summary>
        /// Color of the text of an activated trait.
        /// </summary>
        [Tooltip("Color of the text of an activated trait.")]
        public Color ActivatedColorText;

        /// <summary>
        /// Color of the background of a non activated trait.
        /// </summary>
        [Tooltip("Color of the background of a non activated trait.")]
        public Color DeactivatedColorBackground;

        /// <summary>
        /// Color of the text of a non activated trait.
        /// </summary>
        [Tooltip("Color of the text of a non activated trait.")]
        public Color DeactivatedColorText;

        /// <summary>
        /// Prefab of the Trait UI.
        /// </summary>
        [Tooltip("Prefab of the Trait UI.")]
        public TraitUI TraitUIPrefab;

        /// <summary>
        /// List of current Traits UI:
        /// </summary>
        [HideInInspector]
        public List<TraitUI> traitsUI;

        /// <summary>
        /// How to order the traits in the list.<br />
        /// Name - Sort traits by name.<br />
        /// Number of Traits - Sort traits by number of characters activating the trait.<br />
        /// Option Index - Sort traits by index of the activated option, and then by number of traits.
        /// </summary>
        public enum OrderBy
        {
            [Tooltip("Sort traits by name.")]
            Name,
            [Tooltip("Sort traits by number of characters activating the trait.")]
            NumberOfTraits,
            [Tooltip("Sort traits by index of the activated option, and then by number of traits.")]
            OptionIndex
        }

        /// <summary>
        /// The way the traits are orderer in the list.
        /// </summary>
        [Tooltip("How to order the list of active Traits.")]
        public OrderBy order;

        // Start is called before the first frame update
        void Start()
        {

        }

        /// <summary>
        /// Starts the Traits ui.
        /// </summary>
        /// <param name="traits">List of traits.</param>
        void CreateTraitList(List<Trait> traits)
        {
            for (int i = 0; i < traits.Count; i++)
            {
                TraitUI traitUI = Instantiate(TraitUIPrefab, transform);
                traitsUI.Add(traitUI);
            }
        }

        /// <summary>
        /// Update hte trait UI list.
        /// </summary>
        /// <param name="traits"></param>
        public void UpdateList(List<Trait> traits)
        {
            if (traitsUI.Count == 0)
            {
                CreateTraitList(traits);
            }
            for (int i = 0; i < traits.Count; i++)
            {
                traitsUI[i].SetTrait(traits[i], this);
            }
            Order();
        }

        /// <summary>
        /// Hide all <see cref="TraitUI.DescriptionUI"/>.
        /// </summary>
        public void HideAllDescriptionsUI()
        {
            foreach (TraitUI ui in traitsUI)
            {
                ui.ShowDescriptionUI(false);
            }
        }

        /// <summary>
        /// Order the Trait list by <see cref="OrderBy"/>.
        /// </summary>
        void Order()
        {
            switch (order)
            {
                case TraitListUI.OrderBy.Name:
                    List<TraitUI> nameOrdered = traitsUI.OrderBy(x => x.GetTrait().TraitName).ToList();
                    for (int i = 0; i < nameOrdered.Count; i++)
                    {
                        nameOrdered[i].transform.SetSiblingIndex(i + 1);
                    }
                    break;

                case TraitListUI.OrderBy.NumberOfTraits:
                    List<TraitUI> ordered = traitsUI.OrderByDescending(x => x.GetTrait().TraitNumber).ToList();
                    for(int i=0;i<ordered.Count;i++)
                    {
                        ordered[i].transform.SetSiblingIndex(i + 1);
                    }
                    break;
                    
                case OrderBy.OptionIndex:
                    List<TraitUI> optionOrder = traitsUI.OrderByDescending(x => x.GetTrait().TraitOptions.IndexOf(x.GetTrait().ActivatedOption)).ThenByDescending(x => x.GetTrait().TraitNumber).ToList();
                    for (int i = 0; i < optionOrder.Count; i++)
                    {
                        optionOrder[i].transform.SetSiblingIndex(i + 1);
                    }
                    break;
                
            }
        }
    }
}