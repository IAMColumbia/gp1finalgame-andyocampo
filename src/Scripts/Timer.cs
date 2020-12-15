using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Timer : MonoBehaviour
{
    private float seconds, minutes;
    public string timerString;

    private void Update()
    {
        GameManager.timeLeft -= Time.deltaTime;
    }

    //displays time
    public string DisplayTimer(float timer)
    {
        minutes = (int)(timer / 60f);
        seconds = (int)(timer % 60f);
        timerString = $"Time: {minutes.ToString("00")}:{seconds.ToString("00")}";
        return timerString;
    }
}
