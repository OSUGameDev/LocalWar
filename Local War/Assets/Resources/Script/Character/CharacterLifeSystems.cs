using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.ComponentModel;

[RequireComponent(typeof(NetworkIdentity))]
public class CharacterLifeSystems : LifeSys
{
    public static CharacterLifeSystems LocalPlayer;
    
 
    public PlayerScoreInfo info => SessionInformationManager.Players.getPlayers().FirstOrDefault(item => item.HashCode == this.netId.GetHashCode());

    int lastTeam = -1;

    protected override void Start()
    {
        base.Start();
        if (hasAuthority)
        {
            LocalPlayer = this;


            
            CmdAddToPlayersList(MainMenuSettings.primary.username);
            base.Kill(new DateTime(0));
        }
    }

    protected override void Update()
    {
        if(info != null && info.Team != lastTeam)
        {
            lastTeam = info.Team;
            foreach ( var render in GetComponentsInChildren<Renderer>())
            {
                render.material.color = info.Team == 1 ? Color.red : Color.blue;
            }
        }
    }

    protected override Transform getSpawnLocation()
    {
        if(info == null)
            return base.getSpawnLocation();
        var possibleLocations = GameObject.FindObjectsOfType<TeamNetworkStartPosition>().Where(item => item.team == info.Team);
        if(possibleLocations.Count() <= 0)
        {
            return base.getSpawnLocation();
        }
        System.Random rand = new System.Random();
        return possibleLocations.ElementAt(rand.Next(possibleLocations.Count())).transform;
    }

    [Command]
    private void CmdAddToPlayersList(string name)
    {

        var playerInfo = SessionInformationManager.Players.getPlayers().FirstOrDefault(item => item.HashCode == this.netId.GetHashCode());
        if (playerInfo == null)
        {
            SessionInformationManager.Players.AddPlayer(new PlayerScoreInfo(this.netId.GetHashCode()));
            var infoHandle = SessionInformationManager.Players.getPlayers().FirstOrDefault(item => item.HashCode == this.netId.GetHashCode());
            System.Random rand = new System.Random();
            infoHandle.Name = name;
            infoHandle.Team = rand.Next(1, 3);
        }
    }

    [Command]
    private void CmdRemovePlayer()
    {
        //todo
    }

    [ClientRpc]
    private void RpcRemovePlayer()
    {
        //todo
    }


    [Server]
    public override void Kill()
    {
        info.Deaths += 1;
        if(lastAttacker != null)
            lastAttacker.Kills += 1;
        base.Kill();
    }

    void OnDestroy()
    {
        SessionInformationManager.Players.RemovePlayer(this.info);
    }


    [Command]
    public void CmdSetNameRequest(string name)
    {
        info.Name = name;
    }


    [Command]
    public void CmdSetTeamRequest(int team)
    {
        if (team != info.Team)
        {
            info.Team = team;
            if (this.isAlive)
            {
                info.Deaths -= 1;
                lastAttacker = null;
                Kill();
            }
        }
    }


    private PlayerScoreInfo lastAttacker;

    public override void InflictDamage(Single dmg, int playerHashCode)
    {
        var attacker = SessionInformationManager.Players.FirstOrDefault(item => item.HashCode == playerHashCode);
        if (info.Team == attacker.Team)
            return;
        lastAttacker = attacker;
        base.InflictDamage(dmg, playerHashCode);
    }
}


public class PlayerCollection : INotifyPropertyChanged, IEnumerable<PlayerScoreInfo>
{

    List<PlayerScoreInfo> players = new List<PlayerScoreInfo>();
    public event Action<int> onPlayerRemoved;

    public IEnumerable<PlayerScoreInfo> getPlayers()
    {
        return players;
    }

    public void AddPlayer(PlayerScoreInfo info)
    {
        players.Add(info);
        info.PropertyChanged += PlayerPropertyChanged;
        OnPropertyChanged(nameof(players));
    }

    public void RemovePlayer(PlayerScoreInfo info)
    {
        if (info == null)
            return;
        players.Remove(info);
        info.PropertyChanged -= PlayerPropertyChanged;
        OnPropertyChanged(nameof(players));
        onPlayerRemoved?.Invoke(info.HashCode);
    }

    protected void PlayerPropertyChanged(object sender, PropertyChangedEventArgs arg)
    {
        OnPropertyChanged(nameof(players));
    }


    public event PropertyChangedEventHandler PropertyChanged ;
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public IEnumerator<PlayerScoreInfo> GetEnumerator()
    {
        return players.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return players.GetEnumerator();
    }

    
}

[System.Serializable]
public class PlayerScoreInfo : INotifyPropertyChanged
{
    [SerializeField]
    public int HashCode;

    [SerializeField]
    private int kills;
    public int Kills
    {
        get => kills;
        set {
            kills = value;
            OnPropertyChanged(nameof(Kills));
        }
    }

    [SerializeField]
    private int deaths;
    public int Deaths
    {
        get => deaths;
        set {
            deaths = value;
            OnPropertyChanged(nameof(Deaths));
        }
    }

    [SerializeField]
    private int team;
    public int Team
    {
        get => team;
        set {
            team = value;
            OnPropertyChanged(nameof(Team));
        }
    }

    [SerializeField]
    private string name;
    public string Name
    {
        get => name;
        set {
            name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public PlayerScoreInfo(int HashCode)
    {
        this.HashCode = HashCode;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        changeSinceLastSerial = true;
    }

    public bool changeSinceLastSerial { get; private set;}

    public static PlayerScoreInfo Deserialize(NetworkReader reader)
    {
        return new PlayerScoreInfo(reader.ReadInt32())
        {
            kills = reader.ReadInt32(),
            deaths = reader.ReadInt32(),
            team = reader.ReadInt32(),
            name = reader.ReadString(),
        };
    }

    public void Serialize(NetworkWriter writer)
    {
        changeSinceLastSerial = false;
        writer.Write(HashCode);
        writer.Write(kills);
        writer.Write(deaths);
        writer.Write(team);
        writer.Write(name);
    }

    public void SetFrom(PlayerScoreInfo value)
    {
        this.HashCode = value.HashCode;
        this.kills = value.kills;
        this.deaths = value.deaths;
        this.team = value.team;
        this.name = value.name;
        OnPropertyChanged("Copy");
    }
}