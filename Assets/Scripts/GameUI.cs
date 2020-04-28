using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public Image soundButton;

    public void Quit()
    {
        Destroy(Game.Instance.gameObject);
        SceneManager.LoadScene("Menu");
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
