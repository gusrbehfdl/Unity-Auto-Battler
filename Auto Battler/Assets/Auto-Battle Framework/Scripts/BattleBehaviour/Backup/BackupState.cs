using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.BattleBehaviour.States;
using AutoBattleFramework.Battlefield;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.BattleBehaviour.Backup
{
    /// <summary>
    /// It stores the information necessary to create a backup copy of a state, so that if the player loses, he can return to a previous point, as if nothing had happened.
    /// </summary>
    public class BackupState
    {
        /// <summary>
        /// Backup of the characters in that round.
        /// </summary>
        public List<GameCharacter> CharactersBackup;

        /// <summary>
        /// Backup of all benches.
        /// </summary>
        public Dictionary<Bench, List<GameActor>> BenchesBackup;

        /// <summary>
        /// The round that has been backed up.
        /// </summary>
        public BattleState BattleStateBackup;

        /// <summary>
        /// Backup of characters edited in the shop.
        /// </summary>
        public List<GameCharacter> ShopGameCharactersBackup;

        /// <summary>
        /// Amount of currency the player has at the start of the state.
        /// </summary>
        public int CurrencyBackup;

        /// <summary>
        /// Amount of currency the player has at the start of the state.
        /// </summary>
        public int ShopLevel;

        /// <summary>
        /// Amount of currency the player has at the start of the state.
        /// </summary>
        public int ShopExperience;

        /// <summary>
        /// Amount of currency the player has at the start of the state.
        /// </summary>
        public Random.State SeedBackup;

        /// <summary>
        /// Backup constructor.
        /// </summary>
        /// <param name="battleStateBackup">Current BattleState.</param>
        public BackupState(BattleState battleStateBackup)
        {
            CharactersBackup = new List<GameCharacter>();
            BattleStateBackup = battleStateBackup;
            BenchesBackup = new Dictionary<Bench, List<GameActor>>();
            CurrencyBackup = Battle.Instance.shopManager.currency;
            SeedBackup = Random.state;
            ShopLevel = Battle.Instance.shopManager.shopLevelManager.CurrentLevel;
            ShopExperience = Battle.Instance.shopManager.shopLevelManager.CurrentExp;
        }
    }
}