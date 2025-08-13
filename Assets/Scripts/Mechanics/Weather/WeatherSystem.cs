using System;
using UnityEngine;

namespace Game.Mechanics
{
    // This system determines the weather for each day and notifies listeners when it changes.
    public class WeatherSystem : MonoBehaviour
    {
        [Header("REFERENCES")]
        [Range(0, 1)][SerializeField] public float RainChance; // Probability (0-1) that a new day will be rainy

        // Enum for possible weather types. Add more as needed.
        public enum WeatherType
        {
            Sunny,
            Rainy
        }

        // Event for when the weather changes. Other systems can subscribe to this.
        public static event Action<WeatherType> OnWeatherChanged;
        public WeatherType CurrentWeather { get; private set; } = WeatherType.Sunny;

        // Subscribe to the new day event when enabled
        private void OnEnable()
        {
            GameTimeManager.OnNewDay += HandleNewDay;
        }

        // Unsubscribe when disabled to avoid memory leaks
        private void OnDisable()
        {
            GameTimeManager.OnNewDay -= HandleNewDay;
        }

        // Called when a new day starts
        private void HandleNewDay(object sender, EventArgs e)
        {
            // Roll for weather: if random value is less than RainChance, it's rainy
            WeatherType newWeather = UnityEngine.Random.value < RainChance ? WeatherType.Rainy : WeatherType.Sunny;
            CurrentWeather = newWeather;
            // Notify all listeners about the new weather
            OnWeatherChanged?.Invoke(CurrentWeather);
        }
    }
}
