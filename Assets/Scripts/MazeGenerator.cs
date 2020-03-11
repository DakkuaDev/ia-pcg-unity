using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float wallProbability = 0.2f;
    public int[,] maze = null;
    public GameObject baseWallPrefab;
    public GameObject baseFloorPrefab;
    public int seed = -1;

    public float wallWidth = 3;
    public float wallHeight = 2;
    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (seed == -1)
        {
            seed = (int)(Random.value * 100000);
        }
        maze = GenerateMazeData(seed, width, height);

        string data = "";
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (maze[x,y] == 1)
                {
                    data += "#";
                }
                else
                {
                    data += ".";
                }
            }
            data += "\n";
        }
        Debug.Log(data);

        MatrixToPhysic.GeneratePhysicMaze(maze, transform, baseWallPrefab, baseFloorPrefab, wallWidth, wallHeight);

        var pos = FindStartPosition();
        if (pos.x != -1 && pos.y != -1)
        {
            var player = (GameObject) Instantiate(playerPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
        }
    }

    int[,] GenerateMazeData(int s, int w, int h)
    {
        System.Random r = new System.Random(s);

        int [,] ret = new int[w, h];
        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                if (y == 0 || y == h-1 || x == 0 || x == w-1)
                {
                    ret[x,y] = 1;
                }
                else if ((y%2) == 0 && (x%2) == 0)
                {
                    if (r.NextDouble() > wallProbability)
                    {
                        ret[x,y] = 1;

                        int a = r.NextDouble() < 0.5f ? 0 : (r.NextDouble() < 0.5 ? 1 : -1);
                        int b = a != 0 ? 0 : (r.NextDouble() < 0.5f ? 1 : -1);
                        ret[x+a, y+b] = 1;
                    }
                }
            }
        }

        return ret;
    }

    

    private Vector2 FindStartPosition()
    {
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                // first empty space is our start goal
                if (maze[x,y] == 0)
                {
                    return new Vector2(x * wallWidth, -y * wallWidth);
                }
            }
        }

        return new Vector2(-1,-1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
