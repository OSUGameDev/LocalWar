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
    public static PlayerCollection Players = new PlayerCollection();

    
    public PlayerScoreInfo info;

    protected override void Start()
    {
        base.Start();
        if (hasAuthority)
        {
            LocalPlayer = this;
            CmdAddToPlayersList();
            System.Random rand = new System.Random();
            if (isClient)
            {
                CmdSetTeam(rand.Next(0, 1));
                CmdSetName("Rickst");
            }
        }
    }
    protected void OnDestroy()
    {
        if(hasAuthority)
        {

        }
    }

    [Command]
    private void CmdAddToPlayersList()
    {
        Players.AddPlayer(new PlayerScoreInfo(this.netId.GetHashCode()));
        this.info = Players.getPlayers().FirstOrDefault(item => item.HashCode == this.netId.GetHashCode());
        RpcSetPlayerInfo();
    }

    [ClientRpc]
    private void RpcSetPlayerInfo()
    {
        this.info = Players.getPlayers().FirstOrDefault(item => item.HashCode == this.netId.GetHashCode());
    }


    private void CmdRemovePlayer()
    {
        //todo
    }
    private void RpcRemovePlayer()
    {
        //todo
    }



    public override void Kill()
    {
        info.Deaths += 1;
        base.Kill();
    }

    protected override void RpcKill()
    {
        base.RpcKill();
        info.Deaths += 1;
    }

    [Command]
    public void CmdSetName(string name)
    {
        info.Name = name;
        RpcNameSet(name);
    }

    [ClientRpc]
    protected void RpcNameSet(string name)
    {
        info.Name = name;
    }

    [Command]
    public void CmdSetTeam(int team)
    {
        info.Team = team;
        RpcSetTeam(team);
    }

    [ClientRpc]
    protected void RpcSetTeam(int team)
    {
        info.Team = team;
    }


    public override void InflictDamage(Single dmg, int team)
    {
        if (info.Team == team)
            return;
        base.InflictDamage(dmg,team);
    }
}


public class PlayerCollection : INotifyPropertyChanged
{

    List<PlayerScoreInfo> players = new List<PlayerScoreInfo>();


    public IEnumerable<PlayerScoreInfo> getPlayers()
    {
        return players;
    }

    public void AddPlayer(PlayerScoreInfo info)
    {
        OnPropertyChanged(nameof(players));
        players.Add(info);
        info.PropertyChanged += PlayerPropertyChanged;
    }

    public void RemovePlayer(PlayerScoreInfo info)
    {
        OnPropertyChanged(nameof(players));
        players.Remove(info);
        info.PropertyChanged -= PlayerPropertyChanged;
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
    }
}