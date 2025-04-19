using AutoBattleFramework.BattleBehaviour.Fusion;
using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Shop;
using AutoBattleFramework.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AutoBattleFramework.EditorScripts
{
    /// <summary>
    /// Create shortcuts for framework functionalities, such as creating the necessary structures for quick character creation.
    /// </summary>
    public class NGO_MenuActions
    {
        /// <summary>
        /// Create the Scriptable Object and Animator for a character.
        /// </summary>
        [MenuItem("Assets/Auto-Battle Framework/Create Shop Character")]
        private static void CreateShopCharacter()
        {
            ShopCharacter asset = ScriptableObject.CreateInstance<ShopCharacter>();

            GameObject selected = Selection.activeObject as GameObject;
            GameActor character = selected.GetComponent<GameActor>();
            asset.itemName = Selection.activeObject.name;
            asset.itemID = Selection.activeObject.name;
            asset.shopItem = character;
            character.info = asset;

            string parentPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            parentPath = parentPath.Replace(Selection.activeObject.name + ".prefab", "");

            if (character is GameCharacter)
            {
                string aocPath = CreateOverrideController(asset, asset.itemName, parentPath);
                AnimatorOverrideController aoc = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(aocPath);
                character.GetComponent<Animator>().runtimeAnimatorController = aoc;
            }

            AssetDatabase.CreateAsset(asset, parentPath + asset.itemName + "ShopItem.asset");
            AssetDatabase.SaveAssets();
            PrefabUtility.SavePrefabAsset(character.gameObject);

            EditorUtility.FocusProjectWindow();

            

            Selection.activeObject = asset;
        }

        /// <summary>
        /// Check if the selected object is a Game Character.
        /// </summary>
        /// <returns>True if a Game Character is selected.</returns>
        [MenuItem("Assets/Auto-Battle Framework/Create Shop Character", true)]
        private static bool CheckIfGameCharacter()
        {
            GameObject selected = Selection.activeObject as GameObject;
            if (!selected)
                return false;
            if (selected.GetComponent<GameCharacter>())
                return true;
            return false;
        }

        /// <summary>
        /// Create a new Animator Override for the selected Character.
        /// </summary>
        /// <param name="item">Game Character selected.</param>
        /// <param name="name">Name of the character.</param>
        /// <param name="parent">Parent folder path.</param>
        /// <returns>Path to the created animator override.</returns>
        private static string CreateOverrideController(ScriptableShopItem item, string name, string parent)
        {
            AnimatorOverrideController aoc = new AnimatorOverrideController();
            aoc.name = name + "AOC";
            aoc.runtimeAnimatorController = AutoBattleSettings.GetOrCreateSettings().defaultAnimatorController;

            string path = parent + aoc.name + ".overrideController";

            AssetDatabase.CreateAsset(aoc, parent + aoc.name + ".overrideController");

            return path;
        }

        /// <summary>
        /// Create a Game Character Fusion for leveling up characters.
        /// </summary>
        [MenuItem("Assets/Auto-Battle Framework/Create Fusion Level Up")]
        private static void CreateFusionLevelUp()
        {
            GameCharacterFusion asset = ScriptableObject.CreateInstance<GameCharacterFusion>();

            GameObject selected = Selection.activeObject as GameObject;
            GameCharacter ai = selected.GetComponent<GameCharacter>();

            asset.CharactersToFusion = new List<ShopCharacter>();

            for(int i=0;i< AutoBattleSettings.GetOrCreateSettings().NumberToFusion; i++)
            {
                ShopCharacter sc = ai.info as ShopCharacter;
                asset.CharactersToFusion.Add(sc);
            }

            string parentPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            parentPath = parentPath.Replace(Selection.activeObject.name + ".prefab", "");

            AssetDatabase.CreateAsset(asset, parentPath + selected.name + "FusionLevelUp.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        /// <summary>
        /// Check if the selected object is a Game Character.
        /// </summary>
        /// <returns>True if a Game Character is selected.</returns>
        [MenuItem("Assets/Auto-Battle Framework/Create Fusion Level Up", true)]
        private static bool CheckIfGameCharacter2()
        {
            return CheckIfGameCharacter();
        }

        /// <summary>
        /// Check if the selected object is a Game Item.
        /// </summary>
        /// <returns>True if a Game Item is selected.</returns>
        [MenuItem("Assets/Auto-Battle Framework/Create Shop Game Item", true)]
        private static bool CheckIfGameItem()
        {
            GameObject selected = Selection.activeObject as GameObject;
            if (!selected)
                return false;
            if (selected.GetComponent<GameItem>())
                return true;
            return false;
        }

        /// <summary>
        /// Create the Scriptable Object for an item..
        /// </summary>
        [MenuItem("Assets/Auto-Battle Framework/Create Shop Game Item")]
        private static void CreateShopGameItem()
        {
            ShopGameItem asset = ScriptableObject.CreateInstance <ShopGameItem>();

            GameObject selected = Selection.activeObject as GameObject;
            GameActor item = selected.GetComponent<GameActor>();
            asset.itemName = Selection.activeObject.name;
            asset.itemID = Selection.activeObject.name;
            asset.shopItem = item;
            item.info = asset;

            string parentPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            parentPath = parentPath.Replace(Selection.activeObject.name + ".prefab", "");

            AssetDatabase.CreateAsset(asset, parentPath + asset.itemName + "ShopGameItem.asset");
            AssetDatabase.SaveAssets();
            PrefabUtility.SavePrefabAsset(item.gameObject);

            EditorUtility.FocusProjectWindow();            

            Selection.activeObject = asset;
        }
    }
}