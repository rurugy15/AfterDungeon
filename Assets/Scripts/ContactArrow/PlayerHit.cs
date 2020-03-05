using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : ContactArrow
{
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    public override void OnLodgingEnterAction(GameObject arrow)
    {
        player.Die();
    }

    public override void OnLodgingExitAction(GameObject arrow)
    {
    }

    public override void OnLodgingStayAction(GameObject arrow)
    {
    }
}
