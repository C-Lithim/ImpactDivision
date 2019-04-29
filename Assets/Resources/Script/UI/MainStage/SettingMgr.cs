﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMgr : WindowBasic
{
    public bool active = true;
    public Button applyBtn;
    public Button backBtn;
    public List<GameObject> settingList;
    public GameObject confirmWindow;
    public SoundMgr soundMgr;
    public ControlSettingMgr controlMgr;
    public VideoMgr videoMgr;
    
    public NetworkEvent OnClosePanel;

    // Use this for initialization
    public override void Init () {
        
        soundMgr.Init();
        controlMgr.Init();
        videoMgr.Init();

        applyBtn.interactable = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) && this.active)
        {
            backBtn.onClick.Invoke();
        }
    }

    public void OpenSetting(int index)
    {
        for (int i = 0; i < settingList.Count; i++)
        {
            if (i == index)
                settingList[i].SetActive(true);
            else
                settingList[i].SetActive(false);
        }
    }

    public void ApplyChanges()
    {
        soundMgr.ApplyChanges();
        videoMgr.ApplyChanges();

        Battle.SaveSystemSettingData();

        applyBtn.interactable = false;
    }

    public void Back()
    {
        if (applyBtn.interactable)
        {
            confirmWindow.SetActive(true);
        }
        else
        {
            OnClosePanel.Invoke();
        }
    }

    public void ResetSetting()
    {
        videoMgr.ResetSetting();
        soundMgr.InitSetting();
        applyBtn.interactable = false;
    }
}
