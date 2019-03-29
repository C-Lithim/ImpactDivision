﻿using UnityEngine;
using System.Collections;
using UnityEngine.Animations;

public class Aim : WeaponState
{
    [Header("[Components]")]
    public C_Velocity _velocity;
    public C_IKManager _iKManager;
    public C_WeaponHandle _weaponHandle;
    public C_Camera _camera;
    public WeaponAttribute _weaponAttribute;
    [Header("[Extra Properties]")]
    public float FOV;
    RaycastHit hitInfo;


    public override void Init(GameObject obj)
    {
        _velocity = obj.GetComponent<C_Velocity>();
        _iKManager = obj.GetComponent<C_IKManager>();
        _weaponHandle = obj.GetComponent<C_WeaponHandle>();
        _camera = obj.GetComponent<C_Camera>();
        _weaponAttribute = GetComponent<WeaponAttribute>();
    }

    public override bool Listener() {

        if (_velocity.Daim && !_velocity.jumping && !_weaponAttribute.reload && !_weaponHandle.locked)
        {
            return true; 
        }

        return false;
    }

    public override void Enter() {
        _iKManager.SetAim(true);
        _velocity.Drun = false;
        _velocity.aiming = true;
        _camera.FOVtarget = FOV;
    }

    public override void OnUpdate()
    {

        if (!_velocity.Daim || _velocity.jumping || _weaponHandle.locked)
        {
            this._exitTick = true;
        }
    }
    public override void Exit() {

        _camera.FOVtarget = _camera.FOVdefault;
        _velocity.aiming = false;
    }
}
 