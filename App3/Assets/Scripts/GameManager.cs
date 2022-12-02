using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public List<Unit> purchasableUnits = new List<Unit>();
    public List<Unit> heroUnits = new List<Unit>();
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();
    public Dictionary<string, int> traitsRaces = new Dictionary<string, int>();
    public int heroLevel = 1;
    public int round = 1;
    public int currency = 0;
    private Pathfinding pathfinding;
    public bool isRoundPrep;
    public GameObject square;
    private GameObject heroSelectPanel;
    private GameObject playFieldPanel;
    private Unit hero;
    public TMP_Text levelUpCost;
    public TMP_Text currencyText;
    public TMP_Text totalUnits;
    public TMP_Text currentLevel;
    private Unit randomUnit;
    public Image unitImage;
    public TMP_Text randomUnitName;
    public TMP_Text randomUnitCost;
    public TMP_Text bonusesText;
    private List<string> twoBonus = new List<string> { Traits.WARRIOR.ToString(), Traits.RANGER.ToString(), Traits.KNIGHT.ToString() };

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        pathfinding = new Pathfinding(8, 8, 20f);
        heroLevel = 1;
        round = 1;
        currency = 100;
        currencyText.text = currency.ToString();
        heroSelectPanel = GameObject.Find("HeroSelection");
        playFieldPanel = GameObject.Find("PlayField");
        heroSelectPanel.SetActive(true);
        playFieldPanel.SetActive(false);
        isRoundPrep = true;
    }

    public void Update()
    {
        currencyText.text = currency.ToString();
        totalUnits.text = playerUnits.Count + "/" + heroLevel + " Units";
    }

    private void DrawBoard()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                GameObject go = GameObject.Instantiate(square, new Vector3((float)x * 20 + 10, (float)y * 20 + 10), new Quaternion(0, 0, 0, 0), square.transform.parent);
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                if ((x + y) % 2 == 1)
                {
                    sr.color = new Color(255f, 255f, 255f, .25f);
                }
                else
                {
                    sr.color = new Color(0f, 0f, 0f, .25f);
                }
                go.SetActive(true);
            }
        }
    }
    private static Vector3 GetMouseWorldPosition()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return worldPos;
    }

    public List<Unit> GetEnemyUnits()
    {
        return enemyUnits;
    }

    public List<Unit> GetPlayerUnits()
    {
        return playerUnits;
    }

    public void RemoveUnit(Unit unit)
    {
        if (unit.team == Teams.Player)
        {
            playerUnits.Remove(unit);
        }
        if (unit.team == Teams.Enemy)
        {
            enemyUnits.Remove(unit);
        }

        Destroy(unit.gameObject);
    }

    public void SelectVampireHero()
    {
        Unit newUnit = Instantiate(heroUnits[3]);
        hero = newUnit;
        playerUnits.Add(newUnit);
        newUnit.Setup(Teams.Player, pathfinding.GetUnoccupiedNode(Teams.Player));
        heroSelectPanel.SetActive(false);
        playFieldPanel.SetActive(true);
        DrawBoard();
        traitsRaces.Add(hero.trait.ToString(), 1);
        traitsRaces.Add(hero.race.ToString(), 1);
        UpdateBonusesText();
    }
    public void SelectOrcHero()
    {
        Unit newUnit = Instantiate(heroUnits[2]);
        hero = newUnit;
        playerUnits.Add(newUnit);
        newUnit.Setup(Teams.Player, pathfinding.GetUnoccupiedNode(Teams.Player));
        heroSelectPanel.SetActive(false);
        playFieldPanel.SetActive(true);
        DrawBoard();
        traitsRaces.Add(hero.trait.ToString(), 1);
        traitsRaces.Add(hero.race.ToString(), 1);
        UpdateBonusesText();
    }
    public void SelectHumanHero()
    {
        Unit newUnit = Instantiate(heroUnits[1]);
        hero = newUnit;
        playerUnits.Add(newUnit);
        newUnit.Setup(Teams.Player, pathfinding.GetUnoccupiedNode(Teams.Player));
        heroSelectPanel.SetActive(false);
        playFieldPanel.SetActive(true);
        DrawBoard();
        traitsRaces.Add(hero.trait.ToString(), 1);
        traitsRaces.Add(hero.race.ToString(), 1);
        UpdateBonusesText();
    }
    public void SelectElfHero()
    {
        Unit newUnit = Instantiate(heroUnits[0]);
        hero = newUnit;
        playerUnits.Add(newUnit);
        newUnit.Setup(Teams.Player, pathfinding.GetUnoccupiedNode(Teams.Player));
        heroSelectPanel.SetActive(false);
        playFieldPanel.SetActive(true);
        DrawBoard();
        traitsRaces.Add(hero.trait.ToString(), 1);
        traitsRaces.Add(hero.race.ToString(), 1);
        UpdateBonusesText();
    }

    public void LevelUp()
    {
        if (heroLevel < 6)
        {
            currency -= Convert.ToInt32(Math.Floor(((Math.Pow(heroLevel, 2)) / 2) + 4));
            heroLevel++;
            hero.UpgradeHero();
            levelUpCost.text = Math.Floor(((Math.Pow(heroLevel, 2)) / 2) + 4).ToString();
            currentLevel.text = "Current Level: " + heroLevel;
            if (heroLevel == 6)
            {
                levelUpCost.text = "MAX";
            }
        }
    }

    public void Roll()
    {
        if (currency < 1) return;
        int costChance = UnityEngine.Random.Range(0, 100) + 1;
        int unitChance = UnityEngine.Random.Range(0, 4);
        currency--;
        if (heroLevel == 1)
        {
            // 100% of getting 1 cost
            randomUnit = purchasableUnits[unitChance];

        }
        else if (heroLevel == 2)
        {

            // 75% of getting 1 cost, 25% of getting 2 cost
            if (costChance <= 75)
            {
                randomUnit = purchasableUnits[unitChance];
            }
            else
            {
                randomUnit = purchasableUnits[unitChance + 4];
            }


        }
        else if (heroLevel == 3)
        {
            // 45% of getting 1 cost, 45% of getting 2 cost, 10% chacne of getting 3 cost
            if (costChance <= 45)
            {
                randomUnit = purchasableUnits[unitChance];
            }
            else if (costChance <= 90)
            {
                randomUnit = purchasableUnits[unitChance + 4];
            }
            else
            {
                randomUnit = purchasableUnits[unitChance + 8];
            }
        }
        else if (heroLevel == 4)
        {
            // 30% of getting 1 cost, 40% of getting 2 cost, 30% chance of getting 3 cost
            if (costChance <= 30)
            {
                randomUnit = purchasableUnits[unitChance];
            }
            else if (costChance <= 70)
            {
                randomUnit = purchasableUnits[unitChance + 4];
            }
            else
            {
                randomUnit = purchasableUnits[unitChance + 8];
            }
        }
        else if (heroLevel == 5)
        {
            // 15% of getting 1 cost, 20% of getting 2 cost, 40% chance of getting 3 cost, 25% of getting 4 cost
            if (costChance <= 15)
            {
                randomUnit = purchasableUnits[unitChance];
            }
            else if (costChance <= 35)
            {
                randomUnit = purchasableUnits[unitChance + 4];
            }
            else if (costChance <= 75)
            {
                randomUnit = purchasableUnits[unitChance + 8];
            }
            else
            {
                randomUnit = purchasableUnits[unitChance + 12];
            }
        }
        else
        {
            // 10% of getting 1 cost, 15% of getting 2 cost, 30% of getting 3 cost, 45% of getting 4 cost
            if (costChance <= 10)
            {
                randomUnit = purchasableUnits[unitChance];
            }
            else if (costChance <= 25)
            {
                randomUnit = purchasableUnits[unitChance + 4];
            }
            else if (costChance <= 55)
            {
                randomUnit = purchasableUnits[unitChance + 8];
            }
            else
            {
                randomUnit = purchasableUnits[unitChance + 12];

            }
        }
        unitImage.sprite = randomUnit.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite;
        randomUnitCost.text = randomUnit.cost.ToString();
        randomUnitName.text = randomUnit.name.Replace('_', ' ');
    }
    public void Purchase()
    {
        if (randomUnit == null) return;
        if (randomUnit.cost > currency) return;
        if (playerUnits.Count >= heroLevel) return;
        currency -= randomUnit.cost;
        Unit newUnit = Instantiate(randomUnit);
        if (IsUnitUnique(newUnit))
        {
            if (traitsRaces.TryGetValue(newUnit.trait.ToString(), out int val1))
            {
                traitsRaces[newUnit.trait.ToString()] = val1 + 1;
            }
            else
            {
                traitsRaces.Add(newUnit.trait.ToString(), 1);
            }

            if (traitsRaces.TryGetValue(newUnit.race.ToString(), out int val2))
            {
                traitsRaces[newUnit.race.ToString()] = val2 + 1;
            }
            else
            {
                traitsRaces.Add(newUnit.race.ToString(), 1);
            }
        }
        UpdateBonusesText();
        playerUnits.Add(newUnit);
        newUnit.Setup(Teams.Player, pathfinding.GetUnoccupiedNode(Teams.Player));

        randomUnit = null;
        randomUnitCost.text = "0";
        randomUnitName.text = "";
        unitImage.sprite = null;
    }

    private void UpdateBonusesText()
    {
        string text = "";
        foreach (KeyValuePair<string, int> bonus in traitsRaces)
        {
            if (bonus.Key == Traits.HERO.ToString()) text += bonus.Key + "\n";
            else
            {
                text += bonus.Key + " " + bonus.Value + "/" + (twoBonus.Contains(bonus.Key.ToString()) ? "2" : "3") + "\n";
            }
        }
        bonusesText.text = text;
    }

    private bool IsUnitUnique(Unit check)
    {
        foreach (Unit unit in playerUnits)
        {
            if (check.trait == unit.trait && check.race == unit.race)
            {
                return false;
            }
        }

        return true;
    }
}

public enum Teams
{
    Player,
    Enemy
}

public enum Races
{
    ELF,
    HUMAN,
    ORC,
    VAMPIRE
}

public enum Traits
{
    WARRIOR,
    RANGER,
    DRUID,
    KNIGHT,
    HERO
}