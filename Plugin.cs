using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

[BepInPlugin("com.remghoost.ravenfield.mapeditorhotkeys", "Map Editor Hotkeys (Debug)", "1.1.0")]
public class MapEditorHotkeysDebug : BaseUnityPlugin
{
    private readonly Dictionary<ConfigEntry<KeyCode>, Toggle> hotkeyMap = new();
    private bool initialized;

    private float sceneLogTimer;

    // Configuration entries
    private ConfigEntry<KeyCode> translateKey;
    private ConfigEntry<KeyCode> rotateKey;
    private ConfigEntry<KeyCode> scaleKey;
    private ConfigEntry<KeyCode> terrainKey;
    private ConfigEntry<KeyCode> placeKey;

    void Awake()
    {
        // Create configuration entries with default values
        translateKey = Config.Bind("Hotkeys", 
            "TranslateKey", 
            KeyCode.V, 
            "Key to activate Translate tool");
        
        rotateKey = Config.Bind("Hotkeys", 
            "RotateKey", 
            KeyCode.R, 
            "Key to activate Rotate tool");
        
        scaleKey = Config.Bind("Hotkeys", 
            "ScaleKey", 
            KeyCode.F, 
            "Key to activate Scale tool");
        
        terrainKey = Config.Bind("Hotkeys", 
            "TerrainKey", 
            KeyCode.T, 
            "Key to activate Terrain tool");
        
        placeKey = Config.Bind("Hotkeys", 
            "PlaceKey", 
            KeyCode.G, 
            "Key to activate Place tool");

        Logger.LogInfo("🔥 MapEditorHotkeysDebug loaded");
        Logger.LogInfo($"Hotkeys configured - Translate: {translateKey.Value}, Rotate: {rotateKey.Value}, Scale: {scaleKey.Value}, Terrain: {terrainKey.Value}, Place: {placeKey.Value}");
    }

    void Update()
    {
        // Prove Update is running + scene name
        // sceneLogTimer += Time.unscaledDeltaTime;
        // if (sceneLogTimer > 2f)
        // {
        //     sceneLogTimer = 0f;
        //     Logger.LogInfo($"Current scene: {SceneManager.GetActiveScene().name}");
        // }

        if (SceneManager.GetActiveScene().name != "MapEditor")
        {
            if (initialized)
                // Logger.LogInfo("Left MapEditor scene, clearing state");

            initialized = false;
            hotkeyMap.Clear();
            return;
        }

        if (!initialized)
        {
            // Logger.LogInfo("Attempting MapEditor UI initialization...");
            TryInitialize();
            return;
        }

        foreach (var kv in hotkeyMap)
        {
            if (Input.GetKeyDown(kv.Key.Value))
            {
                // Logger.LogInfo($"Key pressed: {kv.Key.Value} → {kv.Value.name}");
                kv.Value.isOn = true;
            }
        }
    }

    private void TryInitialize()
    {
        var toolsMenu = GameObject.Find("Late Awake/MapEditor UI/Canvas/ToolsMenu");
        if (!toolsMenu)
        {
            // Logger.LogWarning("❌ ToolsMenu not found");
            return;
        }

        var middle = toolsMenu.transform.Find("Middle");
        if (!middle)
        {
            // Logger.LogWarning("❌ ToolsMenu/Middle not found");
            return;
        }

        // Logger.LogInfo("✅ ToolsMenu/Middle found");

        Toggle FindToggle(string name)
        {
            var t = middle.Find(name);
            if (!t)
            {
                // Logger.LogWarning($"❌ Toggle transform not found: {name}");
                return null;
            }

            var toggle = t.GetComponent<Toggle>();
            if (!toggle)
            {
                // Logger.LogWarning($"❌ Toggle component missing: {name}");
                return null;
            }

            // Logger.LogInfo($"✅ Toggle found: {name}");
            return toggle;
        }

        var translate = FindToggle("Translate");
        var rotate    = FindToggle("Rotate");
        var scale     = FindToggle("Scale");
        var terrain   = FindToggle("Terrain");
        var place     = FindToggle("Place");

        if (!translate || !rotate || !scale || !terrain || !place)
        {
            // Logger.LogWarning("⚠️ Not all toggles found yet");
            return;
        }

        hotkeyMap.Clear();
        hotkeyMap[translateKey] = translate;
        hotkeyMap[rotateKey] = rotate;
        hotkeyMap[scaleKey] = scale;
        hotkeyMap[terrainKey] = terrain;
        hotkeyMap[placeKey] = place;

        initialized = true;
        // Logger.LogInfo("🎉 MapEditor hotkeys initialized successfully");
    }
}