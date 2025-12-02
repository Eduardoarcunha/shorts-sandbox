using UnityEngine;

public class PlatformEnabler : MonoBehaviour
{
    [SerializeField] PlatformCollider platformCollider;
    [SerializeField] SpriteRenderer spriteRenderer;


    public void EnableCollider()
    {
        platformCollider.enabled = true;
    }

    public void ShowCollider()
    {
        spriteRenderer.enabled = true;
    }
}