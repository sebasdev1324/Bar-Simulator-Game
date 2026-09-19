using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public Camera playerCamera;
    public Transform playerHand;
    public float interactionRange = 3.5f;

    [Header("Economía y Precios")]
    public int valorCerveza = 10; // Lo que paga el cliente

    private BeerItem heldBeer;
    private bool holdingBeer = false;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
    }

    void Update()
    {
        // 1. ACCIÓN CON LA TECLA 'E': Agarrar o colocar la cerveza en superficies
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            bool hitSomething = Physics.Raycast(ray, out hit, interactionRange);

            if (!holdingBeer)
            {
                // Si no llevamos nada, intentamos agarrar la botella apuntada
                if (hitSomething)
                {
                    BeerItem beer = hit.collider.GetComponent<BeerItem>();
                    if (beer != null && playerHand != null)
                    {
                        beer.PickUp(playerHand);
                        heldBeer = beer;
                        holdingBeer = true;
                        Debug.Log("Cerveza agarrada con la tecla E.");
                    }
                }
            }
            else
            {
                // Si ya la tenemos, la soltamos/colocamos en la mesa o piso donde apuntemos
                if (heldBeer != null)
                {
                    Vector3 placePos;
                    Quaternion placeRot = Quaternion.identity;

                    if (hitSomething)
                    {
                        placePos = hit.point + hit.normal * 0.05f;
                        Vector3 flatForward = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up);
                        placeRot = flatForward != Vector3.zero ? Quaternion.LookRotation(flatForward) : Quaternion.identity;
                    }
                    else
                    {
                        placePos = playerCamera.transform.position + playerCamera.transform.forward * 1.5f;
                    }

                    heldBeer.PlaceDown(placePos, placeRot);
                    Debug.Log("Cerveza colocada en la superficie.");
                    heldBeer = null;
                    holdingBeer = false;
                }
            }
        }

        // 2. ACCIÓN CON CLIC IZQUIERDO: Entregar la botella al cliente y cobrar mediante GameManager
        if (holdingBeer && Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionRange))
            {
                ClienteController cliente = hit.collider.GetComponent<ClienteController>();
                if (cliente != null && cliente.isWaitingForBeer)
                {
                    // El cliente recibe la cerveza
                    cliente.ReceiveBeer();

                    // Sumar el dinero a través del GameManager global
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.GanarDinero(valorCerveza);
                    }
                    else
                    {
                        Debug.LogWarning("No se encontró el GameManager en la escena para sumar el dinero.");
                    }

                    // Destruimos la botella de la mano
                    if (heldBeer != null)
                    {
                        Destroy(heldBeer.gameObject);
                    }

                    heldBeer = null;
                    holdingBeer = false;
                }
            }
        }

        // 3. ACCIÓN DE COMPRA EN TIENDA (Presionando la tecla 'F')
        if (Input.GetKeyDown(KeyCode.F))
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionRange))
            {
                TiendaItem tiendaItem = hit.collider.GetComponent<TiendaItem>();
                if (tiendaItem != null)
                {
                    tiendaItem.Comprar();
                }
            }
        }
    }
}