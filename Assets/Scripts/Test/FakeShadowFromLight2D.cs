using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class FakeShadowFromLight2D : MonoBehaviour
{
    [Header("Light Source (drag Senter/LightOrigin)")]
    [SerializeField] private Transform lightOrigin;

    [Header("Light Cone Check (for showing/hiding shadow)")]
    [SerializeField] private bool onlyShowWhenLit = true;
    [SerializeField] private float range = 10f;
    [SerializeField, Range(1f, 179f)] private float coneAngle = 60f;

    [Header("Shadow Material (recommended)")]
    [Tooltip("Buat material URP 2D Sprite-Unlit-Default, lalu taruh di sini. Jika kosong, Unity akan pakai default material.")]
    [SerializeField] private Material shadowUnlitMaterial;

    [Header("Shadow Visual")]
    [SerializeField] private Color shadowColor = new Color(0f, 0f, 0f, 0.45f);
    [SerializeField] private int orderOffset = -5;
    [SerializeField] private float zOffset = 0.1f;

    [Header("Manual Offset (extra tweak)")]
    [SerializeField] private Vector2 extraOffset = Vector2.zero;

    [Header("Shadow Placement")]
    [SerializeField] private bool dropToFloor = false;
    [SerializeField] private float floorYOffset = -0.8f;

    [Tooltip("Offset minimum saat cahaya jauh")]
    [SerializeField] private float minShadowOffset = 0.2f;

    [Tooltip("Offset maksimum saat cahaya dekat (bayangan makin jauh)")]
    [SerializeField] private float maxShadowOffset = 1.2f;

    [Header("Shadow Scale")]
    [SerializeField] private float baseScale = 1.0f;
    [SerializeField] private float minScale = 0.7f;
    [SerializeField] private float maxScale = 2.0f;
    [SerializeField] private float scaleRange = 10f;

    [Header("Stretch (optional, biar shadow jadi oval)")]
    [SerializeField] private bool makeOval = true;
    [SerializeField] private float ovalX = 1.2f;
    [SerializeField] private float ovalY = 0.8f;

    private SpriteRenderer objSr;
    private Transform shadowTf;
    private SpriteRenderer shadowSr;

    private void Awake()
    {
        objSr = GetComponent<SpriteRenderer>();
        CreateShadowIfNeeded();
    }

    private void OnEnable()
    {
        if (shadowTf != null)
            shadowTf.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (shadowTf != null)
            shadowTf.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (lightOrigin == null || shadowTf == null) return;

        if (onlyShowWhenLit)
        {
            bool lit = IsInCone(lightOrigin, transform.position, range, coneAngle);
            shadowTf.gameObject.SetActive(lit);
            if (!lit) return;
        }
        else
        {
            if (!shadowTf.gameObject.activeSelf)
                shadowTf.gameObject.SetActive(true);
        }

        Vector2 objPos = transform.position;
        Vector2 lightPos = lightOrigin.position;

        float dist = Vector2.Distance(lightPos, objPos);

        // t: dekat cahaya => t besar
        float t = Mathf.InverseLerp(scaleRange, 0.5f, dist);

        // offset dinamis: dekat => offset makin besar (bayangan makin jauh)
        float dynamicOffset = Mathf.Lerp(minShadowOffset, maxShadowOffset, t);

        Vector2 shadowPos;
        if (dropToFloor)
        {
            shadowPos = new Vector2(objPos.x, objPos.y + floorYOffset);
        }
        else
        {
            Vector2 away = (objPos - lightPos);
            if (away.sqrMagnitude < 0.0001f) away = Vector2.right;
            away.Normalize();
            shadowPos = objPos + away * dynamicOffset;
        }

        shadowPos += extraOffset;

        shadowTf.position = new Vector3(shadowPos.x, shadowPos.y, transform.position.z + zOffset);

        // scale dinamis (pakai t yang sama)
        float s = Mathf.Lerp(minScale, maxScale, t) * baseScale;
        shadowTf.localScale = makeOval
            ? new Vector3(s * ovalX, s * ovalY, 1f)
            : Vector3.one * s;

        if (shadowSr != null && shadowSr.sprite != objSr.sprite)
            shadowSr.sprite = objSr.sprite;
    }

    private void CreateShadowIfNeeded()
    {
        if (shadowTf != null) return;

        GameObject shadowGo = new GameObject($"{name}_Shadow");
        shadowTf = shadowGo.transform;

        // IMPORTANT: shadow ikut lifecycle object ini (biar tidak tertinggal saat parent dinonaktifkan)
        shadowTf.SetParent(transform, worldPositionStays: true);

        shadowSr = shadowGo.AddComponent<SpriteRenderer>();
        shadowSr.sprite = objSr.sprite;
        shadowSr.color = shadowColor;

        // Material: pakai yang kamu assign, kalau kosong biarkan default
        shadowSr.sharedMaterial = shadowUnlitMaterial != null ? shadowUnlitMaterial : null;

        shadowSr.sortingLayerID = objSr.sortingLayerID;
        shadowSr.sortingOrder = objSr.sortingOrder + orderOffset;
    }

    private static bool IsInCone(Transform origin, Vector3 targetPos, float maxRange, float angleDeg)
    {
        Vector2 o = origin.position;
        Vector2 to = (Vector2)targetPos - o;

        float dist = to.magnitude;
        if (dist > maxRange || dist < 0.0001f) return false;

        Vector2 dir = origin.right;
        float ang = Vector2.Angle(dir, to);
        return ang <= angleDeg * 0.5f;
    }

    private void OnDestroy()
    {
        if (shadowTf != null)
            Destroy(shadowTf.gameObject);
    }
}