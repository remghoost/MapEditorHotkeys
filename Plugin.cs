using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[BepInPlugin("com.yourname.ravenfield.mapeditorhotkeys", "Map Editor Hotkeys", "1.1.0")]
public class MapEditorHotkeysPlugin : BaseUnityPlugin
{
    private readonly Dictionary<KeyCode, Toggle> hotkeyMap = new();
    private bool initialized;

    private static readonly Dictionary<string, KeyCode> DefaultBindings = new()
    {
        { "Translate", KeyCode.V },
        { "Rotate",    KeyCode.R },
        { "Scale",     KeyCode.F },
        { "Terrain",   KeyCode.T },
        { "Place",     KeyCode.G }
    };

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "MapEditor")
        {
            initialized = false;
            hotkeyMap.Clear();
            return;
        }

        if (!initialized)
        {
            TryInitialize();
            return;
        }

        foreach (var kv in hotkeyMap)
        {
            if (Input.GetKeyDown(kv.Key))
            {
                kv.Value.isOn = true;
            }
        }
    }

    private void TryInitialize()
    {
        var toolsMenu = GameObject.Find("Late Awake/MapEditor UI/Canvas/ToolsMenu");
        if (!toolsMenu)
            return;

        var toggles = toolsMenu.GetComponentsInChildren<Toggle>(true);
        if (toggles == null || toggles.Length == 0)
            return;

        hotkeyMap.Clear();

        foreach (var toggle in toggles)
        {
            if (DefaultBindings.TryGetValue(toggle.gameObject.name, out var key))
            {
                hotkeyMap[key] = toggle;
            }
        }

        if (hotkeyMap.Count > 0)
        {
            initialized = true;
        }
    }
}
