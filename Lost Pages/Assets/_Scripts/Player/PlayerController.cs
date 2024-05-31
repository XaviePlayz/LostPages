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

    public GameObject player;
    private Rigidbody2D rb;

    [Header("Movement / Player Settings")]
    private float moveSpeed;
    private float jumpForce;

    public Transform groundCheck;
    private float groundCheckRadius;
    public LayerMask whatIsGround;
    [SerializeField] private bool isGrounded;

    [Header("Animator")]
    private Animator anim;
    private string idleAnimationTrigger = "Idle";
    private string walkAnimationTrigger = "Walk";
    private string jumpAnimationTrigger = "Jump";
    private string fallAnimationTrigger = "Fall";

    [Header("Booleans")]
    private bool isWalking;
    private bool isFacingRight;
    private bool isRespawning;
    public bool hasReachedTheEnd;
    private bool lookOut = false;

    public bool allowedToWalk;
    public bool allowedToJump;

    public Transform currentRespawnPoint;

    [Header("Keys")]
    public GameObject keyHolder;
    public Sprite newKeyHolderSprite;
    public GameObject unlockPage;

    [Header("Scripts")]
    [SerializeField] private InteractableObject currentInteractable;

    void Awake()
    {
        player = this.gameObject;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        unlockPage.SetActive(false);
    }

    void Start()
    {
        moveSpeed = 3.5f;
        jumpForce = 6f;
        groundCheckRadius = 0.1f;
        isWalking = false;
        isFacingRight = true;
        isRespawning = false;
        hasReachedTheEnd = false;
        allowedToWalk = false;
        allowedToJump = false;
    }

    void Update()
    {
        isWalking = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);

        if (!isRespawning && !DialogueManager.Instance.isDialogueActive || !Tutorial.Instance.tutorialSequenceEnded)
        {
            float moveX = Input.GetAxis("Horizontal");
            // Reset speed to walking speed
            moveSpeed = 3.5f;

            // Move the player horizontally
            if (allowedToWalk)
            {
                rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
            }

            if (!DialogueManager.Instance.isDialogueActive || !Tutorial.Instance.tutorialSequenceEnded)
            {
                if (allowedToWalk)
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

                    if (moveX != 0 && isGrounded)
                    {
                        // Player is walking
                        anim.SetTrigger(walkAnimationTrigger);
                    }
                    else if (isGrounded)
                    {
                        // Player is idle and on the ground, trigger the Idle animation
                        anim.SetTrigger(idleAnimationTrigger);
                    }

                    isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
                    if (allowedToJump)
                    {
                        if (Input.GetButtonDown("Jump") && isGrounded || Input.GetKeyDown(KeyCode.W) && isGrounded || Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
                        {
                            // Jump when the Jump button is pressed
                            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                            anim.SetTrigger(jumpAnimationTrigger);
                        }
                    }
                }
            }
            else
            {
                anim.SetTrigger(idleAnimationTrigger);
            }
        }

        if (DialogueManager.Instance.isDialogueActive && Tutorial.Instance.tutorialSequenceEnded)
        {
            FreezePlayerX();
        }
        else
        {
            UnFreezePlayer();
        }

        if (InventoryManager.Instance.inventoryAlreadyOpened)
        {
            FreezePlayer();
        }
        else
        {
            UnFreezePlayer();
        }

        //Interact
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null && isGrounded)
        {
            currentInteractable.Interact();
        }

        if (rb.velocity.y < 0 && !isGrounded)
        {
            // Player is falling
            anim.SetTrigger(fallAnimationTrigger);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Page"))
        {
            AudioController.Instance.PlaySFX(6);

            Destroy(other.gameObject);

            if (InventoryManager.Instance.currentPage == 0)
            {
                DialogueManager.Instance.visualNovelCanvas.SetActive(true);
                InventoryManager.Instance.hasAccessToInventory = true;
            }
            InventoryManager.Instance.PageCollected();
        }

        if (other.CompareTag("LookOut"))
        {
            if (!lookOut)
            {
                lookOut = true;
                PlayDialogue.Instance.PlayNewDialogue(2);
                DialogueManager.Instance.visualNovelCanvas.SetActive(true);
            }
        }

        if (other.CompareTag("Key"))
        {
            Destroy(other.gameObject);

            AudioController.Instance.PlaySFX(5);

            keyHolder.GetComponent<SpriteRenderer>().sprite = newKeyHolderSprite;
            unlockPage.SetActive(true);
        }

        if (other.CompareTag("Ending"))
        {
            hasReachedTheEnd = true;

            FreezePlayer();

            if (InventoryManager.Instance.pages[11].GetComponent<Button>().interactable == true)
            {
                PlayDialogue.Instance.PlayNewDialogue(4);
            }
            else
            {
                PlayDialogue.Instance.PlayNewDialogue(3);
            }

            AudioController.Instance.PlaySFX(2);
            AudioController.Instance.PlayMusic(0);

            DialogueManager.Instance.visualNovelCanvas.SetActive(true);
            Tutorial.Instance.novelBackground.SetActive(true);
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

    private void FlipCharacter()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void FreezePlayerX()
    {
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        anim.SetTrigger(idleAnimationTrigger);
    }

    public void FreezePlayer()
    {
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        anim.SetTrigger(idleAnimationTrigger);
    }

    public void UnFreezePlayer()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}