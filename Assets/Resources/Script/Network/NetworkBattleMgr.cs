﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBattleMgr : Photon.PunBehaviour {
	// Use this for initialization
	void Start () {
        StartCoroutine(ShowPlayer());
    }

    // Update is called once per frame
    void Update () {
		
	}

    IEnumerator ShowPlayer()
    {
        yield return new WaitForSeconds(2);

        Debug.LogWarning("read player team");
        Battle.localPlayerCamp = int.Parse(PhotonNetwork.player.CustomProperties["team"].ToString());

        Debug.LogWarning("read player team successed");

        var t = Battle.bornMgr.GetPoint(Battle.localPlayerCamp);
        PhotonNetwork.Instantiate("NetworkCreator", t.position, t.rotation, 0);

        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.IsVisible = true;
        }

    }

}
