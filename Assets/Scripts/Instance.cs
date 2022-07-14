using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instance 
{
    float score;
    public float y;
    int generation;

    public Instance() 
    {
        score = 0;
    }
    public Instance(float angle)
    {
        this.y = angle;
    }

    public void SetScore(float _score)
    {
        score = _score;
    }
    public float GetScore()
    {
        return score;
    }

    public void SetY(float value)
    {
        this.y = value;
    }
    public float GetY()
    {
        return this.y;
    }
    public float GetAngle()
    {
        float angle = (float)(Math.Atan(this.y / 20) * (180 / Math.PI));
        return angle;
    }
    public void SetGeneration(int gen)
    {
        generation = gen;
    }
    public int GetGeneration()
    {
        return generation;
    }
}
