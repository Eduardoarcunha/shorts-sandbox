using System.Collections;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform playerCheck;

    [Header("Settings")]
    [SerializeField] private float dropThroughDuration = 0.5f;
    [SerializeField] private LayerMask platformAboveLayer;
    [SerializeField] private LayerMask platformBelowLayer;

    private bool isDroppingThrough = false;
    private int aboveLayerIndex;
    private int belowLayerIndex;

    private void Start()
    {
        // Convert LayerMask to layer index
        aboveLayerIndex = LayerMaskToLayer(platformAboveLayer);
        belowLayerIndex = LayerMaskToLayer(platformBelowLayer);
    }

    private int LayerMaskToLayer(LayerMask layerMask)
    {
        int layerNumber = 0;
        int layer = layerMask.value;
        while (layer > 1)
        {
            layer >>= 1;
            layerNumber++;
        }
        return layerNumber;
    }

    private void Update()
    {
        if (player == null) return;

        // Check if player wants to drop through
        if (player.CurrentVerticalInput < -0.5f && !isDroppingThrough)
        {
            float playerY = player.transform.position.y;
            float platformY = transform.position.y;

            // If player is above platform and pressing down, let them drop through
            if (playerY >= platformY)
            {
                StartCoroutine(DropThroughCoroutine());
                return;
            }
        }

        // Simple Y position check - change platform layer based on player position
        if (!isDroppingThrough)
        {
            float playerY = player.transform.position.y;
            bool isAbove = playerY >= playerCheck.position.y;
            
            // Set platform to "above" layer if player is above (player collides)
            // Set platform to "below" layer if player is below (player passes through, enemies still collide)
            gameObject.layer = isAbove ? aboveLayerIndex : belowLayerIndex;
        }
    }

    private IEnumerator DropThroughCoroutine()
    {
        isDroppingThrough = true;
        gameObject.layer = belowLayerIndex;

        yield return new WaitForSeconds(dropThroughDuration);

        isDroppingThrough = false;
    }
}

