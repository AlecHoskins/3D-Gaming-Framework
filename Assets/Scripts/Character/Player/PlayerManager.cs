using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    PlayerLocomotionManager playerLocomotionManager;
    protected override void Awake()
    {
        base.Awake();

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
    }

    protected override void Update()
    {
        base.Update();

        //so we cant control or edit game objects we don't own
        if (!IsOwner)
        {
            return;
        }
        playerLocomotionManager.HandleAllMovement();
    }
}
