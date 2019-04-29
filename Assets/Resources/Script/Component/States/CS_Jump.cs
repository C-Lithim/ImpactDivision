﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(CS_StateMgr))]
public class CS_Jump : AvatarState {

    [Header("[Components]")]
    C_Camera _camera;
    C_Velocity _velocity;
    C_Animator _animator;
    C_Attributes _attributes;
    C_OnGroundSensor _onGroundSensor;
    AudioSource _audioSource;
    CharacterController _characterController;

    [Header("[Extra Properties]")]
    public Timer timer = new Timer();
    public float activeTime = 0.4f;
    public int part = 1;
    public float gravity = 20f;
    public float force = 5f;
    public int process = 1;
    public AudioClip[] sounds;


    private void OnEnable()
    {

        _camera = GetComponent<C_Camera>();
        _velocity = GetComponent<C_Velocity>();
        _animator = GetComponent<C_Animator>();
        _attributes = GetComponent<C_Attributes>();
        _audioSource = GetComponent<AudioSource>();
        _onGroundSensor = GetComponent<C_OnGroundSensor>();
        _characterController = GetComponent<CharacterController>();

        var stateMgr = GetComponent<CS_StateMgr>();
        //_name = "aim";
        stateMgr.RegState(_name, this);
        
    }
    
    public override bool Listener() {

        if (_velocity.Djump)
        {
            return _onGroundSensor.isGrounded;
        }

        return false;
    }

    public override void Enter()
    {
        base.Enter();
        if (_velocity.crouch)
        {
            _velocity.crouch = false;
            _animator.AddEvent("crouch", 0f);
            this._exitTick = true;
        }
        else
        {
            Sound.PlayOneShot(_audioSource, sounds);
            if (_velocity.isLocalPlayer)
            {
                timer.Enter(activeTime);
                process = 1;
                force = _attributes.jumpForce;
            }
            _animator.animator.SetBool("jump", true);
            _velocity.jumping = true;
        }
    }

    public override void OnUpdate() {


        if (!_attributes.isDead)
        {
            
            if (_velocity.isLocalPlayer)
            {
                timer.Update();
                var currentSpeed = _velocity.currentSpeed;

                Aspect.RotateToCameraY(_camera.Carryer, transform, 0.5f);
                _characterController.Move(
                    transform.forward * currentSpeed * _attributes.rate * _velocity.fwd * Time.deltaTime +
                    transform.right * currentSpeed * _attributes.rate * _velocity.right * Time.deltaTime +
                    transform.up * force * _attributes.rate * Time.deltaTime
                );

                force -= (gravity * Time.deltaTime);

                // 二段跳 （弃用）
                //if (process == 1 && _velocity.Djump && !timer.isRunning)
                //{
                //    process = 2;
                //    if (force < 0)
                //    {
                //        force = _attributes.jumpForce2;
                //    }
                //    else
                //    {
                //        force += _attributes.jumpForce2;
                //    }
                //    this.pView.RPC("NetworkDoubleJump", PhotonTargets.All);
                //}

                if (force < 0 && _onGroundSensor.isGrounded)
                {
                    this._exitTick = true;
                    Sound.PlayOneShot(_audioSource, sounds);
                }
            }
        }
       
    }

    public override void Exit() {

        base.Exit();
        _animator.animator.SetBool("jump", false);
        _velocity.jumping = false;
    }
    
    [PunRPC]
    public void NetworkDoubleJump()
    {
        _animator.animator.SetTrigger("tjump");
    }
    
}
           
