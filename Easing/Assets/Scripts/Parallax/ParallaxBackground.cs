using UnityEngine;

[System.Serializable]
public class BackgroundLayer
{
    [Header("Layer root (ONE sprite object)")]
    public Transform layerRoot;          // The GameObject that has the SpriteRenderer

    [Range(0f, 2f)]
    public float parallaxFactor = 1f;    // 0.2 = far, 1 = mid, 1.5+ = very close

    [HideInInspector] public float spriteWidth;
    [HideInInspector] public Transform[] parts;  // 2 tiles created at runtime
}

public class ParallaxBackground : MonoBehaviour
{
    [Header("Player reference")]
    [SerializeField] private PlayerController player;  // has CurrentHorizontalSpeed

    [Header("Camera (for recycling)")]
    [SerializeField] private Transform cameraTransform;  // drag main camera or leave empty

    [Header("Scroll config")]
    [SerializeField] private float baseScrollSpeed = 1f; // global multiplier

    [Header("Layers")]
    [SerializeField] private BackgroundLayer[] layers;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        foreach (var layer in layers)
        {
            if (layer.layerRoot == null) continue;

            var sr = layer.layerRoot.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                Debug.LogWarning($"ParallaxBackground: {layer.layerRoot.name} has no SpriteRenderer.");
                continue;
            }

            layer.spriteWidth = sr.bounds.size.x;

            // Create 2 tiles: original + 1 clone to the right
            layer.parts = new Transform[2];
            layer.parts[0] = layer.layerRoot;

            Vector3 clonePos = layer.layerRoot.position + Vector3.right * layer.spriteWidth;
            layer.parts[1] = Instantiate(layer.layerRoot, clonePos, layer.layerRoot.rotation, layer.layerRoot.parent);
            layer.parts[1].name = layer.layerRoot.name + "_Tile";
        }
    }

    private void Update()
    {
        float playerSpeed = 0f;
        if (player != null)
            playerSpeed = player.CurrentHorizontalSpeed; // keep the sign (left/right)

        foreach (var layer in layers)
        {
            if (layer.parts == null || layer.parts.Length == 0) continue;

            // Move opposite to player direction
            float scrollSpeed = -playerSpeed * baseScrollSpeed * layer.parallaxFactor;
            Vector3 delta = Vector3.right * scrollSpeed * Time.deltaTime;

            // Move all tiles of this layer
            for (int i = 0; i < layer.parts.Length; i++)
            {
                layer.parts[i].position += delta;
            }

            // Recycle tiles that go off-screen to the left
            float leftLimit = -layer.spriteWidth;
            if (cameraTransform != null)
                leftLimit = cameraTransform.position.x - layer.spriteWidth;

            for (int i = 0; i < layer.parts.Length; i++)
            {
                var part = layer.parts[i];
                if (part.position.x <= leftLimit)
                {
                    // Find the tile that is currently the rightmost
                    Transform rightmost = layer.parts[0];
                    for (int j = 1; j < layer.parts.Length; j++)
                    {
                        if (layer.parts[j].position.x > rightmost.position.x)
                            rightmost = layer.parts[j];
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