using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public static bool paused;
    public float pauseVolume = -40f;
    public PlayerMovement pm;
    public Slider masterSlider, effectsSlider, playerSlider, musicSlider, fovSlider, sensSlider;
    public AudioMixer master;
    public AudioMixerGroup effectsMixer, playerMixer, musicMixer;
    public GameObject pauseMenu, optionsMenu, videoM, audioM;
    public KeyCode pauseKey;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(pauseKey) && !paused)
        {
            PauseGame();
        }
        else if (Input.GetKeyDown(pauseKey) && paused)
        {
            ResumeGame();
        }

        if(paused)
        {
            Time.timeScale = 0f;
            master.SetFloat("Volume", pauseVolume);
            effectsMixer.audioMixer.SetFloat("EffectsVolume", pauseVolume);
            playerMixer.audioMixer.SetFloat("PlayerVolume", pauseVolume);
            musicMixer.audioMixer.SetFloat("MusicVolume", pauseVolume);
        }
        else
        {
            master.SetFloat("Volume", masterSlider.value);
            effectsMixer.audioMixer.SetFloat("EffectsVolume", effectsSlider.value);
            playerMixer.audioMixer.SetFloat("PlayerVolume", playerSlider.value);
            musicMixer.audioMixer.SetFloat("MusicVolume", musicSlider.value);
            pm.currentCamFov = fovSlider.value;
            pm.Cam.GetComponent<CameraMove>().sensX = sensSlider.value;
            pm.Cam.GetComponent<CameraMove>().sensY = sensSlider.value;
        }
    }

    public void PauseGame()
    {
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        pm.Cam.GetComponent<Camera>().fieldOfView = fovSlider.value;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        audioM.SetActive(false);
        videoM.SetActive(false);
    }

    public void DisableMenu(GameObject menu)
    {
        menu.SetActive(false);
    }
    public void EnableMenu(GameObject menu)
    {
        menu.SetActive(true);
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
        paused = false;
    }
}
