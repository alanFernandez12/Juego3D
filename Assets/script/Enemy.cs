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
    // Referencia al objeto espada que est� actualmente dentro del trigger del enemigo (si lo est�)
    private GameObject colisionSword;
    // Referencia al jugador mientras est� dentro del trigger del enemigo (ej. collider de rango)
    private GameObject colisionPlayer;

    // Tiempo m�nimo entre registrar golpes recibidos para evitar m�ltiples registros por un solo impacto
    [SerializeField] private float hitCooldown = 0.05f;
    // Marca de tiempo del �ltimo golpe recibido
    private float lastHitTime;

    // Componente NavMeshAgent que gestiona la navegaci�n en el NavMesh
    private NavMeshAgent agent;

    [Header("Jugador")]
    // Transform del jugador (puede asignarse manualmente o se autocompleta por Tag)
    [SerializeField] private Transform player;          // Se autoasigna si est� vac�o
    // Tag usado para buscar el jugador si player es null
    [SerializeField] private string playerTag = "Player";

    [Header("Rangos / Combate")]
    // Distancia a la que el enemigo empieza a perseguir al jugador
    [SerializeField] private float detectionRange = 12f;
    // Distancia a la que el enemigo puede atacar al jugador
    [SerializeField] private float attackRange = 0.1f;
    // Tiempo m�nimo entre ataques para controlar la cadencia
    [SerializeField] private float timeBetweenAttacks = 1.2f;
    // Marca de tiempo del �ltimo ataque ejecutado
    private float lastAttackTime;

    private void Awake()
    {
        // En Awake intentamos asignar el jugador por Tag si no se arrastr� en el Inspector
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

        // Aviso si no se pudo asignar el jugador (se seguir� intentando en Update)
        if (player == null)
            Debug.LogWarning($"[Enemy:{name}] Player no asignado. Se intentar� localizar por tag '{playerTag}'.");
    }

    void Update()
    {
        // Reintento peri�dico (cada ~30 frames) para encontrar al jugador si a�n es null (�til si se instancia m�s tarde)
        if (player == null && Time.frameCount % 30 == 0)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
        }

        // Si todav�a no hay jugador, se desactiva la animaci�n de movimiento y se aborta el frame
        if (player == null)
        {
            animator.SetBool("move", false);
            return;
        }

        // Si el agente no existe o no est� sobre un NavMesh v�lido, no continuamos
        if (agent == null || !agent.isOnNavMesh)
            return;

        // Distancia actual al jugador
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            // Perseguir: habilitar movimiento y establecer destino hacia el jugador
            agent.isStopped = false;
            agent.SetDestination(player.position);
            // Ajustar bool de movimiento seg�n si realmente se est� desplazando (velocidad > umbral)
            animator.SetBool("move", agent.velocity.sqrMagnitude > 0.01f);

            // Comprobar si ya est� dentro del rango de ataque
            if (dist <= attackRange)
            {
                // Detener el path para evitar microajustes cuando est� atacando
                if (agent.hasPath) agent.ResetPath();
                agent.isStopped = true;

                // Calcular direcci�n hacia el jugador ignorando el eje vertical para rotaci�n suave
                Vector3 dir = player.position - transform.position;
                dir.y = 0f; // Evita inclinaciones indeseadas
                if (dir.sqrMagnitude > 0.0001f)
                {
                    // Rotaci�n interpolada para no girar instant�neamente (10f es la velocidad de giro)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
                }

                // Comprobar cooldown de ataque y que el jugador est� dentro del trigger de colisi�n
                if (Time.time - lastAttackTime >= timeBetweenAttacks)
                {
                    if (colisionPlayer != null)
                    {
                        //Attack();        // Ejecutar ataque (anima y aplica da�o)
                        // Lanzar trigger de animaci�n de ataque
                        animator.SetTrigger(attackHash);
                        lastAttackTime = Time.time; // Actualizar momento del �ltimo ataque
                    }
                }
            }
        }
        else
        {
            // Fuera del rango de detecci�n: detener movimiento y poner animaci�n de idle
            animator.SetBool("move", false);
            if (agent.hasPath) agent.ResetPath();
            agent.isStopped = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Registrar que la espada del jugador ha entrado en el trigger (para validar da�o recibido)
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
        // Si la espada no est� en contacto no procesamos da�o
        if (colisionSword == null) return;
        // Evitar m�ltiples da�os muy seguidos (cooldown)
        if (Time.time - lastHitTime < hitCooldown) return;

        // Actualizar marca de tiempo del �ltimo da�o recibido
        lastHitTime = Time.time;
        // Lanzar trigger de animaci�n de recibir golpe
        animator.SetTrigger(hitHash);

        // Obtener componente de vida y aplicar da�o (si existe)
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
                hs.TakeDamage(20); // Aplicar da�o al jugador
            Debug.Log("Enemy attacks player, player takes damage.");
        }
    }
}





