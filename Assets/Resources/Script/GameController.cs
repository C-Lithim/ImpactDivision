﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UiEvent;

public class GameController : MonoBehaviour {
    public bool isLocal;
    public int camp;
    public GameObject avatar;
    public ConfigWeapon mainWeapon;
    public ConfigWeapon secondWeapon;

    public List<AvatarCreator> creators;
    public List<GameObject> avatars = new List<GameObject>();

    public HUDMgr hudMgr;
    public PlaneHUDMgr PlaneHUDMgr;
    
    void Awake()
    {
        Application.targetFrameRate = 60;
        
        var avatarObj = Factory.CreateAvatar(avatar, camp, isLocal, transform.position, transform.rotation, mainWeapon, secondWeapon);
        var uiEventMgr = avatarObj.GetComponent<C_UiEventMgr>();
        hudMgr.Init(uiEventMgr);
        PlaneHUDMgr.Init(uiEventMgr);
        foreach (var item in creators)
        {
            var avatar = item.Create();
            var _attr = avatar.GetComponent<C_Attributes>();
            var _uiMgr = avatar.GetComponent<C_UiEventMgr>();
            // 添加头顶血条
            var tinyHpBar = GameObject.Instantiate(Resources.Load("Arts/Prefab/UI/TinyHpBarCanvas")) as GameObject;
            tinyHpBar.GetComponent<TinyHpBarMgr>().Init(_attr, _uiMgr);
            tinyHpBar.GetComponent<GameObjectEntity>().enabled = true;

            avatars.Add(avatar);
        }

    }
    
}
