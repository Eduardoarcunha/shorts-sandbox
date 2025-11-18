using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SceneOrchestrator : MonoBehaviour
{
    [SerializeField] private InputActionReference submitAction;

    [SerializeField] private List<UnityEvent> submitEvents = new List<UnityEvent>();
    public int currentEventIndex = 0;

    void OnEnable()
    {
        if (submitAction != null)
            submitAction.action.Enable();

        if (submitAction != null)
            submitAction.action.performed += OnSubmit;
    }

    void OnDisable()
    {
        if (submitAction != null)
            submitAction.action.Disable();

        if (submitAction != null)
            submitAction.action.performed -= OnSubmit;
    }

    void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (currentEventIndex < submitEvents.Count)
            {
                submitEvents[currentEventIndex]?.Invoke();
                currentEventIndex++;
            }
        }
    }

}