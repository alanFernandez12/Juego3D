using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Referencia al Animator para controlar las animaciones del enemigo
    private Animator animator;
    // Hash (entero) para el trigger de golpe recibido (optimiza el acceso frente a usar string cada vez)
    private int hitHash;
    // Hash para el trigger de ataque
    private int attackHash;
    // Referencia al objeto espada que está actualmente dentro del trigger del enemigo (si lo está)
    private GameObject colisionSword;
    // Referencia al jugador mientras esté dentro del trigger del enemigo (ej. collider de rango)
    private GameObject colisionPlayer;

    // Tiempo mínimo entre registrar golpes recibidos para evitar múltiples registros por un solo impacto
    [SerializeField] private float hitCooldown = 0.05f;
    // Marca de tiempo del último golpe recibido
    private float lastHitTime;

    // Componente NavMeshAgent que gestiona la navegación en el NavMesh
    private NavMeshAgent agent;

    [Header("Jugador")]
    // Transform del jugador (puede asignarse manualmente o se autocompleta por Tag)
    [SerializeField] private Transform player;          // Se autoasigna si está vacío
    // Tag usado para buscar el jugador si player es null
    [SerializeField] private string playerTag = "Player";

    [Header("Rangos / Combate")]
    // Distancia a la que el enemigo empieza a perseguir al jugador
    [SerializeField] private float detectionRange = 12f;
    // Distancia a la que el enemigo puede atacar al jugador
    [SerializeField] private float attackRange = 0.1f;
    // Tiempo mínimo entre ataques para controlar la cadencia
    [SerializeField] private float timeBetweenAttacks = 1.2f;
    // Marca de tiempo del último ataque ejecutado
    private float lastAttackTime;

    private void Awake()
    {
        // En Awake intentamos asignar el jugador por Tag si no se arrastró en el Inspector
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform; // Asignamos si lo encontramos
        }
    }

    void Start()
    {
        // Cachear componentes necesarios al iniciar
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // Convertir nombres de triggers a hash para mejorar rendimiento
        hitHash = Animator.StringToHash("hit");
        attackHash = Animator.StringToHash("attack");

        // Aviso si no se pudo asignar el jugador (se seguirá intentando en Update)
        if (player == null)
            Debug.LogWarning($"[Enemy:{name}] Player no asignado. Se intentará localizar por tag '{playerTag}'.");
    }

    void Update()
    {
        // Reintento periódico (cada ~30 frames) para encontrar al jugador si aún es null (útil si se instancia más tarde)
        if (player == null && Time.frameCount % 30 == 0)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
        }

        // Si todavía no hay jugador, se desactiva la animación de movimiento y se aborta el frame
        if (player == null)
        {
            animator.SetBool("move", false);
            return;
        }

        // Si el agente no existe o no está sobre un NavMesh válido, no continuamos
        if (agent == null || !agent.isOnNavMesh)
            return;

        // Distancia actual al jugador
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            // Perseguir: habilitar movimiento y establecer destino hacia el jugador
            agent.isStopped = false;
            agent.SetDestination(player.position);
            // Ajustar bool de movimiento según si realmente se está desplazando (velocidad > umbral)
            animator.SetBool("move", agent.velocity.sqrMagnitude > 0.01f);

            // Comprobar si ya está dentro del rango de ataque
            if (dist <= attackRange)
            {
                // Detener el path para evitar microajustes cuando está atacando
                if (agent.hasPath) agent.ResetPath();
                agent.isStopped = true;

                // Calcular dirección hacia el jugador ignorando el eje vertical para rotación suave
                Vector3 dir = player.position - transform.position;
                dir.y = 0f; // Evita inclinaciones indeseadas
                if (dir.sqrMagnitude > 0.0001f)
                {
                    // Rotación interpolada para no girar instantáneamente (10f es la velocidad de giro)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
                }

                // Comprobar cooldown de ataque y que el jugador esté dentro del trigger de colisión
                if (Time.time - lastAttackTime >= timeBetweenAttacks)
                {
                    if (colisionPlayer != null)
                    {
                        //Attack();        // Ejecutar ataque (anima y aplica daño)
                        // Lanzar trigger de animación de ataque
                        animator.SetTrigger(attackHash);
                        lastAttackTime = Time.time; // Actualizar momento del último ataque
                    }
                }
            }
        }
        else
        {
            // Fuera del rango de detección: detener movimiento y poner animación de idle
            animator.SetBool("move", false);
            if (agent.hasPath) agent.ResetPath();
            agent.isStopped = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Registrar que la espada del jugador ha entrado en el trigger (para validar daño recibido)
        if (other.CompareTag("sword"))
            colisionSword = other.gameObject;
   
    }

    private void OnTriggerExit(Collider other)
    {
        // Limpiar referencia cuando la espada sale del trigger
        if (other.CompareTag("sword"))
            colisionSword = null;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag(playerTag))
        {
           colisionPlayer = collision.gameObject;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            colisionPlayer = null;
        }
    }

    public void HitDamage()
    {
        // Si la espada no está en contacto no procesamos daño
        if (colisionSword == null) return;
        // Evitar múltiples daños muy seguidos (cooldown)
        if (Time.time - lastHitTime < hitCooldown) return;

        // Actualizar marca de tiempo del último daño recibido
        lastHitTime = Time.time;
        // Lanzar trigger de animación de recibir golpe
        animator.SetTrigger(hitHash);

        // Obtener componente de vida y aplicar daño (si existe)
        var hs = GetComponent<HealthSystem>();
        if (hs != null)
            hs.TakeDamage(30);

        Debug.Log("Enemy hit by sword, player takes damage.");
    }

    void Attack()
    {
        Debug.Log("Enemy attacks player.");

        // Verificar que el jugador sigue en el trigger de ataque
        if (colisionPlayer != null)
        {
            Debug.Log("Enemy attack hits player.");
            var hs = colisionPlayer.GetComponent<HealthSystem>();
            if (hs != null)
                hs.TakeDamage(20); // Aplicar daño al jugador
            Debug.Log("Enemy attacks player, player takes damage.");
        }
    }
}





