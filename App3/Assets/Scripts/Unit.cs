using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private float damageMult = 1;
    private float elapsed = 0;

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
        if (race == Races.ELF && GameManager.Instance.activeTraitsRaces.Contains(Races.ELF.ToString()) && UnityEngine.Random.Range(0, 100) < 10) damage = 0;
        if (trait == Traits.KNIGHT && GameManager.Instance.activeTraitsRaces.Contains(Traits.KNIGHT.ToString())) damage -= 3;
        currentHealth -= damage;
        float scale = ((float)currentHealth) / maxHealth;
        healthBar.transform.localScale = new Vector3(scale, 1, 0);
        if (currentHealth <= 0)
        {
            if (trait == Traits.HERO)
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
                    if (race == Races.VAMPIRE && GameManager.Instance.activeTraitsRaces.Contains(Races.VAMPIRE.ToString()))
                    {
                        currentHealth += Convert.ToInt32(Math.Round(damage * 0.1f));
                        float scale = ((float)currentHealth) / maxHealth;
                        healthBar.transform.localScale = new Vector3(scale, 1, 0);
                    }
                    if (race == Races.HUMAN && GameManager.Instance.activeTraitsRaces.Contains(Races.HUMAN.ToString())) damageMult = 1.1f;
                    else if (trait == Traits.RANGER && GameManager.Instance.activeTraitsRaces.Contains(Traits.RANGER.ToString()) && UnityEngine.Random.Range(0, 100) < 5) damageMult = 2f;
                    else damageMult = 1f;
                    target.TakeDamage(Convert.ToInt32(Math.Round(damage * damageMult)), this);
                }
            }
            else
            {
                unitMovement.GetInRange();
            }

            if (trait == Traits.WARRIOR && GameManager.Instance.activeTraitsRaces.Contains(Traits.WARRIOR.ToString()))
            {
                elapsed += Time.deltaTime;
                if (elapsed >= 1f)
                {
                    elapsed = elapsed % 1f;
                    currentHealth += 4;
                }
            }
        }
        else
        {
            elapsed = 0;
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
        this.currentNode.SetIsOccupied(true);
        HealUnit();
    }

    public void OrcTrait(bool isActive)
    {
        if (isActive)
        {
            maxHealth = Convert.ToInt32(Math.Round(maxHealth * 1.1f));
            currentHealth = maxHealth;
        }
        else
        {
            maxHealth = Convert.ToInt32(Math.Round(maxHealth * .91f));
            currentHealth = maxHealth;
        }
    }


}
