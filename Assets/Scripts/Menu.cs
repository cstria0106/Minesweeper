using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public InputField widthField;
    public InputField heightField;
    public InputField mineField;

    public void StartButton()
    {
        int width = int.Parse(widthField.text);
        int height = int.Parse(heightField.text);
        int mine = int.Parse(mineField.text);
        Game.Instance.Load(width, height, mine);
    }
}
