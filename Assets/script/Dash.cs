using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    private Vector3 previousPosition; // Almacena la posici�n del objeto en el frame anterior
    public Vector3 currentDirection; // Direcci�n actual del movimiento
   [SerializeField] private int teleportRange = 5;
    void Start()
    {
        // Inicializar la posici�n previa con la posici�n inicial del objeto
        previousPosition = transform.position;
    }
    void Update()
    {
        CalculateDirection();
        
        Teleport();
    }
    void Teleport()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Dash");
            transform.position += currentDirection * teleportRange;
            Debug.Log(currentDirection);
        }
    }
    void CalculateDirection()
    {
        // Calcular la direcci�n del movimiento
        Vector3 movement = transform.position - previousPosition;
        // Normalizar la direcci�n para obtener un vector unitario
        // Lo adaptamos a un nuevo vector3 para que no tome
        movement = new Vector3(movement.x, 0, movement.z);
        if (movement != Vector3.zero)
        {
            currentDirection = movement.normalized;
        }
        // Actualizar la posici�n previa
        previousPosition = transform.position;
    }
}
