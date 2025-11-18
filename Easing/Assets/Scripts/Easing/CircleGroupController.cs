using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;

public class CircleGroupController : MonoBehaviour
{
    [Header("Prefabs & Hierarchy")]
    [SerializeField] private Transform slotContainer;
    [SerializeField] private GameObject circlePrefab;

    [Header("Layout")]
    [SerializeField] private float spacing = 2.5f;
    [SerializeField] private float recenterDuration = 0.4f;
    [SerializeField] private Ease recenterEase = Ease.OutQuad;

    [Header("Move Config (shared)")]
    [SerializeField] private float initialDelay = 0.5f;
    [SerializeField] private Vector3 localTargetOffset = new Vector3(1.5f, 0f, 0f);
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private float pauseBetweenLoops = 0.5f;

    [SerializeField] private List<Color> circleColors = new();
    [SerializeField] private List<Ease> easeTypes = new();


    readonly List<Transform> slots = new();
    readonly List<MoveTween> movers = new();

    bool pendingAdd = false;
    bool hasAddedThisCycle = false;
    bool initCompleted = false;

    void Awake()
    {
        if (slotContainer == null) slotContainer = this.transform;
    }

    void Start()
    {
        StartCoroutine(StartMovingBalls());
    }

    private IEnumerator StartMovingBalls()
    {
        yield return new WaitForSeconds(initialDelay);
        SpawnCircle(); // 1
        // SpawnCircle(); // 2
        RecenterSlots();
        initCompleted = true;
    }

    void Update()
    {
        if (!initCompleted) return;
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && slots.Count < 3 && !pendingAdd)
        {
            pendingAdd = true;
            // ask all movers to stop at the end of their current loop
            for (int i = 0; i < movers.Count; i++) movers[i].RequestStop();
        }
    }

    void SpawnCircle()
    {
        // Create a Slot
        var slotGO = new GameObject($"Slot_{slots.Count}");
        var slot = slotGO.transform;
        slot.SetParent(slotContainer, worldPositionStays: false);
        slot.localPosition = Vector3.zero;

        // Instantiate circle as child and configure its tween
        var circle = Instantiate(circlePrefab, slot);
        circle.transform.localPosition = Vector3.zero;

        var mover = circle.GetComponent<MoveTween>();
        mover.Configure(easeTypes[slots.Count], localTargetOffset, moveDuration, pauseBetweenLoops, circleColors[slots.Count]);
        slots.Add(slot);

        movers.Add(mover);
        mover.LoopStopReached += OnMoverLoopStop;
    }

    void RecenterSlots()
    {
        int n = slots.Count;
        if (n == 0) return;

        float totalHeight = (n - 1) * spacing;
        float startY = -totalHeight / 2f;

        for (int i = 0; i < n; i++)
        {
            // i = 0 is bottom; map newest (index n-1) to bottom by reversing the slot index
            int slotIndex = n - 1 - i;
            Vector3 target = new Vector3(-localTargetOffset.x / 2f, startY + i * spacing, 0f);
            slots[slotIndex].DOLocalMove(target, recenterDuration).SetEase(recenterEase);
        }
    }

    void OnMoverLoopStop(MoveTween who)
    {
        if (!pendingAdd || hasAddedThisCycle) return;
        hasAddedThisCycle = true;

        if (slots.Count < 3)
        {
            SpawnCircle();
            RecenterSlots();
        }

        // clear flags and resume everyone (including the newly spawned mover)
        for (int i = 0; i < movers.Count; i++) movers[i].ClearStopAndResume();

        pendingAdd = false;
        hasAddedThisCycle = false;
    }
}