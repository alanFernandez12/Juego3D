using System.Collections; // Importa el espacio de nombres para colecciones (no se usa aquí).
using System.Collections.Generic; // Importa el espacio de nombres para colecciones genéricas (no se usa aquí).
using UnityEngine; // Importa las clases principales de Unity.
using Cinemachine; // Importa Cinemachine (no se usa directamente en este script).

public class CameraTargetRotation : MonoBehaviour // Define la clase que controla la rotación del target de la cámara.
{
    [SerializeField] private float mouseSensitivity = 2f; // Sensibilidad del movimiento del ratón para la rotación.
    private bool isRotating = false; // Indica si se está rotando (si el botón derecho está presionado).

    void Update() // Método llamado una vez por frame.
    {
        if (Input.GetMouseButtonDown(1)) // Si se presiona el botón derecho del ratón...
            isRotating = true; // ...activar la rotación.

        if (Input.GetMouseButtonUp(1)) // Si se suelta el botón derecho del ratón...
            isRotating = false; // ...desactivar la rotación.

        if (isRotating) // Si la rotación está activa...
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity; // Captura el movimiento horizontal del ratón y lo multiplica por la sensibilidad.
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity; // Captura el movimiento vertical del ratón y lo multiplica por la sensibilidad.

            // Rotación horizontal alrededor del eje Y global (izquierda/derecha).
            transform.Rotate(Vector3.up, mouseX, Space.World);

            // Rotación vertical alrededor del eje X local (arriba/abajo).
            transform.Rotate(Vector3.right, -mouseY, Space.Self);
        }
    }
}
