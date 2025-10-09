using System.Collections; // Importa el espacio de nombres para colecciones (no se usa aqu�).
using System.Collections.Generic; // Importa el espacio de nombres para colecciones gen�ricas (no se usa aqu�).
using UnityEngine; // Importa las clases principales de Unity.
using Cinemachine; // Importa Cinemachine (no se usa directamente en este script).

public class CameraTargetRotation : MonoBehaviour // Define la clase que controla la rotaci�n del target de la c�mara.
{
    [SerializeField] private float mouseSensitivity = 2f; // Sensibilidad del movimiento del rat�n para la rotaci�n.
    private bool isRotating = false; // Indica si se est� rotando (si el bot�n derecho est� presionado).

    void Update() // M�todo llamado una vez por frame.
    {
        if (Input.GetMouseButtonDown(1)) // Si se presiona el bot�n derecho del rat�n...
            isRotating = true; // ...activar la rotaci�n.

        if (Input.GetMouseButtonUp(1)) // Si se suelta el bot�n derecho del rat�n...
            isRotating = false; // ...desactivar la rotaci�n.

        if (isRotating) // Si la rotaci�n est� activa...
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity; // Captura el movimiento horizontal del rat�n y lo multiplica por la sensibilidad.
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity; // Captura el movimiento vertical del rat�n y lo multiplica por la sensibilidad.

            // Rotaci�n horizontal alrededor del eje Y global (izquierda/derecha).
            transform.Rotate(Vector3.up, mouseX, Space.World);

            // Rotaci�n vertical alrededor del eje X local (arriba/abajo).
            transform.Rotate(Vector3.right, -mouseY, Space.Self);
        }
    }
}
