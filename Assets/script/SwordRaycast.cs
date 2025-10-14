using UnityEngine;

// Adjunta este script al GameObject de la espada.
// Dibuja un raycast desde la espada y detecta enemigos u otros colliders delante.
public class SwordRaycast : MonoBehaviour
{
    [Header("Ray Settings")]
    [SerializeField] private float rayLength = 1.8f;            // Longitud del rayo
    [SerializeField] private LayerMask detectionLayers = ~0;     // Capas a detectar (configurable en el Inspector)
    [SerializeField] private Transform rayOrigin;                // Origen opcional (por ejemplo, la punta de la espada)
    [SerializeField] private Vector3 localOffset;                // Offset local desde el origen

    [Header("Debug Visualization")]
    [SerializeField] private Color rayColor = Color.red;         // Color cuando no hay impacto
    [SerializeField] private Color hitColor = Color.green;       // Color cuando hay impacto
    [SerializeField] private bool drawGizmos = true;             // Mostrar gizmos en el editor

    private RaycastHit lastHit;
    private bool hasHit;

    void Update()
    {
        // Calcular origen y dirección del rayo
        Vector3 origin = GetOriginWorldPosition();
        Vector3 direction = GetForwardDirection();

        // Lanzar raycast
        hasHit = Physics.Raycast(origin, direction, out lastHit, rayLength, detectionLayers, QueryTriggerInteraction.Ignore);

        // Dibujar raycast en modo Play
        Debug.DrawRay(origin, direction * rayLength, hasHit ? hitColor : rayColor);
    }

    // Devuelve la posición mundo del origen del rayo (con offset)
    private Vector3 GetOriginWorldPosition()
    {
        Transform t = rayOrigin != null ? rayOrigin : transform;
        return t.position + t.TransformVector(localOffset);
    }

    // Obtiene la dirección hacia delante a partir del origen elegido
    private Vector3 GetForwardDirection()
    {
        return (rayOrigin != null ? rayOrigin.forward : transform.forward);
    }

    // Permite a otros scripts consultar si hubo impacto y obtener el RaycastHit
    public bool TryGetHit(out RaycastHit hit)
    {
        hit = lastHit;
        return hasHit;
    }

    // Visualización en el editor cuando está seleccionado
    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Vector3 origin = (rayOrigin != null ? rayOrigin.position + rayOrigin.TransformVector(localOffset) : transform.position + transform.TransformVector(localOffset));
        Vector3 direction = (rayOrigin != null ? rayOrigin.forward : transform.forward);

        Gizmos.color = hasHit ? hitColor : rayColor;
        Gizmos.DrawLine(origin, origin + direction * rayLength);

        if (hasHit)
        {
            Gizmos.color = hitColor;
            Gizmos.DrawSphere(lastHit.point, 0.05f);
        }
    }
}
