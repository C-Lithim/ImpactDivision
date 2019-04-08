﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Dead : AvatarState {

    public C_Attributes attributes;
    public C_Velocity velocity;
    public C_Animator anim;
    public C_AttackListener attackListener;
    public C_IKManager iKManager;
    public C_WeaponHandle weaponHandle;
    public C_BattleMgr battleMgr;
    Timer timer = new Timer();
    public float recoverTime = 1f;
    public float vanishTime = 2;
    int process = 1;

    private void OnEnable()
    {
        attributes = GetComponent<C_Attributes>();
        velocity = GetComponent<C_Velocity>();
        anim = GetComponent<C_Animator>();
        attackListener = GetComponent<C_AttackListener>();
        iKManager = GetComponent<C_IKManager>();
        weaponHandle = GetComponent<C_WeaponHandle>();
        battleMgr = GetComponent<C_BattleMgr>();

        var stateMgr = GetComponent<CS_StateMgr>();
        //_name = "aim";
        stateMgr.RegState(_name, this);

    }

    public override void Enter()
    {
        base.Enter();
        process = 1;
        timer.Enter(vanishTime);

        attributes.isDead = true;
        velocity.isActive = false;
        anim.animator.enabled = false;
        attackListener.isActive = false;
        iKManager.SetAim(false);
        iKManager.SetHold(false);
        iKManager.SetDead(true);
        weaponHandle.Reset();
        weaponHandle.active = false;

        Debug.Log("enter dead state");
    }

    public override void OnUpdate()
    {
        timer.Update();
        if (!timer.isRunning)
        {
            if (process == 1)
            {
                iKManager.ragdollMgr.SetRagdollActive(false);
                this.transform.position = Battle.bornMgr.interimPoint.position;
                timer.Enter(recoverTime);
                process = 2;
            }
            else if (process == 2)
            {
                attributes.Recover();
                anim.animator.enabled = true;
                iKManager.SetDead(false);
                weaponHandle.targetWeapon = 1;

                timer.Enter();
                process = 3;
            }
            else if (process == 3)
            {
                this._exitTick = true;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        var t = Battle.bornMgr.GetPoint(attributes.camp);
        this.transform.SetPositionAndRotation(t.position, t.rotation);
        velocity.isActive = true;
        anim.animator.enabled = true;
        attackListener.Reset();
        attackListener.isActive = true;
        attributes.isDead = false;
        weaponHandle.active = true;
    }
}
