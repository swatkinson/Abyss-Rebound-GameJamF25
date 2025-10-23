using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GestureHUD : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private FunctionRandomizer randomizer;
    [SerializeField] private GestureCatalog catalog;

    [Header("UI Slots")]
    [SerializeField] private Image leftIcon;
    [SerializeField] private Image rightIcon;
    [SerializeField] private Image jumpIcon;
    [SerializeField] private TMP_Text leftLabel;   // optional; can be pre-set in scene
    [SerializeField] private TMP_Text rightLabel;  // optional
    [SerializeField] private TMP_Text jumpLabel;   // optional

    private void OnEnable()
    {
        if (randomizer != null)
            randomizer.OnMappingsChanged += ApplyMapping;
    }

    private void Start()
    {
        // Pull once in case the event was fired before we enabled
        if (randomizer != null && randomizer.CurrentMap is { } map)
            ApplyMapping(new Dictionary<ActionType, GestureType>(map));
    }

    private void OnDisable()
    {
        if (randomizer != null)
            randomizer.OnMappingsChanged -= ApplyMapping;
    }

    private void ApplyMapping(Dictionary<ActionType, GestureType> map)
    {
        if (catalog == null) return;

        if (map.TryGetValue(ActionType.MoveLeft, out var gLeft) && leftIcon)
            leftIcon.sprite = catalog.Get(gLeft);

        if (map.TryGetValue(ActionType.MoveRight, out var gRight) && rightIcon)
            rightIcon.sprite = catalog.Get(gRight);

        if (map.TryGetValue(ActionType.Jump, out var gJump) && jumpIcon)
            jumpIcon.sprite = catalog.Get(gJump);

        if (leftLabel) leftLabel.text = "Left";
        if (rightLabel) rightLabel.text = "Right";
        if (jumpLabel) jumpLabel.text = "Jump";
    }
}
