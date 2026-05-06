using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulasiExitButton : MonoBehaviour
{
    [SerializeField] private string bab1SceneName = "Bab1"; // ganti sesuai nama scene kamu

    public void ExitToBab1()
    {
        SceneManager.LoadScene(bab1SceneName);
    }
}