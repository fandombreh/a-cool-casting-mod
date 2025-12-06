using BepInEx;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using z3roCastingMod.Librarys;

namespace sigmarizz
{
    [BepInPlugin("com.sigmarizz.coolcasting", "CoolCasting", "1.0.0")]
    internal class Class1 : BaseUnityPlugin
    {
        static bool openUI;
        static Rect castingrect = new Rect(20, 20, 400, 400);
        static Vector2 Scroll;
        static GUILayoutOption width = GUILayout.Width(371);
        public static bool startedCasting;
        public static bool init;
        public static bool ExampleToggle;
        public static bool kf;
        public static bool MS;
        public static bool NT;
        public static bool ShowFPS;
        public static bool ShowPlatform;
        public static float ExampleSlider;
        public static float Slider2;
        public static float Slider3;
        public static float Slider4;
        public static float Slider = 60;
        public static bool StartCam;
        public static string room;
        public static bool rl;
        public static float la = 0.01f;
        public static string NameTagLoadFix;
        public static bool FirstPerson;
        public static TMP_FontAsset firebird;
        public static TMP_FontAsset sakana;
        public static TMP_FontAsset western;
        public static TMP_FontAsset exa;
        public static TMP_FontAsset CurrentFont;
        public static CastingCamera cc;
        public static CinemachineVirtualCamera CMVirtualCamera;
        public static Camera ThirdPersonCamera;
        public static GameObject CameraGO;
        public static GameObject CMVirtualCameraGO;
        public static GameObject Nametags;
        void OnGUI()
        {
            if (openUI == true)
            {
                GUI.backgroundColor = Color.aquamarine;
                castingrect = GUILayout.Window(4234, castingrect, CastingWindow, "KCM Casting Mod");
            }
            if (ExampleToggle == true)
            {
                GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), CastingCamera.Render, ScaleMode.ScaleAndCrop, false);
            }

        }
        void Update()
        {

            if (GorillaLocomotion.GTPlayer.Instance != null && init == false)
            {
                Nametags = GameObject.Instantiate(AssetHandler.LoadObjectFromBundle("sigmarizz.Resources.ab", "Nametags"));
                firebird = AssetHandler.LoadTMPFontFromBundle("sigmarizz.Resources.ab", "Firebird-Regular SDF");
                sakana = AssetHandler.LoadTMPFontFromBundle("sigmarizz.Resources.ab", "sakana SDF");
                exa = AssetHandler.LoadTMPFontFromBundle("sigmarizz.Resources.ab", "exa SDF");
                western = AssetHandler.LoadTMPFontFromBundle("sigmarizz.Resources.ab", "believe-western SDF");
                CurrentFont = firebird;
                init = true;
            }
                if (UnityInput.Current.GetKeyDown(KeyCode.Tab)) { openUI = !openUI; }
            CameraGO = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera");
            CMVirtualCameraGO = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera/CM vcam1");
            if (ExampleToggle == true)
            {
                if (StartCam == false)
                {
                    cc = CameraGO.AddComponent<CastingCamera>();
                    cc.InstantiateCamera();
                    StartCam = true;
                }
            }
            if (kf == true)
            {
                KeyFly();
            }
            if (NT == true)
            {
                Nametag();
            }
            if (cc != null)
            {
                cc.CameraHeight = ExampleSlider;
                cc.FieldOfView = Slider;
                cc.UseMotionSmoothing = MS;
                cc.MotionSmoothing = Slider2;
                cc.UseRotationSmoothing = MS;
                cc.RotationSmoothing = Slider3;
            }
            PhotonNetworkController.Instance.disableAFKKick = true;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                vrrig.lerpValueBody = (rl ? la : 0.185f);
                vrrig.lerpValueFingers = (rl ? la : 0.185f);
            }
        }
        void CastingWindow(int id)
        {
            GUI.backgroundColor = Color.aquamarine;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            Scroll = GUILayout.BeginScrollView(Scroll, GUIStyle.none, GUIStyle.none);
            kf = GUILayout.Toggle(kf, "Key Fly", width);
            NT = GUILayout.Toggle(NT, "NameTags", width);
            if (NT == true)
            {
                
                if (GUILayout.Button("Font : Sakana"))
                {
                    CurrentFont = sakana;
                }
                if (GUILayout.Button("Font : Firebird"))
                {
                    CurrentFont = firebird;
                }
                if (GUILayout.Button("Font : Exalyptus"))
                {
                    CurrentFont = exa;
                }
                if (GUILayout.Button("Font : Western"))
                {
                    CurrentFont = western;
                }
                ShowFPS = GUILayout.Toggle(ShowFPS, "Show FPS", width);
                ShowPlatform = GUILayout.Toggle(ShowPlatform, "Show Platform", width);
            }
            ExampleToggle = GUILayout.Toggle(ExampleToggle, "Start Casting", width);
            room = GUILayout.TextField(room);
            if (GUILayout.Button("Join Room", width))
            {
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(room.ToUpper(), JoinType.Solo);
            }
            if (GUILayout.Button("Disconnect", width))
            {
                PhotonNetwork.Disconnect();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Select Gamemode Infection", width))
            {
                SetGameMode(GameModeType.Infection);
            }
            if (GUILayout.Button("Select Gamemode Casual", width))
            {
                SetGameMode(GameModeType.Casual);
            }

            if (GUILayout.Button("Select Gamemode Paintbrawl", width))
            {
                SetGameMode(GameModeType.Paintbrawl);
            }
            if (GUILayout.Button("Select Gamemode SuperInfection", width))
            {
                SetGameMode(GameModeType.SuperInfect);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Make Time Day", width))
            {
                BetterDayNightManager.instance.SetTimeOfDay(3);
            }
            if (GUILayout.Button("Make Time Night", width))
            {
                BetterDayNightManager.instance.SetTimeOfDay(0);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Clear Weather", width))
            {
                SetWeather(false);
            }
            if (GUILayout.Button("Rainy Weather", width))
            {
                SetWeather(true);
            }
            GUILayout.Space(10);
            FirstPerson = GUILayout.Toggle(FirstPerson, "Enable First Person", width);
            if (FirstPerson == true)
            {
                CastingCamera.viewMode = CastingCamera.ViewMode.FirstPerson;
            }
            if (FirstPerson == false)
            {
                CastingCamera.viewMode = CastingCamera.ViewMode.ThirdPerson;
            }
            GUILayout.Label($"Camera Height: {ExampleSlider:F1}");
            ExampleSlider = GUILayout.HorizontalSlider(ExampleSlider, 0, 5, width);
            MS = GUILayout.Toggle(MS, "Motion Smoothing", width);
            if (MS == true)
            {
                GUILayout.Label($"Motion Smoothing: {Slider2:F3}");
                Slider2 = GUILayout.HorizontalSlider(Slider2, 0, 0.997f, width);
                GUILayout.Label($"Rotation Smoothing: {Slider3:F3}");
                Slider3 = GUILayout.HorizontalSlider(Slider3, 0, 0.997f, width);
            }
            rl = GUILayout.Toggle(rl, "Rig Lerping", width);
            if (rl == true)
            {
                GUILayout.Label($"Rig Lerping: {la:F2}");
                la = GUILayout.HorizontalSlider(la, 0.01f, 1f, width);
            }

            GUILayout.Label($"FOV: {(int)Slider}");
            Slider = GUILayout.HorizontalSlider(Slider, 60, 120, width);
            GUILayout.Space(10);
            GUILayout.Label("Player List");
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (GUILayout.Button(rig.OwningNetPlayer.NickName, width))
                {
                    cc.Target = rig;
                }
            }

            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            UnityEngine.GUI.DragWindow();
        }

        public static void KeyFly()
        {
            GTPlayer player = GTPlayer.Instance;
            if (UnityInput.Current.GetKey(KeyCode.W)) { player.headCollider.transform.position += player.headCollider.transform.forward * 10 * Time.deltaTime; }
            if (UnityInput.Current.GetKey(KeyCode.S)) { player.headCollider.transform.position += -player.headCollider.transform.forward * 10 * Time.deltaTime; }
            if (UnityInput.Current.GetKey(KeyCode.D)) { player.headCollider.transform.position += player.headCollider.transform.right * 10 * Time.deltaTime; }
            if (UnityInput.Current.GetKey(KeyCode.A)) { player.headCollider.transform.position += -player.headCollider.transform.right * 10 * Time.deltaTime; }

            if (UnityInput.Current.GetKey(KeyCode.Space)) { player.headCollider.transform.position += player.headCollider.transform.up * 10 * Time.deltaTime; }
            if (UnityInput.Current.GetKey(KeyCode.LeftControl)) { player.headCollider.transform.position += -player.headCollider.transform.up * 10 * Time.deltaTime; }

            if (UnityInput.Current.GetKey(KeyCode.E)) { player.headCollider.transform.Rotate(new Vector3(0, 2f, 0)); }
            if (UnityInput.Current.GetKey(KeyCode.Q)) { player.headCollider.transform.Rotate(new Vector3(0, -2f, 0)); }
            player.bodyCollider.attachedRigidbody.velocity = Vector3.zero;
        }
        public static void SetGameMode(GameModeType gmt)
        {
            GorillaComputer.instance.SetGameModeWithoutButton(gmt.ToString());
        }
        public static void SetWeather(bool rain)
        {
            for (int i = 1; i < BetterDayNightManager.instance.weatherCycle.Length; i++)
            {
                if (rain != true)
                {
                    BetterDayNightManager.instance.weatherCycle[i] = BetterDayNightManager.WeatherType.None;
                }
                else
                {
                    BetterDayNightManager.instance.weatherCycle[i] = BetterDayNightManager.WeatherType.Raining;
                }
            }
        }
        public static void Nametag()
        {
            if (PhotonNetwork.InRoom)
            {
                if (Nametags == null)
                {
                    Nametags = GameObject.Instantiate(AssetHandler.LoadObjectFromBundle("sigmarizz.Resources.ab", "Nametags"));
                }
                if (Nametags != null)
                {
                    foreach (VRRig rig in GorillaParent.instance.vrrigs)
                    {
                        GameObject nametag = GameObject.Instantiate(Nametags);
                        nametag.transform.position = rig.transform.position + new Vector3(0f, 0.4f, 0f);
                        nametag.transform.localScale =  new Vector3(0.4f, 0.4f, 0.4f);
                        nametag.transform.LookAt(ExampleToggle ? cc.testGO.transform.position : GTPlayer.Instance.transform.position);
                        nametag.transform.Find("NametagText").GetComponent<TMP_Text>().fontStyle = FontStyles.Italic;
                        nametag.transform.Find("NametagText").GetComponent<TMP_Text>().font = CurrentFont;
                        nametag.transform.Find("NametagText").GetComponent<TMP_Text>().text = rig.OwningNetPlayer.NickName;

                        nametag.transform.Find("FPSText").GetComponent<TMP_Text>().fontStyle = FontStyles.Italic;
                        nametag.transform.Find("FPSText").GetComponent<TMP_Text>().font = CurrentFont;
                        nametag.transform.Find("FPSText").GetComponent<TMP_Text>().text = (string)(ShowFPS ? $"FPS: {(int)Traverse.Create(rig).Field("fps").GetValue()}" : "");

                        if (ShowPlatform == true)
                        {
                            if (rig.concatStringOfCosmeticsAllowed.Contains("FIRST LOGIN"))
                            {
                                nametag.transform.Find("Meta").GetComponent<TMP_Text>().fontStyle = FontStyles.Italic;
                                nametag.transform.Find("Meta").GetComponent<TMP_Text>().font = CurrentFont;
                                nametag.transform.Find("Meta").GetComponent<TMP_Text>().text = "";

                                nametag.transform.Find("Steam").GetComponent<TMP_Text>().fontStyle = FontStyles.Italic;
                                nametag.transform.Find("Steam").GetComponent<TMP_Text>().font = CurrentFont;
                                nametag.transform.Find("Steam").GetComponent<TMP_Text>().text = "STEAM";
                            }
                            if (!rig.concatStringOfCosmeticsAllowed.Contains("FIRST LOGIN"))
                            {
                                nametag.transform.Find("Meta").GetComponent<TMP_Text>().fontStyle = FontStyles.Italic;
                                nametag.transform.Find("Meta").GetComponent<TMP_Text>().font = CurrentFont;
                                nametag.transform.Find("Meta").GetComponent<TMP_Text>().text = "META";

                                nametag.transform.Find("Steam").GetComponent<TMP_Text>().fontStyle = FontStyles.Italic;
                                nametag.transform.Find("Steam").GetComponent<TMP_Text>().font = CurrentFont;
                                nametag.transform.Find("Steam").GetComponent<TMP_Text>().text = "";

                            }
                        }
                        if (ShowPlatform == false)
                        {
                            nametag.transform.Find("Steam").GetComponent<TMP_Text>().text = "";
                            nametag.transform.Find("Meta").GetComponent<TMP_Text>().text = "";
                        }
                        GameObject.Destroy(nametag, Time.deltaTime);
                    }
                }
                
            }
        }
    }
}
