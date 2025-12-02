using UnityEngine;

public class PlatformCollider : MonoBehaviour
{
    private Vector2 lastPosition;
    private Vector2 velocity;
    private PlayerController currentPlayer;

    private void Awake()
    {
        lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector2 currentPosition = transform.position;
        float dt = Time.deltaTime;
        if (dt > 0f)
            velocity = (currentPosition - lastPosition) / dt;
        else
            velocity = Vector2.zero;

        lastPosition = currentPosition;

        if (currentPlayer != null)
            currentPlayer.SetGroundVelocity(velocity);
    }

    private void OnDisable()
    {
        if (currentPlayer != null)
        {
            currentPlayer.SetGroundVelocity(Vector2.zero);
            currentPlayer = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        currentPlayer = other.GetComponent<PlayerController>();
        if (currentPlayer != null)
            currentPlayer.SetGroundVelocity(velocity);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (currentPlayer == null) return;
        if (!other.CompareTag("Player")) return;
        if (other.GetComponent<PlayerController>() != currentPlayer) return;

        currentPlayer.SetGroundVelocity(Vector2.zero);
        currentPlayer = null;
    }
}