using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerManager : MonoBehaviour
{
    public static StickerManager Instance;

    [System.Serializable]
    public class StickerEntry
    {
        public string id;
        public Sprite stickerSprite;           // The sticker image (assign in Inspector)
    }

    [Header("Sticker Config")]
    [SerializeField] private List<StickerEntry> stickers;

    [Header("Sticker Popup (World Space - on Billboard)")]
    [SerializeField] private GameObject stickerPopupPanel;   // A small UI panel that appears briefly
    [SerializeField] private Image stickerPopupImage;        // Image component inside popup panel
    [SerializeField] private float popupDuration = 1.5f;

    [Header("Sticker Book UI")]
    [SerializeField] private GameObject stickerBookPanel;    // The full sticker book panel
    [SerializeField] private List<Image> stickerSlots;       // 10 slots in the book UI grid

    // Internal
    private Dictionary<string, StickerEntry> _stickerMap = new Dictionary<string, StickerEntry>();
    private HashSet<string> _collectedIds = new HashSet<string>();

    private const string PREFS_PREFIX = "Sticker_";

    private void Awake()
    {
        Instance = this;

        foreach (var entry in stickers)
        {
            if (string.IsNullOrEmpty(entry.id)) continue;
            _stickerMap[entry.id] = entry;

            // Load saved state
            if (PlayerPrefs.GetInt(PREFS_PREFIX + entry.id, 0) == 1)
            {
                _collectedIds.Add(entry.id);
            }
        }
    }

    private void Start()
    {
        // Hide popup on start
        if (stickerPopupPanel != null)
            stickerPopupPanel.SetActive(false);

        // Hide sticker book on start
        if (stickerBookPanel != null)
            stickerBookPanel.SetActive(false);

        // Refresh book slots to show already collected stickers
        RefreshStickerBook();
    }

    // Called by StickerTrigger
    public void CollectSticker(string id)
    {
        if (!_stickerMap.ContainsKey(id))
        {
            Debug.LogWarning($"Sticker ID not found: {id}");
            return;
        }

        // Save to PlayerPrefs even if already collected
        if (!_collectedIds.Contains(id))
        {
            _collectedIds.Add(id);
            PlayerPrefs.SetInt(PREFS_PREFIX + id, 1);
            PlayerPrefs.Save();
        }

        // Auto-open the book so player sees the new sticker placed
        OpenStickerBook();

        // Show the popup briefly
        StartCoroutine(ShowPopup(_stickerMap[id].stickerSprite));
    }

    private IEnumerator ShowPopup(Sprite sprite)
    {
        if (stickerPopupPanel != null && stickerPopupImage != null)
        {
            stickerPopupImage.sprite = sprite;
            stickerPopupPanel.SetActive(true);

            yield return new WaitForSeconds(popupDuration);

            stickerPopupPanel.SetActive(false);
        }
    }

    // Called by the Sticker Book button in UI
    public void OpenStickerBook()
    {
        RefreshStickerBook();
        if (stickerBookPanel != null)
            stickerBookPanel.SetActive(true);
    }

    public void CloseStickerBook()
    {
        if (stickerBookPanel != null)
            stickerBookPanel.SetActive(false);
    }

    private void RefreshStickerBook()
    {
        // Slots are indexed 0-9, IDs are "1"-"10"
        for (int i = 0; i < stickerSlots.Count; i++)
        {
            string id = (i + 1).ToString();

            if (_collectedIds.Contains(id) && _stickerMap.ContainsKey(id))
            {
                // Collected — show sticker in full color
                stickerSlots[i].sprite = _stickerMap[id].stickerSprite;
                stickerSlots[i].color = Color.white;
                stickerSlots[i].gameObject.SetActive(true);
            }
            else
            {
                // Not collected — completely hidden, nothing shown on book
                stickerSlots[i].gameObject.SetActive(false);
            }
        }
    }

    // Utility: reset all stickers (for testing)
    [ContextMenu("Clear All Sticker Saves")]
    public void ClearAllStickerSaves()
    {
        for (int i = 1; i <= 10; i++)
        {
            PlayerPrefs.DeleteKey(PREFS_PREFIX + i.ToString());
        }
        PlayerPrefs.Save();
        _collectedIds.Clear();
        RefreshStickerBook();
        Debug.Log("All sticker saves cleared.");
    }
}