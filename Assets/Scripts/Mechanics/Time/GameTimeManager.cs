using System;
using UnityEngine;

namespace Game.Mechanics
{
    // This manager handles the passage of time in the game, including day/night cycles and day counting.
    public sealed class GameTimeManager : MonoBehaviour
    {
        #region SingletonCode
        // Singleton instance for easy access from other scripts
        public static GameTimeManager instance { get; private set; }

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
                DontDestroyOnLoad(gameObject); // Persist across scene loads
            }
        }
        #endregion

        [Header("SETTINGS")]
        [Tooltip("Length of one day,in seconds.")]
        public float DayLength; // How long a day lasts in seconds
        [Tooltip("Hours of daylight (0-24)")]
        [Range(1f, 23f)]
        public float DayLightHours = 16f; // How many hours are considered 'day'
        [Tooltip("Hours of night (0-24)")]
        [Range(1f, 23f)]
        public float NightHours = 8f; // How many hours are considered 'night'

        [Header("REFERENCES")]
        [SerializeField] private Light sun; // Reference to the directional light representing the sun


        [Header("TIME")]
        public float CurrentDayTime { get; private set; } // Time elapsed in the current day
        public float DayCounter { get; private set; } // Number of days passed

        // Event triggered when a new day starts
        public static event EventHandler OnNewDay;

        private void Start()
        {
            CurrentDayTime = 0;
            DayCounter = 0;
        }

        private void Update()
        {
            // Advance the time by the time since last frame
            CurrentDayTime += Time.deltaTime;

            // Rotate the Sun around the X axis for a day-night cycle.
            if (sun != null && DayLength > 0f)
            {
                float dayProgress = (CurrentDayTime % DayLength) / DayLength; // Progress through the day (0-1)
                float totalHours = DayLightHours + NightHours;
                float normalizedDayProgress = (dayProgress * totalHours) / 24f;
                
                // Calculate sun angle based on day/night configuration
                float sunAngle;
                if (normalizedDayProgress <= (DayLightHours / 24f))
                {
                    // Daytime: sun moves from -15f to +210f degrees
                    float dayTimeProgress = normalizedDayProgress / (DayLightHours / 24f);
                    sunAngle = Mathf.Lerp(-15f, 210f, dayTimeProgress);
                }
                else
                {
                    // Nighttime: sun moves from +210f to +340f degrees (below horizon)
                    float nightTimeProgress = (normalizedDayProgress - (DayLightHours / 24f)) / (NightHours / 24f);
                    sunAngle = Mathf.Lerp(210f, 340f, nightTimeProgress);
                }

                // Apply the calculated rotation to the sun
                sun.transform.localRotation = Quaternion.Euler(sunAngle, 0f, 0f);
            }

            // If a day has passed, trigger the new day event and reset the timer
            if (CurrentDayTime >= DayLength)
            {
                OnNewDay?.Invoke(this, EventArgs.Empty);
                CurrentDayTime = 0;
                DayCounter++;
            }

        }

    } 
}
