using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SlowTime : MonoBehaviour
{
    int previousEnemiesKilled;
    bool timeRanOut;
    float weight = 0f;
    float time;
    float timeTime;
    public AnimationCurve timeCurve;
    [Tooltip("Higher Number = Slower Change")]
    public float changeSpeed;
    public Volume timeVol;
    public PlayerMovement playerMovement;
    [Space]
    [Header("Time Rewards")]
    public float enemyKillReward;

    void Update()
    {
        // Get values
        if(!Pause.paused)
        {
            timeTime += Time.unscaledDeltaTime;
            weight = timeTime/10f;
        }
        
        timeTime = Mathf.Clamp(timeTime, 0f, timeTime + 1f);
        time = timeCurve.Evaluate(timeTime/changeSpeed);
        

        timeVol.weight = weight;
        Time.timeScale = time;

        if (time <= .1f && !Pause.paused)
        {
            timeRanOut = true;
            StartCoroutine(TimeDeath());
        }
        if(previousEnemiesKilled < EnemyKillManager.enemiesKilled)
        {
            TimeReward(enemyKillReward);
        }
        previousEnemiesKilled = EnemyKillManager.enemiesKilled;
        Debug.Log(timeTime);
    }

    IEnumerator TimeDeath()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(1f);
        playerMovement.Death();
    }

    void TimeReward(float amount)
    {
        timeTime -= amount;
        Debug.Log("Reward");
    }
}
