using System;
using System.Collections.Generic;
using UnityEngine;
public class Observer : MonoBehaviour
{
    public static Dictionary<string, List<Action>> obsever = new Dictionary<string, List<Action>>();
    public static void AddListener(String notiName, Action action)
    {
        if (!obsever.ContainsKey(notiName))
        {
            obsever.Add(notiName, new List<Action>());
        }
        obsever[notiName].Add(action);
    }
    public static void RemoveListener(String notiName, Action action)
    {
        if (!obsever.ContainsKey(notiName))
        {
            return;
        }
        obsever[notiName].Remove(action);
    }
    public static void Noti(String notiName)
    {
        if (!obsever.ContainsKey(notiName))
        {
            return;
        }
        foreach (Action action in obsever[notiName])
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                Debug.Log("error invoke action :" + e);
            }
        }
    }
}
