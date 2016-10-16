using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RandomTexture : MonoBehaviour {
    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;

    // public float scale = 6.0f;
    private float xOrg;
    private float yOrg;
    private float xOrg2;
    private float yOrg2;
    private int pixWidth = 128;
    private int pixHeight = 128;

    void Awake() {
        rend = GetComponent<Renderer>();        
    }

    /**
     * Public method that allows external object to create textures for the planet
     */
    public void createTexture() {
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.mainTexture = noiseTex;
        xOrg = UnityEngine.Random.Range(-1000.0f, 1000.0f);
        yOrg = UnityEngine.Random.Range(-1000.0f, 1000.0f);

        xOrg2 = UnityEngine.Random.Range(-1000.0f, 1000.0f);
        yOrg2 = UnityEngine.Random.Range(-1000.0f, 1000.0f);

        calcAndApplyTexture();
    }

    /**
     * Calculates a random texture based off perlin noise and sets the gameObject's texture to that random texture
     */
    void calcAndApplyTexture() {
        for (float y=0.0f; y<noiseTex.height; y++) {
            for (float x=0.0f; x<noiseTex.width; x++) {
                // Combine three samples of perlin noise, at different scales, to create a smooth approximation
                float sample = getPerlinNoiseSample(3.0f, x, y, xOrg, yOrg);
                sample += .5f * getPerlinNoiseSample(6.0f, x,y, xOrg, yOrg);
                sample += .25f * getPerlinNoiseSample(30.0f, x,y, xOrg, yOrg);

                // Create a different map to approximate humidity mapping
                float sample2 = getPerlinNoiseSample(2.0f, x, y, xOrg2, yOrg2);
                sample2 += .5f * getPerlinNoiseSample(8.0f, x,y, xOrg2, yOrg2);
                sample2 += .25f * getPerlinNoiseSample(20.0f, x,y, xOrg2, yOrg2);

                // Raise exponentially to create greater differences between valleys and peaks 
                sample = (float)Math.Pow(sample, 2.0f);
                sample2 = (float)Math.Pow(sample2, 2.0f);

                Color newColor = biome(sample, sample2);
                pix[(int) (y * noiseTex.width + x)] = newColor;
            }
        }

        noiseTex.SetPixels(pix);
        noiseTex.Apply();
        GetComponent<Renderer>().material.mainTexture = noiseTex;
    }

    /**
     * Gets a perlin noise value at a specific point, adjusted by a scale
     * 
     * float scale - How much we are going to adjust the scaling by
     * float x - The x coordinate of the point we are attempting to calculate
     * float y - The y coordinate of the point we are attempting to calculate
     * float originX - The x coordinate of the point we want to sample
     * float originY - The y coordinate of the point we want to sample
     * returns - A value between 0 and 1 representing a random noise sampling
     */
    private float getPerlinNoiseSample(float scale, float x, float y, float xOrigin, float yOrigin) {
        float xCoord = xOrigin + x / noiseTex.width * scale;
        float yCoord = yOrigin + y / noiseTex.height * scale;
        return Mathf.PerlinNoise(xCoord, yCoord);
    }

    /**
     * Generates a biome with predominantly water based off of two random values between 0 and 1
     * float heightMapValue - A random value between 0 and 1, representing the height of the world at that point
     * float moistureMapValue - A random value between 0 and 1, representing the moisture of the world at that point
     * return Color - a new color based off the information about height and moisture
     */
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

    /**
     * Generates a biome with predominantly earth-like attributes, based off of two random values between 0 and 1
     * float heightMapValue - A random value between 0 and 1, representing the height of the world at that point
     * float moistureMapValue - A random value between 0 and 1, representing the moisture of the world at that point
     * return Color - a new color based off the information about height and moisture
     */
    private Color biome(float heightMapValue, float moistureMapValue) {
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
