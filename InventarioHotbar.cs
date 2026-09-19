using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ItemDefinition
{
    public string nombreItem;
    public Sprite icono2D;
    public GameObject prefab3DMano;
    public GameObject prefab3DSuelo; // Necesario para soltar al mundo físico
}

[System.Serializable]
public class SlotInventario
{
    public string nombreItem;
    public int cantidad;
    public int capacidadMaximaPila = 10;

    [Header("Referencias Visuales UI")]
    public Image fondoSlotPanel;      // Panel o fondo del recuadro para iluminar el activo
    public Image iconoUI;
    public TextMeshProUGUI textoContadorUI;
}

public class InventarioHotbar : MonoBehaviour
{
    [Header("Catálogo de Ítems")]
    public List<ItemDefinition> catalogoItems = new List<ItemDefinition>();

    [Header("Configuración Hotbar (1-10)")]
    public List<SlotInventario> slots = new List<SlotInventario>(10);
    public int slotSeleccionadoIndex = 0;

    [Header("Estilos de Selección UI")]
    public Color colorSlotActivo = new Color(1f, 0.84f, 0f, 0.85f); // Dorado/Amarillo brillante
    public Color colorSlotInactivo = new Color(0.2f, 0.2f, 0.2f, 0.6f); // Gris oscuro translúcido

    [Header("Mano del Jugador")]
    public Transform playerHand;
    private GameObject instanciaActualEnMano;

    void Start()
    {
        ActualizarUI();
        ActualizarVisualMano();
    }

    void Update()
    {
        int prevSlot = slotSeleccionadoIndex;

        // Teclas 1 al 9 y 0 para el 10
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) slotSeleccionadoIndex = i;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) slotSeleccionadoIndex = 9;

        // Rueda del ratón
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) slotSeleccionadoIndex = (slotSeleccionadoIndex - 1 + slots.Count) % slots.Count;
        else if (scroll < 0f) slotSeleccionadoIndex = (slotSeleccionadoIndex + 1) % slots.Count;

        if (prevSlot != slotSeleccionadoIndex)
        {
            ActualizarUI();
            ActualizarVisualMano();
        }
    }

    public bool AgregarObjeto(string nombreItemNuevo, int cantidadAAnadir)
    {
        if (string.IsNullOrEmpty(nombreItemNuevo)) return false;
        nombreItemNuevo = nombreItemNuevo.Trim();

        // 1. Agrupar si ya existe con espacio
        foreach (var slot in slots)
        {
            if (!string.IsNullOrEmpty(slot.nombreItem) && slot.nombreItem.Equals(nombreItemNuevo, System.StringComparison.OrdinalIgnoreCase))
            {
                if (slot.cantidad < slot.capacidadMaximaPila)
                {
                    int espacioDisponible = slot.capacidadMaximaPila - slot.cantidad;
                    int aColocar = Mathf.Min(cantidadAAnadir, espacioDisponible);
                    slot.cantidad += aColocar;
                    ActualizarUI();
                    ActualizarVisualMano();
                    return true;
                }
            }
        }

        // 2. Buscar slot vacío
        foreach (var slot in slots)
        {
            if (string.IsNullOrEmpty(slot.nombreItem) || slot.cantidad <= 0)
            {
                slot.nombreItem = nombreItemNuevo;
                slot.cantidad = Mathf.Min(cantidadAAnadir, slot.capacidadMaximaPila);
                ActualizarUI();
                ActualizarVisualMano();
                return true;
            }
        }

        Debug.Log("[Hotbar] ¡Inventario lleno!");
        return false;
    }

    public bool ConsumirItemActivo()
    {
        if (slotSeleccionadoIndex < 0 || slotSeleccionadoIndex >= slots.Count) return false;

        SlotInventario slotActual = slots[slotSeleccionadoIndex];
        if (slotActual.cantidad > 0 && !string.IsNullOrEmpty(slotActual.nombreItem))
        {
            slotActual.cantidad--;
            if (slotActual.cantidad <= 0) slotActual.nombreItem = "";
            ActualizarUI();
            ActualizarVisualMano();
            return true;
        }
        return false;
    }

    public bool SoltarItemActivo(Vector3 posicionDrop)
    {
        if (slotSeleccionadoIndex < 0 || slotSeleccionadoIndex >= slots.Count) return false;
        SlotInventario slotActual = slots[slotSeleccionadoIndex];

        if (slotActual.cantidad > 0 && !string.IsNullOrEmpty(slotActual.nombreItem))
        {
            ItemDefinition def = BuscarDefinicion(slotActual.nombreItem);
            if (def != null && def.prefab3DSuelo != null)
            {
                Instantiate(def.prefab3DSuelo, posicionDrop, Quaternion.identity);
            }
            slotActual.cantidad--;
            if (slotActual.cantidad <= 0) slotActual.nombreItem = "";
            ActualizarUI();
            ActualizarVisualMano();
            return true;
        }
        return false;
    }

    public SlotInventario GetSlotActual()
    {
        if (slotSeleccionadoIndex >= 0 && slotSeleccionadoIndex < slots.Count)
            return slots[slotSeleccionadoIndex];
        return null;
    }

    private ItemDefinition BuscarDefinicion(string nombre)
    {
        return catalogoItems.Find(x => x.nombreItem.Equals(nombre, System.StringComparison.OrdinalIgnoreCase));
    }

    private void ActualizarUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            SlotInventario slot = slots[i];
            bool esActivo = (i == slotSeleccionadoIndex);
            ItemDefinition def = BuscarDefinicion(slot.nombreItem);

            // Reflejar resaltado del panel activo/inactivo
            if (slot.fondoSlotPanel != null)
            {
                slot.fondoSlotPanel.color = esActivo ? colorSlotActivo : colorSlotInactivo;
            }

            // Icono 2D
            if (slot.iconoUI != null)
            {
                if (!string.IsNullOrEmpty(slot.nombreItem) && slot.cantidad > 0 && def != null && def.icono2D != null)
                {
                    slot.iconoUI.sprite = def.icono2D;
                    slot.iconoUI.enabled = true;
                    slot.iconoUI.color = Color.white;
                }
                else
                {
                    slot.iconoUI.sprite = null;
                    slot.iconoUI.enabled = false;
                }
            }

            // Texto cantidad
            if (slot.textoContadorUI != null)
            {
                slot.textoContadorUI.color = Color.white;
                if (!string.IsNullOrEmpty(slot.nombreItem) && slot.cantidad > 0)
                {
                    slot.textoContadorUI.text = slot.cantidad > 1 ? $"{slot.cantidad}" : "";
                }
                else
                {
                    slot.textoContadorUI.text = "";
                }
            }
        }
    }

    private void ActualizarVisualMano()
    {
        if (playerHand == null) return;

        if (instanciaActualEnMano != null)
        {
            Destroy(instanciaActualEnMano);
            instanciaActualEnMano = null;
        }

        SlotInventario slotActual = GetSlotActual();
        if (slotActual != null && !string.IsNullOrEmpty(slotActual.nombreItem) && slotActual.cantidad > 0)
        {
            ItemDefinition def = BuscarDefinicion(slotActual.nombreItem);
            if (def != null && def.prefab3DMano != null)
            {
                instanciaActualEnMano = Instantiate(def.prefab3DMano, playerHand);
                instanciaActualEnMano.transform.localPosition = Vector3.zero;
                instanciaActualEnMano.transform.localRotation = Quaternion.identity;
                foreach (Collider col in instanciaActualEnMano.GetComponentsInChildren<Collider>())
                {
                    col.enabled = false;
                }
            }
        }
    }
}