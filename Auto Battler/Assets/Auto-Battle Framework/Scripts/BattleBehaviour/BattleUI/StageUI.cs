using AutoBattleFramework.BattleBehaviour;
using AutoBattleFramework.BattleBehaviour.States;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AutoBattleFramework.BattleUI
{
    /// <summary>
    /// Displays the different <see cref="BattleBehaviour.States.BattleState"/>s that makes up an <see cref="BattleBehaviour.ScriptableBattleStage"/>. The color of the icons vary depending on whether the state has already passed or is the current one.
    /// </summary>
    public class StageUI : MonoBehaviour
    {
        /// <summary>
        /// List of the stage images that have been generated.
        /// </summary>
        [HideInInspector]
        public List<GameObject> stageImages;

        //Current index of battle state.
        int currentIndex = 0;

        /// <summary>
        /// Color of previous states.
        /// </summary>
        [Tooltip("Color of previous states.")]
        public Color PastStateColor = Color.green;

        /// <summary>
        /// Color of current state (For example, in a fight inside <see cref="BattleBehaviour.States.FightState"/>).
        /// </summary>
        [Tooltip("Color of current states.")]
        public Color CurrentStateColor = Color.red;

        /// <summary>
        /// Color of next state.
        /// </summary>
        [Tooltip("Color of the next state.")]
        public Color NextStateColor = Color.yellow;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Creates the UI objects that represents each stage.
        /// </summary>
        public void InitializeUI()
        {
            foreach(GameObject image in stageImages)
            {
                Destroy(image);
            }
            stageImages.Clear();

            foreach (BattleState state in Battle.Instance.stage.stage)
            {
                GameObject stageImage = Instantiate(state.UIPrefab, transform);
                stageImages.Add(stageImage);
            }

            currentIndex = 0;
        }

        /// <summary>
        /// Set the colors according to the current state.
        /// </summary>
        /// <param name="currentState">Index of current state</param>
        public void NextState(int currentState)
        {
            //Past stage
            SetPastStageOptions();

            //Current stage. If ShowInUI set the current stage options. If not, jump to next.
            currentIndex = currentState;
            if (Battle.Instance.stage.stage[currentState].ShowInUI)
            {
                SetCurrentStageOptions(currentIndex);
            }
            else
            {
                SetNextStageOptions(currentIndex);
            }
        }

        /// <summary>
        /// Set the color of the past stage object to <see cref="PastStateColor"/>.
        /// </summary>
        void SetPastStageOptions()
        {
            Image image = GetImageInChildren(stageImages[currentIndex]);
            image.color = PastStateColor;
            //stageImages[currentIndex].GetComponent<Image>().color = PastStateColor;
        }

        /// <summary>
        /// Set the color of the current stage object to <see cref="CurrentStateColor"/>.
        /// </summary>
        void SetCurrentStageOptions(int currentIndex)
        {
            Image image = GetImageInChildren(stageImages[currentIndex]);
            image.color = CurrentStateColor;
            //stageImages[currentIndex].GetComponent<Image>().color = CurrentStateColor;
        }

        /// <summary>
        /// Set the color of the next stage object to <see cref="NextStateColor"/>.
        /// </summary>
        void SetNextStageOptions(int currentIndex)
        {
            if (stageImages.Count < currentIndex + 1)
            {
                Image image = GetImageInChildren(stageImages[currentIndex + 1]);
                image.color = NextStateColor;
            }
            //stageImages[currentIndex + 1].GetComponent<Image>().color = NextStateColor;
        }

        /// <summary>
        /// Get the image contained inside the panel. If no image is found, return the image of the panel.
        /// </summary>
        /// <param name="panel">Panel to found the image inside.</param>
        /// <returns>Children image, or parent image if not found.</returns>
        Image GetImageInChildren(GameObject panel)
        {
            Image image = panel.GetComponentsInChildren<Image>().Where(x => x.transform != panel.transform).FirstOrDefault();
            if (image)
            {
                return image;
            }
            return panel.GetComponent<Image>();
        }
    }
}