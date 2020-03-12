using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlodgingPlatform : ContactArrow
{
    public override void OnLodgingEnterAction(GameObject arrow)
    {
        arrow.GetComponent<ArrowController>().Disable();
    }

    public override void OnLodgingExitAction(GameObject arrow)
    {
    }

    public override void OnLodgingStayAction(GameObject arrow)
    {
    }
}
