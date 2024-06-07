using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Pathfinding_NavNodeAgentType
{
    NONE = 0,
    Player,
    Shade,
    Slime,

}
;
public class Pathfinding_NavnodeAgent : MonoBehaviour {

    public Pathfinding_NavNodeAgentType m_PathfindAgent_Type;

    public Transform m_Target;  //where we currently want to go

    public Transform m_TargetGoal; //Location of wherever we are trying to reach

    public int m_nGoalPreviousRoomID; //Room that the Goal was in last time we performed Pathfind Check

    public Queue<NavNode> m_Path;

   //

    public IndoorDungeonGenerator m_IndoorDungeonGenerator;


    public int m_nCurrentRoomID;
    // Use this for initialization
    void Start () {
        m_PathfindAgent_Type = Pathfinding_NavNodeAgentType.NONE;        
    }

    public void SetType(Pathfinding_NavNodeAgentType agentType)
    {
        m_PathfindAgent_Type = agentType;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
