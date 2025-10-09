using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salto : MonoBehaviour
{
    [SerializeField] public float jumpForce = 5f; // Fuerza del salto
    [SerializeField] public float rayLength = 1.1f; // Longitud del Raycast
    private Rigidbody rb; // Referencia al Rigidbody del jugador
    private int saltosRestantes;
    [SerializeField] private int saltosMaximos = 2;
    public LayerMask groundLayer = 1;
    private Animator animator;



    public bool isGrounded;


    /* public bool isGrounded
     {// al usar la variable, nos da el valor de _health(get)
         get { return _isGrounded; }
        private set{}
     }*/


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        isGrounded = false;
        // Verificar si el jugador está en el suelo usando un Raycast
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, groundLayer, QueryTriggerInteraction.Ignore);
        if (saltosRestantes <= 0)// Si no tengo saltos
        {

            if (isGrounded) // Si estoy tocando el suelo
            {
                saltosRestantes = saltosMaximos; //recupero mis saltos
            }
        }
        // Permitir el salto si el jugador está en el suelo y presiona la barra espaciadora
        // y tiene saltos restantes

        if (Input.GetKeyDown(KeyCode.Space) && (saltosRestantes > 0 && isGrounded))
        {
            saltosRestantes--;
            Jump();
        }
        else { animator.SetBool("jump", false); }
    }
    void Jump()
    {
        // Aplicar fuerza hacia arriba para saltar
        //rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetBool("jump", true);
        //rb.AddForce(0, jumpForce, 0);
    }
    void OnDrawGizmosSelected()
    {
        // Dibujar el raycast en el Editor incluso cuando no está en play mode
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayLength);

        // Dibujar una esfera en el punto de impacto
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, groundLayer))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
    }
}
