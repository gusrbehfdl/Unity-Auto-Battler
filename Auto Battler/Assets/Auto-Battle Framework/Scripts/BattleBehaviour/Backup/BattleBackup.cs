using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.BattleBehaviour.States;
using AutoBattleFramework.Battlefield;
using AutoBattleFramework.Shop;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoBattleFramework.BattleBehaviour.Backup
{
    /// <summary>
    /// It is used in case of losing a round, to maintain the state of the player's equipment before the changes made during the preparation of the last round.
    /// </summary>
    public class BattleBackup : MonoBehaviour
    {

        /// <summary>
        /// Singleton of the Battle backup.
        /// </summary>
        public static BattleBackup Instance { get; private set; }

        /// <summary>
        /// Backup of the last stage. Usually a Preparation State backup.
        /// </summary>
        public BackupState LastStateBackup;

        /// <summary>
        /// Backup of the last stage. Usually the first Preparation State in a stage.
        /// </summary>
        public BackupState LastStageBackup;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Creates a backup copy of the GameActors in the current BattleState.
        /// </summary>
        /// <param name="state">Current BattleState.</param>
        /// <param name="PlayerTeam">List of player characters on the battlefield.</param>
        /// <param name="Benches">All benches for which you want to make a backup copy. This can include character and item banks.</param>
        public void BackupLastState(BattleState state, List<GameCharacter> PlayerTeam, params Bench[] Benches)
        {
            if (LastStateBackup == null)
            {
                LastStateBackup = new BackupState(state);
            }
            else
            {
                if (state == LastStateBackup.BattleStateBackup)
                    return;
            }

            //BackupPlayerTeam(LastStateBackup, state, PlayerTeam, PlayerItems);
            BackupPlayerTeam(LastStateBackup, state, PlayerTeam, Battle.Instance.shopManager.GameActorsModifiedBackupState, Benches);
        }

        /// <summary>
        /// This copy is considered to be the first state of a phase, so it is advisable to call it only during the first state.
        /// </summary>
        /// <param name="state">Current BattleState.</param>
        /// <param name="PlayerTeam">List of player characters on the battlefield.</param>
        /// <param name="Benches">All benches for which you want to make a backup copy. This can include character and item banks.</param>
        public void BackupLastStage(BattleState state, List<GameCharacter> PlayerTeam, params Bench[] Benches)
        {
            if (LastStageBackup == null)
            {
                LastStageBackup = new BackupState(state);
            }
            else
            {
                if (state == LastStageBackup.BattleStateBackup)
                    return;
            }
            BackupPlayerTeam(LastStageBackup, state, PlayerTeam, Battle.Instance.shopManager.GameActorsModifiedBackupStage, Benches);
        }

        /// <summary>
        /// Apply the State backup. Deletes all player characters and objects, and resets those in the state backup.
        /// </summary>
        /// <param name="state">Current BattleState</param>
        /// <param name="PlayerIndex">Index of the player. Default 0.</param>
        /// <returns>Returns true if the backup has been applied successfully.</returns>
        public bool ApplyLastStateBackup(BattleState state, int PlayerIndex = 0)
        {
            if (LastStateBackup != null)
            {
                return ApplyBackup(LastStateBackup, state, Battle.Instance.shopManager.GameActorsModifiedBackupState, PlayerIndex);
            }
            return false;
        }

        /// <summary>
        /// Apply the Stage backup. Deletes all player characters and objects, and resets those in the stage backup.
        /// </summary>
        /// <param name="state">Current BattleState</param>
        /// <param name="PlayerIndex">Index of the player. Default 0.</param>
        /// <returns>Returns true if the backup has been applied successfully.</returns>
        public bool ApplyLastStageBackup(BattleState state, int PlayerIndex = 0)
        {
            if (LastStageBackup != null)
            {
                return ApplyBackup(LastStageBackup, state, Battle.Instance.shopManager.GameActorsModifiedBackupStage, PlayerIndex);
            }
            return false;
        }

        /// <summary>
        /// Remove the Last Stage Backup.
        /// </summary>
        public void RemoveLastStageBackup()
        {
            RemoveBackup(LastStageBackup);
            LastStageBackup = null;
        }


        /// <summary>
        /// Remove a backup state. Destroys all GameActors that has been backed up.
        /// </summary>
        /// <param name="backup"></param>
        protected void RemoveBackup(BackupState backup)
        {
            foreach (GameCharacter charBackup in backup.CharactersBackup.ToList())
            {
                Destroy(charBackup.gameObject);
            }
            foreach (Bench bench in backup.BenchesBackup.Keys)
            {
                foreach (GameActor actor in backup.BenchesBackup[bench].ToList())
                {
                    Destroy(actor.gameObject);
                }
            }

            backup.CharactersBackup.Clear();
            backup.BenchesBackup.Clear();
            backup.BattleStateBackup = null;
        }

        /// <summary>
        /// Backup copy of the status of characters and characters. Can be used to return to the previous state of a round, before making changes.
        /// </summary>
        /// <param name="backup">Backup data.</param>
        /// <param name="state">Current BattleState</param>
        /// <param name="PlayerTeam">List of player characters on the battlefield.</param>
        /// <param name="Benches">All benches for which you want to make a backup copy. This can include character and item banks.</param>
        protected void BackupPlayerTeam(BackupState backup, BattleState state, List<GameCharacter> PlayerTeam, Transform shopBackupTransform, params Bench[] Benches)
        {
            foreach (GameCharacter charBackup in backup.CharactersBackup.ToList())
            {
                Destroy(charBackup.gameObject);
            }
            foreach (Bench bench in backup.BenchesBackup.Keys)
            {
                foreach (GameActor actor in backup.BenchesBackup[bench].ToList())
                {
                    Destroy(actor.gameObject);
                }
            }

            backup.CharactersBackup.Clear();
            backup.BenchesBackup.Clear();

            foreach (GameCharacter character in PlayerTeam)
            {
                GameCharacter characterCopy = Instantiate(character);
                characterCopy.gameObject.SetActive(false);
                backup.CharactersBackup.Add(characterCopy);
            }

            foreach (Bench bench in Benches)
            {
                backup.BenchesBackup.TryAdd(bench, new List<GameActor>());
                foreach (GameActor actor in bench.GetShopItemInBench())
                {
                    GameActor actorCopy = Instantiate(actor);
                    actorCopy.gameObject.SetActive(false);
                    backup.BenchesBackup[bench].Add(actorCopy);
                }
            }

            //Destroy the backups of the edited characters in shop lists.
            List<GameCharacter> shopBackups = shopBackupTransform.GetComponentsInChildren<GameCharacter>(true).ToList();
            foreach (GameCharacter character in shopBackups.ToList())
            {
                Destroy(character.gameObject);
            }

            backup.CurrencyBackup = Battle.Instance.shopManager.currency;
            backup.SeedBackup = Random.state;
            backup.BattleStateBackup = state;
            backup.ShopLevel = Battle.Instance.shopManager.shopLevelManager.CurrentLevel;
            backup.ShopExperience = Battle.Instance.shopManager.shopLevelManager.CurrentExp;
        }

        /// <summary>
        /// Apply the Backup of the current BattleState.
        /// </summary>
        /// <param name="backup">Backup data.</param>
        /// <param name="state">Current BattleState.</param>
        /// <param name="PlayerIndex">Index of the player. Default 0.</param>
        /// <returns></returns>
        protected bool ApplyBackup(BackupState backup, BattleState state, Transform shopBackupTransform, int PlayerIndex = 0)
        {
            if (backup.BattleStateBackup != state) return false;

            foreach (GameCharacter character in Battle.Instance.teams[PlayerIndex].team.ToList())
            {
                Destroy(character.gameObject);
            }

            foreach (Bench bench in backup.BenchesBackup.Keys)
            {
                foreach (GameActor actor in bench.GetShopItemInBench().ToList())
                {
                    if(actor.CurrentGridCell)
                        actor.CurrentGridCell.shopItem = null;
                    Destroy(actor.gameObject);
                }
            }

            List<GameCharacter> newCharacters = new List<GameCharacter>();
            foreach (GameCharacter character in backup.CharactersBackup)
            {
                GameCharacter backedCharacter = Instantiate(character);
                backedCharacter.gameObject.SetActive(true);
                backedCharacter.CurrentGridCell.shopItem = backedCharacter;
                newCharacters.Add(backedCharacter);
            }

            foreach (Bench bench in backup.BenchesBackup.Keys)
            {
                foreach (GameActor actor in backup.BenchesBackup[bench])
                {
                    GameActor backedActor = Instantiate(actor);
                    backedActor.gameObject.SetActive(true);
                    if (backedActor.CurrentGridCell != null)
                    {
                        backedActor.CurrentGridCell.shopItem = backedActor;
                    }
                }
            }


            //If some character in the shop lists has been edited, restore the backups.
            List<GameCharacter> shopBackups = shopBackupTransform.GetComponentsInChildren<GameCharacter>(true).ToList();
            foreach(GameCharacter character in shopBackups.ToList())
            {
                Battle.Instance.shopManager.RestoreGameCharacterBackupForAllLists(character, shopBackupTransform);
            }
            

            Battle.Instance.teams[PlayerIndex].team = newCharacters;

            Battle.Instance.shopManager.currency = backup.CurrencyBackup;
            Battle.Instance.shopManager.shopLevelManager.CurrentLevel = backup.ShopLevel;
            Battle.Instance.shopManager.shopLevelManager.CurrentExp = backup.ShopExperience;

            if (Battle.Instance.KeepSeedWhenUsingBackup)
            {
                Random.state = backup.SeedBackup;
            }

            Battle.Instance.characterStatsUI.gameObject.SetActive(false);
            Battle.Instance.itemDescriptionUI.gameObject.SetActive(false);

            return true;
        }

    }
}