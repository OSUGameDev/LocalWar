using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

using System.Collections;

public class SessionInformationManager : NetworkBehaviour
{
    public static PlayerCollection Players = new PlayerCollection();
    // Use this for initialization
    void Start()
    {
        if (isServer)
            Players.onPlayerRemoved += playerRemoved;
    }

    void OnDestroy()
    {
        if (isServer)
            Players.onPlayerRemoved -= playerRemoved;
    }

    // Update is called once per frame
    void Update()
    {
        if(isServer && Players.Any(item => item.changeSinceLastSerial == true))
        {
            SetDirtyBit(1);
        }
    }


    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if(initialState == false)
        {
            int count = 0;
            foreach (var player in Players)
            {
                if (player.changeSinceLastSerial == true)
                {
                    count += 1;
                }
            }
            if (count != 0)
                writer.Write(count);
            foreach (var player in Players)
            {
                if (player.changeSinceLastSerial == true)
                {
                    player.Serialize(writer);
                }
            }
            return (count != 0) | base.OnSerialize(writer, initialState);
        }
        else
        {
            writer.Write(Players.Count());
            foreach (var player in Players)
            {
                player.Serialize(writer);
            }
            base.OnSerialize(writer, initialState);
            return true;
        }

    }
    
    private void playerRemoved(int hashcode)
    {
        RpcRemoveClient(hashcode);
    }

    [ClientRpc]
    private void RpcRemoveClient(int hashcode)
    {
        Players.RemovePlayer(Players.FirstOrDefault(item => item.HashCode == hashcode));
    }

    public override void OnDeserialize(NetworkReader reader, System.Boolean initialState)
    {
        int count = reader.ReadInt32();
        for(int i = 0; i < count; i++)
        {
            var player = PlayerScoreInfo.Deserialize(reader);
            var oldState = Players.FirstOrDefault(item => item.HashCode == player.HashCode);
            if(oldState == null)
            {
                Players.AddPlayer(player);
                continue;
            }
            oldState.SetFrom(player);
        }
        base.OnDeserialize(reader, initialState);
    }
}
