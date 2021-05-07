using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CloudRenderer : MonoBehaviour
{
    Texture2D texColor;
    Texture2D texPosScale;
    VisualEffect vfx;
    uint resolution = 1024;
    public float particleSize = 0.1f;
    bool toUpdate = false;
    uint particleCount = 0;

    private void Start()
    {
        vfx = GetComponent<VisualEffect>();

        Vector3[] positions = new Vector3[(int)resolution * (int)resolution];
        Color[] colours = new Color[(int)resolution * (int)resolution];
        for (int x = 0; x < (int)resolution; x++)
        {
            for (int y = 0; y < (int)resolution; y++)
            {
                positions[x + y * (int)resolution] = new Vector3(Random.value * 10, Random.value * 10, Random.value * 10);
                colours[x + y * (int)resolution] = new Color(Random.value, Random.value, Random.value, 1);
            }
        }

        SetParticles(positions, colours);
    }

    private void Update()
    {
        if (toUpdate)
        {
            toUpdate = false;
            vfx.Reinit();
            vfx.SetUInt(Shader.PropertyToID("ParticleCount"), particleCount);
            vfx.SetTexture(Shader.PropertyToID("TexColor"), texColor);
            vfx.SetTexture(Shader.PropertyToID("TexPosScale"), texPosScale);
            vfx.SetUInt(Shader.PropertyToID("Resolution"), resolution);
        }
    }

    public void SetParticles(Vector3[] positions, Color[] colours)
    {
        texColor = new Texture2D(positions.Length > (int)resolution ? (int)resolution : positions.Length, Mathf.Clamp(positions.Length / (int)resolution, 1, (int)resolution), TextureFormat.RGBAFloat, false);
        texPosScale = new Texture2D(positions.Length > (int)resolution ? (int)resolution : positions.Length, Mathf.Clamp(positions.Length / (int)resolution, 1, (int)resolution), TextureFormat.RGBAFloat, false);
        int texWidth = texColor.width;
        int texHeight = texColor.height;

        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                int index = x + y * texWidth;
                texColor.SetPixel(x, y, colours[index]);
                var data = new Color(positions[index].x, positions[index].y, positions[index].z, particleSize);
                texPosScale.SetPixel(x, y, data);
            }
        }

        texPosScale.Apply();
        texColor.Apply();

        particleCount = (uint)positions.Length;
        toUpdate = true;
    }
}
