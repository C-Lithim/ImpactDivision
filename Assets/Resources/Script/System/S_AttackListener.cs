﻿using Unity.Entities;
using System;
using UnityEngine;
using UiEvent;

public class S_AttackListener : ComponentSystem {
	struct Group{
		public C_AttackListener _AttackListener;
        public C_Velocity _Velocity;
        public C_Attributes _Attributes;
        public C_BattleMgr _BattleMgr;
        public AudioSource _Audio;
    }

    int killerRoomID = 0;

	protected override void OnUpdate()
	{
		foreach (var e in GetEntities<Group>())
		{
            var _attackListener = e._AttackListener;
            var _attackList = e._AttackListener.attackList;
            var _attribute = e._Attributes;
            var _velocity = e._Velocity;
            if (e._AttackListener.isActive)
            {
                foreach (Attack attack in _attackList)
                {
                    _attackListener.photonView.RPC("Demaged", PhotonTargets.All, attack.demage);
                    
                    var source = attack.source;
                    var sourceAudio = source.GetComponent<AudioSource>();
                    var sourceBattleMgr = source.GetComponent<C_BattleMgr>();

                    sourceBattleMgr.photonView.RPC("AddDemage", PhotonTargets.All, attack.demage);

                    var hitMsg = new UiEvent.UiMsgs.Hit()
                    {
                        HeadShot = attack.headShot
                    };
                    source.GetComponent<C_UiEventMgr>().SendEvent(hitMsg);
                    Sound.PlayOneShot(sourceAudio, _attackListener.hitFeedBackSounds);

                    if (_attribute.HP <= 0)
                    {
                        sourceBattleMgr.photonView.RPC("AddKill", PhotonTargets.All);
                        Sound.PlayOneShot(sourceAudio, _attackListener.killSound);
                        
                        foreach (var item in _attackListener.sourceList.Values)
                        {
                            if (sourceBattleMgr.roomID != item.roomID)
                            {
                                item.photonView.RPC("AddAssists", PhotonTargets.All);
                            }
                        }

                        break;
                    }

                    _attackListener.photonView.RPC("AddAttackSource", PhotonTargets.All, attack.source.roomID);
                }
                
                _attackListener.attackList.Clear();
            }
        }
    }
}
