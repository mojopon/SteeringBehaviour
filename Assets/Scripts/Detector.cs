using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    [SerializeField]
    private LayerMask CollisionLayer;
    [SerializeField]
    private LayerMask NeighborLayer;

    private Collider2D myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
    }

    public List<Vector2> DetectNeighborsWithOverlapCircle(Vector2 sourcePosition, float radius)
    {
        List<Vector2> neighbors = new List<Vector2>();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(sourcePosition, radius, NeighborLayer);
        foreach (var collider in colliders)
        {
            if (collider != myCollider)
            {
                neighbors.Add(collider.transform.position);
            }
        }

        return neighbors;
    }

    public List<int> DetectAllCollisionsWithCircleCast(Vector2 self, float circleCastSize, float circleCastRange)
    {
        List<int> collisionDetectedDirections = new List<int>();
        for (int i = 0; i < Directions.AllDirections.Count; i++)
        {
            var detected = DetectCollisionWithCircleCast(self, i, circleCastSize, circleCastRange);
            if (detected) 
            {
                collisionDetectedDirections.Add(i);
            }
        }

        return collisionDetectedDirections;
    }

    public bool DetectCollisionWithCircleCast(Vector2 self, int directionNumber, float circleCastSize, float circleCastRange)
    {
        var direction = Directions.AllDirections[directionNumber];
        RaycastHit2D hit = Physics2D.CircleCast(self, circleCastSize, direction, circleCastRange, CollisionLayer);
        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
