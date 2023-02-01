using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNDropController : MonoBehaviour
{
    // modifiable values
    public float gridSize = 1.0f;
    public bool snapToGrid = true;
    public bool smartDrag = false;
    public bool isDraggable = true;

    // internal values
    private bool isDragged = false;
    private Camera _cam;
    private Vector2 initialPositionMouse;
    private Vector2 initialPositionObject;

    private void Awake()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        if (isDragged)
        {
            if (!smartDrag)
                transform.position = (Vector2)_cam.ScreenToWorldPoint(Input.mousePosition);
            else
                transform.position = initialPositionObject + (Vector2)_cam.ScreenToWorldPoint(Input.mousePosition) - initialPositionMouse;
            
            if (snapToGrid)
                transform.position = new Vector2(
                    Mathf.RoundToInt(transform.position.x/gridSize) * gridSize, 
                    Mathf.RoundToInt(transform.position.y / gridSize) * gridSize
                    );
        }
    }

    private void OnMouseOver()
    {
        if (isDraggable && Input.GetMouseButtonDown(0))
        {
            if (smartDrag)
            {
                initialPositionMouse = (Vector2)_cam.ScreenToWorldPoint(Input.mousePosition);
                initialPositionObject = transform.position;
            }

            isDragged = true;
        }
    }
    private void OnMouseUp()
    {
        isDragged = false;
    }
}
