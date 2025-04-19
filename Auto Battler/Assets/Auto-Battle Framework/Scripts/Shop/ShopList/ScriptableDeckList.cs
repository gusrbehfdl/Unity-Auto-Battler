using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoBattleFramework.Shop.ShopList
{
    /// <summary>
    /// Mimics the behavior of a deck of cards. You have the main deck, the discarded cards and the cards in hand.
    /// </summary>
    [CreateAssetMenu(fileName = "ScriptableDeckList", menuName = "Auto-Battle Framework/IShopList/ScriptableDeckList", order = 5)]
    public class ScriptableDeckList : IShopList
    {
        /// <summary>
        /// Cards currently in the deck.
        /// </summary>
        public List<ShopItemInfo> DeckList;

        /// <summary>
        /// Cards that have been played or discarded.
        /// </summary>
        public List<ShopItemInfo> DiscardList;

        /// <summary>
        /// Cards currently in hand (shown in the store interface).
        /// </summary>
        public List<ShopItemInfo> CardsInHand;

        /// <summary>
        /// The list has been backed.
        /// </summary>
        bool backedUp;

        /// <summary>
        /// Cards removed from the deck.
        /// </summary>
        public List<ShopItemInfo> RemovedItems;

        /// <summary>
        /// Adds a card to the deck
        /// </summary>
        /// <param name="info">Card to be added</param>
        /// <returns>The added card</returns>
        public override ShopItemInfo AddItemInfo(ShopItemInfo info)
        {
            DeckList.Add(info);
            return DeckList.Last();
        }

        /// <summary>
        /// Initializes all variables.
        /// </summary>
        public override void Initialize()
        {
            if (DiscardList == null)
                DiscardList = new List<ShopItemInfo>();
            if (CardsInHand == null)
                CardsInHand = new List<ShopItemInfo>();

            foreach (ShopItemInfo item in DeckList)
            {
                item.scriptableShopItem = Instantiate(item.scriptableShopItem);
            }

            backedUp = false;
        }

        /// <summary>
        /// Discards all cards in hand and draws a number of cards from the deck.
        /// </summary>
        /// <param name="numberOfItems">Number of cards to be drawn.</param>
        /// <param name="RepeatItems">In this case it is not used.</param>
        /// <returns></returns>
        public override List<ShopItemInfo> GetRandomItems(int numberOfItems, bool RepeatItems)
        {
            DiscardHand();

            numberOfItems = Mathf.Min(numberOfItems, DeckList.Count + CardsInHand.Count + DiscardList.Count);

            for (int i = 0; i < numberOfItems; i++)
            {
                Draw(CardsInHand, RepeatItems);
            }
            return CardsInHand;
        }

        /// <summary>
        /// Discard all cards in the hand.
        /// </summary>
        void DiscardHand()
        {
            DiscardList.AddRange(CardsInHand);
            CardsInHand.Clear();
        }

        /// <summary>
        /// Discard a card from the hand.
        /// </summary>
        /// <param name="info">Card to be discarded</param>
        void DiscardCard(ShopItemInfo info)
        {
            DiscardList.Add(info);
            CardsInHand.Remove(info);
        }


        /// <summary>
        /// Shuffle deck and discards.
        /// </summary>
        void ShuffleDiscard()
        {
            DeckList.AddRange(DiscardList);
            DiscardList.Clear();
            ShuffleDeck();
        }

        /// <summary>
        /// Shuffle the deck.
        /// </summary>
        void ShuffleDeck()
        {
            Shuffle(DeckList);
        }

        /// <summary>
        /// Remove a card from the deck.
        /// </summary>
        /// <param name="info">Card to be removed</param>
        /// <returns>If card removal has been successful</returns>
        public override bool RemoveItemInfo(ShopItemInfo info)
        {
            if (RemovedItems == null)
            {
                RemovedItems = new List<ShopItemInfo>();
            }


            List<ShopItemInfo> selected = null;
            
            ShopItemInfo sii = DeckList.Where(x => x.scriptableShopItem.shopItem == info.scriptableShopItem.shopItem).FirstOrDefault();
            if (sii != null)
            {
                selected = DeckList;
            }

            if (sii == null)
            {
                sii = CardsInHand.Where(x => x.scriptableShopItem.shopItem == info.scriptableShopItem.shopItem).FirstOrDefault();
                if (sii != null)
                {
                    selected = CardsInHand;
                }
            }
            if (sii == null)
            {
                sii = DiscardList.Where(x => x.scriptableShopItem.shopItem == info.scriptableShopItem.shopItem).FirstOrDefault();
                if(sii != null)
                {
                    selected = DiscardList;
                }
            }
            if (selected != null)
            {
                RemovedItems.Add(sii);
                selected.Remove(sii);
                return DeckList.Remove(info);
            }
            return false;
        }

        /// <summary>
        /// Restore a <see cref="ShopItemInfo"/> that has been bought before. Returns true if successful.
        /// </summary>
        /// <param name="info">Item to be restored.</param>
        /// <returns>True if succesful</returns>
        public override bool RestoreItemInfo(ScriptableShopItem info)
        {
            if (RemovedItems == null)
            {
                RemovedItems = new List<ShopItemInfo>();
            }
            ShopItemInfo sii = RemovedItems.Where(x => x.scriptableShopItem.shopItem == info.shopItem).FirstOrDefault();
            if (sii == null)
            {
                return false;
            }
            else
            {
                DiscardList.Add(sii);
                RemovedItems.Remove(sii);
                return true;
            }
        }

        /// <summary>
        /// Shuffle a list of cards
        /// </summary>
        /// <param name="list">Cards to be shuffled</param>
        public void Shuffle(List<ShopItemInfo> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                ShopItemInfo value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Makes a backup of the deck. Prevents the ScriptableObject from being overwritten.
        /// </summary>
        /// <returns>Backed deck.</returns>
        public override IShopList Backup()
        {
            if (backedUp)
                return this;
            else
            {
                backedUp = true;
                ScriptableDeckList sdl = Instantiate(this);
                sdl.Initialize();
                sdl.ShuffleDeck();
                sdl.backedUp = true;
                return sdl;
            }
        }

        /// <summary>
        /// Draw a card from the deck. Add it to a list.
        /// </summary>
        /// <param name="items">List where the card will be added.</param>
        /// <param name="RepeatItems">Not used in this case.</param>
        /// <returns>Drawn card.</returns>
        public override ShopItemInfo Draw(List<ShopItemInfo> items, bool RepeatItems)
        {
            if (DeckList.Count == 0)
            {
                ShuffleDiscard();
            }
            ShopItemInfo selected = DeckList.First();
            DeckList.Remove(selected);
            CardsInHand.Add(selected);
            return selected;
        }

        /// <summary>
        /// When the card is bought, the card is discarded.
        /// </summary>
        /// <param name="info">Bought card.</param>
        public override void OnBuy(ShopItemInfo info)
        {
            DiscardCard(info);
        }

        /// <summary>
        /// Modify the stats of a GameActor.
        /// </summary>
        /// <param name="actor">GameActor to modify</param>
        public override void ModifyGameActor(GameActor actor)
        {
            var allItems = DeckList.Union(CardsInHand).Union(DiscardList).Where(x => x.scriptableShopItem.itemName == actor.info.itemName);

            foreach (ShopItemInfo item in allItems)
            {
                item.scriptableShopItem.shopItem = actor;
            }
        }
    }
}