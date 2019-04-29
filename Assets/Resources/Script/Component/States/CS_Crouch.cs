﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UiEvent;

[RequireComponent(typeof(CS_StateMgr))]
public class CS_Crouch : AvatarState {

    [Header("[Components]")]
    C_Camera _camera;
    C_Animator _animator;
    C_UiEventMgr _uiMgr;
    C_Velocity _velocity;
    CS_StateMgr _stateMgr;
    C_Attributes _attributes;
    C_WeaponHandle _weaponHandle;
    AudioSource _audioSource;
    CharacterController _characterController;

    [Header("[Extra Properties]")]
    public float speed;
    public AudioClip[] sounds;
    
    private void Awake()
    {
        _uiMgr = GetComponent<C_UiEventMgr>();
        _weaponHandle = GetComponent<C_WeaponHandle>();
        _camera = GetComponent<C_Camera>();
        _animator = GetComponent<C_Animator>();
        _velocity = GetComponent<C_Velocity>();
        _attributes = GetComponent<C_Attributes>();
        _stateMgr = GetComponent<CS_StateMgr>();
        _audioSource = GetComponent<AudioSource>();
        _characterController = GetComponent<CharacterController>();

        var stateMgr = GetComponent<CS_StateMgr>();
        //_name = "aim";
        stateMgr.RegState(_name, this);
        
    }
    
    public override bool Listener() {
        if (_velocity.Dcrouch)
        {
            return true;
        }

        return false;
    }

    public override void Enter()
    {

        base.Enter();
        _velocity.Dcrouch = false;
        _velocity.currentSpeed = speed;
        _animator.AddEvent("crouch", 1f);
        _velocity.crouch = true;
        _velocity.Dcrouch = false;
        Sound.PlayOneShot(_audioSource, sounds);
        var att = _weaponHandle.weaponAttributes[_weaponHandle.currentWeapon];
        SendSpreadMsg(att.spread * att.crouchSpreadRate + 10f);
       
    }

    public override void OnUpdate() {
        _animator.AddEvent("Dfwd", _velocity.fwd);
        _animator.AddEvent("Dright", _velocity.right);
        if (_velocity.isLocalPlayer)
        {
            var currentSpeed = _velocity.currentSpeed;

            Aspect.RotateToCameraY(_camera.Carryer, transform, 0.5f);
            _characterController.Move(transform.forward * currentSpeed * _attributes.rate * _velocity.fwd * Time.deltaTime +
                                transform.right * currentSpeed * _attributes.rate * _velocity.right * Time.deltaTime);

            _characterController.SimpleMove(Vector3.zero);

            if (_velocity.Dcrouch || _velocity.Drun || _velocity.Djump)
            {
                _exitTick = true;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        Sound.PlayOneShot(_audioSource, sounds);
        _velocity.crouch = false;
        _velocity.Dcrouch = false;
        _animator.AddEvent("Dfwd", 0);
        _animator.AddEvent("Dright", 0);
        _animator.AddEvent("crouch", 0f);

        SendSpreadMsg(_weaponHandle.weaponAttributes[_weaponHandle.currentWeapon].spread + 10f);
    }

    void SendSpreadMsg(float v)
    {
        var spreadMsg = new UiEvent.UiMsgs.Spread()
        {
            value = v
        };
        _uiMgr.SendEvent(spreadMsg);
    }
}
           
