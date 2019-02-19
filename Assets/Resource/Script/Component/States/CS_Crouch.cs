﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(CS_StateMgr))]
public class CS_Crouch : AvatarState {

    [Header("[Components]")]
    public C_Camera _camera;
    public C_Animator _animator;
    public C_Velocity _velocity;
    public CS_StateMgr _stateMgr;
    public C_Attributes _attributes;
    public AudioSource _audioSource;
    public CharacterController _characterController;

    [Header("[Extra Properties]")]
    public float speed;
    public AudioClip[] sounds;


    private void OnEnable()
    {
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
        _velocity.currentSpeed = speed;
        _animator.AddEvent("crouch", 1f);
        _velocity.crouch = true;
        _velocity.Dcrouch = false;
    }

    public override void OnUpdate() {
        _animator.AddEvent("Dfwd", _velocity.fwd);
        _animator.AddEvent("Dright", _velocity.right);

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

    public override void Exit()
    {
        _velocity.crouch = false;
        _animator.AddEvent("Dfwd", 0);
        _animator.AddEvent("Dright", 0);
        _animator.AddEvent("crouch", 0f);
    }
    
    
}
           
