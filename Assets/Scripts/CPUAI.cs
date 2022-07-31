using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPUAI : MonoBehaviour
{
    private Vector2Int nextBestPlay;
    public int MiniMax(int[,] state, bool isMaximizing, int depth)
    {

        if (CheckGameOver(state))
        {
            return Board.instance.GetScoreForBoard(state, depth);
        }


        List<int> scores = new List<int>();
        List<Vector2Int> moves = new List<Vector2Int>();

        List<Vector2Int> allPossibleMoves = Board.instance.GetAvailableMoves(state);

        for (int i = 0; i < allPossibleMoves.Count; i++)
        {
            int[,] newStateToBeSearched = (int[,])state.Clone();
            Board.instance.GenerateNewBoard(newStateToBeSearched, allPossibleMoves[i], isMaximizing);

            scores.Add(MiniMax(newStateToBeSearched, !isMaximizing, depth - 1));
            moves.Add(allPossibleMoves[i]);
        }

        if (isMaximizing)
        {
            int maxIndex = GetMaxIndex(scores);
            nextBestPlay = moves[maxIndex];
            return scores[maxIndex];
        }
        else
        {
            int minIndex = GetMinIndex(scores);
            return scores[minIndex];
        }
    }

    int GetMinIndex(List<int> scores)
    {
        int minScore = 999;
        int minIndex = -1;
        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i] < minScore)
            {
                minIndex = i;
                minScore = scores[i];
            }
        }
        return minIndex;
    }

    int GetMaxIndex(List<int> scores)
    {
        int maxScore = -999;
        int maxIndex = -1;
        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i] > maxScore)
            {
                maxIndex = i;
                maxScore = scores[i];
            }
        }
        return maxIndex;
    }

    bool CheckGameOver(int[,] board)
    {
        return Board.instance.BoardOutOfMoves(board) || Board.CheckWinConditions(board, 1) || Board.CheckWinConditions(board, 2);
    }

    public void GenerateCPUMove(int[,] grid, int difficulty, int movesPlayed) //Check if there are any valid spaces prior
    {
        int x;
        int y;

        if (movesPlayed >= difficulty)
        {
            MiniMax(grid, true, 9 - movesPlayed);
            x = nextBestPlay.x;
            y = nextBestPlay.y;
        }
        else
        {
            do
            {
                x = Random.Range(0, 3);
                y = Random.Range(0, 3);
            } while (grid[x, y] == 1 || grid[x, y] == 2);
        }

        Board.instance.Draw(Symbol.Circle, x, y);
    }
}
