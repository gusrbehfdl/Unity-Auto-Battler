using AutoBattleFramework.BattleUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AutoBattleFramework.Shop.ShopGUI
{
    /// <summary>
    /// When the cursor is over the <see cref="BattleUI.CharacterStatsUI.SpecialImage"/>, show the panel <see cref="BattleUI.CharacterStatsUI.SpecialPanelImage"/>, which describes the effect.
    /// </summary>
    public class SpecialAttackDescriptionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        CharacterStatsUI UI;
        public void OnPointerEnter(PointerEventData eventData)
        {
            UI.SpecialPanelImage.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UI.SpecialPanelImage.gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            UI = GetComponentInParent<CharacterStatsUI>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}