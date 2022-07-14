using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Threading.Tasks;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{         
    public GameObject ballPrefab;
    public GameObject shootingPoint;
    public int population;
    public int maitingPool;
    public int generations;
    public float mutationchance;
    public float timeScale = 1;
    public float power = 25f;
    public Text timeScaleText;
    public Text scoreText;
    public Text angleText;
    public Text childCountText;
    public Text competitiveText;
    public Text generationText;
    public Text generationsAvrageText;
    public Text bestScoreText;
    float score = 0f;
    List<Instance> children;
    List<float> generationsAvrageScore;
    bool newGenerationReady = false;
    string textBuffer;
    string avrageTextBuffer;
    bool finish = false;
    Instance bestChild;

    void Start()
    {
        bestChild = new Instance();
        generationsAvrageScore = new List<float>();
        Time.timeScale = 1f;
        Time.fixedDeltaTime =  0.02f;
        children = new List<Instance>();
        CreatePopulation();
        StartCoroutine(IterateGenerations());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            timeScale += 0.5f;
            SetTimeScale(timeScale);
            Debug.Log("Time Scale: " + timeScale.ToString());
        }

         if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            timeScale -= 0.5f;
            SetTimeScale(timeScale);
            Debug.Log("Time Scale: " + timeScale.ToString());
        }
         if(finish && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartEvaluation(generations - 1));
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor = new Color(0, 255, 0);
        }
    }
    void CreatePopulation()
    {
        float angle = 0;
        for(int i = 0; i < population; i++)
        {
            angle = Random.Range(13.5f,20f);
            Instance child = new Instance(angle);
            children.Add(child);
        }
    }
    IEnumerator IterateGenerations()
    {
        for (int i = 0; i < generations; i++)
        {
            generationText.text = "Generation: " + (i + 1) + " / " + generations;
            if (i + 1 == generations)
                finish = true;
            StartCoroutine(StartEvaluation(i));
            yield return new WaitUntil(() => newGenerationReady);
            
            newGenerationReady = false;
        }
        finish = true;
    }
    IEnumerator StartEvaluation(int curGeneration)
    {
        foreach (var child in children)
        {
            score = 0;
            SetScoreText();
            EvaluateChild(child);
            yield return new WaitUntil(() => CheckIfBallIsDestroyed());
            child.SetScore(score);
            SetCompetitiveText(children.IndexOf(child) + 1);
            if(child.GetScore() > bestChild.GetScore())
            {
                bestChild.SetGeneration(curGeneration);
                bestChild.SetScore(score);
                bestChild.SetY(child.GetY());
                UpdateBestChild();
            }
        }
        competitiveText.text = "";
        textBuffer = "";
        SetGenerationAvrageText(curGeneration);
        if (!finish)
        {
            SortChildren();
            children.RemoveRange(maitingPool, children.Count() - maitingPool);
            MakeNewChilderen();
        }
    }
    void MakeNewChilderen()
    {
        List<Instance> newChildren = new List<Instance>();
        newChildren.Add(children[0]);      
        Mutate(children[0]);

        while(newChildren.Count() < population)
        {
            Instance child = new Instance();
            float parent1 = children[Random.Range(0, children.Count())].GetY();
            float parent2 = children[Random.Range(0, children.Count())].GetY();
            child.SetY((parent1 + parent2) / 2);
            Mutate(child);
            newChildren.Add(child);
        }
        children = newChildren;
        newGenerationReady = true;
    }
    void Mutate(Instance child)
    {
        if(Random.Range(0,100) <= mutationchance)
        {
            float angle = child.GetY();
            angle += Random.Range(-2f, 2f);
            child.SetY(angle);
        }
    }
  
    bool CheckIfBallIsDestroyed()
    {
        GameObject ball = GameObject.FindGameObjectWithTag("Ball");
        if (ball == null)
            return true;
        return false;
    }
    void EvaluateChild(Instance child)
    {
        SetTextLabels(child);
        GameObject ball = Instantiate(ballPrefab, shootingPoint.transform.position, Quaternion.identity);
        ball.GetComponent<ShooterScript>().SetY(child.y);
        ball.GetComponent<ShooterScript>().Shoot(power);
        
    }
    void SetTextLabels(Instance child)
    {
        angleText.text = "Angle: " + child.GetAngle().ToString("0.0");
        childCountText.text = "Child: " + (children.IndexOf(child) + 1) + " / " + population; 
    }
    void SetCompetitiveText(int childIndex)
    {
        string slice1 = "[" + childIndex + "] - Angle [" + children[childIndex - 1].GetAngle().ToString("0.0") + "] Score [" +
            children[childIndex - 1].GetScore() + "] || ";
        textBuffer = string.Concat(textBuffer, slice1);
        competitiveText.text = textBuffer;
    }
    void SetGenerationAvrageText(int generationIndex)
    {
        float avrage = 0;
        foreach(var child in children)
        {
            avrage += child.GetScore();
        }
        avrage = avrage / children.Count();
        generationsAvrageScore.Add(avrage);
        avrageTextBuffer = string.Concat(avrageTextBuffer, "Gen " + (generationIndex + 1) + " - [" + avrage.ToString() + "] | ");
        if(avrageTextBuffer.Length > 90)
        {
            avrageTextBuffer = "Generation 1 - " + generationsAvrageScore[0] + " | Generation " + (generationIndex+1) + " - " + avrage;
        }
        generationsAvrageText.text = avrageTextBuffer;
        Debug.Log(avrageTextBuffer);
    }

    void SortChildren()
    {
        children.Sort(SortByScore);
    }
    static int SortByScore(Instance p1, Instance p2)
    {
        return p2.GetScore().CompareTo(p1.GetScore());
    }

    void UpdateBestChild()
    {
        bestScoreText.text = "G (" + (bestChild.GetGeneration() + 1) + ") / A (" + bestChild.GetAngle().ToString("0.0") + ") / S (" + bestChild.GetScore() + ")";
    }
    void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
        timeScaleText.text = "Time Scale : " + timeScale.ToString("0.0f");
    }
    public void BouncerHit(float ammount)
    {
        score += ammount;
        SetScoreText();
    }
    void SetScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}
