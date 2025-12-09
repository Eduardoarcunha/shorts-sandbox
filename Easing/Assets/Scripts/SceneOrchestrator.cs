using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SceneOrchestrator : MonoBehaviour
{
    [SerializeField] private InputActionReference submitAction;
    [SerializeField] private InputActionReference quitAction;
    [SerializeField] private InputActionReference restartAction;

    [SerializeField] private List<UnityEvent> submitEvents = new List<UnityEvent>();
    public int currentEventIndex = 0;

    void OnEnable()
    {
        if (submitAction != null)
            submitAction.action.Enable();

        if (submitAction != null)
            submitAction.action.performed += OnSubmit;

        if (quitAction != null)
            quitAction.action.Enable();

        if (quitAction != null)
            quitAction.action.performed += ctx => QuitApplication();

        if (restartAction != null)
            restartAction.action.Enable();

        if (restartAction != null)
            restartAction.action.performed += ctx => ReloadScene();
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

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

}