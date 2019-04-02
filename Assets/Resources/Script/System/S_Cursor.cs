﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class S_Cursor : ComponentSystem {
	struct Group{
		public C_Cursor _Cursor;
		public C_Velocity _Velocity;
	}

	protected override void OnUpdate()
	{
		foreach (var e in GetEntities<Group>())
		{
            var _velocity = e._Velocity;
            if (_velocity.isLocalPlayer)
            {
                var _cursor = e._Cursor;
                if (_velocity.Dback)
                {
                    Debug.Log("Cursor running");
                    _cursor.isLocked = !_cursor.isLocked;
                    Cursor.lockState = _cursor.isLocked ? CursorLockMode.Locked : CursorLockMode.None;
                    Cursor.visible = !_cursor.isLocked;
                }
              
            }
        }
    }
	

}
