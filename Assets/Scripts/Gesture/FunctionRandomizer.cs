using System;
using System.Collections.Generic;
using UnityEngine;

public class FunctionRandomizer : MonoBehaviour
{
    [SerializeField] private PlayerInput player;

    // Event UI can subscribe to
    public event Action<Dictionary<ActionType, GestureType>> OnMappingsChanged;

    // Read-only view for pull-style access
    public IReadOnlyDictionary<ActionType, GestureType> CurrentMap => _actionToGesture;

    // Internals
    private Action[] _gestureBindings;                               // index by (int)GestureType
    private readonly List<Action> _movementFunctions = new();        // order must match ActionType enum order
    private readonly Dictionary<ActionType, GestureType> _actionToGesture = new();

    private void Awake()
    {
        _gestureBindings = new Action[Enum.GetValues(typeof(GestureType)).Length];

        _movementFunctions.Add(player.MoveLeft);   // ActionType.MoveLeft (0)
        _movementFunctions.Add(player.MoveRight);  // ActionType.MoveRight (1)
        _movementFunctions.Add(player.Jump);       // ActionType.Jump (2)
    }

    private void Start()
    {
        Randomize();
    }

    public void Randomize()
    {
        // Clear bindings
        for (int i = 0; i < _gestureBindings.Length; i++) _gestureBindings[i] = null;
        _actionToGesture.Clear();

        // Make a shuffled list of all gesture indices
        var rng = new System.Random();
        var slots = new List<int>(_gestureBindings.Length);
        for (int i = 0; i < _gestureBindings.Length; i++) slots.Add(i);
        for (int i = slots.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (slots[i], slots[j]) = (slots[j], slots[i]);
        }

        // Assign first 3 to MoveLeft/MoveRight/Jump (unique by construction)
        for (int m = 0; m < _movementFunctions.Count; m++)
        {
            int idx = slots[m];
            _gestureBindings[idx] = _movementFunctions[m];

            var actionType = (ActionType)m;
            var gestureType = (GestureType)idx;

            _actionToGesture[actionType] = gestureType;

            Debug.Log($"{actionType} ← {gestureType}");
        }

        OnMappingsChanged?.Invoke(new Dictionary<ActionType, GestureType>(_actionToGesture));
    }

    // --- Gesture entry points (called by your vision/gesture system) ---
    public void TwoFingerPoint() => InvokeGesture(GestureType.TwoFingerPoint);
    public void FourFingers() => InvokeGesture(GestureType.FourFingers);
    public void Thirteen() => InvokeGesture(GestureType.Thirteen);
    public void VulcanSalute() => InvokeGesture(GestureType.VulcanSalute);
    public void Bunny() => InvokeGesture(GestureType.Bunny);
    public void DrEvil() => InvokeGesture(GestureType.DrEvil);
    public void Ermm() => InvokeGesture(GestureType.Ermm);
    public void FingerGun() => InvokeGesture(GestureType.FingerGun);
    public void Fist() => InvokeGesture(GestureType.Fist);
    public void Ok() => InvokeGesture(GestureType.Ok);
    public void Peace() => InvokeGesture(GestureType.Peace);
    public void Pointing() => InvokeGesture(GestureType.Pointing);
    public void Rad() => InvokeGesture(GestureType.Rad);
    public void Rock() => InvokeGesture(GestureType.Rock);
    public void Tea() => InvokeGesture(GestureType.Tea);

    private void InvokeGesture(GestureType type)
    {
        var a = _gestureBindings[(int)type];
        if (a != null) a.Invoke();
        else Debug.Log($"{type} unassigned.");
    }
}
