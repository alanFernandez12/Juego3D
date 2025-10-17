using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    public UnityEvent prueba;
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

        if(Input.GetKey(KeyCode.Q))
        {
            Defend();
        }
        else
        {
            animator.SetBool("defend", false);
           
        }
       if (Input.GetKeyDown(KeyCode.R))
        {
            prueba?.Invoke();
        }
    }

    void FixedUpdate()
    {

        movement();

    }
    void OnLeftClick()
    {
        // Reproducir animación de ataque
        animator.SetBool("attack1", true);
        
    }

    void Defend()
    {
        animator.SetBool("defend", true);
    }
    void movement()
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



        if (animator.GetBool("defend"))
        {
            movementithDefense();

        }
        else
        {
            if (movement != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


            }
            animator.SetBool("movefront", verticalInput != 0 || horizontalInput != 0);
        }


    }

    void movementithDefense()
    {

        animator.SetBool("movergt", horizontalInput > 0);
        animator.SetBool("movelft", horizontalInput < 0);
        animator.SetBool("movefront", verticalInput != 0);

    }


}
