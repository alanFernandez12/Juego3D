using UnityEngine;
using UnityEngine.Events;

// Componente reutilizable para gestionar la vida de jugadores y enemigos.
// Permite recibir da�o, curarse, morir, revivir y notificar cambios (p.ej. UI o animaciones).
[DisallowMultipleComponent]
public class HealthManager : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [Tooltip("Si es < 0, inicia con maxHealth")] [SerializeField] private float startHealth = -1f;

    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public float NormalizedHealth => maxHealth > 0f ? CurrentHealth / maxHealth : 0f;
    public bool IsDead { get; private set; }

    [Header("Death/Despawn")]
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private float destroyDelay = 2f;
    [SerializeField] private Animator animator;
    [Tooltip("Nombre del Trigger para animaci�n de muerte (opcional)")]
    [SerializeField] private string deathTriggerName = "die";

    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    [Header("Events")]
    public UnityEvent onHealthChanged;      // Llamado cuando cambia la vida (da�o o curaci�n)
    public UnityEvent onDeath;              // Llamado al morir
    public UnityEvent onDamaged;            // Llamado al recibir da�o
    public UnityEvent onHealed;             // Llamado al curarse
    public FloatEvent onDamagedAmount;      // Cantidad exacta de da�o aplicada
    public FloatEvent onHealedAmount;       // Cantidad exacta de curaci�n aplicada

    private bool deathHandled;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();

        if (maxHealth <= 0f) maxHealth = 1f;
        CurrentHealth = (startHealth < 0f) ? maxHealth : Mathf.Clamp(startHealth, 0f, maxHealth);
        IsDead = CurrentHealth <= 0f;
    }

    // Aplica da�o a este objeto. Ignora valores <= 0 o si ya est� muerto.
    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;

        float prev = CurrentHealth;
        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);

        onDamaged?.Invoke();
        onDamagedAmount?.Invoke(prev - CurrentHealth);
        onHealthChanged?.Invoke();

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    // Cura a este objeto. Ignora valores <= 0 o si est� muerto.
    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;

        float prev = CurrentHealth;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);

        onHealed?.Invoke();
        onHealedAmount?.Invoke(CurrentHealth - prev);
        onHealthChanged?.Invoke();
    }

    // Permite pasar valores positivos (cura) o negativos (da�o).
    public void AddHealth(float amount)
    {
        if (amount > 0f) Heal(amount);
        else if (amount < 0f) TakeDamage(-amount);
    }

    // Mata inmediatamente a este objeto (da�o letal).
    public void Kill()
    {
        if (IsDead) return;
        CurrentHealth = 0f;
        onHealthChanged?.Invoke();
        Die();
    }

    // Revive con la vida indicada (o al m�ximo si no se pasa valor).
    public void Revive(float? withHealth = null)
    {
        deathHandled = false;
        IsDead = false;
        CurrentHealth = Mathf.Clamp(withHealth.HasValue ? withHealth.Value : maxHealth, 0f, maxHealth);

        if (animator && !string.IsNullOrEmpty(deathTriggerName))
        {
            animator.ResetTrigger(deathTriggerName);
        }

        // Rehabilitar componentes si fuese necesario (se deja hook por si se extiende)
        SetEnabledAfterDeath(true);

        onHealthChanged?.Invoke();
    }

    // Cambia la vida m�xima. Opcionalmente ajusta la vida actual al nuevo rango.
    public void SetMaxHealth(float newMax, bool clampCurrent = true)
    {
        maxHealth = Mathf.Max(1f, newMax);
        if (clampCurrent)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);
            onHealthChanged?.Invoke();
        }
    }

    private void Die()
    {
        if (deathHandled) return;
        deathHandled = true;
        IsDead = true;

        if (animator && !string.IsNullOrEmpty(deathTriggerName))
        {
            animator.SetTrigger(deathTriggerName);
        }

        onDeath?.Invoke();

        // Deshabilitar l�gica de control/movimiento si fuese necesario (se deja hook por si se extiende)
        SetEnabledAfterDeath(false);

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    // Hook para (des)habilitar otros componentes al morir/revivir.
    private void SetEnabledAfterDeath(bool enabled)
    {
        // Por defecto no deshabilitamos nada autom�ticamente para evitar romper otras l�gicas.
        // Puedes extender este m�todo o enlazar onDeath/onHealthChanged desde el Inspector.
    }
}

// Interfaz �til si quieres hacer ataques gen�ricos que apliquen da�o a cualquier objeto con vida.
public interface IDamageable
{
    void TakeDamage(float amount);
    bool IsDead { get; }
}
