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

    private enum prefabfType
    {
        none,
        coin,
        staticEnemy,
        dinamicEnemy,

    }

    prefabfType prefab = prefabfType.none;

    public int width = 25;
    public int height = 25;
    public int tunnels = 50;
    public int maxLength = 10;

    public int[,] maze = null;

    public GameObject baseWallPrefab;
    public GameObject baseWallPrefab2;
    public GameObject baseFloorPrefab;

    public GameObject coinPrefab;
    public GameObject coinParent;

    public GameObject enemyPrefab;
    public GameObject dinamicEnemyPrefab;
    public GameObject enemyParent;

   
    public int seed = -1;
    private System.Random _rdmPrefab;
    private int waitPrefab = 0;
    Vector2 prefabPos = new Vector2(0,0);


    public float wallWidth = 3;
    public float wallHeight = 2;
    public GameObject playerPrefab;

    public GameObject levelCenitalCamera;

    //private List<Vector2> oldVector = new List<Vector2>();
    //int newX, newY = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Cenital Camera
        levelCenitalCamera.transform.position =  CenitalCameraPos();

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
        

        MatrixToPhysic.GeneratePhysicMaze(maze, transform, baseWallPrefab, baseWallPrefab2, baseFloorPrefab, wallWidth, wallHeight);

        // Coin and Enemies Position

        int newInstance = 0;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (maze[x, y] == 0) newInstance++;
                if(newInstance > 5)
                {
                    newInstance = 0;
                    prefab = prefabfType.none;

                    int type = Random.Range(1, 8);


                    if (type <= 4)
                    {
                        prefab = prefabfType.coin;
                        prefabPos = new Vector2(x * wallWidth, -y * wallWidth);
                    }
                    else if(type > 4 && type < 6)
                    {
                        prefab = prefabfType.staticEnemy;
                        prefabPos = new Vector2(x * wallWidth, -y * wallWidth);
                    }
                    else if(type == 7)
                    {
                        prefab = prefabfType.dinamicEnemy;
                        prefabPos = new Vector2(x * wallWidth, -y * wallWidth);
                    }

                switch (prefab)
                    {
                        case prefabfType.coin:
                            try
                            {
                                var coin = (GameObject)Instantiate(coinPrefab, new Vector3(prefabPos.x, 0, prefabPos.y), Quaternion.identity);
                                coin.transform.SetParent(coinParent.transform);
                                coin.name = "Coin" + prefabPos.x + "_" + prefabPos.y;
                            }
                            catch (System.Exception)
                            {
                                return;
                            }
                            break;
                        case prefabfType.staticEnemy:
                            try
                            {
                                var enemy = (GameObject)Instantiate(enemyPrefab, new Vector3(prefabPos.x, 1.5f, prefabPos.y), Quaternion.identity);
                                enemy.transform.Rotate(0, Random.Range(0.0f, 360.0f), 0);
                                enemy.transform.SetParent(enemyParent.transform);
                                enemy.name = "Enemy" + prefabPos.x + "_" + prefabPos.y;
                            }
                            catch (System.Exception)
                            {
                                return;
                            }
                            break;
                        case prefabfType.dinamicEnemy:
                            try
                            {
                                var enemy = (GameObject)Instantiate(dinamicEnemyPrefab, new Vector3(prefabPos.x, 0, prefabPos.y), Quaternion.identity);
                                enemy.transform.Rotate(0, Random.Range(0.0f, 360.0f), 0);
                                enemy.transform.SetParent(enemyParent.transform);
                                enemy.name = "Enemy" + prefabPos.x + "_" + prefabPos.y;
                            }
                            catch (System.Exception)
                            {
                                return;
                            }
                            break;

                        case prefabfType.none: break;
                    }

                }

            }
        }


        // Player Position
        var playerPos = FindStartPosition();
        
        if (playerPos.x != -1 && playerPos.y != -1)
        {
            var player = (GameObject)Instantiate(playerPrefab, new Vector3(playerPos.x, 0, playerPos.y), Quaternion.identity);
            GameObject.Find("PlayerPos").transform.position = player.transform.position;
           // player.transform.SetParent(GameObject.Find("PlayerPos").transform);
            
        }
    }

    public int[,] GenerateMazeData(int s, int w, int h, int t, int l)
    {
        System.Random r = new System.Random(s);

        int[,] ret = new int[w, h];

        // all walls
        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                //Random.seed = System.DateTime.Now.Millisecond;
                _rdmPrefab = new System.Random();
                int generateNumber = _rdmPrefab.Next(1,10);

                if (generateNumber < 9 && waitPrefab == 0) {
                    ret[x, y] = 2;
                    waitPrefab = 10;               
                } 
                else {
                    ret[x, y] = 1;
                    if(waitPrefab > 0) waitPrefab--;
                }

                //Debug.Log(ret[x, y]);
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
                            if (ret[currentX, currentY] == 1 || ret[currentX, currentY] == 2)
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
                            if (ret[currentX, currentY] == 1 || ret[currentX, currentY] == 2)
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
                            if (ret[currentX, currentY] == 1 || ret[currentX, currentY] == 2)
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
                            if (ret[currentX, currentY] == 1 || ret[currentX, currentY] == 2)
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


    // Automatic Cenital Camera Position
    private Vector3 CenitalCameraPos()
    {
        return new Vector3(transform.position.x + width, transform.position.y + width * 4, transform.position.z - height);
    }


    // Update is called once per frame
    void Update()
    {

    }
}

