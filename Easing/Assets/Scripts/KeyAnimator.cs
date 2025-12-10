using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class KeyAnimator : MonoBehaviour
{
    [SerializeField] private GameObject parentObject;

    [SerializeField] private InputActionReference keyAction;

    [SerializeField] private Image keyImage;

    [SerializeField] private Sprite keyUpSprite;
    [SerializeField] private Sprite keyDownSprite;
    [SerializeField]
    private

    void Awake()
    {
        if (keyImage == null)
        {
            keyImage = GetComponent<Image>();
        }
    }

    void OnEnable()
    {
        if (keyAction != null)
            keyAction.action.Enable();

        if (keyAction != null)
        {
            keyAction.action.performed += OnKeyPressed;
            keyAction.action.canceled += OnKeyReleased;
        }
    }

    void OnDisable()
    {
        if (keyAction != null)
        {
            keyAction.action.performed -= OnKeyPressed;
            keyAction.action.canceled -= OnKeyReleased;
        }

        if (keyAction != null)
            keyAction.action.Disable();
    }

    void OnKeyPressed(InputAction.CallbackContext ctx)
    {
        if (keyImage != null && keyDownSprite != null)
        {
            keyImage.sprite = keyDownSprite;
        }
    }

    void OnKeyReleased(InputAction.CallbackContext ctx)
    {
        if (keyImage != null && keyUpSprite != null)
        {
            keyImage.sprite = keyUpSprite;
        }
    }

    public void ShowObject() => parentObject.SetActive(true);
    public void Hide() => parentObject.SetActive(false);
}
