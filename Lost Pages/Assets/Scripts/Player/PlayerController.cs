using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;

    [Header("Movement / Player Settings")]
    public float moveSpeed = 3.5f;
    public float jumpForce = 5f;
    public Transform respawnPoint;

    [Header("Stamina")]
    public GameObject staminaBarGameObject;
    public RectTransform staminaBarRectTransform;
    public float maxStamina = 3f;
    public float currentStamina;
    public float staminaRecoveryRate = 1f;
    public float sprintSpeed = 6f;

    private float staminaRegenDelay = 1f;
    private bool isSprinting = false;
    private bool outOfStamina;
    public Image staminaBarImageLeft;
    public Image staminaBarImageRight;

    private Color barColorFullStamina = Color.white;
    private Color barColorOutOfStamina = new Color(75f / 255f, 75f / 255f, 75f / 255f);

    [Header("Animator")]
    private Animator anim;
    private string idleAnimationTrigger = "Idle";
    private string walkAnimationTrigger = "Walk";
    private string runAnimationTrigger = "Run";
    private string jumpAnimationTrigger = "Jump";
    private string fallAnimationTrigger = "Fall";
    private string hideAnimationTrigger = "Hide";
    private string hurtAnimationTrigger = "Hurt";

    [Header("Booleans")]
    public bool isJumping = false;
    private bool isWalking = false;
    private bool isFacingRight = true;
    private bool isRespawning = false;
    public bool isHiding = false;
    public bool isPlayerVisible = true;
    public bool isUsingDoor = false;
    public bool isHurt = false;
    public bool inDialogue = false;

    [Header("Door Interaction")]
    public AudioSource doorAudioSource;
    public AudioClip doorOpenSound;

    [Header("Closet Interaction")]
    public AudioSource closetAudioSource;
    public AudioClip closetOpenSound;

    [Header("Scripts")]
    [SerializeField] private InteractableObject currentInteractable;

    private void Awake()
    {
        player = this.gameObject;

        currentStamina = maxStamina;

        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        isWalking = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);

        if (!isRespawning)
        {
            if (!isHiding)
            {
                float moveX = Input.GetAxis("Horizontal");

                // Sprinting check
                if (Input.GetKey(KeyCode.LeftShift) && moveX != 0 && !isJumping && isPlayerVisible && !outOfStamina)
                {
                    ShowStaminaBar();
                    if (currentStamina > 0)
                    {
                        isSprinting = true;
                        moveSpeed = sprintSpeed;

                        // Decrease stamina while sprinting
                        currentStamina -= Time.deltaTime;
                    }
                    else
                    {
                        // Out of stamina, reduce speed and start recovery
                        outOfStamina = true;
                        moveSpeed = 3f;
                    }
                }
                else
                {
                    isSprinting = false;

                    StartCoroutine(RecoveringStamina());

                    if (currentStamina >= maxStamina)
                    {
                        // Stop recovering when stamina is full
                        currentStamina = maxStamina;
                        outOfStamina = false;
                    }

                    if (!outOfStamina)
                    {
                        // Reset speed to walking speed
                        moveSpeed = 3.5f;
                    }
                    else
                    {
                        moveSpeed = 2.5f;
                    }

                }

                // Update the stamina bar UI
                UpdateStaminaBar();

                // Move the player horizontally
                rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

                if (!inDialogue)
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
                        if (isSprinting)
                        {
                            // Player is running
                            anim.SetTrigger(runAnimationTrigger);
                        }
                        else
                        {
                            // Player is walking
                            anim.SetTrigger(walkAnimationTrigger);
                        }
                    }
                    else if (!isJumping && !isSprinting)
                    {
                        // Player is idle and on the ground, trigger the Idle animation
                        anim.SetTrigger(idleAnimationTrigger);
                    }

                    if (Input.GetButtonDown("Jump") && !isJumping && isPlayerVisible || Input.GetKeyDown(KeyCode.W) && !isJumping && isPlayerVisible || Input.GetKeyDown(KeyCode.UpArrow) && !isJumping && isPlayerVisible)
                    {
                        // Jump when the Jump button is pressed
                        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                        isJumping = true;
                        anim.SetTrigger(jumpAnimationTrigger);
                    }

                    //Start Crouching (Hiding)
                    if (Input.GetKeyDown(KeyCode.S) && !isJumping && moveX == 0 || Input.GetKeyDown(KeyCode.DownArrow) && !isJumping && moveX == 0)
                    {
                        // Hide animation triggered when the S key is pressed and the player is on the ground and standing still
                        anim.SetBool(hideAnimationTrigger, true);
                        isHiding = true;
                        player.tag = "HidingPlayer";
                    }
                }
                else
                {
                    anim.SetTrigger(idleAnimationTrigger);
                }
            }

            //Stop Crouching (Hiding)
            if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                isHiding = false;
                anim.SetBool(hideAnimationTrigger, false);
                anim.SetTrigger(idleAnimationTrigger);
                player.tag = "Player";
            }

            //Interact
            if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null && !isJumping)
            {
                if (!isUsingDoor)
                {
                    isPlayerVisible = false;
                }
                else
                {
                    // Play door open sound when interacting with the door
                    doorAudioSource.clip = doorOpenSound;
                    doorAudioSource.Play();
                }


                if(!isPlayerVisible)
                {
                    // Play door open sound when interacting with the door
                    closetAudioSource.clip = closetOpenSound;
                    closetAudioSource.Play();
                }
                currentInteractable.Interact();
            }

            if (rb.velocity.y < 0 && isJumping)
            {
                // Player is falling
                anim.SetTrigger(fallAnimationTrigger);
            }
        }
    }

    private void LateUpdate()
    {
        UpdateStaminaBarPosition();
    }

    private void UpdateStaminaBar()
    {
        // Calculate the fill amount based on current stamina
        float fillAmount = currentStamina / maxStamina;

        // Assuming you have a UI Image component for the stamina bar
        staminaBarImageLeft.fillAmount = fillAmount;
        staminaBarImageRight.fillAmount = fillAmount;

        // Change the color based on stamina
        if (!outOfStamina)
        {
            Debug.Log("Full Stamina");

            if (!isSprinting && currentStamina == maxStamina)
            {
                HideStaminaBar();
            }
        }
        else
        {
            Debug.Log("Out of Stamina");
            StartCoroutine(RecoveringStamina());
        }
    }

    private void HideStaminaBar()
    {
        staminaBarGameObject.SetActive(false);
    }

    private void ShowStaminaBar()
    {
        staminaBarGameObject.SetActive(true);
    }

    IEnumerator RecoveringStamina()
    {
        yield return new WaitForSeconds(staminaRegenDelay);
        currentStamina += Time.deltaTime * staminaRecoveryRate;

        while (outOfStamina)
        {
            staminaBarImageLeft.color = barColorOutOfStamina;
            staminaBarImageRight.color = barColorOutOfStamina;
        }

        staminaBarImageLeft.color = barColorFullStamina;
        staminaBarImageRight.color = barColorFullStamina;
    }

    private void UpdateStaminaBarPosition()
    {
        if (staminaBarRectTransform != null)
        {
            Vector3 playerPosition = player.transform.position;

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(playerPosition);

            staminaBarRectTransform.position = new Vector3(screenPosition.x, staminaBarRectTransform.position.y, staminaBarRectTransform.position.z);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interactable_Door"))
        {
            currentInteractable = other.GetComponent<InteractableObject>();
            isUsingDoor = true;
        }
        else if (other.CompareTag("Interactable_Object"))
        {
            currentInteractable = other.GetComponent<InteractableObject>();
            isUsingDoor = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable_Door") && isPlayerVisible)
        {
            currentInteractable = null;
            isUsingDoor = false;
        }
        else if (other.CompareTag("Interactable_Object") && isPlayerVisible)
        {
            currentInteractable = null;
            isUsingDoor = false;
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