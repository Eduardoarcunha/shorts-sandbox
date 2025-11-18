using System.Collections;
using TMPro;
using UnityEngine;

public class RandomCharacterInput : MonoBehaviour
{
    [SerializeField] private bool useSeed;
    [SerializeField] private int seed = 42;

    [SerializeField] private float actionInverval = 0.75f;
    [SerializeField] private float startDelay = 0f;

    [SerializeField] private PlayerInputReader inputReader;

    [SerializeField] private int jumpWeight = 1;
    [SerializeField] private int attackWeight = 1;
    [SerializeField] private int nothingWeight = 3;

    [SerializeField] private TextMeshProUGUI seedText;

    private float timer;
    private System.Random rng;
    private bool initialized = false;

    private enum ActionType { Jump, Attack, Nothing }

    void Awake()
    {
        seed = useSeed ? seed : UnityEngine.Random.Range(0, 9999);
        rng = new System.Random(seed);
        timer = actionInverval;
    }

    void Start()
    {
        if (inputReader == null)
        {
            inputReader = GetComponent<PlayerInputReader>();
        }

        if (seedText != null)
        {
            seedText.text = seed.ToString();
        }

        StartCoroutine(StartDelayCoroutine());
    }

    IEnumerator StartDelayCoroutine()
    {
        yield return new WaitForSeconds(startDelay);
        initialized = true;
    }

    void Update()
    {
        if (inputReader == null || actionInverval <= 0f || !initialized)
            return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer += actionInverval;

        switch (PickAction())
        {
            case ActionType.Jump: inputReader.DoJump(); break;
            case ActionType.Attack: inputReader.DoAttack(); break;
            case ActionType.Nothing: default: break;
        }
    }

    private ActionType PickAction()
    {
        int total = jumpWeight + attackWeight + nothingWeight;
        if (total <= 0) return ActionType.Nothing;

        int roll = rng.Next(total);
        Debug.Log($"Random roll: {roll} / {total}");
        if (roll < jumpWeight) return ActionType.Jump;
        roll -= jumpWeight;
        if (roll < attackWeight) return ActionType.Attack;
        return ActionType.Nothing;
    }
}