using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIPanelID
{
    MainMenu,
    Inventory,
    Dialogue,
    Settings,
    Store,
    Quest,
    // add more as neededddddddddddddddddddddddddddddddddddd
}

public class UIManager : Singleton<UIManager>
{

    [System.Serializable]
    public struct PanelEntry
    {
        public UIPanelID id;
        public GameObject panelPrefab;
    }

    [Header("UI Panels to Register")]
    [SerializeField] private PanelEntry[] panelEntries;

    private Dictionary<UIPanelID, GameObject> panels = new Dictionary<UIPanelID, GameObject>();

    protected override void Awake()
    {
        base.Awake();

        // Instantiate and hide all panels
        foreach (var entry in panelEntries)
        {
            var go = Instantiate(entry.panelPrefab, transform);
            go.name = entry.id.ToString();
            go.SetActive(false);
            panels.Add(entry.id, go);
        }
    }

    public GameObject GetPanelGO(UIPanelID id)
    {
        GameObject panel = panels[id];

        return panel;
    }

    // Show only the specified panel, hide all others
    public void ShowExclusive(UIPanelID id)
    {
        foreach (var kv in panels)
            kv.Value.SetActive(kv.Key == id);
    }

    // Toggle the specified panel On/Off
    public void TogglePanel(UIPanelID id)
    {
        if (!panels.ContainsKey(id)) return;
        var go = panels[id];
        go.SetActive(!go.activeSelf);
    }

    // Show the specified panel
    public void ShowPanel(UIPanelID id)
    {
        if (panels.TryGetValue(id, out var go))
            go.SetActive(true);
    }

    // Hide the specified panel
    public void HidePanel(UIPanelID id)
    {
        if (panels.TryGetValue(id, out var go))
            go.SetActive(false);
    }

    public bool IsAnyUIOpen
    {
        get
        {
            foreach (var panel in panels.Values)
            {
                if (panel.activeSelf)
                    return true;
            }
            return false;
        }
    }
}
