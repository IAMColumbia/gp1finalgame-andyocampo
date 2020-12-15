using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    public bool leftMouseButtonPressed;
    public Collider2D objectClicked;
    public bool objectHit;

    private void Awake()
    {
        leftMouseButtonPressed = false;
        objectHit = false;
    }

    private void Update()
    {
        LeftMouseButtonPressed();
    }

    /// <summary>
    /// Checks whether the left mouse button has been pressed and whether an object has been hit.
    /// </summary>
    /// <returns>Left mouse button pressed & object hit</returns>
    protected bool LeftMouseButtonPressed()
    {
        leftMouseButtonPressed = false;
        objectHit = false;

        if (Input.GetMouseButtonDown(0))
        {
            leftMouseButtonPressed = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit)
            {
                objectHit = true;
                objectClicked = hit.collider;
                return objectHit;
            }

            return leftMouseButtonPressed;
        }

        return leftMouseButtonPressed;
    }
}
