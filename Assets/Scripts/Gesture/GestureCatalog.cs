using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gestures/Gesture Catalog")]
public class GestureCatalog : ScriptableObject
{
    [Serializable]
    public struct Item
    {
        public GestureType type;
        public Sprite sprite;
    }

    [SerializeField] private List<Item> items = new();

    private Dictionary<GestureType, Sprite> _cache;

    private void OnEnable()
    {
        _cache = new Dictionary<GestureType, Sprite>(items.Count);
        foreach (var it in items) _cache[it.type] = it.sprite;
    }

    public Sprite Get(GestureType t) => _cache != null && _cache.TryGetValue(t, out var s) ? s : null;
}
