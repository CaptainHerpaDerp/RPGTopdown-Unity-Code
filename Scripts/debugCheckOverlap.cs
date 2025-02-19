using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugCheckOverlap : MonoBehaviour
{
    [SerializeField] private BoxCollider object1, object2;

    // Method to check if two BoxColliders are overlapping
    public bool CheckBoxCollidersOverlap(BoxCollider boxCollider1, BoxCollider boxCollider2)
    {
        if (boxCollider1 == null || boxCollider2 == null)
        {
            return false;
        }

        // Get the bounds of each BoxCollider
        Bounds bounds1 = boxCollider1.bounds;
        Bounds bounds2 = boxCollider2.bounds;

        // Check for overlap between the bounds
        return bounds1.Intersects(bounds2);
    }
}