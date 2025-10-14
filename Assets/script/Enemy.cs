using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator animator;
    private int hitHash;
    private GameObject colisionSword;
    [SerializeField] private float hitCooldown = 0.05f;
    private float lastHitTime;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        hitHash = Animator.StringToHash("hit");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("sword"))
        {
            colisionSword = other.gameObject;
           

        }
         
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("sword"))
        {
            colisionSword = null;


        }
       
    }
    public void HitDamage()
    {
        if (colisionSword != null)
        {
            if (Time.time - lastHitTime < hitCooldown) return;
            lastHitTime = Time.time;

            //animator.SetBool("hit", true);
            animator.SetTrigger(hitHash); // Trigger en vez de Bool
            gameObject.GetComponent<HealthSystem>().TakeDamage(30);


            Debug.Log("Enemy hit by sword, player takes damage.");
        }
        else animator.SetBool("hit", false);



    }

}
   





