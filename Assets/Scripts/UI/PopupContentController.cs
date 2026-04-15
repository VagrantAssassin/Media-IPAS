using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupContentController : MonoBehaviour
{
    private enum PopupType { Guide, AboutUs }

    [Serializable]
    public class Page
    {
        public string title;
        [TextArea(3, 10)] public string description;
        public Sprite image;
    }

    [Header("Popup Root (Modal Overlay)")]
    [SerializeField] private GameObject popupPanel; // panel 1 layar (blok input)

    [Header("Content UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Image contentImage;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    [Header("Pages Data")]
    [SerializeField] private List<Page> guidePages = new();
    [SerializeField] private List<Page> aboutUsPages = new();

    private PopupType currentType;
    private int currentIndex;

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }

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

        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void OpenGuide()
    {
        Open(PopupType.Guide);
    }

    public void OpenAboutUs()
    {
        Open(PopupType.AboutUs);
    }

    public void Close()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void NextPage()
    {
        var pages = GetActivePages();
        if (pages == null || pages.Count == 0) return;

        currentIndex = (currentIndex + 1) % pages.Count;
        RefreshUI();
    }

    public void PrevPage()
    {
        var pages = GetActivePages();
        if (pages == null || pages.Count == 0) return;

        currentIndex = (currentIndex - 1 + pages.Count) % pages.Count;
        RefreshUI();
    }

    private void Open(PopupType type)
    {
        currentType = type;
        currentIndex = 0;

        if (popupPanel != null)
            popupPanel.SetActive(true);

        RefreshUI();
    }

    private List<Page> GetActivePages()
    {
        return currentType == PopupType.Guide ? guidePages : aboutUsPages;
    }

    private void RefreshUI()
    {
        var pages = GetActivePages();

        if (pages == null || pages.Count == 0)
        {
            if (titleText != null) titleText.text = "Konten belum tersedia";
            if (descriptionText != null) descriptionText.text = "Halaman belum diisi di Inspector.";

            if (contentImage != null)
                contentImage.gameObject.SetActive(false);

            SetNavButtonsVisible(false);
            return;
        }

        // Pastikan index aman
        currentIndex = Mathf.Clamp(currentIndex, 0, pages.Count - 1);

        var page = pages[currentIndex];

        if (titleText != null) titleText.text = page.title ?? "";
        if (descriptionText != null) descriptionText.text = page.description ?? "";

        if (contentImage != null)
        {
            if (page.image == null)
            {
                contentImage.gameObject.SetActive(false);
            }
            else
            {
                contentImage.gameObject.SetActive(true);
                contentImage.sprite = page.image;
                contentImage.preserveAspect = true;
            }
        }

        SetNavButtonsVisible(pages.Count > 1);
    }

    private void SetNavButtonsVisible(bool visible)
    {
        if (prevButton != null) prevButton.gameObject.SetActive(visible);
        if (nextButton != null) nextButton.gameObject.SetActive(visible);
    }
}