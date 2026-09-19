using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiendaItem : MonoBehaviour
{
    [Header("Configuración del Artículo")]
    public string nombreItem = "Mesa de Bar";
    public int precio = 50;

    [Header("Tipo de Artículo")]
    public bool esMuebleFisico = true; // True si spawnea un objeto en el mundo, False si añade stock al inventario
    public GameObject prefabObjeto;   // El Prefab del mueble o de la caja de cerveza que aparecerá

    [Header("Punto de Spawn")]
    public Transform puntoDeSpawn; // Dónde aparece el mueble o producto al comprarlo

    // Método que ejecuta la compra
    public void Comprar()
    {
        if (GameManager.Instance != null)
        {
            // Intentamos gastar el dinero usando el método que ya creamos en el GameManager
            bool compraExitosa = GameManager.Instance.GastarDinero(precio);

            if (compraExitosa)
            {
                Debug.Log($"¡Has comprado {nombreItem} con éxito!");

                if (esMuebleFisico && prefabObjeto != null)
                {
                    // Determinar dónde colocar el objeto comprado
                    Vector3 spawnPos = puntoDeSpawn != null ? puntoDeSpawn.position : transform.position + transform.forward * 2f;
                    Quaternion spawnRot = puntoDeSpawn != null ? puntoDeSpawn.rotation : Quaternion.identity;

                    // Instanciar el mueble o producto en el mundo real del bar
                    Instantiate(prefabObjeto, spawnPos, spawnRot);
                }
                else
                {
                    // Lógica alternativa si es una recarga de stock (ej. cajas de cerveza para reabastecer la barra)
                    Debug.Log("Stock de productos abastecido en el almacén.");
                }
            }
        }
    }
}