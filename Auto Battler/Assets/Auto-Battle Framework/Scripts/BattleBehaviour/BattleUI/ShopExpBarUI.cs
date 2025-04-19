using AutoBattleFramework.Shop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AutoBattleFramework.BattleUI
{
    /// <summary>
    /// Updates the shop bar text and fills the exp bar.
    /// </summary>
    public class ShopExpBarUI : MonoBehaviour
    {
        // Reference to the shop system.
        ShopManager shopSystem;

        /// <summary>
        /// Text that displays the current and max exp of the <see cref="Shop.ShopManager.shopLevelManager"/>.
        /// </summary>
        public TMPro.TextMeshProUGUI ExpText;

        /// <summary>
        /// Exp bar to be filled.
        /// </summary>
        public Image expBarFill;

        // Start is called before the first frame update
        void Start()
        {
            shopSystem = FindObjectOfType<ShopManager>();
        }

        // Update is called once per frame
        void Update()
        {
            int nextLevelIndex = shopSystem.shopLevelManager.CurrentLevel + 1;
            if(nextLevelIndex < shopSystem.shopLevelManager.shopLevels.Count)
            {
                ShopLevel nextLevel = shopSystem.shopLevelManager.shopLevels[nextLevelIndex];
                expBarFill.fillAmount = (float)shopSystem.shopLevelManager.CurrentExp / (float)nextLevel.ExpRequired;
                ExpText.SetText(shopSystem.shopLevelManager.CurrentExp + "/" + nextLevel.ExpRequired);
            }
            else
            {
                ExpText.SetText("MAX");
                expBarFill.fillAmount = 1f;
            }
        }
    }

}