using UnityEngine;

[System.Serializable]
public class BackgroundLayer
{
    public Transform[] parts;      // 2 sprites for this layer
    [Range(0f, 2f)]
    public float parallaxFactor = 1f; // 1 = normal, <1 = slower (far), >1 = faster (near)

    [HideInInspector] public float spriteWidth;
}

public class ParallaxBackground : MonoBehaviour
{
    [Header("Player reference")]
    [SerializeField] private PlayerController2D player; // drag your player here

    [Header("Scroll config")]
    [SerializeField] private float baseScrollSpeed = 3f; // how fast at full run

    [Header("Layers")]
    [SerializeField] private BackgroundLayer[] layers;

    private void Start()
    {
        foreach (var layer in layers)
        {
            if (layer.parts == null || layer.parts.Length == 0) continue;

            var sr = layer.parts[0].GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            layer.spriteWidth = sr.bounds.size.x;
        }
    }

    private void Update()
    {
        // How fast the player is moving horizontally
        float playerSpeed = 0f;
        if (player != null)
            playerSpeed = player.CurrentHorizontalSpeed; // keep the sign!

        foreach (var layer in layers)
        {
            if (layer.parts == null || layer.parts.Length == 0) continue;

            // Final scroll speed for this layer
            float scroll = -playerSpeed * baseScrollSpeed * layer.parallaxFactor;

            // Always scroll to the left (runner-style)
            Vector3 move = new Vector3(scroll * Time.deltaTime, 0f, 0f);
            // Move all pieces
            foreach (var part in layer.parts)
            {
                part.position += move;
            }

            // Recycle pieces that go off-screen to the left
            foreach (var part in layer.parts)
            {
                if (part.position.x <= -layer.spriteWidth)
                {
                    Transform rightmost = layer.parts[0];
                    foreach (var other in layer.parts)
                    {
                        if (other.position.x > rightmost.position.x)
                            rightmost = other;
                    }

                    part.position = new Vector3(
                        rightmost.position.x + layer.spriteWidth,
                        part.position.y,
                        part.position.z
                    );
                }
            }
        }
    }
}