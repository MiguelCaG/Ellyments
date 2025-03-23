using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundsManager : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner confiner;

    public enum Axis { X, Y }

    [SerializeField] private Axis axis = Axis.X;

    [SerializeField] private PolygonCollider2D leftDownConfiner;
    [SerializeField] private PolygonCollider2D rightUpConfiner;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - gameObject.GetComponent<Collider2D>().bounds.center).normalized;

            if ((axis == Axis.X && exitDirection.x > 0f) || (axis == Axis.Y && exitDirection.y > 0f))
            {
                if (confiner != null && rightUpConfiner != null)
                {
                    confiner.m_BoundingShape2D = rightUpConfiner;
                    confiner.InvalidatePathCache();
                }
            }
            else if ((axis == Axis.X && exitDirection.x < 0f) || (axis == Axis.Y && exitDirection.y < 0f))
            {
                if (confiner != null && leftDownConfiner != null)
                {
                    confiner.m_BoundingShape2D = leftDownConfiner;
                    confiner.InvalidatePathCache();
                }
            }
        }
    }
}
