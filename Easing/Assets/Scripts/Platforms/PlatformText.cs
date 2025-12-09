using TMPro;
using UnityEngine;
using DG.Tweening;

public class PlatformText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private OneWayPlatform platform;

    [Header("Style")]
    [SerializeField] private Color layerAboveColor = Color.green;
    [SerializeField] private Color layerBelowColor = Color.yellow;

    private int lastLayer = -1;

    void Update()
    {
        if (platform == null || textDisplay == null)
            return;

        int currentLayer = platform.gameObject.layer;
        string layerName = LayerMask.LayerToName(currentLayer);

        // Color based on layer
        Color layerColor = DetermineLayerColor(currentLayer);
        string coloredLayerName = WrapColor(layerName, layerColor);

        textDisplay.text = $"Layer: {coloredLayerName}";

        // Trigger animation when layer changes
        if (currentLayer != lastLayer && lastLayer != -1)
        {
            PopUp();
        }

        lastLayer = currentLayer;
    }

    private Color DetermineLayerColor(int layer)
    {
        // You can customize this logic based on your layer setup
        // Assuming lower layer numbers are "above" and higher are "below"
        return layer <= 6 ? layerAboveColor : layerBelowColor;
    }

    void PopUp()
    {
        if (textDisplay == null)
            return;

        textDisplay.transform.DOKill();
        textDisplay.transform.localScale = Vector3.one * 0.5f;
        textDisplay.transform
            .DOScale(1f, 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(RunPulseAnimation);
    }

    void RunPulseAnimation()
    {
        if (textDisplay == null)
            return;

        textDisplay.transform.DOKill();
        textDisplay.transform.localScale = Vector3.one;
        textDisplay.transform
            .DOScale(1.02f, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    string ToHex(Color c) => ColorUtility.ToHtmlStringRGB(c);

    string WrapColor(string text, Color c) =>
        $"<color=#{ToHex(c)}>{text}</color>";

    public void EnableText() => textDisplay.gameObject.SetActive(true);
    public void DisableText() => textDisplay.gameObject.SetActive(false);
}

