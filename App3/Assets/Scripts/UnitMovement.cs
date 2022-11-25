using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private Unit unit;
    public List<Vector3> pathVectorList;
    private List<Unit> opposingUnits = new List<Unit>();
    private GameManager gameManager;
    public Vector3 destination;

    void Start()
    {
        unit = GetComponent<Unit>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private bool MoveToNode(Vector3 location)
    {
        Vector3 moveDir = (location - transform.position);
        if (moveDir.sqrMagnitude <= 0.05f)
        {
            transform.position = location;
            return true;
        }
        transform.position = transform.position + moveDir.normalized * unit.MS * Time.deltaTime;
        return false;
    }

    public void GetInRange()
    {
        if (unit.target == null)
        {
            return;
        }

        if (!unit.isMoving)
        {
            SetTargetPosition(Pathfinding.Instance.GetWorldPosition(unit.target.currentNode));
        }

        unit.isMoving = !MoveToNode(destination);
        if (!unit.isMoving)
        {
            if (pathVectorList != null && pathVectorList[0] == destination)
            {
                unit.currentNode.SetIsOccupied(true);
            }
            else
            {
                unit.currentNode.SetIsOccupied(false);
            }
            unit.currentNode = Pathfinding.Instance.GetNode(destination);
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetTargetPosition(Vector3 targetPos)
    {
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPos);

        if (pathVectorList == null || pathVectorList.Count <= 1) return;

        if (Pathfinding.Instance.GetNode(pathVectorList[1]).isOccupied) return;

        Pathfinding.Instance.GetNode(pathVectorList[1]).SetIsOccupied(true);
        destination = pathVectorList[1];
    }

    public void FindClosestTarget(Teams team)
    {
        float minDistance = float.PositiveInfinity;
        Unit targetFound = null;
        if (team == Teams.Player)
        {
            opposingUnits = gameManager.GetEnemyUnits();
        }
        else
        {
            opposingUnits = gameManager.GetPlayerUnits();
        }

        foreach (Unit enemy in opposingUnits)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= minDistance && !Pathfinding.Instance.IsNeighbourOccupied(enemy.currentNode))
            {
                minDistance = Vector3.Distance(transform.position, enemy.transform.position);
                targetFound = enemy;
            }
        }
        unit.target = targetFound;
    }
}
