using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public float speed;
    Rigidbody2D rigidbody2d;
    public bool vertical;
    public float changeTime = 3.0f;

    float timer;
    int direction = 1;

    Animator animator;

    bool aggressive = true;

    public ParticleSystem smokeEffect;

     private PlayerController playerController;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
        GameObject playerControllerObject = GameObject.FindWithTag("RubyController");
         if (playerControllerObject != null)
        {
            playerController = playerControllerObject.GetComponent<PlayerController>();
            print ("Found the GameConroller Script!");
        }
        if (playerController == null)
        {
            print ("Cannot find GameController Script!");
        }
    }

    void Update()
    {

       timer = timer-Time.deltaTime;
       //Debug.Log(timer);


      if (timer < 0)
      {
        direction = -direction;
        timer = changeTime;
      }
    }

    void FixedUpdate()
    {
        if(!aggressive)
        {
           return;
        }
        Vector2 position = rigidbody2d.position;
        if (vertical)
        {
           position.y = position.y + speed * direction * Time.deltaTime;
           animator.SetFloat("Move X", 0);
           animator.SetFloat("Move Y", direction);
        }
       else
        {
           position.x = position.x + speed * direction * Time.deltaTime;
           animator.SetFloat("Move X", direction);
           animator.SetFloat("Move Y", 0);
        }
       rigidbody2d.MovePosition(position);
    }

   void OnCollisionEnter2D(Collision2D other)
    {
      Debug.Log("EnemyCollider: " + other.gameObject.name);
      PlayerController player = other.gameObject.GetComponent<PlayerController>();
      
      
      if (player != null)
       {
          player.ChangeHealth(-1);
       }
    }

    public void Fix()
   {
       aggressive = false;
       rigidbody2d.simulated = false;
       animator.SetTrigger("Fixed");
       smokeEffect.Stop();
       if (playerController != null)
       {
         playerController.ChangeScore(1);
       }
   }

}
