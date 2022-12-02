using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private const int straight = 10;
    private const int diagonal = 14;

    public static Pathfinding Instance { get; private set; }
    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closeList;

    public Pathfinding(int width, int height, float cellsize)
    {
        Instance = this;
        grid = new Grid<PathNode>(width, height, cellsize, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }

    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
    {
        grid.GetXY(startPos, out int startX, out int startY);
        grid.GetXY(endPos, out int endX, out int endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if (path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * 0.5f);
            }
            return vectorPath;
        }
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetValue(startX, startY);
        PathNode endNode = grid.GetValue(endX, endY);
        openList = new List<PathNode> { startNode };
        closeList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetValue(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            foreach (PathNode neighbour in GetNeighbourList(currentNode))
            {
                if (closeList.Contains(neighbour)) continue;
                if (neighbour != endNode && neighbour.isOccupied )
                {
                    closeList.Add(neighbour);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbour);
                if (tentativeGCost < neighbour.gCost)
                {
                    neighbour.cameFromNode = currentNode;
                    neighbour.gCost = tentativeGCost;
                    neighbour.hCost = CalculateDistanceCost(neighbour, endNode);
                    neighbour.CalculateFCost();

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }

        return null;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return diagonal * Mathf.Min(xDistance, yDistance) + straight * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    public PathNode GetNode(PathNode node)
    {
        return grid.GetValue(node.x, node.y);
    }

    public PathNode GetNode(int x, int y)
    {
        return grid.GetValue(x, y);
    }

    public PathNode GetNode(Vector3 worldPos)
    {
        grid.GetXY(worldPos, out int x, out int y);
        return grid.GetValue(x, y);
    }

    public PathNode GetUnoccupiedNode(Teams team)
    {
        if (team == 0) // Allies are bottom side of grid
        {
            for (int y = 0; y < grid.GetHeight() / 2; y++)
            {
                for (int x = 0; x < grid.GetWidth(); x++)
                {
                    if (GetNode(x, y).isOccupied == false) return GetNode(x, y);
                }
            }
        }
        else
        {
            for (int y = grid.GetHeight() - 1; y >= grid.GetHeight() / 2; y--)
            {
                for (int x = 0; x < grid.GetWidth(); x++)
                {
                    if (GetNode(x, y).isOccupied == false) return GetNode(x, y);
                }
            }
        }
        return null;
    }

    public PathNode GetRandomUnoccupiedNode(Teams team)
    {
        if (team == 0) // Allies are bottom side of grid
        {
            while (true)
            {
                int randy = Random.Range(0, (int)grid.GetHeight() / 2);
                int randx = Random.Range(0, (int)grid.GetWidth());
                if (GetNode(randx, randy).isOccupied == false) return GetNode(randx, randy);
            }
        }
        else
        {
            while (true)
            {
                int randy = Random.Range((int)grid.GetHeight() / 2, (int)grid.GetHeight());
                int randx = Random.Range(0, (int)grid.GetWidth());
                if (GetNode(randx, randy).isOccupied == false) return GetNode(randx, randy);
            }
        }
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.x - 1 >= 0)
        {
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        if (currentNode.x + 1 < grid.GetWidth())
        {
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    public bool IsNeighbourOccupied(PathNode targetNode)
    {
        bool allOccupied = true;
        List<PathNode> neighbourList = GetNeighbourList(targetNode);
        foreach(PathNode neighbour in neighbourList)
        {
            if(!neighbour.isOccupied) allOccupied = false;
        }
        return allOccupied;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    public Grid<PathNode> GetGrid()
    {
        return grid;
    }

    public Vector3 GetWorldPosition(PathNode node)
    {
        return grid.GetWorldPosition(node.x, node.y) + new Vector3(grid.GetCellSize() / 2, grid.GetCellSize() / 2);
    }

    public Vector3 GetWorldPosition(Vector3 worldPos)
    {
        grid.GetXY(worldPos, out int x, out int y);
        return grid.GetWorldPosition(x, y) + new Vector3(grid.GetCellSize() / 2, grid.GetCellSize() / 2);;
    }

}
