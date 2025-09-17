using UnityEngine;
using Il2CppOpsive.UltimateCharacterController.Character;
using System.Collections.Generic;
using System.Collections;
using MelonLoader;
using Il2CppPhoton.Bolt;
using Il2CppPhoton;

namespace DevourClient.Helpers
{
    public class BasePlayer
    {
        public GameObject p_GameObject { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Id { get; set; } = default!;

        public void Kill()
        {
            if (p_GameObject == null)
            {
                return;
            }

            // Prevent client from raising server-only events
            if (!BoltNetwork.IsServer)
            {
                MelonLogger.Msg("You need to be server !");
                return;
            }

            Il2Cpp.SurvivalAzazelBehaviour sab = Il2Cpp.SurvivalAzazelBehaviour.FindObjectOfType<Il2Cpp.SurvivalAzazelBehaviour>();

            if (sab == null)
            {
                return;
            }

            sab.OnKnockout(sab.gameObject, p_GameObject);
        }

        public void Revive()
        {
            if (p_GameObject == null)
            {
                return;
            }

            Il2Cpp.NolanBehaviour nb = p_GameObject.GetComponent<Il2Cpp.NolanBehaviour>();
            Il2Cpp.SurvivalReviveInteractable _reviveInteractable = UnityEngine.Object.FindObjectOfType<Il2Cpp.SurvivalReviveInteractable>();

            _reviveInteractable.Interact(nb.gameObject);
        }

        public void Jumpscare()
        {
            if (!BoltNetwork.IsServer)
            {
                MelonLogger.Msg("You need to be server !");
                return;
            }

            if (p_GameObject == null)
            {
                return;
            }

            Il2Cpp.SurvivalAzazelBehaviour sab = Il2Cpp.SurvivalAzazelBehaviour.FindObjectOfType<Il2Cpp.SurvivalAzazelBehaviour>();

            if (sab == null)
            {
                return;
            }

            sab.OnPickedUpPlayer(sab.gameObject, p_GameObject, false);

            /*
            MelonLogger.Msg(Name);
            Il2Cpp.JumpScare _jumpscare = UnityEngine.Object.FindObjectOfType<Il2Cpp.JumpScare>();
            _jumpscare.player = p_GameObject;
            _jumpscare.Activate(p_GameObject.GetComponent<BoltEntity>());
            */
        }

        public void LockInCage()
        {
            if (p_GameObject == null)
            {
                return;
            }

            // Only server can instantiate networked prefabs
            if (!BoltNetwork.IsServer)
            {
                MelonLogger.Msg("You need to be server !");
                return;
            }

            BoltNetwork.Instantiate(BoltPrefabs.Cage, p_GameObject.transform.position, Quaternion.identity);
        }

        public void TP()
        {
            if (p_GameObject == null)
            {
                return;
            }

            Il2Cpp.NolanBehaviour nb = Player.GetPlayer();
            nb.TeleportTo(p_GameObject.transform.position, Quaternion.identity);
        }

        public void TPAzazel()
        {
            if (p_GameObject == null)
            {
                return;
            }

            UltimateCharacterLocomotion ucl = Helpers.Map.GetAzazel().GetComponent<UltimateCharacterLocomotion>();

            if (ucl)
            {
                ucl.SetPosition(p_GameObject.transform.position);
            }
            else
            {
                MelonLogger.Error("Azazel not found!");
                return;
            }
        }

        public void ShootPlayer()
        {
            if (!BoltNetwork.IsServer)
            {
                MelonLogger.Msg("You need to be server !");
                return;
            }

            if (p_GameObject == null)
            {
                return;
            }

            Il2Cpp.AzazelSamBehaviour _azazelSam = UnityEngine.Object.FindObjectOfType<Il2Cpp.AzazelSamBehaviour>();

            if (_azazelSam)
            {
                _azazelSam.OnShootPlayer(p_GameObject, true);
            }
        }
    }
    public class Player
    {
        public static bool IsInGame()
        {
            try
            {
                // Devour 5.2.11 compatibility - multiple detection methods
                Il2Cpp.OptionsHelpers optionsHelpers = UnityEngine.Object.FindObjectOfType<Il2Cpp.OptionsHelpers>();
                if (optionsHelpers != null)
                    return optionsHelpers.inGame;
                
                // New method for 5.2.11+
                Il2Cpp.GameUI gameUI = UnityEngine.Object.FindObjectOfType<Il2Cpp.GameUI>();
                if (gameUI != null)
                {
                    // Use reflection or alternative properties based on actual GameUI structure
                    var menuOpenField = gameUI.GetType().GetProperty("isMenuOpen") ?? gameUI.GetType().GetProperty("menuOpen");
                    var inGameField = gameUI.GetType().GetProperty("isInGame") ?? gameUI.GetType().GetProperty("inGame");
                    
                    if (menuOpenField != null && inGameField != null)
                    {
                        bool isMenuOpen = (bool)menuOpenField.GetValue(gameUI);
                        bool isInGame = (bool)inGameField.GetValue(gameUI);
                        return !isMenuOpen && isInGame;
                    }
                    
                    // Fallback to alternative detection methods - check for local NolanBehaviour
                    return UnityEngine.Object.FindObjectOfType<Il2Cpp.NolanBehaviour>() != null;
                }
                
                // No reliable GameManager type in 5.2.11 via Il2Cpp; avoid direct reference
                
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsInGameOrLobby()
        {
            return GetPlayer() != null;
        }

        public static Il2Cpp.NolanBehaviour? GetPlayer()
        {
            if (Entities.LocalPlayer_?.p_GameObject == null)
            {
                return null;
            }

            var nolan = Entities.LocalPlayer_.p_GameObject.GetComponent<Il2Cpp.NolanBehaviour>();
            if (nolan == null)
            {
                // Try to find player using alternative methods for newer versions
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in players)
                {
                    var nb = player.GetComponent<Il2Cpp.NolanBehaviour>();
                    if (nb != null && nb.entity != null && nb.entity.IsOwner)
                    {
                        return nb;
                    }
                }
            }
            return nolan;
        }

        public static bool IsPlayerCrawling()
        {
            Il2Cpp.NolanBehaviour nb = Player.GetPlayer();

            if (nb == null)
            {
                return false;
            }

            return nb.IsCrawling();
        }

    }

    public class Entities
    {
        public static int MAX_PLAYERS = 4; //will change by calling CreateCustomizedLobby

        public static BasePlayer LocalPlayer_ = new BasePlayer();
        public static BasePlayer[] Players = default!;
        public static Il2Cpp.GoatBehaviour[] GoatsAndRats = default!;
        public static Il2Cpp.SurvivalInteractable[] SurvivalInteractables = default!;
        public static Il2Cpp.KeyBehaviour[] Keys = default!;
        public static Il2Cpp.SurvivalDemonBehaviour[] Demons = default!;
        public static Il2Cpp.SpiderBehaviour[] Spiders = default!;
        public static Il2Cpp.GhostBehaviour[] Ghosts = default!;
        public static Il2Cpp.SurvivalAzazelBehaviour[] Azazels = default!;
        public static Il2Cpp.BoarBehaviour[] Boars = default!;
        public static Il2Cpp.CorpseBehaviour[] Corpses = default!;
        public static Il2Cpp.CrowBehaviour[] Crows = default!;
        public static Il2Cpp.ManorLumpController[] Lumps = default!;

        public static IEnumerator GetLocalPlayer()
        {
            while (true)
            {
                try
                {
                    // Devour 5.2.11 compatibility - enhanced player detection
                    GameObject[] currentPlayers = GameObject.FindGameObjectsWithTag("Player");

                    for (int i = 0; i < currentPlayers.Length; i++)
                    {
                        if (currentPlayers[i] == null) continue;
                        
                        var nolan = currentPlayers[i].GetComponent<Il2Cpp.NolanBehaviour>();
                        if (nolan != null)
                        {
                            // Check both entity and photon view for 5.2.11+
                            bool isLocal = false;
                            
                            try
                            {
                                if (nolan.entity != null && nolan.entity.IsOwner)
                                    isLocal = true;
                            }
                            catch { }
                            
                            // Avoid Il2CppPhoton.PhotonView dependency; rely on entity ownership only
                            
                            if (isLocal)
                            {
                                LocalPlayer_.p_GameObject = currentPlayers[i];
                                break;
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"Error in GetLocalPlayer: {ex.Message}");
                }

                yield return new WaitForSeconds(3f); // Reduced delay for 5.2.11
            }
        }

        public static IEnumerator GetAllPlayers()
        {
            while (true)
            {
                try
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    Players = new BasePlayer[players.Length];

                    int i = 0;
                    foreach (GameObject p in players)
                    {
                        string player_name = "Unknown";
                        string player_id = "-1";

                        try
                        {
                            Il2Cpp.DissonancePlayerTracking dpt = p.gameObject.GetComponent<Il2Cpp.DissonancePlayerTracking>();
                            if (dpt != null && dpt.state != null)
                            {
                                player_name = dpt.state.PlayerName ?? "Unknown";
                                player_id = dpt.state.PlayerId ?? "-1";
                            }
                        }
                        catch { }

                        if (Players[i] == null)
                        {
                            Players[i] = new BasePlayer();
                        }

                        Players[i].Id = player_id;
                        Players[i].Name = player_name;
                        Players[i].p_GameObject = p;

                        i++;
                    }
                }
                catch (System.Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"Error in GetAllPlayers: {ex.Message}");
                    Players = new BasePlayer[0];
                }

                // Wait 5 seconds before caching objects again.
                yield return new WaitForSeconds(5f);
            }
        }
        public static IEnumerator GetGoatsAndRats()
        {
            while (true)
            {
                try
                {
                    GoatsAndRats = Il2Cpp.GoatBehaviour.FindObjectsOfType<Il2Cpp.GoatBehaviour>() ?? new Il2Cpp.GoatBehaviour[0];
                }
                catch (System.Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"Error in GetGoatsAndRats: {ex.Message}");
                    GoatsAndRats = new Il2Cpp.GoatBehaviour[0];
                }
                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetSurvivalInteractables()
        {
            while (true)
            {
                try
                {
                    SurvivalInteractables = Il2Cpp.SurvivalInteractable.FindObjectsOfType<Il2Cpp.SurvivalInteractable>() ?? new Il2Cpp.SurvivalInteractable[0];
                }
                catch (System.Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"Error in GetSurvivalInteractables: {ex.Message}");
                    SurvivalInteractables = new Il2Cpp.SurvivalInteractable[0];
                }
                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetKeys()
        {
            while (true)
            {
                try
                {
                    Keys = Il2Cpp.KeyBehaviour.FindObjectsOfType<Il2Cpp.KeyBehaviour>() ?? new Il2Cpp.KeyBehaviour[0];
                }
                catch (System.Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"Error in GetKeys: {ex.Message}");
                    Keys = new Il2Cpp.KeyBehaviour[0];
                }
                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetDemons()
        {
            while (true)
            {
                try
                {
                    Demons = Il2Cpp.SurvivalDemonBehaviour.FindObjectsOfType<Il2Cpp.SurvivalDemonBehaviour>() ?? new Il2Cpp.SurvivalDemonBehaviour[0];
                }
                catch (System.Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"Error in GetDemons: {ex.Message}");
                    Demons = new Il2Cpp.SurvivalDemonBehaviour[0];
                }
                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetSpiders()
        {
            while (true)
            {
                try
                {
                    Spiders = Il2Cpp.SpiderBehaviour.FindObjectsOfType<Il2Cpp.SpiderBehaviour>() ?? new Il2Cpp.SpiderBehaviour[0];
                }
                catch (System.Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"Error in GetSpiders: {ex.Message}");
                    Spiders = new Il2Cpp.SpiderBehaviour[0];
                }
                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetGhosts()
        {
            while (true)
            {
                try
                {
                    Ghosts = Il2Cpp.GhostBehaviour.FindObjectsOfType<Il2Cpp.GhostBehaviour>() ?? new Il2Cpp.GhostBehaviour[0];
                }
                catch (System.Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"Error in GetGhosts: {ex.Message}");
                    Ghosts = new Il2Cpp.GhostBehaviour[0];
                }
                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetBoars()
        {
            while (true)
            {
                try
                {
                    Boars = Il2Cpp.BoarBehaviour.FindObjectsOfType<Il2Cpp.BoarBehaviour>() ?? new Il2Cpp.BoarBehaviour[0];
                }
                catch (System.Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"Error in GetBoars: {ex.Message}");
                    Boars = new Il2Cpp.BoarBehaviour[0];
                }
                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetCorpses()
        {
            while (true)
            {
                try
                {
                    Corpses = Il2Cpp.CorpseBehaviour.FindObjectsOfType<Il2Cpp.CorpseBehaviour>() ?? new Il2Cpp.CorpseBehaviour[0];
                }
                catch (System.Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"Error in GetCorpses: {ex.Message}");
                    Corpses = new Il2Cpp.CorpseBehaviour[0];
                }
                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetCrows()
        {
            while (true)
            {
                try
                {
                    Crows = Il2Cpp.CrowBehaviour.FindObjectsOfType<Il2Cpp.CrowBehaviour>() ?? new Il2Cpp.CrowBehaviour[0];
                }
                catch (System.Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"Error in GetCrows: {ex.Message}");
                    Crows = new Il2Cpp.CrowBehaviour[0];
                }
                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetLumps()
        {
            while (true)
            {
                try
                {
                    Lumps = Il2Cpp.ManorLumpController.FindObjectsOfType<Il2Cpp.ManorLumpController>() ?? new Il2Cpp.ManorLumpController[0];
                }
                catch (System.Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"Error in GetLumps: {ex.Message}");
                    Lumps = new Il2Cpp.ManorLumpController[0];
                }
                yield return new WaitForSeconds(5f);
            }
        }

        public static IEnumerator GetAzazels()
        {
            while (true)
            {
                try
                {
                    Azazels = Il2Cpp.SurvivalAzazelBehaviour.FindObjectsOfType<Il2Cpp.SurvivalAzazelBehaviour>() ?? new Il2Cpp.SurvivalAzazelBehaviour[0];
                }
                catch (System.Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"Error in GetAzazels: {ex.Message}");
                    Azazels = new Il2Cpp.SurvivalAzazelBehaviour[0];
                }
                yield return new WaitForSeconds(5f);
            }
        }
    }
}
