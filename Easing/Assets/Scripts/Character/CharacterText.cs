using TMPro;
using UnityEngine;
using DG.Tweening;

public enum TextType { None, Position, Velocity, YPos }

public class CharacterText : MonoBehaviour
{

    [SerializeField] private TextType textType = TextType.Position;
    [SerializeField] private TextMeshProUGUI topText;

    [SerializeField] private PlayerController characterController;


    [Header("Velocity Style")]
    [SerializeField] private float headerSizeMultiplier = 1.3f;
    [SerializeField] private Color headerColor = Color.white;
    [SerializeField] private Color inputColor = Color.cyan;
    [SerializeField] private Color groundColor = Color.yellow;

    void Awake()
    {
        if (textType == TextType.None)
        {
            topText.gameObject.SetActive(false);
        }
    }

    void UpdateTextType(TextType newType)
    {
        textType = newType;
        if (textType == TextType.None && topText != null)
        {
            topText.gameObject.SetActive(false);
        }
        else if (topText != null)
        {
            topText.gameObject.SetActive(true);
            PopUp();
        }
    }

    void Update()
    {
        if (characterController == null || topText == null)
            return;

        switch (textType)
        {
            case TextType.Position:
                Vector3 pos = characterController.transform.position;
                topText.text = $"X={pos.x:F2} Y={pos.y:F2}";
                break;
            case TextType.YPos:
                float yPos = characterController.transform.position.y;
                topText.text = $"Y={yPos:F2}";
                break;
            case TextType.Velocity:
                float vel = characterController.CurrentHorizontalSpeed;
                float input = characterController.CurrentInputSpeed;
                float ground = characterController.GroundSpeed;

                string header = WrapSize(WrapColor($"Vel ({vel:F2}) =", headerColor), headerSizeMultiplier);
                string inputText = WrapColor($"Input ({input:F2}) +", inputColor);
                string groundText = WrapColor($"Ground ({ground:F2})", groundColor);

                topText.text = $"{header}\n{inputText} \n{groundText}";
                break;
        }
    }

    void PopUp()
    {
        if (topText == null)
            return;

        topText.transform.DOKill();
        topText.transform.localScale = Vector3.one * 0.5f;
        topText.transform
            .DOScale(1f, 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(RunPulseAnimation);
    }

    void RunPulseAnimation()
    {
        if (topText == null)
            return;

        topText.transform.DOKill();
        topText.transform.localScale = Vector3.one;
        topText.transform
            .DOScale(1.02f, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    string ToHex(Color c) => ColorUtility.ToHtmlStringRGB(c);

    string WrapColor(string text, Color c) =>
    $"<color=#{ToHex(c)}>{text}</color>";

    string WrapSize(string text, float multiplier) =>
        $"<size={multiplier * 100f}%>{text}</size>";

    public void SetTextTypePosition() => UpdateTextType(TextType.Position);
    public void SetTextTypeVelocity() => UpdateTextType(TextType.Velocity);
    public void SetTextTypeYPos() => UpdateTextType(TextType.YPos);
    public void SetTextTypeNone() => UpdateTextType(TextType.None);
}