using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    private Vector3 previousPosition; // Almacena la posición del objeto en el frame anterior
    public Vector3 currentDirection; // Dirección actual del movimiento
   [SerializeField] private int teleportRange = 5;
    void Start()
    {
        // Inicializar la posición previa con la posición inicial del objeto
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
        // Calcular la dirección del movimiento
        Vector3 movement = transform.position - previousPosition;
        // Normalizar la dirección para obtener un vector unitario
        // Lo adaptamos a un nuevo vector3 para que no tome
        movement = new Vector3(movement.x, 0, movement.z);
        if (movement != Vector3.zero)
        {
            currentDirection = movement.normalized;
        }
        // Actualizar la posición previa
        previousPosition = transform.position;
    }
}
