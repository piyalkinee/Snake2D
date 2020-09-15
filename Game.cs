using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game : MonoBehaviour
{
    #region Main
    //Fields 
    private GameObject CategoryWorld;
    public Sprite WhiteQuad;
    private Cell[,] Cells;
    private List<Cell> Snake;

    private TextMeshProUGUI ScoreText;

    private int SizeX = 25;
    private int SizeY = 15;

    private float SnakeSpeed = 0.2f;
    private Direction SnakeDirection = Direction.Up;
    private bool SnakeBlockControl = false;

    private bool GameIsStarted = false;
    private int Score = 0;

    enum Direction
    {
        Up, Right, Down, Left
    }

    //Methods
    private void Start()
    {
        CategoryWorld = GameObject.Find("-World-");

        ScoreText = GameObject.Find("text_Score").GetComponent<TextMeshProUGUI>();

        StartNewGame();
    }
    private void Update()
    {
        if (!SnakeBlockControl)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (SnakeDirection != Direction.Down)
                {
                    SnakeDirection = Direction.Up;
                    SnakeBlockControl = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (SnakeDirection != Direction.Left)
                {
                    SnakeDirection = Direction.Right;
                    SnakeBlockControl = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (SnakeDirection != Direction.Up)
                {
                    SnakeDirection = Direction.Down;
                    SnakeBlockControl = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (SnakeDirection != Direction.Right)
                {
                    SnakeDirection = Direction.Left;
                    SnakeBlockControl = true;
                }
            }
        }
    }
    private void StartNewGame()
    {
        GameIsStarted = true;
        Score = 0;
        ScoreText.text = "Your score: " + Score;
        SnakeDirection = Direction.Up;

        ClearWorld();
        GenerateWorld();
        SnakeCreate();
        AppleCreate();
        UpdateCells();

        StartCoroutine("SnakeMove");
    }
    #endregion
    #region World
    private void GenerateWorld()
    {
        Cells = new Cell[SizeX, SizeY];

        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                //Create object
                Cells[x, y] = new Cell(new GameObject(), new Vector2(x, y), CategoryWorld, WhiteQuad);
                //Create cell
                if (x < 1 ||
                    y < 1 ||
                    x > SizeX - 2 ||
                    y > SizeY - 2)
                {
                    Cells[x, y].ID = 1;
                }
                else
                {
                    Cells[x, y].ID = 0;
                }
            }
        }
    }
    private void ClearWorld()
    {
        if (Cells != null)
        {
            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    Cells[x, y].DestroyCell();
                }
            }

            Cells = null;
        }
    }
    public void UpdateCells()
    {
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                switch (Cells[x, y].ID)
                {
                    case 0: Cells[x, y].SpriteRenderer.color = new Color32(220, 199, 190, 255); break;
                    case 1: Cells[x, y].SpriteRenderer.color = new Color32(255, 0, 110, 255); break;
                    case 2: Cells[x, y].SpriteRenderer.color = new Color32(131, 56, 236, 255); break;
                    case 3: Cells[x, y].SpriteRenderer.color = new Color32(251, 85, 7, 255); break;
                }
            }
        }
    }
    #endregion
    #region Snake
    private void SnakeCreate()
    {
        Snake = new List<Cell>();

        int snakeHeadX;
        int snakeHeadY;

        if (SizeX % 2 == 0)
        {
            snakeHeadX = (SizeX / 2) - 1;
        }
        else
        {
            snakeHeadX = ((SizeX - 1) / 2);
        }
        if (SizeY % 2 == 0)
        {
            snakeHeadY = (SizeY / 2) - 1;
        }
        else
        {
            snakeHeadY = ((SizeY - 1) / 2);
        }

        Cells[snakeHeadX, snakeHeadY].ID = 2;
        Snake.Add(Cells[snakeHeadX, snakeHeadY]);

        for (int i = 1; i < 3; i++)
        {
            Cells[snakeHeadX, snakeHeadY - i].ID = 2;
            Snake.Add(Cells[snakeHeadX, snakeHeadY - i]);
        }
    }
    private IEnumerator SnakeMove()
    {
        yield return new WaitForSeconds(SnakeSpeed);

        List<Cell> snakeNextPositins = new List<Cell>();

        int correctX = 0;
        int correctY = 0;

        switch (SnakeDirection)
        {
            case Direction.Up:
                correctX = 0;
                correctY = 1;
                break;
            case Direction.Right:
                correctX = 1;
                correctY = 0;
                break;
            case Direction.Down:
                correctX = 0;
                correctY = -1;
                break;
            case Direction.Left:
                correctX = -1;
                correctY = 0;
                break;
        }

        if (Snake[0].Transform.position.x + correctX >= 0 ||
            Snake[0].Transform.position.x + correctX <= SizeX ||
            Snake[0].Transform.position.y + correctY >= 0 ||
            Snake[0].Transform.position.y + correctY <= SizeY)
        {
            if (Cells[(int)Snake[0].Transform.position.x + correctX, (int)Snake[0].Transform.position.y + correctY].ID == 1 ||
                Cells[(int)Snake[0].Transform.position.x + correctX, (int)Snake[0].Transform.position.y + correctY].ID == 2)
            {
                Debug.Log("Game over, your score " + Score);
                GameIsStarted = false;
            }
            else
            {
                if (Cells[(int)Snake[0].Transform.position.x + correctX, (int)Snake[0].Transform.position.y + correctY].ID == 3)
                {
                    Cells[(int)Snake[0].Transform.position.x + correctX, (int)Snake[0].Transform.position.y + correctY].SnakeNewCell = true;
                    AppleEat();
                }

                //Move
                for (int i = 0; i < Snake.Count; i++)
                {
                    if (i == 0)
                    {
                        snakeNextPositins.Add(Cells[(int)Snake[0].Transform.position.x + correctX, (int)Snake[0].Transform.position.y + correctY]);
                        Cells[(int)Snake[0].Transform.position.x + correctX, (int)Snake[0].Transform.position.y + correctY].ID = 2;
                    }
                    else
                    {
                        snakeNextPositins.Add(Snake[i - 1]);
                    }
                }
            }
        }

        //AddLong
        if (Snake[Snake.Count - 1].SnakeNewCell)
        {
            Snake[Snake.Count - 1].SnakeNewCell = false;
            snakeNextPositins.Add(Snake[Snake.Count - 1]);
        }
        else
        {
            Snake[Snake.Count - 1].ID = 0;
        }

        Snake = snakeNextPositins;

        UpdateCells();

        SnakeBlockControl = false;

        if (GameIsStarted)
            StartCoroutine("SnakeMove");
        else
            StartNewGame();
    }
    #endregion
    #region Apple
    private void AppleCreate()
    {
        while (true)
        {
            int randomX = Random.Range(0, Cells.GetLength(0));
            int randomY = Random.Range(0, Cells.GetLength(1));

            if (Cells[randomX, randomY].ID == 0)
            {
                Cells[randomX, randomY].ID = 3;
                break;
            }
        }
    }
    private void AppleEat()
    {
        Score += 4;
        ScoreText.text = "Your score: " + Score;
        AppleCreate();
    }
    #endregion
}