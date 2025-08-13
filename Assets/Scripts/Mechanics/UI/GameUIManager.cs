using System;
using TMPro;
using UnityEngine;

namespace Game.Mechanics.UI
{
    // This manager updates the UI elements that display time and currency to the player.
    [RequireComponent(typeof(GameTimeManager))]
    public class GameUIManager : MonoBehaviour
    {
        #region SingletonCode
        // Singleton instance for easy access from other scripts
        public static GameUIManager instance { get; private set; }

        private void Awake()
        {
            // Standard singleton pattern: ensures only one instance exists
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }
        #endregion

        [Header("REFERENCES")]
        [SerializeField] private TMP_Text dayTimeText; // UI text for displaying the current time of day
        [SerializeField] private TMP_Text dayCounterText; // UI text for displaying the current day number
        [SerializeField] private TMP_Text GlimmerCountText; // UI text for displaying the player's Glimmer count
        private GameTimeManager timeManager;
        private CurrencyManager currencyManager;

        private void Start()
        {
            // Cache references to the managers for performance
            timeManager = GameTimeManager.instance;
            currencyManager = CurrencyManager.instance;
        }

        private void Update()
        {
            // TODO: This update should be optimized to run less frequently (e.g., every 10 seconds)
            UpdateTimeUI();
            UpdateGlimmerUI();
        }

        // Updates the time display in the UI
        private void UpdateTimeUI()
        {
            // Display time as a 24-hour cycle
            float dayLength = timeManager.DayLength;

            if (dayLength == 0) dayLength = 1; // Prevent division by 0

            float currentDayTime = timeManager.CurrentDayTime % dayLength;
            float normalizedTime = currentDayTime / dayLength;
            int hours = Mathf.FloorToInt(normalizedTime * 24f);
            int minutes = Mathf.FloorToInt((normalizedTime * 24f - hours) * 60f);

            dayTimeText.text = $"{hours:00}:{minutes:00}";

            // Update the day counter
            dayCounterText.text = "Day:" + timeManager.DayCounter.ToString();
        }

        // Updates the Glimmer count display in the UI
        private void UpdateGlimmerUI()
        {
            GlimmerCountText.text = "Glimmers:" + currencyManager.Glimmers.ToString(); 
        }
    }
}

