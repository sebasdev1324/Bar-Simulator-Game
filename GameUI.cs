using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Para controlar los textos de los paneles

public class GameUI : MonoBehaviour
{
    public static GameUI instancia;

    [Header("Panel de Estado (Abajo Derecha)")]
    public TextMeshProUGUI textoDinero;
    public TextMeshProUGUI textoHora;

    [Header("Panel de Misiones (Arriba)")]
    public TextMeshProUGUI textoMisiones;

    [Header("Inventario (Abajo Izquierda)")]
    public TextMeshProUGUI textoItemMano; // Un texto temporal para saber qué slot está activo

    // Variables internas del juego
    private float dineroActual = 0f;
    private int hora = 8;
    private int minutos = 0;
    private float contadorTiempo = 0f;

    void Awake()
    {
        if (instancia == null) instancia = this;
    }

    void Start()
    {
        // Valores iniciales al arrancar el bar
        ActualizarDineroVisual();
        textoMisiones.text = "Misión: Espera a que llegue un cliente a la barra.";
        if (textoItemMano != null) textoItemMano.text = "Mano: Vacía";
    }

    void Update()
    {
        // Simular el paso del tiempo para el reloj del juego
        contadorTiempo += Time.deltaTime;
        if (contadorTiempo >= 2f) // Cada 2 segundos de la vida real pasa 1 minuto en el juego
        {
            minutos++;
            if (minutos >= 60)
            {
                minutos = 0;
                hora++;
                if (hora >= 24) hora = 0;
            }
            contadorTiempo = 0f;
            ActualizarRelojVisual();
        }
    }

    // FUNCIÓN PARA SUMAR PLATA (La llamaremos cuando entregues el pedido)
    public void AñadirDinero(float cantidad)
    {
        dineroActual += cantidad;
        ActualizarDineroVisual();
    }

    // FUNCIÓN PARA CAMBIAR LAS MISIONES EN PANTALLA
    public void CambiarMision(string nuevaMision)
    {
        textoMisiones.text = "Misión: " + nuevaMision;
    }

    // FUNCIÓN PARA EL INVENTARIO VISIBLE
    public void ActualizarObjetoEnMano(string nombreObjeto)
    {
        if (textoItemMano != null) textoItemMano.text = "Cargando: " + nombreObjeto;
    }

    void ActualizarDineroVisual()
    {
        textoDinero.text = "$ " + dineroActual.ToString("F2");
    }

    void ActualizarRelojVisual()
    {
        // Esto hace que se vea bonito como un reloj digital (ej: 08:05)
        textoHora.text = hora.ToString("D2") + ":" + minutos.ToString("D2");
    }
}
