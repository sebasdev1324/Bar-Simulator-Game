using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClienteController : MonoBehaviour
{
    [Header("Destino en la Barra")]
    public Transform counterDestination;
    public float moveSpeed = 3.5f;

    [Header("Estado")]
    public bool isWaitingForBeer = false;
    public bool hasReceivedBeer = false;
    private bool hasArrived = false;

    void Update()
    {
        if (counterDestination != null && !hasArrived)
        {
            // Mover directamente hacia la posición del destino
            transform.position = Vector3.MoveTowards(transform.position, counterDestination.position, moveSpeed * Time.deltaTime);

            // Rotar opcionalmente para que mire hacia el mostrador
            Vector3 direction = (counterDestination.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, moveSpeed * Time.deltaTime * 5f);
            }

            // Comprobar si llegó (distancia menor a 0.2 metros)
            if (Vector3.Distance(transform.position, counterDestination.position) <= 0.2f)
            {
                hasArrived = true;
                ArrivedAtCounter();
            }
        }
    }

    void ArrivedAtCounter()
    {
        isWaitingForBeer = true;
        Debug.Log("Cliente: ¡Buenas! Me pones una cerveza, por favor.");
    }

    public void ReceiveBeer()
    {
        hasReceivedBeer = true;
        isWaitingForBeer = false;
        Debug.Log("Cliente: ¡Excelente servicio, gracias crack!");
    }
}