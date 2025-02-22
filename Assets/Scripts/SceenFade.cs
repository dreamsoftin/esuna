﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceenFade : MonoBehaviour
{

    public float fadeTime = 2.0f;
    public Color fadeColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public Shader fadeShader = null;

    private Material fadeMaterial = null;
    private bool isFading = false;

    void Awake()
    {
        fadeMaterial = (fadeShader != null) ? new Material(fadeShader) : new Material(Shader.Find("Sprites/Default"));
    }

    void OnEnable()
    {
        StartCoroutine(FadeIn());
    }

    public void LoadSceenWithFade(string sceenname)
    {
        StartCoroutine(FadeOut(sceenname));
    }

    void OnDestroy()
    {
        if (fadeMaterial != null)
        {
            Destroy(fadeMaterial);
        }
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0.0f;
        Color color = fadeMaterial.color = fadeColor;
        isFading = true;
        while (elapsedTime < fadeTime)
        {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
            color.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            fadeMaterial.color = color;
        }
        isFading = false;
    }

    IEnumerator FadeOut(string sceen)
    {
        float elapsedTime = 0.0f;
        fadeColor.a = 0f;
        Color color = fadeMaterial.color = fadeColor;
        isFading = true;
        while (elapsedTime < fadeTime)
        {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
            color.a = 0.0f + Mathf.Clamp01(elapsedTime / fadeTime);
            fadeMaterial.color = color;
        }
        SceneManager.LoadScene(sceen);
        isFading = false;
    }

    void OnPostRender()
    {
        if (isFading)
        {
            fadeMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Color(fadeMaterial.color);
            GL.Begin(GL.QUADS);
            GL.Vertex3(0f, 0f, -12f);
            GL.Vertex3(0f, 1f, -12f);
            GL.Vertex3(1f, 1f, -12f);
            GL.Vertex3(1f, 0f, -12f);
            GL.End();
            GL.PopMatrix();
        }
    }
}