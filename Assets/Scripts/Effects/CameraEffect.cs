using UnityEngine;
using System.Collections;

public class CameraEffect : MonoBehaviour
{
    private Material mat;			//Creatses material for image effect shader

    private Shader s;				//Creates empty shader for image effects
    private void Start()
    {
        s = Shader.Find("Example/DizzyVignette");
        mat = new Material(s);
    }
    public void RenderImage(RenderTexture source, RenderTexture destination, float distance, float power)
    {
        mat.SetFloat("_DizzyDist", distance);
        mat.SetFloat("_VignettePower", (1 - power) + 0.5f);
        mat.SetFloat("_Timer", Time.time);
        Graphics.Blit(source, destination, mat);
    }

}