using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Letakkan komponen ini di setiap Button chapter yang ada di sidebar materi.
/// Isi "Controller" dengan MateriContentController, dan "Chapter Index" sesuai urutan (0 = Chapter 1, dst).
/// Hubungkan OnClick() ke event onClick Button di Inspector.
/// </summary>
[RequireComponent(typeof(Button))]
public class MateriSidebarButton : MonoBehaviour
{
    [SerializeField] private MateriContentController controller;
    [SerializeField] private int chapterIndex;

    [Header("Visual State")]
    [SerializeField] private Color activeColor = new Color(0.65f, 0.65f, 0.65f, 1f);  // lebih gelap saat aktif
    [SerializeField] private Color normalColor = Color.white;

    private Image buttonImage;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }

    private void Start()
    {
        if (controller == null)
        {
            Debug.LogWarning($"[MateriSidebarButton] Controller belum di-assign pada '{gameObject.name}'. Tombol tidak akan berfungsi.", this);
            return;
        }

        controller.RegisterSidebarButton(this);
    }

    /// <summary>
    /// Indeks chapter yang diwakili tombol ini (0-based).
    /// </summary>
    public int ChapterIndex => chapterIndex;

    /// <summary>
    /// Hubungkan method ini ke onClick Button di Inspector.
    /// Akan memanggil SelectSubBab pada controller dengan index yang sudah dikonfigurasi.
    /// </summary>
    public void OnClick()
    {
        controller?.SelectSubBab(chapterIndex);
    }

    /// <summary>
    /// Dipanggil oleh MateriContentController untuk memperbarui tampilan aktif/tidak-aktif.
    /// </summary>
    public void SetActiveState(bool isActive)
    {
        if (buttonImage != null)
            buttonImage.color = isActive ? activeColor : normalColor;
    }
}
