using UnityEngine;

namespace Game.Mechanics
{
    // This manager handles the player's currency (Glimmers) and provides methods to add, remove, and check currency.
    public class CurrencyManager : MonoBehaviour
    {
        #region SingletonCode
        // Singleton instance for easy access from other scripts
        public static CurrencyManager instance { get; private set; }

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

        [SerializeField] public int Glimmers { get; private set; } // The player's current amount of Glimmers

        // Initialize Glimmers to 0 at the start of the game
        void Start()
        {
            Glimmers = 1000;
        }

        // Add a specified amount of Glimmers to the player's total
        public void AddGlimmers(int amount)
        {
            Glimmers += amount;
        }

        // Check if the player can afford a purchase of a given cost
        public bool CanAfford(int cost)
        {
            if (Glimmers >= cost) return true;
            return false;
        }

        // Remove a specified amount of Glimmers from the player's total
        public void RemoveGlimmers(int amount)
        {
            Glimmers -= amount;
        }
    }
}

