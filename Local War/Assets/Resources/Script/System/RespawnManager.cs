using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public interface ISpawnable
{
    void Spawn();
}


public class RespawnManager : MonoBehaviour
{
    private struct SpawnRequest
    {
        public DateTime time;
        public ISpawnable item;
        public SpawnRequest(DateTime time, ISpawnable item)
        {
            this.time = time;
            this.item = item;
        }
    }

    public static RespawnManager singleton;
    List<SpawnRequest> requests = new List<SpawnRequest>();

    public void QueueRespawn(DateTime when, ISpawnable item)
    {
        requests.Add(new SpawnRequest(when, item));
    }

    public void QueueRespawn(TimeSpan wait, ISpawnable item)
    {
        QueueRespawn(DateTime.Now + wait, item);
    }


    // Use this for initialization
    void Start()
    {
        singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        int length = requests.Count;
        for (int i = 0; i < length; i++)
        {
            if(requests[i].time < DateTime.Now)
            {
                requests[i].item.Spawn();
                requests.RemoveAt(i);
                length -= 1;
                i--;
            }
        }
    }
}
