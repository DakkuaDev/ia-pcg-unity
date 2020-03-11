using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneratorWalker : MonoBehaviour
{ 
    public enum Direction : int
    {
        North,
        South,
        West,
        East,
        None,
    }

    public int width = 10;
    public int height = 10;
    public int tunnels = 5;
    public int maxLength = 4;

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
        maze = GenerateMazeData(seed, width, height, tunnels, maxLength);

        string data = "";
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (maze[x, y] == 1)
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
            var player = (GameObject)Instantiate(playerPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
        }
    }

    int[,] GenerateMazeData(int s, int w, int h, int t, int l)
    {
        System.Random r = new System.Random(s);

        int[,] ret = new int[w, h];

        // all walls
        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                ret[x, y] = 1;
            }
        }

        int currentX = r.Next(1, w-1);
        int currentY = r.Next(1, h-1);

        ret[currentX, currentY] = 0;

        int iterations = 0;
        Direction lastDirection = Direction.None;

        while (t > 0 && (iterations < 10000))
        {
            int tunnelSize = r.Next(1, l);
            int currentTunnelLenght = 0;

            int direction = -1;
            bool validDirection = false;
            do
            {
                direction = r.Next(0, 4);
                if ((Direction)direction == Direction.North || (Direction)direction == Direction.South)
                {
                    validDirection = lastDirection != Direction.North && lastDirection != Direction.South;
                }
                else if ((Direction)direction == Direction.East || (Direction)direction == Direction.West)
                {
                    validDirection = lastDirection != Direction.West && lastDirection != Direction.East;
                }
            }
            while (!validDirection);

            for (int i = 0; i < tunnelSize; ++i)
            {
                switch ((Direction)direction)
                {
                    // north
                    case Direction.North:
                        if (currentY > 1)
                        {
                            currentY--;
                            if (ret[currentX, currentY] == 1)
                            {
                                ret[currentX, currentY] = 0;
                                currentTunnelLenght++;
                            }
                        }
                        break;

                    // south
                    case Direction.South:
                        if (currentY < h-2)
                        {
                            currentY++;
                            if (ret[currentX, currentY] == 1)
                            {
                                ret[currentX, currentY] = 0;
                                currentTunnelLenght++;
                            }
                        }
                        break;

                    // west
                    case Direction.West:
                        if (currentX > 1)
                        {
                            currentX--;
                            if (ret[currentX, currentY] == 1)
                            {
                                ret[currentX, currentY] = 0;
                                currentTunnelLenght++;
                            }
                        }
                        break;

                    // east
                    case Direction.East:
                        if (currentX < w-2)
                        {
                            currentX++;
                            if (ret[currentX, currentY] == 1)
                            {
                                ret[currentX, currentY] = 0;
                                currentTunnelLenght++;
                            }
                        }
                        break;
                }
            }

            lastDirection = (Direction)direction;
            if (currentTunnelLenght > 0)
            {
                t--;
            }

            iterations++;
        }

        Debug.Log("Iterations: " + iterations);

        return ret;
    }



    private Vector2 FindStartPosition()
    {
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                // first empty space is our start goal
                if (maze[x, y] == 0)
                {
                    return new Vector2(x * wallWidth, -y * wallWidth);
                }
            }
        }

        return new Vector2(-1, -1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

