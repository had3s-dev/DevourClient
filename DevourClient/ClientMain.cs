using DevourClient.Helpers;
using MelonLoader;
using System.Threading.Tasks;
using Il2CppPhoton.Bolt;
using UnityEngine;
using Il2Cpp;
using System.Threading;

namespace DevourClient
{
    public class ClientMain : MonoBehaviour
    {
        public static bool unlockCosmeticsEnabled = false;
        public ClientMain(System.IntPtr ptr)
            : base(ptr)
        {
        }

        enum CurrentTab : int
        {
            Visuals = 0,
            Entities = 1,
            Map = 2,
            ESP = 3,
            Items = 4,
            Misc = 5,
            Players = 6
        }

        static Rect windowRect = new Rect(Settings.Settings.x + 10, Settings.Settings.y + 10, 800, 780);
        static CurrentTab current_tab = CurrentTab.Visuals;

        static bool flashlight_toggle = false;
        static bool flashlight_colorpick = false;
        static bool player_esp_colorpick = false;
        static bool azazel_esp_colorpick = false;
        static bool spoofLevel = false;
        static float spoofLevelValue = 0;
        static bool change_server_name = false;
        static bool change_steam_name = false;
        static bool fly = false;
        static float fly_speed = 5;
        static bool fastMove = false;
        static float _PlayerSpeedMultiplier = 1;
        public static float lobbySize = 4;
        public static bool _IsAutoRespawn = false;
        public static bool unlimitedUV = false;
        public static bool exp_modifier = false;
        public static float exp = 1000f;
        public static bool _walkInLobby = false;
        public static bool infinite_mirrors = false;
        static bool player_esp = false;
        static bool player_skel_esp = false;
        static bool player_snapline = false;
        static bool player_nameplate_info = false;
        static bool azazel_esp = false;
        static bool azazel_skel_esp = false;
        static bool azazel_snapline = false;
        static float _lastEspDrawTime = 0f;
        static bool spam_message = false;
        // Throttle for chat spam to avoid log flooding and excessive calls
        static float _spamIntervalSeconds = 0.5f;
        static float _lastSpamTime = 0f;
        static bool item_esp = false;
        static bool goat_rat_esp = false;
        static bool demon_esp = false;
        static bool fullbright = false;
        static bool need_fly_reset = false;
        static bool crosshair = false;
        static bool in_game_cache = false;
        static bool should_show_start_message = true;
        static Texture2D crosshairTexture = default!;
        static bool captureFlyKey = false;
        static bool captureEspKey = false;
        static bool captureSpeedKey = false;
        static bool showLobbySection = false;
        static bool showRiskySection = false;

        public void OnApplicationStart()
        {
            MelonLogger.Msg("DevourClient loaded!");
            MelonLogger.Msg("Press INSERT to open the menu");
            // Load preferences
            Settings.Settings.InitializePreferences();
            _spamIntervalSeconds = Settings.Settings.spamIntervalSeconds;
        }

        public void Start()
        {
            MelonLogger.Msg("For the Queen !");
            MelonLogger.Warning("Made with <3 by patate and Jadis.");
            MelonLogger.Warning("Github : https://github.com/ALittlePatate/DevourClient");
            MelonLogger.Warning("Note : if you payed for this you most likely got scammed.");

            crosshairTexture = Helpers.GUIHelper.GetCircularTexture(5, 5);

            MelonCoroutines.Start(Helpers.Entities.GetLocalPlayer());
            MelonCoroutines.Start(Helpers.Entities.GetGoatsAndRats());
            MelonCoroutines.Start(Helpers.Entities.GetSurvivalInteractables());
            MelonCoroutines.Start(Helpers.Entities.GetKeys());
            MelonCoroutines.Start(Helpers.Entities.GetDemons());
            MelonCoroutines.Start(Helpers.Entities.GetSpiders());
            MelonCoroutines.Start(Helpers.Entities.GetGhosts());
            MelonCoroutines.Start(Helpers.Entities.GetBoars());
            MelonCoroutines.Start(Helpers.Entities.GetCorpses());
            MelonCoroutines.Start(Helpers.Entities.GetCrows());
            MelonCoroutines.Start(Helpers.Entities.GetLumps());
            MelonCoroutines.Start(Helpers.Entities.GetAzazels());
            MelonCoroutines.Start(Helpers.Entities.GetAllPlayers());
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                try
                {
                    Il2Cpp.GameUI gameUI = UnityEngine.Object.FindObjectOfType<Il2Cpp.GameUI>();
                    if (gameUI != null)
                    {
                        if (Settings.Settings.menu_enable)
                        {
                            gameUI.HideMouseCursor();
                        }
                        else
                        {
                            gameUI.ShowMouseCursor();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"Error toggling menu: {ex.Message}");
                }

                Settings.Settings.menu_enable = !Settings.Settings.menu_enable;
            }

            // ESP hotkey toggle
            if (Input.GetKeyDown(Settings.Settings.espToggleKey))
            {
                player_esp = !player_esp;
                player_skel_esp = player_esp ? player_skel_esp : false;
                player_snapline = player_esp ? player_snapline : false;
            }

            if (Player.IsInGame())
            {
                if (flashlight_toggle && !fullbright)
                {
                    Hacks.Misc.BigFlashlight(false);
                }
                else if (!flashlight_toggle && !fullbright)
                {
                    Hacks.Misc.BigFlashlight(true);
                }

                if (fullbright && !flashlight_toggle)
                {
                    Hacks.Misc.Fullbright(false);
                }
                else if (!fullbright && !flashlight_toggle)
                {
                    Hacks.Misc.Fullbright(true);
                }

                if (_IsAutoRespawn && Helpers.Player.IsPlayerCrawling())
                {
                    Hacks.Misc.AutoRespawn();
                }

                if (crosshair && !in_game_cache)
                {
                    in_game_cache = true;
                }
            }
            else
            {
                if (change_server_name)
                {
                    Hacks.Misc.SetServerName("patate on top !");
                }

                if (change_steam_name)
                {
                    Hacks.Misc.SetSteamName("patate");
                }

                if (crosshair && in_game_cache)
                {
                    in_game_cache = false;
                }
            }

            if (spam_message)
            {
                if (Time.time - _lastSpamTime >= _spamIntervalSeconds)
                {
                    // Keep logs minimal to reduce noise
                    // MelonLogger.Msg("chat spam tick");
                    Hacks.Misc.MessageSpam(Settings.Settings.message_to_spam);
                    _lastSpamTime = Time.time;
                }
            }

            if (spoofLevel)
            {
                Hacks.Misc.SetRank((int)spoofLevelValue);
            }

            if (Input.GetKeyDown(Settings.Settings.flyKey))
            {
                fly = !fly;
            }

            if (Player.IsInGameOrLobby())
            {
                if (fly && !need_fly_reset)
                {
                    Il2Cpp.NolanBehaviour nb = Player.GetPlayer();
                    if (nb)
                    {
                        Collider coll = nb.GetComponentInChildren<Collider>();
                        if (coll)
                        {
                            coll.enabled = false;
                            need_fly_reset = true;
                        }
                    }
                }

                else if (!fly && need_fly_reset)
                {
                    Il2Cpp.NolanBehaviour nb = Player.GetPlayer();
                    if (nb)
                    {
                        Collider coll = nb.GetComponentInChildren<Collider>();
                        if (coll)
                        {
                            coll.enabled = true;
                            need_fly_reset = false;
                        }
                    }
                }

                if (fly)
                {
                    Hacks.Misc.Fly(fly_speed);
                }

                if (Input.GetKeyDown(Settings.Settings.speedToggleKey))
                {
                    fastMove = !fastMove;
                }

            }

            if (Helpers.Map.GetActiveScene() == "Menu")
            {
                Hacks.Misc.WalkInLobby(_walkInLobby);
            }

            if (fastMove)
            {
                try
                {
                    Helpers.Entities.LocalPlayer_.p_GameObject.GetComponent<Il2CppOpsive.UltimateCharacterController.Character.UltimateCharacterLocomotion>().TimeScale = _PlayerSpeedMultiplier;
                }
                catch { return; }
            }
        }

        public void OnGUI()
        {
            // Handle key capture for hotkeys
            if (Settings.Settings.menu_enable)
            {
                UnityEngine.Event e = UnityEngine.Event.current;
                if (e != null && e.type == EventType.KeyDown)
                {
                    if (captureFlyKey)
                    {
                        Settings.Settings.flyKey = e.keyCode;
                        captureFlyKey = false;
                        e.Use();
                    }
                    else if (captureEspKey)
                    {
                        Settings.Settings.espToggleKey = e.keyCode;
                        captureEspKey = false;
                        e.Use();
                    }
                    else if (captureSpeedKey)
                    {
                        Settings.Settings.speedToggleKey = e.keyCode;
                        captureSpeedKey = false;
                        e.Use();
                    }
                }
            }
            if (should_show_start_message)
            {
                if (DevourClient.Hacks.Misc.ShowMessageBox("Welcome to DevourClient.\n\nPress the INS key to open the menu.") == 0)
                    should_show_start_message = false;
            }

            GUI.backgroundColor = Color.grey;

            // Theme
            GUI.skin.button.normal.background = GUIHelper.MakeTex(2, 2, new Color(0.12f, 0.12f, 0.12f, 1f));
            GUI.skin.button.normal.textColor = Color.white;

            GUI.skin.button.hover.background = GUIHelper.MakeTex(2, 2, new Color(0.2f, 0.5f, 0.2f, 1f));
            GUI.skin.button.hover.textColor = Color.white;

            GUI.skin.toggle.onNormal.textColor = Color.yellow;

            //from https://www.unknowncheats.me/forum/unity/437277-mono-internal-optimisation-tips.html
            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                // Throttle ESP draw if configured
                if (Settings.Settings.espDrawIntervalMs > 0f)
                {
                    if (Time.time * 1000f - _lastEspDrawTime < Settings.Settings.espDrawIntervalMs)
                    {
                        return;
                    }
                    _lastEspDrawTime = Time.time * 1000f;
                }

                if (player_esp || player_snapline || player_skel_esp)
                {
                    foreach (Helpers.BasePlayer p in Helpers.Entities.Players)
                    {
                        if (p == null)
                        {
                            continue;
                        }

                        GameObject player = p.p_GameObject;
                        if (player != null)
                        {
                            Il2Cpp.NolanBehaviour nb = player.GetComponent<Il2Cpp.NolanBehaviour>();
                            if (nb.entity.IsOwner)
                            {
                                continue;
                            }

                            if (Camera.main != null && (Settings.Settings.espDistanceCulling || Settings.Settings.cullPlayers))
                            {
                                float dist = Vector3.Distance(Camera.main.transform.position, player.transform.position);
                                float cap = Settings.Settings.cullPlayers ? Settings.Settings.maxDistPlayers : Settings.Settings.espMaxDistance;
                                if (dist > cap) continue;
                            }

                            if (player_skel_esp)
                            {
                                Render.Render.DrawAllBones(Hacks.Misc.GetAllBones(nb.animator), Settings.Settings.player_esp_color);
                            }
                            string label = p.Name;
                            if (player_nameplate_info && Camera.main != null)
                            {
                                float dist = Vector3.Distance(Camera.main.transform.position, player.transform.position);
                                bool isCrawling = false;
                                try { isCrawling = nb.IsCrawling(); } catch { }
                                label = $"{p.Name} [{dist:0}m]{(isCrawling ? " [Crawling]" : "")}";
                            }

                            Render.Render.DrawBoxESP(player, -0.25f, 1.75f, label, Settings.Settings.player_esp_color, player_snapline, player_esp);
                        }
                    }
                }

                if (goat_rat_esp)
                {
                    foreach (Il2Cpp.GoatBehaviour goat in Helpers.Entities.GoatsAndRats)
                    {
                        if (goat != null)
                        {
                            if (Camera.main != null && (Settings.Settings.espDistanceCulling || Settings.Settings.cullAnimals))
                            {
                                float dist = Vector3.Distance(Camera.main.transform.position, goat.transform.position);
                                float cap = Settings.Settings.cullAnimals ? Settings.Settings.maxDistAnimals : Settings.Settings.espMaxDistance;
                                if (dist > cap) continue;
                            }
                            Render.Render.DrawNameESP(goat.transform.position, goat.name.Replace("Survival", "").Replace("(Clone)", ""), new Color(0.94f, 0.61f, 0.18f, 1.0f));
                        }
                    }
                }

                if (item_esp)
                {
                    foreach (Il2Cpp.SurvivalInteractable obj in Helpers.Entities.SurvivalInteractables)
                    {
                        if (obj != null)
                        {
                            if (Camera.main != null && (Settings.Settings.espDistanceCulling || Settings.Settings.cullItems))
                            {
                                float dist = Vector3.Distance(Camera.main.transform.position, obj.transform.position);
                                float cap = Settings.Settings.cullItems ? Settings.Settings.maxDistItems : Settings.Settings.espMaxDistance;
                                if (dist > cap) continue;
                            }
                            Render.Render.DrawNameESP(obj.transform.position, obj.prefabName.Replace("Survival", ""), new Color(1.0f, 1.0f, 1.0f));
                        }
                    }

                    foreach (Il2Cpp.KeyBehaviour key in Helpers.Entities.Keys)
                    {
                        if (key != null)
                        {
                            if (Camera.main != null && (Settings.Settings.espDistanceCulling || Settings.Settings.cullItems))
                            {
                                float dist = Vector3.Distance(Camera.main.transform.position, key.transform.position);
                                float cap = Settings.Settings.cullItems ? Settings.Settings.maxDistItems : Settings.Settings.espMaxDistance;
                                if (dist > cap) continue;
                            }
                            Render.Render.DrawNameESP(key.transform.position, "Key", new Color(1.0f, 1.0f, 1.0f));
                        }
                    }
                }

                if (demon_esp)
                {
                    foreach (Il2Cpp.SurvivalDemonBehaviour demon in Helpers.Entities.Demons)
                    {
                        if (demon != null)
                        {
                            if (Camera.main != null && (Settings.Settings.espDistanceCulling || Settings.Settings.cullDemons))
                            {
                                float dist = Vector3.Distance(Camera.main.transform.position, demon.transform.position);
                                float cap = Settings.Settings.cullDemons ? Settings.Settings.maxDistDemons : Settings.Settings.espMaxDistance;
                                if (dist > cap) continue;
                            }
                            Render.Render.DrawNameESP(demon.transform.position, demon.name.Replace("Survival", "").Replace("(Clone)", ""), new Color(1.0f, 0.0f, 0.0f, 1.0f));
                        }
                    }

                    foreach (Il2Cpp.SpiderBehaviour spider in Helpers.Entities.Spiders)
                    {
                        if (spider != null)
                        {
                            if (Camera.main != null && (Settings.Settings.espDistanceCulling || Settings.Settings.cullDemons))
                            {
                                float dist = Vector3.Distance(Camera.main.transform.position, spider.transform.position);
                                float cap = Settings.Settings.cullDemons ? Settings.Settings.maxDistDemons : Settings.Settings.espMaxDistance;
                                if (dist > cap) continue;
                            }
                            Render.Render.DrawNameESP(spider.transform.position, "Spider", new Color(1.0f, 0.0f, 0.0f, 1.0f));
                        }
                    }

                    foreach (Il2Cpp.GhostBehaviour ghost in Helpers.Entities.Ghosts)
                    {
                        if (ghost != null)
                        {
                            if (Camera.main != null && (Settings.Settings.espDistanceCulling || Settings.Settings.cullDemons))
                            {
                                float dist = Vector3.Distance(Camera.main.transform.position, ghost.transform.position);
                                float cap = Settings.Settings.cullDemons ? Settings.Settings.maxDistDemons : Settings.Settings.espMaxDistance;
                                if (dist > cap) continue;
                            }
                            Render.Render.DrawNameESP(ghost.transform.position, "Ghost", new Color(1.0f, 0.0f, 0.0f, 1.0f));
                        }
                    }

                    foreach (Il2Cpp.BoarBehaviour boar in Helpers.Entities.Boars)
                    {
                        if (boar != null)
                        {
                            if (Camera.main != null && (Settings.Settings.espDistanceCulling || Settings.Settings.cullAnimals))
                            {
                                float dist = Vector3.Distance(Camera.main.transform.position, boar.transform.position);
                                float cap = Settings.Settings.cullAnimals ? Settings.Settings.maxDistAnimals : Settings.Settings.espMaxDistance;
                                if (dist > cap) continue;
                            }
                            Render.Render.DrawNameESP(boar.transform.position, "Boar", new Color(1.0f, 0.0f, 0.0f, 1.0f));
                        }
                    }

                    foreach (Il2Cpp.CorpseBehaviour corpse in Helpers.Entities.Corpses)
                    {
                        if (corpse != null)
                        {
                            if (Camera.main != null && (Settings.Settings.espDistanceCulling || Settings.Settings.cullDemons))
                            {
                                float dist = Vector3.Distance(Camera.main.transform.position, corpse.transform.position);
                                float cap = Settings.Settings.cullDemons ? Settings.Settings.maxDistDemons : Settings.Settings.espMaxDistance;
                                if (dist > cap) continue;
                            }
                            Render.Render.DrawNameESP(corpse.transform.position, "Corpse", new Color(1.0f, 0.0f, 0.0f, 1.0f));
                        }
                    }

                    foreach (Il2Cpp.CrowBehaviour crow in Helpers.Entities.Crows)
                    {
                        if (crow != null)
                        {
                            if (Camera.main != null && (Settings.Settings.espDistanceCulling || Settings.Settings.cullAnimals))
                            {
                                float dist = Vector3.Distance(Camera.main.transform.position, crow.transform.position);
                                float cap = Settings.Settings.cullAnimals ? Settings.Settings.maxDistAnimals : Settings.Settings.espMaxDistance;
                                if (dist > cap) continue;
                            }
                            Render.Render.DrawNameESP(crow.transform.position, "Crow", new Color(1.0f, 0.0f, 0.0f, 1.0f));
                        }
                    }

                    foreach (Il2Cpp.ManorLumpController lump in Helpers.Entities.Lumps)
                    {
                        if (lump != null)
                        {
                            if (Camera.main != null && (Settings.Settings.espDistanceCulling || Settings.Settings.cullDemons))
                            {
                                float dist = Vector3.Distance(Camera.main.transform.position, lump.transform.position);
                                float cap = Settings.Settings.cullDemons ? Settings.Settings.maxDistDemons : Settings.Settings.espMaxDistance;
                                if (dist > cap) continue;
                            }
                            Render.Render.DrawNameESP(lump.transform.position, "Lump", new Color(1.0f, 0.0f, 0.0f, 1.0f));
                        }
                    }
                }

                if (azazel_esp || azazel_snapline || azazel_skel_esp)
                {
                    foreach (Il2Cpp.SurvivalAzazelBehaviour survivalAzazel in Helpers.Entities.Azazels)
                    {
                        if (survivalAzazel != null)
                        {
                            if (Camera.main != null && (Settings.Settings.espDistanceCulling || Settings.Settings.cullAzazel))
                            {
                                float dist = Vector3.Distance(Camera.main.transform.position, survivalAzazel.transform.position);
                                float cap = Settings.Settings.cullAzazel ? Settings.Settings.maxDistAzazel : Settings.Settings.espMaxDistance;
                                if (dist > cap) continue;
                            }
                            if (azazel_skel_esp)
                            {
                                Render.Render.DrawAllBones(Hacks.Misc.GetAllBones(survivalAzazel.animator), Settings.Settings.azazel_esp_color);
                            }

                            Render.Render.DrawBoxESP(survivalAzazel.gameObject, -0.25f, 2.0f, "Azazel", Settings.Settings.azazel_esp_color, azazel_snapline, azazel_esp);
                        }
                    }
                }

                if (crosshair && in_game_cache) //&& !Player.IsPlayerCrawling())
                {
                    const float crosshairSize = 4;

                    float xMin = (Settings.Settings.width) - (crosshairSize / 2);
                    float yMin = (Settings.Settings.height) - (crosshairSize / 2);

                    if (crosshairTexture == null)
                    {
                        crosshairTexture = Helpers.GUIHelper.GetCircularTexture(5, 5);
                    }

                    GUI.DrawTexture(new Rect(xMin, yMin, crosshairSize, crosshairSize), crosshairTexture);
                }
            }

            if (Settings.Settings.menu_enable) //Si on appuie sur INSERT
            {
                windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)Tabs, "DevourClient");
            }
        }

        public static void Tabs(int windowID)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Visual", GUILayout.Height(40)) || Input.GetKeyDown(KeyCode.F1))
            {
                current_tab = CurrentTab.Visuals;
            }

            if (GUILayout.Button("Entites", GUILayout.Height(40)) || Input.GetKeyDown(KeyCode.F2))
            {
                current_tab = CurrentTab.Entities;
            }

            if (GUILayout.Button("Map Specific", GUILayout.Height(40)) || Input.GetKeyDown(KeyCode.F3))
            {
                current_tab = CurrentTab.Map;
            }

            if (GUILayout.Button("ESP", GUILayout.Height(40)) || Input.GetKeyDown(KeyCode.F4))
            {
                current_tab = CurrentTab.ESP;
            }

            if (GUILayout.Button("Items", GUILayout.Height(40)) || Input.GetKeyDown(KeyCode.F5))
            {
                current_tab = CurrentTab.Items;
            }

            if (GUILayout.Button("Misc", GUILayout.Height(40)) || Input.GetKeyDown(KeyCode.F6))
            {
                current_tab = CurrentTab.Misc;
            }

            if (GUILayout.Button("Player", GUILayout.Height(40)) || Input.GetKeyDown(KeyCode.F7))
            {
                current_tab = CurrentTab.Players;
            }

            GUILayout.EndHorizontal();

            switch (current_tab)
            {
                case CurrentTab.Visuals:
                    VisualsTab();
                    break;
                case CurrentTab.Entities:
                    EntitiesTab();
                    break;
                case CurrentTab.Map:
                    MapSpecificTab();
                    break;
                case CurrentTab.ESP:
                    EspTab();
                    break;
                case CurrentTab.Items:
                    ItemsTab();
                    break;
                case CurrentTab.Misc:
                    MiscTab();
                    break;
                case CurrentTab.Players:
                    PlayersTab();
                    break;

            }

            GUI.DragWindow();
        }

        private static void VisualsTab()
        {
            // draw visuals

            flashlight_toggle = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 70, 120, 30), flashlight_toggle, "Big Flashlight");
            fullbright = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 100, 120, 30), fullbright, "Fullbright");
            unlimitedUV = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 130, 130, 30), unlimitedUV, "Unlimited UV Light");
            crosshair = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 160, 130, 30), crosshair, "Crosshair");


            if (GUI.Button(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 190, 130, 30), "Flashlight Color"))
            {
                flashlight_colorpick = !flashlight_colorpick;
                MelonLogger.Msg("Flashlight color picker : " + flashlight_colorpick.ToString());

            }

            if (flashlight_colorpick)
            {
                Color flashlight_color_input = DevourClient.Helpers.GUIHelper.ColorPick("Flashlight Color", Settings.Settings.flashlight_color);
                Settings.Settings.flashlight_color = flashlight_color_input;

                if (Player.IsInGame())
                {
                    Hacks.Misc.FlashlightColor(flashlight_color_input);
                }
            }
        }

        private static void EntitiesTab()
        {
            //draw entities

            if (GUI.Button(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 70, 130, 30), "TP items to you"))
            {
                Hacks.Misc.TPItems();
                MelonLogger.Msg("TP Items!");
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 110, 130, 30), "Freeze azazel"))
            {
                Hacks.Misc.FreezeAzazel();
            }

            GUI.Label(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 150, 120, 30), "Azazel & Demons");

            // azazel
            GUI.enabled = Player.IsInGameOrLobby() && BoltNetwork.IsServer;

            if (GUI.Button(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 180, 60, 25), "Sam"))
            {
                Hacks.Misc.SpawnAzazel((PrefabId)BoltPrefabs.AzazelSam);
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 80, Settings.Settings.y + 180, 60, 25), "Molly"))
            {
                Hacks.Misc.SpawnAzazel((PrefabId)BoltPrefabs.SurvivalAzazelMolly);
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 150, Settings.Settings.y + 180, 60, 25), "Anna"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SurvivalAnnaNew, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 220, Settings.Settings.y + 180, 60, 25), "Zara"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.AzazelZara, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 290, Settings.Settings.y + 180, 60, 25), "Nathan"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.AzazelNathan, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 360, Settings.Settings.y + 180, 60, 25), "April"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.AzazelApril, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            // demon
            if (GUI.Button(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 220, 60, 25), "Ghost"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.Ghost, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 80, Settings.Settings.y + 220, 60, 25), "Inmate"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SurvivalInmate, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 150, Settings.Settings.y + 220, 60, 25), "Demon"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SurvivalDemon, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 220, Settings.Settings.y + 220, 60, 25), "Boar"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.Boar, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 290, Settings.Settings.y + 220, 60, 25), "Corpse"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.Corpse, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 360, Settings.Settings.y + 220, 60, 25), "Crow"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.Crow, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 430, Settings.Settings.y + 220, 60, 25), "Lump"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.ManorLump, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            GUI.enabled = true;

            // Animal

            if (GUI.Button(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 260, 60, 25), "Rat"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalRat, Player.GetPlayer().transform.position, Quaternion.identity);
                }

                if (Player.IsInGame() && !Player.IsPlayerCrawling())
                {
                    Hacks.Misc.CarryObject("SurvivalRat");
                }
            }

            GUI.enabled = BoltNetwork.IsServer && Player.IsInGameOrLobby();
            if (GUI.Button(new Rect(Settings.Settings.x + 150, Settings.Settings.y + 260, 60, 25), "Spider"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.Spider, Player.GetPlayer().transform.position, Quaternion.identity);
            }
            GUI.enabled = true;

            if (GUI.Button(new Rect(Settings.Settings.x + 220, Settings.Settings.y + 260, 60, 25), "Pig"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalPig, Player.GetPlayer().transform.position, Quaternion.identity);
                }

                if (Player.IsInGame() && !Player.IsPlayerCrawling())
                {
                    Hacks.Misc.CarryObject("SurvivalPig");
                }
            }
        }

        private static void MapSpecificTab()
        {
            Rect instantWinRect = new Rect(Settings.Settings.x + 10, Settings.Settings.y + 70, 150, 30);
            if (GUI.Button(instantWinRect, "Instant Win") && Player.IsInGame() && BoltNetwork.IsSinglePlayer)
            {
                Hacks.Misc.InstantWin();
                MelonLogger.Msg("EZ Win");
            }
            Helpers.GUIHelper.Tooltip(instantWinRect, "Risky: triggers the end state for current map. Singleplayer only.");

            if (GUI.Button(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 110, 150, 30), "Burn a ritual object"))
            {
                Hacks.Misc.BurnRitualObj(Helpers.Map.GetActiveScene(), false);
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 150, 150, 30), "Burn all ritual objects"))
            {
                Hacks.Misc.BurnRitualObj(Helpers.Map.GetActiveScene(), true);
            }

            switch (Helpers.Map.GetActiveScene())
            {
                case "Menu":
                    GUI.enabled = BoltNetwork.IsServer && !Player.IsInGame();
                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 70, 150, 30), "Force Start Game"))
                    {
                        Il2CppHorror.Menu menu = UnityEngine.Object.FindObjectOfType<Il2CppHorror.Menu>();
                        menu.OnLobbyStartButtonClick();
                    }
                    GUI.enabled = true;
                    break;

                case "Devour":
                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 70, 150, 30), "TP to Azazel"))
                    {
                        try
                        {
                            Il2Cpp.NolanBehaviour nb = Player.GetPlayer();

                            nb.TeleportTo(Helpers.Map.GetAzazel().transform.position, Quaternion.identity);
                        }
                        catch
                        {
                            MelonLogger.Msg("Azazel not found !");
                        }
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 110, 150, 30), "Despawn Demons"))
                    {
                        Hacks.Misc.DespawnDemons();
                    }
                    break;
                case "Molly":
                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 70, 150, 30), "TP to Azazel"))
                    {
                        try
                        {
                            Il2Cpp.NolanBehaviour nb = Player.GetPlayer();

                            nb.TeleportTo(Helpers.Map.GetAzazel().transform.position, Quaternion.identity);
                        }
                        catch
                        {
                            MelonLogger.Msg("Azazel not found !");
                        }
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 110, 150, 30), "Despawn Inmates"))
                    {
                        Hacks.Misc.DespawnDemons();
                    }
                    break;
                case "Inn":
                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 70, 150, 30), "TP to Azazel"))
                    {
                        try
                        {
                            Il2Cpp.NolanBehaviour nb = Player.GetPlayer();

                            nb.TeleportTo(Helpers.Map.GetAzazel().transform.position, Quaternion.identity);
                        }
                        catch
                        {
                            MelonLogger.Msg("Azazel not found !");
                        }
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 110, 150, 30), "Clean The Fountains"))
                    {
                        Hacks.Misc.CleanFountain();
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 150, 150, 30), "Despawn Spiders"))
                    {
                        Hacks.Misc.DespawnSpiders();
                    }
                    break;

                case "Town":
                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 70, 150, 30), "TP to Azazel"))
                    {
                        try
                        {
                            Il2Cpp.NolanBehaviour nb = Player.GetPlayer();

                            nb.TeleportTo(Helpers.Map.GetAzazel().transform.position, Quaternion.identity);
                        }
                        catch
                        {
                            MelonLogger.Msg("Azazel not found !");
                        }
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 110, 150, 30), "Despawn Ghosts"))
                    {
                        Hacks.Misc.DespawnGhosts();
                    }
                    break;

                case "Slaughterhouse":
                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 70, 150, 30), "TP to Azazel"))
                    {
                        try
                        {
                            Il2Cpp.NolanBehaviour nb = Player.GetPlayer();

                            nb.TeleportTo(Helpers.Map.GetAzazel().transform.position, Quaternion.identity);
                        }
                        catch
                        {
                            MelonLogger.Msg("Azazel not found !");
                        }
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 110, 150, 30), "Despawn Boars"))
                    {
                        Hacks.Misc.DespawnBoars();
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 150, 150, 30), "Despawn Corpses"))
                    {
                        Hacks.Misc.DespawnCorpses();
                    }
                    break;

                case "Manor":
                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 70, 150, 30), "TP to Azazel"))
                    {
                        try
                        {
                            Il2Cpp.NolanBehaviour nb = Player.GetPlayer();

                            nb.TeleportTo(Helpers.Map.GetAzazel().transform.position, Quaternion.identity);
                        }
                        catch
                        {
                            MelonLogger.Msg("Azazel not found !");
                        }
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 110, 150, 30), "Despawn Crows"))
                    {
                        Hacks.Misc.DespawnCrows();
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 190, Settings.Settings.y + 150, 150, 30), "Despawn Lumps"))
                    {
                        Hacks.Misc.DespawnLumps();
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 370, Settings.Settings.y + 70, 150, 30), "Switch realm"))
                    {
                        Il2Cpp.NolanBehaviour nb = Player.GetPlayer();
                        Vector3 pos = nb.transform.position;

                        ManorDeadRealmTrigger realm = Il2Cpp.ManorDeadRealmTrigger.FindObjectOfType<Il2Cpp.ManorDeadRealmTrigger>();
                        if (realm == null)
                        {
                            MelonLogger.Warning("realm was null.");
                            return;
                        }

                        if (realm.IsInDeadRealm)
                        {
                            // normal dimension
                            pos.x += 150f;// -10.216758f;
                            //pos.y = 0.009999979f;
                            //pos.z = -7.632657f;

                        }
                        else
                        {
                            // other dimension
                            pos.x -= 150f;//-160.03688f;
                            //pos.y = 0.010014875f;
                            //pos.z = -7.5686994f;
                        }

                        nb.locomotion.SetPosition(pos, false);
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 370, Settings.Settings.y + 110, 150, 30), "Switch realm (house)"))
                    {
                        Il2Cpp.NolanBehaviour nb = Player.GetPlayer();
                        Vector3 pos = nb.transform.position;

                        ManorDeadRealmTrigger realm = Il2Cpp.ManorDeadRealmTrigger.FindObjectOfType<Il2Cpp.ManorDeadRealmTrigger>();
                        if (realm == null)
                        {
                            MelonLogger.Warning("realm was null.");
                            return;
                        }

                        if (realm.IsInDeadRealm)
                        {
                            // normal dimension
                            pos.x = -10.216758f;
                            pos.y = 0.009999979f;
                            pos.z = -7.632657f;

                        }
                        else
                        {
                            // other dimension
                            pos.x = -160.03688f;
                            pos.y = 0.010014875f;
                            pos.z = -7.5686994f;
                        }

                        nb.locomotion.SetPosition(pos, false);
                    }

                    infinite_mirrors = GUI.Toggle(new Rect(Settings.Settings.x + 370, Settings.Settings.y + 150, 150, 20), infinite_mirrors, "Infinite mirrors");
                    break;
            }

            // load map
            GUI.Label(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 210, 100, 30), "Load Map: ");
            GUI.enabled = BoltNetwork.IsServer;

            if (GUI.Button(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 240, 100, 30), "Farmhouse"))
            {
                Helpers.Map.LoadMap("Devour");
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 120, Settings.Settings.y + 240, 100, 30), "Asylum"))
            {
                Helpers.Map.LoadMap("Molly");
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 230, Settings.Settings.y + 240, 100, 30), "Inn"))
            {
                Helpers.Map.LoadMap("Inn");
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 340, Settings.Settings.y + 240, 100, 30), "Town"))
            {
                Helpers.Map.LoadMap("Town");
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 450, Settings.Settings.y + 240, 100, 30), "Slaughterhouse"))
            {
                Helpers.Map.LoadMap("Slaughterhouse");
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 560, Settings.Settings.y + 240, 100, 30), "Manor"))
            {
                Helpers.Map.LoadMap("Manor");
            }

            GUI.enabled = true;
            // Region selection for server creation
            GUI.Label(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 270, 150, 20), "Region: 0=Best,1=US,2=EU,3=AS,4=AU");
            Settings.Settings.lobbyRegionIndex = (int)GUI.HorizontalSlider(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 290, 120, 10), Settings.Settings.lobbyRegionIndex, 0, 4);
        }

        private static void EspTab()
        {
            // Left column (players/Azazel)
            player_esp = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 70, 180, 22), player_esp, "Player ESP");
            player_skel_esp = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 96, 180, 22), player_skel_esp, "Skeleton ESP");
            player_snapline = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 122, 180, 22), player_snapline, "Player Snapline");
            player_nameplate_info = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 148, 260, 22), player_nameplate_info, "Nameplates: distance and crawling");
            Settings.Settings.espDistanceCulling = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 160, 200, 20), Settings.Settings.espDistanceCulling, "Enable distance culling");
            Settings.Settings.espMaxDistance = GUI.HorizontalSlider(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 180, 140, 12), Settings.Settings.espMaxDistance, 25f, 300f);
            GUI.Label(new Rect(Settings.Settings.x + 160, Settings.Settings.y + 176, 160, 20), $"Max {Settings.Settings.espMaxDistance:0}m");
            Settings.Settings.espDrawIntervalMs = GUI.HorizontalSlider(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 200, 140, 12), Settings.Settings.espDrawIntervalMs, 0f, 100f);
            GUI.Label(new Rect(Settings.Settings.x + 160, Settings.Settings.y + 196, 200, 20), $"Draw every {Settings.Settings.espDrawIntervalMs:0}ms");
            if (GUI.Button(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 225, 120, 20), Settings.Settings.espToggleKey == KeyCode.None ? "ESP Hotkey: None" : $"ESP Hotkey: {Settings.Settings.espToggleKey}"))
            {
                captureEspKey = true;
            }
            // Middle column (per-category culling)
            int leftX = (int)Settings.Settings.x + 300;
            Settings.Settings.cullPlayers = GUI.Toggle(new Rect(leftX, Settings.Settings.y + 70, 180, 20), Settings.Settings.cullPlayers, "Cull players");
            Settings.Settings.maxDistPlayers = GUI.HorizontalSlider(new Rect(leftX, Settings.Settings.y + 90, 140, 12), Settings.Settings.maxDistPlayers, 25f, 300f);
            GUI.Label(new Rect(leftX + 150, Settings.Settings.y + 86, 100, 20), $"{Settings.Settings.maxDistPlayers:0}m");

            Settings.Settings.cullAzazel = GUI.Toggle(new Rect(leftX, Settings.Settings.y + 110, 180, 20), Settings.Settings.cullAzazel, "Cull Azazel");
            Settings.Settings.maxDistAzazel = GUI.HorizontalSlider(new Rect(leftX, Settings.Settings.y + 130, 140, 12), Settings.Settings.maxDistAzazel, 25f, 400f);
            GUI.Label(new Rect(leftX + 150, Settings.Settings.y + 126, 100, 20), $"{Settings.Settings.maxDistAzazel:0}m");

            Settings.Settings.cullItems = GUI.Toggle(new Rect(leftX, Settings.Settings.y + 150, 180, 20), Settings.Settings.cullItems, "Cull items");
            Settings.Settings.maxDistItems = GUI.HorizontalSlider(new Rect(leftX, Settings.Settings.y + 170, 140, 12), Settings.Settings.maxDistItems, 25f, 300f);
            GUI.Label(new Rect(leftX + 150, Settings.Settings.y + 166, 100, 20), $"{Settings.Settings.maxDistItems:0}m");

            Settings.Settings.cullAnimals = GUI.Toggle(new Rect(leftX, Settings.Settings.y + 190, 180, 20), Settings.Settings.cullAnimals, "Cull animals");
            Settings.Settings.maxDistAnimals = GUI.HorizontalSlider(new Rect(leftX, Settings.Settings.y + 210, 140, 12), Settings.Settings.maxDistAnimals, 25f, 300f);
            GUI.Label(new Rect(leftX + 150, Settings.Settings.y + 206, 100, 20), $"{Settings.Settings.maxDistAnimals:0}m");

            Settings.Settings.cullDemons = GUI.Toggle(new Rect(leftX, Settings.Settings.y + 230, 180, 20), Settings.Settings.cullDemons, "Cull demons");
            Settings.Settings.maxDistDemons = GUI.HorizontalSlider(new Rect(leftX, Settings.Settings.y + 250, 140, 12), Settings.Settings.maxDistDemons, 25f, 400f);
            GUI.Label(new Rect(leftX + 150, Settings.Settings.y + 246, 100, 20), $"{Settings.Settings.maxDistDemons:0}m");
            if (GUI.Button(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 160, 130, 30), "Player ESP Color"))
            {
                player_esp_colorpick = !player_esp_colorpick;
            }

            if (player_esp_colorpick)
            {
                Color player_esp_color_input = GUIHelper.ColorPick("Player ESP Color", Settings.Settings.player_esp_color);
                Settings.Settings.player_esp_color = player_esp_color_input;
            }

            // Right column (Azazel)
            int rightX = (int)Settings.Settings.x + 540;
            azazel_esp = GUI.Toggle(new Rect(rightX, Settings.Settings.y + 70, 160, 20), azazel_esp, "Azazel ESP");
            azazel_skel_esp = GUI.Toggle(new Rect(rightX, Settings.Settings.y + 92, 160, 20), azazel_skel_esp, "Skeleton ESP");
            azazel_snapline = GUI.Toggle(new Rect(rightX, Settings.Settings.y + 114, 160, 20), azazel_snapline, "Azazel Snapline");
            if (GUI.Button(new Rect(rightX, Settings.Settings.y + 140, 140, 26), "Azazel ESP Color"))
            {
                azazel_esp_colorpick = !azazel_esp_colorpick;
            }

            if (azazel_esp_colorpick)
            {
                Color azazel_esp_color_input = GUIHelper.ColorPick("Azazel ESP Color", Settings.Settings.azazel_esp_color);
                Settings.Settings.azazel_esp_color = azazel_esp_color_input;
            }

            item_esp = GUI.Toggle(new Rect(rightX, Settings.Settings.y + 180, 160, 20), item_esp, "Item ESP");
            goat_rat_esp = GUI.Toggle(new Rect(rightX, Settings.Settings.y + 202, 160, 20), goat_rat_esp, "Goat/Rat ESP");
            demon_esp = GUI.Toggle(new Rect(rightX, Settings.Settings.y + 224, 160, 20), demon_esp, "Demon ESP");
        }

        private static void ItemsTab()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            GUILayout.Label("Items");

            Settings.Settings.itemsScrollPosition = GUILayout.BeginScrollView(Settings.Settings.itemsScrollPosition, GUILayout.Width(220), GUILayout.Height(190));

            if (GUILayout.Button("Hay"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalHay, Player.GetPlayer().transform.position, Quaternion.identity);
                }
                else
                {
                    Hacks.Misc.CarryObject("SurvivalHay");
                }
            }

            if (GUILayout.Button("First aid"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalFirstAid, Player.GetPlayer().transform.position, Quaternion.identity);
                }
                else
                {
                    Hacks.Misc.CarryObject("SurvivalFirstAid");
                }
            }

            if (GUILayout.Button("Battery"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalBattery, Player.GetPlayer().transform.position, Quaternion.identity);
                }
                else
                {
                    Hacks.Misc.CarryObject("SurvivalBattery");
                }
            }

            if (GUILayout.Button("Gasoline"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalGasoline, Player.GetPlayer().transform.position, Quaternion.identity);
                }
                else
                {
                    Hacks.Misc.CarryObject("SurvivalGasoline");
                }
            }

            if (GUILayout.Button("Fuse"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalFuse, Player.GetPlayer().transform.position, Quaternion.identity);
                }
                else
                {
                    Hacks.Misc.CarryObject("SurvivalFuse");
                }
            }

            if (GUILayout.Button("Food"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalRottenFood, Player.GetPlayer().transform.position, Quaternion.identity);
                }
                else
                {
                    Hacks.Misc.CarryObject("SurvivalRottenFood");
                }
            }

            if (GUILayout.Button("Bone"))
            {

                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalBone, Player.GetPlayer().transform.position, Quaternion.identity);
                }
                else
                {
                    Hacks.Misc.CarryObject("SurvivalBone");
                }
            }

            if (GUILayout.Button("Bleach"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalBleach, Player.GetPlayer().transform.position, Quaternion.identity);
                }
                else
                {
                    Hacks.Misc.CarryObject("SurvivalBleach");
                }
            }

            if (GUILayout.Button("Matchbox"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalMatchbox, Player.GetPlayer().transform.position, Quaternion.identity);
                }
                else
                {
                    Hacks.Misc.CarryObject("Matchbox-3");
                }
            }

            if (GUILayout.Button("Shovel"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalSpade, Player.GetPlayer().transform.position, Quaternion.identity);
                }
                else
                {
                    Hacks.Misc.CarryObject("SurvivalSpade");
                }
            }

            if (GUILayout.Button("Cake"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalCake, Player.GetPlayer().transform.position, Quaternion.identity);
                }
                else
                {
                    Hacks.Misc.CarryObject("SurvivalCake");
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            GUILayout.Label("Ritual Objects");

            Settings.Settings.rituelObjectsScrollPosition = GUILayout.BeginScrollView(Settings.Settings.rituelObjectsScrollPosition, GUILayout.Width(220), GUILayout.Height(190));

            if (GUILayout.Button("Egg-1"))
            {
                Hacks.Misc.CarryObject("Egg-Clean-1");
            }

            if (GUILayout.Button("Egg-2"))
            {
                Hacks.Misc.CarryObject("Egg-Clean-2");
            }

            if (GUILayout.Button("Egg-3"))
            {
                Hacks.Misc.CarryObject("Egg-Clean-3");
            }

            if (GUILayout.Button("Egg-4"))
            {
                Hacks.Misc.CarryObject("Egg-Clean-4");
            }

            if (GUILayout.Button("Egg-5"))
            {
                Hacks.Misc.CarryObject("Egg-Clean-5");
            }

            if (GUILayout.Button("Egg-6"))
            {
                Hacks.Misc.CarryObject("Egg-Clean-6");
            }

            if (GUILayout.Button("Egg-7"))
            {
                Hacks.Misc.CarryObject("Egg-Clean-7");
            }

            if (GUILayout.Button("Egg-8"))
            {
                Hacks.Misc.CarryObject("Egg-Clean-8");
            }

            if (GUILayout.Button("Egg-9"))
            {
                Hacks.Misc.CarryObject("Egg-Clean-9");
            }

            if (GUILayout.Button("Egg-10"))
            {
                Hacks.Misc.CarryObject("Egg-Clean-10");
            }

            if (GUILayout.Button("Ritual Book"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalRitualBook, Player.GetPlayer().transform.position, Quaternion.identity);
                }
                else
                {
                    Hacks.Misc.CarryObject("RitualBook-Active-1");
                }
            }

            if (GUILayout.Button("Dirty head"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalHead, Player.GetPlayer().transform.position, Quaternion.identity);
                }
                else
                {
                    Hacks.Misc.CarryObject("SurvivalHead");
                }
            }

            if (GUILayout.Button("Clean head"))
            {
                if (BoltNetwork.IsServer && !Player.IsInGame())
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SurvivalCleanHead, Player.GetPlayer().transform.position, Quaternion.identity);
                }
                else
                {
                    Hacks.Misc.CarryObject("SurvivalCleanHead");
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            GUILayout.Label("Spawnable Prefabs");

            Settings.Settings.stuffsScrollPosition = GUILayout.BeginScrollView(Settings.Settings.stuffsScrollPosition, GUILayout.Width(220), GUILayout.Height(190));
            GUI.enabled = BoltNetwork.IsServer;

            if (GUILayout.Button("Animal_Gate"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.Animal_Gate, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("AsylumDoor"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.AsylumDoor, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("AsylumDoubleDoor"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.AsylumDoubleDoor, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("AsylumWhiteDoor"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.AsylumWhiteDoor, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("DevourDoorBack"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.DevourDoorBack, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("DevourDoorMain"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.DevourDoorMain, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("DevourDoorRoom"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.DevourDoorRoom, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("Elevator_Door"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.Elevator_Door, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("InnDoor"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.InnDoor, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("InnDoubleDoor"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.InnDoubleDoor, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("InnShojiDoor"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.InnShojiDoor, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("InnShrine"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.InnShrine, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("InnWardrobe"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.InnWardrobe, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("InnWoodenDoor"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.InnWoodenDoor, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("PigExcrement"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.PigExcrement, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("SlaughterhouseFireEscapeDoor"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SlaughterhouseFireEscapeDoor, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("SurvivalAltarMolly"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SurvivalAltarMolly, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("SurvivalAltarSlaughterhouse"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SurvivalAltarSlaughterhouse, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("SurvivalAltarTown"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SurvivalAltarTown, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("SurvivalCultist"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SurvivalCultist, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("SurvivalKai"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SurvivalKai, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("SurvivalNathan"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SurvivalNathan, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("SurvivalMolly"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SurvivalMolly, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("SurvivalApril"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SurvivalApril, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("SurvivalFrank"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SurvivalFrank, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("SurvivalRose"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SurvivalRose, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("SurvivalSmashableWindow"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.SurvivalSmashableWindow, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("TownDoor"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.TownDoor, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("TownDoor2"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.TownDoor2, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("TownPentagram"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.TownPentagram, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("TrashCan"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.TrashCan, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("Truck_Shutter"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.Truck_Shutter, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("TV"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.TV, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            if (GUILayout.Button("Mirror"))
            {
                BoltNetwork.Instantiate(BoltPrefabs.ManorMirror, Player.GetPlayer().transform.position, Quaternion.identity);
            }

            GUI.enabled = true;
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private static void MiscTab()
        {
            // Cosmetic unlock toggle
            unlockCosmeticsEnabled = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 40, 200, 20), unlockCosmeticsEnabled, "Unlock cosmetics (menu)");
            Rect unlockBtnRect = new Rect(Settings.Settings.x + 10, Settings.Settings.y + 70, 150, 30);
            if (GUI.Button(unlockBtnRect, "Unlock Achievements"))
            {
                // Require Shift held to confirm
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    Thread AchievementsThread = new Thread(new ThreadStart(Hacks.Unlock.Achievements));
                    AchievementsThread.Start();
                    MelonLogger.Msg("Achievements Unlocked!");
                }
                else
                {
                    MelonLogger.Warning("Hold SHIFT and click to confirm achievements unlock.");
                }
            }
            Helpers.GUIHelper.Tooltip(unlockBtnRect, "Risky: permanently sets many Steam stats/achievements. Hold SHIFT to confirm.");

            if (GUI.Button(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 110, 150, 30), "Unlock Doors"))
            {
                Hacks.Unlock.Doors();

                MelonLogger.Msg("Doors Unlocked!");
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 150, 150, 30), "TP Keys") && Player.IsInGame())
            {
                Hacks.Misc.TPKeys();
                MelonLogger.Msg("Here are your keys!");
            }

            if (GUI.Button(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 190, 150, 30), "Make Random Noise"))
            {
                Hacks.Misc.PlaySound();
                MelonLogger.Msg("Playing a random sound!");
            }

            spam_message = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 240, 140, 30), spam_message, "Chat spam");
            Settings.Settings.spamIntervalSeconds = GUI.HorizontalSlider(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 270, 120, 12), Settings.Settings.spamIntervalSeconds, 0.1f, 3.0f);
            GUI.Label(new Rect(Settings.Settings.x + 140, Settings.Settings.y + 265, 180, 20), $"Spam every {Settings.Settings.spamIntervalSeconds:0.0}s");
            _spamIntervalSeconds = Settings.Settings.spamIntervalSeconds;
            change_steam_name = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 270, 140, 30), change_steam_name, "Change Steam Name");
            change_server_name = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 300, 140, 30), change_server_name, "Change Server Name");
            _walkInLobby = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 330, 140, 30), _walkInLobby, "Walk In Lobby");
            _IsAutoRespawn = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 360, 140, 30), _IsAutoRespawn, "Auto Respawn");

            fly = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 400, 40, 20), fly, "Fly");
            if (GUI.Button(new Rect(Settings.Settings.x + 60, Settings.Settings.y + 400, 40, 20), Settings.Settings.flyKey.ToString()))
            {
                captureFlyKey = true;
            }

            fly_speed = GUI.HorizontalSlider(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 430, 100, 10), fly_speed, 5f, 20f);
            GUI.Label(new Rect(Settings.Settings.x + 120, Settings.Settings.y + 425, 100, 30), ((int)fly_speed).ToString());


            // Risky group (collapsed by default)
            showRiskySection = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 450, 180, 20), showRiskySection, "Show Risky (Profile & EXP)");
            Rect expRiskRect1 = new Rect(Settings.Settings.x + 10, Settings.Settings.y + 470, 150, 20);
            Rect expRiskRect2 = new Rect(Settings.Settings.x + 10, Settings.Settings.y + 540, 150, 20);
            if (showRiskySection)
            {
                spoofLevel = GUI.Toggle(expRiskRect1, spoofLevel, "Spoof Level");
                spoofLevelValue = GUI.HorizontalSlider(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 500, 120, 12), spoofLevelValue, 0f, 666f);
                GUI.Label(new Rect(Settings.Settings.x + 140, Settings.Settings.y + 495, 100, 20), ((int)spoofLevelValue).ToString());

                exp_modifier = GUI.Toggle(expRiskRect2, exp_modifier, "EXP Modifier");
                exp = GUI.HorizontalSlider(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 570, 120, 12), exp, 1000f, 3000f);
                GUI.Label(new Rect(Settings.Settings.x + 140, Settings.Settings.y + 565, 100, 20), ((int)exp).ToString());

                Helpers.GUIHelper.Tooltip(expRiskRect1, "Risky: may persist in profile.");
                Helpers.GUIHelper.Tooltip(expRiskRect2, "Risky: changes EXP at end, persistent.");
            }


            fastMove = GUI.Toggle(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 610, 150, 20), fastMove, "Player Speed");
            if (GUI.Button(new Rect(Settings.Settings.x + 165, Settings.Settings.y + 610, 60, 20), Settings.Settings.speedToggleKey == KeyCode.None ? "Key:None" : $"Key:{Settings.Settings.speedToggleKey}"))
            {
                captureSpeedKey = true;
            }
            _PlayerSpeedMultiplier = GUI.HorizontalSlider(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 640, 100, 10), _PlayerSpeedMultiplier, (int)1f, (int)10f);
            GUI.Label(new Rect(Settings.Settings.x + 120, Settings.Settings.y + 635, 100, 30), ((int)_PlayerSpeedMultiplier).ToString());

            // Lobby/Host group (collapsed by default)
            showLobbySection = GUI.Toggle(new Rect(Settings.Settings.x + 300, Settings.Settings.y + 50, 160, 20), showLobbySection, "Show Lobby & Host");
            if (showLobbySection)
            {
                GUI.Label(new Rect(Settings.Settings.x + 300, Settings.Settings.y + 70, 150, 20), "Max players");
                Settings.Settings.privateLobby = GUI.Toggle(new Rect(Settings.Settings.x + 300, Settings.Settings.y + 90, 150, 20), Settings.Settings.privateLobby, "Private lobby");
                lobbySize = GUI.HorizontalSlider(new Rect(Settings.Settings.x + 300, Settings.Settings.y + 112, 140, 12), lobbySize, (int)0f, (int)30f);
                GUI.Label(new Rect(Settings.Settings.x + 450, Settings.Settings.y + 108, 60, 20), ((int)lobbySize).ToString());

                if (GUI.Button(new Rect(Settings.Settings.x + 300, Settings.Settings.y + 134, 150, 26), "Create server"))
                {
                    MelonLogger.Msg("Creating the server...");
                    Hacks.Misc.CreateCustomizedLobby((int)lobbySize, Settings.Settings.privateLobby);
                    MelonLogger.Msg("Done !");
                }
            }

            Helpers.GUIHelper.DrawTooltip();
            Helpers.GUIHelper.DrawTooltip();
        }

        private static void PlayersTab()
        {
            if (Helpers.Map.GetActiveScene() != "Menu")
            {
                GUI.Label(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 70, 150, 30), "Player list:");
                int i = 0;
                foreach (BasePlayer bp in Entities.Players)
                {
                    if (bp == null || bp.Name == "")
                    {
                        MelonLogger.Warning("players is null");
                        continue;
                    }

                    string display = string.IsNullOrEmpty(bp.Id) ? bp.Name : $"{bp.Name} [{bp.Id}]";
                    GUI.Label(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 110 + i, 250, 30), display);

                    GUI.enabled = BoltNetwork.IsServer;
                    if (GUI.Button(new Rect(Settings.Settings.x + 70, Settings.Settings.y + 105 + i, 60, 30), "Kill"))
                    {
                        bp.Kill();
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 140, Settings.Settings.y + 105 + i, 60, 30), "Revive"))
                    {
                        bp.Revive();
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 210, Settings.Settings.y + 105 + i, 90, 30), "Jumpscare"))
                    {
                        bp.Jumpscare();
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 310, Settings.Settings.y + 105 + i, 60, 30), "TP to"))
                    {
                        bp.TP();
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 380, Settings.Settings.y + 105 + i, 100, 30), "Lock in cage"))
                    {
                        bp.LockInCage();
                    }

                    GUI.enabled = true;

                    if (GUI.Button(new Rect(Settings.Settings.x + 490, Settings.Settings.y + 105 + i, 90, 30), "TP Azazel"))
                    {
                        bp.TPAzazel();
                    }

                    if (Helpers.Map.GetActiveScene() == "Town")
                    {
                        GUI.enabled = BoltNetwork.IsServer;
                        if (GUI.Button(new Rect(Settings.Settings.x + 590, Settings.Settings.y + 105 + i, 90, 30), "Shoot Player"))
                        {
                            bp.ShootPlayer();
                        }
                        GUI.enabled = true;
                    }

                    if (GUI.Button(new Rect(Settings.Settings.x + 690, Settings.Settings.y + 105 + i, 80, 30), "Copy ID"))
                    {
                        if (!string.IsNullOrEmpty(bp.Id))
                        {
                            GUIUtility.systemCopyBuffer = bp.Id;
                            MelonLogger.Msg($"Copied ID for {bp.Name}");
                        }
                    }

                    i += 30;
                }
            }
            else
            {
                GUI.Label(new Rect(Settings.Settings.x + 10, Settings.Settings.y + 70, 150, 30), "Waiting for the game to start.");
            }
        }

    }
}
