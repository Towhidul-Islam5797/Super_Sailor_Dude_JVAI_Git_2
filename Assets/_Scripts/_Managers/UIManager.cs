using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject _gameOverPanel;
    public GameObject _winPanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _gameOverPanel.SetActive(false);
        _winPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        _gameOverPanel.SetActive(true);
    }

    public void ShowWinPanel()
    {
        _winPanel.SetActive(true);
    }

    // Called by Sticker Book button (wire this up in Inspector)
    public void OpenStickerBook()
    {
        StickerManager.Instance.OpenStickerBook();
    }

    public void CloseStickerBook()
    {
        StickerManager.Instance.CloseStickerBook();
    }
}