using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " recibió " + damage + " de daño. Salud restante: " + currentHealth);

        if (currentHealth < 0)
        {
            Die();

        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " murió.");
        animator.SetBool("die", true);

        if (gameObject.CompareTag("Player"))
        {
            //gameObject.SetActive(false);

        }
        else
        {
            //Destroy(gameObject, 2f);// O cualquier lógica de muerte (animación, etc.)
        }



    }
}
