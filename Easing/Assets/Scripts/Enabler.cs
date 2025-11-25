using UnityEngine;

public class Enabler : MonoBehaviour
{
    public void EnableGameObject()
    {
        gameObject.SetActive(true);
    }

    public void DisableGameObject()
    {
        gameObject.SetActive(false);
    }
}
