using UnityEngine;
using UnityEngine.UI;

public class UIMuteButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite mutedSprite;
    [SerializeField] private Sprite unmutedSprite;

    private void Reset()
    {
        button = GetComponent<Button>();
    }

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            if (AudioController.Instance == null) return;
            AudioController.Instance.ToggleMute();
            RefreshIcon();
        });

        RefreshIcon();
    }

    private void RefreshIcon()
    {
        if (AudioController.Instance == null || targetImage == null) return;

        bool muted = AudioController.Instance.IsMuted();
        targetImage.sprite = muted ? mutedSprite : unmutedSprite;
    }
}