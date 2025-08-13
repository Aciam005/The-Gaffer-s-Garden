using UnityEngine;

namespace Game.Mechanics.FX
{
    // This component manages the visual weather effects in the scene, such as rain.
    // It listens for weather changes from the WeatherSystem and enables/disables the rain particle system accordingly.
    [RequireComponent(typeof(WeatherSystem))]
    public class WeatherFXManager : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private ParticleSystem rainParticleSystem; // Reference to the rain particle system prefab in the scene
        [SerializeField] private WeatherSystem weatherSystem; // Reference to the WeatherSystem script

        // Subscribe to the weather change event when this object is enabled
        private void OnEnable()
        {
            WeatherSystem.OnWeatherChanged += HandleWeatherChanged;
        }

        // Unsubscribe from the event when this object is disabled to avoid memory leaks
        private void OnDisable()
        {
            WeatherSystem.OnWeatherChanged -= HandleWeatherChanged;
        }

        // On start, set the initial state of the rain effect based on the current weather
        private void Start()
        {
            if (weatherSystem != null)
                HandleWeatherChanged(weatherSystem.CurrentWeather);
        }

        // This method is called whenever the weather changes
        private void HandleWeatherChanged(WeatherSystem.WeatherType weatherType)
        {
            if (rainParticleSystem == null) return; // Safety check: make sure the reference is set
            if (weatherType == WeatherSystem.WeatherType.Rainy)
            {
                // If it's rainy and the rain isn't already playing, start it
                if (!rainParticleSystem.isPlaying)
                    rainParticleSystem.Play();
            }
            else
            {
                // If it's not rainy and the rain is playing, stop it
                if (rainParticleSystem.isPlaying)
                    rainParticleSystem.Stop();
            }
        }
    }
}

