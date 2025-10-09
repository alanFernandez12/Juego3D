using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;
    public float moveSpeed = 5f; // Velocidad de movimiento del jugador.
    private Rigidbody rb; // Referencia al Rigidbody.
    //private bool isGrounded;
    private float ogSpeed;
    [SerializeField] private float multSpeed;
    [SerializeField] private float valMultSpeed = 2f;
   // [SerializeField] float rayLenght = 3f;
    //public float jumpForce = 2f;
   //private Salto sal;
    private Animator animator;
    public float rotationSpeed = 10f;
    public LayerMask groundLayer = 1; // Layer por defecto

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        ogSpeed = moveSpeed; // Guardar la velocidad original

    }
    void Update()
    {
        /*
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.up, rayLenght);

        Debug.DrawRay(transform.position,Vector3.down * rayLenght, Color.red);
        if (isGrounded)
        {
            Debug.Log("estoy tocando el suelo");
        }
        */        /*
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            
            jump();
            
        }
        else { animator.SetBool("jump", false); }
        */

        if (Input.GetMouseButton(0))
        {
            OnLeftClick();
        }
        else
        {
            animator.SetBool("attack1", false);
        }

        // Sprint: alterna entre velocidad normal y de sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = ogSpeed * valMultSpeed;
            animator.SetBool("sprint", true);
        }
        else
        {
            moveSpeed = ogSpeed;
            animator.SetBool("sprint", false);
        }
    }

    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        /*Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
        movement = movement.normalized;
        */
        // Dirección de la cámara en el plano XZ

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        // Eliminamos la componente en Y para que no influya la inclinación de la cámara
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Movimiento relativo a la cámara
        Vector3 movement = (forward * verticalInput + right * horizontalInput).normalized;

        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);

       
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            
        }

        animator.SetBool("movefront", verticalInput != 0 || horizontalInput != 0);
        
        }

   /*void jump()
    {
        Debug.Log("Salto");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetBool("jump", true);
    }
    void OnDrawGizmosSelected()
    {
        // Dibujar el raycast en el Editor incluso cuando no está en play mode
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayLenght);

        // Dibujar una esfera en el punto de impacto
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLenght, groundLayer))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
    }*/
    void OnLeftClick()
    {
        // Reproducir animación de ataque
        animator.SetBool("attack1", true);
        
    }
}
