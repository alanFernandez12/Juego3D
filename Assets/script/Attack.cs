using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Raycast Source")]
    [Tooltip("Asigna aquí el componente SwordRaycast de la espada (o deja vacío y haré un fallback por reflexión si existe).")]
    [SerializeField] private Component swordRaycastComponent; // Espera un SwordRaycast, pero sin referenciar el tipo directamente

    [Header("Fallback Ray (si no hay SwordRaycast)")]
    [SerializeField] private Transform rayOrigin; // Origen opcional
    [SerializeField] private Vector3 localOffset; // Offset local
    [SerializeField] private float rayLength = 1.8f;
    [SerializeField] private LayerMask detectionLayers = ~0;

    [Header("Damage")]
    [SerializeField] private int defaultDamage = 30; // Daño si no hay Enemy pero sí HealthSystem

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Método llamado desde un evento de animación
    public void hit()
    {
        // 1) Intentar usar SwordRaycast si está asignado (vía reflexión para evitar dependencia directa)
        if (TryHitViaSwordRaycast(out RaycastHit hitInfo))
        {
            ApplyDamage(hitInfo);
            return;
        }

       
    }

    private bool TryHitViaSwordRaycast(out RaycastHit hitInfo)
    {
        hitInfo = default;
        if (swordRaycastComponent == null) return false;

        MethodInfo tryGetHit = swordRaycastComponent.GetType().GetMethod("TryGetHit", BindingFlags.Public | BindingFlags.Instance);
        if (tryGetHit == null) return false;

        object[] args = new object[] { null };
        bool gotHit = false;
        try
        {
            gotHit = (bool)tryGetHit.Invoke(swordRaycastComponent, args);
        }
        catch
        {
            return false;
        }

        if (gotHit && args[0] is RaycastHit rh)
        {
            hitInfo = rh;
            return true;
        }
        return false;
    }

    private Vector3 GetOriginWorldPosition()
    {
        Transform t = rayOrigin != null ? rayOrigin : transform;
        return t.position + t.TransformVector(localOffset);
    }

    private void ApplyDamage(RaycastHit hitInfo)
    {
        // Intentar dañar un Enemy específico
        var enemy = hitInfo.collider.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.HitDamage();
            return;
        }

     
    }
}
