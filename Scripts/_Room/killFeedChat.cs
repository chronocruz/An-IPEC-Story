using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class killFeedChat : Photon.MonoBehaviour 
{
    Rect GuiRect = new Rect(Screen.width - 250 ,0, 250,300);
    public bool IsVisible = true;
    public bool AlignBottom = false;
    public List<string> messages = new List<string>();
    private string inputLine = "";
    private Vector2 scrollPos = Vector2.zero;
    bool inputVis = false;
    public static readonly string ChatRPC = "Chat";

    public void Start()
    {
        if (this.AlignBottom)
        {
            this.GuiRect.y = Screen.height - this.GuiRect.height;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.Y))
        {
            if (inputVis)
            {
                inputVis = false;
            } else
            {
                inputVis = true;
            }
        }
    }

    public void OnGUI()
    {
        if (!this.IsVisible || !PhotonNetwork.inRoom)
        {
            return;
        }

        if (GameObject.Find("_Room") != null)
        {
            GUI.skin = GameObject.Find("_Room").GetComponent<roomManager>().skin;
        }
        
        if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(this.inputLine))
            {
                this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
                this.inputLine = "";
                GUI.FocusControl("");
                return; // printing the now modified list would result in an error. to avoid this, we just skip this single frame
            }
            else
            {
                GUI.FocusControl("ChatInput");
            }
        }

        GUI.SetNextControlName("");
        GUILayout.BeginArea(this.GuiRect);

        scrollPos = GUILayout.BeginScrollView(scrollPos, "Label");
        GUILayout.FlexibleSpace();
        for (int i = messages.Count - 1; i >= 0; i--)
        {
            GUILayout.Label(messages[i]);
        }
        GUILayout.EndScrollView();
        if (inputVis)
        {
            GUILayout.BeginHorizontal();
            GUI.SetNextControlName("ChatInput");
            inputLine = GUILayout.TextField(inputLine);
            if (GUILayout.Button("Send", GUILayout.ExpandWidth(false)))
            {
                this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
                this.inputLine = "";
                GUI.FocusControl("");
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    [PunRPC]
    public void Chat(string newLine, PhotonMessageInfo mi)
    {
        string senderName = "anonymous";

        if (mi.sender != null)
        {
            if (!string.IsNullOrEmpty(mi.sender.NickName))
            {
                senderName = mi.sender.NickName;
            }
            else
            {
                senderName = "player " + mi.sender.ID;
            }
        }

        this.messages.Add(senderName +": " + newLine);
    }

    public void AddLine(string newLine)
    {
        this.messages.Add(newLine);
    }

    [PunRPC]
    public void addFeed(string killer, string victim)
    {
        this.messages.Add(killer + " [FRAG] " + victim);
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
       this.messages.Add(player.NickName + " Connected!");
    }


    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        if (player.ID == PhotonNetwork.room.MasterClientId)
        {
            this.messages.Add("Host Disconnected! Migrating Hosts - All Bots and Zombies have been deleted");
        }
        else {
            this.messages.Add(player.NickName + " Disconnected ");
        }
    }

    [PunRPC]
    public void promo(string name, string rank)
    {
        this.messages.Add(name + " was Promoted to " + rank + "!");
    }
}
