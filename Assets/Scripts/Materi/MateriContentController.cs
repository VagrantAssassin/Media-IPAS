using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MateriContentController : MonoBehaviour
{
    [Serializable]
    public class Page
    {
        public string title;
        [TextArea(5, 20)] public string bodyText;

        public Sprite image1;
        public Sprite image2;
        public Sprite image3;
    }

    [Serializable]
    public class SubBab
    {
        public string subTitle;
        public List<Page> pages = new();
    }

    [Serializable]
    public class SimulationRule
    {
        public int subIndex;
        public int pageIndex;
        public string simulationSceneName;
        public string buttonLabel = "Coba Simulasi";
    }

    [Header("Data Materi")]
    [SerializeField] private string materiTitle = "Judul Materi";
    [SerializeField] private List<SubBab> subBabs = new();

    [Header("Simulation Button Rules")]
    [SerializeField] private List<SimulationRule> simulationRules = new();

    [Header("UI - Header")]
    [SerializeField] private TMP_Text materiTitleText;

    [Header("UI - Material Area")]
    [SerializeField] private TMP_Text contentTitleText;
    [SerializeField] private TMP_Text contentBodyText;

    [SerializeField] private Image imageSlot1;
    [SerializeField] private Image imageSlot2;
    [SerializeField] private Image imageSlot3;

    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    [Header("UI - Simulation")]
    [SerializeField] private Button simulationButton;
    [SerializeField] private TMP_Text simulationButtonText;

    [Header("UI - Sidebar")]
    [SerializeField] private RectTransform sidebarPanel;
    [SerializeField] private Button sidebarToggleButton;
    [SerializeField] private Button[] subBabButtons;
    [SerializeField] private Button exitButton;

    [Header("Scene Names")]
    [SerializeField] private string exitTargetSceneName = "MainMenu";

    [Header("Sidebar Slide Settings")]
    [SerializeField] private float sidebarSlideDuration = 0.2f;
    [SerializeField] private float hiddenOffsetX = -300f;
    // hiddenOffsetX: seberapa jauh sidebar digeser ke kiri dari posisi shown.
    // Sesuaikan dengan lebar sidebar kamu (mis. -280, -320, dst).

    private int currentSubIndex;
    private int currentPageIndex;

    private bool sidebarShown = true;
    private Vector2 sidebarShownPos;
    private Vector2 sidebarHiddenPos;
    private Coroutine sidebarSlideRoutine;

    // =========================
    // SAVE/LOAD LAST PAGE (NEW)
    // =========================
    // Kalau kamu mau per-scene/per-bab, ganti prefix ini memakai SceneManager.GetActiveScene().name
    private const string PrefSubIndex = "MediaIPAS.LastMateri.SubIndex";
    private const string PrefPageIndex = "MediaIPAS.LastMateri.PageIndex";

    private void SaveLastViewed()
    {
        PlayerPrefs.SetInt(PrefSubIndex, currentSubIndex);
        PlayerPrefs.SetInt(PrefPageIndex, currentPageIndex);
        PlayerPrefs.Save();
    }

    private void LoadLastViewedOrDefault(out int subIndex, out int pageIndex)
    {
        subIndex = PlayerPrefs.GetInt(PrefSubIndex, 0);
        pageIndex = PlayerPrefs.GetInt(PrefPageIndex, 0);
    }

    private void Awake()
    {
        if (materiTitleText != null)
            materiTitleText.text = materiTitle;

        WireNavButtons();
        WireSidebarButtons();
        SetupSidebarPositions();
        HideSimulationButton();
    }

    private void Start()
    {
        // Restore last viewed page instead of always starting at 0
        if (subBabs == null || subBabs.Count == 0)
            return;

        LoadLastViewedOrDefault(out int savedSub, out int savedPage);

        savedSub = Mathf.Clamp(savedSub, 0, subBabs.Count - 1);

        currentSubIndex = savedSub;
        currentPageIndex = savedPage; // nanti di-clamp di RefreshUI()

        RefreshUI();
    }

    private void WireNavButtons()
    {
        if (prevButton != null)
        {
            prevButton.onClick.RemoveAllListeners();
            prevButton.onClick.AddListener(PrevPage);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(NextPage);
        }

        if (simulationButton != null)
        {
            simulationButton.onClick.RemoveAllListeners();
            simulationButton.onClick.AddListener(OnClickSimulation);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(() => SceneManager.LoadScene(exitTargetSceneName));
        }

        if (sidebarToggleButton != null)
        {
            sidebarToggleButton.onClick.RemoveAllListeners();
            sidebarToggleButton.onClick.AddListener(ToggleSidebar);
        }
    }

    private void WireSidebarButtons()
    {
        if (subBabButtons == null) return;

        for (int i = 0; i < subBabButtons.Length; i++)
        {
            var btn = subBabButtons[i];
            if (btn == null) continue;

            btn.onClick.RemoveAllListeners();

            int capturedIndex = i;
            btn.onClick.AddListener(() => SelectSubBab(capturedIndex));
        }

        RefreshSidebarButtonsInteractable();
    }

    private void RefreshSidebarButtonsInteractable()
    {
        if (subBabButtons == null) return;

        for (int i = 0; i < subBabButtons.Length; i++)
        {
            if (subBabButtons[i] == null) continue;

            bool hasData = (subBabs != null && i >= 0 && i < subBabs.Count);
            subBabButtons[i].interactable = hasData;
        }
    }

    public void SelectSubBab(int subIndex)
    {
        if (subBabs == null || subBabs.Count == 0) return;
        if (subIndex < 0 || subIndex >= subBabs.Count) return;

        currentSubIndex = subIndex;
        currentPageIndex = 0;

        RefreshUI();
    }

    public void NextPage()
    {
        var pages = GetCurrentPages();
        if (pages == null || pages.Count == 0) return;

        if (currentPageIndex < pages.Count - 1)
        {
            currentPageIndex++;
            RefreshUI();
        }
    }

    public void PrevPage()
    {
        var pages = GetCurrentPages();
        if (pages == null || pages.Count == 0) return;

        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            RefreshUI();
        }
    }

    private List<Page> GetCurrentPages()
    {
        if (subBabs == null || subBabs.Count == 0) return null;
        if (currentSubIndex < 0 || currentSubIndex >= subBabs.Count) return null;

        return subBabs[currentSubIndex].pages;
    }

    private void RefreshUI()
    {
        if (subBabs == null || subBabs.Count == 0) return;
        if (currentSubIndex < 0 || currentSubIndex >= subBabs.Count) currentSubIndex = 0;

        if (materiTitleText != null)
            materiTitleText.text = materiTitle;

        var pages = GetCurrentPages();
        var sub = subBabs[currentSubIndex];

        if (pages == null || pages.Count == 0)
        {
            if (contentTitleText != null) contentTitleText.text = sub.subTitle;
            if (contentBodyText != null) contentBodyText.text = "Konten belum diisi.";

            SetImageSlot(imageSlot1, null);
            SetImageSlot(imageSlot2, null);
            SetImageSlot(imageSlot3, null);

            if (prevButton != null) prevButton.interactable = false;
            if (nextButton != null) nextButton.interactable = false;

            HideSimulationButton();

            // tetap simpan state sub/page (page akan 0)
            currentPageIndex = 0;
            SaveLastViewed();
            return;
        }

        currentPageIndex = Mathf.Clamp(currentPageIndex, 0, pages.Count - 1);
        var page = pages[currentPageIndex];

        string title = string.IsNullOrEmpty(page.title) ? sub.subTitle : page.title;
        if (contentTitleText != null) contentTitleText.text = title;
        if (contentBodyText != null) contentBodyText.text = page.bodyText ?? "";

        SetImageSlot(imageSlot1, page.image1);
        SetImageSlot(imageSlot2, page.image2);
        SetImageSlot(imageSlot3, page.image3);

        if (prevButton != null) prevButton.interactable = currentPageIndex > 0;
        if (nextButton != null) nextButton.interactable = currentPageIndex < pages.Count - 1;

        RefreshSimulationButton();

        // NEW: simpan setiap kali UI di-refresh (pindah page/sub-bab)
        SaveLastViewed();
    }

    private void SetImageSlot(Image slot, Sprite sprite)
    {
        if (slot == null) return;

        if (sprite == null)
        {
            slot.gameObject.SetActive(false);
            return;
        }

        slot.gameObject.SetActive(true);
        slot.sprite = sprite;
        slot.preserveAspect = true;
    }

    private void RefreshSimulationButton()
    {
        var rule = FindSimulationRule(currentSubIndex, currentPageIndex);

        if (rule == null || string.IsNullOrEmpty(rule.simulationSceneName))
        {
            HideSimulationButton();
            return;
        }

        if (simulationButton != null)
            simulationButton.gameObject.SetActive(true);

        if (simulationButtonText != null)
            simulationButtonText.text = string.IsNullOrEmpty(rule.buttonLabel) ? "Coba Simulasi" : rule.buttonLabel;
    }

    private SimulationRule FindSimulationRule(int subIndex, int pageIndex)
    {
        if (simulationRules == null) return null;

        for (int i = 0; i < simulationRules.Count; i++)
        {
            var r = simulationRules[i];
            if (r == null) continue;

            if (r.subIndex == subIndex && r.pageIndex == pageIndex)
                return r;
        }

        return null;
    }

    private void HideSimulationButton()
    {
        if (simulationButton != null)
            simulationButton.gameObject.SetActive(false);
    }

    private void OnClickSimulation()
    {
        var rule = FindSimulationRule(currentSubIndex, currentPageIndex);
        if (rule == null || string.IsNullOrEmpty(rule.simulationSceneName))
            return;

        // NEW: pastikan tersimpan sebelum pindah scene simulasi
        SaveLastViewed();

        SceneManager.LoadScene(rule.simulationSceneName);
    }

    private void SetupSidebarPositions()
    {
        if (sidebarPanel == null) return;

        sidebarShownPos = sidebarPanel.anchoredPosition;
        sidebarHiddenPos = sidebarShownPos + new Vector2(hiddenOffsetX, 0f);
    }

    public void ToggleSidebar()
    {
        if (sidebarPanel == null) return;

        sidebarShown = !sidebarShown;

        if (sidebarSlideRoutine != null)
            StopCoroutine(sidebarSlideRoutine);

        Vector2 from = sidebarPanel.anchoredPosition;
        Vector2 to = sidebarShown ? sidebarShownPos : sidebarHiddenPos;

        sidebarSlideRoutine = StartCoroutine(SlideSidebar(from, to, sidebarSlideDuration));
    }

    private IEnumerator SlideSidebar(Vector2 from, Vector2 to, float duration)
    {
        if (duration <= 0f)
        {
            sidebarPanel.anchoredPosition = to;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            sidebarPanel.anchoredPosition = Vector2.Lerp(from, to, k);
            yield return null;
        }

        sidebarPanel.anchoredPosition = to;
    }
}