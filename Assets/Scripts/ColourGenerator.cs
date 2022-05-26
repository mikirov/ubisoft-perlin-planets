using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourGenerator
{

    ColourSettings settings;
    const int textureResolution = 50;

    private float MaxElevation = 0;

    public void UpdateSettings(ColourSettings settings, Texture2D texture)
    {
        this.settings = settings;
        if (texture == null)
        {
            texture = new Texture2D(textureResolution, 1);
        }
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));

        MaxElevation = elevationMinMax.Max;
    }

    public float GetMaxElevation()
    {
        return MaxElevation;
    }

    public void UpdateColours(Texture2D texture)
    {
        if (texture == null)
        {
            texture = new Texture2D(textureResolution, 1);
        }
        Color[] colours = new Color[textureResolution];
        for (int i = 0; i < textureResolution; i++)
        {
            colours[i] = settings.gradient.Evaluate(i / (textureResolution - 1f));
        }
        texture.SetPixels(colours);
        texture.Apply();
        settings.planetMaterial.SetTexture("_texture", texture);
    }
}
