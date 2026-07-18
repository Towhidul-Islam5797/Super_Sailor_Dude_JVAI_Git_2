using System.Collections;
using UnityEngine;

public class StickerTrigger : MonoBehaviour
{
    [SerializeField] private string stickerId;

    [Header("Billboard Visual")]
    [SerializeField] private SpriteRenderer billboardRenderer; // Drag the billboard SpriteRenderer here
    [SerializeField] private Sprite billboardSticker;          // Drag the sticker sprite (1 copy, 2 copy etc.)

    private bool _triggered = false;

    private void Start()
    {
        // Always show the sticker on the billboard from the start
        if (billboardRenderer != null && billboardSticker != null)
        {
            billboardRenderer.sprite = billboardSticker;
            Color c = billboardRenderer.color;
            c.a = 1f;
            billboardRenderer.color = c;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_triggered) return;

        if (collision.CompareTag("PlayerBody"))
        {
            _triggered = true;

            // Get the PlayerStateManager from the parent Player object
            PlayerStateManager player = GameObject.FindGameObjectWithTag("Player")
                .GetComponent<PlayerStateManager>();

            if (player != null)
            {
                StartCoroutine(HandleStickerCollection(player));
            }
            else
            {
                // Fallback: just show sticker without freezing
                StickerManager.Instance.CollectSticker(stickerId);
            }
        }
    }

    private IEnumerator HandleStickerCollection(PlayerStateManager player)
    {
        // Step 1: Freeze the player
        player.FreezePlayer();

        // Step 2: Brief pause (feels like the character "stops and looks")
        yield return new WaitForSeconds(0.3f);

        // Step 3: Trigger the sticker collection (shows popup + saves)
        StickerManager.Instance.CollectSticker(stickerId);

        // Step 4: Wait for the sticker popup to be visible
        yield return new WaitForSeconds(1.5f);

        // Step 5: Unfreeze player
        player.UnfreezePlayer();
    }
}