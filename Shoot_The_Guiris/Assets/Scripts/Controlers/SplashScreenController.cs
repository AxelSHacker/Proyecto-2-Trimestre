using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenController : MonoBehaviour
{
    //Esacna a la que iremos despues del splash
    public string sceneAftrSplash = "Main Menu";
    //Tiempo de duracion de la pantalla de splash
    [Range(1, 5)]
    public float splashDuration;
    //Referencia a la imagen de fade
    public Image fadeImage;
    //Gradiiente üpara configurar las duracion del fade
    public Gradient fadeColorGradiaent;

    public AnimationCurve fadeCurve;

    private float timeCounter = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Color color = fadeColorGradiaent.Evaluate(0);
        //fadeImage.color = color;

        float alpha = fadeCurve.Evaluate(0);
        Color imageColor = fadeImage.color;
        imageColor.a = alpha;
        fadeImage.color = imageColor;
    }

    // Update is called once per frame
    void Update()
    {
        timeCounter += Time.deltaTime;
        //Color color = fadeColorGradiaent.Evaluate(timeCounter / splashDuration);
        //fadeImage.color = color;

        float alpha = fadeCurve.Evaluate(timeCounter / splashDuration);
        Color imageColor = fadeImage.color;
        imageColor.a = alpha;
        fadeImage.color = imageColor;

        if (timeCounter >= splashDuration)
        {
            SceneManager.LoadScene(sceneAftrSplash);
        }
    }
}
