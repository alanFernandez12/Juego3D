using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Attack2 : MonoBehaviour
{
    private Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hit()
    {
        if(enemy != null)
        {
            enemy.HitDamage();
            Debug.Log("Enemy hit by sword, player takes damage.");
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
      if (collision.gameObject.CompareTag("enemy"))
        {
           enemy = collision.gameObject.GetComponent<Enemy>();
           
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            enemy = null;

        }
    }
}
