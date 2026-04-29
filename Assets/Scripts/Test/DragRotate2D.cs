using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DragRotate2D : MonoBehaviour
{
    [Header("Drag")]
    [SerializeField] private float dragZ = 0f; // plane z untuk screen->world

    [Header("Rotate")]
    [SerializeField] private float scrollDegreesPerNotch = 15f;
    [SerializeField] private bool invertScroll = false;

    // Only one selected object at a time
    private static DragRotate2D active;

    private Camera cam;
    private bool dragging;
    private Vector3 dragOffset;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        // rotate hanya untuk object yang sedang aktif/terselect
        if (active == this)
        {
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.0001f)
            {
                float dir = invertScroll ? -1f : 1f;
                float degrees = scroll * scrollDegreesPerNotch * dir;
                transform.rotation = Quaternion.AngleAxis(degrees, Vector3.forward) * transform.rotation;
            }
        }

        if (dragging)
        {
            Vector3 mouseWorld = ScreenToWorldOnPlane(Input.mousePosition);
            Vector3 newPos = mouseWorld + dragOffset;
            newPos.z = transform.position.z;
            transform.position = newPos;
        }
    }

    private void OnMouseDown()
    {
        active = this;
        dragging = true;

        Vector3 mouseWorld = ScreenToWorldOnPlane(Input.mousePosition);
        dragOffset = transform.position - mouseWorld;
    }

    private void OnMouseUp()
    {
        dragging = false;
    }

    private Vector3 ScreenToWorldOnPlane(Vector3 mouseScreen)
    {
        if (cam == null) cam = Camera.main;

        Vector3 p = mouseScreen;
        p.z = Mathf.Abs(cam.transform.position.z - dragZ);
        Vector3 w = cam.ScreenToWorldPoint(p);
        w.z = dragZ;
        return w;
    }
}