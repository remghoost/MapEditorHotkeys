using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

[BepInPlugin("com.remghoost.ravenfield.mapeditorhotkeys", "Map Editor Hotkeys (Debug)", "1.2.0")]
public class MapEditorHotkeysDebug : BaseUnityPlugin
{
    private readonly Dictionary<ConfigEntry<KeyCode>, Toggle> hotkeyMap = new();
    private bool initialized;

    private float sceneLogTimer;
    
    // UI references
    private GameObject assetBrowser;
    private InputField searchInputField;

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
            assetBrowser = null;
            searchInputField = null;
            return;
        }

        if (!initialized)
        {
            // Logger.LogInfo("Attempting MapEditor UI initialization...");
            TryInitialize();
            return;
        }

        // Check if user is typing in the search field
        if (IsInputFieldFocused())
        {
            return; // Don't process hotkeys while typing
        }

        foreach (var kv in hotkeyMap)
        {
            if (Input.GetKeyDown(kv.Key.Value))
            {
                // Logger.LogInfo($"Key pressed: {kv.Key.Value} → {kv.Value.name}");
                
                // Special handling for Place tool - toggle AssetBrowser
                if (kv.Key == placeKey && assetBrowser != null)
                {
                    if (kv.Value.isOn && assetBrowser.activeSelf)
                    {
                        // Place is already active and AssetBrowser is open - close it
                        assetBrowser.SetActive(false);
                        kv.Value.isOn = false;
                    }
                    else
                    {
                        // Activate Place tool (which opens AssetBrowser)
                        kv.Value.isOn = true;
                    }
                }
                else
                {
                    kv.Value.isOn = true;
                }
            }
        }
    }

    private bool IsInputFieldFocused()
    {
        // Check if search input field exists and is focused
        if (searchInputField != null && searchInputField.isFocused)
        {
            return true;
        }

        // Fallback: check if any InputField has focus
        // This helps catch edge cases where searchInputField reference is stale
        var focused = UnityEngine.EventSystems.EventSystem.current?.currentSelectedGameObject;
        if (focused != null && focused.GetComponent<InputField>() != null)
        {
            return true;
        }

        return false;
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

        // Find AssetBrowser window
        var assetBrowserObj = GameObject.Find("Late Awake/MapEditor UI/Canvas/Windows #0/AssetBrowser");
        if (assetBrowserObj)
        {
            assetBrowser = assetBrowserObj;
            // Logger.LogInfo("✅ AssetBrowser found");
            
            // Find search input field
            var searchInputTransform = assetBrowserObj.transform.Find("Window/Content/Search Input");
            if (searchInputTransform)
            {
                searchInputField = searchInputTransform.GetComponent<InputField>();
                if (searchInputField)
                {
                    // Logger.LogInfo("✅ Search InputField found");
                }
                else
                {
                    // Logger.LogWarning("❌ Search InputField component not found");
                }
            }
            else
            {
                // Logger.LogWarning("❌ Search Input transform not found");
            }
        }
        else
        {
            // Logger.LogWarning("❌ AssetBrowser not found (may not exist until Place is activated)");
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