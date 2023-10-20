using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }

    [SerializeField, Tooltip("The canvas for the loading screen.")] private GameObject loaderCanvas;
    [SerializeField, Tooltip("The loading progress bar.")] private Image progressBar;
    private float target;
    private float loadMaxDelta = 3f;
    private bool loadingScene = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        AudioManager = GetComponentInChildren<AudioManager>();

    }

    /// <summary>
    /// Loads the scene asynchronously with a loading screen.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public async void LoadScene(string sceneName)
    {
        target = 0f;
        progressBar.fillAmount = 0f;
        loadingScene = true;

        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        loaderCanvas?.SetActive(true);

        do
        {
            await Task.Delay(100);
            target = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(500);

        scene.allowSceneActivation = true;
        loaderCanvas?.SetActive(false);
        loadingScene = false;
        GameManager.Instance?.AudioManager.StopAllSounds();
        SceneManager.sceneLoaded += OnSceneLoaded;
   

    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("TEST");
        switch(scene.buildIndex) {
            case 1:
                Debug.Log("level 1");
            break;
            case 2:
                Debug.Log("level 2");
            break;
            default:
                Debug.Log("TITLE");
                        GameManager.Instance?.AudioManager.StopAllSounds();
                                GameManager.Instance?.AudioManager.Play(AudioManager.GameSound.Sound.TitlescreenMusic);
                                break;
        }
    }

    private void Update()
    {
        if(loadingScene)
            progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, target, loadMaxDelta * Time.deltaTime);
    }
}
