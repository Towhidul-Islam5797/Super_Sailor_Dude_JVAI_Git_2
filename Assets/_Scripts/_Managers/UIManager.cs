using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject _gameOverPanel;
    public GameObject _winPanel;


    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(false);

        if (_winPanel != null)
            _winPanel.SetActive(false);
    }


    // =====================================================
    // GAME OVER
    // =====================================================

    public void ShowGameOver()
    {
        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(true);
    }


    // =====================================================
    // WIN
    // =====================================================

    public void ShowWinPanel()
    {
        if (_winPanel != null)
            _winPanel.SetActive(true);
    }


    // =====================================================
    // STICKER BOOK TOGGLE
    // =====================================================

    public void OpenStickerBook()
    {
        if (StickerManager.Instance != null)
        {
            // একবার click = Open
            // আবার click = Close
            StickerManager.Instance.ToggleStickerBook();
        }
        else
        {
            Debug.LogError(
                "StickerManager.Instance is NULL!"
            );
        }
    }
}