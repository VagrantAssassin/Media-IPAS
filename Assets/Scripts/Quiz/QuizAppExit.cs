using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizAppExit : MonoBehaviour
{
    [SerializeField] private string exitTargetSceneName = "Materi";

    public void ExitApp()
    {
        if (!string.IsNullOrEmpty(exitTargetSceneName))
        {
            SceneManager.LoadScene(exitTargetSceneName);
            return;
        }

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}