using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode 
{
    private Grid<PathNode> grid;
    public int x;
    public int y;
    public int gCost;
    public int hCost;
    public int fCost;

    public PathNode cameFromNode;
    public bool isOccupied;
    public Unit unit;
    public int cellNum;

    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.isOccupied = false;
        this.unit = null;
    }

    public override string ToString()
    {
        return x + "," + y;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsOccupied(bool occupied)
    {
        this.isOccupied = occupied;
    }

    public bool GetIsOccupied()
    {
        return this.isOccupied;
    }
}
