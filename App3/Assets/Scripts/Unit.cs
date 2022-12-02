using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : UnitMovement
{
    private UnitMovement unitMovement;
    public Traits trait;
    public Races race;
    public int cost;
    public GameObject healthBar;
    public int damage = 10;
    private int currentHealth;
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
    }

    public void Setup(Teams team, PathNode spawnNode)
    {
        this.team = team;
        this.currentNode = spawnNode;
        transform.position = Pathfinding.Instance.GetWorldPosition(currentNode);
        currentNode.SetIsOccupied(true);
        currentHealth = maxHealth;
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

    private void TakeDamage(int damage, Unit damageDealer)
    {
        currentHealth -= damage;
        float scale = ((float)currentHealth) / maxHealth;
        healthBar.transform.localScale = new Vector3(scale, 1, 0);
        if (currentHealth <= 0)
        {
            if(trait == Traits.HERO)
            {
                damageDealer.target = null;
                gameObject.SetActive(false);
                Pathfinding.Instance.GetNode(currentNode).isOccupied = false;
                GameManager.Instance.playerUnits.Remove(this);
            }
            else
            {
                GameManager.Instance.RemoveUnit(this);
            }
        }
    }

    public void Update()
    {
        if (GameManager.Instance.isRoundPrep == false)
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
                    target.TakeDamage(damage, this);
                }
            }
            else
            {
                unitMovement.GetInRange();
            }
        }
    }

    public void UpgradeHero()
    {
        maxHealth += 20;
        damage += 2;
    }

    public void HealUnit()
    {
        currentHealth = maxHealth;
        healthBar.transform.localScale = new Vector3(1, 1, 0);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void RelocateUnit(PathNode node)
    {
        Pathfinding.Instance.GetNode(currentNode).isOccupied = false;
        this.currentNode = node;
        transform.position = Pathfinding.Instance.GetWorldPosition(currentNode);
        currentNode.SetIsOccupied(true);
        HealUnit();
    }


}
