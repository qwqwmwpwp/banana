using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyCommon
{
    public abstract class EnemyCommonStateBase : StateBase
    {
        public Transform playerTransform = PlayerManager.instance.player.transform;
        public Transform transform;
        public Rigidbody2D rb;
    }
}