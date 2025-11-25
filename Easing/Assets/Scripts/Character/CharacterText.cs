using TMPro;
using UnityEngine;
using DG.Tweening;

public enum TextType { None, Position, Velocity }

public class CharacterText : MonoBehaviour
{

    [SerializeField] private TextType textType = TextType.Position;
    [SerializeField] private TextMeshProUGUI topText;

    [SerializeField] private Transform characterTransform;
    [SerializeField] private Rigidbody2D characterRigidbody;

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
        if (characterTransform == null || characterRigidbody == null || topText == null)
            return;

        switch (textType)
        {
            case TextType.Position:
                Vector3 pos = characterTransform.position;
                topText.text = $"X={pos.x:F2} Y={pos.y:F2}";
                break;
            case TextType.Velocity:
                Vector2 vel = characterRigidbody.linearVelocity;
                topText.text = $"X={vel.x:F2} Y={vel.y:F2}";
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

    public void SetTextTypePosition() => UpdateTextType(TextType.Position);
    public void SetTextTypeVelocity() => UpdateTextType(TextType.Velocity);
    public void SetTextTypeNone() => UpdateTextType(TextType.None);
}