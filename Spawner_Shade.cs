using UnityEngine;
using System.Collections;

public class Spawner_Shade : MonoBehaviour
{

    int m_nTotalShades;     //number of shades that can spawn
    float m_fSpawnTime;
    float m_fMaxSpawnTime;
    //int m_nTargetToSpawn;  	//array position to spawn
    Vector3 m_vOffset;      //offset to spawn shades at

    public Monster_Shade[] shadeArray;
    Queue m_qRespawnQueue;

   public int m_nCurrentRoomID;         //current room this object is in

    // Use this for initialization
    void Start()
    {
        //Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        //if we aren't at max active shades, countdown until spawn time...then, spawn
        if (m_qRespawnQueue.Count > 0)
        {
            m_fSpawnTime -= Time.deltaTime;

            if (m_fSpawnTime <= 0f)
            {
                Spawn();
            }
        }
    }

    public void Initialize()
    {
        //TODO: Spawn time needs to be random
        m_fMaxSpawnTime = m_fSpawnTime = 9.0f;

        m_nTotalShades = 3;
        shadeArray = new Monster_Shade[m_nTotalShades];

        m_vOffset = new Vector3(.0205f, .02985f, 0f);

        Vector3 vPosition = transform.position + m_vOffset;

        m_qRespawnQueue = new Queue(shadeArray.Length);//+1 for the original object may want to remove later

        //fill our respawn queue with each shade in the shadeArray
        for (int i = 0; i < shadeArray.Length; i++)
        {
            m_qRespawnQueue.Enqueue(i);
            //shadeArray[i] = Instantiate (shadeToInstantiate, vPosition, Quaternion.identity) as Transform;
            shadeArray[i] = Instantiate(Resources.Load("Prefabs/Monster_Shade", typeof(Monster_Shade)), vPosition, Quaternion.identity) as Monster_Shade;
            shadeArray[i].transform.parent = transform;
            shadeArray[i].Initialize();
            shadeArray[i].SetSpawnID(i);
            shadeArray[i].gameObject.SetActive(false);
        }
    }

    public void AddToRespawner(int nRespawnID)
    {
        m_qRespawnQueue.Enqueue(nRespawnID);

        Vector3 vPosition = transform.position + m_vOffset;

        shadeArray[nRespawnID].transform.position = vPosition;
        shadeArray[nRespawnID].gameObject.SetActive(false);
    }

    void Spawn()
    {
        //Vector3 vPosition = transform.position + m_vOffset;

        int nShadeToRespawn = (int)m_qRespawnQueue.Dequeue();

        shadeArray[nShadeToRespawn].gameObject.SetActive(true);
        shadeArray[nShadeToRespawn].m_nCurrentRoomID = m_nCurrentRoomID;
        shadeArray[nShadeToRespawn].GetComponent<Animator>().enabled = true;

        shadeArray[nShadeToRespawn].Spawn();

        ResetSpawnTimer();
    }

    void ResetSpawnTimer()
    {
        m_fSpawnTime = m_fMaxSpawnTime;
    }

}
