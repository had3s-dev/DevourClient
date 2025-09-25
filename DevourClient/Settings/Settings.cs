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
        public static KeyCode speedToggleKey = KeyCode.None;
        public static KeyCode espToggleKey = KeyCode.None;
        public static Vector2 itemsScrollPosition = Vector2.zero;
        public static Vector2 rituelObjectsScrollPosition = Vector2.zero;
        public static Vector2 stuffsScrollPosition = Vector2.zero;
        public static float spamIntervalSeconds = 0.5f;
        public static bool privateLobby = false;
        public static bool espDistanceCulling = false;
        public static float espMaxDistance = 150f;
        public static float espDrawIntervalMs = 0f;
        public static int lobbyRegionIndex = 0;
        // Per-category ESP caps
        public static bool cullPlayers = false;
        public static bool cullAzazel = false;
        public static bool cullItems = false;
        public static bool cullAnimals = false;
        public static bool cullDemons = false;
        public static float maxDistPlayers = 150f;
        public static float maxDistAzazel = 200f;
        public static float maxDistItems = 120f;
        public static float maxDistAnimals = 150f;
        public static float maxDistDemons = 180f;
        
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
        private static MelonPreferences_Entry<bool> PrefEspDistanceCulling = default!;
        private static MelonPreferences_Entry<float> PrefEspMaxDistance = default!;
        private static MelonPreferences_Entry<float> PrefEspDrawIntervalMs = default!;
        private static MelonPreferences_Entry<int> PrefLobbyRegionIndex = default!;
        private static MelonPreferences_Entry<bool> PrefCullPlayers = default!;
        private static MelonPreferences_Entry<bool> PrefCullAzazel = default!;
        private static MelonPreferences_Entry<bool> PrefCullItems = default!;
        private static MelonPreferences_Entry<bool> PrefCullAnimals = default!;
        private static MelonPreferences_Entry<bool> PrefCullDemons = default!;
        private static MelonPreferences_Entry<float> PrefMaxDistPlayers = default!;
        private static MelonPreferences_Entry<float> PrefMaxDistAzazel = default!;
        private static MelonPreferences_Entry<float> PrefMaxDistItems = default!;
        private static MelonPreferences_Entry<float> PrefMaxDistAnimals = default!;
        private static MelonPreferences_Entry<float> PrefMaxDistDemons = default!;

        public static void InitializePreferences()
        {
            var cat = MelonPreferences.CreateCategory("DevourClient", "DevourClient");

            PrefSpamInterval = MelonPreferences.CreateEntry("DevourClient", "SpamIntervalSeconds", 0.5f, "Chat spam interval (seconds)");
            PrefPrivateLobby = MelonPreferences.CreateEntry("DevourClient", "DefaultPrivateLobby", false, "Default Private Lobby");
            PrefUnlockCosmetics = MelonPreferences.CreateEntry("DevourClient", "UnlockCosmetics", false, "Unlock cosmetics in menu");
            PrefEspDistanceCulling = MelonPreferences.CreateEntry("DevourClient", "EspDistanceCulling", false, "Enable ESP distance culling");
            PrefEspMaxDistance = MelonPreferences.CreateEntry("DevourClient", "EspMaxDistance", 150f, "ESP max distance (meters)");
            PrefEspDrawIntervalMs = MelonPreferences.CreateEntry("DevourClient", "EspDrawIntervalMs", 0f, "ESP draw throttle (ms, 0=off)");
            PrefLobbyRegionIndex = MelonPreferences.CreateEntry("DevourClient", "LobbyRegionIndex", 0, "Default Lobby Region Index");
            PrefCullPlayers = MelonPreferences.CreateEntry("DevourClient", "CullPlayers", false, "Distance cull for players");
            PrefCullAzazel = MelonPreferences.CreateEntry("DevourClient", "CullAzazel", false, "Distance cull for Azazel");
            PrefCullItems = MelonPreferences.CreateEntry("DevourClient", "CullItems", false, "Distance cull for items");
            PrefCullAnimals = MelonPreferences.CreateEntry("DevourClient", "CullAnimals", false, "Distance cull for animals");
            PrefCullDemons = MelonPreferences.CreateEntry("DevourClient", "CullDemons", false, "Distance cull for demons");
            PrefMaxDistPlayers = MelonPreferences.CreateEntry("DevourClient", "MaxDistPlayers", 150f, "Max distance for players");
            PrefMaxDistAzazel = MelonPreferences.CreateEntry("DevourClient", "MaxDistAzazel", 200f, "Max distance for Azazel");
            PrefMaxDistItems = MelonPreferences.CreateEntry("DevourClient", "MaxDistItems", 120f, "Max distance for items");
            PrefMaxDistAnimals = MelonPreferences.CreateEntry("DevourClient", "MaxDistAnimals", 150f, "Max distance for animals");
            PrefMaxDistDemons = MelonPreferences.CreateEntry("DevourClient", "MaxDistDemons", 180f, "Max distance for demons");

            spamIntervalSeconds = PrefSpamInterval.Value;
            privateLobby = PrefPrivateLobby.Value;
            DevourClient.ClientMain.unlockCosmeticsEnabled = PrefUnlockCosmetics.Value;
            espDistanceCulling = PrefEspDistanceCulling.Value;
            espMaxDistance = PrefEspMaxDistance.Value;
            espDrawIntervalMs = PrefEspDrawIntervalMs.Value;
            lobbyRegionIndex = PrefLobbyRegionIndex.Value;
            cullPlayers = PrefCullPlayers.Value;
            cullAzazel = PrefCullAzazel.Value;
            cullItems = PrefCullItems.Value;
            cullAnimals = PrefCullAnimals.Value;
            cullDemons = PrefCullDemons.Value;
            maxDistPlayers = PrefMaxDistPlayers.Value;
            maxDistAzazel = PrefMaxDistAzazel.Value;
            maxDistItems = PrefMaxDistItems.Value;
            maxDistAnimals = PrefMaxDistAnimals.Value;
            maxDistDemons = PrefMaxDistDemons.Value;
        }

        public static void SavePreferences()
        {
            if (PrefSpamInterval != null) PrefSpamInterval.Value = spamIntervalSeconds;
            if (PrefPrivateLobby != null) PrefPrivateLobby.Value = privateLobby;
            if (PrefUnlockCosmetics != null) PrefUnlockCosmetics.Value = DevourClient.ClientMain.unlockCosmeticsEnabled;
            if (PrefEspDistanceCulling != null) PrefEspDistanceCulling.Value = espDistanceCulling;
            if (PrefEspMaxDistance != null) PrefEspMaxDistance.Value = espMaxDistance;
            if (PrefEspDrawIntervalMs != null) PrefEspDrawIntervalMs.Value = espDrawIntervalMs;
            if (PrefLobbyRegionIndex != null) PrefLobbyRegionIndex.Value = lobbyRegionIndex;
            if (PrefCullPlayers != null) PrefCullPlayers.Value = cullPlayers;
            if (PrefCullAzazel != null) PrefCullAzazel.Value = cullAzazel;
            if (PrefCullItems != null) PrefCullItems.Value = cullItems;
            if (PrefCullAnimals != null) PrefCullAnimals.Value = cullAnimals;
            if (PrefCullDemons != null) PrefCullDemons.Value = cullDemons;
            if (PrefMaxDistPlayers != null) PrefMaxDistPlayers.Value = maxDistPlayers;
            if (PrefMaxDistAzazel != null) PrefMaxDistAzazel.Value = maxDistAzazel;
            if (PrefMaxDistItems != null) PrefMaxDistItems.Value = maxDistItems;
            if (PrefMaxDistAnimals != null) PrefMaxDistAnimals.Value = maxDistAnimals;
            if (PrefMaxDistDemons != null) PrefMaxDistDemons.Value = maxDistDemons;
            MelonPreferences.Save();
        }
    }
}
