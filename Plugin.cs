using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

[BepInPlugin("com.yourname.ravenfield.mapeditorhotkeys", "Map Editor Hotkeys (Debug)", "1.0.1")]
public class MapEditorHotkeysDebug : BaseUnityPlugin
{
    private readonly Dictionary<KeyCode, Toggle> hotkeyMap = new();
    private bool initialized;

    private float sceneLogTimer;

    void Awake()
    {
        Logger.LogInfo("🔥 MapEditorHotkeysDebug loaded");
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
            if (Input.GetKeyDown(kv.Key))
            {
                // Logger.LogInfo($"Key pressed: {kv.Key} → {kv.Value.name}");
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
        hotkeyMap[KeyCode.V] = translate;
        hotkeyMap[KeyCode.R] = rotate;
        hotkeyMap[KeyCode.F] = scale;
        hotkeyMap[KeyCode.T] = terrain;
        hotkeyMap[KeyCode.G] = place;

        initialized = true;
        // Logger.LogInfo("🎉 MapEditor hotkeys initialized successfully");
    }
}
