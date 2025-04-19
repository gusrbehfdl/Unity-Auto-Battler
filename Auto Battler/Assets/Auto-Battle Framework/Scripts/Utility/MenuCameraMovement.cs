using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AutoBattleFramework.Utility
{
    /// <summary>
    /// Camera movement for the menu
    /// </summary>
    public class MenuCameraMovement : MonoBehaviour
    {
        /// <summary>
        /// Transform to look at
        /// </summary>
        [SerializeField] Transform lookAt;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //Rotate while aiming to the transform.
            transform.LookAt(lookAt);
            transform.Translate(Vector3.right * Time.deltaTime);
        }

        /// <summary>
        /// Load the Sample Game
        /// </summary>
        public void StartScene1()
        {
            SceneManager.LoadScene("SampleGame");
        }

        /// <summary>
        /// Load the Sample Scene
        /// </summary>
        public void StartScene2()
        {
            SceneManager.LoadScene("Battle14x10");
        }

        /// <summary>
        /// Load the Sample Scene
        /// </summary>
        public void OpenAssetStoreURL()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/templates/packs/auto-battle-framework-232883");
        }

        /// <summary>
        /// Load the Sample Scene
        /// </summary>
        public void OpenMultiplayerAssetStore()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/templates/packs/multiplayer-for-auto-battle-framework-239974");
        }

        /// <summary>
        /// Open asset store to Gambitegy profile
        /// </summary>
        public void OpenAssetStoreProfile()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/templates/packs/auto-battle-framework-232883");
        }
    }
}