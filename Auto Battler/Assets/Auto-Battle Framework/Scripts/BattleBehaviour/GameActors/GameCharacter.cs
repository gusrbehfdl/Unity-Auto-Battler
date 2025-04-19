using AutoBattleFramework.Battlefield;
using AutoBattleFramework.BattleUI;
using AutoBattleFramework.Movement;
using AutoBattleFramework.Shop;
using AutoBattleFramework.Skills;
using AutoBattleFramework.Stats;
using AutoBattleFramework.Utility;
using AutoBattleFramework.Formulas;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using AutoBattleFramework.Multiplayer.BattleBehaviour.Player;

namespace AutoBattleFramework.BattleBehaviour.GameActors
{
    /// <summary>
    /// Game character that can move and battle with other characters.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NetworkObject))]
    public class GameCharacter : GameActor
    {
        [Header("Character Stats and Traits")]

        /// <summary>
        /// List of the character traits.
        /// </summary>
        [Tooltip("List of character Traits.")]
        public List<Trait> traits;

        /// <summary>
        /// If recieving a <see cref="Skills.Projectile"/>, where should it aim to.
        /// </summary>
        [Tooltip("If recieving a Projectile, where should it aim to (usually the body transform).")]
        public Transform EnemyShootingPoint;

        /// <summary>
        /// Original stats of the character. Stats of the character without any <see cref="Stats.StatsModificator"/>.
        /// </summary>
        [Tooltip("Original stats of the character. Stats of the character without any stat modificator.")]
        public CharacterStats OriginalStats;

        [Header("Basic attack")]

        /// <summary>
        /// Attack effect to be applied when <see cref="Attack"/> is called in the animation event.
        /// </summary>
        [Tooltip("Attack effect to be applied when the Attack() method is called in the animation event.")]
        public IAttackEffect attackEffect;

        /// <summary>
        /// If the <see cref="attackEffect"/> shoots a <see cref="Skills.Projectile"/>, the point where the projectile should spawn.
        /// </summary>
        [Tooltip("If the Attack Effect shoots a Projectile, the point where the projectile should spawn.")]
        public Transform ProjectileShootingPoint;

        [Header("Special attack")]
        /// <summary>
        /// Attack effect to be applied when <see cref="SpecialAttack"/> is called in the animation event.
        /// </summary>
        [Tooltip("Special Attack effect to be applied when the SpecialAttack() method is called in the animation event.")]
        public IAttackEffect SpecialAttackEffect;


        [Header("UI")]

        /// <summary>
        /// Position where the <see cref="HealthBarPrefab"/> will follow the character.
        /// </summary>
        [Tooltip("Position where the Health Bar will follow the character.")]
        public Transform UIBarPosition;

        /// <summary>
        /// Offset from <see cref="UIBarPosition"/>, where the damage will be shown.
        /// </summary>
        [Tooltip("Offset from UIBarPosition, where the damage will be shown.")]
        public Vector3 OffsetDamagePosition = new Vector3(0.75f, -1.2f, 0);

        /// <summary>
        /// Prebab of the health bar.
        /// </summary>
        [Tooltip("Prefab of the health bar.")]
        public CharacterHealthUI HealthBarPrefab;

        [Header("In-game information (Read-Only)")]

        /// <summary>
        /// Current <see cref="AIState"/> of the character.
        /// </summary>
        [Tooltip("Current state of the character.")]
        [ReadOnly] public AIState State = AIState.NoTarget;

        /// <summary>
        /// The <see cref="Battlefield.GridCell"/> where the character should go.
        /// </summary>
        [Tooltip("The cell where the charater should go.")]
        [ReadOnly] public GridCell TargetGridCell;

        /// <summary>
        /// The current enemy target of the character.
        /// </summary>
        [Tooltip("The current enemy target of the character.")]
        [ReadOnly] public GameCharacter TargetEnemy;

        /// <summary>
        /// List of GridCells that the character should follow to reach <see cref="TargetGridCell"/>.
        /// </summary>
        [Tooltip("List of GridCells that the character should follow to reach the Target cell.")]
        [ReadOnly] public List<GridCell> currentPath = null;

        /// <summary>
        /// List of the <see cref="Skills.BuffEffect"/> being applied on the character.
        /// </summary>
        [Tooltip("List of the Buff Effects being applied on the character.")]
        [ReadOnly] public List<BuffEffectInfo> BuffList;

        /// <summary>
        /// List of <see cref="GameItem.itemModificator"/> of items attached to the character.
        /// </summary>
        [Tooltip("List of Item Modificators attached to the character.")]
        [ReadOnly] public List<ItemModificator> itemModificators;

        /// <summary>
        /// List of <see cref="Stats.TraitOption.modificator"/> being applied to the character.
        /// </summary>
        [Tooltip("List of Trait modificators being applied to the character.")]
        [ReadOnly] public List<StatsModificator> TraitModificators;

        /// <summary>
        /// Initials stats before a battle. Makes a save point of the character's statistics between rounds.
        /// </summary>
        [Tooltip("Initials stats before a battle.")]
        [ReadOnly] public CharacterStats InitialStats;

        /// <summary>
        /// The stats currently in use during a battle.
        /// </summary>
        [Tooltip("The stats currently in use during a battle.")]
        [ReadOnly] public CharacterStats CurrentStats;

        /// <summary>
        /// The damage done by this character is only visual. It does not do damage or activate effects.
        /// In single player, the damage can�t be visual only.
        /// </summary>
        public virtual bool DamageVisualOnly { get { return false; } }

        /// <summary>
        /// Current state of the character.
        /// </summary>
        public enum AIState
        {
            NoTarget,
            Attacking,
            Moving,
            Benched,
            Dead,
            SpecialAttacking
        }



        /// <summary>
        /// Reference to the navigation agent.
        /// </summary>
        [HideInInspector]
        public NavMeshAgent agent;

        /// <summary>
        /// Reference to the character animator.
        /// </summary>
        [HideInInspector]
        public Animator animator;

        /// <summary>
        /// True if the <see cref="attackEffect"/> animation consists of two attacks (like holding and releasing an arrow, or a double attack).
        /// </summary>
        bool doubleAnimationAttack;

        /// <summary>
        /// True if the <see cref="SpecialAttackEffect"/> animation consists of two attacks (like holding and releasing an arrow, or a double attack).
        /// </summary>
        bool doubleAnimationSpecialAttack;

        /// <summary>
        /// Time to look at the enemy.
        /// </summary>
        [HideInInspector]
        public float FaceEnemyTime = 0.25f;

        /// <summary>
        /// Instance of <see cref="HealthBarPrefab"/>.
        /// </summary>
        CharacterHealthUI HealthBarInstance;

        /// <summary>
        /// World position of the character before starting <see cref="States.FightState"/>. 
        /// </summary>
        [HideInInspector]
        public Vector3 StartingFightPosition;

        // Start is called before the first frame update
        protected void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            GetCurrentCell();
            animator = GetComponent<Animator>();
            if (!UIBarPosition)
            {
                UIBarPosition = transform;
            }
            HealthBarInstance = Instantiate(HealthBarPrefab);
            HealthBarInstance.Initialize(this);
            CharacterEnergyUI energyBar = HealthBarInstance.GetComponent<CharacterEnergyUI>();
            if (energyBar)
                energyBar.Initialize(this);

            ShowUIBar(false);

            if (attackEffect.DoubleAnimation)
            {
                doubleAnimationAttack = true;
            }
            if (SpecialAttackEffect)
            {
                if (SpecialAttackEffect.DoubleAnimation)
                {
                    doubleAnimationSpecialAttack = true;
                }
            }
            if (attackEffect)
                attackEffect = Instantiate(attackEffect);
            if (SpecialAttackEffect)
                SpecialAttackEffect = Instantiate(SpecialAttackEffect);
            InitialStats = OriginalStats.Copy();
            CurrentStats = InitialStats.Copy();
            CurrentStats.Energy = 0;

            foreach (ItemModificator item in itemModificators.ToList())
            {
                item.AddStats(this, true);
            }

            foreach (StatsModificator trait in TraitModificators.ToList())
            {
                trait.AddStats(this, true);
            }
        }

        private void Reset()
        {
            HealthBarPrefab = AutoBattleSettings.GetOrCreateSettings().defaultHealthBar;
            GetComponent<NavMeshAgent>().enabled = false;
            gameObject.layer = LayerMask.NameToLayer("CharacterAI");
            Debug.Log("Do not forget that Attack and Special animations needs an event that triggers the Attack() and SpecialAttack() methods.");
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            Battle.Instance.stage.GetCurrentState().CharacterAIUpdate(this);

            if (State == AIState.Benched || State == AIState.Dead)
            {
                ShowUIBar(false);
            }
            else
            {
                ShowUIBar(true);
            }
        }

        /// <summary>
        /// Show or hide the <see cref="HealthBarInstance"/> based on the Characters state.
        /// </summary>
        protected virtual void RefreashUI()
        {
            if (State == AIState.Benched || State == AIState.Dead)
            {
                ShowUIBar(false);
            }
            else
            {
                ShowUIBar(true);
            }
        }

        /// <summary>
        /// Show or hide the <see cref="HealthBarInstance"/>.
        /// </summary>
        /// <param name="value">Show or hide the bar.</param>
        public void ShowUIBar(bool value)
        {
             if(HealthBarInstance)  HealthBarInstance.gameObject.SetActive(value);
        }

        /// <summary>
        /// Returns the next cell where the character will move to. 
        /// </summary>
        /// <returns>Next cell where the character will move to</returns>
        public GridCell NextMoveCell()
        {
            int currentPathIndex = currentPath.IndexOf(CurrentGridCell);
            if (currentPathIndex == -1)
            {
                return null;
            }
            if (currentPathIndex + 1 < currentPath.Count)
            {
                return currentPath[currentPathIndex + 1];
            }
            return null;
        }

        /// <summary>
        /// Start the drag.
        /// </summary>
        public override void OnMouseDownAction()
        {
            isDragged = true;
        }

        /// <summary>
        /// Allows an character, to be moved within the <see cref="Battle.TeamBenches[0]"/>, within the <see cref="Battle.teams[0].team"/> inside the battle grid, or to exchange its position with that of another character.
        /// In Android, if the character is dragged to the same cell, shows the description of the character.
        /// </summary>
        public override void OnMouseUpAction()
        {
            int teamIndex = GetTeamIndex();
            if (teamIndex == -1)
            {
                return;
            }

            if (CurrentDragCell)
            {
                //If its a grid cell and the battle state allow trades. Or if its a bench cell.
                if ((!CurrentDragCell.shopItem || CurrentDragCell.shopItem == this) && (CurrentDragCell.CanPlaceCharacter == teamIndex) && (Battle.Instance.grid.GridCells.Contains(CurrentDragCell)
                    && Battle.Instance.stage.GetCurrentState().AllowFieldDrag(this)
                    || Battle.Instance.TeamBenches[teamIndex].GridCells.Contains(CurrentDragCell)))
                {

                    //If team size is less that max, or the character is already in the field, or the character is moved to the bench.
                    if ((Battle.Instance.teams[(int)OwnerClientId].team.Count < Battle.Instance.GetMaxCharactersInTeam() || Battle.Instance.teams[(int)OwnerClientId].team.Contains(this) || Battle.Instance.TeamBenches[teamIndex].GridCells.Contains(CurrentDragCell)) && CurrentDragCell.shopItem != this)
                    {
                        GetCurrentCell();
                        MoveCharacterTo(CurrentGridCell);
                    }
                    else
                    {
                        CancelDrag();
#if UNITY_ANDROID
                        ShowUI(true);
#endif
                    }
                }
                // If its a grid cell that contains another character, swap positions.
                else if (CurrentDragCell.shopItem && (CurrentDragCell.CanPlaceCharacter == teamIndex) &&
                    CurrentDragCell.shopItem != this &&
                    (Battle.Instance.grid.GridCells.Contains(CurrentDragCell)
                    && Battle.Instance.stage.GetCurrentState().AllowFieldDrag(this)
                    || Battle.Instance.TeamBenches[teamIndex].GridCells.Contains(CurrentDragCell)))
                {
                    GridCell c1 = CurrentGridCell;
                    GridCell c2 = CurrentDragCell;
                    SwapCharactersInCells(c1, c2);
                }
                else
                {
                    CancelDrag();
#if UNITY_EDITOR
                    ShowUI(true);
#endif
                }
                Battle.Instance.SetBattleOrBenchState(this);
            }
        }

        /// <summary>
        /// Swap the position of two characters in the <see cref="Battlefield.BattleGrid"/>.
        /// </summary>
        /// <param name="c1">Cell of the first character.</param>
        /// <param name="c2">Cell of the second character.</param>
        protected void SwapCharactersInCells(GridCell c1, GridCell c2)
        {
            GameCharacter character2 = CurrentDragCell.shopItem as GameCharacter;

            character2.CurrentGridCell = c1;
            character2.MoveCharacterTo(c1);

            CurrentGridCell = c2;
            MoveCharacterTo(c2);

            c1.shopItem = character2;
            c2.shopItem = this;

            Battle.Instance.SetBattleOrBenchState(character2);
        }

        /// <summary>
        /// Force the movement of a character to a given cell.
        /// </summary>
        /// <param name="cell">Cell where the character will move.</param>
        protected virtual void MoveCharacterTo(GridCell cell)
        {
            agent.enabled = true;
            Vector3 pos = cell.transform.position;
            pos.y = transform.position.y;
            agent.Warp(pos);
            isDragged = false;
            GetCurrentCell();
            Battle.Instance.SetBattleOrBenchState(this);

            if (CurrentDragCell)
                CurrentDragCell.SetDragEffect(false);

        }

        /// <summary>
        /// Cancel the drag effect on the character.
        /// </summary>
        public void CancelDrag()
        {
            if (isDragged)
            {
                agent.enabled = true;
                //transform.position = getInitialPosition();
                agent.Warp(getInitialPosition());
                GetCurrentCell();
                Battle.Instance.SetBattleOrBenchState(this);
                isDragged = false;
                if (CurrentDragCell)
                {
                    CurrentDragCell.SetDragEffect(false);
                }
            }
        }

        /// <summary>
        /// Allows the movement of the character between cells.
        /// </summary>
        public override void OnDragObjectAction()
        {
            if (CanBeMoved)
            {
                if (State == AIState.Benched || Battle.Instance.stage.GetCurrentState().AllowFieldDrag(this))
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
                            agent.enabled = false;

                            RaycastHit hitDown;

                            //In case the center of the transform is at the feet and coincides with the navigation layer, a small offset is applied on the y-axis.
                            Vector3 yOffset = new Vector3(0, 0.2f, 0);
                            Vector3 yPosition = transform.position + yOffset;

                            if (Physics.Raycast(yPosition, Vector3.down, out hitDown, 10, LayerMask.GetMask("GridCell"), QueryTriggerInteraction.Collide))
                            {
                                if (hitDown.collider.GetComponent<GridCell>())
                                {
                                    if (CurrentDragCell != null)
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
        /// Buys a character and places it on the <see cref="Battlefield.Bench"/> if there is enough space in it.
        /// </summary>
        /// <param name="shopItem">Game character that will be bought.</param>
        /// <returns>Bought character.</returns>
        public override GameActor Buy(GameActor shopItem)
        {
            GridCell cell = Battle.Instance.TeamBenches[0].GridCells.Where(x => x.shopItem == null).FirstOrDefault();
            if (cell)
            {
                GameCharacter character = Instantiate(shopItem, cell.transform.position, Quaternion.identity) as GameCharacter;
                SetVariablesOnSpawn(character);
                return character;
            }
            return null;
        }

        /// <summary>
        /// When spawned, apply traits, set state, and rotation.
        /// </summary>
        /// <param name="character">Character spawned.</param>
        /// <param name="player">Multiplayer only. Player prefab in multiplayer.</param>
        protected virtual void SetVariablesOnSpawn(GameCharacter character, IPlayer player = null)
        {
            character.gameObject.SetActive(true);
            GridCell cell = Battle.Instance.TeamBenches[0].GridCells.Where(x => x.shopItem == null).FirstOrDefault();
            List<Trait> tList = Battle.Instance.TraitsToCheck;
            character.ApplyTraitsToNewCharacter(tList);

            if (Battle.Instance.grid.GridCells.Contains(character.CurrentGridCell))
            {
                character.State = AIState.NoTarget;
            }
            else
            {
                character.State = AIState.Benched;
            }

                // Fixed the display problem of extra rotation
            Quaternion rotation = Quaternion.LookRotation(
                character.transform.position - transform.position,
                transform.TransformDirection(Vector3.up)
            );
            transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
            
            //Set variables
            cell.shopItem = character;
            AfterBought();

            ShowUIBar(true);
        }


        /// <summary>
        /// Apply the traits effects to a recently bought character.
        /// </summary>
        /// <param name="character">Character to apply the traits effects.</param>
        /// <param name="traits">List of traits.</param>
        public void ApplyTraitsToNewCharacter(List<Trait> traits)
        {
            foreach (Trait trait in traits)
            {
                trait.ActivateOption(this);
            }
        }

        /// <summary>
        /// Perform a fusion check after being bought
        /// </summary>
        /// <param name="shopItem"></param>
        public override void AfterBought()
        {
            Battle.Instance.TryFusion(info as ShopCharacter);
        }

        /// <summary>
        /// Change the state of the character and sets the <see cref="animator"/> variables.
        /// </summary>
        /// <param name="state">State of the character.</param>
        public void ChangeState(AIState state)
        {
            State = state;

            switch (State)
            {
                case AIState.NoTarget:
                    SetAnimatorVariables(true, false, false, false, false);
                    break;
                case AIState.Moving:
                    SetAnimatorVariables(false, false, true, false, false);
                    break;
                case AIState.Attacking:
                    SetAnimatorVariables(false, true, false, false, false);
                    break;
                case AIState.Dead:
                    SetAnimatorVariables(false, false, false, true, false);
                    break;
                case AIState.SpecialAttacking:
                    SetAnimatorVariables(false, false, false, false, true);
                    break;
            }
        }

        /// <summary>
        /// Set the animator variables.
        /// </summary>
        void SetAnimatorVariables(bool idle, bool attack, bool move, bool death, bool special)
        {
            if (animator)
            {
                animator.SetBool("Idle", idle);
                animator.SetBool("Attack", attack);
                animator.SetBool("Move", move);
                animator.SetBool("Shoot", doubleAnimationAttack);
                animator.SetBool("SpecialShoot", doubleAnimationSpecialAttack);
                animator.SetBool("Death", death);
                animator.SetBool("Special", special);
            }
        }

        /// <summary>
        /// Check if the character is in range from its current enemy.
        /// </summary>
        /// <returns>If the character is in range from its current enemy.</returns>
        public bool InAttackRange()
        {
            return CurrentGridCell.DistanceToOtherCell(TargetEnemy.CurrentGridCell) <= CurrentStats.Range + 1;
        }

        /// <summary>
        /// Make the character look at the current enemy.
        /// </summary>
        public void FaceTarget()
        {
            Vector3 direction = (TargetEnemy.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = lookRotation;
        }

        /// <summary>
        /// Method that is triggered from the animation event. Performs a basic attack.
        /// </summary>
        public virtual void Attack()
        {

            attackEffect.Attack(this, ProjectileShootingPoint);

            //Triggers all on attack effects of all modificators and buffs attached (Only trigers OnAttackEffects, On hit effects are triggered when the character makes some damage)
            foreach (StatsModificator item in itemModificators)
            {
                foreach (IAttackEffect effect in item.attackEffects)
                {
                    effect.Attack(this, ProjectileShootingPoint);
                }
            }

            foreach (StatsModificator trait in TraitModificators)
            {
                foreach (IAttackEffect effect in trait.attackEffects)
                {
                    effect.Attack(this, ProjectileShootingPoint);
                }
            }

            foreach (BuffEffectInfo info in BuffList.ToList())
            {
                foreach (IAttackEffect effect in info.buff.modificator.attackEffects)
                {
                    effect.Attack(this, ProjectileShootingPoint);
                }
            }
        }

        /// <summary>
        /// Method that is activated from the animation event once the <see cref="Stats.CharacterStats.Energy"/> reaches its maximum. Performs a special attack.
        /// </summary>
        public virtual void SpecialAttack()
        {
            if (SpecialAttackEffect && ProjectileShootingPoint)
            {
                SpecialAttackEffect.Attack(this, ProjectileShootingPoint);

                CurrentStats.Energy = 0;
            }
        }

        /// <summary>
        /// When right click, show the character UI.
        /// </summary>
        public override void OnMouseOverAction()
        {
            if (Input.GetMouseButtonUp(1))
            {
                ShowUI(true);
            }

        }

        /// <summary>
        /// Returns the cell closest to the current <see cref="GameActor.CurrentGridCell"/>, but different from the <see cref="GameActor.CurrentGridCell"/>.
        /// </summary>
        /// <returns>Cell closest to the current cell, but different from the current cell.</returns>
        public GridCell GetClosestCellDifferentToCurrent()
        {
            float distance = float.MaxValue;
            GridCell closest = null;
            foreach (GridCell cell in Battle.Instance.grid.GridCells)
            {
                if (cell != CurrentGridCell && cell.shopItem == null)
                {
                    float cDist = Vector3.Distance(transform.position, cell.transform.position);
                    if (cDist < distance)
                    {
                        distance = cDist;
                        closest = cell;
                    }
                }
            }
            return closest;
        }

        /// <summary>
        /// Destroy the health bar when the character is destroyed.
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();
            if (HealthBarInstance)
            {
                Destroy(HealthBarInstance.gameObject);
            }
        }

        /// <summary>
        /// the character and gain <see cref="Shop.ShopManager.currency"/>.
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

                Battle.Instance.teams[0].team.Remove(this);
                Battle.Instance.TraitCheck(Battle.Instance.teams[0].team, Battle.Instance.TeamBenches[0].GetGameCharacterInBench(), Battle.Instance.TraitsToCheck, Battle.Instance.TeamTraitListUI[0]);
                Destroy(this.gameObject);


            }
            else
            {
                CancelDrag();
            }
        }




        /// <summary>
        /// Shows the <see cref="BattleUI.CharacterStatsUI"/> that describes the character stats. 
        /// </summary>
        /// <param name="show">Show or hide the UI</param>
        /// <returns>GameObject of the CharacterStatsUI.</returns>
        public override GameObject ShowUI(bool show)
        {
            if (show)
            {
                Battle.Instance.characterStatsUI.SetUI(this);
                Battle.Instance.characterStatsUI.ShowUI(true);
            }
            else
            {
                Battle.Instance.characterStatsUI.ShowUI(false);
            }
            return Battle.Instance.characterStatsUI.gameObject;
        }

        /// <summary>
        /// Get the color of the damage.
        /// </summary>
        /// <param name="damageType">Type of damage infringed.</param>
        /// <returns>Color of the text damage.</returns>
        public Color GetDamageColor(BattleFormulas.DamageType damageType)
        {
            return Battle.Instance.GetDamageColor(damageType);

        }

        /// <summary>
        /// Creates a text that shows the damage infringed to another character.
        /// </summary>
        /// <param name="text">Number of the damage infringed.</param>
        /// <param name="color">Color of the text.</param>
        /// <returns>Damage text</returns>
        public virtual DamagePopup CreateDamagePopup(string text, Color color)
        {
            DamagePopup popup = Instantiate(Battle.Instance.damagePrefab);
            Vector3 pos = UIBarPosition.position;
            pos += OffsetDamagePosition;
            popup.transform.position = pos;
            popup.displayText = text;
            popup.Setcolor(color);
            return popup;
        }

        /// <summary>
        /// Unequips an item's modifier.
        /// </summary>
        /// <param name="itemModificatorIndex">Index of the item modificator.</param>
        public virtual void UnequipItemModificator(int itemModificatorIndex)
        {
            GameItem gItem = itemModificators[itemModificatorIndex].scriptableShopItem.shopItem.GetComponent<GameItem>();
            itemModificators[itemModificatorIndex].RemoveItemModificator(this);
            gItem.Buy(gItem);
        }
    }

}