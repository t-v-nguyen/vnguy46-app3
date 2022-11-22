using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Unit> allUnits = new List<Unit>();
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();
    public Dictionary<Teams, List<Unit>> unitsOnField = new Dictionary<Teams, List<Unit>>();
    int level = 1;
    private Pathfinding pathfinding;

    // Start is called before the first frame update
    void Start()
    {
        pathfinding = new Pathfinding(8, 8, 20f);
        InstantiatePlayerUnits();
        InstantiateEnemyUnits();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            pathfinding.GetGrid().GetXY(GetMouseWorldPosition(), out int x, out int y);
            PathNode node = pathfinding.GetNode(x,y);
            Debug.Log(node.isOccupied);
        }
    }
    private static Vector3 GetMouseWorldPosition()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return worldPos;
    }

    private void InstantiatePlayerUnits()
    {
        unitsOnField.Add(Teams.Player, new List<Unit>());
        for (int i = 0; i < level; i++)
        {
            Unit newUnit = Instantiate(allUnits[0]);
            unitsOnField[Teams.Player].Add(newUnit);
            playerUnits.Add(newUnit);

            newUnit.Setup(Teams.Player, pathfinding.GetRandomUnoccupiedNode(Teams.Player));


        }
    }

    private void InstantiateEnemyUnits()
    {
        unitsOnField.Add(Teams.Enemy, new List<Unit>());
        for(int i=0;i<2;i++)
        {
            Unit newUnit = Instantiate(allUnits[0]);
            unitsOnField[Teams.Enemy].Add(newUnit);
            enemyUnits.Add(newUnit);

            newUnit.Setup(Teams.Enemy, pathfinding.GetRandomUnoccupiedNode(Teams.Enemy));
        }
    }

    public List<Unit> GetEnemyUnits()
    {
        return enemyUnits;
    }

    public List<Unit> GetPlayerUnits()
    {
        return playerUnits;
    }
}

public enum Teams
{
    Player,
    Enemy
}
