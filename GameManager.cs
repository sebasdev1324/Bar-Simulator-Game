using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Obligatorio para usar textos de TextMeshPro

public class GameManager : MonoBehaviour
{
    // Patrón Singleton para acceder al GameManager desde cualquier otro script fácilmente
    public static GameManager Instance;

    [Header("Finanzas")]
    public int dineroActual = 0;

    [Header("Referencia UI")]
    public TextMeshProUGUI textoDineroUI;

    void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ActualizarInterfaz();
    }

    // Método para sumar dinero (cuando te pagan una cerveza)
    public void GanarDinero(int cantidad)
    {
        dineroActual += cantidad;
        ActualizarInterfaz();
        Debug.Log($"[Economía] Ganaste ${cantidad}. Total en caja: ${dineroActual}");
    }

    // Método para restar dinero (cuando compras mejoras, objetos, etc.)
    public bool GastarDinero(int cantidad)
    {
        if (dineroActual >= cantidad)
        {
            dineroActual -= cantidad;
            ActualizarInterfaz();
            Debug.Log($"[Economía] Gastaste ${cantidad}. Total restante: ${dineroActual}");
            return true; // Compra exitosa
        }
        else
        {
            Debug.Log("[Economía] No tienes suficiente dinero.");
            return false; // No hay fondos
        }
    }

    private void ActualizarInterfaz()
    {
        if (textoDineroUI != null)
        {
            textoDineroUI.text = $"Dinero: ${dineroActual}";
        }
    }
}