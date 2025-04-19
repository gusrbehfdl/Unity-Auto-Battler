using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using AutoBattleFramework.BattleUI;
using UnityEngine.EventSystems;
using AutoBattleFramework.BattleBehaviour;
using AutoBattleFramework.Utility;
using AutoBattleFramework.BattleBehaviour.GameActors;
using Unity.Netcode;
using AutoBattleFramework.Battlefield;
using AutoBattleFramework.Multiplayer.BattleBehaviour.Player;

namespace AutoBattleFramework.Shop.ShopGUI
{
    /// <summary>
    /// Purchase panel of a character or object.
    /// </summary>
    public class ShopItemUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Prefab")]

        /// <summary>
        /// Prefab that represents a <see cref="Stats.Trait"/>, to be instantiated inside <see cref="ItemTraits"/>. 
        /// </summary>
        public TraitStatsUI TraitPrefab;

        [Header("Shop Item (Read-Only)")]
                /// <summary>
        /// Item available for purchase.
        /// </summary>
        [ReadOnly] public ScriptableShopItem item;

        [Header("UI References")]

        /// <summary>
        /// Item name text.
        /// </summary>
        public TMPro.TextMeshProUGUI ItemName;

        /// <summary>
        /// Item purchase price text.
        /// </summary>
        public TMPro.TextMeshProUGUI ItemCost;

        /// <summary>
        /// Panel where the character's traits are displayed.
        /// </summary>
        public Image ItemTraits;

        /// <summary>
        /// Image of the item.
        /// </summary>
        public Image ItemImage;

        /// <summary>
        /// Alpha of the elements after the purchase.
        /// </summary>
        [Range(0f,1f)]
        public float alphaAfterBought = 0.25f;



        /// <summary>
        /// Item purchase information.
        /// </summary>
        ShopItemInfo info;

        /// <summary>
        /// Reference to the shop system.
        /// </summary>
        ShopManager shop;

        /// <summary>
        /// Purchase cost.
        /// </summary>
        int cost;

        /// <summary>
        /// The button is disabled because it has already been purchased once.
        /// </summary>
        bool disabled = false;

        // These variables are used to display the description if the mouse (or finger in Android) has been held over the panel for a certain time.

        /// <summary>
        /// Time spent with the mouse over the panel.
        /// </summary>
        float elapsed = 0f;

        /// <summary>
        /// Minimum time to display the description.
        /// </summary>
        float timeToShow = 1f;

        /// <summary>
        /// If the mouse is over the panel.
        /// </summary>
        bool entered = false;

        private void Update()
        {
            if (entered)
            {
                if (elapsed > timeToShow)
                {
                    ShowUI();
                }
                elapsed += Time.deltaTime;
            }
            else
            {
                elapsed = 0f;
            }
        }

        /// <summary>
        /// Places the object information in the panel.
        /// </summary>
        /// <param name="item">Item for purchase.</param>
        /// <param name="info">Purchase information of the item.</param>
        /// <param name="cost">Purchase cost oof the item.</param>
        /// <param name="shop">ShopSystem reference.</param>
        public void SetInfo(ScriptableShopItem item, ShopItemInfo info, int cost, ShopManager shop)
        {
            this.item = item;
            this.info = info;
            this.cost = cost;
            this.shop = shop;

            ItemName.SetText(item.itemName);
            ItemCost.SetText(cost.ToString());
            ItemImage.sprite = item.itemImage;

            item.ShowUIAdditional(this);
        }

        /// <summary>
        /// Returns true if the player has enough currency to buy the item.
        /// </summary>
        /// <returns></returns>
        public bool CanBeBought()
        {
            //Multiplayer
            if (NetworkManager.Singleton)
            {
                int count = 0;
                int benchCount = -1;
                if (item is ShopCharacter)
                {
                    count = Battle.Instance.TeamBenches[(int)IPlayer.instance.OwnerClientId].GetGameCharacterInBench().Count;
                    benchCount = Battle.Instance.TeamBenches[(int)IPlayer.instance.OwnerClientId].GridCells.Length;
                }
                else
                {
                    count = Battle.Instance.ItemBenches[(int)IPlayer.instance.OwnerClientId].GetShopItemInBench().Count;
                    benchCount = Battle.Instance.ItemBenches[(int)IPlayer.instance.OwnerClientId].GridCells.Length;
                }
                return shop.currency >= cost && count < benchCount;
            }
            //Single player
            else 
            {
                int count = 0;
                int benchCount = -1;
                if(item is ShopCharacter)
                {
                    count = Battle.Instance.TeamBenches[0].GetGameCharacterInBench().Count;
                    benchCount = Battle.Instance.TeamBenches[0].GridCells.Length;
                }
                else
                {
                    count = Battle.Instance.ItemBenches[0].GetShopItemInBench().Count;
                    benchCount = Battle.Instance.ItemBenches[0].GridCells.Length;
                }
                return shop.currency >= cost && count < benchCount;
            }
        }

        /// <summary>
        /// Purchase the item, deducting the currency from the total and disabling the panel.
        /// </summary>
        public void Buy()
        {
            if (CanBeBought() && !disabled)
            {
                GameActor bought = item.shopItem.Buy(item.shopItem);
                shop.currency -= cost;

                shop.shopLevelManager.GetCurrentList().OnBuy(info);
                if (shop.RemoveFromListWhenBought)
                {
                    shop.RemoveShopItemFromAllLists(info);
                }


                // Reduce the Alpha of all components

                ItemImage.color = ChangeAlpha(ItemImage.color, alphaAfterBought);
                ItemName.color = ChangeAlpha(ItemName.color, alphaAfterBought);
                ItemCost.color = ChangeAlpha(ItemCost.color, alphaAfterBought);

                foreach(TraitStatsUI trait in ItemTraits.GetComponentsInChildren<TraitStatsUI>())
                {
                    trait.TraitImage.color = ChangeAlpha(trait.TraitImage.color, alphaAfterBought);
                    trait.TraitText.color = ChangeAlpha(trait.TraitText.color, alphaAfterBought);
                }

                //

                disabled = true;

                if (bought)
                {
                    bought.AfterBought();
                }
            }
        }

        Color ChangeAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        /// <summary>
        /// When clicked on the panel, the item is purchased. If right-clicked, the description of the item is displayed.
        /// </summary>
        /// <param name="eventData">Pointer data.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                ShowUI();
            }
            else
            {
                Buy();
            }
        }

        /// <summary>
        /// Show the description UI of the item. The type of description UI that will be displayed depends on the type of <see cref="GameActor"/>, specifically the <see cref="GameActor.ShowUI"/> method.
        /// </summary>
        void ShowUI()
        {
            Battle.Instance.itemDescriptionUI.ShowUI(false);
            Battle.Instance.TeamTraitListUI[0].HideAllDescriptionsUI();
            Battle.Instance.characterStatsUI.ShowUI(false);
            GameObject ui = item.shopItem.ShowUI(true);
            SetPositionAbove(ui);
        }

        /// <summary>
        /// Move the item description so that it is displayed on the object, always staying within the boundaries of the screen.
        /// </summary>
        /// <param name="ui">GameObject of the description UI.</param>
        void SetPositionAbove(GameObject ui)
        {
            RectTransform buttonRect = GetComponent<RectTransform>();
            RectTransform uiRect = ui.GetComponent<RectTransform>();
            Vector3 pos = GetComponent<RectTransform>().position;
            //rect.position.Set(rect.position.x, rect.position.y+rect.sizeDelta.y, rect.position.z);
            float sizeDiff = buttonRect.rect.height - uiRect.rect.height;
            pos.y = buttonRect.position.y + buttonRect.rect.height/2 + uiRect.rect.height/2;
            uiRect.position = pos;

            UIUtility.KeepInsideScreen(uiRect);
        }

        /// <summary>
        /// Notification that the pointer has entered the panel.
        /// </summary>
        /// <param name="eventData">Pointer data.</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            entered = true;
        }

        /// <summary>
        /// Notification that the pointer has left the panel.
        /// </summary>
        /// <param name="eventData">Pointer data.</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            entered = false;
            elapsed = 0f;
        }

    }
}