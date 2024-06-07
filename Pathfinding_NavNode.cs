using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A pathfinding manager for NPCs that roam the playable area
/// </summary>
public class Pathfinding_NavNode : MonoBehaviour
{

    //Goal to create a queue for calling the navnode pathfinding function on gameobjects that need to navigate through the dungeon
    public List<Pathfinding_NavnodeAgent> m_Objects;
    public int m_nMaxIterations;
    //public int m_nCurrentIteration;

    public Pathfinding_NavnodeAgent m_GoalDestination;

    public IndoorDungeonGenerator m_IndoorDungeonGenerator;

    // Use this for initialization
    void Start()
    {
        m_Objects = new List<Pathfinding_NavnodeAgent>();
        m_nMaxIterations = 1;
        //m_nCurrentIteration = 0;
        //m_Player = GameObject.FindGameObjectWithTag("Player1").GetComponent<BlueOrc_PC>();
    }

    // Update is called once per frame
    void Update()
    {

        //If there are any objects
        if (m_Objects.Count > 0)
        {
            for (int nCurrentIteration = 0; nCurrentIteration < m_nMaxIterations; nCurrentIteration++)
            {
                Pathfinding_NavnodeAgent currentObject = m_Objects[0];
                m_Objects.RemoveAt(0);
                if (Pathfind(currentObject))
                {
                    //the target is not in our room, so we found a navnode path to the target
                    //dequeue the first node in that path and set it to the target

                    //There's a special case we have to deal with
                    //If we're in the space between the first node on our path and the next node, (the exit space before the next room), we'll get stuck in an infinite loop
                    //where we're constantly running back to the first node in our path
                    //if we're in that exit space or within 1 tile of the first node, dequeue the first node and start with the second node

                    NavNode currentNavNode = currentObject.m_Path.Dequeue();

                    currentObject.m_Target = currentNavNode.transform;
                    //switch (currentNavNode.m_ExitDirection)
                    //{
                    //    case Direction.Up:
                    //        {
                    //            if (currentObject.transform.position.y >= currentNavNode.transform.position.y)
                    //                currentObject.m_Target = currentObject.m_Path.Dequeue().transform;
                    //            else
                    //                currentObject.m_Target = currentNavNode.transform;
                    //            break;
                    //        }
                    //    case Direction.Down:
                    //        {
                    //            if (currentObject.transform.position.y <= currentNavNode.transform.position.y)
                    //                currentObject.m_Target = currentObject.m_Path.Dequeue().transform;
                    //            else
                    //                currentObject.m_Target = currentNavNode.transform;
                    //            break;
                    //        }
                    //    case Direction.Right:
                    //        {
                    //            if (currentObject.transform.position.x >= currentNavNode.transform.position.x)
                    //                currentObject.m_Target = currentObject.m_Path.Dequeue().transform;
                    //            else
                    //                currentObject.m_Target = currentNavNode.transform;
                    //            break;
                    //        }
                    //    case Direction.Left:
                    //        {
                    //            if (currentObject.transform.position.x <= currentNavNode.transform.position.x)
                    //                currentObject.m_Target = currentObject.m_Path.Dequeue().transform;
                    //            else
                    //                currentObject.m_Target = currentNavNode.transform;
                    //            break;

                    //        }
                    //}
                }
                else
                {
                    //we do not need a navnode path
                }


                m_Objects.Add(currentObject);
            }
        }
    }

    /// <summary>
    /// Purpose is to set ourPathfinder.m_Target to the appropriate transform (position) based on rooms/distance from player
    /// true = we placed nodes in currentObject.m_Path
    /// false = we're in the target's room and do not need navnode pathfinding
    /// </summary>
    /// <param name="ourPathfinder"></param>
    bool Pathfind(Pathfinding_NavnodeAgent ourPathfinder)
    {
        //Our pathfinder is the agent who is trying to reach targetGoal
        //target is whatever the agent is currently moving towards on its path to targetGoal
        //the pathfinder should already know it's targetgoal


        //has the Goal changed rooms since our last check? if not, we don't need to perform pathfinding again, return false
        if (ourPathfinder.m_nGoalPreviousRoomID == m_GoalDestination.m_nCurrentRoomID)
        {
            //we already have a path for this room
            return false;
        }

        //are we standing in an exit portion? if so, don't perform a pathfinding update until we step into the center portion of a room...unless we need  path
        if (StandingInExit(ourPathfinder) && ourPathfinder.m_Path.Count > 0)
            return false;

        ourPathfinder.m_nGoalPreviousRoomID = m_GoalDestination.m_nCurrentRoomID;

        ourPathfinder.m_Path.Clear();

        //is the goal in my room?
        if (ourPathfinder.m_nCurrentRoomID == m_GoalDestination.m_nCurrentRoomID)
        {
            //We found the target's room, set our target to targetgoal
            ourPathfinder.m_Target = ourPathfinder.m_TargetGoal;
            return false;
        }

        //otherwise...we need to find the set of nodes that lead us to the room that the targetgoal is in

        //for each node in the goal's room, check the first connection of that node (which leads to another room).  
        for (int nNode = 0; nNode < m_IndoorDungeonGenerator.m_Rooms[m_GoalDestination.m_nCurrentRoomID].m_NavNodes.Count; nNode++)
        {
            //If that connection node in the new room is in my room, we're done,
            if (m_IndoorDungeonGenerator.m_Rooms[m_GoalDestination.m_nCurrentRoomID].m_NavNodes[nNode].m_NavNodeConnections[0].m_nRoomID == ourPathfinder.m_nCurrentRoomID)
            {
                //we found our room.             
                ourPathfinder.m_Path.Enqueue(m_IndoorDungeonGenerator.m_Rooms[m_GoalDestination.m_nCurrentRoomID].m_NavNodes[nNode].m_NavNodeConnections[0]);
                ourPathfinder.m_Path.Enqueue(m_IndoorDungeonGenerator.m_Rooms[m_GoalDestination.m_nCurrentRoomID].m_NavNodes[nNode]);
                return true;
            }
            //otherwise check all navnodes in the room
            if (CheckOtherNodesInRoom(
                ourPathfinder,
                m_IndoorDungeonGenerator.m_Rooms[m_GoalDestination.m_nCurrentRoomID].m_NavNodes[nNode].m_NavNodeConnections[0].m_nRoomID,
                m_IndoorDungeonGenerator.m_Rooms[m_GoalDestination.m_nCurrentRoomID].m_NavNodes[nNode].m_NavNodeConnections[0].m_nNodeID))
            {
                //this node led us to the solution
                ourPathfinder.m_Path.Enqueue(m_IndoorDungeonGenerator.m_Rooms[m_GoalDestination.m_nCurrentRoomID].m_NavNodes[nNode].m_NavNodeConnections[0]);
                ourPathfinder.m_Path.Enqueue(m_IndoorDungeonGenerator.m_Rooms[m_GoalDestination.m_nCurrentRoomID].m_NavNodes[nNode]);
                return true;
            }
        }
        Debug.Log("Pathfinding Agent did not find a path.", ourPathfinder);
        return false;
    }

    bool CheckOtherNodesInRoom(Pathfinding_NavnodeAgent ourPathfinder, int nRoomToCheckID, int nNodeAlreadyVisitedID)
    {
        //otherwise we check every other node's(connection's) first connection (which leads to yet another another room) and repeat this process until we find my room.
        for (int nNode = 0; nNode < m_IndoorDungeonGenerator.m_Rooms[nRoomToCheckID].m_NavNodes.Count; nNode++)
        {
            //make sure we haven't already checked this node
            if (m_IndoorDungeonGenerator.m_Rooms[nRoomToCheckID].m_NavNodes[nNode].m_nNodeID == nNodeAlreadyVisitedID)
                continue;

            //If that connection node in the new room is in my room, we're done,
            if (m_IndoorDungeonGenerator.m_Rooms[nRoomToCheckID].m_NavNodes[nNode].m_NavNodeConnections[0].m_nRoomID == ourPathfinder.m_nCurrentRoomID)
            {
                //we found our room
                //int nSolutionRoomIndex = m_IndoorDungeonGenerator.m_Rooms[nRoomToCheckID].m_NavNodes[nNode].m_NavNodeConnections[0].m_nRoomID;
                //int nSolutionNodeID = m_IndoorDungeonGenerator.m_Rooms[nRoomToCheckID].m_NavNodes[nNode].m_NavNodeConnections[0].m_nNodeID;//First node we found that is in the room ourPathfinder is in
                //Direction exitPathfinderStandsIn = StandingInExitAddExitNode(ourPathfinder);
                
                //if we're standing in an exit, enqueue that node first
                //for (int nSolutionRoomNodes = 0; nSolutionRoomNodes < m_IndoorDungeonGenerator.m_Rooms[nSolutionRoomIndex].m_NavNodes.Count; nSolutionRoomNodes++)
                //{
                //    //make sure we skip the node we're already going to add
                //    if (m_IndoorDungeonGenerator.m_Rooms[nSolutionRoomIndex].m_NavNodes[nSolutionRoomNodes].m_nNodeID == nSolutionNodeID)
                //        continue;

                //    if (exitPathfinderStandsIn == m_IndoorDungeonGenerator.m_Rooms[nSolutionRoomIndex].m_NavNodes[nSolutionRoomNodes].m_ExitDirection)
                //    {
                //        //this is an additional node that is closest to us in our path to the goal
                //        ourPathfinder.m_Path.Enqueue(m_IndoorDungeonGenerator.m_Rooms[nSolutionRoomIndex].m_NavNodes[nSolutionRoomNodes]);
                //        break;
                //    }
                //}
                
                                    
                ourPathfinder.m_Path.Enqueue(m_IndoorDungeonGenerator.m_Rooms[nRoomToCheckID].m_NavNodes[nNode].m_NavNodeConnections[0]);
                return true;
            }

            if (CheckOtherNodesInRoom(
                ourPathfinder,
                m_IndoorDungeonGenerator.m_Rooms[nRoomToCheckID].m_NavNodes[nNode].m_NavNodeConnections[0].m_nRoomID,
                m_IndoorDungeonGenerator.m_Rooms[nRoomToCheckID].m_NavNodes[nNode].m_NavNodeConnections[0].m_nNodeID))
            {
                //we found our room lower down

                ourPathfinder.m_Path.Enqueue(m_IndoorDungeonGenerator.m_Rooms[nRoomToCheckID].m_NavNodes[nNode].m_NavNodeConnections[0]);
                ourPathfinder.m_Path.Enqueue(m_IndoorDungeonGenerator.m_Rooms[nRoomToCheckID].m_NavNodes[nNode]);
                return true;
            }

        }

        return false;

    }

    /// <summary>
    /// If we're standing in an exit...don't do any new pathfinding until we make it into the center portion of a room
    /// </summary>
    bool StandingInExit(Pathfinding_NavnodeAgent ourPathfinder)
    {
        for (int nRoomNode = 0; nRoomNode < m_IndoorDungeonGenerator.m_Rooms[ourPathfinder.m_nCurrentRoomID].m_NavNodes.Count; nRoomNode++)
        {
            NavNode currNavNode = m_IndoorDungeonGenerator.m_Rooms[ourPathfinder.m_nCurrentRoomID].m_NavNodes[nRoomNode];

            switch (m_IndoorDungeonGenerator.m_Rooms[ourPathfinder.m_nCurrentRoomID].m_NavNodes[nRoomNode].m_ExitDirection)
            {
                case Direction.Up:
                    {
                        if (ourPathfinder.transform.position.y >= currNavNode.transform.position.y)
                            return true;
                        break;
                    }
                case Direction.Down:
                    {
                        if (ourPathfinder.transform.position.y <= currNavNode.transform.position.y)
                            return true;
                        break;
                    }
                case Direction.Right:
                    {
                        if (ourPathfinder.transform.position.x >= currNavNode.transform.position.x)
                            return true;
                        break;
                    }
                case Direction.Left:
                    {
                        if (ourPathfinder.transform.position.x <= currNavNode.transform.position.x)
                            return true;
                        break;
                    }
            }
        }
        return false;
    }

    /// <summary>
    /// If we're standing in an exit...don't do any new pathfinding until we make it into the center portion of a room
    /// </summary>
    Direction StandingInExitAddExitNode(Pathfinding_NavnodeAgent ourPathfinder)
    {
        for (int nRoomNode = 0; nRoomNode < m_IndoorDungeonGenerator.m_Rooms[ourPathfinder.m_nCurrentRoomID].m_NavNodes.Count; nRoomNode++)
        {
            NavNode currNavNode = m_IndoorDungeonGenerator.m_Rooms[ourPathfinder.m_nCurrentRoomID].m_NavNodes[nRoomNode];

            switch (m_IndoorDungeonGenerator.m_Rooms[ourPathfinder.m_nCurrentRoomID].m_NavNodes[nRoomNode].m_ExitDirection)
            {
                case Direction.Up:
                    {
                        if (ourPathfinder.transform.position.y >= currNavNode.transform.position.y)
                            return Direction.Up;                      
                        break;
                    }
                case Direction.Down:
                    {
                        if (ourPathfinder.transform.position.y <= currNavNode.transform.position.y)
                            return Direction.Down;
                        break;
                    }
                case Direction.Right:
                    {
                        if (ourPathfinder.transform.position.x >= currNavNode.transform.position.x)
                            return Direction.Right;
                        break;
                    }
                case Direction.Left:
                    {
                        if (ourPathfinder.transform.position.x <= currNavNode.transform.position.x)
                            return Direction.Left;
                        break;
                    }
            }
        }
        return Direction.None;
    }
}
