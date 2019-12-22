using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;

    private void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    private void Start()
    {
        menuPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(menuPanel.activeInHierarchy) menuPanel.SetActive(false);
            else menuPanel.SetActive(true);
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Tutorial()
    {
        SceneManager.LoadScene(0);
    }
}
