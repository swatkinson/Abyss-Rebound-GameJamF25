using System;
using System.Collections.Generic;
using UnityEngine;

public class FunctionRandomizer : MonoBehaviour
{
    [SerializeField] private PlayerInput player;

    // One delegate slot per gesture
    private Action[] gestureBindings;
    private List<Action> movementFunctions;

    private readonly string[] gestureNames = new[]
    {
        "ThumbRightL","ThumbDownL","ThumbLeftL","HandOpenL","ThumbUpL",
        "OpenHandRightL","OpenHandLeftL","FistUpL","FistRightL","FistLeftL",
        "ThumbUpR","ThumbRightR","ThumbDownR","ThumbLeftR","OpenHandR",
        "OpenHandLeftR","OpenHandRightR","FistUpR","FistLeftR","FistRightR"
    };

    private void Awake()
    {
        gestureBindings = new Action[gestureNames.Length];
        movementFunctions = new List<Action>
        {
            player.MoveLeft,
            player.MoveRight,
            player.Jump
        };

        Randomize();
    }

    public void Randomize()
    {
        // Clear all bindings
        for (int i = 0; i < gestureBindings.Length; i++)
            gestureBindings[i] = null;

        // Build and shuffle index list (correct Fisher–Yates)
        var rng = new System.Random();
        var slots = new List<int>(gestureBindings.Length);
        for (int i = 0; i < gestureBindings.Length; i++) slots.Add(i);

        for (int i = slots.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (slots[i], slots[j]) = (slots[j], slots[i]);
        }

        // Assign exactly three unique gestures to MoveLeft/MoveRight/Jump
        for (int m = 0; m < movementFunctions.Count; m++)
        {
            int idx = slots[m];
            string oldName = "null";
            gestureBindings[idx] = movementFunctions[m];
            string newName = movementFunctions[m].Method.Name;
            Debug.Log($"{gestureNames[idx]}: {oldName} -> {newName}");
        }
    }

    // --- Gesture entry points (call these from your gesture system) ---
    public void ThumbRightL() => InvokeGesture(0);
    public void ThumbDownL() => InvokeGesture(1);
    public void ThumbLeftL() => InvokeGesture(2);
    public void HandOpenL() => InvokeGesture(3);
    public void ThumbUpL() => InvokeGesture(4);
    public void OpenHandRightL() => InvokeGesture(5);
    public void OpenHandLeftL() => InvokeGesture(6);
    public void FistUpL() => InvokeGesture(7);
    public void FistRightL() => InvokeGesture(8);
    public void FistLeftL() => InvokeGesture(9);
    public void ThumbUpR() => InvokeGesture(10);
    public void ThumbRightR() => InvokeGesture(11);
    public void ThumbDownR() => InvokeGesture(12);
    public void ThumbLeftR() => InvokeGesture(13);
    public void OpenHandR() => InvokeGesture(14);
    public void OpenHandLeftR() => InvokeGesture(15);
    public void OpenHandRightR() => InvokeGesture(16);
    public void FistUpR() => InvokeGesture(17);
    public void FistLeftR() => InvokeGesture(18);
    public void FistRightR() => InvokeGesture(19);

    private void InvokeGesture(int index)
    {
        var a = gestureBindings[index];
        if (a != null) a.Invoke();
        else Debug.Log($"{gestureNames[index]} unassigned.");
    }
}
