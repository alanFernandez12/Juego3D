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
        Debug.Log(gameObject.name + " recibió " + damage + " de daño. Salud restante: " + currentHealth);

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
        Debug.Log(gameObject.name + " murió.");
        animator.SetBool("die", true);

        if (gameObject.CompareTag("Player"))
        {
            // Lógica adicional para el jugador (deshabilitar control, etc.) si se desea
        }
        else
        {
            // Lógica adicional para enemigos si se desea (Destroy tras animación, etc.)
        }
    }
    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
