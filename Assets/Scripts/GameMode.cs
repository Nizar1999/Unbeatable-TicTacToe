using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class GameMode : MonoBehaviour
{
    [SerializeField] GameObject diffWarning;
    [SerializeField] Button diffButton;
    [SerializeField] GameObject[] scoreBoards;

    private int tieCount = 0;
    private int playerWinCount = 0;
    private int cpuWinCount = 0;
    private int movesPlayed = 0;
    private int difficulty = 6;
    private int pendingDiff = 6;

    private bool playerTurn = false;
    private bool pendingRestart = false;
    private bool pendingPlay = false;

    private CPUAI cpu;

    private void Awake()
    {
        cpu = GetComponent<CPUAI>();
    }

    private void Start()
    {
        diffButton.onClick.AddListener(ChangeDifficulty);
        InitGame();
    }

    private void Update()
    {
        if (pendingRestart)
            return;
        if (movesPlayed == 9)
        {
            Tie();
            pendingRestart = true;
        }
        else
        {
            if (playerTurn)
            {
                HandlePlayerInput();
            }
            else
            {
                if (!pendingPlay)
                {
                    Invoke("HandleCPUTurn", 0.5f);
                    pendingPlay = true;
                }
            }
            if(Board.CheckWinConditions(Board.instance.grid, 1))
            {
                PlayerWin();
                pendingRestart = true;
            }
            if (Board.CheckWinConditions(Board.instance.grid, 2))
            {
                CpuWin();
                pendingRestart = true;
            }

        }
    }

    void InitGame()
    {
        Board.instance.InitGrid();
        movesPlayed = 0;
        difficulty = pendingDiff;
        pendingRestart = false;
        pendingPlay = false;
        diffWarning.SetActive(false);
        DecideWhoGoesFirst();
    }

    void DecideWhoGoesFirst()
    {
        if (Random.Range(0, 2).Equals(1))
            playerTurn = true;
        else
            playerTurn = false;
    }

    void HandlePlayerInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int mousePos = Board.instance.gridUI.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if ((mousePos.x >= 0 && mousePos.x <= 2) && (-mousePos.y >= 0 && -mousePos.y <= 2))
            {
                if (Board.instance.grid[-mousePos.y, mousePos.x] == 0)
                {

                    Board.instance.Draw(Symbol.Cross, mousePos.x, mousePos.y);
                    playerTurn = false;
                    movesPlayed++;
                }
            }
        }
    }

    void HandleCPUTurn()
    {
        cpu.GenerateCPUMove(Board.instance.grid, difficulty, movesPlayed);
        playerTurn = true;
        pendingPlay = false;
        movesPlayed++;
    }

    void PlayerWin()
    {
        playerWinCount++;
        scoreBoards[0].GetComponent<Text>().text = "Player: " + playerWinCount.ToString();
        Invoke("InitGame", 2);
    }

    void CpuWin()
    {
        cpuWinCount++;
        scoreBoards[1].GetComponent<Text>().text = "CPU: " + cpuWinCount.ToString();
        Invoke("InitGame", 2);
    }

    void Tie()
    {
        tieCount++;
        scoreBoards[2].GetComponent<Text>().text = "Tie: " + tieCount.ToString();
        Invoke("InitGame", 2);
    }

    public void ChangeDifficulty()
    {
        Text diffText = diffButton.GetComponentsInChildren<Text>()[0];
        diffWarning.SetActive(true);
        diffButton.enabled = false;
        diffButton.enabled = true;
        switch (pendingDiff)
        {
            case 6:
                diffText.text = "Medium";
                pendingDiff = 3;
                break;
            case 3:
                diffText.text = "Impossible";
                pendingDiff = 1;
                break;
            case 1:
                diffText.text = "Easy";
                pendingDiff = 6;
                break;
        }
    }
}
