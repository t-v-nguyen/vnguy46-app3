using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool isDragged = false;
    private Vector3 mouseStartPos;
    private Vector3 spriteStartPos;
    private Unit unit;

    private void Awake() {
        unit = GetComponent<Unit>();
    }

    public void OnMouseDown()
    {
        isDragged = true;
        mouseStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spriteStartPos = transform.localPosition;
    }

    public void OnMouseDrag()
    {
        if(isDragged && GameManager.Instance.isRoundPrep)
        {
            transform.localPosition = spriteStartPos + (Camera.main.ScreenToWorldPoint(Input.mousePosition) - mouseStartPos);
        }
    }

    public void OnMouseUp()
    {
        isDragged = false;
        if(Pathfinding.Instance.GetNode(transform.localPosition).isOccupied || Pathfinding.Instance.GetNode(transform.localPosition).y >= 4 )
        {
            transform.localPosition = spriteStartPos;
        }
        else
        {
            transform.localPosition = Pathfinding.Instance.GetWorldPosition(transform.localPosition);
            Pathfinding.Instance.GetNode(transform.localPosition).isOccupied = true;
            unit.currentNode = Pathfinding.Instance.GetNode(transform.localPosition);
            Pathfinding.Instance.GetNode(spriteStartPos).isOccupied = false;
        }
    }
}
