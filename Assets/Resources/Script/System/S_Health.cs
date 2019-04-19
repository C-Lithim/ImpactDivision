﻿using Unity.Entities;
using UnityEngine;
using UiEvent;

public class S_Health : ComponentSystem {
    struct Group {
        public C_AttackListener _AttackListener;
        public C_Attributes _Attributes;
        public CS_StateMgr _StateMgr;
        public C_BattleMgr _BattleMgr;
    }

    protected override void OnUpdate()
    {
        foreach (var e in GetEntities<Group>())
        {
            var _attribute = e._Attributes;

            if (!_attribute.isDead) {

                if (e._Attributes.HP > 300f)
                {
                    e._Attributes.HP = 1;
                }
                if (e._Attributes.HP > 320f)
                {
                    e._Attributes.HP = 1;
                }

                if (e._Attributes.HP <= 0 && e._AttackListener.isActive)
                {
                    e._StateMgr.EnterState("dead");
                    e._BattleMgr.AddDead();
                }
            }
        }
    }
    
}
