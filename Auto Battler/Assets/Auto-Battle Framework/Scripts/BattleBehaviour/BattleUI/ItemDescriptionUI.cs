using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutoBattleFramework.Stats;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using AutoBattleFramework.Utility;
using AutoBattleFramework.BattleBehaviour.GameActors;

namespace AutoBattleFramework.BattleUI
{
    /// <summary>
    /// Allows the visualization of an item <see cref="BattleBehaviour.GameActors.GameItem.itemModificator"/>.
    /// </summary>
    public class ItemDescriptionUI : MonoBehaviour
    {
        /// <summary>
        /// GameItem reference.
        /// </summary>
        [HideInInspector]
        public GameObject item;

        /// <summary>
        /// Set to true if you want the panel to be displayed on the selected item.
        /// </summary>
        [Tooltip("The panel is displayed over the selected item.")]
        public bool MoveToItemPosition = true;

        /// <summary>
        /// Item image reference.
        /// </summary>
        [Tooltip("Reference to the item image.")]
        public Image itemImage;

        /// <summary>
        /// Item name text reference.
        /// </summary>
        [Tooltip("Item name text reference.")]
        public TMPro.TextMeshProUGUI itemName;

        /// <summary>
        /// Item description text reference.
        /// </summary>
        [Tooltip("Item description text reference.")]
        public TMPro.TextMeshProUGUI itemDescription;


        private void Start()
        {

        }

        private void Update()
        {
            HideIfClicked();
        }



        /// <summary>
        /// Set the itms stats, image and texts in the UI:
        /// </summary>
        /// <param name="item">GameItem to show.</param>
        /// <param name="statsModificator">Item modificator.</param>
        /// <param name="sprite">Item image.</param>
        /// <param name="name">Name of the item.</param>
        public void SetUI(GameObject item, StatsModificator statsModificator, Sprite sprite, string name, string description)
        {
            this.item = item;
            itemImage.sprite = sprite;
            itemName.SetText(name);
            itemDescription.SetText(description);
        }

        /// <summary>
        /// If true, show the stats of the item.
        /// </summary>
        /// <param name="show">Show the stats of the characters. </param>
        public void ShowUI(bool show)
        {
            if (!show)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(gameObject);

                if (MoveToItemPosition)
                {
                    RectTransform rect = GetComponent<RectTransform>();
                    Vector2 coordinates = Camera.main.WorldToScreenPoint(item.transform.position);
                    rect.position = coordinates;
                    rect.rotation = Quaternion.identity;
                    UIUtility.KeepInsideScreen(rect);
                }
            }
        }

        /// <summary>
        /// If true, show the stats of the item.
        /// </summary>
        /// <param name="show">Show the stats of the characters.</param>
        /// <param name="fixedPosition">The panel is moved to this position.</param>
        public void ShowUI(bool show, Vector3 fixedPosition)
        {
            if (!show)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(gameObject);

                if (MoveToItemPosition)
                {
                    RectTransform rect = GetComponent<RectTransform>();
                    rect.position = fixedPosition;
                    rect.rotation = Quaternion.identity;
                    UIUtility.KeepInsideScreen(rect);
                }
            }
        }


        /// <summary>
        /// If clicked outside the panel, hide it.
        /// </summary>
        private void HideIfClicked()
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                gameObject.SetActive(false);
            }

        }
    }
}