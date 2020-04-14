using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : MonoBehaviour
{

    public float speed = 3.5f; // Velocidad fija
    Vector3 direction = Vector3.forward; // Variable vectorial que avanza una unidad en z
    public Transform targetPosition; // Posición del target
    static Transform targetPos;
    
    
    // Start is called before the first frame update
    void Start()
    {
        try{
            targetPos = targetPosition.GetComponentInChildren<Transform>();
        }
        catch (System.Exception)
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            Debug.Log(targetPos.transform.position);
            var Vel = GetVelocity(targetPos.position); // llamamos a nuestra función de velocidad

            transform.position += Vel * Time.deltaTime; // Actualizamos la posición de la IA en cada tick
        }
        catch (System.Exception)
        {
            return;
        }
        

    }

    Vector3 GetVelocity(Vector3 targetPosition)
    {
        // Obtenemos la dirección que vamos a seguir calculado un vector del punto target a nosotros
        Vector3 dir = (targetPosition - transform.position).normalized;
        dir.y = 0; // No se mueve en el eje vertical

        return dir * speed; // devolvemos un FLOAT numérico
    }

    Vector3 GetOrientation(Vector3 targetPosition)
    {
        // devolvemos un VECTOR en la dirección que esta nuestro objetivo
        return (targetPosition - transform.position).normalized;
    }
}
