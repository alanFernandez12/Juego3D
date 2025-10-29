using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    private Animator animator;
    private bool isDead;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        isDead = false;
    }

    public bool IsDead => isDead;

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " recibi� " + damage + " de da�o. Salud restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log(gameObject.name + " muri�.");
        animator.SetBool("die", true);

        if (gameObject.CompareTag("Player"))
        {
            // L�gica adicional para el jugador (deshabilitar control, etc.) si se desea
        }
        else
        {
            // L�gica adicional para enemigos si se desea (Destroy tras animaci�n, etc.)
        }
    }
    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
