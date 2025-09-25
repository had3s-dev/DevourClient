using UnityEngine;
using System.Threading;
using MelonLoader;

namespace DevourClient.Settings
{
    public class Settings
    {
        public static bool menu_enable = false;
        public static float width = Screen.width / 2f;
        public static float height = Screen.height / 2f;
        public static float x = 0;
        public static float y = 0;
        public static Color flashlight_color = new Color(1.00f, 1.00f, 1.00f, 1);
        public static Color player_esp_color = new Color(0.00f, 1.00f, 0.00f, 1);
        public static Color azazel_esp_color = new Color(1.00f, 0.00f, 0.00f, 1);
        public static float speed = 1f;
        public const string message_to_spam = "Deez Nutz";
        public static KeyCode flyKey = KeyCode.None;
        public static Vector2 itemsScrollPosition = Vector2.zero;
        public static Vector2 rituelObjectsScrollPosition = Vector2.zero;
        public static Vector2 stuffsScrollPosition = Vector2.zero;
        public static float spamIntervalSeconds = 0.5f;
        public static bool privateLobby = false;
        
        public static KeyCode GetKey()
        {
            Thread.Sleep(50); //TOFIX tried using anyKeydown, no success
            foreach (KeyCode vkey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(vkey))
                {
                    if (vkey != KeyCode.Delete)
                    {
                        return vkey;
                    }
                }
            }

            return KeyCode.None;
        }

        // Preferences
        private static MelonPreferences_Entry<float> PrefSpamInterval = default!;
        private static MelonPreferences_Entry<bool> PrefPrivateLobby = default!;
        private static MelonPreferences_Entry<bool> PrefUnlockCosmetics = default!;

        public static void InitializePreferences()
        {
            var cat = MelonPreferences.CreateCategory("DevourClient", "DevourClient");

            PrefSpamInterval = MelonPreferences.CreateEntry("DevourClient", "SpamIntervalSeconds", 0.5f, "Chat spam interval (seconds)");
            PrefPrivateLobby = MelonPreferences.CreateEntry("DevourClient", "DefaultPrivateLobby", false, "Default Private Lobby");
            PrefUnlockCosmetics = MelonPreferences.CreateEntry("DevourClient", "UnlockCosmetics", false, "Unlock cosmetics in menu");

            spamIntervalSeconds = PrefSpamInterval.Value;
            privateLobby = PrefPrivateLobby.Value;
            DevourClient.ClientMain.unlockCosmeticsEnabled = PrefUnlockCosmetics.Value;
        }

        public static void SavePreferences()
        {
            if (PrefSpamInterval != null) PrefSpamInterval.Value = spamIntervalSeconds;
            if (PrefPrivateLobby != null) PrefPrivateLobby.Value = privateLobby;
            if (PrefUnlockCosmetics != null) PrefUnlockCosmetics.Value = DevourClient.ClientMain.unlockCosmeticsEnabled;
            MelonPreferences.Save();
        }
    }
}
