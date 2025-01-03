using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    
    public InputAction MoveAction;
    Rigidbody2D rigidbody2d;

    public int maxHealth = 5;
    public int health { get { return currentHealth; }}
    public int currentHealth;
    
    Vector2 move;
    public float speed = 3.0f;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damageCooldown;

    Animator animator;
    Vector2 moveDirection = new Vector2(1,0);

    public GameObject projectilePrefab;
    public InputAction launchAction;

    public InputAction talkAction;

    AudioSource audioSource;

    public int score;
    public bool gameOverstate;

    
    // Start is called before the first frame update
    void Start()
        {
            //QualitySettings.vSyncCount = 0;
            //Application.targetFrameRate = 10;
            MoveAction.Enable();
            rigidbody2d = GetComponent<Rigidbody2D>();
            currentHealth = maxHealth;
            animator = GetComponent<Animator>();
            launchAction.Enable();
            launchAction.performed += Launch;
            talkAction.Enable();
            talkAction.performed += FindFriend;
            audioSource = GetComponent<AudioSource>();
            gameOverstate = false;
        }

    // Update is called once per frame
    void Update()
        {
            move = MoveAction.ReadValue<Vector2>();
            //Debug.Log(move);
            if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y,0.0f))
                {
                    moveDirection.Set(move.x, move.y);
                    moveDirection.Normalize(); 
                }
            animator.SetFloat("Look X", moveDirection.x);
            animator.SetFloat("Look Y", moveDirection.y);
            animator.SetFloat("Speed", move.magnitude);
            if (isInvincible)
                {
                    damageCooldown -= Time.deltaTime;
                    if (damageCooldown < 0)
                        {
                            isInvincible = false;
                        }
                }
            if (score == 4 || health == 0)
                {
                    GameOver();
                }

            if(Input.GetKey(KeyCode.R))
                {
                    if (gameOverstate == true)
                        {
                            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                        }
                }
        }

    void FixedUpdate()
        {
        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);  
        }

    public void ChangeHealth (int amount)
        {
         if (amount < 0)
        {
         if (isInvincible)
         {
            return;
         }
        isInvincible = true;
        damageCooldown = timeInvincible;
        animator.SetTrigger("Hit");
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);

         
        }

    void Launch(InputAction.CallbackContext context)
         {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(moveDirection, 300);
            animator.SetTrigger("Launch");
         }

    void FindFriend(InputAction.CallbackContext context)
         {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f,  moveDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
                {
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                        {
                            UIHandler.instance.DisplayDialogue();
                        }
                }
         }

    public void PlaySound(AudioClip clip)
         {
            audioSource.PlayOneShot(clip);
         }

    public void ChangeScore (int Scoreamount)
        {
         
        score = score + Scoreamount;
        Debug.Log(score);
         
        }

    public void GameOver()
    {
        
        UIHandler.instance.DisplayGameOver();
        speed = 0;
        gameOverstate = true;

    
    }
}

