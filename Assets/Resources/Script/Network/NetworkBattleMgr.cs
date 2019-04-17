﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkBattleMgr : Photon.PunBehaviour {

    public ConfigBattle configBattle;
    public bool preparing = true;
    public float gameTime = 0f;
    public int lastKillCamp = 0;
    public Dictionary<int, int> score = new Dictionary<int, int>();
    Timer timer = new Timer();
    public BattleInfoMgr battleInfo;
    public PanelSwitch menuPanel;
    public ScoreboardMgr scoreboard;

    [Header("Event")]
    public NetworkEvent OnPrepareEnd;
    public NetworkEvent OnGameOver;

    private void Awake()
    {
        Battle.battleMgr = this;
        score.Add(1, 0);
        score.Add(2, 0);
    }

    void Start () {
        StartCoroutine(ShowPlayer());
        Battle.started = true;

        battleInfo.SetTarget(configBattle.targetKill);
        battleInfo.SetTimer(configBattle.prepareTime);

        timer.Enter(configBattle.prepareTime);

        timer.OnComplet = PrepareEnd;
    }

    private void Update()
    {
        if (!this.preparing)
        {
            if (Input.GetKeyDown("tab"))
            {
                scoreboard.SwitchPanel(true);
            }
            else if (Input.GetKeyUp("tab"))
            {
                scoreboard.SwitchPanel(false);
            }

            if (Input.GetKeyDown("escape"))
            {
                menuPanel.SwitchPanel();
            }
        }

        if (timer.isRunning && this.preparing)
        {
            battleInfo.SetTimer(configBattle.prepareTime * (1 - timer.rate));
        }
    }

    void FixedUpdate()
    {
        timer.FixedUpdate();

        if (!preparing)
        {
            gameTime += Time.fixedDeltaTime;
            battleInfo.SetTimer(gameTime);
            if (gameTime >= configBattle.outTime)
            {
                GameEnd();
            }
        }


    }

    IEnumerator Minwayjoin()
    {
        yield return new WaitForSeconds(2);
        Battle.localPlayerBattleInfo.SetPlayerEnable(true);
    }

    IEnumerator ShowPlayer()
    {
        yield return new WaitForSeconds(1);

        Battle.localPlayerCamp = int.Parse(PhotonNetwork.player.CustomProperties["team"].ToString());
       
        var t = Battle.bornMgr.GetPoint(Battle.localPlayerCamp);
        PhotonNetwork.Instantiate("NetworkCreator", t.position, t.rotation, 0);
        
    }

    public void AddScore(int camp)
    {
        score[camp]++;
        battleInfo.SetScore(camp, score[camp]);
        lastKillCamp = camp;
        this.CheckFinishCondition();
    }

    public void PrepareEnd()
    {
        PrepareEnd(false);
    }

    public void PrepareEnd(bool midway)
    {
        OnPrepareEnd.Invoke();
        preparing = false;
        if (!midway)
        {
            Battle.localPlayerBattleInfo.SetPlayerEnable(true);
        }
        Battle.UpdateFriendlyMark();
    }

    public void GameEnd()
    {

        this.preparing = true;
        Battle.localPlayerBattleInfo.SetPlayerEnable(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        OnGameOver.Invoke();
    }

    private void CheckFinishCondition()
    {
        if (score[1] >= configBattle.targetKill || score[2] >= configBattle.targetKill)
        {
            GameEnd();
        };

    }
    
    public void BackToMainStage(bool isLeave)
    {
        Battle.ClearBattlefield();
        Battle.started = false;
        if (isLeave)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.LoadLevelAsync("MainStage");
            }
        }
    }

    public void SetSync(float time, bool prepare, int score1, int score2)
    {
        this.preparing = prepare;
        
        if (prepare)
        {
            var temp = configBattle.prepareTime * time;
            battleInfo.SetTimer(temp);
            timer.Enter(temp);
        }
        else
        {
            score[1] = score1;
            score[2] = score2;

            battleInfo.SetScore(1, score1);
            battleInfo.SetScore(2, score2);

            PrepareEnd(true);
            StartCoroutine(Minwayjoin());
            gameTime = time;
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainStage");
        Battle.inRoom = false;
        Battle.started = false;
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.isMasterClient)
        {
            Battle.localPlayerBattleInfo.photonView.RPC("SyncBattleMgr", PhotonTargets.Others, this.preparing ? timer.rate : gameTime, this.preparing, score[1], score[2]);
        }
    }

}
