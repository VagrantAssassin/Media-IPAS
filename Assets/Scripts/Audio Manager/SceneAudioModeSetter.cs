using UnityEngine;

public class SceneAudioModeSetter : MonoBehaviour
{
    [SerializeField] private AudioController.BgmMode mode = AudioController.BgmMode.Menu;

    private void Start()
    {
        if (AudioController.Instance != null)
            AudioController.Instance.SetMode(mode);
    }
}