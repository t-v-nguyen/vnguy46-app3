using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private Unit unit;
    private int currentPathIndex;
    public List<Vector3> pathVectorList;
    private bool isMoving;
    private List<Unit> opposingUnits = new List<Unit>();
    private GameManager gameManager;

    void Start()
    {
        unit = GetComponent<Unit>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        FindClosestTarget(unit.GetTeam());
    }


    // Update is called once per frame
    void Update()
    {
        if (unit.target == null)
        {
            FindClosestTarget(unit.GetTeam());
        }
        HandleMovement();
        // check if moving and attacking
        // if moving then move towards target
        // if attacking dont move
        // if not moving or attacking find new target
        // if(isMoving)
        // {
        //     HandleMovement();
        // }
        // else if(targetInRange && !isMoving)
        // {
        //     if(canAttack)
        //     {
        //         Attack();
        //         Debug.Log("Attacked");
        //     }
        // }
        // else
        // {
        //     FindClosestTarget(team);
        // }
    }

    private void HandleMovement()
    {
        if (pathVectorList != null)
        {
            Vector3 targetPos = pathVectorList[currentPathIndex];

            if (Vector3.Distance(transform.position, targetPos) > 0.5f)
            {
                Vector3 moveDir = (targetPos - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPos);
                transform.position = transform.position + moveDir * unit.MS * Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
                // if (CheckEnemiesInRange(unit.team == Teams.Player ? Teams.Enemy : Teams.Player)) // check for enemies
                // {
                //     Debug.Log("Enemy Found");
                //     isMoving = false;
                //     pathVectorList = null;
                // }
                if (currentPathIndex >= pathVectorList.Count) // reached location
                {
                    pathVectorList = null;
                    isMoving = false;
                }
                else if (Pathfinding.Instance.GetNode(pathVectorList[currentPathIndex]) == unit.target.GetNode()) // next node contains target
                {
                    isMoving = false;
                    pathVectorList = null;
                }
                else if (Pathfinding.Instance.GetNode(pathVectorList[currentPathIndex]).GetIsOccupied()) // next node is already occupied
                {
                    FindClosestTarget(unit.team);
                }
                else // next node is empty
                {
                    unit.currentNode.SetIsOccupied(false);
                    Pathfinding.Instance.GetNode(pathVectorList[currentPathIndex - 1]).unit = null;
                    unit.currentNode = Pathfinding.Instance.GetNode(pathVectorList[currentPathIndex]);
                    unit.currentNode.SetIsOccupied(true);
                    Pathfinding.Instance.GetNode(pathVectorList[currentPathIndex]).unit = unit;
                }
            }


        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetTargetPosition(Vector3 targetPos)
    {
        currentPathIndex = 0;
        isMoving = true;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPos);
        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            Pathfinding.Instance.GetNode(pathVectorList[0]).SetIsOccupied(false);
            Pathfinding.Instance.GetNode(pathVectorList[0]).unit = null;
            Pathfinding.Instance.GetNode(pathVectorList[1]).SetIsOccupied(true);
            Pathfinding.Instance.GetNode(pathVectorList[1]).unit = unit;
            pathVectorList.RemoveAt(0);
            pathVectorList.RemoveAt(pathVectorList.Count-1);
        }
    }

    protected void FindClosestTarget(Teams team)
    {
        float minDistance;
        if (team == Teams.Player)
        {
            opposingUnits = gameManager.GetEnemyUnits();
            minDistance = Vector3.Distance(transform.position, opposingUnits[0].transform.position);
        }
        else
        {
            opposingUnits = gameManager.GetPlayerUnits();
            minDistance = Vector3.Distance(transform.position, opposingUnits[0].transform.position);
        }

        foreach (Unit enemy in opposingUnits)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= minDistance)
            {
                minDistance = Vector3.Distance(transform.position, enemy.transform.position);
                unit.target = enemy;
            }
        }
        // PathNode closestUnoccupiedNode = Pathfinding.Instance.GetClosestAdjacentUnoccupiedNode(unit.currentNode, unit.target.currentNode);
        SetTargetPosition(Pathfinding.Instance.GetWorldPosition(unit.target.currentNode));
    }

    protected bool CheckEnemiesInRange(Teams oppositeTeam)
    {
        bool enemyInRange = false;
        if (unit.currentNode.x - 1 >= 0)
        {
            if (Pathfinding.Instance.GetNode(unit.currentNode.x - 1, unit.currentNode.y).isOccupied && Pathfinding.Instance.GetNode(unit.currentNode.x - 1, unit.currentNode.y).unit.team == oppositeTeam)
            {
                enemyInRange = true;
            }
        }
        else if (unit.currentNode.x + 1 < Pathfinding.Instance.GetGrid().GetWidth())
        {
            if (Pathfinding.Instance.GetNode(unit.currentNode.x + 1, unit.currentNode.y).isOccupied && Pathfinding.Instance.GetNode(unit.currentNode.x + 1, unit.currentNode.y).unit.team == oppositeTeam)
            {
                enemyInRange = true;
            }
        }
        else if (unit.currentNode.y - 1 >= 0)
        {
            if (Pathfinding.Instance.GetNode(unit.currentNode.x, unit.currentNode.y-1).isOccupied && Pathfinding.Instance.GetNode(unit.currentNode.x, unit.currentNode.y - 1).unit.team == oppositeTeam)
            {
                enemyInRange = true;
            }
        }
        else if (unit.currentNode.y + 1 < Pathfinding.Instance.GetGrid().GetHeight())
        {
            if (Pathfinding.Instance.GetNode(unit.currentNode.x, unit.currentNode.y+1).isOccupied && Pathfinding.Instance.GetNode(unit.currentNode.x, unit.currentNode.y + 1).unit.team == oppositeTeam)
            {
                enemyInRange = true;
            }
        }
        return enemyInRange;
    }
}
