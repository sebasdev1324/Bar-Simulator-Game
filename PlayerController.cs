using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;

    [Header("Movimiento y Velocidad")]
    public float walkSpeed = 5.0f;
    public float runSpeed = 8.5f;
    public float gravity = -20f;
    public float jumpHeight = 1.2f;

    [Header("Configuración de Agachado")]
    public float crouchSpeed = 2.5f;
    public float standingHeight = 2.0f;
    public float crouchHeight = 1.0f;
    public float transitionSpeed = 12f;
    private bool isCrouching = false;

    [Header("Cámara y Vista")]
    public Transform playerCamera;
    public float mouseSensitivity = 2.0f;
    private float xRotation = 0f;
    private Vector3 velocity;
    private float defaultCameraY = 0.6f;

    [Header("Interacción y Tienda")]
    public Transform playerHand;
    public float interactionRange = 3.5f;
    public int valorCerveza = 10;

    private BeerItem heldBeer;
    private bool holdingBeer = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null && Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }

        if (playerCamera != null)
        {
            defaultCameraY = playerCamera.localPosition.y;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. GESTIÓN DE CÁMARA Y ROTACIÓN
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        transform.Rotate(Vector3.up * mouseX);

        // 2. MECÁNICA DE AGACHARSE (Ctrl o C)
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
        }

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * transitionSpeed);

        if (playerCamera != null)
        {
            Vector3 camPos = playerCamera.localPosition;
            float targetCamY = isCrouching ? 0.2f : defaultCameraY;
            camPos.y = Mathf.Lerp(camPos.y, targetCamY, Time.deltaTime * transitionSpeed);
            playerCamera.localPosition = camPos;
        }

        // 3. VELOCIDAD (Correr con Shift)
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching;
        float currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);

        // 4. MOVIMIENTO (WASD)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 5. SUELO, GRAVEDAD Y SALTO
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (isGrounded && Input.GetButtonDown("Jump") && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ==========================================
        // 6. SISTEMA DE INTERACCIÓN (TECLAS E, CLIC IZQUIERDO Y F)
        // ==========================================

        // Tecla E: Recoger objeto y guardarlo en la Hotbar
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionRange))
            {
                BeerItem beer = hit.collider.GetComponent<BeerItem>();
                if (beer != null)
                {
                    InventarioHotbar hotbar = GetComponent<InventarioHotbar>();
                    if (hotbar != null)
                    {
                        // Intentamos guardarlo en la barra inferior (máximo 10 por ranura)
                        bool guardado = hotbar.AgregarObjeto("Cerveza", 1);
                        if (guardado)
                        {
                            Destroy(beer.gameObject); // Destruye el objeto del suelo al meterlo al inventario
                        }
                    }
                }
            }
        }

        // Clic Izquierdo: Entregar botella al cliente y cobrar
        if (holdingBeer && Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionRange))
            {
                ClienteController cliente = hit.collider.GetComponent<ClienteController>();
                if (cliente != null && cliente.isWaitingForBeer)
                {
                    cliente.ReceiveBeer();

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.GanarDinero(valorCerveza);
                    }

                    if (heldBeer != null)
                    {
                        Destroy(heldBeer.gameObject);
                    }

                    heldBeer = null;
                    holdingBeer = false;
                }
            }
        }

        // Tecla F: Comprar objetos en la tienda (computadora/mostrador)
        if (Input.GetKeyDown(KeyCode.F))
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
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