using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : UnitMovement
{
    private GameManager gameManager;
    private UnitMovement unitMovement;
    public Traits trait;
    public Races race;
    public int cost;
    public GameObject healthBar;
    public int damage = 10;
    private int currentHealth = 100;
    public int maxHealth = 100;
    public int range = 60;
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
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void Setup(Teams team, PathNode spawnNode)
    {
        this.team = team;
        this.currentNode = spawnNode;
        transform.position = Pathfinding.Instance.GetWorldPosition(currentNode);
        currentNode.SetIsOccupied(true);
        maxHealth = currentHealth;
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

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        float scale = ((float)currentHealth)/maxHealth;
        healthBar.transform.localScale = new Vector3(scale, 1, 0);
        if(currentHealth <= 0)
        {
            gameManager.RemoveUnit(this);
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
            if (canAttack)
            {
                Attack();
                target.TakeDamage(damage);
            }
        }
        else
        {
            unitMovement.GetInRange();
        }
    }


}
