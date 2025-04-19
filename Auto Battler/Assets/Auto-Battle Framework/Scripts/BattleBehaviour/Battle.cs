using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using AutoBattleFramework.Stats;
using AutoBattleFramework.Battlefield;
using AutoBattleFramework.BattleUI;
using AutoBattleFramework.Shop;
using AutoBattleFramework.Shop.ShopGUI;
using AutoBattleFramework.Formulas;
using AutoBattleFramework.BattleBehaviour.Fusion;
using AutoBattleFramework.BattleBehaviour.GameActors;
using UnityEngine.UI;
using Unity.Netcode;
using AutoBattleFramework.Multiplayer.BattleBehaviour.Player;

namespace AutoBattleFramework.BattleBehaviour
{
    /// <summary>
    /// Main class of the framework. Controls the battle phase and the interface.
    /// Contains the references to all the main systems.
    /// </summary>
    public class Battle : MonoBehaviour
    {

        /// <summary>
        /// Singleton of the Battle.
        /// </summary>
        public static Battle Instance { get; private set; }

        [Header("Teams")]

        /// <summary>
        /// List of lists of player characters on the battlefield.
        /// In single player mode, the first index corresponds to the player's team, and the second to the enemy's team.
        /// In multiplayer, The first in the list will belong to player 1, the second to player 2 and so on.
        /// </summary>
        [SerializeField]
        public List<TeamData> teams;

        /// <summary>
        /// Number of characters that the player can place in play at the start of the game.
        /// </summary>
        [Tooltip("Initial number of characters.")]
        public int InitialCharacters = 2;

        [Header("Battlefield")]

        /// <summary>
        /// Game board where the battle between the teams in <see cref="teams"/> takes place.
        /// </summary>
        [Tooltip("Game board.")]
        public BattleGrid grid;

        /// <summary>
        /// List of player character benches.
        /// In single player mode, the first index corresponds to the player's bench, and the second to the enemy's bench.
        /// In multiplayer, The first in the list will belong to player 1, the second to player 2 and so on.
        /// </summary>
        [Tooltip("Bench of player characters.")]
        public List<Bench> TeamBenches;

        /// <summary>
        /// List of player item benches.
        /// In single player mode, the first index corresponds to the player's bench, and the second to the enemy's bench.
        /// In multiplayer, The first in the list will belong to player 1, the second to player 2 and so on.
        /// </summary>
        [Tooltip("Bench of items not equipped to characters.")]
        public List<Bench> ItemBenches;

        /// <summary>
        /// List of player item benches.
        /// In single player mode, the first index corresponds to the player's bench, and the second to the enemy's bench.
        /// In multiplayer, The first in the list will belong to player 1, the second to player 2 and so on.
        /// </summary>
        [Tooltip("Sell zones of each character.")]
        public List<SellZone> SellZones;


        [Header("Stage")]

        /// <summary>
        /// Battle stage. Contains the different sub-states that of the stage, which controls how it passes from one phase to another and how the characters behave in each of them.
        /// </summary>
        [Tooltip("Current battle stage.")]
        public ScriptableBattleStage stage;

        /// <summary>
        /// Timer that controls the time of each battle state. When it reaches zero, the next phase is executed.
        /// </summary>
        [Tooltip("Timer that controls the time of each battle state.")]
        public Timer timer;

        [Header("Traits")]

        /// <summary>
        /// In single player, the player checks his traits using this list.
        /// In multiplayer, this list is copied to each player.
        /// </summary>
        [Tooltip("List of traits that the system must check to apply the bonuses of each trait.")]
        public List<Trait> TraitsToCheck;


        /// <summary>
        /// In single player, the enemy checks his traits using this list.
        /// In multiplayer this list is not used.
        /// </summary>
        [HideInInspector]
        [Tooltip("List of traits that the system must check to apply the bonuses of each trait for Team 2 members.")]
        public List<Trait> TraitsToCheckTeam2;

        /// <summary>
        /// If you want only characters that are different from each other to count towards the trait bonus.
        /// Used only in single player. In multiplayer the traits are always checked.
        /// </summary>
        [Tooltip("When checking for traits, only count characters that are different for each other.")]
        public bool TraitCheckForDistinctCharacters = true;

        /// <summary>
        /// If the enemy team also applies trait modificators to their members.
        /// Used only in single player for the enemy traits.
        /// </summary>
        [Tooltip("The enemy team also applies trait modificators to their members.")]
        public bool ApplyTeam2TraitModificators = true;

        [Header("Managers")]
        /// <summary>
        /// The shop system. It contains the current currency, the system for buying and selling characters and items, and leveling up the store.
        /// </summary>
        [Tooltip("Shop manager reference.")]
        public ShopManager shopManager;

        /// <summary>
        /// The fusion manager. When a new character is added to a team or a bench, it checks if fusions are available, and if so, performs the merge.
        /// </summary>
        [Tooltip("Fusion manager reference.")]
        public FusionManager fusionManager;


        [Header("UI Related")]

        /// <summary>
        /// List of Trait List interface of each player. If the bonus of a trait is applied, a panel describing that trait (<see cref="BattleUI.TraitUI"/>) will be added to the list.
        /// In multiplayer, The first in the list will belong to player 1, the second to player 2 and so on.
        /// </summary>
        [Tooltip("Reference to the parent UI where the UIs of the traits will be displayed.")]
        public List<TraitListUI> TeamTraitListUI;

        /// <summary>
        /// The parent transform  where the HP bars of all characters will be created.
        /// </summary>
        [Tooltip("The parent transform  where the HP bars of all characters will be created.")]
        public Transform UIBars;

        /// <summary>
        /// The interface describing the stats of the selected character.
        /// </summary>
        [Tooltip("The panel describing the character's stats.")]
        public CharacterStatsUI characterStatsUI;

        /// <summary>
        /// The interface describing the stats of the selected item.
        /// </summary>
        [Tooltip("The panel describing the item's stats.")]
        public ItemDescriptionUI itemDescriptionUI;

        /// <summary>
        /// Text containing the number of characters in play and the maximum number of characters on the battlefield.
        /// </summary>
        [Tooltip("Text containing the number of characters in play and the maximum number of characters on the battlefield.")]
        public TMPro.TextMeshProUGUI characterCountText;

        /// <summary>
        /// Panel displayed when losing a round.
        /// </summary>
        [Tooltip("Panel displayed when losing a round.")]
        public Image losePanel;

        /// <summary>
        /// Panel displayed when wining the game.
        /// </summary>
        [Tooltip("Panel displayed when wining the game.")]
        public Image winPanel;

        [Header("Damage popup")]

        /// <summary>
        /// Prefab to show the damage received.
        /// </summary>
        [Tooltip("Prefab to show the damage received.")]
        public DamagePopup damagePrefab;

        /// <summary>
        /// Color of the physical damage recieved.
        /// </summary>
        [Tooltip("Color of the physical damage recieved.")]
        public Color PhysicalDamageColor;

        /// <summary>
        /// Color of the magic damage recieved.
        /// </summary>
        [Tooltip("Color of the magic damage recieved.")]
        public Color MagicDamageColor;

        /// <summary>
        /// Default color of the effect damage recieved.
        /// </summary>
        [Tooltip("Default color of the effect damage recieved.")]
        public Color EffectColor;

        [Header("Randomness")]
        /// <summary>
        /// Seed of randomness of the game. Leave it blank if you want each game to be different.
        /// </summary>
        [Tooltip("Seed of randomness of the game. Leave it blank if you want each game to be different.")]
        public string Seed;

        /// <summary>
        /// Keep the same seed when a backup is used, so the same GameActors are displayed in the store.
        /// </summary>
        [Tooltip("Keep the same seed when a backup is used, so the same GameActors are displayed in the store.")]
        public bool KeepSeedWhenUsingBackup = true;

        /// <summary>
        /// Initializes the <see cref="traitstocheck"/> list with new instances so as not to overwrite the ScriptableObject themselves.
        /// It also initializes the first stage of <see cref="stage"/>.
        /// </summary>
        void Start()
        {
            if(!string.IsNullOrEmpty(Seed))
            {
                Random.InitState(Seed.GetHashCode());
            }

            // Makes a new Instance of each Trait so it does not override itself.
            foreach (Trait traits in TraitsToCheck)
            {
                traits.InitializeTrait();

                TraitsToCheckTeam2.Add(Instantiate(traits));
            }

            // Initialize the first battle state of the stage.
            stage.InitializeBattleStage(-1);
            stage.NextState();
        }

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
        /// Calls the Update method of the current state of the stage.
        /// It controls the text of the current characters in play and max characcters.
        /// </summary>
        void Update()
        {
            stage.GetCurrentState().Update();
            if (!NetworkManager.Singleton)
            {
                characterCountText.SetText("Characters: " + teams[0].team.Count + "/" + GetMaxCharactersInTeam());
            }
        }

        /// <summary>
        /// Set the GameCharacter <see cref="GameActors.GameCharacter.State"/> depending on whether it is on the bench (Benched status) or in play (NoTarget status).
        /// If it is in play, adds it to team list.
        /// </summary>
        /// <param name="character">GameCharacter to assign the <see cref="GameActors.GameCharacter.State"/> to.</param>
        public void SetBattleOrBenchState(GameCharacter character)
        {
            IPlayer player = null;
            Bench teamBench = TeamBenches[0];
            if (NetworkManager.Singleton)
            {
                player = IPlayer.GetPlayerById(character.OwnerClientId);
                teamBench = player.CharacterBench;
            }
            if (grid.GridCells.Contains(character.CurrentGridCell) && character.State == GameCharacter.AIState.Benched)
            {
                BenchToBattle(character, player);
            }
            else if (teamBench.GridCells.Contains(character.CurrentGridCell) && character.State != GameCharacter.AIState.Benched)
            {
                BattleToBench(character, player);
            }
        }

        /// <summary>
        /// Set the GameCharacter <see cref="GameActors.GameCharacter.State"/> to NoTarget and adds it to team list.
        /// </summary>
        void BenchToBattle(GameCharacter character, IPlayer player = null)
        {
            TraitListUI traitListUI = TeamTraitListUI[0];
            int teamIndex = 0;
            List<Trait> traitList = TraitsToCheck;
            if (player)
            {
                teamIndex = (int)player.OwnerClientId;
                traitListUI = player.TraitList;
                traitList = player.TraitsToCheckTeam;
            }
            teams[teamIndex].team.Add(character);
            character.State = GameCharacter.AIState.NoTarget;

            TraitCheck(teams[teamIndex].team, TeamBenches[teamIndex].GetGameCharacterInBench(), traitList, traitListUI);
        }

        /// <summary>
        /// Set the GameCharacter <see cref="GameActors.GameCharacter.State"/> to Benched and removes it from the team list.
        /// </summary>
        void BattleToBench(GameCharacter character, IPlayer player)
        {
            TraitListUI traitListUI = TeamTraitListUI[0];
            int teamIndex = 0;
            List<Trait> traitList = TraitsToCheck;
            if (player)
            {
                teamIndex = (int)player.OwnerClientId;
                traitListUI = player.TraitList;
                traitList = player.TraitsToCheckTeam;
            }
            teams[teamIndex].team.Remove(character);
            character.State = GameCharacter.AIState.Benched;

            TraitCheck(teams[teamIndex].team, TeamBenches[teamIndex].GetGameCharacterInBench(), traitList, traitListUI);
        }


        /// <summary>
        /// Checks if a <see cref="Stats.Trait"/> is active in the game and applies the <see cref="Stats.StatModificator"/> from the activated <see cref="Stats.TraitOption"/> to the characters in the team.
        /// </summary>
        /// <param name="team">Team being checked for trait changes.</param>
        /// <param name="benched">Benched characters.</param>
        /// <param name="traits">List of traits to be checked.</param>
        /// <param name="traitList">Trait list where the traits will be displayed.</param>
        /// <param name="changeActivation">Default true. Only check if trait is activated, not applying the effects on characters.</param>
        public void TraitCheck(List<GameCharacter> team, List<GameCharacter> benched, List<Trait> traits, TraitListUI traitList, bool changeActivation = true)
        {

            //Check for trait activation. The activated TraitOption is stored in trait.ActivateOption
            foreach (Trait trait in traits)
            {
                trait.CheckForActivation(team, TraitCheckForDistinctCharacters);
            }

            //Gets a list of all Team1 and benched characters to apply the StatModificator
            if (benched == null)
            {
                benched = new List<GameCharacter>();
            }
            List<GameCharacter> allCharacters = benched;
            allCharacters.AddRange(team);

            if (changeActivation)
            {
                foreach (GameCharacter ai in allCharacters)
                {
                    foreach (Trait trait in traits)
                    {
                        //If the ActivatedOption changed, deactivate previous traitOption and activate the new one.
                        if (trait.OptionChange())
                        {
                            trait.DeactivateOption(ai, trait.PreviousOption);
                            trait.ActivateOption(ai);
                        }
                    }
                }
            }

            //Update the trait list UI.
            traitList.UpdateList(traits);
        }

        /// <summary>
        /// Checks if a cell is the character's immediate next step. This function is used to prevent that two characters cannot move through the same cell at the same time.
        /// </summary>
        /// <param name="character">Character that checks if another character has the cell as next step.</param>
        /// <param name="gridCell">Cell to check if it is the next movement of the GameCharacter.</param>
        /// 
        public bool CellIsNextOtherCharacterMovement(GameCharacter character, GridCell gridCell)
        {
            List<GameCharacter> ais = new List<GameCharacter>(teams[0].team);
            ais.AddRange(teams[1].team);

            foreach (GameCharacter ai in ais)
            {
                GridCell cell = ai.NextMoveCell();
                if ((cell == gridCell || gridCell == ai.CurrentGridCell) && ai != character && ai.State != GameCharacter.AIState.Dead)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Returns the value of the color depending on the type of damage being inflicted.
        /// </summary>
        /// <param name="damageType">Type of the infringed damage.</param>
        public Color GetDamageColor(BattleFormulas.DamageType damageType)
        {
            switch (damageType)
            {
                case BattleFormulas.DamageType.Physical:
                    return PhysicalDamageColor;
                case BattleFormulas.DamageType.Magic:
                    return MagicDamageColor;
                case BattleFormulas.DamageType.Effect:
                    return EffectColor;
            }
            return Color.white;
        }

        /// <summary>
        /// Returns the maximum number of characters in play for a player.
        /// </summary>
        public int GetMaxCharactersInTeam()
        {
            return shopManager.shopLevelManager.CurrentLevel + InitialCharacters;
        }

        /// <summary>
        /// Try to perform a fusion of a character.
        /// </summary>
        /// <param name="scriptableCharacter">Character to fuse.</param>
        public void TryFusion(ShopCharacter scriptableCharacter)
        {
            if (fusionManager)
            {
                fusionManager.TryFusion(scriptableCharacter);
            }
        }

        /// <summary>
        /// Toogle between the Trait lists of both teams.
        /// </summary>
        public void ToogleList()
        {
            if (TeamTraitListUI[0].gameObject.activeSelf)
            {
                TeamTraitListUI[0].gameObject.SetActive(false);
                TeamTraitListUI[1].gameObject.SetActive(true);
            }
            else
            {
                TeamTraitListUI[0].gameObject.SetActive(true);
                TeamTraitListUI[1].gameObject.SetActive(false);
            }
        }
    }
}