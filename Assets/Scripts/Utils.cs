using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static string FormatTimeToHours(float time)
    {
        int hours = Mathf.RoundToInt(time / 3600f);
        int minutes = Mathf.FloorToInt(time % 3600f / 60f);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
    public static string FormatTimeToMinutes(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public static RaycastHit RaycastFromMouseUsingPerspectiveCamera(LayerMask layerMask, float maxDistance = 1000)
    {
        Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        Physics.Raycast(ray, out hitData, maxDistance, layerMask);
        return hitData;
    }
}