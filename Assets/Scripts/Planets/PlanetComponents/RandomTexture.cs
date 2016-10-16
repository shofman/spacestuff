using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RandomTexture : MonoBehaviour {
    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;
    private System.Random generator;

    // public float scale = 6.0f;
    public float xOrg = 0.0f;
    public float yOrg = 0.0f;
    public float xOrg2 = 0.0f;
    public float yOrg2 = 0.0f;
    public int pixWidth = 128;
    public int pixHeight = 128;
    public bool checkedValue = false;

    void Awake() {
        rend = GetComponent<Renderer>();
        // noiseTex = new Texture2D(pixWidth, pixHeight);
        // pix = new Color[noiseTex.width * noiseTex.height];
        // rend.material.mainTexture = noiseTex;
        // generator = new System.Random();
        // xOrg = UnityEngine.Random.Range(-1000.0f, 1000.0f);
        // yOrg = UnityEngine.Random.Range(-1000.0f, 1000.0f);

        // xOrg2 = UnityEngine.Random.Range(-1000.0f, 1000.0f);
        // yOrg2 = UnityEngine.Random.Range(-1000.0f, 1000.0f);        
    }

    void Start () {
    }

    void calcNoise() {
        for (float y=0.0f; y<noiseTex.height; y++) {
            for (float x=0.0f; x<noiseTex.width; x++) {
                // THIS IS GOOD HERE
                float scale = 3f;
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                scale = 6f;
                xCoord = xOrg + x / noiseTex.width * scale;
                yCoord = yOrg + y / noiseTex.height * scale;
                sample += .5f * Mathf.PerlinNoise(xCoord, yCoord);

                scale = 30f;
                xCoord = xOrg + x / noiseTex.width * scale;
                yCoord = yOrg + y / noiseTex.height * scale;
                sample += .25f * Mathf.PerlinNoise(xCoord, yCoord);

                scale = 2f;
                xCoord = xOrg2 + x / noiseTex.width * scale;
                yCoord = yOrg2 + y / noiseTex.height * scale;
                float sample2 = Mathf.PerlinNoise(xCoord, yCoord);

                scale = 8f;
                xCoord = xOrg2 + x / noiseTex.width * scale;
                yCoord = yOrg2 + y / noiseTex.height * scale;
                sample2 += .5f * Mathf.PerlinNoise(xCoord, yCoord);

                scale = 20f;
                xCoord = xOrg2 + x / noiseTex.width * scale;
                yCoord = yOrg2 + y / noiseTex.height * scale;
                sample2 += .25f * Mathf.PerlinNoise(xCoord, yCoord);


                sample = (float)Math.Pow(sample, 2.0f);
                sample2 = (float)Math.Pow(sample2, 2.0f);
                // if (sample > max) {
                //     max = sample;
                // }
                // if (sample < min) {
                //     min = sample;
                // }
                // avg += sample;

                Color newColor2 = new Color(sample2, sample2, sample2);
                Color newColor = new Color(sample, sample, sample);

                newColor = biome(sample, sample2);
                pix[(int) (y * noiseTex.width + x)] = newColor;
            }
        }

        noiseTex.SetPixels(pix);
        noiseTex.Apply();
        GetComponent<Renderer>().material.mainTexture = noiseTex;
    }


    // Update is called once per frame
    void Update () {

    }

    public void createTexture() {
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.mainTexture = noiseTex;
        generator = new System.Random();
        xOrg = UnityEngine.Random.Range(-1000.0f, 1000.0f);
        yOrg = UnityEngine.Random.Range(-1000.0f, 1000.0f);

        xOrg2 = UnityEngine.Random.Range(-1000.0f, 1000.0f);
        yOrg2 = UnityEngine.Random.Range(-1000.0f, 1000.0f);

        calcNoise();

        // // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        // Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        
        // // set the pixel values
        // texture.SetPixel(0, 0, Color.red);
        // texture.SetPixel(1, 0, Color.clear);
        // texture.SetPixel(0, 1, Color.white);
        // texture.SetPixel(1, 1, Color.black);
        
        // Apply all SetPixel calls
        // texture.Apply();
        
        // connect texture to material of GameObject this script is attached to
        // GetComponent<Renderer>().material.mainTexture = texture;
    }

    private Color waterBiome(float heightMapValue, float moistureMapValue) {
      if (heightMapValue < 0.5) return Color.green; // Ocean
      if (heightMapValue < 0.55) return Color.yellow; // Beach
      
      if (heightMapValue > 0.8) {
        if (moistureMapValue < 0.1) return Color.black; // Scorched
        if (moistureMapValue < 0.2) return Color.grey; // Bare
        // if (moistureMapValue < 0.5) return TUNDRA;
        return Color.blue; //Snow
      }

      if (heightMapValue > 0.6) {
        if (moistureMapValue < 0.33) return new Color(250.0f/255.0f, 250.0f/255.0f, 210.0f/255.0f); //TEMPERATE_DESERT;
        // if (moistureMapValue < 0.66) return SHRUBLAND;
        // return TAIGA;
        return Color.blue;
      }

      if (heightMapValue > 0.3) {
        if (moistureMapValue < 0.16) return new Color(250.0f/255.0f, 250.0f/255.0f, 210.0f/255.0f); //TEMPERATE_DESERT;
        // if (moistureMapValue < 0.50) return GRASSLAND;
        // if (moistureMapValue < 0.83) return TEMPERATE_DECIDUOUS_FOREST;
        // return TEMPERATE_RAIN_FOREST;
        return Color.blue;
      }

      // if (moistureMapValue < 0.16) return SUBTROPICAL_DESERT;
      // if (moistureMapValue < 0.33) return GRASSLAND;
      // if (moistureMapValue < 0.66) return TROPICAL_SEASONAL_FOREST;
      // return TROPICAL_RAIN_FOREST;
      return Color.white;
    }

    private Color biome(float heightMapValue, float moistureMapValue) {
        // if (sample < 0.5) {
        //     newColor = Color.blue;
        // } else if (sample < .6) {
        //     newColor = Color.yellow;
        // } else if (sample < .8) {
        //     newColor = Color.green;
        // } else {
        //     newColor = Color.white;
        // }
      if (heightMapValue < 0.5) return Color.blue; // Ocean
      if (heightMapValue < 0.6) return Color.yellow; // Beach
      
      if (heightMapValue > 0.95) {
        if (moistureMapValue < 0.1) return Color.black; // Scorched
        if (moistureMapValue < 0.2) return Color.grey; // Bare
        // if (moistureMapValue < 0.5) return TUNDRA;
        return Color.white; //Snow
      }

      if (heightMapValue > 0.6) {
        if (moistureMapValue < 0.16) {
            return new Color(250.0f/255.0f, 250.0f/255.0f, 210.0f/255.0f); //TEMPERATE_DESERT;
        }
        // if (moistureMapValue < 0.66) return SHRUBLAND;2
        // return TAIGA;
        return Color.green;
      }

      if (heightMapValue > 0.3) {
        if (moistureMapValue < 0.16) {
            return new Color(250.0f/255.0f, 250.0f/255.0f, 210.0f/255.0f); //TEMPERATE_DESERT;
        }
        // if (moistureMapValue < 0.50) return GRASSLAND;
        // if (moistureMapValue < 0.83) return TEMPERATE_DECIDUOUS_FOREST;
        // return TEMPERATE_RAIN_FOREST;
        return Color.green;
      }

      // if (moistureMapValue < 0.16) return SUBTROPICAL_DESERT;
      // if (moistureMapValue < 0.33) return GRASSLAND;
      // if (moistureMapValue < 0.66) return TROPICAL_SEASONAL_FOREST;
      // return TROPICAL_RAIN_FOREST;
      return Color.green;
    }
}
