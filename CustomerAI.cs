using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAI : MonoBehaviour
{
    public Transform puntoDeDestino;
    public float velocidad = 3.0f;

    // Lista de tragos que el cliente puede pedir
    private string[] menuDeTragos = { "Cerveza", "Aguardiente", "Ron con Cola", "Juguito" };
    private bool yaHizoElPedido = false;

    void Update()
    {
        if (puntoDeDestino == null) return;

        Vector3 direccion = puntoDeDestino.position - transform.position;
        direccion.y = 0;

        // Si todavía está lejos de la barra, camina
        if (direccion.magnitude > 0.2f)
        {
            transform.Translate(direccion.normalized * velocidad * Time.deltaTime, Space.World);
            transform.forward = direccion.normalized;
        }
        else
        {
            // SI YA LLEGÓ A LA BARRA:
            if (!yaHizoElPedido)
            {
                HacerPedido();
            }
        }
    }

    void HacerPedido()
    {
        yaHizoElPedido = true; // Esto evita que pida mil veces por segundo

        // Elige un trago al azar de la lista
        int indiceAlzar = Random.Range(0, menuDeTragos.Length);
        string tragoElegido = menuDeTragos[indiceAlzar];

        // Muestra el pedido en la pantalla de Unity (Consola)
        Debug.Log("?? [CLIENTE]: ¡Hola! Por favor me da un(a) " + tragoElegido + ".");
    }
}