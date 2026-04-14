using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChapterSelectUI : MonoBehaviour
{
    [Serializable]
    public class ChapterButtonBinding
    {
        public string chapterId;
        public Button button;
        public string targetSceneName;
    }

    [Header("Chapter Buttons (predefined in Hierarchy)")]
    [SerializeField] private ChapterButtonBinding[] chapters;

    [Header("Navigation Buttons")]
    [SerializeField] private Button previousButton;
    [SerializeField] private string previousSceneName = "MainMenu";
    [SerializeField] private Button exitButton;

    [Header("Options")]
    [SerializeField] private bool saveSelectedChapterToPlayerPrefs = true;
    [SerializeField] private string playerPrefsKey = "SelectedChapterId";

    private void Awake()
    {
        if (previousButton != null)
        {
            previousButton.onClick.RemoveAllListeners();
            previousButton.onClick.AddListener(OnClickPrevious);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(OnClickExit);
        }

        if (chapters == null) return;

        foreach (var ch in chapters)
        {
            if (ch == null || ch.button == null) continue;

            ch.button.onClick.RemoveAllListeners();

            var capturedId = ch.chapterId;
            var capturedScene = ch.targetSceneName;

            ch.button.onClick.AddListener(() => OnClickChapter(capturedId, capturedScene));
        }
    }

    public void OnClickPrevious()
    {
        if (string.IsNullOrEmpty(previousSceneName))
        {
            Debug.LogWarning("Previous Scene Name is empty. Assign it in Inspector.");
            return;
        }

        SceneManager.LoadScene(previousSceneName);
    }

    public void OnClickExit()
    {
        Debug.Log("Exit clicked.");
        Application.Quit();
    }

    private void OnClickChapter(string chapterId, string targetSceneName)
    {
        if (saveSelectedChapterToPlayerPrefs && !string.IsNullOrEmpty(chapterId))
        {
            PlayerPrefs.SetString(playerPrefsKey, chapterId);
            PlayerPrefs.Save();
        }

        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning($"Target scene is empty for chapter '{chapterId}'. Assign it in Inspector.");
            return;
        }

        SceneManager.LoadScene(targetSceneName);
    }
}