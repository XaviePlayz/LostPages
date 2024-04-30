using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    #region Singleton

    private static PlayerController _instance;
    public static PlayerController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerController>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(PlayerController).Name;
                    _instance = obj.AddComponent<PlayerController>();
                }
            }
            return _instance;
        }
    }

    #endregion


    private GameObject player;
    private Rigidbody2D rb;

    [Header("Movement / Player Settings")]
    public float moveSpeed = 3.5f;
    public float jumpForce = 5f;
    public Transform respawnPoint;

    [Header("Animator")]
    private Animator anim;
    private string idleAnimationTrigger = "Idle";
    private string walkAnimationTrigger = "Walk";
    private string jumpAnimationTrigger = "Jump";
    private string fallAnimationTrigger = "Fall";
    private string hurtAnimationTrigger = "Hurt";

    [Header("PageInventory")]
    public int pagesCollected;

    [Header("Booleans")]
    public bool isJumping = false;
    private bool isWalking = false;
    private bool isFacingRight = true;
    private bool isRespawning = false;
    public bool isHurt = false;
    public bool isInDialogue = false;

    [Header("Scripts")]
    [SerializeField] private InteractableObject currentInteractable;

    private void Awake()
    {
        player = this.gameObject;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        isWalking = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);

        if (!isRespawning)
        {
            float moveX = Input.GetAxis("Horizontal");
            // Reset speed to walking speed
            moveSpeed = 3.5f;

            // Move the player horizontally
            rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

            if (!isInDialogue)
                {

                if (moveX > 0 && !isFacingRight)
                {
                    // Player is moving right and facing left, flip the character
                    FlipCharacter();
                }
                else if (moveX < 0 && isFacingRight)
                {
                    // Player is moving left and facing right, flip the character
                    FlipCharacter();
                }

                if (moveX != 0 && !isJumping)
                {
                    // Player is walking
                    anim.SetTrigger(walkAnimationTrigger);
                }
                else if (!isJumping)
                {
                    // Player is idle and on the ground, trigger the Idle animation
                    anim.SetTrigger(idleAnimationTrigger);
                }

                if (Input.GetButtonDown("Jump") && !isJumping || Input.GetKeyDown(KeyCode.W) && !isJumping || Input.GetKeyDown(KeyCode.UpArrow) && !isJumping)
                {
                    // Jump when the Jump button is pressed
                    rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                    isJumping = true;
                    anim.SetTrigger(jumpAnimationTrigger);
                }
            }
            else
            {
                anim.SetTrigger(idleAnimationTrigger);
            }
        }

        //Interact
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null && !isJumping)
        {
            currentInteractable.Interact();
        }

        if (rb.velocity.y < 0 && isJumping)
        {
            // Player is falling
            anim.SetTrigger(fallAnimationTrigger);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Page"))
        {
            Destroy(other.gameObject);

            if (pagesCollected == 0)
            {
                DialogueManager.Instance.visualNovelCanvas.SetActive(true);
                InventoryManager.Instance.hasAccessToInventory = true;
            }
            pagesCollected++;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interactable_Object"))
        {
            currentInteractable = other.GetComponent<InteractableObject>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable_Object"))
        {
            currentInteractable = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Reset jumping state when touching the ground
            isJumping = false;
        }
    }

    private void FlipCharacter()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TakeDamage()
    {
        if (!isRespawning && !isHurt)
        {
            // Player takes damage (e.g., enemy collision)
            isHurt = true;
            isRespawning = true;
            FreezePlayer();
            anim.SetTrigger(hurtAnimationTrigger);
            rb.velocity = Vector2.zero;
            StartCoroutine(RespawnAfterDelay());
        }
    }

    public void FreezePlayer()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
    }

    public void UnFreezePlayer()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private IEnumerator RespawnAfterDelay()
    {
        // Freeze player movement
        rb.velocity = Vector2.zero;
        isRespawning = true;

        // Wait for the duration of the hurt animation
        yield return new WaitForSeconds(GetAnimationDuration(anim, hurtAnimationTrigger));

        // Respawn player
        transform.position = respawnPoint.position;
        UnFreezePlayer();
        isRespawning = false;

        yield return new WaitForSeconds(2);
        isHurt = false;
    }

    private float GetAnimationDuration(Animator animator, string animationTrigger)
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        AnimationClip clip = clipInfo[0].clip;
        return clip.length;
    }
}