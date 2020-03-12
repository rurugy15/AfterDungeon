using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    public static ArrowManager instance;

    [SerializeField] private GameObject arrowPrefab;

    private List<GameObject> arrows = new List<GameObject>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void Shoot(Vector2 shootPos, float speed, bool isFacingRight)
    {
        GameObject arrow = FindDeactivateArrow();
        arrow.GetComponent<ArrowController>().Initiailize(shootPos, speed, isFacingRight);
    }

    private GameObject FindDeactivateArrow()
    {
        for (int i = 0; i < arrows.Count; i++)
        {
            if (arrows[i].activeInHierarchy == false) return arrows[i];
        }

        GameObject arrow = Instantiate(arrowPrefab, new Vector2(999, 999), Quaternion.identity);
        arrow.name = "Arrow " + arrows.Count;
        arrows.Add(arrow);
        return arrow;
    }

    public void DeactivateAllArrow()
    {
        foreach (GameObject arrow in arrows)
        {
            arrow.transform.parent = null;
            arrow.SetActive(false);
        }
    }
}
