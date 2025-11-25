using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerTimerVisualizer : MonoBehaviour
{
    [SerializeField] private PlayerController2D player;

    [Header("Coyote Time UI")]
    [SerializeField] private Slider coyoteSlider;
    [SerializeField] private Image coyoteFillImage;
    [SerializeField] private TextMeshProUGUI coyoteText;
    [SerializeField] private Color coyoteActiveColor = Color.green;
    [SerializeField] private Color coyoteInactiveColor = Color.gray;

    [Header("Jump Buffer UI")]
    [SerializeField] private Slider jumpBufferSlider;
    [SerializeField] private Image jumpBufferFillImage;
    [SerializeField] private TextMeshProUGUI jumpBufferText;
    [SerializeField] private Color jumpBufferActiveColor = Color.yellow;
    [SerializeField] private Color jumpBufferInactiveColor = Color.gray;

    private void Update()
    {
        if (player == null) return;

        UpdateCoyoteUI();
        UpdateJumpBufferUI();
    }

    private void UpdateCoyoteUI()
    {
        float current = player.CoyoteTimeCounter;
        float max = player.MaxCoyoteTime;
        float normalized = Mathf.Clamp01(max > 0 ? current / max : 0);

        if (coyoteSlider != null) coyoteSlider.value = normalized;
        if (coyoteText != null) coyoteText.text = $"Coyote: {current:F2}s";

        if (coyoteFillImage != null)
        {
            coyoteFillImage.color = current > 0 ? coyoteActiveColor : coyoteInactiveColor;
        }
    }

    private void UpdateJumpBufferUI()
    {
        float current = player.JumpBufferCounter;
        float max = player.MaxJumpBufferTime;
        float normalized = Mathf.Clamp01(max > 0 ? current / max : 0);

        if (jumpBufferSlider != null) jumpBufferSlider.value = normalized;
        if (jumpBufferText != null) jumpBufferText.text = $"Buffer: {current:F2}s";

        if (jumpBufferFillImage != null)
        {
            jumpBufferFillImage.color = current > 0 ? jumpBufferActiveColor : jumpBufferInactiveColor;
        }
    }
}

