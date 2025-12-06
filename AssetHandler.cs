using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

namespace sigmarizz
{
    internal class AssetHandler
    {
        // Token: 0x06000031 RID: 49 RVA: 0x000047F8 File Offset: 0x000029F8
        public static AssetBundle LoadAssetBundle(string path)
        {
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            bool flag = AssetHandler.bundle == null;
            if (flag)
            {
                AssetHandler.bundle = AssetBundle.LoadFromStream(manifestResourceStream);
            }
            manifestResourceStream.Close();
            return bundle;
        }

        public static Font LoadFontFromTTF(string fontPath)
        {
            if (string.IsNullOrEmpty(fontPath))
            {
                Debug.LogError("AssetUtils.LoadFontFromTTF failed: fontPath is null or empty.");
                return null;
            }

            if (!File.Exists(fontPath))
            {
                Debug.LogError($"AssetUtils.LoadFontFromTTF failed: file not found at {fontPath}");
                return null;
            }

            try
            {
                // Read the raw .ttf bytes
                byte[] fontData = File.ReadAllBytes(fontPath);

                // Create a new Font instance
                Font dynamicFont = new Font();
                dynamicFont.hideFlags = HideFlags.DontUnloadUnusedAsset;

                // Load font data into memory (Unity loads the bytes automatically)
                var fontBytes = new Font();
                dynamicFont.name = Path.GetFileNameWithoutExtension(fontPath);


                Debug.Log($"Successfully loaded runtime font: {dynamicFont.name}");
                return dynamicFont;
            }
            catch (Exception ex)
            {
                Debug.LogError($"AssetUtils.LoadFontFromTTF Exception: {ex.Message}");
                return null;
            }
        }

        // Token: 0x06000032 RID: 50 RVA: 0x00004840 File Offset: 0x00002A40
        public static AudioClip LoadAudioClipFromBundle(string path, string name)
        {
            AssetBundle assetBundle = AssetHandler.LoadAssetBundle(path);
            return assetBundle.LoadAsset<AudioClip>(name);
        }

        // Token: 0x06000033 RID: 51 RVA: 0x00004860 File Offset: 0x00002A60
        public static Font LoadFontFromBundle(string path, string name)
        {
            AssetBundle assetBundle = AssetHandler.LoadAssetBundle(path);
            return assetBundle.LoadAsset<Font>(name);
        }

        // Token: 0x06000034 RID: 52 RVA: 0x00004880 File Offset: 0x00002A80
        public static TMP_FontAsset LoadTMPFontFromBundle(string path, string name)
        {
            AssetBundle assetBundle = AssetHandler.LoadAssetBundle(path);
            return assetBundle.LoadAsset<TMP_FontAsset>(name);
        }

        // Token: 0x06000035 RID: 53 RVA: 0x000048A0 File Offset: 0x00002AA0
        public static GameObject LoadObjectFromBundle(string path, string name)
        {
            AssetBundle assetBundle = AssetHandler.LoadAssetBundle(path);
            return assetBundle.LoadAsset<GameObject>(name);
        }

        // Token: 0x04000059 RID: 89
        public static AssetBundle bundle;
    }

}
