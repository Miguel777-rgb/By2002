using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMovimiento : MonoBehaviour
{
    // Declaramos las variables aquí para que sean accesibles en toda la clase
    private float mousePosX;
    private float mousePosY;

    [SerializeField] float movementQuantity;
    // Start is called before the first frame update
    void Start()
    {
        // Puedes inicializarlas aquí si es necesario
    }

    // Update is called once per frame
    void Update()
    {
        mousePosX = Input.mousePosition.x;
        mousePosY = Input.mousePosition.y;

        this.GetComponent<RectTransform>().position = new Vector2(
            (mousePosX / Screen.width) * movementQuantity + (Screen.width / 2),
            (mousePosY / Screen.height) * movementQuantity + (Screen.height / 2));
    }
}
