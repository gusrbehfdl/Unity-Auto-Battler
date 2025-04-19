using AutoBattleFramework.BattleBehaviour;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AutoBattleFramework.Shop.ShopGUI
{
    /// <summary>
    /// Timer used to set a maximum round time. Once it reaches zero, a method that depends on each <see cref="BattleBehaviour.States.BattleState"/> is activated.
    /// </summary>
    public class Timer : MonoBehaviour
    {
        /// <summary>
        /// Time remaining until it reaches zero.
        /// </summary>
        public float timeRemaining = 10;

        /// <summary>
        /// If the timer is activated.
        /// </summary>
        public bool timerIsRunning = false;

        /// <summary>
        /// Text showing the <see cref="timeRemaining"/>.
        /// </summary>
        public TMPro.TextMeshProUGUI timeText;

        /// <summary>
        /// Current battle reference.
        /// </summary>
        public Battle battle;

        private void Start()
        {

        }

        /// <summary>
        /// When the timer reaches 0, stop the timer and call the current state <see cref="BattleBehaviour.States.BattleState.OnTimerFinish"/>.
        /// </summary>
        void Update()
        {
            if (timerIsRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    DisplayTime(timeRemaining);
                }
                else
                {
                    timeRemaining = 0;
                    timerIsRunning = false;
                    battle.stage.GetCurrentState().OnTimerFinish();
                }
            }
        }

        /// <summary>
        /// Updates the text with the <see cref="timeRemaining"/>.
        /// </summary>
        /// <param name="timeToDisplay">Time remaining.</param>
        public virtual void DisplayTime(float timeToDisplay)
        {
            if (timeText)
            {
                timeToDisplay += 1;
                float minutes = Mathf.FloorToInt(timeToDisplay / 60);
                float seconds = Mathf.FloorToInt(timeToDisplay % 60);
                timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }

        /// <summary>
        /// Set the <see cref="timeRemaining"/> with the given value. This function does not start the timer.
        /// </summary>
        /// <param name="time">New remaining time.</param>
        public void ResetTimer(float time)
        {
            timeRemaining = time;
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void StartTimer()
        {
            timerIsRunning = true;
        }

        /// <summary>
        /// Stop the timer.
        /// </summary>
        public void StopTimer()
        {
            timerIsRunning = false;
        }
    }
}