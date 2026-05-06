using UnityEngine;

public class PencilRefraction2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform pencilInside;   // drag Pencil_Inside
    [SerializeField] private Transform pencilOutside;  // drag Pencil_Outside

    [Header("Refraction Settings")]
    [Tooltip("Seberapa besar geser maksimum bagian dalam air (satuan world/local).")]
    [SerializeField] private float maxInsideOffsetX = 0.15f;

    [Tooltip("Seberapa cepat transisi offset.")]
    [SerializeField] private float smooth = 12f;

    private float targetT; // 0..1 seberapa masuk air
    private float currentT;

    private Vector3 insideBaseLocalPos;

    private void Awake()
    {
        if (pencilInside != null)
            insideBaseLocalPos = pencilInside.localPosition;
    }

    private void Update()
    {
        if (pencilInside == null) return;

        currentT = Mathf.Lerp(currentT, targetT, 1f - Mathf.Exp(-smooth * Time.deltaTime));

        float offsetX = Mathf.Lerp(0f, maxInsideOffsetX, currentT);
        pencilInside.localPosition = insideBaseLocalPos + new Vector3(offsetX, 0f, 0f);
    }

    // Trigger dari collider air
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
            targetT = 1f;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
            targetT = 0f;
    }
}