using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Battlefield;
using AutoBattleFramework.BattleUI;
using AutoBattleFramework.Multiplayer.BattleBehaviour.Player;
using AutoBattleFramework.Shop;
using AutoBattleFramework.Stats;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace AutoBattleFramework.BattleBehaviour.Fusion
{
    /// <summary>
    /// Given an <see cref="BattleBehaviour.Fusion.GameCharacterFusion"/>, it is responsible for performing character merging if possible.
    /// </summary>
    public class FusionManager : MonoBehaviour
    {
        /// <summary>
        /// List of possible fusions to consider.
        /// </summary>
        [Tooltip("List of possible fusions.")]
        public List<GameCharacterFusion> FusionList;

        /// <summary>
        /// When fusing, merge all items into the new character.
        /// </summary>
        [Tooltip("When fusing, merge all items into the new character.")]
        public bool KeepItems = true;

        /// <summary>
        /// Given a character, try to perform a fusion.
        /// </summary>
        /// <param name="character">Character to try to fusion.</param>
        public void TryFusion(ShopCharacter character)
        {
            int teamIndex = 0;
            List<Trait> traitList = Battle.Instance.TraitsToCheck;
            TraitListUI traitUI = Battle.Instance.TeamTraitListUI[0];
            List<GameCharacterFusion> possibleFusions = FindFusionRelatedToCharacter(character);

            foreach(GameCharacterFusion fusion in possibleFusions)
            {
                bool sucessful = Fusion(fusion);

                if (sucessful)
                {
                    if (IPlayer.instance)
                    {
                        teamIndex = (int)IPlayer.instance.OwnerClientId;
                        traitList = IPlayer.instance.TraitsToCheckTeam;
                        traitUI = IPlayer.instance.TraitList;
                    }
                    Battle.Instance.TraitCheck(Battle.Instance.teams[teamIndex].team, Battle.Instance.TeamBenches[teamIndex].GetGameCharacterInBench(), traitList, traitUI);
                }
            }
        }

        /// <summary>
        /// Find a list of possible fusions related to the character.
        /// </summary>
        /// <param name="character">Character to be found inside fusions.</param>
        /// <returns>List of possible fusions.</returns>
        List<GameCharacterFusion> FindFusionRelatedToCharacter(ShopCharacter character)
        {
            return FusionList.Where(x => x.CharactersToFusion.Contains(character)).ToList();
        }

        /// <summary>
        /// Given a fusion, try to perform the merger. Returns true if the fussion is successful.
        /// </summary>
        /// <param name="fusion">Fusion information.</param>
        /// <returns>True if fusion is successful.</returns>
        bool Fusion(GameCharacterFusion fusion)
        {
            List<GameCharacter> fusionMembers = FindFusionCharacters(fusion);
            if (fusionMembers == null)
            {
                return false;
            }

            if (!IPlayer.instance)
            {
                //Single player
                FusionCharacters(fusionMembers, fusion.FusionResult);
            }
            else
            {
                //Multiplayer
                FusionCharactersMultiplayer(fusionMembers, fusion.FusionResult);
            }

            return true;
        }

        /// <summary>
        /// Find a list of GameCharacters that will be part of the fusion.
        /// </summary>
        /// <param name="fusion">Fusion information.</param>
        /// <returns>List of GameCharacters that will be fused. Return null if not enought characters are found.</returns>
        List<GameCharacter> FindFusionCharacters(GameCharacterFusion fusion)
        {
            int teamIndex = 0;
            if (NetworkManager.Singleton)
            {
                teamIndex = (int)IPlayer.instance.OwnerClientId;
            }

            List<GameCharacter> team1 = new List<GameCharacter>();
            if (Battle.Instance.stage.GetCurrentState().AllowFieldDrag(null))
            {
                team1 = Battle.Instance.teams[teamIndex].team;
            }
            List<GameCharacter> bench = Battle.Instance.TeamBenches[teamIndex].GetGameCharacterInBench();

            List<GameCharacter> all = new List<GameCharacter>(team1);
            all.AddRange(bench);

            List<GameCharacter> usedInFusion = new List<GameCharacter>();

            foreach (ScriptableShopItem fusionMember in fusion.CharactersToFusion)
            {
                bool found = false;
                List<GameCharacter> foundMembers = all.Where(x => x.info == fusionMember).ToList();

                foreach (GameCharacter character in foundMembers)
                {
                    if (!usedInFusion.Contains(character))
                    {
                        usedInFusion.Add(character);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return null;
                }
            }

            return usedInFusion;
        }


        /// <summary>
        /// Removes the characters involved in the fusion, and instantiate the result.
        /// </summary>
        /// <param name="fusionMembers">List of characters that will be destroyed.</param>
        /// <param name="FusionResult">Character that will be created.</param>
        void FusionCharacters(List<GameCharacter> fusionMembers, ShopCharacter FusionResult)
        {
            Vector3 positionToSpawn = fusionMembers[0].transform.position;
            GridCell gridCell = fusionMembers[0].CurrentGridCell;
            bool addToTeam = Battle.Instance.teams[0].team.Contains(fusionMembers[0]);

            //Get the items before the character get destroyed
            List<ItemModificator> allItems = GetItemsFromMembers(fusionMembers);

            //Destroy the characters.
            foreach (GameCharacter character in fusionMembers)
            {
                if (Battle.Instance.teams[0].team.Contains(character))
                {
                    Battle.Instance.teams[0].team.Remove(character);
                    Destroy(character.gameObject);
                }
                else
                {
                    Destroy(character.gameObject);
                }
            }

            //Instantiate the result.
            var result = Instantiate(FusionResult.shopItem) as GameCharacter;
            if (addToTeam)
            {
                Battle.Instance.teams[0].team.Add(result);
            }
            else
            {
                result.State = GameCharacter.AIState.Benched;
            }

            //Equip the items to the new character
            if (KeepItems)
            {
                foreach (ItemModificator item in allItems)
                {
                    item.AddItemModificator(result);
                }
            }

            //Set the position of the character and assign the state.
            result.gameObject.transform.position = positionToSpawn;
            result.CurrentGridCell = gridCell;
            result.CurrentGridCell.shopItem = result;
            Battle.Instance.SetBattleOrBenchState(result);
        }


        /// <summary>
        /// In multiplayer, removes the characters involved in the fusion, and instantiate the result.
        /// </summary>
        /// <param name="fusionMembers">List of characters that will be destroyed.</param>
        /// <param name="FusionResult">Character that will be created.</param>
        void FusionCharactersMultiplayer(List<GameCharacter> fusionMembers, ShopCharacter FusionResult)
        {
            IPlayer player = IPlayer.GetPlayerById(fusionMembers[0].OwnerClientId);

            player.FusionMembers(fusionMembers,FusionResult);
        }

        /// <summary>
        /// Get all item modificators from all fusion members.
        /// </summary>
        /// <param name="fusionMemebers">List of characters to merge.</param>
        /// <returns>List of item modificators equipped to all characters.</returns>
        List<ItemModificator> GetItemsFromMembers(List<GameCharacter> fusionMemebers)
        {
            return fusionMemebers.SelectMany(x => x.itemModificators).ToList();
        }
    }
}