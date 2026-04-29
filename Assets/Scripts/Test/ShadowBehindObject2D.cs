using UnityEngine;

public class ShadowBehindObject2D : MonoBehaviour
{
    [SerializeField] private Transform beamOrigin;     // AsalCahaya
    [SerializeField] private float range = 30f;
    [SerializeField] private LayerMask opaqueMask;     // layer Opaque
    [SerializeField] private LayerMask hitMask;        // biasanya sama seperti Beam2D (minimal Opaque)

    [Header("Shadow")]
    [SerializeField] private Transform shadowBlob;     // ShadowBlob object
    [SerializeField] private float shadowOffset = 0.5f; // jarak bayangan dari titik kena sinar
    [SerializeField] private float baseSize = 0.5f;     // ukuran dasar
    [SerializeField] private float minScale = 0.6f;
    [SerializeField] private float maxScale = 2.5f;

    private void Awake()
    {
        if (shadowBlob != null)
            shadowBlob.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (beamOrigin == null || shadowBlob == null) return;

        Vector2 origin = beamOrigin.position;
        Vector2 dir = beamOrigin.right; // sama seperti Beam2D kamu (kalau sinar ke kanan)

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, range, hitMask);

        bool hitOpaque = hit.collider != null &&
                         ((opaqueMask.value & (1 << hit.collider.gameObject.layer)) != 0);

        if (!hitOpaque)
        {
            shadowBlob.gameObject.SetActive(false);
            return;
        }

        shadowBlob.gameObject.SetActive(true);

        // Posisi bayangan: di belakang objek searah sinar (setelah titik hit)
        Vector2 shadowPos = hit.point + dir.normalized * shadowOffset;
        shadowBlob.position = new Vector3(shadowPos.x, shadowPos.y, shadowBlob.position.z);

        // Skala bayangan: benda makin dekat ke senter => bayangan membesar (stabil, di-clamp)
        float dObj = Vector2.Distance(origin, hit.point);
        float t = Mathf.InverseLerp(range, 0.5f, dObj); // dekat origin => t mendekati 1
        float scaleFactor = Mathf.Lerp(minScale, maxScale, t);

        shadowBlob.localScale = Vector3.one * (baseSize * scaleFactor);
    }
}