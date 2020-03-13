using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechArrowManager : MonoBehaviour
{
    public static MechArrowManager instance;

    private List<MechArrow> mechArrows = new List<MechArrow>();
    private GameObject player;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        player = GameObject.Find("Character");
    }

    private void FixedUpdate()
    {
        foreach (MechArrow mechArrow in mechArrows)
        {
            float posY = player.transform.position.y;
            if (mechArrow.transform.position.y < posY - 15f ||
                mechArrow.transform.position.y > posY + 25f)
                mechArrow.SetActive(false);
            else mechArrow.SetActive(true);
        }
    }

    public void Assign(MechArrow mechArrow)
    {
        mechArrows.Add(mechArrow);
    }
}
