using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;


public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private List<Transform> points;
    [SerializeField] private float speed = 2f;
    [SerializeField] private Ease easeType;

    private int currentPointIndex = 0;
    private Vector3 targetPosition;


    private void Start()
    {
        currentPointIndex = 0;
        targetPosition = points[currentPointIndex].position;
        MoveToNextPoint();
    }

    private void MoveToNextPoint()
    {
        currentPointIndex = (currentPointIndex + 1) % points.Count;
        targetPosition = points[currentPointIndex].position;

        float distance = Vector3.Distance(transform.position, targetPosition);
        float duration = distance / speed;

        transform.DOMove(targetPosition, duration).SetEase(easeType).OnComplete(MoveToNextPoint);
    }
}