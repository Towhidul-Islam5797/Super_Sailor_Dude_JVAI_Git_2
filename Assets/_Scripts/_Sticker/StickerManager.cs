using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerManager : MonoBehaviour
{
    public static StickerManager Instance;

    [System.Serializable]
    public class StickerData
    {
        public string id;
        public Sprite sprite;
    }

    [Header("All Stickers")]
    [SerializeField] private List<StickerData> stickers = new List<StickerData>();

    [Header("Collection Popup")]
    [SerializeField] private GameObject popup;
    [SerializeField] private Image popupImage;

    [Header("Sticker Book")]
    [SerializeField] private GameObject stickerBookPanel;

    [Tooltip("Book > Content > Grid-এর ভিতরের Image component গুলো এখানে drag করো")]
    [SerializeField] private List<Image> stickerSlots = new List<Image>();

    private string currentStickerID;
    private Sprite currentStickerSprite;
    private PlayerStateManager currentPlayer;

    private const string SAVE_PREFIX = "Sticker_";


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    private void Start()
    {
        if (popup != null)
            popup.SetActive(false);

        if (stickerBookPanel != null)
            stickerBookPanel.SetActive(false);

        RefreshBook();
    }


    // =========================================================
    // SHOW POPUP
    // =========================================================

    public void ShowStickerPopup(
        string stickerID,
        Sprite sprite,
        PlayerStateManager player)
    {
        if (string.IsNullOrEmpty(stickerID))
        {
            Debug.LogWarning("Sticker ID is empty!");
            return;
        }

        // যদি আগে collect করা থাকে
        if (IsStickerCollected(stickerID))
        {
            if (player != null)
                player.UnfreezePlayer();

            return;
        }

        currentStickerID = stickerID;
        currentStickerSprite = sprite;
        currentPlayer = player;

        // Popup-এ trigger-এর sticker দেখাও
        if (popupImage != null)
        {
            popupImage.sprite = sprite;
            popupImage.color = Color.white;
            popupImage.enabled = true;
        }

        if (popup != null)
            popup.SetActive(true);

        Debug.Log("Sticker Popup Open: " + stickerID);
    }


    // =========================================================
    // COLLECTION BUTTON
    // =========================================================

    public void CollectCurrentSticker()
    {
        if (string.IsNullOrEmpty(currentStickerID))
        {
            Debug.LogWarning("No sticker waiting for collection.");
            return;
        }

        // Save sticker
        PlayerPrefs.SetInt(
            SAVE_PREFIX + currentStickerID,
            1
        );

        PlayerPrefs.Save();

        Debug.Log("Sticker Collected: " + currentStickerID);

        // Book update
        RefreshBook();

        // Popup close
        if (popup != null)
            popup.SetActive(false);

        // Player unfreeze
        if (currentPlayer != null)
            currentPlayer.UnfreezePlayer();

        currentStickerID = null;
        currentStickerSprite = null;
        currentPlayer = null;
    }


    // =========================================================
    // BOOK TOGGLE
    // =========================================================

    public void ToggleStickerBook()
    {
        if (stickerBookPanel == null)
            return;

        if (stickerBookPanel.activeSelf)
        {
            // Open থাকলে Close
            stickerBookPanel.SetActive(false);
        }
        else
        {
            // Close থাকলে Open
            RefreshBook();
            stickerBookPanel.SetActive(true);
        }
    }


    // =========================================================
    // BOOK REFRESH
    // =========================================================

    private void RefreshBook()
    {
        // সব collected sticker বের করব
        List<Sprite> collectedSprites = new List<Sprite>();

        foreach (StickerData sticker in stickers)
        {
            if (sticker == null)
                continue;

            if (string.IsNullOrEmpty(sticker.id))
                continue;

            if (sticker.sprite == null)
                continue;

            if (IsStickerCollected(sticker.id))
            {
                collectedSprites.Add(sticker.sprite);
            }
        }


        // -----------------------------------------------------
        // Content > Grid-এর Image slots-এ বসানো
        // -----------------------------------------------------

        for (int i = 0; i < stickerSlots.Count; i++)
        {
            Image slot = stickerSlots[i];

            if (slot == null)
                continue;


            if (i < collectedSprites.Count)
            {
                // Collected sticker আছে
                slot.sprite = collectedSprites[i];
                slot.color = Color.white;
                slot.enabled = true;
            }
            else
            {
                // Empty slot
                slot.sprite = null;
                slot.enabled = false;
            }

            // Slot-এর GameObject কখনো disable করছি না
            slot.gameObject.SetActive(true);
        }
    }


    // =========================================================
    // CHECK SAVE
    // =========================================================

    public bool IsStickerCollected(string id)
    {
        return PlayerPrefs.GetInt(
            SAVE_PREFIX + id,
            0
        ) == 1;
    }


    // =========================================================
    // CLEAR SAVE - TESTING
    // =========================================================

    [ContextMenu("Clear All Sticker Saves")]
    public void ClearAllStickerSaves()
    {
        foreach (StickerData sticker in stickers)
        {
            if (sticker == null)
                continue;

            if (string.IsNullOrEmpty(sticker.id))
                continue;

            PlayerPrefs.DeleteKey(
                SAVE_PREFIX + sticker.id
            );
        }

        PlayerPrefs.Save();

        RefreshBook();

        Debug.Log("All sticker saves cleared.");
    }
} 