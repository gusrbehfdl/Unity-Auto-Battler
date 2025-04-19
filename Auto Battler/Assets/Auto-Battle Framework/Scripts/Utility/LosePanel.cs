using AutoBattleFramework.BattleBehaviour;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AutoBattleFramework.BattleUI
{
    /// <summary>
    /// Logic of the buttons on the defeat panel.
    /// </summary>
    public class LosePanel : MonoBehaviour
    {

        /// <summary>
        /// Reset the stage when defeated.
        /// </summary>
        public bool AutomaticResetStage = false;

        // Update is called once per frame
        void Update()
        {
            if (AutomaticResetStage)
            {
                ResetLastStage();
            }
        }

        /// <summary>
        /// Reset to the previous state, usually a Preparation State
        /// </summary>
        public void ResetLastState()
        {
            Battle.Instance.stage.PreviousState();
            Battle.Instance.losePanel.gameObject.SetActive(false);
            Battle.Instance.winPanel.gameObject.SetActive(false);
        }

        /// <summary>
        /// Reset to the first state of the current stage. Usually to the first Preparation State.
        /// </summary>
        public void ResetLastStage()
        {
            Battle.Instance.stage.ResetStage();
            Battle.Instance.losePanel.gameObject.SetActive(false);
            Battle.Instance.winPanel.gameObject.SetActive(false);
        }

        /// <summary>
        /// Reset the current scene.
        /// </summary>
        public void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Go to the main menu
        /// </summary>
        public void BackToMainMenu()
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}