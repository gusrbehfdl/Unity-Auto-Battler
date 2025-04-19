using AutoBattleFramework.Battlefield;
using AutoBattleFramework.Shop;
using AutoBattleFramework.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace AutoBattleFramework.BattleBehaviour.GameActors
{
    /// <summary>
    /// A GameActor can be purchased through the shop and placed on the Grid. For example, a character or an equipable item.
    /// </summary>
    public abstract class GameActor : NetworkBehaviour
    {
        [Header("Game Information")]
        /// <summary>
        /// Amount of <see cref="Shop.ShopManager.currency"/> for which it can be sold.
        /// </summary>
        [Tooltip("If the item can be sold.")]
        public bool CanBeSold = true;

        /// <summary>
        /// Amount of <see cref="Shop.ShopManager.currency"/> for which it can be sold.
        /// </summary>
        [Tooltip("Amount of currency for which it can be sold.")]
        public int SellFor;

        /// <summary>
        /// Purchase information.
        /// </summary>
        [Tooltip("Scriptable Shop Item information.")]
        public ScriptableShopItem info;

        [Header("Position (Read-only)")]
        /// <summary>
        /// The cell it currently occupies.
        /// </summary>
        [Tooltip("The cell it currently occupies.")]
        [ReadOnly] public GridCell CurrentGridCell;

        /// <summary>
        /// If the item is being dragged, the cell where is being dragged to.
        /// </summary>
        [Tooltip("The cell where is being dragged to.")]
        [ReadOnly] public GridCell CurrentDragCell;

        /// <summary>
        /// If the character can be moved by the player.
        /// </summary>
        [Tooltip("Can be moved by the player.")]
        [ReadOnly] public bool CanBeMoved = true;

        /// <summary>
        /// The cell previously occupied.
        /// </summary>
        [Tooltip("The cell previously occupied..")]
        [ReadOnly] public GridCell PreviousCell;

        /// <summary>
        /// The item NavMeshAgent needs to find a new path.
        /// </summary>
        [HideInInspector] public bool ForceRepath = false;

        /// <summary>
        /// Character is being dragged.
        /// </summary>
        protected bool isDragged = false;

        /// <summary>
        /// Initial position in the <see cref="States.FightState"/>. Allows to put the characters back in place at the end of <see cref="States.FightState"/>.
        /// </summary>
        private Vector3 initialPosition;


        protected NetworkObject networkObject { get { return GetComponent<NetworkObject>(); } }


        /// <summary>
        /// When the mouse is clicked, the <see cref="OnMouseDownAction"/> method is started. Used to enable dragging.
        /// </summary>
        private void OnMouseDown()
        {
            initialPosition = gameObject.transform.position;
            OnMouseDownAction();
        }

        /// <summary>
        /// Returns the <see cref="initialPosition"/>
        /// </summary>
        /// <returns>The initial position</returns>
        protected Vector3 getInitialPosition()
        {
            return initialPosition;
        }

        /// <summary>
        /// Action invoked when the mouse is held down.
        /// </summary>
        public abstract void OnMouseDownAction();

        /// <summary>
        /// Allows the sale of the item and invokes the <see cref="OnMouseUpAction"/> method.
        /// </summary>
        private void OnMouseUp()
        {
            if (NetworkManager.Singleton)
            {
                if (!IsOwner)
                    return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f, LayerMask.GetMask("BattleSurface"));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.GetComponent<SellZone>())
                {
                    Sell();
                    Battle.Instance.shopManager.shopUI.SellForText.gameObject.SetActive(false);
                    CurrentDragCell.SetDragEffect(false);
                    return;
                }
            }

            OnMouseUpAction();
        }

        /// <summary>
        /// Action invoked when the mouse is no longer pressed.
        /// </summary>
        public abstract void OnMouseUpAction();

        /// <summary>
        /// When the object is being dragged, the <see cref="OnDragObjectAction"/> method is invoked.
        /// </summary>
        public void OnMouseDrag()
        {
            if (NetworkManager.Singleton)
            {
                if (!IsOwner)
                    return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f, LayerMask.GetMask("BattleSurface"));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.GetComponent<SellZone>())
                {
                    SellAttempt();
                    CurrentDragCell.SetDragEffect(false);
                    break;
                }
                SellAttemptCancel();
            }

            OnDragObjectAction();
        }

        /// <summary>
        /// Action invoked when the object is being dragged.
        /// </summary>
        public abstract void OnDragObjectAction();

        /// <summary>
        /// When the mouse is over the object, the <see cref="OnMouseOverAction"/> is invoked.
        /// </summary>
        public void OnMouseOver()
        {
            OnMouseOverAction();
        }

        /// <summary>
        /// Action invoked when the mouse is over the object.
        /// </summary>
        public abstract void OnMouseOverAction();

        /// <summary>
        /// Displays or hide the UI associated.
        /// </summary>
        /// <param name="show">Show or hide the UI</param>
        /// <returns>The GameObject of the UI</returns>
        public abstract GameObject ShowUI(bool show);

        /// <summary>
        /// Instantiates the object when the player buys it in the store.
        /// </summary>
        /// <param name="shopItem">Item bought</param>
        /// <returns>The instantiated bought item</returns>
        public abstract GameActor Buy(GameActor shopItem);

        /// <summary>
        /// Method called after the items has been bought
        /// </summary>
        /// <param name="shopItem">Item bought</param>
        public abstract void AfterBought();

        /// <summary>
        /// Removes the item from the game when the player decides to sell it.
        /// </summary>
        public abstract void Sell();

        /// <summary>
        /// Method called when an item is dragged to the <see cref="Battlefield.SellZone"/>. Used to show a warning that the item will be sold.
        /// </summary>
        public virtual void SellAttempt()
        {
            if (!CanBeSold)
            {
                Battle.Instance.shopManager.shopUI.SellForText.gameObject.SetActive(true);
                Battle.Instance.shopManager.shopUI.SellForText.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText("Cannot be sold");
            }
            else
            {
                Battle.Instance.shopManager.shopUI.SellForText.gameObject.SetActive(true);
                Battle.Instance.shopManager.shopUI.SellForText.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText("Sell for " + SellFor);
            }
        }

        /// <summary>
        /// Method called when an item is dragged out of the <see cref="Battlefield.SellZone"/>.
        /// </summary>
        public virtual void SellAttemptCancel()
        {
            Battle.Instance.shopManager.shopUI.SellForText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Returns true if the item is being dragged.
        /// </summary>
        /// <returns>The dragging status</returns>
        public bool IsBeingDragged()
        {
            return isDragged;
        }

        /// <summary>
        /// Gets the cell currently occupied by the item and stores it in <see cref="CurrentGridCell"/>.
        /// </summary>
        public void GetCurrentCell()
        {
            RaycastHit hit;
            Vector3 position = transform.position;
            position.y += 5;
            if (Physics.Raycast(position, Vector3.down, out hit, 30, LayerMask.GetMask("GridCell"), QueryTriggerInteraction.Collide))
            {
                GridCell NowGridCell = hit.collider.GetComponent<GridCell>();
                if (NowGridCell)
                {
                    if (CurrentGridCell)
                    {
                        //In case more than 1 character are stuck in one cell, the one refered in the cell keeps the position, and the other is forced to repath
                        if (CurrentGridCell == NowGridCell)
                        {
                            // If another character is in that cell, force repath
                            if (CurrentGridCell.shopItem != null && CurrentGridCell.shopItem != this)
                            {
                                ForceRepath = true;
                            }
                            // If the cell is empty, and the character is over that cell, assign the character to the cell.
                            else if(CurrentGridCell.shopItem == null)
                            {
                                CurrentGridCell.shopItem = this;
                            }
                        }
                        //If the character enters another cell
                        else
                        {
                            //The previus cell has no character inside
                            if (CurrentGridCell.shopItem == this)
                            {
                                CurrentGridCell.shopItem = null;
                                PreviousCell = CurrentGridCell;
                            }

                            //Set the current cell as the new one
                            CurrentGridCell = NowGridCell;

                            //If other character is in the cell, that one remains in the cell
                            if (!CurrentGridCell.shopItem)
                            {
                                CurrentGridCell.shopItem = this;
                            }
                        }
                    }
                    // Set the current cell
                    else
                    {
                        CurrentGridCell = hit.collider.GetComponent<GridCell>();
                        CurrentGridCell.shopItem = this;
                    }
                }
            }
        }

        /// <summary>
        /// Get the team index. In single player always returns 0.
        /// </summary>
        /// <returns></returns>
        protected virtual int GetTeamIndex()
        {
            return 0;
        }
    }
}