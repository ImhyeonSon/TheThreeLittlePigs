using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class RoomData
{
    public string RoomName { get; set; }
    public bool Status { get; set; }
    public int CurrentPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public string Password { get; set; }    

    public RoomData() { }
    public RoomData(string RoomName, bool Status, int CurrentPlayers, int MaxPlayers, string Password)
    {
        this.RoomName = RoomName;
        this.Status = Status;
        this.CurrentPlayers = CurrentPlayers;
        this.MaxPlayers = MaxPlayers;
        this.Password = Password;
    }
}
