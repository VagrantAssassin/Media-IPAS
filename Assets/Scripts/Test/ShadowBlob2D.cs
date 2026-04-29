using UnityEngine;

public class ShadowBlob2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform beamOrigin;      // drag AsalCahaya ke sini
    [SerializeField] private Transform shadowBlob;      // drag ShadowBlob ke sini
    [SerializeField] private float range = 30f;

    [Header("Layers")]
    [SerializeField] private LayerMask opaqueMask;      // pilih layer Opaque
    [SerializeField] private LayerMask screenMask;      // pilih layer Screen
    [SerializeField] private LayerMask hitMask;         // gabungan Opaque + Screen (atau isi sama seperti Beam2D)

    [Header("Shadow Size")]
    [SerializeField] private float baseSize = 0.5f;     // ukuran dasar blob
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 4f;

    private void Awake()
    {
        if (shadowBlob != null)
            shadowBlob.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (beamOrigin == null || shadowBlob == null) return;

        Vector2 origin = beamOrigin.position;
        Vector2 dir = beamOrigin.right; // sama seperti Beam2D kamu (karena sinar ke kanan)

        // 1) Cari hit pertama (Opaque atau Screen)
        RaycastHit2D firstHit = Physics2D.Raycast(origin, dir, range, hitMask);

        // 2) Cari titik screen (untuk menaruh blob di screen)
        RaycastHit2D screenHit = Physics2D.Raycast(origin, dir, range, screenMask);

        // Kalau screen tidak kena sama sekali, kita gak bisa taruh bayangan
        if (screenHit.collider == null)
        {
            shadowBlob.gameObject.SetActive(false);
            return;
        }

        // 3) Jika hit pertama adalah Opaque (dan bukan screen), berarti screen terhalang => bayangan muncul
        bool blockedByOpaque = firstHit.collider != null &&
                               ((opaqueMask.value & (1 << firstHit.collider.gameObject.layer)) != 0);

        if (!blockedByOpaque)
        {
            shadowBlob.gameObject.SetActive(false);
            return;
        }

        // 4) Posisi bayangan di screen
        shadowBlob.gameObject.SetActive(true);
        shadowBlob.position = screenHit.point;

        // 5) Ukuran bayangan (membesar jika benda dekat senter)
        float dObj = Vector2.Distance(origin, firstHit.point);
        float dScreen = Vector2.Distance(origin, screenHit.point);

        if (dObj <= 0.0001f)
        {
            shadowBlob.localScale = Vector3.one * baseSize;
            return;
        }

        float scaleFactor = dScreen / dObj;                 // makin kecil dObj => makin besar
        scaleFactor = Mathf.Clamp(scaleFactor, minScale, maxScale);

        shadowBlob.localScale = Vector3.one * (baseSize * scaleFactor);
    }
}