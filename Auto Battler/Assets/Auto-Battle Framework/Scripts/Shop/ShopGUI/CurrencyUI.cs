using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Shop.ShopGUI
{
    /// <summary>
    /// Updates the text with the amount of currency the player has.
    /// </summary>
    public class CurrencyUI : MonoBehaviour
    {
        /// <summary>
        /// Currency text
        /// </summary>
        TMPro.TextMeshProUGUI timeText;

        /// <summary>
        /// Reference to the shop system
        /// </summary>
        ShopManager shop;


        // Start is called before the first frame update
        void Start()
        {
            shop = FindObjectOfType<ShopManager>();
            timeText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            timeText.SetText(shop.currency.ToString());
        }
    }
}