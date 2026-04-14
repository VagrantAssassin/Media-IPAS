using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string chapterSelectSceneName = "ChapterSelect";

    public void OnClickPlay()
    {
        SceneManager.LoadScene(chapterSelectSceneName);
    }

    public void OnClickAbout()
    {
        // Sementara: nanti bisa dibuka panel About atau pindah scene About
        Debug.Log("About Us clicked (TODO: open About panel/scene).");
    }

    public void OnClickGuide()
    {
        // Sementara: nanti bisa dibuka panel Guide atau pindah scene Guide
        Debug.Log("Guide clicked (TODO: open Guide panel/scene).");
    }

    public void OnClickExit()
    {
        Debug.Log("Exit clicked.");
        Application.Quit();
    }
}