using AutoBattleFramework.BattleBehaviour;
using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.BattleUI;
using AutoBattleFramework.Multiplayer.BattleBehaviour.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AutoBattleFramework.Shop.ShopGUI
{
    /// <summary>
    /// When the cursor is over the <see cref="BattleUI.CharacterStatsUI.SpecialImage"/>, show the panel <see cref="BattleUI.CharacterStatsUI.SpecialPanelImage"/>, which describes the effect.
    /// </summary>
    public class EquippedItemDescriptionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public ShopGameItem item;

        /// <summary>
        /// Show the item description panel.
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            Battle.Instance.characterStatsUI.SetOverPanel(true);
            GameItem gItem = item.shopItem as GameItem;
            Battle.Instance.itemDescriptionUI.SetUI(gameObject, gItem.itemModificator, item.itemImage, item.itemName, item.itemDescription);
            Vector3 position = transform.position;

            Vector2 itemBounds = GetBoundsOfUI(GetComponent<RectTransform>());
            Vector2 panelBounds = GetBoundsOfUI(Battle.Instance.itemDescriptionUI.GetComponent<RectTransform>());

            //position.x += Battle.Instance.itemDescriptionUI.GetComponent<RectTransform>().rect.width*0.5f + GetComponent<RectTransform>().rect.width * 0.5f;
            //position.y += Battle.Instance.itemDescriptionUI.GetComponent<RectTransform>().rect.height * 0.5f + GetComponent<RectTransform>().rect.height * 0.5f;

            position.x += panelBounds.x * 0.5f + (itemBounds.x * 0.5f);
            position.y += panelBounds.y * 0.5f + (itemBounds.y * 0.5f);
            Battle.Instance.itemDescriptionUI.ShowUI(true,position);
        }

        /// <summary>
        /// Get the bounds of an UI element
        /// </summary>
        /// <param name="UI">UI element</param>
        /// <returns>Bound of UI element.</returns>
        Vector2 GetBoundsOfUI(RectTransform UI)
        {
            Vector2 size = new Vector2(UI.rect.width,UI.rect.height);
            CanvasScaler scaler = GetComponentInParent<CanvasScaler>();
            size.x = size.x * (Screen.width / scaler.referenceResolution.x);
            size.y = size.y * (Screen.height / scaler.referenceResolution.y);

            return size;
        }

        /// <summary>
        /// Hide the item description panel.
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            Battle.Instance.itemDescriptionUI.ShowUI(false);
        }

        /// <summary>
        /// On right-click, unequip de item.
        /// </summary>
        /// <param name="eventData">Pointer event data</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            GameCharacter character = Battle.Instance.characterStatsUI.character;
            if (IPlayer.instance)
            {
                if(!(Battle.Instance.teams[(int)IPlayer.instance.OwnerClientId].team.Contains(character) || Battle.Instance.TeamBenches[(int)IPlayer.instance.OwnerClientId].GetGameCharacterInBench().Contains(character)))
                {
                    return;
                }
            }
            else if(!(Battle.Instance.teams[0].team.Contains(character) || Battle.Instance.TeamBenches[0].GetGameCharacterInBench().Contains(character)))
            {
                return;                
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                int itemModificatorIndex = transform.GetSiblingIndex();
                character.UnequipItemModificator(itemModificatorIndex);
                Battle.Instance.itemDescriptionUI.ShowUI(false);
                character.ShowUI(true);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
