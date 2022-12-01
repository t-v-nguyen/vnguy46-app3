using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Unit> purchasableUnits = new List<Unit>();
    public List<Unit> heroUnits = new List<Unit>();
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();
    public int heroLevel = 1;
    public int round = 1;
    public int currency = 0;
    private Pathfinding pathfinding;
    public GameObject square;
    private GameObject heroSelectPanel;
    private GameObject playFieldPanel;

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
        currency = 0;
        heroSelectPanel = GameObject.Find("HeroSelection");
        playFieldPanel = GameObject.Find("PlayField");
        heroSelectPanel.SetActive(true);
        playFieldPanel.SetActive(false);

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
        playerUnits.Add(newUnit);
        newUnit.Setup(Teams.Player, pathfinding.GetUnoccupiedNode(Teams.Player));
        heroSelectPanel.SetActive(false);
        playFieldPanel.SetActive(true);
        DrawBoard();
    }
    public void SelectOrcHero()
    {
        Unit newUnit = Instantiate(heroUnits[2]);
        playerUnits.Add(newUnit);
        newUnit.Setup(Teams.Player, pathfinding.GetUnoccupiedNode(Teams.Player));
        heroSelectPanel.SetActive(false);
        playFieldPanel.SetActive(true);
        DrawBoard();
    }
    public void SelectHumanHero()
    {
        Unit newUnit = Instantiate(heroUnits[1]);
        playerUnits.Add(newUnit);
        newUnit.Setup(Teams.Player, pathfinding.GetUnoccupiedNode(Teams.Player));
        heroSelectPanel.SetActive(false);
        playFieldPanel.SetActive(true);
        DrawBoard();
    }
    public void SelectElfHero()
    {
        Debug.Log("Elf");
        Unit newUnit = Instantiate(heroUnits[0]);
        playerUnits.Add(newUnit);
        newUnit.Setup(Teams.Player, pathfinding.GetUnoccupiedNode(Teams.Player));
        heroSelectPanel.SetActive(false);
        playFieldPanel.SetActive(true);
        DrawBoard();
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