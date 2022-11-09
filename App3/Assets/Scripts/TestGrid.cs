using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrid : MonoBehaviour
{
    private Pathfinding pathfinding;

    void Start()
    {
        pathfinding = new Pathfinding(8, 8);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            pathfinding.GetGrid().GetXY(GetMouseWorldPosition(), out int x, out int y);
            List<PathNode> path = pathfinding.FindPath(0,0,x,y);
            print(path);
            if (path != null)
            {
                for(int i=0;i<path.Count;i++)
                {
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f, new Vector3(path[i+1].x, path[i+1].y));
                }
            }
        }

        // if(Input.GetMouseButtonDown(1))
        // {
        //     Debug.Log(grid.GetValue(GetMouseWorldPosition()));
        // }
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return worldPos;
    }
}
