
using UnityEngine;

public static class Utils
{
    public static bool CollidersOverlap(BoxCollider boxCollider1, BoxCollider boxCollider2)
    {
        if (boxCollider1 == null || boxCollider2 == null)
        {
            return false;
        }

        // Get the bounds of each BoxCollider
        Bounds bounds1 = boxCollider1.bounds;
        Bounds bounds2 = boxCollider2.bounds;

        // Check for overlap between the bounds
        return Physics.ComputePenetration(boxCollider1, boxCollider1.transform.position, boxCollider1.transform.rotation, boxCollider2, boxCollider2.transform.position, boxCollider2.transform.rotation, out _, out _);
    }
}
