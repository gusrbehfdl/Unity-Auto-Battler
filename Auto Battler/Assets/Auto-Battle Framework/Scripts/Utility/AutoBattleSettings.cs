using UnityEngine;
using UnityEditor;
using AutoBattleFramework.BattleUI;
using AutoBattleFramework.Shop.ShopGUI;

namespace AutoBattleFramework.Utility
{
    /// <summary>
    /// Framework options. Helps editor methods to put references in the Inspector automatically.
    /// </summary>
    // Uncomment this option in case of unintentional deletion from the options file.
    //[CreateAssetMenu(fileName = "AutoBattleSettings", menuName = "Auto-Battle Framework/Settings", order = 1)]
    public class AutoBattleSettings : ScriptableObject
    {
        /// <summary>
        /// Address within the project where the option object will be included. Change the address in case of moving the object to another folder.
        /// </summary>
        public const string SettingsPath = "Assets/Auto-Battle Framework/Scripts/Editor/Settings/AutoBattleSettings.asset";

        /// <summary>
        /// Default health bar used when creating a new character in the editor.
        /// </summary>
        public CharacterHealthUI defaultHealthBar;

        /// <summary>
        /// Default panel to be displayed in the shop when creating a new character in the editor.
        /// </summary>
        public ShopItemUI defaultCharacterShopItemUI;

        /// <summary>
        /// Default animator controler used when creating a new character in the editor.
        /// </summary>
        public RuntimeAnimatorController defaultAnimatorController;

        /// <summary>
        /// Default texture used in <see cref="BattleBehaviour.ScriptableBattlePosition"/> editor when the <see cref="Battlefield.BattleGrid"/> is <see cref="Battlefield.BattleGrid.GridType.Hex"/>.
        /// </summary>
        public Texture defaultHexTexture;

        /// <summary>
        /// Default texture used in <see cref="BattleBehaviour.ScriptableBattlePosition"/> editor when the <see cref="Battlefield.BattleGrid"/> is <see cref="Battlefield.BattleGrid.GridType.Hex"/>.
        /// </summary>
        public Texture defaultSquaredTexture;

        /// <summary>
        /// Default Stage Empty prefab.
        /// </summary>
        public GameObject defaultStageEmptyUIPrefab;

        /// <summary>
        /// Default size of cells in the <see cref="BattleBehaviour.ScriptableBattlePosition"/> Inspector.
        /// </summary>
        [Range(20f, 120f)]
        public float BattlePositionEditorSize = 60f;

        /// <summary>
        /// Default number of same characters to perform a fusion. Used when creating an <see cref="BattleBehaviour.Fusion.GameCharacterFusion"/> to level up characters using the menu action shortcuts.
        /// </summary>
        public int NumberToFusion = 3;

        /// <summary>
        /// Returns the framework options. If they do not exist, create the new file at <see cref="SettingsPath"/> address.
        /// </summary>
        /// <returns>Framework options object.</returns>
        public static AutoBattleSettings GetOrCreateSettings()
        {
#if UNITY_EDITOR
            var settings = AssetDatabase.LoadAssetAtPath<AutoBattleSettings>(SettingsPath);
            if (settings == null)
            {
                settings = CreateInstance<AutoBattleSettings>();
                AssetDatabase.CreateAsset(settings, SettingsPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
#endif

#pragma warning disable CS0162
            return null;
#pragma warning restore CS0162
        }
    }
}