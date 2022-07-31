using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public static Board instance { get; private set; }

    public int[,] grid = new int[3, 3];
    public Tilemap gridUI;

    [SerializeField] Tile cross;
    [SerializeField] Tile circle;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            instance = this;
        }
    }

    public void InitGrid()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                grid[i, j] = 0;
                gridUI.SetTile(new Vector3Int(j, -i, 0), null);
            }
        }
    }

    public static bool CheckWinConditions(int[,] board, int nb)
    {
        if (
            (board[0, 0] == nb && board[0, 1] == nb && board[0, 2] == nb) ||
            (board[1, 0] == nb && board[1, 1] == nb && board[1, 2] == nb) ||
            (board[2, 0] == nb && board[2, 1] == nb && board[2, 2] == nb) ||

            (board[0, 0] == nb && board[1, 0] == nb && board[2, 0] == nb) ||
            (board[0, 1] == nb && board[1, 1] == nb && board[2, 1] == nb) ||
            (board[0, 2] == nb && board[1, 2] == nb && board[2, 2] == nb) ||

            (board[0, 0] == nb && board[1, 1] == nb && board[2, 2] == nb) ||
            (board[0, 2] == nb && board[1, 1] == nb && board[2, 0] == nb)
        )
        {
            return true;
        }
        return false;
    }

    public void Draw(Symbol symbolToDraw, int x, int y)
    {
        if(symbolToDraw == Symbol.Circle)
        {
            grid[x, y] = 2;
            gridUI.SetTile(new Vector3Int(y, -x, 0), circle);
        } else
        {
            grid[-y, x] = 1;
            gridUI.SetTile(new Vector3Int(x, y, 0), cross);
        }
    }
    public bool BoardOutOfMoves(int[,] board)
    {
        bool foundMoves = false;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == 0)
                {
                    foundMoves = true;
                    break;
                }
            }
        }
        return !foundMoves;
    }

    public int GetScoreForBoard(int[,] board, int depth)
    {
        if (Board.CheckWinConditions(board, 2))
            return 10 * (depth + 1);
        if (Board.CheckWinConditions(board, 1))
            return -10 * (depth + 1);
        return 0 * (depth + 1);
    }

    public List<Vector2Int> GetAvailableMoves(int[,] board)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == 0)
                    availableMoves.Add(new Vector2Int(i, j));
            }
        }
        return availableMoves;
    }

    public void GenerateNewBoard(int[,] board, Vector2Int move, bool cpuTurn)
    {
        if (cpuTurn)
            board[move.x, move.y] = 2;
        else
            board[move.x, move.y] = 1;
    }
}

public enum Symbol
{
    Circle,
    Cross
}
