using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserReflect2D : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private Transform origin;           // drag Laser/Origin
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private int maxBounces = 10;
    [SerializeField] private float surfaceEpsilon = 0.01f;

    [Header("Layers")]
    [SerializeField] private LayerMask hitMask;          // Mirror | Blocker
    [SerializeField] private LayerMask mirrorMask;       // Mirror only

    private LineRenderer lr;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
    }

    private void Update()
    {
        if (origin == null) return;

        Vector2 pos = origin.position;
        Vector2 dir = origin.right.normalized;

        float remaining = maxDistance;

        List<Vector3> points = new();
        points.Add(pos);

        for (int i = 0; i <= maxBounces; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(pos, dir, remaining, hitMask);

            if (!hit.collider)
            {
                points.Add(pos + dir * remaining);
                break;
            }

            points.Add(hit.point);

            float traveled = Vector2.Distance(pos, hit.point);
            remaining -= traveled;
            if (remaining <= 0.0001f) break;

            bool isMirror = ((1 << hit.collider.gameObject.layer) & mirrorMask) != 0;
            if (!isMirror)
            {
                // kena blocker/objek non-mirror => stop
                break;
            }

            // reflect
            dir = Vector2.Reflect(dir, hit.normal).normalized;
            pos = hit.point + hit.normal * surfaceEpsilon;
        }

        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
    }
}