using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : UnitMovement
{
    private UnitMovement unitMovement;
    public int damage = 1;
    public int health = 100;
    public int range = 1;
    public float AS = 1f; // Attack speed
    public float MS = 20f; // Movement speed
    public Teams team;
    public PathNode currentNode;
    public Unit target;
    public bool isMoving;
    protected bool canAttack = true;
    private float timeUntilAttack;
    protected bool targetInRange => target != null && Vector3.Distance(this.transform.position, target.transform.position) <= range;

    private void Start()
    {
        unitMovement = GetComponent<UnitMovement>();
    }

    public void Setup(Teams team, PathNode spawnNode)
    {
        this.team = team;
        this.currentNode = spawnNode;
        transform.position = Pathfinding.Instance.GetWorldPosition(currentNode);
        currentNode.SetIsOccupied(true);
    }

    protected void Attack()
    {
        if (canAttack == false) return;

        timeUntilAttack = 1 / AS;
        StartCoroutine(AttackCooldown());

        IEnumerator AttackCooldown()
        {
            canAttack = false;
            yield return null;

            yield return new WaitForSeconds(timeUntilAttack);
            canAttack = true;
        }
    }

    public void Update()
    {
        if (target == null)
        {
            unitMovement.FindClosestTarget(team);
        }

        if (targetInRange && !isMoving)
        {

        }
        else
        {
            unitMovement.GetInRange();
        }
    }


}
