using Redsilver2.Core;
using Redsilver2.Core.Counters;
using Redsilver2.Core.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI creditsDisplayer;

    [Space]
    [SerializeField] private string[] creditsTexts;
    [SerializeField] private float duration = 5f;

    [Space]
    [SerializeField] private bool playOnStart = false;
    private IEnumerator playCreditCoroutine;

    private void Start()
    {
        if(creditsDisplayer != null)
        {
            creditsDisplayer.text = string.Empty;
            creditsDisplayer.canvasRenderer.SetAlpha(0f);
        }


        if (playOnStart)
        {
            Play();
        }
    }

    public void Play()
    {
        playCreditCoroutine = PlayCreditsCoroutine();
        StartCoroutine(playCreditCoroutine);
    }

    public void Stop()
    {
        if (playCreditCoroutine != null) StopCoroutine(playCreditCoroutine);
    }
   
    private IEnumerator PlayCreditsCoroutine()
    {
        CanvasRenderer     canvasRenderer = creditsDisplayer.canvasRenderer;
        SceneLoaderManager sceneLoaderManager = SceneLoaderManager.Instance;


        if (canvasRenderer != null && creditsTexts.Length > 0) 
        {
            float duration = this.duration / creditsTexts.Length;

            if(canvasRenderer.GetAlpha() != 0f)
            {
                yield return canvasRenderer.Fade(false, 1.5f);
            }

            while (SceneLoaderManager.IsLoadingSingleScene) yield return null;

            for (int i = 0; i < creditsTexts.Length; i++)
            {
                creditsDisplayer.text = creditsTexts[i];
                yield return canvasRenderer.Fade(true, 1.5f);
                yield return Counter.WaitForSeconds(duration);
                yield return canvasRenderer.Fade(false, 1.5f);
            }
        }

        if (sceneLoaderManager != null && SceneLoaderManager.SelectedSingleLevelIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            sceneLoaderManager.LoadSingleScene(0);
        }
    }
}
