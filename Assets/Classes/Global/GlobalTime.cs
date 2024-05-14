using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GlobalTime : MonoBehaviour
{
    public static GlobalTime Instance { get; private set; }

    public int monthsPerYear = 12;
    public float baseSecondsPerDay = 60.0f;
    private float secondsPerDay;

    // La Start Date, efectivament!
    public float currentDayTime = 0.0f;
    public int currentDay = 1;
    public int currentMonth = 1;
    public int currentYear = 1356;

    private float[] speedModifiers = { 0.1f, 0.3f, 1.0f, 2.5f, 10.0f };
    private int currentSpeedIndex = 3;  // Recorda, el 0 és el primer valor. 2 = tercer valor

    public event Action OnDayChanged;
    public event Action OnMonthChanged;
    public event Action OnYearChanged;

    private Dictionary<int, int> daysInMonth = new Dictionary<int, int>()
    {
        {1, 31}, {2, 28}, {3, 31}, {4, 30}, {5, 31}, {6, 30},
        {7, 31}, {8, 31}, {9, 30}, {10, 31}, {11, 30}, {12, 31}
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            secondsPerDay = baseSecondsPerDay / speedModifiers[currentSpeedIndex];
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        currentDayTime += Time.deltaTime * speedModifiers[currentSpeedIndex];
        if (currentDayTime >= secondsPerDay)
        {
            AdvanceDay();
            currentDayTime = 0.0f;
        }
    }

    private void AdvanceDay()
    {
        currentDay++;
        OnDayChanged?.Invoke();

        if (currentDay > daysInMonth[currentMonth])
        {
            currentDay = 1;
            AdvanceMonth();
        }
    }

    private void AdvanceMonth()
    {
        currentMonth++;
        OnMonthChanged?.Invoke();

        if (currentMonth > monthsPerYear)
        {
            currentMonth = 1;
            AdvanceYear();
        }
    }

    private void AdvanceYear()
    {
        currentYear++;
        OnYearChanged?.Invoke();
    }

    public string GetCurrentDate()
    {
        return $"{currentDay}/{currentMonth}/{currentYear}";
    }

    public void IncreaseSpeed()
    {
        if (currentSpeedIndex < speedModifiers.Length - 1)
        {
            currentSpeedIndex++;
            secondsPerDay = baseSecondsPerDay / speedModifiers[currentSpeedIndex];
        }
    }

    public void DecreaseSpeed()
    {
        if (currentSpeedIndex > 0)
        {
            currentSpeedIndex--;
            secondsPerDay = baseSecondsPerDay / speedModifiers[currentSpeedIndex];
        }
    }
}