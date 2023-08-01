using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinChooser : MonoBehaviour
{
    bool up = false;
    public Material[] gunMats;
    public GameObject[] gunChoices;
    public KeyCode menuPullup = KeyCode.U;
    public PlayerMovement pm;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(menuPullup) && !up)
        {
            Menu();
        }
        else if (Input.GetKeyDown(menuPullup) && up)
        {
            CloseMenu();
        }
    }

    void Menu()
    {
        up = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        int i = 0;
        foreach(GameObject choice in gunChoices)
        {
            if(i == pm.gunIndex)
            {
                choice.SetActive(true);
            }
            else
            {
                choice.SetActive(false);
            }

            i++;
        }
    }

    void CloseMenu()
    {
        up = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        foreach (GameObject choice in gunChoices)
        {
            choice.SetActive(false);
        }
    }

    public void Skin(Texture skin)
    {
        gunMats[pm.gunIndex].mainTexture = skin;
    }
}
