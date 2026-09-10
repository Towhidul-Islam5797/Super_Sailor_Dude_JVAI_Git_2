using UnityEngine;

public class StickerTrigger : MonoBehaviour
{
    [Header("Sticker")]
    [SerializeField] private string stickerID = "1";
    [SerializeField] private Sprite stickerSprite;

    private bool alreadyTriggered = false;


    private void Start()
    {
        // শুধু এই sticker-টাই আগে collect হয়েছে কিনা check করবে
        if (StickerManager.Instance != null)
        {
            alreadyTriggered =
                StickerManager.Instance.IsStickerCollected(stickerID);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyTriggered)
            return;


        // PlayerStateManager যেই parent/child-এ থাকুক খুঁজবে
        PlayerStateManager player =
            other.GetComponentInParent<PlayerStateManager>();


        if (player == null)
            return;


        if (StickerManager.Instance == null)
        {
            Debug.LogError("StickerManager not found!");
            return;
        }


        Debug.Log(
            "TRIGGERED STICKER: " + stickerID
        );


        // শুধু এই trigger বন্ধ হবে
        alreadyTriggered = true;


        // Player freeze
        player.FreezePlayer();


        // এই trigger-এর নিজের sticker popup-এ যাবে
        StickerManager.Instance.ShowStickerPopup(
            stickerID,
            stickerSprite,
            player
        );
    }
}