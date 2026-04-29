using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Beam2D : MonoBehaviour
{
    [SerializeField] private Transform beamOrigin;
    [SerializeField] private float range = 30f;
    [SerializeField] private LayerMask hitMask;

    private LineRenderer lr;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.useWorldSpace = true;
    }

    private void Update()
    {
        if (beamOrigin == null) return;

        Vector2 origin = beamOrigin.position;
        Vector2 dir = beamOrigin.right; // pastikan arah senter benar

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, range, hitMask);

        Vector2 endPoint = hit.collider != null ? hit.point : origin + dir * range;

        lr.SetPosition(0, origin);
        lr.SetPosition(1, endPoint);
    }
}