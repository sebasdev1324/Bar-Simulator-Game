using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necesario para TextMeshPro

public class PlayerInventory : MonoBehaviour
{
    [Header("Configuración de Inventario")]
    public int capacidadMaxima = 3;
    public List<BeerItem> itemsEnInventario = new List<BeerItem>();

    [Header("Referencia UI")]
    public TextMeshProUGUI textoInventarioUI;

    [Header("Punto de Referencia visual")]
    public Transform playerHand;

    void Start()
    {
        ActualizarInterfazInventario();
    }

    public bool AgregarItem(BeerItem nuevoItem)
    {
        if (itemsEnInventario.Count < capacidadMaxima)
        {
            itemsEnInventario.Add(nuevoItem);

            // Ocultamos el objeto del mundo mientras lo lleva el jugador
            nuevoItem.gameObject.SetActive(false);

            ActualizarInterfazInventario();
            Debug.Log($"[Inventario] Objeto añadido. Total: {itemsEnInventario.Count}/{capacidadMaxima}");
            return true;
        }
        else
        {
            Debug.Log("[Inventario] ¡Inventario lleno!");
            return false;
        }
    }

    public BeerItem UsarObrirItem()
    {
        if (itemsEnInventario.Count > 0)
        {
            BeerItem itemAUsar = itemsEnInventario[0];
            itemsEnInventario.RemoveAt(0);

            itemAUsar.gameObject.SetActive(true);
            ActualizarInterfazInventario();
            return itemAUsar;
        }
        return null;
    }

    private void ActualizarInterfazInventario()
    {
        if (textoInventarioUI != null)
        {
            textoInventarioUI.text = $"Inventario: {itemsEnInventario.Count} / {capacidadMaxima}";
        }
    }
}