using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Shop;
using AutoBattleFramework.Shop.ShopGUI;
using AutoBattleFramework.Stats;
using AutoBattleFramework.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AutoBattleFramework.BattleUI
{
    /// <summary>
    /// Allows the visualization of a character큦 <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/>.
    /// </summary>
    public class CharacterStatsUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        /// <summary>
        /// Characte rreference
        /// </summary>
        [HideInInspector]
        public GameCharacter character;

        [Header("Position Settings")]

        /// <summary>
        /// Set to true if you want the panel to be displayed on the selected character.
        /// </summary>
        [Tooltip("The panel is displayed over the selected character.")]
        public bool MoveToCharacterPosition = true;

        [Header("Images")]

        /// <summary>
        /// Reference of the character image.
        /// </summary>
        [Tooltip("Image of the character.")]
        public Image CharacterImage;

        /// <summary>
        /// Reference of the character Health bar.
        /// </summary>
        [Tooltip("Reference to the character Health bar fill.")]
        public Image CharacterHPBar;

        /// <summary>
        /// Reference of the character Energy bar.
        /// </summary>
        [Tooltip("Reference to the character Energy bar fill.")]
        public Image CharacterEnergyBar;

        /// <summary>
        /// Reference to the image that represents the character큦 <see cref="BattleBehaviour.GameActors.GameCharacter.SpecialAttack"/>.
        /// </summary>
        [Tooltip("Reference to the image that represents the character큦 Special Attack.")]
        public Image SpecialImage;

        /// <summary>
        /// Reference to the image that represents the character큦 <see cref="BattleBehaviour.GameActors.GameCharacter.SpecialAttack"/> panel description.
        /// </summary>
        [Tooltip("Reference to the image that represents the character큦 Special Attack.")]
        public Image SpecialPanelImage;

        [Header("Texts")]

        /// <summary>
        /// Display the character큦 name.
        /// </summary>
        [Tooltip("Display the character큦 name.")]
        public TMPro.TextMeshProUGUI CharacterName;

        /// <summary>
        /// Display the <see cref="BattleBehaviour.GameActors.GameCharacter.SpecialAttackEffect"/> description.
        /// </summary>
        [Tooltip("Display the character큦 name.")]
        public TMPro.TextMeshProUGUI SpecialAttackDescriptionText;

        /// <summary>
        /// Reference to <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> damage text.
        /// </summary>
        [Tooltip("Reference to the damage stat text.")]
        public TMPro.TextMeshProUGUI DamageText;

        /// <summary>
        /// Reference to <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> magic damage text.
        /// </summary>
        [Tooltip("Reference to the magic damage stat text.")]
        public TMPro.TextMeshProUGUI MagicDamageText;

        /// <summary>
        /// Reference to <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> defense text.
        /// </summary>
        [Tooltip("Reference to the defense stat text.")]
        public TMPro.TextMeshProUGUI DefText;

        /// <summary>
        /// Reference to <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> magic defense text.
        /// </summary>
        [Tooltip("Reference to the magic defense stat text.")]
        public TMPro.TextMeshProUGUI MagicDefText;

        /// <summary>
        /// Reference to <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> attack speed text.
        /// </summary>
        [Tooltip("Reference to the attack speed stat text.")]
        public TMPro.TextMeshProUGUI AtkSpeedText;

        /// <summary>
        /// Reference to <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> range text.
        /// </summary>
        [Tooltip("Reference to the range stat text.")]
        public TMPro.TextMeshProUGUI RangeText;

        /// <summary>
        /// Reference to <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> critical chance text.
        /// </summary>
        [Tooltip("Reference to the critical chance stat text.")]
        public TMPro.TextMeshProUGUI CritChanceText;

        /// <summary>
        /// Reference to <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> critical damage text.
        /// </summary>
        [Tooltip("Reference to the critical damage stat text.")]
        public TMPro.TextMeshProUGUI CritDamageText;

        /// <summary>
        /// Reference to text where <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> health and <see cref="BattleBehaviour.GameActors.GameCharacter.InitialStats"/> health will be displayed.
        /// </summary>
        [Tooltip("Reference to the current health text.")]
        public TMPro.TextMeshProUGUI HPText;

        /// <summary>
        /// Reference to text where <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> energy and <see cref="BattleBehaviour.GameActors.GameCharacter.InitialStats"/> energy will be displayed.
        /// </summary>
        [Tooltip("Reference to the current energy text.")]
        public TMPro.TextMeshProUGUI EnergyText;


        [Header("Transforms")]

        /// <summary>
        /// Reference to the list of traits.
        /// </summary>
        [Tooltip("Reference to the list of traits.")]
        public Transform TraitList;

        /// <summary>
        /// Scroll content that will display the items that the character has equipped.
        /// </summary>
        [Tooltip("Reference to the scroll content that will display the items that the character has equipped.")]
        public Transform itemScrollContent;

        [Header("Prefabs")]

        /// <summary>
        /// Prefab of the trait UI that will be Instantiated inside <see cref="TraitList"/>.
        /// </summary>
        [Tooltip("Prefab of the trait UI that will be Instantiated inside the Trait List.")]
        public TraitStatsUI TraitStatsPrefab;

        /// <summary>
        /// Prefab of the item displayed in <see cref="itemScrollContent"/>.
        /// </summary>
        [Tooltip("Prefab of the item displayed in the scroll content.")]
        public EquippedItemDescriptionUI itemImagePrefab;

        /// <summary>
        /// List of character큦 traits 
        /// </summary>
        [HideInInspector]
        public List<TraitStatsUI> traitstats;

        /// <summary>
        /// List of items inside <see cref="itemScrollContent"/>.
        /// </summary>
        [HideInInspector]
        public List<EquippedItemDescriptionUI> items;

        /// <summary>
        /// The cursor is over the panel.
        /// </summary>
        bool OverPanel = false;

        private void Start()
        {

        }

        void Update()
        {
            if (character && gameObject.activeSelf)
            {
                CharacterHPBar.fillAmount = character.CurrentStats.Health / (float)character.InitialStats.Health;
                CharacterEnergyBar.fillAmount = character.CurrentStats.Energy / (float)character.InitialStats.Energy;
                SetStats();
                HideIfClickedOutside();
            }
        }

        /// <summary>
        /// Sets the character reference and sets the stats values in all texts.
        /// </summary>
        /// <param name="character"></param>
        public void SetUI(GameCharacter character)
        {
            this.character = character;
            SetCharacterStatsUI();
        }

        /// <summary>
        /// If true, show the stats of the character.
        /// </summary>
        /// <param name="show">Show the stats of the characters. </param>
        public void ShowUI(bool show)
        {
            if (!show)
            {
                OverPanel = false;
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(gameObject);

                if (MoveToCharacterPosition)
                {
                    RectTransform rect = GetComponent<RectTransform>();
                    Vector2 coordinates = Camera.main.WorldToScreenPoint(character.transform.position);
                    rect.position = coordinates;
                    rect.rotation = Quaternion.identity;
                    UIUtility.KeepInsideScreen(rect);
                }                
            }
        }

        /// <summary>
        /// Set the stats inside the panel.
        /// </summary>
        void SetCharacterStatsUI()
        {
            foreach (TraitStatsUI ui in traitstats)
            {
                Destroy(ui.gameObject);
            }
            traitstats.Clear();

            SetSpecialImage();
            if (character.info.descriptionImage)
            {
                CharacterImage.sprite = character.info.descriptionImage;
            }
            else
            {
                CharacterImage.sprite = character.info.itemImage;
            }
            
            CharacterHPBar.fillAmount = (float)character.CurrentStats.Health / (float)character.InitialStats.Health;
            CharacterEnergyBar.fillAmount = (float)character.CurrentStats.Energy / (float)character.InitialStats.Energy;

            CharacterName.SetText(character.info.itemName);

            foreach (Trait trait in character.traits)
            {
                TraitStatsUI traitUI = Instantiate(TraitStatsPrefab, TraitList);
                traitUI.SetUI(trait);
                traitstats.Add(traitUI);
            }

            SetItems();
            SetStats();
        }

        /// <summary>
        /// Set the image of the <see cref="GameCharacter.SpecialAttack"/>.
        /// </summary>
        void SetSpecialImage()
        {
            if (character.SpecialAttackEffect)
            {
                SpecialImage.sprite = character.SpecialAttackEffect.EffectImage;
                SpecialImage.transform.parent.gameObject.SetActive(true);
                SpecialImage.gameObject.SetActive(true);
                SpecialAttackDescriptionText.SetText(character.SpecialAttackEffect.EffectDescription);
            }
            else
            {
                SpecialImage.transform.parent.gameObject.SetActive(false);
                SpecialImage.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// If clicked outside the panel, hide it.
        /// </summary>
        private void HideIfClickedOutside()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                if (!OverPanel || character.IsBeingDragged())
                {
                    gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Set the images of the items inside the scroll panel.
        /// </summary>
        void SetItems()
        {
            foreach (EquippedItemDescriptionUI itemImage in items)
            {
                Destroy(itemImage.gameObject);
            }
            items.Clear();

            foreach (ItemModificator item in character.itemModificators)
            { 
                EquippedItemDescriptionUI ui = Instantiate(itemImagePrefab, itemScrollContent);
                ui.item = item.scriptableShopItem as ShopGameItem;
                ui.GetComponent<Image>().sprite = ui.item.itemImage;
                items.Add(ui);
            }
        }

        /// <summary>
        /// Set the text of all stats.
        /// </summary>
        void SetStats()
        {
            DamageText.SetText(character.CurrentStats.Damage.ToString());
            MagicDamageText.SetText(character.CurrentStats.MagicDamage.ToString());
            DefText.SetText(character.CurrentStats.Defense.ToString());
            MagicDefText.SetText(character.CurrentStats.MagicDefense.ToString());
            AtkSpeedText.SetText(character.CurrentStats.AttackSpeed.ToString());
            RangeText.SetText(character.CurrentStats.Range.ToString());
            CritChanceText.SetText((character.CurrentStats.CriticalProbability * 100).ToString() + "%");
            CritDamageText.SetText((character.CurrentStats.CriticalDamage * 100).ToString() + "%");
            HPText.SetText(character.CurrentStats.Health.ToString() + "/" + character.InitialStats.Health.ToString());
            EnergyText.SetText(character.CurrentStats.Energy.ToString() + "/" + character.InitialStats.Energy.ToString());
        }

        /// <summary>
        /// Prevents the panel to go off-screen.
        /// </summary>
        /// <param name="newPos">Position of the panel.</param>
        void KeepInsideScreen(Vector3 newPos)
        {
            RectTransform rect = GetComponent<RectTransform>();
            RectTransform CanvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

            float minX = (CanvasRect.sizeDelta.x - rect.sizeDelta.x) * -0.5f;
            float maxX = (CanvasRect.sizeDelta.x - rect.sizeDelta.x) * 0.5f;
            float minY = (CanvasRect.sizeDelta.y - rect.sizeDelta.y) * -0.5f;
            float maxY = (CanvasRect.sizeDelta.y - rect.sizeDelta.y) * 0.5f;

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

            rect.position = newPos;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetOverPanel(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetOverPanel(false);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            SetOverPanel(true);
        }

        /// <summary>
        /// The components within the panel use this function to indicate that the cursor is still on the panel when they are over them.
        /// </summary>
        /// <param name="value">Over the panel.</param>
        public void SetOverPanel(bool value)
        {
            OverPanel = value;
        }
    }
}