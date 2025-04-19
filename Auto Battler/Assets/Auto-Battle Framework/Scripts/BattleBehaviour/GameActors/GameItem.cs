using AutoBattleFramework.Battlefield;
using AutoBattleFramework.Multiplayer.BattleBehaviour.Player;
using AutoBattleFramework.Stats;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace AutoBattleFramework.BattleBehaviour.GameActors
{
    /// <summary>
    /// Game item that can be attached to <see cref="GameCharacter"/> when dragged over it.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class GameItem : GameActor
    {
        /// <summary>
        /// The stats modifiers that will be applied when equipping the character with this item.
        /// </summary>
        public ItemModificator itemModificator;


        // Start is called before the first frame update
        protected virtual void Start()
        {
            GetCurrentCell();
            for (int i = 0; i < itemModificator.attackEffects.Count; i++)
            {
                itemModificator.attackEffects[i] = Instantiate(itemModificator.attackEffects[i]);
            }
            for (int i = 0; i < itemModificator.onHitEffects.Count; i++)
            {
                itemModificator.onHitEffects[i] = Instantiate(itemModificator.onHitEffects[i]);
            }
            itemModificator.scriptableShopItem = info;
        }

        private void Reset()
        {
            CanBeSold = false;
        }

        protected virtual void Update()
        {
            
        }

        /// <summary>
        /// When the object is purchased, it is created in an empty slot in the <see cref="Battle.ItemBenches[0]"/>.
        /// </summary>
        /// <param name="shopItem">This item.</param>
        /// <returns>Bough item in the <see cref="Battle.ItemBenches[0]"/>.</returns>
        public override GameActor Buy(GameActor shopItem)
        {
            GridCell cell = Battle.Instance.ItemBenches[0].GridCells.Where(x => x.shopItem == null).FirstOrDefault();
            GameItem item = Instantiate(shopItem) as GameItem;
            SetVariablesOnBuy(item);
            return item;
        }

        protected virtual void SetVariablesOnBuy(GameItem item, IPlayer player = null)
        {
            GridCell cell = Battle.Instance.ItemBenches[0].GridCells.Where(x => x.shopItem == null).FirstOrDefault();

            Vector3 pos = cell.transform.position;
            pos.y += 1f;
            item.transform.position = pos;
        }



        /// <summary>
        /// The items does nothing after being bought
        /// </summary>
        /// <param name="shopItem"></param>
        public override void AfterBought()
        {
            //
        }

        /// <summary>
        /// Allows dragging of items.
        /// </summary>
        public override void OnDragObjectAction()
        {
            if (CanBeMoved)
            {
                if (Battle.Instance.stage.GetCurrentState().AllowFieldDrag(this))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit[] hits = Physics.RaycastAll(ray, 100f, LayerMask.GetMask("BattleSurface"));

                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("BattleSurface"))
                        {
                            Vector3 newPos = hit.point;
                            newPos.y = transform.position.y;
                            transform.position = newPos;

                            RaycastHit hitDown;
                            if (Physics.Raycast(transform.position, Vector3.down, out hitDown, 10, LayerMask.GetMask("GridCell"), QueryTriggerInteraction.Collide))
                            {
                                if (hitDown.collider.GetComponent<GridCell>())
                                {
                                    if (CurrentDragCell)
                                    {
                                        CurrentDragCell.SetDragEffect(false);
                                    }
                                    CurrentDragCell = hitDown.collider.GetComponent<GridCell>();
                                    CurrentDragCell.SetDragEffect(true);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If an item is dragged to the position where another item is located, they exchange their positions.
        /// </summary>
        /// <param name="c1">Cell where the first item is located.</param>
        /// <param name="c2">Cell where the second item is located.</param>
        protected void SwapItemsInCells(GridCell c1, GridCell c2)
        {
            GameItem item2 = CurrentDragCell.shopItem as GameItem;

            item2.CurrentGridCell = c2;
            item2.MoveGameItemTo(c2);

            CurrentGridCell = c1;
            MoveGameItemTo(c1);

            c2.shopItem = item2;
            c1.shopItem = this;

        }

        /// <summary>
        /// Forces the movement of an item to a cell.
        /// </summary>
        /// <param name="cell">New cell where the item is located</param>
        protected void MoveGameItemTo(GridCell cell)
        {
            Vector3 pos = cell.transform.position;
            pos.y = transform.position.y;
            transform.position = pos;
            isDragged = false;

            if (CurrentDragCell)
                CurrentDragCell.SetDragEffect(false);
        }

        /// <summary>
        /// Start the drag.
        /// </summary>
        public override void OnMouseDownAction()
        {
            isDragged = true;
        }

        /// <summary>
        /// Allows an item to be equipped to a character, to be moved within the <see cref="Battle.ItemBenches[0]"/> or to exchange its position with that of another item.
        /// In Android, if the item is dragged to the same cell, shows the description of the item.
        /// </summary>
        public override void OnMouseUpAction()
        {
            int teamIndex = GetTeamIndex();

            if (CurrentDragCell)
            {
                if (CurrentDragCell.shopItem)
                {
                    GameCharacter ai = CurrentDragCell.shopItem.GetComponent<GameCharacter>();
                    GameItem item = CurrentDragCell.shopItem.GetComponent<GameItem>();
                    if (ai && Battle.Instance.stage.GetCurrentState().AllowFieldDrag(this) && (Battle.Instance.teams[teamIndex].team.Contains(ai) || Battle.Instance.TeamBenches[teamIndex].GridCells.Contains(CurrentDragCell)))
                    {
                        AddItemModificator(ai);
                        CurrentDragCell.SetDragEffect(false);
                    }
                    else if (item && Battle.Instance.stage.GetCurrentState().AllowFieldDrag(this) && Battle.Instance.ItemBenches[teamIndex].GridCells.Contains(CurrentDragCell) && this != item)
                    {
                        SwapItemsInCells(CurrentDragCell, CurrentGridCell);
                    }
                    else
                    {
                        CancelDrag();
#if UNITY_ANDROID
                        ShowUI(true);
#endif
                    }
                }

                else if ((!CurrentDragCell.shopItem || CurrentDragCell.shopItem == this) && (CurrentDragCell.CanPlaceCharacter == teamIndex) && (Battle.Instance.ItemBenches[teamIndex].GridCells.Contains(CurrentDragCell)
                    && Battle.Instance.stage.GetCurrentState().AllowFieldDrag(this)
                    || Battle.Instance.ItemBenches[teamIndex].GridCells.Contains(CurrentDragCell)))
                {
                    GetCurrentCell();
                    Vector3 pos = CurrentGridCell.transform.position;
                    pos.y = transform.position.y;
                    transform.position = pos;
                    isDragged = false;
                }
                else
                {
                    CancelDrag();
                }
            }
        }

        /// <summary>
        /// Adds the item's <see cref="itemModificator"/> to the character's <see cref="BattleBehaviour.GameActors.GameCharacter.itemModificators"/>.
        /// </summary>
        /// <param name="character">Character to be equipped with the item</param>
        public virtual void AddItemModificator(GameCharacter character)
        {
            itemModificator.AddItemModificator(character);
            Battle.Instance.TraitCheck(Battle.Instance.teams[0].team, Battle.Instance.TeamBenches[0].GetGameCharacterInBench(), Battle.Instance.TraitsToCheck, Battle.Instance.TeamTraitListUI[0]);
            Destroy(gameObject);
        }


        /// <summary>
        /// Cancels the dragging of the item.
        /// </summary>
        public void CancelDrag()
        {
            if (isDragged)
            {
                transform.position = getInitialPosition();
                GetCurrentCell();
                isDragged = false;
                CurrentDragCell.SetDragEffect(false);
            }
        }

        /// <summary>
        /// Diplays the interface describing the item.
        /// </summary>
        public override void OnMouseOverAction()
        {
            if (Input.GetMouseButtonUp(1))
            {
                ShowUI(true);
            }
        }

        /// <summary>
        /// Sell the character and gain <see cref="Shop.ShopManager.currency"/>.
        /// </summary>
        public override void Sell()
        {
            if (CanBeSold)
            {
                Battle.Instance.shopManager.currency += SellFor;
                Debug.Log("Sell for " + SellFor);
                if (Battle.Instance.shopManager.RemoveFromListWhenBought)
                {
                    Battle.Instance.shopManager.RestoreShopItemForAllLists(info);
                }

                Destroy(gameObject);
            }
            else
            {
                CancelDrag();
            }
        }

        /// <summary>
        /// Diplays the <see cref="Battle.itemDescriptionUI"/> describing the item.
        /// </summary>
        /// <param name="show">Show or hide the UI.</param>
        /// <returns>The gameobject of <see cref="Battle.itemDescriptionUI"/>.</returns>
        public override GameObject ShowUI(bool show)
        {
            if (show)
            {
                Battle.Instance.itemDescriptionUI.SetUI(gameObject, itemModificator, info.itemImage, info.itemName, info.itemDescription);
                Battle.Instance.itemDescriptionUI.ShowUI(true);
            }
            else
            {
                Battle.Instance.itemDescriptionUI.ShowUI(false);
            }
            return Battle.Instance.itemDescriptionUI.gameObject;
        }

    }
}