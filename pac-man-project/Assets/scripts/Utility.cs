using UnityEngine;
using System.Collections;

public class Utility
{

    public static bool WallHit(Transform transform, Vector2 dir)
    {
        Vector2 pos = transform.position;
        RaycastHit2D[] hit = Physics2D.LinecastAll(pos + dir + dir + (dir * 0.1f), pos);
        bool wallHit = false;
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.gameObject.name.CompareTo("Wall") == 0)
            {
                wallHit = true;
                break;
            }
        }
        return wallHit;
    }
}
