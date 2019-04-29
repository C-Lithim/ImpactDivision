﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class S_Input : ComponentSystem {
	struct Group{
		public C_Input _Input;
		public C_Velocity _Velocity;
        public C_Cursor _Cursor;
	}

	protected override void OnUpdate()
	{
		foreach (var e in GetEntities<Group>())
		{
            var _velocity = e._Velocity;
            var _input = e._Input;

            _velocity.Dback = Input.GetKeyDown(_input.back);
            if (_velocity.isActive && e._Cursor.isLocked)
            {
                _velocity.Dfwd = Input.GetKey(_input.fwd);
                _velocity.Dbwd = Input.GetKey(_input.bwd);
                _velocity.Dleft = Input.GetKey(_input.left);
                _velocity.Dright = Input.GetKey(_input.right);
                _velocity.Djump = Input.GetKeyDown(_input.jump);
                _velocity.Dmouse_x = Input.GetAxis(_input.mouse_x);
                _velocity.Dmouse_y = Input.GetAxis(_input.mouse_y);
                _velocity.Dflash = Input.GetKey(_input.flash);
                _velocity.DfireHold = Input.GetKey(_input.openFire);
                _velocity.DfirePressed = Input.GetKeyDown(_input.openFire);
                _velocity.Dreload = Input.GetKey(_input.reload);
                _velocity.Daim = Input.GetKey(_input.aim);
                _velocity.DswitchWeapon = Input.GetKeyDown(_input.switchWeapon);
                if (!_velocity.DswitchWeapon)
                {
                    _velocity.DswitchWeapon = Input.GetAxis(_input.switchWeapon2) != 0;
                }
                _velocity.DpickRifle = Input.GetKeyDown(_input.pickRifle);
                _velocity.DpickPistol = Input.GetKeyDown(_input.pickPistol);
                _velocity.Dcombat = Input.GetKeyDown(_input.combat);
                _velocity.DcutCameraSide = Input.GetKeyDown(_input.cutCameraSide);
                if (Input.GetKeyDown(_input.crouch))
                {
                    _velocity.Dcrouch = !_velocity.Dcrouch;
                }
                if (Input.GetKeyDown(_input.run))
                {
                    _velocity.Drun = !_velocity.Drun;
                }

            }

        }
    }
	

}
