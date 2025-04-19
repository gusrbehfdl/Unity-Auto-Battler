using AutoBattleFramework.BattleBehaviour.Fusion;
using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Battlefield;
using AutoBattleFramework.BattleUI;
using AutoBattleFramework.Shop;
using AutoBattleFramework.Stats;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace AutoBattleFramework.Multiplayer.BattleBehaviour.Player
{
    /// <summary>
    /// Base player that stores all the information necessary to identify objects and areas belonging to the player. It also allows sending and receiving messages from the host.
    /// It will be used in a package featuring multiplayer to be released in version 1.2.
    /// </summary>
    public abstract class IPlayer : NetworkBehaviour
    {
        /// <summary>
        /// P^ayer Character bench.
        /// </summary>
        public Bench CharacterBench;

        /// <summary>
        /// Player item bench.
        /// </summary>
        public Bench ItemBench;

        /// <summary>
        /// Trait List UI of the player.
        /// </summary>
        public TraitListUI TraitList;

        /// <summary>
        /// Trat list of the player.
        /// </summary>
        public List<Trait> TraitsToCheckTeam;

        /// <summary>
        /// Static reference to the player.
        /// </summary>
        public static IPlayer instance;

        /// <summary>
        /// If the player is the host.
        /// </summary>
        public bool IsPlayerHost { get { return IsHost; } }


        /// <summary>
        /// Get the player instance by its ID.
        /// </summary>
        /// <param name="ID">ID of the player.</param>
        /// <returns>Player instance with the given ID.</returns>
        public static IPlayer GetPlayerById(ulong ID)
        {
            return FindObjectsOfType<IPlayer>().Where(x => x.OwnerClientId == ID).FirstOrDefault();
        }

        /// <summary>
        /// Make the player change the stage.
        /// </summary>
        public abstract void NextStage();

        /// <summary>
        /// Ask the host to spawn a GameActor at the given position.
        /// </summary>
        /// <param name="itemName">Actor´s item name</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="z">Z position</param>
        public abstract void SpawnGameActorServer(Unity.Collections.FixedString128Bytes itemName, float x, float y, float z);

        /// <summary>
        /// The host moves the character to the starting position.
        /// </summary>
        public abstract void ResetCharactersPositionsClient();

        /// <summary>
        /// The host makes a fusion of characters.
        /// </summary>
        /// <param name="fusionMembers">List of fusion members.</param>
        /// <param name="FusionResult">Result of the fusion</param>
        public abstract void FusionMembers(List<GameCharacter> fusionMembers, ShopCharacter FusionResult);

        /// <summary>
        /// ASk the server to remove the item from a character.
        /// </summary>
        /// <param name="GameCharacterID">Character's network ID.</param>
        /// <param name="itemModificatorIndex">Index of the item modificator.</param>
        public abstract void RemoveItemFromCharacterServer(ulong GameCharacterID, ushort itemModificatorIndex);

        /// <summary>
        /// The host removes the item from a character.
        /// </summary>
        /// <param name="GameCharacterID">Character's network ID.</param>
        /// <param name="itemModificatorIndex">Index of the item modificator.</param>
        public abstract void RemoveItemFromCharacterClient(ulong GameCharacterID, ushort itemModificatorIndex);


    }
}