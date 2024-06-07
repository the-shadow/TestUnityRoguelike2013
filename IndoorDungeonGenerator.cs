using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Direction
{
    Up = 0,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight,
    None
}

public enum RoomTheme
{
    BasicDungeon,
    None
}

public enum SpecialRoomType
{
    BossRoom = 0,
    None
}

public enum BossID
{
    CorpseFondler = 0,
    None
}

public enum AreaExitRegion
{
    LargestX = 0,
    LargestY,
    SmallestX,
    SmallestY,
    LargestXY,
    SmallestXY,
    LargestXSmallestY,
    LargestYSmallestX,
    None
}

//Used to Track Dungeon Loading Process in Update()
public enum IndoorDungeonGeneratorBuildProgress
{
    FirstRoom = 0,
    RoomCreation,//25
    DungeonSizeCheck,
    BonusRoomCreation,//20
    RemoveDuplicateJunctionExits,//5
    RemoveDeadEnds,//5
    SyncRoomTiles,//5
    ErectLightBlockingWalls,
    NavNodes_Creation,//5
    NavNodes_Linking,//5
    NavNodes_Cleaning,//5
    SceneryPlacementZoning,
    PlaceTorches,//5
    PlaceTreasureChests,//5
    PlaceJars,//5
    PlaceWoodenCrates,//5
    PlaceShadeSpawners,//5
    PlaceTriggers,
    PlaceBossRooms,
    Circularity,
    FindBranchEnds,

    DONE
}

//public enum GeneratedFeatureType
//{
//		Room = 0,
//		HallwayHorizontal,
//		HallwayVertical,
//		Junction
//}

public struct Index2D
{
    int m_nXIndex;
    int m_nYIndex;

    public Index2D(int X, int Y)
    {
        m_nXIndex = X;
        m_nYIndex = Y;
    }

    public int X
    {
        get { return m_nXIndex; }
        set { m_nXIndex = value; }
    }

    public int Y
    {
        get { return m_nYIndex; }
        set { m_nYIndex = value; }
    }
}
;

public struct RoomExit
{
    int m_nRoomID;
    TMBuildType m_BuildType;
    Direction m_Direction;
    int m_nOffSet;
    Vector3 m_vWorldPosition;

    public int RoomID
    {
        get { return m_nRoomID; }
        set { m_nRoomID = value; }
    }

    public TMBuildType BuildType
    {
        get { return m_BuildType; }
        set { m_BuildType = value; }
    }

    public Direction ExitDirection
    {
        get { return m_Direction; }
        set { m_Direction = value; }
    }

    public int OffSet
    {
        get { return m_nOffSet; }
        set { m_nOffSet = value; }
    }

    public Vector3 WorldPosition
    {
        get { return m_vWorldPosition; }
        set { m_vWorldPosition = value; }
    }
};

//should be a room ID and a tile coordinate in that room, and the direction that the room would create an opening in
public struct AreaExit
{
    int m_nRoomID;
    int m_nTileCoordX;
    int m_nTileCoordY;
    Direction m_eDirectionToOpenRoomExit;
    AreaExitRegion m_eAreaExitRegion; //What section of the region this area exit owns (example: +x or +y, -x or -y)
    Vector3 m_vWorldLocation;


    public int RoomID
    {
        get { return m_nRoomID; }
        set { m_nRoomID = value; }
    }

    public int TileCoordX
    {
        get { return m_nTileCoordX;  }
        set { m_nTileCoordX = value; }
    }

    public int TileCoordY
    {
        get { return m_nTileCoordY; }
        set { m_nTileCoordY = value; }
    }

    public Direction DirectionToOpenRoom
    {
        get { return m_eDirectionToOpenRoomExit; }
        set { m_eDirectionToOpenRoomExit = value; }
    }

    public AreaExitRegion AreaOwned
    {
        get { return m_eAreaExitRegion; }
        set { m_eAreaExitRegion = value; }
    }

    public Vector3 WorldLocation
    {
        get { return m_vWorldLocation; }
        set { m_vWorldLocation = value; }
    }
};

public enum RoomGenerationStep
{ 
    //Steps:
  //1a) Use an exit selection to store a possible room in a new member structure.  Store width/height/buildtype/currentCenter (found with nEntranceoffset)
  //1b) Decide what type of room we're using.
  //2) Test if theoretical room can be placed
  //3) Open Previous Room
  //4) Create New Room
  //5) Open Current Room
  //6) Add new exits (CreateUnusedExits)
    RollRoomTypeAndAttributes = 0,
    TestPlacement,
    OpenPreviousRoom,
    GenerateNewRoom,
    WorkOnTileMap,
    OpenNewRoom,
    AddNewExits,
    DONE
}

public enum TileMapCreationStep
{
   SetTileMapVariables = 0,
   SetMesh,
   SetWalls,
   SetTexture,
   DONE
}

public struct RoomAttributes
{
    //Store width/height/buildtype/currentCenter(found with nEntranceoffset)
    int m_nTileWidth;
    int m_nTileHeight;
    int m_nEntranceOffset;  //used for calculating currentcenter and used for opening the new room in the correct location
    TMBuildType m_eBuildType;   //used for construction, maybe different than real room type
    TMBuildType m_eRoomType;    //Actual Room Type
    Vector3 m_vCurrentCenter;  //Center that is in line with the center of the other room opening.
    ////need the center to be in line with the center of the opening
    //Vector3 vCurrentCenter = new Vector3(((nTestX - 1) * m_fTileSize * .5f), 0f, 0f);
    //vCurrentCenter += connectingExit.WorldPosition;

    //                //Offset for the Entrance
    //                int nMaxOffsetY = 0;
    //int nEntranceOffsetY = 0;
    //nMaxOffsetY = CalculateMaxOffset(nTestX, nTestY, Direction.Left);

    //nEntranceOffsetY = Random.Range(-nMaxOffsetY, nMaxOffsetY);
    //                vCurrentCenter.y += (nEntranceOffsetY* m_fTileSize);
    public int TileWidth
    {
        get { return m_nTileWidth; }
        set { m_nTileWidth = value; }
    }

    public int TileHeight
    {
        get { return m_nTileHeight; }
        set { m_nTileHeight = value; }
    }

    public int EntranceOffset
    {
        get { return m_nEntranceOffset; }
        set { m_nEntranceOffset = value; }
    }

    public TMBuildType BuildType
    {
        get { return m_eBuildType; }
        set { m_eBuildType = value; }
    }

    public TMBuildType RoomType
    {
        get { return m_eRoomType; }
        set { m_eRoomType = value; }
    }

    public Vector3 CurrentCenter
    {
        get { return m_vCurrentCenter; }
        set { m_vCurrentCenter = value; }
    }
};

public struct GlobalTileMapData
{
    Texture2D m_TerrainTexture;
    Color[][] m_TerrainTextureTileData;   //Data for each corresponding Terrain Texture, basically for tile stamping
    int m_nTileResolution;
    int m_nTilesPerRow;
    int m_nRows;

    public Texture2D TerrainTexture
    {
        get { return m_TerrainTexture; }
        set { m_TerrainTexture = value; }
    }

    public Color[][] TerrainTextureTileData
    {
        get { return m_TerrainTextureTileData; }
        set { m_TerrainTextureTileData = value; }
    }

    public int TileResolution
    {
        get { return m_nTileResolution; }
        set { m_nTileResolution = value; }
    }

    public int TilesPerRow
    {
        get { return m_nTilesPerRow; }
        set { m_nTilesPerRow = value; }
    }

    public int Rows
    {
        get { return m_nRows; }
        set { m_nRows = value; }
    }

};

public class IndoorDungeonGenerator : MonoBehaviour
{

    //List<Rect> m_AllRooms;              //The Rectangle dimensions for every room we want to store.
    List<Rect> m_CircularRoomBuffer;    //Room info for 

    //Pathfinding Manager
    public Pathfinding_NavNode m_PathfindingManager;

    public List<Tile_Map> m_Rooms;
    List<RoomExit> m_UnusedExits;
    List<RoomExit> m_UsedExits;
    List<RoomExit> m_UnusedHallwayExits;
    List<RoomExit> m_UnusedJunctionExits;
    List<Direction> possibleExits;
    Queue<Tile_Map> m_DeadEndRooms;  //stuff we need to remove after building
    //TMBuildType m_LastBuiltFeature;
    int m_nTotalRooms;
    int m_nCurrentRooms;
    Vector3 m_vStartingRoomCenter;
    List<string> m_TileMapLocations;
    //Our Textures - Switching to GlobalTileMapStorage
    public List<Texture2D> m_TerrainTextures;
    //List<Color[][]> m_TerrainTextureTileData;   //Data for each corresponding Terrain Texture, basically for tile stamping
    //int m_nTileResolution;
    //int m_nTilesPerRow;
    //int m_nRows;
    List<GlobalTileMapData> m_GlobalTileMaps;


    int m_nExitSize;
    public float m_fTileSize;
    GenericGraph<Tile_Map> m_LevelGraph;
    List<AreaExit> m_AreaExits;     //List that helps us determine what rooms to designate as area exits
    List<Tile_Map> m_MapAreaExits;  //List that stores the area exits that were determined with m_AreaExits
    List<Tile_Map> m_DungeonBranchEndRooms; //Every branch ends with a room, this is a list of each end point to aid with circularly connecting some of these to area exits

    //Player Reference
    public BlueOrc_PC m_Player;

    //All NonNPC objects that can be interacted with (store a player reference)
    List<Interactable> m_Interactables;

    //Monster Spawners
    List<Spawner_Shade> m_ShadeSpawners;

    //Loading Bar Tracking
    public Loading_Screen m_LoadingScreen;
    public float m_fLoadPercent = 0f;
    float m_fRoomCreationPercentage;
    float m_fBonusRoomCreationPercentage;
    float m_fRemoveDuplicateJunctionExits;
    float m_fRemoveDeadEndsPercentage;
    float m_fPrepareRoomTilesPercentage;
    float m_fNavCreatePercent;
    float m_fNavLinkPercent;
    float m_fNavCleanPercent;
    float m_fSceneryPlacementZoningPercent;
    float m_fTorchesPlacedPercentage;
    float m_fTreasureChestPlacedPercentage;
    float m_fJarsPlacedPercentage;
    float m_fWoodCratesPlacedPercentage;
    float m_fShadeSpawnerPlacedPercentage;
    //float m_fCurrentRooms;
    //float m_fTotalRooms;
    //float m_fExit;


    //Build Variables
    RoomAttributes m_RoomToGenerate;
    RoomExit m_exitSelection;
    RoomGenerationStep m_eRoomGenerationStep;
    TileMapCreationStep m_eTileMapCreationStep;
    bool m_bBuilding;
    int m_nRoomTileRangeMin;
    int m_nRoomTileRangeMax;
    int m_nHallwayTileRangeMin;
    int m_nHallwayTileRangeMax;
    int m_nCurrentAreaExit;
    int m_nAreaBosses;
    IndoorDungeonGeneratorBuildProgress m_eCurrentBuildSection;
    int m_nCurrentFirstRoomAttempts;
    int m_nMaxFirstRoomAttempts;
    bool m_bFirstRoomSuccessful;
    int m_nBonusRoomMaxAttempts;
    int m_nBonusRoomAttempts;
    int m_nExit;
    int m_nRoom;
    int m_nNavNodesCreated;
    int m_nRoomSpawnPercentage;
    int m_nHallwaySpawnPercentage; //If it's not a room or hallway, it's a junction for now, no need for a variable
    public bool m_bCreateInteriorObjects; //True to create all interior objects (Crates / Jars / Treasure Chests, etc. Turn off to test basic node navigation with no obstacles
    public bool m_bCreateLightBlockingWalls; //True to create cubes around every room to keep light from touching nearby rooms
    //End Build Variables
    //Build Interior Variables
    int m_BI_nMaxMonsterSpawners;
    int m_BI_nCurrentMonsterSpawners;
    float m_BI_fSpawnChance;
    float m_BI_fTorchSpawnPercent;
    float m_BI_fTreasureChestSpawnPercent;
    float m_BI_fShadeSpawnerPercentage;
    float m_BI_fFoodJarSpawnPercent;
    float m_BI_fWoodenCrateSpawnPercent;
    int m_BI_nObjectCount;
    int m_BI_nIndexSelection;
    List<Index2D> m_BI_vValidTileIndices;
    //End Build Interior

    //Physics2D Material
    public PhysicsMaterial2D m_Physics2DMaterial;

    // Use this for initialization
    void Start()
    {

    }

    public void LoadIndoorDungeonGenerator()
    {
        List<string> tempTileMapStrings = new List<string>();
        m_CircularRoomBuffer = new List<Rect>();

        tempTileMapStrings.Add("Sprites/BasicDungeon_Tileset_Complete_v4");
        //tempTileMapStrings.Add ("Sprites/BasicDungeon_Tileset_FloorRandomizer_1");
        Initialize(Random.Range(75, 150), Vector3.zero, tempTileMapStrings);

        //		//Hallway Test
        //		Tile_Map roomToAdd = Instantiate (Resources.Load ("Prefabs/Tile_Map", typeof(Tile_Map)), m_vStartingRoomCenter, Quaternion.identity) as Tile_Map;
        //	
        //		roomToAdd.Initialization (17, 21, TMBuildType.Hallway_Vertical);
        //		roomToAdd.CreateTileMap (tempTileMapStrings[0]);
        //		roomToAdd.SetCorners ();

        //				//Junction Test
        //				Tile_Map roomToAdd = Instantiate (Resources.Load ("Prefabs/Tile_Map", typeof(Tile_Map)), m_vStartingRoomCenter, Quaternion.identity) as Tile_Map;
        //			
        //				roomToAdd.Initialization (7, 7, TMBuildType.Junction);
        //				roomToAdd.CreateTileMap (tempTileMapStrings [0]);
        //				roomToAdd.SetCorners ();
        ////		roomToAdd.OpenHallway (0, Direction.Up);
        ////		roomToAdd.OpenHallway (0, Direction.Right);
        //		roomToAdd.OpenHallway (0, Direction.Down);
        //
        //				roomToAdd.JunctionEdit (0, Direction.Left, Direction.Down);
    }

    public void Initialize(int nTotalRooms, Vector3 vFirstRoomCenter, List<string> listofTileMaps)
    {
        //for use with fancier generation
        m_nTotalRooms = nTotalRooms;
        Debug.Log("Area Begin...");

        m_nCurrentRooms = 0;
        m_nExitSize = 8;
        m_fTileSize = .32f;

        m_vStartingRoomCenter = vFirstRoomCenter;

        m_TileMapLocations = listofTileMaps;
        //We are deprecating this with GlobalTileMap info
        m_TerrainTextures.Add((Texture2D)Resources.Load(m_TileMapLocations[0]));

        //Using a new object to store all of our Texture Data Globally for additional tile maps we create
        m_GlobalTileMaps = new List<GlobalTileMapData>();
        GlobalTileMapData TileMapToAdd = new GlobalTileMapData();
        TileMapToAdd.TerrainTexture = (Texture2D)Resources.Load(m_TileMapLocations[0]);
        TileMapToAdd.TileResolution = 64; //Tiles are 64 x 64, change this if we need to change tile sizes
        TileMapToAdd.TilesPerRow = TileMapToAdd.TerrainTexture.width / TileMapToAdd.TileResolution;
        TileMapToAdd.Rows = TileMapToAdd.TerrainTexture.height / TileMapToAdd.TileResolution;

        //Get Tile Data for the terrain texture we just added
        TileMapToAdd.TerrainTextureTileData = TileSeperator(TileMapToAdd);

        m_GlobalTileMaps.Add(TileMapToAdd);


        m_Rooms = new List<Tile_Map>();
        m_UnusedExits = new List<RoomExit>();
        m_UsedExits = new List<RoomExit>();
        m_UnusedHallwayExits = new List<RoomExit>();
        m_UnusedJunctionExits = new List<RoomExit>();
        possibleExits = new List<Direction>();
        m_Interactables = new List<Interactable>();
        m_AreaExits = new List<AreaExit>();
        m_MapAreaExits = new List<Tile_Map>();
        m_DungeonBranchEndRooms = new List<Tile_Map>();
        m_DeadEndRooms = new Queue<Tile_Map>();
        m_RoomToGenerate = new RoomAttributes();
        //m_Player = GameObject.FindGameObjectWithTag("Player1").GetComponent<BlueOrc_PC>();

        //Monster Spawners
        m_ShadeSpawners = new List<Spawner_Shade>();

        ResetBuildVariables();

        //Start Building
        m_bBuilding = true;
        //BuildArea();
    }

    void ClearGenerationContainers()
    {
        m_Rooms.Clear();
        m_UnusedExits.Clear();
        m_UsedExits.Clear();
        m_UnusedHallwayExits.Clear();
        m_UnusedJunctionExits.Clear();
        possibleExits.Clear();
        m_AreaExits.Clear();
        m_MapAreaExits.Clear();
        m_DungeonBranchEndRooms.Clear();
        //m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.FirstRoom;
        m_nCurrentRooms = 0;
    }

    //Called once per frame
    void Update()
    {
        //TODO:Call BuildArea here and we need to pass in what part we're working on and possibly change some local variables to class variables..
        //Gotta get smooth loading screens so we'll update our dungeon generator here and keep track of loading variables
        //should pass in whatever enumerator for the current section is and keep track of that value m_eCurrentBuildSection
        //IndoorDungeonGeneratorBuildProgress.FirstRoom
        if (m_bBuilding == true)
        {
            BuildArea();
        }
    }

    void ResetBuildVariables()
    {
        //TODO: Reset Every Variable used in BuildArea() here...
        m_eTileMapCreationStep = TileMapCreationStep.SetTileMapVariables;
        m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.FirstRoom;
        m_eRoomGenerationStep = RoomGenerationStep.RollRoomTypeAndAttributes;

        m_nCurrentRooms = 0;
        m_nCurrentFirstRoomAttempts = 0;
        m_nMaxFirstRoomAttempts = 20;
        m_bFirstRoomSuccessful = false;
        m_nRoomTileRangeMin = 5;
        m_nRoomTileRangeMax = 10;
        m_nHallwayTileRangeMin = 3;
        m_nHallwayTileRangeMax = 10;

        //Room / Hallway / Junction Percentages
        m_nRoomSpawnPercentage = 40; //45
        m_nHallwaySpawnPercentage = 50; //21

        m_nBonusRoomMaxAttempts = 3;
        m_nBonusRoomAttempts = 0;

        m_nExit = 0;
        m_nRoom = 0;
        m_nNavNodesCreated = 0;

        m_nAreaBosses = 2;
        m_nCurrentAreaExit = 0;

        //Build Interior Variables
        m_BI_nMaxMonsterSpawners = 2;
        m_BI_nCurrentMonsterSpawners = 0;
        m_BI_fSpawnChance = 0f;
        m_BI_fTorchSpawnPercent = 45.0f;
        m_BI_fTreasureChestSpawnPercent = 33.0f;
        m_BI_fShadeSpawnerPercentage = 30.0f;
        m_BI_fFoodJarSpawnPercent = 40.0f;
        m_BI_fWoodenCrateSpawnPercent = 55.0f;
        m_BI_nObjectCount = 0;
        m_BI_nIndexSelection = 0;
        m_BI_vValidTileIndices = new List<Index2D>();
        //End Build Interior

        //pulled out m_fBonusRoomCreation Deprecated for now
        m_fBonusRoomCreationPercentage = 30f;
        //Loading Bar values
        m_fRoomCreationPercentage = 40f;
        //m_fBonusRoomCreationPercentage = m_fRoomCreationPercentage + 15f;
        m_fRemoveDuplicateJunctionExits = m_fRoomCreationPercentage + 5f;
        m_fRemoveDeadEndsPercentage = m_fRemoveDuplicateJunctionExits + 5f;
        m_fPrepareRoomTilesPercentage = m_fRemoveDeadEndsPercentage + 5f;
        m_fNavCreatePercent = m_fPrepareRoomTilesPercentage + 5f;
        m_fNavLinkPercent = m_fNavCreatePercent + 5f;
        m_fNavCleanPercent = m_fNavLinkPercent + 5f;
        m_fSceneryPlacementZoningPercent = m_fNavCleanPercent + 5f;
        m_fTorchesPlacedPercentage = m_fSceneryPlacementZoningPercent + 5f;
        m_fTreasureChestPlacedPercentage = m_fTorchesPlacedPercentage + 5f;
        m_fJarsPlacedPercentage = m_fTreasureChestPlacedPercentage + 5f;
        m_fWoodCratesPlacedPercentage = m_fJarsPlacedPercentage + 5f;
        m_fShadeSpawnerPlacedPercentage = m_fWoodCratesPlacedPercentage + 5f;
        //m_fRoomCreationPercentage = .2f;
        //m_fBonusRoomCreationPercentage = m_fRoomCreationPercentage * 2f;
        //m_fPrepareRoomTilesPercentage = m_fBonusRoomCreationPercentage + .1f;
        //m_fTorchesPlacedPercentage = m_fPrepareRoomTilesPercentage + .1f;
        //m_fTreasureChestPlacedPercentage = m_fTorchesPlacedPercentage + .1f;
        //m_fJarsPlacedPercentage = m_fTreasureChestPlacedPercentage + .1f;
        //m_fWoodCratesPlacedPercentage = m_fJarsPlacedPercentage + .1f;
        //m_fShadeSpawnerPlacedPercentage = m_fWoodCratesPlacedPercentage + .1f;

    }

    /// <summary>
    /// Extracts each Tile from a texture for use by the GlobalTileMap object
    /// </summary>
    /// <returns>Array that contains one of each Tile.  The GlobalTileMap references these separated Tiles.</returns>
    Color[][] TileSeperator(GlobalTileMapData ourGlobalTileMap)
    {
        Color[][] tiles = new Color[ourGlobalTileMap.TilesPerRow * ourGlobalTileMap.Rows][];

        for (int y = 0; y < ourGlobalTileMap.Rows; y++)
        {
            for (int x = 0; x < ourGlobalTileMap.TilesPerRow; x++)
            {
                tiles[y * ourGlobalTileMap.TilesPerRow + x] = ourGlobalTileMap.TerrainTexture.GetPixels(x * ourGlobalTileMap.TileResolution, y * ourGlobalTileMap.TileResolution, ourGlobalTileMap.TileResolution, ourGlobalTileMap.TileResolution);
            }
        }
        return tiles;
    }

    public void StartAllSpawners()
    {
        for (int nSpawner = 0; nSpawner < m_ShadeSpawners.Count; nSpawner++)
        {
            foreach (Transform child in m_ShadeSpawners[nSpawner].transform)
            {
                child.gameObject.SetActive(true);
            }
            m_ShadeSpawners[nSpawner].gameObject.SetActive(true);
        }
    }

    //Set Each gameobject to it's room
    void ReturnRoomLocation()
    {

    }

    void BuildArea()
    {
        if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.FirstRoom)
        {
            Tile_Map roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), m_vStartingRoomCenter, Quaternion.identity) as Tile_Map;

            //find 2 odd numbers for now
            int nX = Random.Range(5, 10) * 2 + 1;
            int nY = Random.Range(5, 10) * 2 + 1;
            //Debug.Log ("Room#: " + m_nCurrentRooms);
            roomToAdd.Initialization(nX, nY);
            //roomToAdd.CreateTileMap (m_TileMapLocations [0]);
            roomToAdd.CreateTileMap(m_TerrainTextures[0]);
            roomToAdd.SetCorners();
            roomToAdd.m_RoomType = TMBuildType.Basic_Room;
            m_Rooms.Add(roomToAdd);

            m_nCurrentRooms++;

            //Start Our LevelGraph
            m_LevelGraph = new GenericGraph<Tile_Map>();
            m_LevelGraph.AddNode(roomToAdd);

            roomToAdd.m_RoomID = m_Rooms.Count - 1;

            List<Direction> currentExits = new List<Direction>();

            InsertRandomExits(1, currentExits);

            int nMaxOffset = 0;

            for (int nUnusedExit = 0; nUnusedExit < currentExits.Count; nUnusedExit++)
            {
                RoomExit exitToAdd = new RoomExit();
                exitToAdd.RoomID = m_Rooms.Count - 1;
                exitToAdd.ExitDirection = currentExits[nUnusedExit];

                nMaxOffset = CalculateMaxOffset(nX, nY, exitToAdd.ExitDirection);

                exitToAdd.OffSet = Random.Range(-nMaxOffset, nMaxOffset);//need to calculate other offsets
                exitToAdd.WorldPosition = roomToAdd.GetExitPoint(exitToAdd.OffSet, exitToAdd.ExitDirection);
                m_UnusedExits.Add(exitToAdd);
            }
            currentExits.Clear();

            //First Room Pass After StarterRoom...we want to make sure we don't infinite loop before we place more exits
            m_nMaxFirstRoomAttempts = 20;
            m_nCurrentFirstRoomAttempts = 0;
            m_bFirstRoomSuccessful = false;
            while (m_nCurrentFirstRoomAttempts < m_nMaxFirstRoomAttempts && !m_bFirstRoomSuccessful)
            {
                RoomExit exitSelection = ChooseUnusedExit();

                int nRandomFeature = Random.Range(0, 100);
                if (nRandomFeature >= 45)
                    m_bFirstRoomSuccessful = TestNextRoom(exitSelection.OffSet, exitSelection.ExitDirection, m_Rooms[exitSelection.RoomID], exitSelection, 5, 10, 4);
                else if (exitSelection.BuildType != TMBuildType.Hallway && nRandomFeature >= 21)
                    m_bFirstRoomSuccessful = TestNextHallway(exitSelection.OffSet, exitSelection.ExitDirection, m_Rooms[exitSelection.RoomID], exitSelection, 3, 10);
                else
                    m_bFirstRoomSuccessful = TestJunction(exitSelection.OffSet, exitSelection.ExitDirection, m_Rooms[exitSelection.RoomID], exitSelection);

                m_nCurrentFirstRoomAttempts++;
            }
            //First Room Complete - Move to Next Section
            Debug.Log("First Room Complete");
            m_nCurrentFirstRoomAttempts = 0;
            m_eRoomGenerationStep = RoomGenerationStep.RollRoomTypeAndAttributes;
            m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.RoomCreation;
            if (!m_bFirstRoomSuccessful)
            {
                //we need to restart...can't create the first room for some reason
                Destroy(m_Rooms[0].gameObject);
                m_Rooms.Clear();
                m_UnusedExits.Clear();

                ClearGenerationContainers();
                ResetBuildVariables();

                //SAFETY, WE SHOULD NEVER ENCOUNTER THIS...Sometimes we do though :D
                Debug.Log("First Room Failed, Rebuilding...");

                return;
            }
            Debug.Log("First Room Successful after " + m_nCurrentFirstRoomAttempts + " attempts.");
            return;
        }//END IndoorDungeonGeneratorBuildProgress.FirstRoom
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.RoomCreation)
        {
            //if (m_nCurrentRooms < m_nTotalRooms && m_nCurrentFirstRoomAttempts < m_nMaxFirstRoomAttempts)
            //{
            //    //TODO: Unfortunately we need to break this up into chunks to make this smoother.
            //    //Steps:
            //    //1a) Use an exit selection to store a possible room in a new member structure.  Store width/height/buildtype/currentCenter (found with nEntranceoffset)
            //    //1b) Decide what type of room we're using.
            //    //2) Test if theoretical room can be placed
            //    //3) Open Previous Room
            //    //4) Create New Room
            //    //5) Open Current Room
            //    //6) Add new exits (CreateUnusedExits)
            //    RoomExit exitSelection = ChooseUnusedExit();

            //    int nRandomFeature = Random.Range(0, 100);
            //    int nRoomChance = 100 - m_nRoomSpawnPercentage;
            //    if (nRandomFeature >= nRoomChance)
            //        m_bFirstRoomSuccessful = TestNextRoom(exitSelection.OffSet, exitSelection.ExitDirection, m_Rooms[exitSelection.RoomID], exitSelection, 5, 10, 4);
            //    else if (exitSelection.BuildType != TMBuildType.Hallway && nRandomFeature >= (nRoomChance - m_nHallwaySpawnPercentage))
            //        m_bFirstRoomSuccessful = TestNextHallway(exitSelection.OffSet, exitSelection.ExitDirection, m_Rooms[exitSelection.RoomID], exitSelection, 3, 10);
            //    else
            //        m_bFirstRoomSuccessful = TestJunction(exitSelection.OffSet, exitSelection.ExitDirection, m_Rooms[exitSelection.RoomID], exitSelection);

            //    m_nCurrentFirstRoomAttempts++;
            //    if (m_bFirstRoomSuccessful)
            //        m_nCurrentFirstRoomAttempts = 0;

            //    m_fCurrentRooms = (float)m_nCurrentRooms;
            //    m_fTotalRooms = (float)m_nTotalRooms;

            //    m_fLoadPercent = Mathf.Lerp(0f, m_fRoomCreationPercentage, ((float)m_nCurrentRooms) / ((float)m_nTotalRooms));
            //    return;
            //}
            //else
            //{
            //    //All Rooms Complete - Next Check to Make sure we have more than 2
            //    Debug.Log("All Rooms Complete");
            //    m_fLoadPercent = Mathf.Lerp(0f, m_fRoomCreationPercentage, m_fCurrentRooms / m_fTotalRooms);
            //    m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.DungeonSizeCheck;
            //    return;
            //}
            if (m_nCurrentRooms < m_nTotalRooms && m_nCurrentFirstRoomAttempts < m_nMaxFirstRoomAttempts)
            {
                //new stuff here
                if (m_eRoomGenerationStep == RoomGenerationStep.RollRoomTypeAndAttributes)
                {
                    
                    m_exitSelection = ChooseUnusedExit();

                    int nRandomFeature = Random.Range(0, 100);
                    int nRoomChance = 100 - m_nRoomSpawnPercentage;
                    if (nRandomFeature >= nRoomChance)
                        m_RoomToGenerate.BuildType = TMBuildType.Basic_Room;
                    else if (m_exitSelection.BuildType != TMBuildType.Hallway && nRandomFeature >= (nRoomChance - m_nHallwaySpawnPercentage))
                        m_RoomToGenerate.BuildType = TMBuildType.Hallway;
                    else
                        m_RoomToGenerate.BuildType = TMBuildType.Junction; //TestJunction calls openhallway on the first pass!

                    switch (m_RoomToGenerate.BuildType)
                    {
                        case TMBuildType.Basic_Room:
                            {

                                int nMaxOffSet = 0;
                                int nEntranceOffSet = 0;
                                Vector3 vCurrentCenter = Vector3.zero;

                                m_RoomToGenerate.TileWidth = Random.Range(m_nRoomTileRangeMin, m_nRoomTileRangeMax) * 2 + 1;
                                m_RoomToGenerate.TileHeight = Random.Range(m_nRoomTileRangeMin, m_nRoomTileRangeMax) * 2 + 1;

                                if (m_exitSelection.ExitDirection == Direction.Right)
                                    nMaxOffSet = CalculateMaxOffset(m_RoomToGenerate.TileWidth, m_RoomToGenerate.TileHeight, Direction.Left);
                                else if (m_exitSelection.ExitDirection == Direction.Left)
                                    nMaxOffSet = CalculateMaxOffset(m_RoomToGenerate.TileWidth, m_RoomToGenerate.TileHeight, Direction.Right);
                                else if (m_exitSelection.ExitDirection == Direction.Up)
                                    nMaxOffSet = CalculateMaxOffset(m_RoomToGenerate.TileWidth, m_RoomToGenerate.TileHeight, Direction.Down);
                                else if (m_exitSelection.ExitDirection == Direction.Down)
                                    nMaxOffSet = CalculateMaxOffset(m_RoomToGenerate.TileWidth, m_RoomToGenerate.TileHeight, Direction.Up);

                                nEntranceOffSet = Random.Range(-nMaxOffSet, nMaxOffSet);

                                m_RoomToGenerate.EntranceOffset = nEntranceOffSet;
                              
                                if (m_exitSelection.ExitDirection == Direction.Right)
                                {
                                    vCurrentCenter = new Vector3(((m_RoomToGenerate.TileWidth - 1) * m_fTileSize * .5f), 0f, 0f);
                                    vCurrentCenter += m_exitSelection.WorldPosition;

                                    //vCurrentCenter.y += (nEntranceOffSet * m_fTileSize);

                                    vCurrentCenter.y += (nEntranceOffSet * m_fTileSize);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Left)
                                {
                                    vCurrentCenter = new Vector3(((m_RoomToGenerate.TileWidth - 1) * -m_fTileSize * .5f), 0f, 0f);
                                    vCurrentCenter += m_exitSelection.WorldPosition;

                                    vCurrentCenter.y += (nEntranceOffSet * m_fTileSize);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Up)
                                {
                                    vCurrentCenter = new Vector3(0f, ((m_RoomToGenerate.TileHeight - 1) * m_fTileSize * .5f), 0f);
                                    vCurrentCenter += m_exitSelection.WorldPosition;

                                    //vCurrentCenter.x += (nEntranceOffSet * m_fTileSize);

                                    vCurrentCenter.x += (nEntranceOffSet * m_fTileSize);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Down)
                                {
                                    vCurrentCenter = new Vector3(0f, ((m_RoomToGenerate.TileHeight - 1) * -m_fTileSize * .5f), 0f);
                                    vCurrentCenter += m_exitSelection.WorldPosition;

                                    vCurrentCenter.x += (nEntranceOffSet * m_fTileSize);
                                }

                                m_RoomToGenerate.CurrentCenter = vCurrentCenter;

                                break;
                            }
                        case TMBuildType.Hallway:
                            {
                                //offset = 0
                                Vector3 vCurrentCenter = Vector3.zero;

                                m_RoomToGenerate.EntranceOffset = 0;

                                if (m_exitSelection.ExitDirection == Direction.Right)
                                {
                                    m_RoomToGenerate.TileWidth = Random.Range(m_nHallwayTileRangeMin, m_nHallwayTileRangeMax) * 2 + 1;
                                    m_RoomToGenerate.TileHeight = 7;
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Left)
                                {
                                    m_RoomToGenerate.TileWidth = Random.Range(m_nHallwayTileRangeMin, m_nHallwayTileRangeMax) * 2 + 1;
                                    m_RoomToGenerate.TileHeight = 7;
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Up)
                                {
                                    m_RoomToGenerate.TileWidth = 7;
                                    m_RoomToGenerate.TileHeight = Random.Range(m_nHallwayTileRangeMin, m_nHallwayTileRangeMax) * 2 + 1;
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Down)
                                {
                                    m_RoomToGenerate.TileWidth = 7;
                                    m_RoomToGenerate.TileHeight = Random.Range(m_nHallwayTileRangeMin, m_nHallwayTileRangeMax) * 2 + 1;
                                }
                             
                                if (m_exitSelection.ExitDirection == Direction.Right)
                                {
                                    vCurrentCenter = new Vector3(((m_RoomToGenerate.TileWidth - 1) * m_fTileSize * .5f), 0f, 0f);
                                    vCurrentCenter += m_exitSelection.WorldPosition;
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Left)
                                {
                                    vCurrentCenter = new Vector3(((m_RoomToGenerate.TileWidth - 1) * -m_fTileSize * .5f), 0f, 0f);
                                    vCurrentCenter += m_exitSelection.WorldPosition;
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Up)
                                {
                                    vCurrentCenter = new Vector3(0f, ((m_RoomToGenerate.TileHeight - 1) * m_fTileSize * .5f), 0f);
                                    vCurrentCenter += m_exitSelection.WorldPosition;
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Down)
                                {
                                    vCurrentCenter = new Vector3(0f, ((m_RoomToGenerate.TileHeight - 1) * -m_fTileSize * .5f), 0f);
                                    vCurrentCenter += m_exitSelection.WorldPosition;
                                }
                                m_RoomToGenerate.CurrentCenter = vCurrentCenter;
                                break;
                            }
                        case TMBuildType.Junction:
                            {
                                //offset = 0
                                Vector3 vCurrentCenter = Vector3.zero;

                                m_RoomToGenerate.EntranceOffset = 0;

                                m_RoomToGenerate.TileWidth = 7;
                                m_RoomToGenerate.TileHeight = 7;

                                if (m_exitSelection.ExitDirection == Direction.Right)
                                {
                                    vCurrentCenter = new Vector3(((m_RoomToGenerate.TileWidth - 1) * m_fTileSize * .5f), 0f, 0f);
                                    vCurrentCenter += m_exitSelection.WorldPosition;
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Left)
                                {
                                    vCurrentCenter = new Vector3(((m_RoomToGenerate.TileWidth - 1) * -m_fTileSize * .5f), 0f, 0f);
                                    vCurrentCenter += m_exitSelection.WorldPosition;
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Up)
                                {
                                    vCurrentCenter = new Vector3(0f, ((m_RoomToGenerate.TileHeight - 1) * m_fTileSize * .5f), 0f);
                                    vCurrentCenter += m_exitSelection.WorldPosition;
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Down)
                                {
                                    vCurrentCenter = new Vector3(0f, ((m_RoomToGenerate.TileHeight - 1) * -m_fTileSize * .5f), 0f);
                                    vCurrentCenter += m_exitSelection.WorldPosition;
                                }
                                m_RoomToGenerate.CurrentCenter = vCurrentCenter;
                                break;
                            }
                    }

                    m_fLoadPercent = Mathf.Lerp(0f, m_fRoomCreationPercentage, ((float)m_nCurrentRooms) / ((float)m_nTotalRooms));

                    m_eRoomGenerationStep = RoomGenerationStep.TestPlacement;
                    return;
                }
                else if (m_eRoomGenerationStep == RoomGenerationStep.TestPlacement)
                {
                    //TestRoomPlacement(nTestX, nTestY, vCurrentCenter, connectingExit.RoomID
                    if (TestRoomPlacement(m_RoomToGenerate.TileWidth, m_RoomToGenerate.TileHeight, m_RoomToGenerate.CurrentCenter, m_exitSelection.RoomID))
                    {
                        //we failed

                        if (m_exitSelection.BuildType == TMBuildType.Hallway)
                            m_UnusedHallwayExits.Remove(m_exitSelection);
                        else if (m_exitSelection.BuildType == TMBuildType.Junction)
                            m_UnusedJunctionExits.Remove(m_exitSelection);

                        m_nCurrentFirstRoomAttempts++;

                        m_eRoomGenerationStep = RoomGenerationStep.RollRoomTypeAndAttributes;
                    }
                    else
                    {
                        //success
                        m_nCurrentFirstRoomAttempts = 0;
                        m_eRoomGenerationStep = RoomGenerationStep.OpenPreviousRoom;

                    }

                    //m_fLoadPercent = Mathf.Lerp(0f, m_fRoomCreationPercentage, ((float)m_nCurrentRooms) / ((float)m_nTotalRooms));

                    return;
                }
                else if (m_eRoomGenerationStep == RoomGenerationStep.OpenPreviousRoom)
                {
                    OpenPreviousRoom(m_Rooms[m_exitSelection.RoomID], m_exitSelection, m_exitSelection.ExitDirection);

                    m_fLoadPercent = Mathf.Lerp(0f, m_fRoomCreationPercentage, ((float)m_nCurrentRooms) / ((float)m_nTotalRooms));

                    m_eRoomGenerationStep = RoomGenerationStep.GenerateNewRoom;

                    return;
                }
                else if (m_eRoomGenerationStep == RoomGenerationStep.GenerateNewRoom)
                {
                    Tile_Map roomToAdd = null;

                    roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), m_RoomToGenerate.CurrentCenter, Quaternion.identity) as Tile_Map;

                    switch(m_RoomToGenerate.BuildType)
                    {
                        case TMBuildType.Basic_Room:
                            {
                                roomToAdd.Initialization(m_RoomToGenerate.TileWidth, m_RoomToGenerate.TileHeight);

                                roomToAdd.m_RoomType = TMBuildType.Basic_Room;
                                m_RoomToGenerate.RoomType = TMBuildType.Basic_Room;

                                break;
                            }
                        case TMBuildType.Hallway:
                            {
                                roomToAdd.Initialization(m_RoomToGenerate.TileWidth, m_RoomToGenerate.TileHeight, TMBuildType.Basic_Room);

                                if (m_exitSelection.ExitDirection == Direction.Right)
                                {
                                    roomToAdd.m_RoomType = TMBuildType.Hallway_Horizontal;
                                    m_RoomToGenerate.RoomType = TMBuildType.Hallway_Horizontal;
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Left)
                                {
                                    roomToAdd.m_RoomType = TMBuildType.Hallway_Horizontal;
                                    m_RoomToGenerate.RoomType = TMBuildType.Hallway_Horizontal;
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Up)
                                {
                                    roomToAdd.m_RoomType = TMBuildType.Hallway_Vertical;
                                    m_RoomToGenerate.RoomType = TMBuildType.Hallway_Vertical;
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Down)
                                {
                                    roomToAdd.m_RoomType = TMBuildType.Hallway_Vertical;
                                    m_RoomToGenerate.RoomType = TMBuildType.Hallway_Vertical;
                                }

                                break;
                            }
                        case TMBuildType.Junction:
                            {
                                roomToAdd.Initialization(m_RoomToGenerate.TileWidth, m_RoomToGenerate.TileHeight, TMBuildType.Junction);

                                roomToAdd.m_RoomType = TMBuildType.Junction;
                                m_RoomToGenerate.RoomType = TMBuildType.Junction;

                                break;
                            }
                    }

                    //TODO: We NEED to make this split this function up into the following:
                    //1)Tile Separator - ALSO optimize m_TileBuffer by storing each tilemap in a global storage container in IndoorDungeonGenerator.
                    //If it's already been separated, just set the global variable to m_TileBuffer
                    //2)Create Mesh
                    //3)Create Walls
                    //4)Create Texture
                    m_eTileMapCreationStep = TileMapCreationStep.SetTileMapVariables;

                    //OLD METHOD - Broken into smaller parts
                    //roomToAdd.CreateTileMap(m_TerrainTextures[0]);
                    //roomToAdd.SetCorners();

                    m_Rooms.Add(roomToAdd);

                    roomToAdd.m_RoomID = m_Rooms.Count - 1;

                    m_fLoadPercent = Mathf.Lerp(0f, m_fRoomCreationPercentage, ((float)m_nCurrentRooms) / ((float)m_nTotalRooms));

                    m_eRoomGenerationStep = RoomGenerationStep.WorkOnTileMap;

                    return;
                }
                else if (m_eRoomGenerationStep == RoomGenerationStep.WorkOnTileMap)
                {
                    //m_eRoomGenerationStep = RoomGenerationStep.OpenNewRoom;
                    switch (m_eTileMapCreationStep)
                    {
                        case TileMapCreationStep.SetTileMapVariables:
                            {
                                //**IMPORTANT** - THIS IS WHAT CONTROLS WHICH TILE MAP WE USE FOR A ROOM
                                m_Rooms[m_Rooms.Count-1].AddTileMap(m_GlobalTileMaps[0]);

                                m_eTileMapCreationStep = TileMapCreationStep.SetMesh;
                                break;
                            }
                        case TileMapCreationStep.SetMesh:
                            {
                                m_Rooms[m_Rooms.Count - 1].CreateMesh(Vector3.zero);

                                m_eTileMapCreationStep = TileMapCreationStep.SetWalls;
                                break;
                            }
                        case TileMapCreationStep.SetWalls:
                            {
                                m_Rooms[m_Rooms.Count - 1].BuildWalls();

                                m_eTileMapCreationStep = TileMapCreationStep.SetTexture;
                                break;
                            }
                        case TileMapCreationStep.SetTexture:
                            {
                                m_Rooms[m_Rooms.Count - 1].CreateTexture();
                                m_Rooms[m_Rooms.Count - 1].SetCorners();


                                m_eTileMapCreationStep = TileMapCreationStep.DONE;
                                m_eRoomGenerationStep = RoomGenerationStep.OpenNewRoom;
                                break;
                            }
                    }

                    return;
                }
                else if (m_eRoomGenerationStep == RoomGenerationStep.OpenNewRoom)
                {
                    switch (m_RoomToGenerate.BuildType)
                    {
                        case TMBuildType.Basic_Room:
                            {
                                if (m_exitSelection.ExitDirection == Direction.Right)
                                    m_Rooms[m_Rooms.Count - 1].OpenRoom(-m_RoomToGenerate.EntranceOffset, Direction.Left);
                                else if (m_exitSelection.ExitDirection == Direction.Left)
                                    m_Rooms[m_Rooms.Count - 1].OpenRoom(-m_RoomToGenerate.EntranceOffset, Direction.Right);
                                else if (m_exitSelection.ExitDirection == Direction.Up)
                                    m_Rooms[m_Rooms.Count - 1].OpenRoom(-m_RoomToGenerate.EntranceOffset, Direction.Down);
                                else if (m_exitSelection.ExitDirection == Direction.Down)
                                    m_Rooms[m_Rooms.Count - 1].OpenRoom(-m_RoomToGenerate.EntranceOffset, Direction.Up);

                                break;
                            }
                        case TMBuildType.Hallway:
                            {
                                if (m_exitSelection.ExitDirection == Direction.Right)
                                    m_Rooms[m_Rooms.Count - 1].OpenHallway(0, Direction.Left);
                                else if (m_exitSelection.ExitDirection == Direction.Left)
                                    m_Rooms[m_Rooms.Count - 1].OpenHallway(0, Direction.Right);
                                else if (m_exitSelection.ExitDirection == Direction.Up)
                                    m_Rooms[m_Rooms.Count - 1].OpenHallway(0, Direction.Down);
                                else if (m_exitSelection.ExitDirection == Direction.Down)
                                    m_Rooms[m_Rooms.Count - 1].OpenHallway(0, Direction.Up);

                                break;
                            }
                        case TMBuildType.Junction:
                            {

                                if (m_exitSelection.ExitDirection == Direction.Right)
                                {
                                    m_Rooms[m_Rooms.Count - 1].m_GenerationEntrance = Direction.Left;
                                    m_Rooms[m_Rooms.Count - 1].OpenHallway(0, Direction.Left);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Left)
                                {
                                    m_Rooms[m_Rooms.Count - 1].m_GenerationEntrance = Direction.Right;
                                    m_Rooms[m_Rooms.Count - 1].OpenHallway(0, Direction.Right);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Up)
                                {
                                    m_Rooms[m_Rooms.Count - 1].m_GenerationEntrance = Direction.Down;
                                    m_Rooms[m_Rooms.Count - 1].OpenHallway(0, Direction.Down);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Down)
                                {
                                    m_Rooms[m_Rooms.Count - 1].m_GenerationEntrance = Direction.Up;
                                    m_Rooms[m_Rooms.Count - 1].OpenHallway(0, Direction.Up);
                                }
                                break;
                            }
                    }

                    //previous room exit needs to know room ID of this room, this room's exit needs to know room ID of previous room
                    ExchangeRoomIDsForExits(m_Rooms[m_exitSelection.RoomID], m_Rooms[m_Rooms.Count - 1]);

                    m_eRoomGenerationStep = RoomGenerationStep.AddNewExits;

                    return;
                }
                else if (m_eRoomGenerationStep == RoomGenerationStep.AddNewExits)
                {
                    //TODO: add this
                    List<Direction> currentExits = new List<Direction>();

                    switch (m_RoomToGenerate.BuildType)
                    {
                        case TMBuildType.Basic_Room:
                            {
                                if (m_exitSelection.ExitDirection == Direction.Right)
                                {
                                    currentExits.Add(Direction.Right);
                                    currentExits.Add(Direction.Up);
                                    currentExits.Add(Direction.Down);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Left)
                                {
                                    currentExits.Add(Direction.Left);
                                    currentExits.Add(Direction.Up);
                                    currentExits.Add(Direction.Down);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Up)
                                {
                                    currentExits.Add(Direction.Up);
                                    currentExits.Add(Direction.Right);
                                    currentExits.Add(Direction.Left);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Down)
                                {
                                    currentExits.Add(Direction.Down);
                                    currentExits.Add(Direction.Right);
                                    currentExits.Add(Direction.Left);
                                }
                                break;
                            }
                        case TMBuildType.Hallway:
                            {
                                if (m_exitSelection.ExitDirection == Direction.Right)
                                {
                                    currentExits.Add(Direction.Right);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Left)
                                {
                                    currentExits.Add(Direction.Left);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Up)
                                {
                                    currentExits.Add(Direction.Up);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Down)
                                {
                                    currentExits.Add(Direction.Down);
                                }
                                break;
                            }
                        case TMBuildType.Junction:
                            {
                                if (m_exitSelection.ExitDirection == Direction.Right)
                                {
                                    currentExits.Add(Direction.Up);
                                    currentExits.Add(Direction.Down);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Left)
                                {
                                    currentExits.Add(Direction.Up);
                                    currentExits.Add(Direction.Down);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Up)
                                {
                                    currentExits.Add(Direction.Left);
                                    currentExits.Add(Direction.Right);
                                }
                                else if (m_exitSelection.ExitDirection == Direction.Down)
                                {
                                    currentExits.Add(Direction.Left);
                                    currentExits.Add(Direction.Right);
                                }
                                break;
                            }

                    }

                    CreateUnusedExit(currentExits, m_RoomToGenerate.TileWidth, m_RoomToGenerate.TileHeight, m_Rooms[m_Rooms.Count - 1], 0, m_RoomToGenerate.BuildType);

                    //m_fLoadPercent = Mathf.Lerp(0f, m_fRoomCreationPercentage, m_fCurrentRooms / m_fTotalRooms);

                    m_nCurrentRooms++;

                    m_eRoomGenerationStep = RoomGenerationStep.RollRoomTypeAndAttributes;

                    return;
                }
            }
            else
            {
                //we're done with room creation, move to
                //All Rooms Complete - Next Check to Make sure we have more than 2
                Debug.Log("All Rooms Complete");
                m_fLoadPercent = Mathf.Lerp(0f, m_fRoomCreationPercentage, ((float)m_nCurrentRooms) / ((float)m_nTotalRooms));
                m_eRoomGenerationStep = RoomGenerationStep.RollRoomTypeAndAttributes;
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.DungeonSizeCheck;
                return;
            }
        }//END IndoorDungeonGeneratorBuildProgress.RoomCreation
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.DungeonSizeCheck)
        {
            //Check to see if there are only 2 rooms...if so...clear my containers and call build again...
            if (m_Rooms.Count <= 2)
            {
                //we need to reroll...
                Debug.Log("Not Enough Rooms...Rerolling");

                for (int nRoom = 0; nRoom < m_Rooms.Count; nRoom++)
                {
                    Destroy(m_Rooms[nRoom].gameObject);
                }

                ClearGenerationContainers();
                //Build Bookmark Reset in ResetBuildVariables();
                ResetBuildVariables();
                Resources.UnloadUnusedAssets();

                BuildArea();
                return;
            }
            else //PASSED DUNGEON SIZE CHECK - Continue to Bonus Room Creation
            {
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.RemoveDuplicateJunctionExits; //Currently Skipping BonusRoom Creation for the sake of speed?
                m_nRoom = 0;
                return;
            }
        }//END IndoorDungeonGeneratorBuildProgress.DungeonSizeCheck
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.BonusRoomCreation)//DEPRECATED, REMOVING DEAD ENDS eliminated my need for this section, we skip it for now but keep it around in case I'd like to use it later
        {
            //add bonus rooms to all the nub hallway dead ends
            while (m_nExit < m_UnusedExits.Count)
            {
                m_nBonusRoomAttempts = 0;
                RoomExit targetExit = m_UnusedExits[m_nExit];
                if (targetExit.BuildType == TMBuildType.Hallway || targetExit.BuildType == TMBuildType.Junction)
                {
                    //Debug.Log ("Bonus Room");
                    //nHallwayAttempts++;

                    while (m_nBonusRoomAttempts <= m_nBonusRoomMaxAttempts && !TestNextRoom(targetExit.OffSet, targetExit.ExitDirection, m_Rooms[targetExit.RoomID], targetExit, 5, 10, 4))
                        m_nBonusRoomAttempts++;

                }
                m_nExit++;

                m_fLoadPercent = Mathf.Lerp(m_fRoomCreationPercentage, m_fBonusRoomCreationPercentage, ((float)m_nExit) / m_UnusedExits.Count);
                return;
            }
            //PASSED BonusRoomCreation - Continue to Room Tile Sync
            Debug.Log("Bonus Rooms Created");
            m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.RemoveDuplicateJunctionExits;
            m_fLoadPercent = Mathf.Lerp(m_fRoomCreationPercentage, m_fBonusRoomCreationPercentage, ((float)m_nExit) / m_UnusedExits.Count);
            m_nRoom = 0;
            return;
        }//END IndoorDungeonGeneratorBuildProgress.BonusRoomCreation
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.RemoveDuplicateJunctionExits)
        {
            //double check exits on junctions....
            //ALSO, we're going to double check exits here to prevent any duplicate info from junctions
            //Careful to not compare exits to themselves!!
            if (m_nRoom < m_Rooms.Count)
            {

                if (m_Rooms[m_nRoom].m_RoomType == TMBuildType.Junction)
                {
                    for (int nExit = 0; nExit < m_Rooms[m_nRoom].m_Exits.Count; nExit++)
                    {
                        for (int nOtherExit = nExit + 1; nOtherExit < m_Rooms[m_nRoom].m_Exits.Count; nOtherExit++)
                        {
                            if (m_Rooms[m_nRoom].m_Exits[nExit].Direction == m_Rooms[m_nRoom].m_Exits[nOtherExit].Direction)
                            {
                                m_Rooms[m_nRoom].m_Exits.RemoveAt(nOtherExit);
                            }
                        }
                    }
                }
                m_nRoom++;
                m_fLoadPercent = Mathf.Lerp(m_fRoomCreationPercentage, m_fRemoveDuplicateJunctionExits, ((float)m_nRoom) / m_Rooms.Count);//used to do bonus rooms, removed
                return;
            }
            else
            {
                Debug.Log("Junction Duplicate Exits Cleaned");
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.RemoveDeadEnds;
                m_fLoadPercent = Mathf.Lerp(m_fRoomCreationPercentage, m_fRemoveDuplicateJunctionExits, ((float)m_nRoom) / m_Rooms.Count);//used to do bonus rooms, removed
                m_nRoom = 0;
                return;
            }
        }//END RemoveDuplicateJunctionExits - We need to consider adding in Grooming, destroying any dead ends and literally trying some manual circularity before "SyncRoomTiles"
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.RemoveDeadEnds)
        {
            //TODO: We need to remove dead ends from my generated map
            //MAKE SURE TO UPDATE THE ROOM ID's for each room
            //MAKE SURE TO UPDATE the m_Exits collection in each room to reflect any exits we remove as a result of dead ends!
            //To Remove something from the list of Rooms, we need the exact Room Object, don't use ID's because they're constantly changing
            //ALSO EXTREMELY IMPORTANT
            //If while removing a dead end you encounter a 3-way junction (junction with 3 exits) STOP REMOVING ROOMS from that sequence or you'll cutt off that section of the dungeon
            if (m_nRoom < m_Rooms.Count)
            {
                Tile_Map theTargetRoom = m_Rooms[m_nRoom];
                //Tile_Map otherRoom;

                if (theTargetRoom.m_RoomType == TMBuildType.Hallway_Horizontal || theTargetRoom.m_RoomType == TMBuildType.Hallway_Vertical || theTargetRoom.m_RoomType == TMBuildType.Junction)
                {
                    if (theTargetRoom.m_Exits.Count <= 1)
                    {
                        //Debug.Log("Dead End Found");
                        m_DeadEndRooms.Enqueue(theTargetRoom);
                        CheckDeadEndSequence(theTargetRoom.m_Exits[0].OtherRoomID, m_nRoom);
                    }
                }
                m_nRoom++;
                m_fLoadPercent = Mathf.Lerp(m_fRemoveDuplicateJunctionExits, m_fRemoveDeadEndsPercentage, ((float)m_nRoom) / m_Rooms.Count);
                return;
            }
            else
            {
                //FindDeadEnds();
                //ok...now we need to destroy our list of game objects, REMOVE THEM FROM THE LIST OF ROOMS FIRST
                Tile_Map targetRoom;

                while (m_DeadEndRooms.Count > 0)
                {
                    targetRoom = m_DeadEndRooms.Dequeue();
                    m_Rooms.Remove(targetRoom);
                    Destroy(targetRoom.gameObject);
                }
                m_DeadEndRooms.Clear();

                //IMPORTANT - refresh all Room ID's
                for (int nAllRooms = 0; nAllRooms < m_Rooms.Count; nAllRooms++)
                {
                    m_Rooms[nAllRooms].m_RoomID = nAllRooms;
                }

                m_nRoom = m_Rooms.Count;
                Debug.Log("Remove Dead Ends Complete");
                m_fLoadPercent = Mathf.Lerp(m_fRemoveDuplicateJunctionExits, m_fRemoveDeadEndsPercentage, ((float)m_nRoom) / m_Rooms.Count);
                m_nRoom = 0;
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.SyncRoomTiles;
                return;
            }
        }//END RemoveDeadEnds
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.SyncRoomTiles)
        {

            if (m_nRoom < m_Rooms.Count)
            {
                m_Rooms[m_nRoom].SyncTileData();

                m_nRoom++;

                m_fLoadPercent = Mathf.Lerp(m_fRemoveDeadEndsPercentage, m_fPrepareRoomTilesPercentage, ((float)m_nRoom) / m_Rooms.Count);
                return;
            }
            else //PASSED SyncTileData - Continue to Navigation Node Data
            {
                Debug.Log("Tile Data Complete");
                m_fLoadPercent = Mathf.Lerp(m_fRemoveDeadEndsPercentage, m_fPrepareRoomTilesPercentage, ((float)m_nRoom) / m_Rooms.Count);
                m_nRoom = 0;
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.ErectLightBlockingWalls;
                return;
            }
        }//END SyncTileData
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.ErectLightBlockingWalls)
        {
            //Note: Shadows seem to look much better by setting the player light to "Point".  I use the "Spot" light when 
            if (!m_bCreateLightBlockingWalls)
                m_nRoom = m_Rooms.Count;

            if (m_nRoom < m_Rooms.Count)
            {
                m_Rooms[m_nRoom].ErectLightWalls();
                m_nRoom++;
                //m_fLoadPercent = Mathf.Lerp(m_fPrepareRoomTilesPercentage, m_fNavCreatePercent, ((float)m_nRoom) / m_Rooms.Count);
                return;
            }
            else
            {
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.NavNodes_Creation;
                Debug.Log("Done with Light Obstructing Walls");
                //m_fLoadPercent = Mathf.Lerp(m_fPrepareRoomTilesPercentage, m_fNavCreatePercent, ((float)m_nRoom) / m_Rooms.Count);
                m_nRoom = 0;
                return;
            }
        }//END Navigation Nodes
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.NavNodes_Creation)
        {
            if (m_nRoom < m_Rooms.Count)
            {


                //For each exit inside of a room, create a node that sits on the center tile to that exit
                for (int nExit = 0; nExit < m_Rooms[m_nRoom].m_Exits.Count; nExit++)
                {
                    switch (m_Rooms[m_nRoom].m_Exits[nExit].Direction)
                    {
                        case Direction.Down:
                            {
                                Vector3 vNavNodePosition = m_Rooms[m_nRoom].m_TileReferenceData[((int)((m_Rooms[m_nRoom].m_fWidthInTiles) * .5f) + m_Rooms[m_nRoom].m_Exits[nExit].Offset), 2].WorldLocation;

                                NavNode navNodeToCreate = Instantiate(Resources.Load("Prefabs/NavNode", typeof(NavNode)), vNavNodePosition, Quaternion.identity) as NavNode;
                                //navNodeToCreate.Initilialize();
                                navNodeToCreate.m_nNodeID = m_nNavNodesCreated;
                                m_nNavNodesCreated++;
                                navNodeToCreate.m_nRoomID = m_nRoom;
                                navNodeToCreate.m_ExitDirection = Direction.Down;

                                m_Rooms[m_nRoom].m_NavNodes.Add(navNodeToCreate);

                                //navNodeToCreate.m_NavNodeConnections.

                                break;
                            }
                        case Direction.Left:
                            {
                                Vector3 vNavNodePosition = m_Rooms[m_nRoom].m_TileReferenceData[2, ((int)((m_Rooms[m_nRoom].m_fHeightInTiles) * .5f) + m_Rooms[m_nRoom].m_Exits[nExit].Offset)].WorldLocation;

                                NavNode navNodeToCreate = Instantiate(Resources.Load("Prefabs/NavNode", typeof(NavNode)), vNavNodePosition, Quaternion.identity) as NavNode;
                                navNodeToCreate.m_nNodeID = m_nNavNodesCreated;
                                m_nNavNodesCreated++;
                                navNodeToCreate.m_nRoomID = m_nRoom;
                                navNodeToCreate.m_ExitDirection = Direction.Left;
                                m_Rooms[m_nRoom].m_NavNodes.Add(navNodeToCreate);
                                break;
                            }
                        case Direction.Up:
                            {
                                Vector3 vNavNodePosition = m_Rooms[m_nRoom].m_TileReferenceData[((int)((m_Rooms[m_nRoom].m_fWidthInTiles) * .5f) + m_Rooms[m_nRoom].m_Exits[nExit].Offset), ((int)(m_Rooms[m_nRoom].m_fHeightInTiles) - 3)].WorldLocation;

                                NavNode navNodeToCreate = Instantiate(Resources.Load("Prefabs/NavNode", typeof(NavNode)), vNavNodePosition, Quaternion.identity) as NavNode;
                                navNodeToCreate.m_nNodeID = m_nNavNodesCreated;
                                m_nNavNodesCreated++;
                                navNodeToCreate.m_nRoomID = m_nRoom;
                                navNodeToCreate.m_ExitDirection = Direction.Up;
                                m_Rooms[m_nRoom].m_NavNodes.Add(navNodeToCreate);
                                break;
                            }
                        case Direction.Right:
                            {
                                Vector3 vNavNodePosition = m_Rooms[m_nRoom].m_TileReferenceData[((int)(m_Rooms[m_nRoom].m_fWidthInTiles) - 3), ((int)((m_Rooms[m_nRoom].m_fHeightInTiles) * .5f) + m_Rooms[m_nRoom].m_Exits[nExit].Offset)].WorldLocation;

                                NavNode navNodeToCreate = Instantiate(Resources.Load("Prefabs/NavNode", typeof(NavNode)), vNavNodePosition, Quaternion.identity) as NavNode;
                                navNodeToCreate.m_nNodeID = m_nNavNodesCreated;
                                m_nNavNodesCreated++;
                                navNodeToCreate.m_nRoomID = m_nRoom;
                                navNodeToCreate.m_ExitDirection = Direction.Right;
                                m_Rooms[m_nRoom].m_NavNodes.Add(navNodeToCreate);
                                break;
                            }
                    }
                }


                m_nRoom++;
                m_fLoadPercent = Mathf.Lerp(m_fPrepareRoomTilesPercentage, m_fNavCreatePercent, ((float)m_nRoom) / m_Rooms.Count);
                return;
            }
            else
            {
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.NavNodes_Linking;
                Debug.Log("Done with NavNodes_Creation");
                m_fLoadPercent = Mathf.Lerp(m_fPrepareRoomTilesPercentage, m_fNavCreatePercent, ((float)m_nRoom) / m_Rooms.Count);
                m_nRoom = 0;

                return;
            }
        }//END Navigation Nodes
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.NavNodes_Linking)
        {
            if (m_nRoom < m_Rooms.Count)
            {
                //Let's try something a little more direct...
                //we're literally going to raycast our way to each node in the direction of the exit :D
                for (int nNavNode = 0; nNavNode < m_Rooms[m_nRoom].m_NavNodes.Count; nNavNode++)
                {
                    Vector2 vDirection = Vector2.zero;
                    Vector2 vPosition = m_Rooms[m_nRoom].m_NavNodes[nNavNode].transform.position;
                    switch (m_Rooms[m_nRoom].m_NavNodes[nNavNode].m_ExitDirection)
                    {
                        case Direction.Up:
                            {
                                vDirection.y = 1f;
                                vPosition.y += .65f;
                                break;
                            }
                        case Direction.Left:
                            {
                                vDirection.x = -1f;
                                vPosition.x -= .65f;
                                break;
                            }
                        case Direction.Right:
                            {
                                vDirection.x = 1f;
                                vPosition.x += .65f;
                                break;
                            }
                        case Direction.Down:
                            {
                                vDirection.y = -1f;
                                vPosition.y -= .65f;
                                break;
                            }
                    }
                    RaycastHit2D hit = Physics2D.Raycast(vPosition, vDirection, Mathf.Infinity, (1 << LayerMask.NameToLayer("NavNode")));

                    if (hit.collider != null)
                    {
                        NavNode ourNavNode = hit.collider.gameObject.GetComponent<NavNode>();
                        m_Rooms[m_nRoom].m_NavNodes[nNavNode].m_NavNodeConnections.Add(ourNavNode);

                        //we should also track distance here - TRACKED again below for nodes that are linked inside the same room
                        m_Rooms[m_nRoom].m_NavNodes[nNavNode].m_Distances.Add(Vector3.Distance(m_Rooms[m_nRoom].m_NavNodes[nNavNode].transform.position, ourNavNode.transform.position));
                    }

                    //ALSO, we need to link each navnode to the other navnodes in the same room
                    for (int nOtherRoomNodes = 0; nOtherRoomNodes < m_Rooms[m_nRoom].m_NavNodes.Count; nOtherRoomNodes++)
                    {
                        //don't link a node to itself
                        if (nOtherRoomNodes == nNavNode)
                            continue;

                        m_Rooms[m_nRoom].m_NavNodes[nNavNode].m_NavNodeConnections.Add(m_Rooms[m_nRoom].m_NavNodes[nOtherRoomNodes]);

                        //we should also track distance here
                        m_Rooms[m_nRoom].m_NavNodes[nNavNode].m_Distances.Add(Vector3.Distance(m_Rooms[m_nRoom].m_NavNodes[nNavNode].transform.position, m_Rooms[m_nRoom].m_NavNodes[nOtherRoomNodes].transform.position));
                    }
                }
                m_nRoom++;
                m_fLoadPercent = Mathf.Lerp(m_fNavCreatePercent, m_fNavLinkPercent, ((float)m_nRoom) / m_Rooms.Count);
                return;
            }
            else
            {
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.NavNodes_Cleaning;
                Debug.Log("Done with NavNodes_Linking");
                m_fLoadPercent = Mathf.Lerp(m_fNavCreatePercent, m_fNavLinkPercent, ((float)m_nRoom) / m_Rooms.Count);
                m_nRoom = 0;
                return;
            }
        }//END Navigation Nodes Linking
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.NavNodes_Cleaning)
        {
            if (m_nRoom < m_Rooms.Count)
            {
                //Let's try something a little more direct...
                //we're literally going to raycast our way to each node in the direction of the exit :D
                for (int nNavNode = 0; nNavNode < m_Rooms[m_nRoom].m_NavNodes.Count; nNavNode++)
                {
                    Destroy(m_Rooms[m_nRoom].m_NavNodes[nNavNode].GetComponent<BoxCollider2D>());
                }
                m_nRoom++;
                m_fLoadPercent = Mathf.Lerp(m_fNavLinkPercent, m_fNavCleanPercent, ((float)m_nRoom) / m_Rooms.Count);
                return;
            }
            else
            {
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.SceneryPlacementZoning;
                Debug.Log("Done with NavNodes_Cleaning");
                m_fLoadPercent = Mathf.Lerp(m_fNavLinkPercent, m_fNavCleanPercent, ((float)m_nRoom) / m_Rooms.Count);
                m_nRoom = 2; //Exclude Starting Room [0] and Hallway [1]
                return;
            }
        }//END Navigation Nodes Cleaning
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.SceneryPlacementZoning)
        {
            if (m_nRoom < m_Rooms.Count)
            {
                //For each exit inside of a room, turn off scenery placement on the floor tiles in the exit space of that room
                for (int nExit = 0; nExit < m_Rooms[m_nRoom].m_Exits.Count; nExit++)
                {
                    switch (m_Rooms[m_nRoom].m_Exits[nExit].Direction)
                    {
                        case Direction.Down:
                            {
                                //Vector3 vNavNodePosition = m_Rooms[m_nRoom].m_TileReferenceData[((int)((m_Rooms[m_nRoom].m_fWidthInTiles) * .5f) + m_Rooms[m_nRoom].m_Exits[nExit].Offset), 2].WorldLocation;

                                int nExitCenter = ((int)((m_Rooms[m_nRoom].m_fWidthInTiles) * .5f) + m_Rooms[m_nRoom].m_Exits[nExit].Offset); //center tile of a room exit

                                for (int nExitTileY = 0; nExitTileY < 3; nExitTileY++)
                                {
                                    m_Rooms[m_nRoom].m_TileReferenceData[nExitCenter - 1, nExitTileY].ContainsSceneryObject = true;
                                    m_Rooms[m_nRoom].m_TileReferenceData[nExitCenter, nExitTileY].ContainsSceneryObject = true;
                                    m_Rooms[m_nRoom].m_TileReferenceData[nExitCenter + 1, nExitTileY].ContainsSceneryObject = true;
                                }
                                break;
                            }
                        case Direction.Left:
                            {
                                //Vector3 vNavNodePosition = m_Rooms[m_nRoom].m_TileReferenceData[2, ((int)((m_Rooms[m_nRoom].m_fHeightInTiles) * .5f) + m_Rooms[m_nRoom].m_Exits[nExit].Offset)].WorldLocation;

                                int nExitCenter = ((int)((m_Rooms[m_nRoom].m_fHeightInTiles) * .5f) + m_Rooms[m_nRoom].m_Exits[nExit].Offset);

                                for (int nExitTileX = 0; nExitTileX < 3; nExitTileX++)
                                {
                                    m_Rooms[m_nRoom].m_TileReferenceData[nExitTileX, nExitCenter - 1].ContainsSceneryObject = true;
                                    m_Rooms[m_nRoom].m_TileReferenceData[nExitTileX, nExitCenter].ContainsSceneryObject = true;
                                    m_Rooms[m_nRoom].m_TileReferenceData[nExitTileX, nExitCenter + 1].ContainsSceneryObject = true;
                                }
                                break;
                            }
                        case Direction.Up:
                            {
                                //Vector3 vNavNodePosition = m_Rooms[m_nRoom].m_TileReferenceData[((int)((m_Rooms[m_nRoom].m_fWidthInTiles) * .5f) + m_Rooms[m_nRoom].m_Exits[nExit].Offset), ((int)(m_Rooms[m_nRoom].m_fHeightInTiles) - 3)].WorldLocation;

                                int nExitCenter = ((int)((m_Rooms[m_nRoom].m_fWidthInTiles) * .5f) + m_Rooms[m_nRoom].m_Exits[nExit].Offset);
                                int nExitHeight = (int)(m_Rooms[m_nRoom].m_fHeightInTiles);

                                for (int nTileSubY = 1; nTileSubY < 4; nTileSubY++)
                                {
                                    m_Rooms[m_nRoom].m_TileReferenceData[nExitCenter - 1, (nExitHeight - nTileSubY)].ContainsSceneryObject = true;
                                    m_Rooms[m_nRoom].m_TileReferenceData[nExitCenter, (nExitHeight - nTileSubY)].ContainsSceneryObject = true;
                                    m_Rooms[m_nRoom].m_TileReferenceData[nExitCenter + 1, (nExitHeight - nTileSubY)].ContainsSceneryObject = true;
                                }
                                break;
                            }
                        case Direction.Right:
                            {
                                //Vector3 vNavNodePosition = m_Rooms[m_nRoom].m_TileReferenceData[((int)(m_Rooms[m_nRoom].m_fWidthInTiles) - 3), ((int)((m_Rooms[m_nRoom].m_fHeightInTiles) * .5f) + m_Rooms[m_nRoom].m_Exits[nExit].Offset)].WorldLocation;

                                int nExitCenter = ((int)((m_Rooms[m_nRoom].m_fHeightInTiles) * .5f) + m_Rooms[m_nRoom].m_Exits[nExit].Offset);
                                int nExitHeight = (int)(m_Rooms[m_nRoom].m_fWidthInTiles);

                                for (int nTileSubX = 1; nTileSubX < 4; nTileSubX++)
                                {
                                    m_Rooms[m_nRoom].m_TileReferenceData[nExitHeight - nTileSubX, nExitCenter - 1].ContainsSceneryObject = true;
                                    m_Rooms[m_nRoom].m_TileReferenceData[nExitHeight - nTileSubX, nExitCenter].ContainsSceneryObject = true;
                                    m_Rooms[m_nRoom].m_TileReferenceData[nExitHeight - nTileSubX, nExitCenter + 1].ContainsSceneryObject = true;
                                }
                                break;
                            }
                    }
                }

                m_nRoom++;
                m_fLoadPercent = Mathf.Lerp(m_fNavCleanPercent, m_fSceneryPlacementZoningPercent, ((float)m_nRoom) / m_Rooms.Count);
                return;
            }
            else
            {
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.PlaceTorches;
                Debug.Log("Done with Scenery Placement Zoning");
                m_fLoadPercent = Mathf.Lerp(m_fNavCleanPercent, m_fSceneryPlacementZoningPercent, ((float)m_nRoom) / m_Rooms.Count);
                m_nRoom = 2; //Exclude Starting Room [0] and Hallway [1]
                return;
            }
        }//END Scenery Placement Zoning
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.PlaceTorches)
        {
            if (m_nRoom < m_Rooms.Count)
            {
                switch (m_Rooms[m_nRoom].m_RoomType)
                {
                    case TMBuildType.Basic_Room:
                        {
                            //For Torches - Spawn along top facing walls for now
                            m_BI_fSpawnChance = Random.Range(0f, 100f);
                            if (m_BI_fSpawnChance < m_BI_fTorchSpawnPercent)
                            {
                                //if there's room to place a torch, place it
                                //save each index
                                for (int nTileX = 0; nTileX < m_Rooms[m_nRoom].m_fWidthInTiles; nTileX++)
                                    for (int nTileY = 0; nTileY < m_Rooms[m_nRoom].m_fHeightInTiles; nTileY++)
                                    {
                                        //type check for Wall Top
                                        if (m_Rooms[m_nRoom].m_TileReferenceData[nTileX, nTileY].TileType == TileType.Wall_Top_1 && m_Rooms[m_nRoom].m_TileReferenceData[nTileX, nTileY].ContainsSceneryObject == false)
                                        {
                                            //save the X,Y index for candidates
                                            m_BI_vValidTileIndices.Add(new Index2D(nTileX, nTileY));
                                        }

                                        //For rotating the torch...i need to edit the particle velocity using a script... and rotate the torch as well.


                                    }

                                m_BI_nObjectCount = Random.Range(1, 3);
                                while (m_BI_nObjectCount > 0 && m_BI_vValidTileIndices.Count > 0)
                                {

                                    //now that we have a list of valid indices...if there's 1 or more, pick one to place a torch in                              
                                    m_BI_nIndexSelection = Random.Range(0, m_BI_vValidTileIndices.Count - 1);
                                    Torch_Base TorchToCreate = Instantiate(Resources.Load("Prefabs/Torch_Base", typeof(Torch_Base)), m_Rooms[m_nRoom].m_TileReferenceData[m_BI_vValidTileIndices[m_BI_nIndexSelection].X, m_BI_vValidTileIndices[m_BI_nIndexSelection].Y].WorldLocation, Quaternion.identity) as Torch_Base;

                                    //Need A Collection for Scenery?  Room Should own stationary scenery really
                                    m_Rooms[m_nRoom].m_Scenery.Add(TorchToCreate.gameObject);
                                    //Note that the referenced tile has a scenery object on it? probably a good idea
                                    m_Rooms[m_nRoom].m_TileReferenceData[m_BI_vValidTileIndices[m_BI_nIndexSelection].X, m_BI_vValidTileIndices[m_BI_nIndexSelection].Y].ContainsSceneryObject = true;

                                    m_BI_vValidTileIndices.Remove(m_BI_vValidTileIndices[m_BI_nIndexSelection]);
                                    m_BI_nObjectCount--;
                                }
                                m_BI_vValidTileIndices.Clear();
                            }
                            break;
                        }
                }
                m_nRoom++;
                m_fLoadPercent = Mathf.Lerp(m_fSceneryPlacementZoningPercent, m_fTorchesPlacedPercentage, ((float)m_nRoom) / m_Rooms.Count);
                return;
            }
            else //All Torches Placed - Now Treasure Chests
            {
                m_fLoadPercent = Mathf.Lerp(m_fSceneryPlacementZoningPercent, m_fTorchesPlacedPercentage, ((float)m_nRoom) / m_Rooms.Count);
                m_nRoom = 2;
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.PlaceTreasureChests;
                Debug.Log("Done With Torches");
                return;
            }
        } //END Torches
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.PlaceTreasureChests)
        {
            if (!m_bCreateInteriorObjects)
                m_nRoom = m_Rooms.Count;

            if (m_nRoom < m_Rooms.Count)
            {
                switch (m_Rooms[m_nRoom].m_RoomType)
                {
                    case TMBuildType.Basic_Room:
                        {
                            //For Treasure Chest - Basic Room Floor
                            m_BI_fSpawnChance = Random.Range(0f, 100f);
                            if (m_BI_fSpawnChance < m_BI_fTreasureChestSpawnPercent)
                            {
                                //if there's room to place a torch, place it
                                //save each index
                                for (int nTileX = 0; nTileX < m_Rooms[m_nRoom].m_fWidthInTiles; nTileX++)
                                    for (int nTileY = 0; nTileY < m_Rooms[m_nRoom].m_fHeightInTiles; nTileY++)
                                    {
                                        //type check for Floor
                                        if (m_Rooms[m_nRoom].m_TileReferenceData[nTileX, nTileY].TileType == TileType.Floor && m_Rooms[m_nRoom].m_TileReferenceData[nTileX, nTileY].ContainsSceneryObject == false)
                                        {
                                            //save the X,Y index for candidates
                                            m_BI_vValidTileIndices.Add(new Index2D(nTileX, nTileY));
                                        }

                                    }

                                m_BI_nObjectCount = 1;
                                while (m_BI_nObjectCount > 0 && m_BI_vValidTileIndices.Count > 0)
                                {

                                    //now that we have a list of valid indices...if there's 1 or more, pick one to place a torch in                              
                                    m_BI_nIndexSelection = Random.Range(0, m_BI_vValidTileIndices.Count - 1);
                                    Treasure_Chest TreasureChestToCreate = Instantiate(Resources.Load("Prefabs/Treasure_Chest", typeof(Treasure_Chest)), m_Rooms[m_nRoom].m_TileReferenceData[m_BI_vValidTileIndices[m_BI_nIndexSelection].X, m_BI_vValidTileIndices[m_BI_nIndexSelection].Y].WorldLocation, Quaternion.identity) as Treasure_Chest;

                                    //Need A Collection for Scenery?  Room Should own stationary scenery really
                                    m_Rooms[m_nRoom].m_Scenery.Add(TreasureChestToCreate.gameObject);
                                    m_Interactables.Add(TreasureChestToCreate.GetComponent<Interactable>());
                                    //Note that the referenced tile has a scenery object on it? probably a good idea
                                    m_Rooms[m_nRoom].m_TileReferenceData[m_BI_vValidTileIndices[m_BI_nIndexSelection].X, m_BI_vValidTileIndices[m_BI_nIndexSelection].Y].ContainsSceneryObject = true;

                                    m_BI_vValidTileIndices.Remove(m_BI_vValidTileIndices[m_BI_nIndexSelection]);
                                    m_BI_nObjectCount--;
                                }
                                m_BI_vValidTileIndices.Clear();
                            }
                            break;
                        }
                }
                m_nRoom++;
                m_fLoadPercent = Mathf.Lerp(m_fTorchesPlacedPercentage, m_fTreasureChestPlacedPercentage, ((float)m_nRoom) / m_Rooms.Count);

                return;
            }
            else //All Treasure Chests Placed - Now Food Jars
            {
                m_fLoadPercent = Mathf.Lerp(m_fTorchesPlacedPercentage, m_fTreasureChestPlacedPercentage, ((float)m_nRoom) / m_Rooms.Count);
                m_nRoom = 2;
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.PlaceJars;
                Debug.Log("Done With Treasure");
                return;
            }
        } //END Treasure Chests
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.PlaceJars)
        {
            if (!m_bCreateInteriorObjects)
                m_nRoom = m_Rooms.Count;

            if (m_nRoom < m_Rooms.Count)
            {
                switch (m_Rooms[m_nRoom].m_RoomType)
                {
                    case TMBuildType.Basic_Room:
                        {
                            //For Treasure Chest - Basic Room Floor
                            m_BI_fSpawnChance = Random.Range(0f, 100f);
                            if (m_BI_fSpawnChance < m_BI_fFoodJarSpawnPercent)
                            {
                                //if there's room to place a torch, place it
                                //save each index
                                for (int nTileX = 0; nTileX < m_Rooms[m_nRoom].m_fWidthInTiles; nTileX++)
                                    for (int nTileY = 0; nTileY < m_Rooms[m_nRoom].m_fHeightInTiles; nTileY++)
                                    {
                                        //type check for Floor
                                        if (m_Rooms[m_nRoom].m_TileReferenceData[nTileX, nTileY].TileType == TileType.Floor && m_Rooms[m_nRoom].m_TileReferenceData[nTileX, nTileY].ContainsSceneryObject == false)
                                        {
                                            //save the X,Y index for candidates
                                            m_BI_vValidTileIndices.Add(new Index2D(nTileX, nTileY));
                                        }

                                    }

                                m_BI_nObjectCount = Random.Range(1, 2);
                                while (m_BI_nObjectCount > 0 && m_BI_vValidTileIndices.Count > 0)
                                {

                                    //now that we have a list of valid indices...if there's 1 or more, pick one to place a torch in                              
                                    m_BI_nIndexSelection = Random.Range(0, m_BI_vValidTileIndices.Count - 1);
                                    Food_Jar FoodJarToCreate = Instantiate(Resources.Load("Prefabs/Food_Jar", typeof(Food_Jar)), m_Rooms[m_nRoom].m_TileReferenceData[m_BI_vValidTileIndices[m_BI_nIndexSelection].X, m_BI_vValidTileIndices[m_BI_nIndexSelection].Y].WorldLocation, Quaternion.identity) as Food_Jar;

                                    //Need A Collection for Scenery?  Room Should own stationary scenery really
                                    m_Rooms[m_nRoom].m_Scenery.Add(FoodJarToCreate.gameObject);
                                    m_Interactables.Add(FoodJarToCreate.GetComponent<Interactable>());
                                    //Note that the referenced tile has a scenery object on it? probably a good idea
                                    m_Rooms[m_nRoom].m_TileReferenceData[m_BI_vValidTileIndices[m_BI_nIndexSelection].X, m_BI_vValidTileIndices[m_BI_nIndexSelection].Y].ContainsSceneryObject = true;

                                    m_BI_vValidTileIndices.Remove(m_BI_vValidTileIndices[m_BI_nIndexSelection]);
                                    m_BI_nObjectCount--;
                                }
                                m_BI_vValidTileIndices.Clear();
                            }
                            break;
                        }
                }
                m_nRoom++;
                m_fLoadPercent = Mathf.Lerp(m_fTreasureChestPlacedPercentage, m_fJarsPlacedPercentage, ((float)m_nRoom) / m_Rooms.Count);
                return;
            }
            else //All Jars Placed - Now Wooden Crates
            {
                m_fLoadPercent = Mathf.Lerp(m_fTreasureChestPlacedPercentage, m_fJarsPlacedPercentage, ((float)m_nRoom) / m_Rooms.Count);
                m_nRoom = 2;
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.PlaceWoodenCrates;
                Debug.Log("Done With Food Jars");
                return;
            }
        } //END Food Jars
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.PlaceWoodenCrates)
        {
            if (!m_bCreateInteriorObjects)
                m_nRoom = m_Rooms.Count;

            if (m_nRoom < m_Rooms.Count)
            {
                switch (m_Rooms[m_nRoom].m_RoomType)
                {
                    case TMBuildType.Basic_Room:
                        {
                            //For Wooden Crates - Basic Room Floor
                            m_BI_fSpawnChance = Random.Range(0f, 100f);
                            if (m_BI_fSpawnChance < m_BI_fWoodenCrateSpawnPercent)
                            {
                                //if there's room to place a torch, place it
                                //save each index
                                for (int nTileX = 0; nTileX < m_Rooms[m_nRoom].m_fWidthInTiles; nTileX++)
                                    for (int nTileY = 0; nTileY < m_Rooms[m_nRoom].m_fHeightInTiles; nTileY++)
                                    {
                                        //type check for Floor
                                        if (m_Rooms[m_nRoom].m_TileReferenceData[nTileX, nTileY].TileType == TileType.Floor && m_Rooms[m_nRoom].m_TileReferenceData[nTileX, nTileY].ContainsSceneryObject == false)
                                        {
                                            //save the X,Y index for candidates
                                            m_BI_vValidTileIndices.Add(new Index2D(nTileX, nTileY));
                                        }

                                    }

                                m_BI_nObjectCount = Random.Range(1, 2);
                                while (m_BI_nObjectCount > 0 && m_BI_vValidTileIndices.Count > 0)
                                {

                                    //now that we have a list of valid indices...if there's 1 or more, pick one to place a torch in                              
                                    m_BI_nIndexSelection = Random.Range(0, m_BI_vValidTileIndices.Count - 1);
                                    Wooden_Crate WoodenCrateToCreate = Instantiate(Resources.Load("Prefabs/Wooden_Crate", typeof(Wooden_Crate)), m_Rooms[m_nRoom].m_TileReferenceData[m_BI_vValidTileIndices[m_BI_nIndexSelection].X, m_BI_vValidTileIndices[m_BI_nIndexSelection].Y].WorldLocation, Quaternion.identity) as Wooden_Crate;

                                    //Need A Collection for Scenery?  Room Should own stationary scenery really
                                    m_Rooms[m_nRoom].m_Scenery.Add(WoodenCrateToCreate.gameObject);
                                    m_Interactables.Add(WoodenCrateToCreate.GetComponent<Interactable>());
                                    //Note that the referenced tile has a scenery object on it? probably a good idea
                                    m_Rooms[m_nRoom].m_TileReferenceData[m_BI_vValidTileIndices[m_BI_nIndexSelection].X, m_BI_vValidTileIndices[m_BI_nIndexSelection].Y].ContainsSceneryObject = true;

                                    m_BI_vValidTileIndices.Remove(m_BI_vValidTileIndices[m_BI_nIndexSelection]);
                                    m_BI_nObjectCount--;
                                }
                                m_BI_vValidTileIndices.Clear();
                            }
                            break;
                        }
                }
                m_nRoom++;
                m_fLoadPercent = Mathf.Lerp(m_fJarsPlacedPercentage, m_fWoodCratesPlacedPercentage, ((float)m_nRoom) / m_Rooms.Count);

                return;
            }
            else //All Wooden Crates Placed - Now Spawner:Shades
            {
                m_fLoadPercent = Mathf.Lerp(m_fJarsPlacedPercentage, m_fWoodCratesPlacedPercentage, ((float)m_nRoom) / m_Rooms.Count);
                m_nRoom = 2;
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.PlaceShadeSpawners;
                Debug.Log("Done With Wood Crates");
                return;
            }
        } //END Wooden Crates
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.PlaceShadeSpawners)
        {
            if (m_nRoom < m_Rooms.Count)
            {
                switch (m_Rooms[m_nRoom].m_RoomType)
                {
                    case TMBuildType.Basic_Room:
                        {
                            if (m_BI_nCurrentMonsterSpawners < m_BI_nMaxMonsterSpawners)
                            {
                                //For Spawner:Shade - Basic Room Floor
                                m_BI_fSpawnChance = Random.Range(0f, 100f);
                                if (m_BI_fSpawnChance < m_BI_fShadeSpawnerPercentage)
                                {
                                    //save each index
                                    for (int nTileX = 0; nTileX < m_Rooms[m_nRoom].m_fWidthInTiles; nTileX++)
                                        for (int nTileY = 0; nTileY < m_Rooms[m_nRoom].m_fHeightInTiles; nTileY++)
                                        {
                                            //type check for Floor
                                            if (m_Rooms[m_nRoom].m_TileReferenceData[nTileX, nTileY].TileType == TileType.Floor && m_Rooms[m_nRoom].m_TileReferenceData[nTileX, nTileY].ContainsSceneryObject == false)
                                            {
                                                //save the X,Y index for candidates
                                                m_BI_vValidTileIndices.Add(new Index2D(nTileX, nTileY));
                                            }

                                        }

                                    m_BI_nObjectCount = 1;
                                    while (m_BI_nObjectCount > 0 && m_BI_vValidTileIndices.Count > 0)
                                    {

                                        //now that we have a list of valid indices...if there's 1 or more, pick one to place a torch in                              
                                        m_BI_nIndexSelection = Random.Range(0, m_BI_vValidTileIndices.Count - 1);
                                        Spawner_Shade spawnerToCreate = Instantiate(Resources.Load("Prefabs/Spawner_Shade", typeof(Spawner_Shade)), m_Rooms[m_nRoom].m_TileReferenceData[m_BI_vValidTileIndices[m_BI_nIndexSelection].X, m_BI_vValidTileIndices[m_BI_nIndexSelection].Y].WorldLocation, Quaternion.identity) as Spawner_Shade;
                                        spawnerToCreate.Initialize();
                                        spawnerToCreate.m_nCurrentRoomID = m_nRoom;
                                        //Give each shade a reference to the generator that it belongs to
                                        for (int nNPC = 0; nNPC < spawnerToCreate.shadeArray.Length; nNPC++)
                                            spawnerToCreate.shadeArray[nNPC].m_IndoorDungeonGenerator = this;
                                        m_ShadeSpawners.Add(spawnerToCreate);
                                        spawnerToCreate.gameObject.SetActive(false);
                                        //spawner, not a scenery object


                                        //Note that the referenced tile has a scenery object on it? probably a good idea
                                        m_Rooms[m_nRoom].m_TileReferenceData[m_BI_vValidTileIndices[m_BI_nIndexSelection].X, m_BI_vValidTileIndices[m_BI_nIndexSelection].Y].ContainsSceneryObject = true;

                                        m_BI_vValidTileIndices.Remove(m_BI_vValidTileIndices[m_BI_nIndexSelection]);
                                        m_BI_nObjectCount--;

                                    }
                                    m_BI_vValidTileIndices.Clear();
                                    m_BI_nCurrentMonsterSpawners++;

                                }
                            }
                            break;
                        }
                }
                m_nRoom++;
                m_fLoadPercent = Mathf.Lerp(m_fWoodCratesPlacedPercentage, m_fShadeSpawnerPlacedPercentage, ((float)m_nRoom) / m_Rooms.Count);

                return;
            }
            else //All Spawner:Shade Placed - Now Place Triggers
            {
                m_fLoadPercent = Mathf.Lerp(m_fWoodCratesPlacedPercentage, m_fShadeSpawnerPlacedPercentage, ((float)m_nRoom) / m_Rooms.Count);
                m_nRoom = 2;
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.PlaceBossRooms;
                Debug.Log("Done With Spawner:Shades");
                return;
            }
        } //END Spawner:Shades
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.PlaceBossRooms)
        {
            //we want to place between 1 and 3 boss rooms
            //should have different sized areas
            //small areas should have 1-2 bosses
            //medium and large areas should have 2-3 bosses
            //preferrably these need to be located on the outskirts of the current area.
            //Also avoid placing them where we came from, so if we came from the southwest, we should use north and eastern regions
            //consider 2 functions
            //one that creates a special room
            //second one that creates a long hallway between two rooms

            //FindAreaBoundaries();
            //pick a spot that is well outside of range...not even going to test collision
            //find which boss we're going to place.  This should give us the size of the room we're going to use
            //int nAreaBosses = Random.Range(1, 3);
            //int nAreaBosses = 2;

            // int nBosses = 4;
            //List<Direction> unusedBossLocations = new List<Direction>();
            //unusedBossLocations.Add(Direction.UpLeft);
            ////unusedBossLocations.Add (Direction.Up);
            //unusedBossLocations.Add(Direction.UpRight);
            ////unusedBossLocations.Add (Direction.Right);
            //unusedBossLocations.Add(Direction.DownRight);
            ////unusedBossLocations.Add (Direction.Down);
            //unusedBossLocations.Add(Direction.DownLeft);
            ////unusedBossLocations.Add (Direction.Left);

            Debug.Log("Create " + m_nAreaBosses + " Boss Rooms");
            FindAreaBoundaries(m_nAreaBosses);
            //for (int nBoss = 0; nBoss < nAreaBosses; nBoss++)
            //{
            //    Direction currentBossLocation = unusedBossLocations[Random.Range(0, unusedBossLocations.Count - 1)];
            //    unusedBossLocations.Remove(currentBossLocation);

            //    FindFurthestRooms(currentBossLocation);
            //}
            //create special room should really do asynchronous loadlevel additive.  The scene it loads should just be the boss room with everything in it
            //The "boss room" should include everything, the boss door, the hallway leading up to the boss, a second door into the boss chamber, all scenery, traps,events, everything, all in that one scene
            //CreateSpecialRoom(SpecialRoomType.Boss, BossID.CorpseFondler)
            //Edit one of the rooms to open up a path to this special room...see if we can use what we already have.
            m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.FindBranchEnds;
            Debug.Log("Done With Boss Rooms");
            m_nRoom = 2;
            return;
        }
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.FindBranchEnds)
        {
            //First find all of our dungeon path branches
            if (m_nRoom < m_Rooms.Count)
            {
                if (m_Rooms[m_nRoom].m_Exits.Count <= 1)
                {
                    //we have a dead end branch.  make sure it's not an area exit
                    if (RoomIsNotAreaExit(m_nRoom))
                    {
                        //room is not an area exit and is a branch endpoint.  Add to a list of dungeon endpoints
                        //Debug.Log("Branch End Room Found");
                        m_DungeonBranchEndRooms.Add(m_Rooms[m_nRoom]);
                    }
                }
                m_nRoom++;
                return;
            }
            else
            {
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.Circularity;
                Debug.Log("Dungeon Branch Endpoints Designated");
                m_nRoom = 0;
                return;
            }
        }
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.Circularity)
        {
            //so in the name of speed, exert the least amount of effort to open some alternate pathways to our area exit points.  These areas unfortunately do not have navnodes on them.
            //identify all rooms that have an area exit in them. DONE during boss room placement
            //For each room containing an area exit, look for the two nearest "DungeonBranchEndRooms" on both sides and create a hallway connection to those rooms however we can
            if (m_nCurrentAreaExit < m_nAreaBosses)
            {
                

                //attempt to link inwards, towards the starting room.
                //step by step
                //find the 2 walls that have no exit on them.
                List<Direction> possibleExitDirections = new List<Direction>();
                possibleExitDirections.Add(Direction.Up);
                possibleExitDirections.Add(Direction.Left);
                possibleExitDirections.Add(Direction.Right);
                possibleExitDirections.Add(Direction.Down);
                for (int nExit = 0; nExit < m_MapAreaExits[m_nCurrentAreaExit].m_Exits.Count; nExit++)
                {
                    possibleExitDirections.Remove(m_MapAreaExits[m_nCurrentAreaExit].m_Exits[nExit].Direction);
                }
                //now we should only be left with exits that have not been used :D
                //try to create circularity with both exits
               
                for(int nCircularPath = 0; nCircularPath < possibleExitDirections.Count; nCircularPath++)
                {
                    //GREAT IDEA: Any room other than the area exits and the starting room that has only 1 exit is the end of a dungeon branch pathway.  If we store all of these, we can simply find the closest endpoint branch!!!
                    //THIS WILL ELIMINATE THE NEED FOR RAY CASTING
                 
                    //TODO:FIND the closest "DungeonBranchEndRoom" to this AreaExitRoom in the current direction
                    //FindClosestBranchInDirection(possibleExitDirections[nCircularPath]);
                }

                m_nCurrentAreaExit++;
                return;
            }
            else
            {
                m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.PlaceTriggers;
                Debug.Log("Done With Circularity");
                return;
            }
        }
        else if (m_eCurrentBuildSection == IndoorDungeonGeneratorBuildProgress.PlaceTriggers)
        {
            //PLACE Area Trigger (In the starting Room) - Displays a GUI Text Object that shows the name of the area and plays an intro SFX
            PlaceAreaTrigger();
            Debug.Log("Done With Triggers");
            Debug.Log("Building Set to False");
            m_eCurrentBuildSection = IndoorDungeonGeneratorBuildProgress.DONE;
            m_nRoom = 0;
            m_bBuilding = false;
        }
        

        m_fLoadPercent = 100f;

        Debug.Log("Total Rooms: " + m_Rooms.Count);

        //Activate the Player
        m_Player.gameObject.SetActive(true);

        //Turn Lights off (Black)
        RenderSettings.ambientLight = Color.black;

        //Give the player reference to all interactables
        for (int nInteractable = 0; nInteractable < m_Interactables.Count; nInteractable++)
            m_Interactables[nInteractable].m_Player = m_Player;

        //set basic physics 2d material for every wall in every room
        for(int nRoom = 0; nRoom < m_Rooms.Count; nRoom++)
        {
            Collider2D[] boxColliders = m_Rooms[nRoom].GetComponents<Collider2D>();
            for(int nCollider = 0; nCollider < boxColliders.Length; nCollider++)
            {
                boxColliders[nCollider].sharedMaterial = m_Physics2DMaterial;
            }
        }

        //Activate Enemies
        for (int nShadeSpawner = 0; nShadeSpawner < m_ShadeSpawners.Count; nShadeSpawner++)
        {
            m_ShadeSpawners[nShadeSpawner].gameObject.SetActive(true);

            for (int nShade = 0; nShade < m_ShadeSpawners[nShadeSpawner].shadeArray.Length; nShade++)
            {
                m_ShadeSpawners[nShadeSpawner].shadeArray[nShade].m_Target = GameObject.FindGameObjectWithTag("Player1").transform;
                m_ShadeSpawners[nShadeSpawner].shadeArray[nShade].m_TargetGoal = GameObject.FindGameObjectWithTag("Player1").transform;
            }
        }
    }

    /// <summary>
    /// Used for Circularity.  We want to find the DungeonBranchEndRoom that is closest in the direction we are given to the current room (MapAreaExit)
    /// </summary>
    /// <param name="directionToSearch"></param>
    void FindClosestBranchInDirection(Direction directionToSearch)
    {
        //TODO:Finish This, comments explain the basic setup, keep it simple
        switch(directionToSearch)
        {
            case Direction.Up:
                {
                    //branch with a larger Y value and the closest difference in Y
                    break;
                }
            case Direction.Right:
                {
                    //branch with a larger X value and the closest difference in X
                    break;
                }
            case Direction.Left:
                {
                    //branch with a smaller X value, and the closest difference in X
                    break;
                }
            case Direction.Down:
                {
                    //branch with a smaller Y value and the closest difference in Y
                    break;
                }
        }
    }

    /// <summary>
    /// True if the room is not an area exit
    /// False if room is an area exit
    /// </summary>
    /// <param name="nRoomID"></param>
    /// <returns></returns>
    bool RoomIsNotAreaExit(int nRoomID)
    {
        for (int nAreaExit = 0; nAreaExit < m_MapAreaExits.Count; nAreaExit++)
        {
            if (m_Rooms[nRoomID] == m_MapAreaExits[nAreaExit])
                return false;
        }
        return true;
    }

    /// <summary>
    /// Find the largest +x, -x, +y and -y values based on room center + width/height depending on axis
    /// Let's make it the tile center points for each room, the floor tile that can also be used to open that room up to connect the boss room to it
    /// Essentially these tiles will be considered our "Area Exits"
    /// An "Area Exit" should be a room ID and a tile coordinate in that room, and the direction that the room would create an opening in
    /// Caveat, a room must not already have an exit on that side of the room
    /// </summary>
    void FindAreaBoundaries(int nAreaExits)
    {
        AreaExit largeX = new AreaExit();
        AreaExit largeY = new AreaExit();
        AreaExit smallX = new AreaExit();
        AreaExit smallY = new AreaExit();
        largeX.AreaOwned = AreaExitRegion.LargestX;
        largeY.AreaOwned = AreaExitRegion.LargestY;
        smallX.AreaOwned = AreaExitRegion.SmallestX;
        smallY.AreaOwned = AreaExitRegion.SmallestY;
        int nCenterTileX;
        int nCenterTileY;
        int nEndTileX;
        int nEndTileY;
        //x = 2 and nEndTileX - 2
        //y = 2 and nEndTileY - 2
        //int nExitTileIndexLeft = 2;
        int nExitTileIndexRight;
        int nExitTileIndexUp;
        //int nExitTileIndexDown = 2;
        
        //need to know the room's center tile
        nCenterTileX = (int)((m_Rooms[m_nRoom].m_fWidthInTiles - 1) * .5f);
        nCenterTileY = (int)((m_Rooms[m_nRoom].m_fHeightInTiles - 1) * .5f);
        nEndTileX = (int)(m_Rooms[m_nRoom].m_fWidthInTiles - 1);
        nEndTileY = (int)(m_Rooms[m_nRoom].m_fHeightInTiles - 1);

        nExitTileIndexRight = nEndTileX - 2;
        nExitTileIndexUp = nEndTileY - 2;

        //largest +x
        largeX.RoomID = m_Rooms[m_nRoom].m_RoomID;
        largeX.TileCoordX = nExitTileIndexRight;
        largeX.TileCoordY = nCenterTileY;
        largeX.DirectionToOpenRoom = Direction.Right;
        largeX.WorldLocation = m_Rooms[m_nRoom].m_TileReferenceData[nExitTileIndexRight, nCenterTileY].WorldLocation;

        //largest +y
        largeY.RoomID = m_Rooms[m_nRoom].m_RoomID;
        largeY.TileCoordX = nCenterTileX;
        largeY.TileCoordY = nExitTileIndexUp;
        largeY.DirectionToOpenRoom = Direction.Up;
        largeY.WorldLocation = m_Rooms[m_nRoom].m_TileReferenceData[nCenterTileX, nExitTileIndexUp].WorldLocation;

        //smallest -x
        smallX.RoomID = m_Rooms[m_nRoom].m_RoomID;
        smallX.TileCoordX = 2;
        smallX.TileCoordY = nCenterTileY;
        smallX.DirectionToOpenRoom = Direction.Left;
        smallX.WorldLocation = m_Rooms[m_nRoom].m_TileReferenceData[2, nCenterTileY].WorldLocation;

        //smallest -y
        smallY.RoomID = m_Rooms[m_nRoom].m_RoomID;
        smallY.TileCoordX = nCenterTileX;
        smallY.TileCoordY = 2;
        smallY.DirectionToOpenRoom = Direction.Down;
        smallY.WorldLocation = m_Rooms[m_nRoom].m_TileReferenceData[nCenterTileX, 2].WorldLocation;

        Vector3 vWorldPosition = Vector3.zero;

        for (int nCurrentRoom = m_nRoom; nCurrentRoom < m_Rooms.Count; nCurrentRoom++)
        {
            //skip if not a basic room
            if (m_Rooms[nCurrentRoom].m_RoomType != TMBuildType.Basic_Room)
                continue;

            //need to know the room's center tile
            nCenterTileX = (int)((m_Rooms[nCurrentRoom].m_fWidthInTiles - 1) * .5f);
            nCenterTileY = (int)((m_Rooms[nCurrentRoom].m_fHeightInTiles - 1) * .5f);
            nEndTileX = (int)(m_Rooms[nCurrentRoom].m_fWidthInTiles - 1);
            nEndTileY = (int)(m_Rooms[nCurrentRoom].m_fHeightInTiles - 1);      

            nExitTileIndexRight = nEndTileX - 2;
            nExitTileIndexUp = nEndTileY - 2;

            //test largest +x
            vWorldPosition = m_Rooms[nCurrentRoom].m_TileReferenceData[nExitTileIndexRight, nCenterTileY].WorldLocation;
            if(vWorldPosition.x > largeX.WorldLocation.x)
            {
                //search all exits to make sure there's no Direction.Up exit
                //for(int nExits = 0; nExits < m_Rooms[nCurrentRoom].m_Exits.Count; nExits++)
                //{
                //    //m_Rooms[nCurrentRoom].m_Exits[nExit]
                //}
                //set largeX to this room's +X (right-most) tile
                largeX.RoomID = nCurrentRoom;
                largeX.TileCoordX = nExitTileIndexRight;
                largeX.TileCoordY = nCenterTileY;
                largeX.WorldLocation = vWorldPosition;
            }

            //test largest +y
            vWorldPosition = m_Rooms[nCurrentRoom].m_TileReferenceData[nCenterTileX, nExitTileIndexUp].WorldLocation;
            if (vWorldPosition.y > largeY.WorldLocation.y)
            {
                //set largeX to this room's +X (right-most) tile
                largeY.RoomID = nCurrentRoom;
                largeY.TileCoordX = nCenterTileX;
                largeY.TileCoordY = nExitTileIndexUp;
                largeY.WorldLocation = vWorldPosition;
            }
        
            //test smallest -x
            vWorldPosition = m_Rooms[nCurrentRoom].m_TileReferenceData[2, nCenterTileY].WorldLocation;
            if (vWorldPosition.x < smallX.WorldLocation.x)
            {
                //set largeX to this room's +X (right-most) tile
                smallX.RoomID = nCurrentRoom;
                smallX.TileCoordX = 2;
                smallX.TileCoordY = nCenterTileY;
                smallX.WorldLocation = vWorldPosition;
            }

            //test smallest -y
            vWorldPosition = m_Rooms[nCurrentRoom].m_TileReferenceData[nCenterTileX, 2].WorldLocation;
            if (vWorldPosition.y < smallY.WorldLocation.y)
            {
                //set largeX to this room's +X (right-most) tile
                smallY.RoomID = nCurrentRoom;
                smallY.TileCoordX = nCenterTileX;
                smallY.TileCoordY = 2;
                smallY.WorldLocation = vWorldPosition;
            }

        }
        
       
        m_AreaExits.Add(largeX);
        m_AreaExits.Add(largeY);
        m_AreaExits.Add(smallX);
        m_AreaExits.Add(smallY);
        
        
        

        List<Direction> unusedBossLocations = new List<Direction>();
        //unusedBossLocations.Add(Direction.UpLeft);
        unusedBossLocations.Add (Direction.Up);
        //unusedBossLocations.Add(Direction.UpRight);
        unusedBossLocations.Add (Direction.Right);
        //unusedBossLocations.Add(Direction.DownRight);
        unusedBossLocations.Add (Direction.Down);
        //unusedBossLocations.Add(Direction.DownLeft);
        unusedBossLocations.Add (Direction.Left);
        Direction eAreaExitDirection;
        for (int nBoss = 0; nBoss < nAreaExits; nBoss++)
        {
            eAreaExitDirection = unusedBossLocations[Random.Range(0, unusedBossLocations.Count - 1)];
            unusedBossLocations.Remove(eAreaExitDirection);

            switch (eAreaExitDirection)
            {
                case Direction.Up:
                    {
                        Debug.Log("Upwards Area Exit Found");
                        //place Token on the room so we can see the exit we need to place
                        AreaExitToken tokenToPlace = Instantiate(Resources.Load("Prefabs/AreaExitToken", typeof(AreaExitToken)), m_AreaExits[1].WorldLocation, Quaternion.identity) as AreaExitToken;
                        tokenToPlace.transform.parent = m_Rooms[m_AreaExits[1].RoomID].transform;
                        m_MapAreaExits.Add(m_Rooms[m_AreaExits[1].RoomID]);
                        break;
                    }
                case Direction.Right:
                    {
                        Debug.Log("Rightmost Area Exit Found");
                        //place Token on the room so we can see the exit we need to place
                        AreaExitToken tokenToPlace = Instantiate(Resources.Load("Prefabs/AreaExitToken", typeof(AreaExitToken)), m_AreaExits[0].WorldLocation, Quaternion.identity) as AreaExitToken;
                        tokenToPlace.transform.parent = m_Rooms[m_AreaExits[0].RoomID].transform;
                        m_MapAreaExits.Add(m_Rooms[m_AreaExits[0].RoomID]);
                        break;
                    }
                case Direction.Down:
                    {
                        Debug.Log("Downwards Area Exit Found");
                        //place Token on the room so we can see the exit we need to place
                        AreaExitToken tokenToPlace = Instantiate(Resources.Load("Prefabs/AreaExitToken", typeof(AreaExitToken)), m_AreaExits[3].WorldLocation, Quaternion.identity) as AreaExitToken;
                        tokenToPlace.transform.parent = m_Rooms[m_AreaExits[3].RoomID].transform;
                        m_MapAreaExits.Add(m_Rooms[m_AreaExits[3].RoomID]);
                        break;
                    }
                case Direction.Left:
                    {
                        Debug.Log("Leftmost Area Exit Found");
                        //place Token on the room so we can see the exit we need to place
                        AreaExitToken tokenToPlace = Instantiate(Resources.Load("Prefabs/AreaExitToken", typeof(AreaExitToken)), m_AreaExits[2].WorldLocation, Quaternion.identity) as AreaExitToken;
                        tokenToPlace.transform.parent = m_Rooms[m_AreaExits[2].RoomID].transform;
                        m_MapAreaExits.Add(m_Rooms[m_AreaExits[2].RoomID]);
                        break;
                    }
            }
        }
    }

    void InsertRandomExits(int nNumExitsToAdd, List<Direction> exitList)
    {
        possibleExits.Add(Direction.Left);
        possibleExits.Add(Direction.Up);
        possibleExits.Add(Direction.Right);
        possibleExits.Add(Direction.Down);

        for (int nExit = 0; nExit < nNumExitsToAdd; nExit++)
        {
            int numExits = possibleExits.Count;
            int nIndex = Random.Range(0, numExits);
            exitList.Add(possibleExits[nIndex]);
            possibleExits.RemoveAt(nIndex);
        }

        possibleExits.Clear();
    }

    void PlaceAreaTrigger()
    {
        //the first starting room is easy, m_Rooms[0]...we'll have to figure out which room is the starting room after each boss..
        //m_Rooms[0].m_Exit[0].ExitCenter;

        //:D we can use m_UsedExits[0].WorldPosition + new Vector3(0f, \
        Vector3 vPosition = Vector3.zero;
        switch (m_UsedExits[0].ExitDirection)
        {
            case Direction.Down:
                {
                    vPosition = m_UsedExits[0].WorldPosition + new Vector3(0f, m_fTileSize * 2f, 0f);
                    break;
                }
            case Direction.Left:
                {
                    vPosition = m_UsedExits[0].WorldPosition + new Vector3(m_fTileSize * 2f, 0f, 0f);
                    break;
                }
            case Direction.Right:
                {
                    vPosition = m_UsedExits[0].WorldPosition + new Vector3(-m_fTileSize * 2f, 0f, 0f);
                    break;
                }
            case Direction.Up:
                {
                    vPosition = m_UsedExits[0].WorldPosition + new Vector3(0f, -m_fTileSize * 2f, 0f);
                    break;
                }
        }
        Debug.Log("Start Room Exit: " + vPosition);
        AreaEntryTextTrigger textTrigger = Instantiate(Resources.Load("Prefabs/AreaEntryTextTrigger", typeof(AreaEntryTextTrigger)), vPosition, Quaternion.identity) as AreaEntryTextTrigger;
        textTrigger.Initialization("The Pit");
        textTrigger.Resize(m_UsedExits[0].ExitDirection, 3);
    }

    void BuildRoomInteriors()
    {
        //We need to place some content in each room to give it some life...
        //for now we'll leave hallways/junctions empty
        //eventually we'll add in a small amount of content there also
        //int nMaxSceneryObjects = 0;
        //int nMaxMonsterSpawners = 5;
        //int nCurrentMonsterSpawners = 0;
        //float fSpawnChance = 0f;
        //float fTorchSpawnPercent = 45.0f;
        //float fTreasureChestSpawnPercent = 33.0f;
        //float fShadeSpawnerPercentage = 30.0f;
        //float fFoodJarSpawnPercent = 40.0f;
        //float fWoodenCrateSpawnPercent = 55.0f;
        //int nObjectCount = 0;
        ////int nMaxAttempts = 3;
        ////int nAttempt = 0;
        //int nIndexSelection = 0;

        List<Index2D> vValidTileIndices = new List<Index2D>();
        for (int nRoom = 2; nRoom < m_Rooms.Count; nRoom++)
        {
            switch (m_Rooms[nRoom].m_RoomType)
            {
                case TMBuildType.Basic_Room:
                    {
                        //Need some variety here...brainstorm objects to add to a database.

                        //nMaxSceneryObjects++; //delete this

                        //objects to consider
                        //Rubble
                        //Table
                        //Chair
                        //Potted Plant
                        //Statue
                        //Painting
                        //Rusted Weapon Rack
                        //Barrels
                        //Wooden Crates
                        //Bench
                        //Various Rugs
                        //Candle
                        //Torch - Do this early
                        //Brazzier
                        //Various Monster Spawner

                        //nMaxSceneryObjects = 5;
                        //nAttempt = 0;
                        ////unused atm
                        //while (nMaxSceneryObjects > 0 && nAttempt < nMaxAttempts)
                        //{
                        //    nAttempt++;

                        //    //Select Object to spawn
                        //    nMaxSceneryObjects--;
                        //}

                        //For Torches - Spawn along top facing walls for now
                        m_BI_fSpawnChance = Random.Range(0f, 100f);
                        if (m_BI_fSpawnChance < m_BI_fTorchSpawnPercent)
                        {
                            //if there's room to place a torch, place it
                            //save each index
                            for (int nTileX = 0; nTileX < m_Rooms[nRoom].m_fWidthInTiles; nTileX++)
                                for (int nTileY = 0; nTileY < m_Rooms[nRoom].m_fHeightInTiles; nTileY++)
                                {
                                    //type check for Wall Top
                                    if (m_Rooms[nRoom].m_TileReferenceData[nTileX, nTileY].TileType == TileType.Wall_Top_1 && m_Rooms[nRoom].m_TileReferenceData[nTileX, nTileY].ContainsSceneryObject == false)
                                    {
                                        //save the X,Y index for candidates
                                        vValidTileIndices.Add(new Index2D(nTileX, nTileY));
                                    }

                                    //For rotating the torch...i need to edit the particle velocity using a script... and rotate the torch as well.


                                }

                            m_BI_nObjectCount = Random.Range(1, 3);
                            while (m_BI_nObjectCount > 0 && vValidTileIndices.Count > 0)
                            {

                                //now that we have a list of valid indices...if there's 1 or more, pick one to place a torch in                              
                                m_BI_nIndexSelection = Random.Range(0, vValidTileIndices.Count - 1);
                                Torch_Base TorchToCreate = Instantiate(Resources.Load("Prefabs/Torch_Base", typeof(Torch_Base)), m_Rooms[nRoom].m_TileReferenceData[vValidTileIndices[m_BI_nIndexSelection].X, vValidTileIndices[m_BI_nIndexSelection].Y].WorldLocation, Quaternion.identity) as Torch_Base;

                                //Need A Collection for Scenery?  Room Should own stationary scenery really
                                m_Rooms[nRoom].m_Scenery.Add(TorchToCreate.gameObject);

                                //Note that the referenced tile has a scenery object on it? probably a good idea
                                m_Rooms[nRoom].m_TileReferenceData[vValidTileIndices[m_BI_nIndexSelection].X, vValidTileIndices[m_BI_nIndexSelection].Y].ContainsSceneryObject = true;

                                vValidTileIndices.Remove(vValidTileIndices[m_BI_nIndexSelection]);
                                m_BI_nObjectCount--;
                            }
                            vValidTileIndices.Clear();
                        }

                        //For Treasure Chest - Only spawn in basic rooms (no hallways / junctions)
                        m_BI_fSpawnChance = Random.Range(0f, 100f);
                        if (m_Rooms[nRoom].m_RoomType == TMBuildType.Basic_Room && m_BI_fSpawnChance < m_BI_fTreasureChestSpawnPercent)
                        {
                            for (int nTileX = 0; nTileX < m_Rooms[nRoom].m_fWidthInTiles; nTileX++)
                                for (int nTileY = 0; nTileY < m_Rooms[nRoom].m_fHeightInTiles; nTileY++)
                                {
                                    //type check for Floor...may want to add other floor types
                                    if (m_Rooms[nRoom].m_TileReferenceData[nTileX, nTileY].TileType == TileType.Floor && m_Rooms[nRoom].m_TileReferenceData[nTileX, nTileY].ContainsSceneryObject == false)
                                    {
                                        //save the X,Y index for candidates
                                        vValidTileIndices.Add(new Index2D(nTileX, nTileY));
                                    }
                                }

                            //spawn it on one of the tiles
                            m_BI_nObjectCount = 1;
                            while (m_BI_nObjectCount > 0 && vValidTileIndices.Count > 0)
                            {
                                m_BI_nIndexSelection = Random.Range(0, vValidTileIndices.Count - 1);
                                Treasure_Chest TreasureChestToCreate = Instantiate(Resources.Load("Prefabs/Treasure_Chest", typeof(Treasure_Chest)), m_Rooms[nRoom].m_TileReferenceData[vValidTileIndices[m_BI_nIndexSelection].X, vValidTileIndices[m_BI_nIndexSelection].Y].WorldLocation, Quaternion.identity) as Treasure_Chest;

                                //Need A Collection for Scenery?  Room Should own stationary scenery really
                                m_Rooms[nRoom].m_Scenery.Add(TreasureChestToCreate.gameObject);

                                //Note that the referenced tile has a scenery object on it? probably a good idea
                                m_Rooms[nRoom].m_TileReferenceData[vValidTileIndices[m_BI_nIndexSelection].X, vValidTileIndices[m_BI_nIndexSelection].Y].ContainsSceneryObject = true;
                                vValidTileIndices.Remove(vValidTileIndices[m_BI_nIndexSelection]);
                                m_BI_nObjectCount--;
                            }
                            vValidTileIndices.Clear();
                        }

                        //Jars - Basic Rooms Only
                        m_BI_fSpawnChance = Random.Range(0f, 100f);
                        if (m_Rooms[nRoom].m_RoomType == TMBuildType.Basic_Room && m_BI_fSpawnChance < m_BI_fFoodJarSpawnPercent)
                        {
                            for (int nTileX = 0; nTileX < m_Rooms[nRoom].m_fWidthInTiles; nTileX++)
                                for (int nTileY = 0; nTileY < m_Rooms[nRoom].m_fHeightInTiles; nTileY++)
                                {
                                    //type check for Floor...may want to add other floor types
                                    if (m_Rooms[nRoom].m_TileReferenceData[nTileX, nTileY].TileType == TileType.Floor && m_Rooms[nRoom].m_TileReferenceData[nTileX, nTileY].ContainsSceneryObject == false)
                                    {
                                        //save the X,Y index for candidates
                                        vValidTileIndices.Add(new Index2D(nTileX, nTileY));
                                    }
                                }

                            //spawn it on one of the tiles
                            m_BI_nObjectCount = Random.Range(1, 5);
                            while (m_BI_nObjectCount > 0 && vValidTileIndices.Count > 0)
                            {
                                m_BI_nIndexSelection = Random.Range(0, vValidTileIndices.Count - 1);
                                Food_Jar JarToCreate = Instantiate(Resources.Load("Prefabs/Food_Jar", typeof(Food_Jar)), m_Rooms[nRoom].m_TileReferenceData[vValidTileIndices[m_BI_nIndexSelection].X, vValidTileIndices[m_BI_nIndexSelection].Y].WorldLocation, Quaternion.identity) as Food_Jar;

                                //Need A Collection for Scenery?  Room Should own stationary scenery really
                                m_Rooms[nRoom].m_Scenery.Add(JarToCreate.gameObject);

                                //Note that the referenced tile has a scenery object on it? probably a good idea
                                m_Rooms[nRoom].m_TileReferenceData[vValidTileIndices[m_BI_nIndexSelection].X, vValidTileIndices[m_BI_nIndexSelection].Y].ContainsSceneryObject = true;
                                vValidTileIndices.Remove(vValidTileIndices[m_BI_nIndexSelection]);
                                m_BI_nObjectCount--;
                            }
                            vValidTileIndices.Clear();
                        }

                        //Wooden_Crate - Basic Room Only
                        m_BI_fSpawnChance = Random.Range(0f, 100f);
                        if (m_Rooms[nRoom].m_RoomType == TMBuildType.Basic_Room && m_BI_fSpawnChance < m_BI_fWoodenCrateSpawnPercent)
                        {
                            for (int nTileX = 0; nTileX < m_Rooms[nRoom].m_fWidthInTiles; nTileX++)
                                for (int nTileY = 0; nTileY < m_Rooms[nRoom].m_fHeightInTiles; nTileY++)
                                {
                                    //type check for Floor...may want to add other floor types
                                    if (m_Rooms[nRoom].m_TileReferenceData[nTileX, nTileY].TileType == TileType.Floor && m_Rooms[nRoom].m_TileReferenceData[nTileX, nTileY].ContainsSceneryObject == false)
                                    {
                                        //save the X,Y index for candidates
                                        vValidTileIndices.Add(new Index2D(nTileX, nTileY));
                                    }
                                }

                            //spawn it on one of the tiles
                            m_BI_nObjectCount = Random.Range(1, 5);
                            while (m_BI_nObjectCount > 0 && vValidTileIndices.Count > 0)
                            {
                                m_BI_nIndexSelection = Random.Range(0, vValidTileIndices.Count - 1);
                                Wooden_Crate WoodenCrateToCreate = Instantiate(Resources.Load("Prefabs/Wooden_Crate", typeof(Wooden_Crate)), m_Rooms[nRoom].m_TileReferenceData[vValidTileIndices[m_BI_nIndexSelection].X, vValidTileIndices[m_BI_nIndexSelection].Y].WorldLocation, Quaternion.identity) as Wooden_Crate;

                                //Need A Collection for Scenery?  Room Should own stationary scenery really
                                m_Rooms[nRoom].m_Scenery.Add(WoodenCrateToCreate.gameObject);

                                //Note that the referenced tile has a scenery object on it? probably a good idea
                                m_Rooms[nRoom].m_TileReferenceData[vValidTileIndices[m_BI_nIndexSelection].X, vValidTileIndices[m_BI_nIndexSelection].Y].ContainsSceneryObject = true;
                                vValidTileIndices.Remove(vValidTileIndices[m_BI_nIndexSelection]);
                                m_BI_nObjectCount--;
                            }
                            vValidTileIndices.Clear();
                        }

                        //Shade Spawners - Only spawn in Basic Rooms for now
                        if (m_BI_nCurrentMonsterSpawners < m_BI_nMaxMonsterSpawners)
                        {

                            m_BI_fSpawnChance = Random.Range(0f, 100f);
                            if (m_Rooms[nRoom].m_RoomType == TMBuildType.Basic_Room && m_BI_fSpawnChance <= m_BI_fShadeSpawnerPercentage)
                            {


                                for (int nTileX = 0; nTileX < m_Rooms[nRoom].m_fWidthInTiles; nTileX++)
                                    for (int nTileY = 0; nTileY < m_Rooms[nRoom].m_fHeightInTiles; nTileY++)
                                    {
                                        //type check for Floor...may want to add other floor types
                                        if (m_Rooms[nRoom].m_TileReferenceData[nTileX, nTileY].TileType == TileType.Floor && m_Rooms[nRoom].m_TileReferenceData[nTileX, nTileY].ContainsSceneryObject == false)
                                        {
                                            //save the X,Y index for candidates
                                            vValidTileIndices.Add(new Index2D(nTileX, nTileY));
                                        }
                                    }

                                //spawn it on one of the tiles
                                m_BI_nObjectCount = 1;
                                while (m_BI_nObjectCount > 0 && vValidTileIndices.Count > 0)
                                {
                                    m_BI_nIndexSelection = Random.Range(0, vValidTileIndices.Count - 1);
                                    //Treasure_Chest TreasureChestToCreate = Instantiate(Resources.Load("Prefabs/Treasure_Chest", typeof(Treasure_Chest)), m_Rooms[nRoom].m_TileReferenceData[vValidTileIndices[nIndexSelection].X, vValidTileIndices[nIndexSelection].Y].WorldLocation, Quaternion.identity) as Treasure_Chest;
                                    Spawner_Shade spawnerToCreate = Instantiate(Resources.Load("Prefabs/Spawner_Shade", typeof(Spawner_Shade)), m_Rooms[nRoom].m_TileReferenceData[vValidTileIndices[m_BI_nIndexSelection].X, vValidTileIndices[m_BI_nIndexSelection].Y].WorldLocation, Quaternion.identity) as Spawner_Shade;

                                    //spawner, not a scenery object
                                    m_ShadeSpawners.Add(spawnerToCreate);

                                    //Note that the referenced tile has a scenery object on it? probably a good idea
                                    m_Rooms[nRoom].m_TileReferenceData[vValidTileIndices[m_BI_nIndexSelection].X, vValidTileIndices[m_BI_nIndexSelection].Y].ContainsSceneryObject = true;
                                    vValidTileIndices.Remove(vValidTileIndices[m_BI_nIndexSelection]);
                                    m_BI_nObjectCount--;
                                }
                                vValidTileIndices.Clear();

                                m_BI_nCurrentMonsterSpawners++;
                                ////TODO: now we instantiate a Spawner_Shade Object here...or any other monster spawner I create in the future.
                                ////Future (Different) Monster Spawners might require additional adjustments to keep them properly aligned on the floor

                            }
                        }
                        break;
                    }

            }
        }
    }

    void ConvertRoomToBoss(Direction eDirection)
    {
        float fMaxAxis = 0;
        int nFurthestRoom = 0;
        //going to find a room in some furthest direction within this segment of the dungeon
        switch (eDirection)
        {
            case Direction.Up:
                {
                    for (int nRoom = 1; nRoom < m_Rooms.Count; nRoom++)
                    {
                        if (m_Rooms[nRoom].m_RoomType == TMBuildType.Basic_Room)
                        {
                            if (fMaxAxis <= m_Rooms[nRoom].m_vTop.y)
                            {
                                nFurthestRoom = nRoom;
                                fMaxAxis = m_Rooms[nRoom].m_vTop.y;

                            }
                        }
                    }
                    break;
                }
            case Direction.Right:
                {
                    for (int nRoom = 1; nRoom < m_Rooms.Count; nRoom++)
                    {
                        if (m_Rooms[nRoom].m_RoomType == TMBuildType.Basic_Room)
                        {
                            if (fMaxAxis <= m_Rooms[nRoom].m_vRight.x)
                            {
                                nFurthestRoom = nRoom;
                                fMaxAxis = m_Rooms[nRoom].m_vRight.x;

                            }
                        }
                    }
                    break;
                }
            case Direction.Down:
                {
                    for (int nRoom = 1; nRoom < m_Rooms.Count; nRoom++)
                    {
                        if (m_Rooms[nRoom].m_RoomType == TMBuildType.Basic_Room)
                        {
                            if (fMaxAxis >= m_Rooms[nRoom].m_vBottom.y)
                            {
                                nFurthestRoom = nRoom;
                                fMaxAxis = m_Rooms[nRoom].m_vBottom.y;

                            }
                        }
                    }
                    break;
                }
            case Direction.Left:
                {
                    for (int nRoom = 1; nRoom < m_Rooms.Count; nRoom++)
                    {
                        if (m_Rooms[nRoom].m_RoomType == TMBuildType.Basic_Room)
                        {
                            if (fMaxAxis >= m_Rooms[nRoom].m_vLeft.x)
                            {
                                nFurthestRoom = nRoom;
                                fMaxAxis = m_Rooms[nRoom].m_vLeft.x;

                            }
                        }
                    }
                    break;
                }
        }
        //now that we have the boss room..we need to swap the texture and place the contents inside...
        nFurthestRoom += 1;
    }

    void FindFurthestRooms(Direction bossDirection)
    {
        //for each room in our master list of rooms, find the rooms with the greatest distance
        float fMaxX = 0;
        float fMaxY = 0;
        int nFurthestRoom = 0;

        switch (bossDirection)
        {
            case Direction.UpLeft:
                {
                    //-x,+y Max
                    for (int nRoom = 1; nRoom < m_Rooms.Count; nRoom++)
                    {

                        //										if (m_Rooms [nRoom].m_RoomType == TMBuildType.Hallway_Horizontal || m_Rooms [nRoom].m_RoomType == TMBuildType.Junction || m_Rooms [nRoom].m_RoomType == TMBuildType.BOSS)
                        //												continue;
                        if (m_Rooms[nRoom].m_RoomType != TMBuildType.Basic_Room)
                            continue;
                        //										if (fMaxX > m_Rooms [nRoom].transform.position.x) {
                        //												fMaxX = m_Rooms [nRoom].transform.position.x;
                        //												nFurthestRoom = nRoom;
                        //					
                        //										}
                        //										if (fMaxY < m_Rooms [nRoom].transform.position.y) {
                        //												fMaxY = m_Rooms [nRoom].transform.position.y;
                        //												nFurthestRoom = nRoom;
                        //										}

                        if (fMaxX > m_Rooms[nRoom].transform.position.x && fMaxY < m_Rooms[nRoom].transform.position.y)
                        {
                            fMaxX = m_Rooms[nRoom].transform.position.x;
                            fMaxY = m_Rooms[nRoom].transform.position.y;
                            nFurthestRoom = nRoom;
                        }
                    }
                    Debug.Log("UpLeft Area Exit Found");
                    //place Token on the room so we can see the exit we need to place
                    AreaExitToken tokenToPlace = Instantiate(Resources.Load("Prefabs/AreaExitToken", typeof(AreaExitToken)), m_Rooms[nFurthestRoom].transform.position, Quaternion.identity) as AreaExitToken;
                    tokenToPlace.transform.parent = m_Rooms[nFurthestRoom].transform;
                    break;
                }
            case Direction.Up:
                {
                    break;
                }
            case Direction.UpRight:
                {
                    //+x,+y Max
                    for (int nRoom = 1; nRoom < m_Rooms.Count; nRoom++)
                    {

                        //										if (m_Rooms [nRoom].m_RoomType == TMBuildType.Hallway_Horizontal || m_Rooms [nRoom].m_RoomType == TMBuildType.Junction || m_Rooms [nRoom].m_RoomType == TMBuildType.BOSS)
                        //												continue;
                        if (m_Rooms[nRoom].m_RoomType != TMBuildType.Basic_Room)
                            continue;
                        //										if (fMaxX < m_Rooms [nRoom].transform.position.x) {
                        //												fMaxX = m_Rooms [nRoom].transform.position.x;
                        //												nFurthestRoom = nRoom;
                        //
                        //										}
                        //										if (fMaxY < m_Rooms [nRoom].transform.position.y) {
                        //												fMaxY = m_Rooms [nRoom].transform.position.y;
                        //												nFurthestRoom = nRoom;
                        //										}

                        if (fMaxX < m_Rooms[nRoom].transform.position.x && fMaxY < m_Rooms[nRoom].transform.position.y)
                        {
                            fMaxX = m_Rooms[nRoom].transform.position.x;
                            fMaxY = m_Rooms[nRoom].transform.position.y;
                            nFurthestRoom = nRoom;
                        }
                    }
                    Debug.Log("UpRight Area Exit Found");
                    //place Token on the room so we can see the exit we need to place
                    AreaExitToken tokenToPlace = Instantiate(Resources.Load("Prefabs/AreaExitToken", typeof(AreaExitToken)), m_Rooms[nFurthestRoom].transform.position, Quaternion.identity) as AreaExitToken;
                    tokenToPlace.transform.parent = m_Rooms[nFurthestRoom].transform;
                    break;
                }
            case Direction.Right:
                {
                    break;
                }
            case Direction.DownRight:
                {
                    //+x,-y Max
                    for (int nRoom = 1; nRoom < m_Rooms.Count; nRoom++)
                    {

                        //										if (m_Rooms [nRoom].m_RoomType == TMBuildType.Hallway_Horizontal || m_Rooms [nRoom].m_RoomType == TMBuildType.Junction || m_Rooms [nRoom].m_RoomType == TMBuildType.BOSS)
                        //												continue;
                        if (m_Rooms[nRoom].m_RoomType != TMBuildType.Basic_Room)
                            continue;
                        //										if (fMaxX < m_Rooms [nRoom].transform.position.x) {
                        //												fMaxX = m_Rooms [nRoom].transform.position.x;
                        //												nFurthestRoom = nRoom;
                        //					
                        //										}
                        //										if (fMaxY > m_Rooms [nRoom].transform.position.y) {
                        //												fMaxY = m_Rooms [nRoom].transform.position.y;
                        //												nFurthestRoom = nRoom;
                        //										}

                        if (fMaxX < m_Rooms[nRoom].transform.position.x && fMaxY > m_Rooms[nRoom].transform.position.y)
                        {
                            fMaxX = m_Rooms[nRoom].transform.position.x;
                            fMaxY = m_Rooms[nRoom].transform.position.y;
                            nFurthestRoom = nRoom;
                        }
                    }
                    Debug.Log("DownRight Area Exit Found");
                    //place Token on the room so we can see the exit we need to place
                    AreaExitToken tokenToPlace = Instantiate(Resources.Load("Prefabs/AreaExitToken", typeof(AreaExitToken)), m_Rooms[nFurthestRoom].transform.position, Quaternion.identity) as AreaExitToken;
                    tokenToPlace.transform.parent = m_Rooms[nFurthestRoom].transform;
                    break;
                }
            case Direction.Down:
                {
                    break;
                }
            case Direction.DownLeft:
                {

                    //-x,-y Max
                    for (int nRoom = 1; nRoom < m_Rooms.Count; nRoom++)
                    {

                        //										if (m_Rooms [nRoom].m_RoomType == TMBuildType.Hallway_Horizontal || m_Rooms [nRoom].m_RoomType == TMBuildType.Junction || m_Rooms [nRoom].m_RoomType == TMBuildType.BOSS)
                        //												continue;
                        if (m_Rooms[nRoom].m_RoomType != TMBuildType.Basic_Room)
                            continue;
                        //										if (fMaxX > m_Rooms [nRoom].transform.position.x) {
                        //												fMaxX = m_Rooms [nRoom].transform.position.x;
                        //												nFurthestRoom = nRoom;
                        //					
                        //										}
                        //										if (fMaxY > m_Rooms [nRoom].transform.position.y) {
                        //												fMaxY = m_Rooms [nRoom].transform.position.y;
                        //												nFurthestRoom = nRoom;
                        //										}

                        if (fMaxX > m_Rooms[nRoom].transform.position.x && fMaxY > m_Rooms[nRoom].transform.position.y)
                        {
                            fMaxX = m_Rooms[nRoom].transform.position.x;
                            fMaxY = m_Rooms[nRoom].transform.position.y;
                            nFurthestRoom = nRoom;
                        }
                    }
                    Debug.Log("DownLeft Area Exit Found");
                    //place Token on the room so we can see the exit we need to place
                    AreaExitToken tokenToPlace = Instantiate(Resources.Load("Prefabs/AreaExitToken", typeof(AreaExitToken)), m_Rooms[nFurthestRoom].transform.position, Quaternion.identity) as AreaExitToken;
                    tokenToPlace.transform.parent = m_Rooms[nFurthestRoom].transform;
                    break;
                }
            case Direction.Left:
                {
                    break;
                }
        }

        //we now have the index of the furthest room, and a direction.  Place the boss room beyond that direction
        //PlaceBossRoom(bossDirection, m_Rooms[nFurthestRoom]);

        //						float fDistanceCompare = m_Rooms [nRoom].transform.position.sqrMagnitude;
        //
        //				
        //						if (fDistanceCompare > maxDistance) {
        //								maxDistance = m_Rooms [nRoom].transform.position.sqrMagnitude;
        //								nFurthestRoom = nRoom;
        //						}
        //				}

    }

    //void PlaceBossRoom(Direction bossLocation, Tile_Map areaToConnectToBoss)
    //{

    //    //TODO: I MESSED UP ROYALLY USE THE FUNCTIONS THAT I ALREADY HAVE TO JUST MAKE A HALLWAY AND A ROOM........
    //    //Step 1:
    //    //Use TestNextHallway
    //    RoomExit areaExit = new RoomExit();
    //    //RoomExit exitToAdd = new RoomExit ();
    //    areaExit.RoomID = areaToConnectToBoss.m_RoomID;
    //    areaExit.BuildType = areaToConnectToBoss.m_BuildType;
    //    switch (bossLocation)
    //    {
    //        case Direction.UpLeft:
    //            {
    //                areaExit.ExitDirection = Direction.Up;
    //                break;
    //            }
    //        case Direction.UpRight:
    //            {
    //                areaExit.ExitDirection = Direction.Up;
    //                break;
    //            }
    //        case Direction.DownRight:
    //            {
    //                areaExit.ExitDirection = Direction.Down;
    //                break;

    //            }
    //        case Direction.DownLeft:
    //            {
    //                areaExit.ExitDirection = Direction.Down;
    //                break;
    //            }
    //    }




    //    if (areaToConnectToBoss.m_RoomType != TMBuildType.Basic_Room)
    //        areaExit.OffSet = 0;
    //    else
    //    {
    //        int nMaximumOffset = CalculateMaxOffset((int)areaToConnectToBoss.m_fWidthInTiles, (int)areaToConnectToBoss.m_fHeightInTiles, areaExit.ExitDirection);
    //        areaExit.OffSet = Random.Range(-nMaximumOffset, nMaximumOffset);//need to calculate other offsets

    //    }

    //    areaExit.OffSet = 0;
    //    areaExit.WorldPosition = areaToConnectToBoss.GetExitPoint(areaExit.OffSet, areaExit.ExitDirection);
    //    bool bHallwayCreated = TestNextHallway(areaExit.OffSet, areaExit.ExitDirection, areaToConnectToBoss, areaExit, 7, 10);
    //    //Debug.Log ("BossHallway Done near" + areaExit.WorldPosition);

    //    //TODO: FALLBACK PLAN...place like a ladder/staircase/teleporter to boss room....seriously this needs to get done easier
    //    //3/1/2015 I'm working here. Gotta get bosses finished up
    //    //Step 2:
    //    //Use TestNextRoom
    //    if (bHallwayCreated)
    //    {
    //        TestNextRoom(0, m_UnusedExits[m_UnusedExits.Count - 1].ExitDirection, m_Rooms[m_UnusedExits[m_UnusedExits.Count - 1].RoomID], m_UnusedExits[m_UnusedExits.Count - 1], 9, 12, 1);
    //        Debug.Log("Boss Room Placed");
    //    }

    //    //Boss Room created....now we need to put stuff in it




    //    //				//TODO:Create a boss room here after testing to make sure the location is clear, then connect from areaToConnectToBoss one block at a time..straight hallways are okay
    //    //
    //    //				//First we need to select a boss room
    //    //
    //    //				//for now we'll just use set data
    //    //				//place bossroom at the center of the areaToConnectToBoss, then Move it in the direction away from it
    //    //				float fRoomPadding = 21f * m_fTileSize;	//21 tiles away, 3 junction sizes
    //    //				float fRoomWidth = 17f * m_fTileSize;
    //    //				float fRoomHeight = 17f * m_fTileSize;
    //    //				int nRoomTileWidth = 17;
    //    //				int nRoomTileHeight = 17;
    //    //				Vector3 vBossRoomCenter = areaToConnectToBoss.transform.position;
    //    //				//Vector3 vMoveBossRoom
    //    //				
    //    //				Tile_Map roomToAdd = null;
    //    //
    //    //				switch (bossLocation) {
    //    //				case Direction.UpLeft:
    //    //						{
    //    //								vBossRoomCenter.y += areaToConnectToBoss.m_fHeight * .5f + fRoomHeight * .5f + fRoomPadding;
    //    //								if (TestRoomPlacement (nRoomTileWidth, nRoomTileHeight, vBossRoomCenter)) {
    //    //										Debug.Log ("Boss Room [UpLeft] Could not be placed");
    //    //										return;
    //    //								}
    //    //
    //    //								roomToAdd = Instantiate (Resources.Load ("Prefabs/Tile_Map", typeof(Tile_Map)), vBossRoomCenter, Quaternion.identity) as Tile_Map;
    //    //			
    //    //								roomToAdd.Initialization (nRoomTileWidth, nRoomTileHeight);
    //    //								roomToAdd.CreateTileMap (m_TileMapLocations [0]);
    //    //								roomToAdd.SetCorners ();
    //    //								roomToAdd.m_RoomType = TMBuildType.BOSS;
    //    //								m_Rooms.Add (roomToAdd);
    //    //			
    //    //								m_nCurrentRooms++;
    //    //			
    //    //								roomToAdd.m_RoomID = m_Rooms.Count - 1;
    //    //								
    //    //								ConnectBossToArea (bossLocation, roomToAdd, areaToConnectToBoss);
    //    //								break;
    //    //						}
    //    //				case Direction.UpRight:
    //    //						{						
    //    //								vBossRoomCenter.y += areaToConnectToBoss.m_fHeight * .5f + fRoomHeight * .5f + fRoomPadding;
    //    //
    //    //								if (TestRoomPlacement (nRoomTileWidth, nRoomTileHeight, vBossRoomCenter)) {
    //    //										Debug.Log ("Boss Room [UpRight] Could not be placed");
    //    //										return;
    //    //								}
    //    //
    //    //								roomToAdd = Instantiate (Resources.Load ("Prefabs/Tile_Map", typeof(Tile_Map)), vBossRoomCenter, Quaternion.identity) as Tile_Map;
    //    //			
    //    //								roomToAdd.Initialization (nRoomTileWidth, nRoomTileHeight);
    //    //								roomToAdd.CreateTileMap (m_TileMapLocations [0]);
    //    //								roomToAdd.SetCorners ();
    //    //								roomToAdd.m_RoomType = TMBuildType.BOSS;
    //    //								m_Rooms.Add (roomToAdd);
    //    //			
    //    //								m_nCurrentRooms++;
    //    //			
    //    //								roomToAdd.m_RoomID = m_Rooms.Count - 1;
    //    //								break;
    //    //						}
    //    //				case Direction.DownRight:
    //    //						{
    //    //								vBossRoomCenter.y = vBossRoomCenter.y - areaToConnectToBoss.m_fHeight * .5f - fRoomHeight * .5f - fRoomPadding;
    //    //
    //    //								if (TestRoomPlacement (nRoomTileWidth, nRoomTileHeight, vBossRoomCenter)) {
    //    //										Debug.Log ("Boss Room [DownRight] Could not be placed");
    //    //										return;
    //    //								}
    //    //								roomToAdd = Instantiate (Resources.Load ("Prefabs/Tile_Map", typeof(Tile_Map)), vBossRoomCenter, Quaternion.identity) as Tile_Map;
    //    //			
    //    //								roomToAdd.Initialization (nRoomTileWidth, nRoomTileHeight);
    //    //								roomToAdd.CreateTileMap (m_TileMapLocations [0]);
    //    //								roomToAdd.SetCorners ();
    //    //								roomToAdd.m_RoomType = TMBuildType.BOSS;
    //    //								m_Rooms.Add (roomToAdd);
    //    //			
    //    //								m_nCurrentRooms++;
    //    //			
    //    //								roomToAdd.m_RoomID = m_Rooms.Count - 1;
    //    //								break;
    //    //						}
    //    //				case Direction.DownLeft:
    //    //						{
    //    //								vBossRoomCenter.y = vBossRoomCenter.y - areaToConnectToBoss.m_fHeight * .5f - fRoomHeight * .5f - fRoomPadding;
    //    //
    //    //								if (TestRoomPlacement (nRoomTileWidth, nRoomTileHeight, vBossRoomCenter)) {
    //    //										Debug.Log ("Boss Room [DownLeft] Could not be placed");
    //    //										return;
    //    //								}
    //    //								roomToAdd = Instantiate (Resources.Load ("Prefabs/Tile_Map", typeof(Tile_Map)), vBossRoomCenter, Quaternion.identity) as Tile_Map;
    //    //			
    //    //								roomToAdd.Initialization (nRoomTileWidth, nRoomTileHeight);
    //    //								roomToAdd.CreateTileMap (m_TileMapLocations [0]);
    //    //								roomToAdd.SetCorners ();
    //    //								roomToAdd.m_RoomType = TMBuildType.BOSS;
    //    //								m_Rooms.Add (roomToAdd);
    //    //			
    //    //								m_nCurrentRooms++;
    //    //			
    //    //								roomToAdd.m_RoomID = m_Rooms.Count - 1;
    //    //								break;
    //    //						}
    //    //				}
    //    //
    //    //				//TODO:Once we know the correct location to go to, use two rays the size of a hallway to see if we can reach that room without bumping into anything.
    //    //				//if(TestRoomPlacement(
    //}

    //void ConnectBossToArea(Direction bossLocation, Tile_Map bossRoom, Tile_Map areaToConnectToBoss)
    //{
    //    switch (bossLocation)
    //    {
    //        case Direction.UpLeft:
    //            {
    //                //go down
    //                //place a start point at just below the tile map
    //                Vector3 vStartPointCenter = new Vector3(bossRoom.transform.position.x, bossRoom.transform.position.y - bossRoom.m_fHeight * .5f, 0f);
    //                Vector3 vEndpointCenter = new Vector3(areaToConnectToBoss.transform.position.x, areaToConnectToBoss.transform.position.y + areaToConnectToBoss.m_fHeight * .5f, 0f);

    //                //This is our new connector piece's center...the size should be startpointy - endpointy
    //                Vector3 vConnectorCenter = new Vector3(bossRoom.transform.position.x, (vStartPointCenter.y + vEndpointCenter.y) * .5f, 0f);
    //                int nHeight = (int)((vStartPointCenter.y - vEndpointCenter.y) / m_fTileSize);
    //                int nWidth = 7;

    //                //build a vertical hallway using this information
    //                Tile_Map bossRoomConnector = null;

    //                bossRoomConnector = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), vConnectorCenter, Quaternion.identity) as Tile_Map;

    //                bossRoomConnector.Initialization(nWidth, nHeight);
    //                bossRoomConnector.CreateTileMap(m_TileMapLocations[0]);
    //                bossRoomConnector.SetCorners();
    //                bossRoomConnector.m_RoomType = TMBuildType.Hallway_Vertical;
    //                m_Rooms.Add(bossRoomConnector);

    //                m_nCurrentRooms++;

    //                bossRoomConnector.m_RoomID = m_Rooms.Count - 1;

    //                //Open Entrance
    //                bossRoomConnector.OpenHallway(0, Direction.Up);
    //                bossRoomConnector.OpenHallway(0, Direction.Down);

    //                bossRoom.OpenRoom(0, Direction.Down);

    //                switch (areaToConnectToBoss.m_RoomType)
    //                {
    //                    case TMBuildType.Basic_Room:
    //                        {
    //                            areaToConnectToBoss.OpenRoom(0, Direction.Up);
    //                            break;
    //                        }
    //                    case TMBuildType.Hallway_Horizontal:
    //                        {
    //                            areaToConnectToBoss.OpenJunction(0, Direction.Up);
    //                            break;
    //                        }
    //                    case TMBuildType.Hallway_Vertical:
    //                        {
    //                            areaToConnectToBoss.OpenHallway(0, Direction.Up);
    //                            break;
    //                        }
    //                    case TMBuildType.Junction:
    //                        {
    //                            areaToConnectToBoss.OpenJunction(0, Direction.Up);
    //                            break;
    //                        }
    //                }
    //                break;
    //            }
    //        case Direction.UpRight:
    //            {
    //                //go down
    //                break;
    //            }
    //        case Direction.DownLeft:
    //            {
    //                //go up
    //                break;
    //            }
    //        case Direction.DownRight:
    //            {
    //                //go up
    //                break;
    //            }
    //    }
    //}

    public void CheckDeadEndSequence(int nNextRoomInSequence, int nRoomWeCameFrom)
    {
        //it would be helpful to store the room ID that the exit is connected to
        if (m_Rooms[nNextRoomInSequence].m_RoomType == TMBuildType.Junction)
        {
            //we need to add this room to the list of rooms to delete, unless it's a junction with 3 exits

            if (m_Rooms[nNextRoomInSequence].m_Exits.Count >= 3)
            {
                //special case, we need to close one exit and remove it from m_Exits
                for(int nExit = 1; nExit < m_Rooms[nNextRoomInSequence].m_Exits.Count; nExit++)
                {
                    //Check the exits, if an exit matches the ID of the room(DeadEnd) we came from, this is the exit that needs to be closed and removed from the junction's list
                    if(m_Rooms[nNextRoomInSequence].m_Exits[nExit].OtherRoomID == nRoomWeCameFrom)
                    {
                        //this is the exit that needs to be closed
                        ///Debug.Log("Closing Junction Exit");
                        //TODO: Finish the CloseJunctionExit function                       
                        m_Rooms[nNextRoomInSequence].CloseJunctionExit(m_Rooms[nNextRoomInSequence].m_Exits[nExit], nExit);
                        
                    }
                }
            }
            else
            {
                //there are 2 exits and we can delete this junction
                //add this to deadendrooms and continue to the next room
                m_DeadEndRooms.Enqueue(m_Rooms[nNextRoomInSequence]);
                CheckDeadEndSequence(m_Rooms[nNextRoomInSequence].m_Exits[0].OtherRoomID, nNextRoomInSequence);
            }


        }
        else //if it's not a basic room, keep going
        {
            if (m_Rooms[nNextRoomInSequence].m_RoomType != TMBuildType.Basic_Room)
            {
                //add this to deadendrooms and continue to the next room
                m_DeadEndRooms.Enqueue(m_Rooms[nNextRoomInSequence]);
                CheckDeadEndSequence(m_Rooms[nNextRoomInSequence].m_Exits[0].OtherRoomID, nNextRoomInSequence);
            }
            else
            {
                //we found the end of our sequence...we need to do a couple of things here
                //close the exit of this basic room
                for (int nExit = 0; nExit < m_Rooms[nNextRoomInSequence].m_Exits.Count; nExit++)
                {
                    //Check the exits, if an exit matches the ID of the room(DeadEnd) we came from, this is the exit that needs to be closed and removed from the basic room's list
                    if (m_Rooms[nNextRoomInSequence].m_Exits[nExit].OtherRoomID == nRoomWeCameFrom)
                    {
                        m_Rooms[nNextRoomInSequence].CloseRoomExit(m_Rooms[nNextRoomInSequence].m_Exits[nExit]);
                    }
                }
               
                //remove the exit from the room's m_Exits list
            }
        }
        ////also, we need to add every room connected to this one until we reach a basic room. STOP a sequence though if you encounter a 3 way junction, change that junction into a 2 way junction
        ////might need to update m_Rooms[x] exits while doing this so that we know if a 3 way junction became a 2 way junction so we can delete it also (to handle the dreaded T-shaped double dead end scenario)

    }

    /// <summary>
    /// Return a list of TileMaps that compose our dead ends, we are going to remove these from m_Rooms and then destroy them, then close the exits they are associated with.
    /// </summary>
    /// <returns>List of Tile Maps to remove from the area (m_Rooms)</returns>
    public void FindDeadEnds()
    {
        //List<Tile_Map> deadendRooms = new List<Tile_Map>();
        //Find a junction or hallway that only has one entrance
        //for (int nRoom = 0; nRoom < m_LevelGraph.NodeCount; nRoom++)
        for (int nRoom = 0; nRoom < m_Rooms.Count; nRoom++)
        {           
            Tile_Map targetRoom = m_Rooms[nRoom];
            //Tile_Map otherRoom;

            if (targetRoom.m_RoomType == TMBuildType.Hallway_Horizontal || targetRoom.m_RoomType == TMBuildType.Hallway_Vertical || targetRoom.m_RoomType == TMBuildType.Junction)
            {
                if (targetRoom.m_Exits.Count <= 1)
                {
                    //Debug.Log("Dead End Found");
                    m_DeadEndRooms.Enqueue(targetRoom);
                    CheckDeadEndSequence(targetRoom.m_Exits[0].OtherRoomID, nRoom);
                }
            }
        }
    }

    RoomExit FindClosestUnusedExit(GenericGraphNode<Tile_Map> nodeRoomToConnect)
    {
        //				if (m_UnusedExits.Count == 0)
        //						return ;

        //TODO: Problem...how do we filter our rooms we're already connected to...?
        //Maybe create a graph before doing this??? we can reuse our graph class
        //that way we can check the roomToConnect's neighbors in that graph and filter them out

        //Graph Created...
        //If Target room is our neighbor...skip
        //Check roomToConnect's neighbors in the graph
        RoomExit targetRoom = m_UnusedExits[0];

        List<int> neighborIDList = new List<int>();
        for (int nNeighbor = 0; nNeighbor < nodeRoomToConnect.NeighborsList.Count; nNeighbor++)
        {
            neighborIDList.Add(nodeRoomToConnect.NeighborsList[nNeighbor].Node.m_RoomID);
        }


        int nSmallestID = 0;
        float smallestDistance = (targetRoom.WorldPosition - nodeRoomToConnect.Node.transform.position).sqrMagnitude;
        float distanceToTest = 0f;

        bool bMatch = false;

        for (int nExit = 1; nExit < m_UnusedExits.Count; nExit++)
        {
            bMatch = false;
            //only allowed to target basic rooms for now
            if (m_UnusedExits[nExit].BuildType == TMBuildType.Basic_Room)
            {
                //if the exit is a neighbor of ours...don't use it
                for (int nNeighbor = 0; nNeighbor < neighborIDList.Count; nNeighbor++)
                {
                    if (neighborIDList[nNeighbor] == m_UnusedExits[nExit].RoomID)
                    {
                        bMatch = true;
                        nNeighbor = neighborIDList.Count;
                    }
                }

                //if a target room is our neighbor... skip this
                if (bMatch)
                    continue;

                distanceToTest = (m_UnusedExits[nExit].WorldPosition - nodeRoomToConnect.Node.transform.position).sqrMagnitude;

                if (distanceToTest < smallestDistance)
                {
                    smallestDistance = distanceToTest;
                    nSmallestID = nExit;
                }
            }
        }
        targetRoom = m_UnusedExits[nSmallestID];
        return targetRoom;
    }

    RoomExit ChooseUnusedExit()
    {
        if (m_UnusedJunctionExits.Count > 0)
            return m_UnusedJunctionExits[Random.Range(0, m_UnusedJunctionExits.Count - 1)];

        if (m_UnusedHallwayExits.Count > 0)
            return m_UnusedHallwayExits[Random.Range(0, m_UnusedHallwayExits.Count - 1)];
        return m_UnusedExits[Random.Range(0, m_UnusedExits.Count - 1)];
    }

    int CalculateMaxOffset(int nX, int nY, Direction eDirection)
    {
        switch (eDirection)
        {
            case Direction.Right:
                {
                    //nMaxOffset = nY - m_nOffsetConstant;
                    return CalculateMaxOffset(nY);
                }
            case Direction.Left:
                {
                    //nMaxOffset = nY - m_nOffsetConstant;
                    return CalculateMaxOffset(nY);
                }
            case Direction.Up:
                {
                    //	nMaxOffset = nX - m_nOffsetConstant;
                    return CalculateMaxOffset(nX);
                }
            case Direction.Down:
                {
                    return CalculateMaxOffset(nX);
                }
        }
        return 0;
    }

    int CalculateMaxOffset(int nAxis)
    {
        return Mathf.FloorToInt((nAxis - m_nExitSize) * .5f);
    }

    void OpenPreviousRoom(Tile_Map previousRoom, RoomExit connectingExit, Direction eDirection)
    {
        switch (connectingExit.BuildType)
        {
            case TMBuildType.Basic_Room:
                {
                    previousRoom.OpenRoom(connectingExit.OffSet, connectingExit.ExitDirection);
                    break;
                }
            case TMBuildType.Hallway:
                {
                    //already open
                    previousRoom.OpenHallway(0, connectingExit.ExitDirection);
                    break;
                }
            case TMBuildType.Junction:
                {
                    previousRoom.JunctionEdit(0, connectingExit.ExitDirection, previousRoom.m_GenerationEntrance, m_Rooms.Count);//not count - 1 because we're about to add the next room (haven't added it at this step yet though but we know it will be added)
                    break;
                }
        }

        m_UnusedExits.Remove(connectingExit);
        m_UnusedHallwayExits.Remove(connectingExit);
        m_UnusedJunctionExits.Remove(connectingExit);
        m_UsedExits.Add(connectingExit);
    }

    void CreateUnusedExit(List<Direction> exits, int xCoord, int yCoord, Tile_Map roomToAdd, int nMaximumOffset, TMBuildType buildType)
    {

        for (int nUnusedExit = 0; nUnusedExit < exits.Count; nUnusedExit++)
        {
            RoomExit exitToAdd = new RoomExit();
            exitToAdd.RoomID = m_Rooms.Count - 1;
            exitToAdd.BuildType = buildType;
            exitToAdd.ExitDirection = exits[nUnusedExit];



            if (buildType != TMBuildType.Basic_Room)
                exitToAdd.OffSet = 0;
            else
            {
                nMaximumOffset = CalculateMaxOffset(xCoord, yCoord, exitToAdd.ExitDirection);
                exitToAdd.OffSet = Random.Range(-nMaximumOffset, nMaximumOffset);//need to calculate other offsets

            }
            exitToAdd.WorldPosition = roomToAdd.GetExitPoint(exitToAdd.OffSet, exitToAdd.ExitDirection);
            m_UnusedExits.Add(exitToAdd);
        }
    }

    void TestCircularConnection(int nOffset, Direction eDirection, Tile_Map previousRoom, RoomExit connectingExit)
    {
        //Trying something simpler...let's just do single tile size rooms...from the exit...we'll start with 1
        int nTestX = 7;
        int nTestY = 7;
        Tile_Map roomToAdd = null;

        Vector3 vCurrentCenter = Vector3.zero;

        switch (eDirection)
        {
            case Direction.Right:
                {
                    nTestX = 1;
                    nTestY = 7;
                    vCurrentCenter = new Vector3(m_fTileSize * .5f, 0f, 0f);
                    vCurrentCenter += connectingExit.WorldPosition;

                    roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), vCurrentCenter, Quaternion.identity) as Tile_Map;

                    roomToAdd.Initialization(nTestX, nTestY, TMBuildType.Hallway_Horizontal);
                    //roomToAdd.CreateTileMap (m_TileMapLocations [0]);
                    roomToAdd.CreateTileMap(m_TerrainTextures[0]);
                    roomToAdd.SetCorners();
                    m_Rooms.Add(roomToAdd);

                    m_nCurrentRooms++;

                    roomToAdd.m_RoomID = m_Rooms.Count - 1;

                    //Open Entrance);
                    roomToAdd.m_GenerationEntrance = Direction.Left;
                    roomToAdd.OpenHallway(0, Direction.Left);
                    Debug.Log("Circular Connection: 1 Horizontal Slice");
                    break;
                }
            case Direction.Left:
                {
                    break;
                }
            case Direction.Up:
                {
                    break;
                }
            case Direction.Down:
                {
                    break;
                }
        }



        //				//ok so we may need to do some special testing here since we need to build from one point to another point
        //			
        //				//unsure how to do this so let's just write out the steps
        //
        //				//1. Find a straight line from our current roomCenter to the worldposition of the connectingExitPoint
        //				Vector3 vLineToTarget = connectingExit.WorldPosition - previousRoom.transform.position;
        //
        //				//2. Get the X and Y of that line.  Create 2 vectors, X and Y.
        //				Vector3 vX = Vector3.zero;
        //				vX.x = vLineToTarget.x;
        //				vX.x = previousRoom.transform.position.y;
        //
        //
        //				Vector3 vY = Vector3.zero;
        //				vY.y = vLineToTarget.y;
        //			
        //				//3. We build based on the X and Y info
        //
        //				//3a -> first try to place a junction on the x.  If this fails, try on the Y
        //				//need new functions...we'll build a list of rooms to place, if everything is TRUE, we build.  If anything fails, we scrap the list.  However we'll try both x and y junctions.
        //				//TestJunction(0, 
        //				//first the junction
        ////				if (!GhostJunction (vX)) {
        ////					
        ////				}
        //				
        //
        //				//4. possibilities, a junction and 2 hallways. OR, just one hallway, OR just opening the previousRoom(Hallway/Junction) and the room of the connectingExit
        //
        //				//5. To Open the target connectingRoom, the exit's room ID should point to its location in m_Rooms


        //		//Update Graph
        //		m_LevelGraph.AddNode(roomToAdd);
        //		m_LevelGraph.AddUndirectedEdge(m_LevelGraph.NodeList[connectingExit.RoomID], m_LevelGraph.NodeList[m_Rooms.Count-1], (int)((m_LevelGraph.NodeList[connectingExit.RoomID].Node.transform.position - m_LevelGraph.NodeList[m_Rooms.Count-1].Node.transform.position).sqrMagnitude)*100);
    }

    bool GhostJunction(Vector3 vRoomCenterPoint)
    {

        if (TestRoomPlacement(7, 7, vRoomCenterPoint))
        {
            m_CircularRoomBuffer.Clear();
            return false;
        }
        //the room can be placed add it to our special container for circular connnectivity preparation.
        Rect circlePathRect = new Rect();
        circlePathRect.width = 7 * m_fTileSize;
        circlePathRect.height = 7 * m_fTileSize;
        circlePathRect.center = vRoomCenterPoint;
        m_CircularRoomBuffer.Add(circlePathRect);

        return true;

    }

    bool TestJunction(int nOffset, Direction eDirection, Tile_Map previousRoom, RoomExit connectingExit)
    {
        //Debug.Log("Junction");
        //Debug.Log ("Room#(Junction): " + m_nCurrentRooms);
        //find what our room size would be first so we know how much size to test against
        int nTestX = 7;
        int nTestY = 7;

        Tile_Map roomToAdd = null;
        //Add Exits
        List<Direction> currentExits = new List<Direction>();

        switch (eDirection)
        {
            case Direction.Right:
                {
                    //need the center to be in line with the center of the opening
                    Vector3 vCurrentCenter = new Vector3(((nTestX - 1) * m_fTileSize * .5f), 0f, 0f);
                    vCurrentCenter += connectingExit.WorldPosition;

                    //at this point we have the size and position of the room we're trying to add, need to test all rooms in this area and not do this if we're colliding
                    if (TestRoomPlacement(nTestX, nTestY, vCurrentCenter, connectingExit.RoomID))
                    {
                        if (connectingExit.BuildType == TMBuildType.Hallway)
                            m_UnusedHallwayExits.Remove(connectingExit);
                        else if (connectingExit.BuildType == TMBuildType.Junction)
                            m_UnusedJunctionExits.Remove(connectingExit);
                        return false;
                    }

                    //Safe to Open Previous Room now
                    OpenPreviousRoom(previousRoom, connectingExit, eDirection);

                    roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), vCurrentCenter, Quaternion.identity) as Tile_Map;

                    roomToAdd.Initialization(nTestX, nTestY, TMBuildType.Junction);
                    //roomToAdd.CreateTileMap (m_TileMapLocations [0]);
                    roomToAdd.CreateTileMap(m_TerrainTextures[0]);
                    roomToAdd.SetCorners();
                    roomToAdd.m_RoomType = TMBuildType.Junction;
                    m_Rooms.Add(roomToAdd);

                    m_nCurrentRooms++;

                    roomToAdd.m_RoomID = m_Rooms.Count - 1;

                    //Open Entrance);
                    roomToAdd.m_GenerationEntrance = Direction.Left;
                    roomToAdd.OpenHallway(0, Direction.Left);

                    //previous room exit needs to know room ID of this room, this room's exit needs to know room ID of previous room
                    ExchangeRoomIDsForExits(previousRoom, roomToAdd);

                    //Exits that are available
                    //currentExits.Add (Direction.Right);
                    currentExits.Add(Direction.Up);
                    currentExits.Add(Direction.Down);
                    break;
                }
            case Direction.Left:
                {
                    //need the center to be in line with the center of the opening
                    Vector3 vCurrentCenter = new Vector3(((nTestX - 1) * -m_fTileSize * .5f), 0f, 0f);
                    vCurrentCenter += connectingExit.WorldPosition;

                    //at this point we have the size and position of the room we're trying to add, need to test all rooms in this area and not do this if we're colliding
                    if (TestRoomPlacement(nTestX, nTestY, vCurrentCenter, connectingExit.RoomID))
                    {
                        if (connectingExit.BuildType == TMBuildType.Hallway)
                            m_UnusedHallwayExits.Remove(connectingExit);
                        else if (connectingExit.BuildType == TMBuildType.Junction)
                            m_UnusedJunctionExits.Remove(connectingExit);
                        return false;
                    }
                    //Safe to Open Previous Room now
                    OpenPreviousRoom(previousRoom, connectingExit, eDirection);

                    roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), vCurrentCenter, Quaternion.identity) as Tile_Map;

                    roomToAdd.Initialization(nTestX, nTestY, TMBuildType.Junction);
                    //roomToAdd.CreateTileMap (m_TileMapLocations [0]);
                    roomToAdd.CreateTileMap(m_TerrainTextures[0]);
                    roomToAdd.SetCorners();
                    roomToAdd.m_RoomType = TMBuildType.Junction;
                    m_Rooms.Add(roomToAdd);

                    m_nCurrentRooms++;

                    roomToAdd.m_RoomID = m_Rooms.Count - 1;

                    //Open Entrance
                    roomToAdd.m_GenerationEntrance = Direction.Right;
                    roomToAdd.OpenHallway(0, Direction.Right);

                    //previous room exit needs to know room ID of this room, this room's exit needs to know room ID of previous room
                    ExchangeRoomIDsForExits(previousRoom, roomToAdd);

                    //Exits that are available
                    //currentExits.Add (Direction.Left);
                    currentExits.Add(Direction.Up);
                    currentExits.Add(Direction.Down);
                    break;
                }

            case Direction.Up:
                {
                    //need the center to be in line with the center of the opening
                    Vector3 vCurrentCenter = new Vector3(0f, ((nTestY - 1) * m_fTileSize * .5f), 0f);
                    vCurrentCenter += connectingExit.WorldPosition;


                    //at this point we have the size and position of the room we're trying to add, need to test all rooms in this area and not do this if we're colliding
                    if (TestRoomPlacement(nTestX, nTestY, vCurrentCenter, connectingExit.RoomID))
                    {
                        if (connectingExit.BuildType == TMBuildType.Hallway)
                            m_UnusedHallwayExits.Remove(connectingExit);
                        else if (connectingExit.BuildType == TMBuildType.Junction)
                            m_UnusedJunctionExits.Remove(connectingExit);
                        return false;
                    }
                    //Safe to Open Previous Room now
                    OpenPreviousRoom(previousRoom, connectingExit, eDirection);

                    roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), vCurrentCenter, Quaternion.identity) as Tile_Map;

                    roomToAdd.Initialization(nTestX, nTestY, TMBuildType.Junction);
                    //roomToAdd.CreateTileMap (m_TileMapLocations [0]);
                    roomToAdd.CreateTileMap(m_TerrainTextures[0]);
                    roomToAdd.SetCorners();
                    roomToAdd.m_RoomType = TMBuildType.Junction;
                    m_Rooms.Add(roomToAdd);

                    m_nCurrentRooms++;

                    //Find our entrance offset

                    roomToAdd.m_RoomID = m_Rooms.Count - 1;

                    //Open Entrance
                    roomToAdd.m_GenerationEntrance = Direction.Down;
                    roomToAdd.OpenHallway(0, Direction.Down);

                    //previous room exit needs to know room ID of this room, this room's exit needs to know room ID of previous room
                    ExchangeRoomIDsForExits(previousRoom, roomToAdd);

                    //Exits that are available
                    currentExits.Add(Direction.Left);
                    //currentExits.Add (Direction.Up);
                    currentExits.Add(Direction.Right);
                    break;
                }

            case Direction.Down:
                {
                    //need the center to be in line with the center of the opening
                    Vector3 vCurrentCenter = new Vector3(0f, ((nTestY - 1) * -m_fTileSize * .5f), 0f);
                    vCurrentCenter += connectingExit.WorldPosition;

                    //at this point we have the size and position of the room we're trying to add, need to test all rooms in this area and not do this if we're colliding
                    if (TestRoomPlacement(nTestX, nTestY, vCurrentCenter, connectingExit.RoomID))
                    {
                        if (connectingExit.BuildType == TMBuildType.Hallway)
                            m_UnusedHallwayExits.Remove(connectingExit);
                        else if (connectingExit.BuildType == TMBuildType.Junction)
                            m_UnusedJunctionExits.Remove(connectingExit);
                        return false;
                    }
                    //Safe to Open Previous Room now
                    OpenPreviousRoom(previousRoom, connectingExit, eDirection);

                    roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), vCurrentCenter, Quaternion.identity) as Tile_Map;

                    roomToAdd.Initialization(nTestX, nTestY);
                    //roomToAdd.CreateTileMap (m_TileMapLocations [0]);
                    roomToAdd.CreateTileMap(m_TerrainTextures[0]);
                    roomToAdd.SetCorners();
                    roomToAdd.m_RoomType = TMBuildType.Junction;
                    m_Rooms.Add(roomToAdd);

                    m_nCurrentRooms++;

                    roomToAdd.m_RoomID = m_Rooms.Count - 1;

                    //Find our entrance offset
                    roomToAdd.m_GenerationEntrance = Direction.Up;
                    roomToAdd.OpenHallway(0, Direction.Up);

                    //previous room exit needs to know room ID of this room, this room's exit needs to know room ID of previous room
                    ExchangeRoomIDsForExits(previousRoom, roomToAdd);

                    //Exits that are available
                    currentExits.Add(Direction.Left);
                    //currentExits.Add (Direction.Down);
                    currentExits.Add(Direction.Right);
                    break;
                }
        }

        //Update Graph
        m_LevelGraph.AddNode(roomToAdd);
        m_LevelGraph.AddUndirectedEdge(m_LevelGraph.NodeList[connectingExit.RoomID], m_LevelGraph.NodeList[m_Rooms.Count - 1], (int)((m_LevelGraph.NodeList[connectingExit.RoomID].Node.transform.position - m_LevelGraph.NodeList[m_Rooms.Count - 1].Node.transform.position).sqrMagnitude) * 100);
        //Debug.Log ("Room Size: " + "Width: " + roomToAdd.m_fWidthInTiles + " , " + "Height: " + roomToAdd.m_fHeightInTiles);	
        //Debug.Log ("Exits");

        CreateUnusedExit(currentExits, nTestX, nTestY, roomToAdd, 0, TMBuildType.Junction);
        //				for (int nUnusedExit = 0; nUnusedExit < currentExits.Count; nUnusedExit++) {
        //						RoomExit exitToAdd = new RoomExit ();
        //						exitToAdd.RoomID = m_Rooms.Count - 1;
        //						exitToAdd.BuildType = TMBuildType.Junction;
        //						exitToAdd.ExitDirection = currentExits [nUnusedExit];
        //
        //						exitToAdd.OffSet = 0;
        //						exitToAdd.WorldPosition = roomToAdd.GetExitPoint (exitToAdd.OffSet, exitToAdd.ExitDirection);
        //						m_UnusedExits.Add (exitToAdd);
        //						m_UnusedJunctionExits.Add (exitToAdd);
        //				}
        currentExits.Clear();

        return true;
    }

    bool TestNextHallway(int nOffset, Direction eDirection, Tile_Map previousRoom, RoomExit connectingExit, int nRangeMin, int nRangeMax)
    {
        //Debug.Log("Hallway");
        //find what our room size would be first so we know how much size to test against
        int nTestX = 0;
        int nTestY = 0;
        Tile_Map roomToAdd = null;
        //Add Exits
        List<Direction> currentExits = new List<Direction>();

        switch (eDirection)
        {
            case Direction.Right:
                {
                    //Debug.Log ("Room#(Hallway Horizontal): " + m_nCurrentRooms);
                    //Are we vertical or horizontal
                    nTestX = Random.Range(nRangeMin, nRangeMax) * 2 + 1;
                    nTestY = 7;

                    //need the center to be in line with the center of the opening
                    Vector3 vCurrentCenter = new Vector3(((nTestX - 1) * m_fTileSize * .5f), 0f, 0f);
                    vCurrentCenter += connectingExit.WorldPosition;

                    //Offset for the Entrance
                    //int nMaxOffsetY = 0;
                    //int nEntranceOffsetY = 0;
                    //nMaxOffsetY = CalculateMaxOffset (nTestX, nTestY, Direction.Left);

                    //nEntranceOffsetY = Random.Range (-nMaxOffsetY, nMaxOffsetY);
                    //vCurrentCenter.y += (nEntranceOffsetY * m_fTileSize);
                    //at this point we have the size and position of the room we're trying to add, need to test all rooms in this area and not do this if we're colliding
                    if (TestRoomPlacement(nTestX, nTestY, vCurrentCenter, connectingExit.RoomID))
                    {
                        if (connectingExit.BuildType == TMBuildType.Hallway)
                            m_UnusedHallwayExits.Remove(connectingExit);
                        else if (connectingExit.BuildType == TMBuildType.Junction)
                            m_UnusedJunctionExits.Remove(connectingExit);
                        return false;
                    }

                    //Safe to Open Previous Room now
                    OpenPreviousRoom(previousRoom, connectingExit, eDirection);

                    //Debug.Log ("Max OffSetY: " + nMaxOffsetY);
                    //Debug.Log ("OffsetY: " + -nEntranceOffsetY);
                    roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), vCurrentCenter, Quaternion.identity) as Tile_Map;

                    roomToAdd.Initialization(nTestX, nTestY, TMBuildType.Basic_Room);
                    //roomToAdd.CreateTileMap (m_TileMapLocations [0]);
                    roomToAdd.CreateTileMap(m_TerrainTextures[0]);
                    roomToAdd.SetCorners();
                    roomToAdd.m_RoomType = TMBuildType.Hallway_Horizontal;
                    m_Rooms.Add(roomToAdd);

                    m_nCurrentRooms++;

                    roomToAdd.m_RoomID = m_Rooms.Count - 1;

                    //Open Entrance
                    roomToAdd.OpenHallway(0, Direction.Left);

                    //previous room exit needs to know room ID of this room, this room's exit needs to know room ID of previous room
                    ExchangeRoomIDsForExits(previousRoom, roomToAdd);

                    //Exits that are available
                    currentExits.Add(Direction.Right);
                    //currentExits.Add (Direction.Up);
                    //currentExits.Add (Direction.Down);

                    break;
                }
            case Direction.Left:
                {
                    //Debug.Log ("Room#(Hallway Horizontal): " + m_nCurrentRooms);
                    //Are we vertical or horizontal
                    nTestX = Random.Range(nRangeMin, nRangeMax) * 2 + 1;
                    nTestY = 7;

                    //need the center to be in line with the center of the opening
                    Vector3 vCurrentCenter = new Vector3(((nTestX - 1) * -m_fTileSize * .5f), 0f, 0f);
                    vCurrentCenter += connectingExit.WorldPosition;

                    //Offset for the Entrance
                    //int nMaxOffsetY = 0;
                    //int nEntranceOffsetY = 0;
                    //nMaxOffsetY = CalculateMaxOffset (nTestX, nTestY, Direction.Right);

                    //nEntranceOffsetY = Random.Range (-nMaxOffsetY, nMaxOffsetY);
                    //vCurrentCenter.y += (nEntranceOffsetY * m_fTileSize);
                    //at this point we have the size and position of the room we're trying to add, need to test all rooms in this area and not do this if we're colliding
                    if (TestRoomPlacement(nTestX, nTestY, vCurrentCenter, connectingExit.RoomID))
                    {
                        if (connectingExit.BuildType == TMBuildType.Hallway)
                            m_UnusedHallwayExits.Remove(connectingExit);
                        else if (connectingExit.BuildType == TMBuildType.Junction)
                            m_UnusedJunctionExits.Remove(connectingExit);
                        return false;
                    }
                    //Safe to Open Previous Room now
                    OpenPreviousRoom(previousRoom, connectingExit, eDirection);

                    //Debug.Log ("Max OffSetY: " + nMaxOffsetY);
                    //Debug.Log ("OffsetY: " + -nEntranceOffsetY);
                    roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), vCurrentCenter, Quaternion.identity) as Tile_Map;

                    roomToAdd.Initialization(nTestX, nTestY, TMBuildType.Basic_Room);
                    //roomToAdd.CreateTileMap (m_TileMapLocations [0]);
                    roomToAdd.CreateTileMap(m_TerrainTextures[0]);
                    roomToAdd.SetCorners();
                    roomToAdd.m_RoomType = TMBuildType.Hallway_Horizontal;
                    m_Rooms.Add(roomToAdd);

                    m_nCurrentRooms++;

                    roomToAdd.m_RoomID = m_Rooms.Count - 1;

                    //Open Entrance
                    roomToAdd.OpenHallway(0, Direction.Right);

                    //previous room exit needs to know room ID of this room, this room's exit needs to know room ID of previous room
                    ExchangeRoomIDsForExits(previousRoom, roomToAdd);

                    //Exits that are available
                    currentExits.Add(Direction.Left);
                    //currentExits.Add (Direction.Up);
                    //currentExits.Add (Direction.Down);
                    break;
                }
            case Direction.Up:
                {
                    //Debug.Log ("Room#(Hallway Vertical): " + m_nCurrentRooms);
                    //Are we vertical or horizontal
                    nTestX = 7;
                    nTestY = Random.Range(nRangeMin, nRangeMax) * 2 + 1;

                    //need the center to be in line with the center of the opening
                    Vector3 vCurrentCenter = new Vector3(0f, ((nTestY - 1) * m_fTileSize * .5f), 0f);
                    vCurrentCenter += connectingExit.WorldPosition;

                    //Offset for the Entrance
                    //int nMaxOffsetX = 0;
                    //int nEntranceOffsetX = 0;
                    //nMaxOffsetX = CalculateMaxOffset (nTestX, nTestY, Direction.Down);

                    //nEntranceOffsetX = Random.Range (-nMaxOffsetX, nMaxOffsetX);
                    //vCurrentCenter.x += (nEntranceOffsetX * m_fTileSize);
                    //at this point we have the size and position of the room we're trying to add, need to test all rooms in this area and not do this if we're colliding
                    if (TestRoomPlacement(nTestX, nTestY, vCurrentCenter, connectingExit.RoomID))
                    {
                        if (connectingExit.BuildType == TMBuildType.Hallway)
                            m_UnusedHallwayExits.Remove(connectingExit);
                        else if (connectingExit.BuildType == TMBuildType.Junction)
                            m_UnusedJunctionExits.Remove(connectingExit);
                        return false;
                    }
                    //Safe to Open Previous Room now
                    OpenPreviousRoom(previousRoom, connectingExit, eDirection);

                    //Debug.Log ("Max OffSetX: " + nMaxOffsetX);
                    //Debug.Log ("OffsetX: " + -nEntranceOffsetX);
                    roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), vCurrentCenter, Quaternion.identity) as Tile_Map;

                    roomToAdd.Initialization(nTestX, nTestY, TMBuildType.Basic_Room);
                    //roomToAdd.CreateTileMap (m_TileMapLocations [0]);
                    roomToAdd.CreateTileMap(m_TerrainTextures[0]);
                    roomToAdd.SetCorners();
                    roomToAdd.m_RoomType = TMBuildType.Hallway_Vertical;
                    m_Rooms.Add(roomToAdd);

                    m_nCurrentRooms++;

                    roomToAdd.m_RoomID = m_Rooms.Count - 1;

                    //Find our entrance offset
                    //Open Entrance
                    roomToAdd.OpenHallway(0, Direction.Down);

                    //previous room exit needs to know room ID of this room, this room's exit needs to know room ID of previous room
                    ExchangeRoomIDsForExits(previousRoom, roomToAdd);

                    //Exits that are available
                    //currentExits.Add (Direction.Left);
                    currentExits.Add(Direction.Up);
                    //currentExits.Add (Direction.Right);
                    break;
                }
            case Direction.Down:
                {
                    //Debug.Log ("Room#(Hallway Vertical): " + m_nCurrentRooms);
                    //Are we vertical or horizontal
                    nTestX = 7;
                    nTestY = Random.Range(nRangeMin, nRangeMax) * 2 + 1;

                    //need the center to be in line with the center of the opening
                    Vector3 vCurrentCenter = new Vector3(0f, ((nTestY - 1) * -m_fTileSize * .5f), 0f);
                    vCurrentCenter += connectingExit.WorldPosition;

                    //Offset for the Entrance
                    //int nMaxOffsetX = 0;
                    //int nEntranceOffsetX = 0;
                    //nMaxOffsetX = CalculateMaxOffset (nTestX, nTestY, Direction.Up);

                    //nEntranceOffsetX = Random.Range (-nMaxOffsetX, nMaxOffsetX);
                    //vCurrentCenter.x += (nEntranceOffsetX * m_fTileSize);
                    //at this point we have the size and position of the room we're trying to add, need to test all rooms in this area and not do this if we're colliding
                    if (TestRoomPlacement(nTestX, nTestY, vCurrentCenter, connectingExit.RoomID))
                    {
                        if (connectingExit.BuildType == TMBuildType.Hallway)
                            m_UnusedHallwayExits.Remove(connectingExit);
                        else if (connectingExit.BuildType == TMBuildType.Junction)
                            m_UnusedJunctionExits.Remove(connectingExit);
                        return false;
                    }
                    //Safe to Open Previous Room now
                    OpenPreviousRoom(previousRoom, connectingExit, eDirection);

                    //Debug.Log ("Max OffSetX: " + nMaxOffsetX);
                    //Debug.Log ("OffsetX: " + -nEntranceOffsetX);
                    roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), vCurrentCenter, Quaternion.identity) as Tile_Map;

                    roomToAdd.Initialization(nTestX, nTestY, TMBuildType.Basic_Room);
                    //roomToAdd.CreateTileMap (m_TileMapLocations [0]);
                    roomToAdd.CreateTileMap(m_TerrainTextures[0]);
                    roomToAdd.SetCorners();
                    roomToAdd.m_RoomType = TMBuildType.Hallway_Vertical;
                    m_Rooms.Add(roomToAdd);

                    m_nCurrentRooms++;

                    roomToAdd.m_RoomID = m_Rooms.Count - 1;

                    //Find our entrance offset
                    roomToAdd.OpenHallway(0, Direction.Up);

                    //previous room exit needs to know room ID of this room, this room's exit needs to know room ID of previous room
                    ExchangeRoomIDsForExits(previousRoom, roomToAdd);

                    //Exits that are available
                    //currentExits.Add (Direction.Left);
                    currentExits.Add(Direction.Down);
                    //currentExits.Add (Direction.Right);
                    break;
                }
        }

        //Update Graph
        m_LevelGraph.AddNode(roomToAdd);
        m_LevelGraph.AddUndirectedEdge(m_LevelGraph.NodeList[connectingExit.RoomID], m_LevelGraph.NodeList[m_Rooms.Count - 1], (int)((m_LevelGraph.NodeList[connectingExit.RoomID].Node.transform.position - m_LevelGraph.NodeList[m_Rooms.Count - 1].Node.transform.position).sqrMagnitude) * 100);
        //int nMaxOffset = 0;
        //Debug.Log ("Room Size: " + "Width: " + roomToAdd.m_fWidthInTiles + " , " + "Height: " + roomToAdd.m_fHeightInTiles);	
        //Debug.Log ("Exits");
        CreateUnusedExit(currentExits, nTestX, nTestY, roomToAdd, 0, TMBuildType.Hallway);
        //				for (int nUnusedExit = 0; nUnusedExit < currentExits.Count; nUnusedExit++) {
        //						RoomExit exitToAdd = new RoomExit ();
        //						exitToAdd.RoomID = m_Rooms.Count - 1;
        //						exitToAdd.BuildType = TMBuildType.Hallway;
        //
        //						exitToAdd.ExitDirection = currentExits [nUnusedExit];
        //
        //						exitToAdd.OffSet = 0;
        //						exitToAdd.WorldPosition = roomToAdd.GetExitPoint (exitToAdd.OffSet, exitToAdd.ExitDirection);
        //						m_UnusedExits.Add (exitToAdd);
        //						m_UnusedHallwayExits.Add (exitToAdd);
        //				}
        currentExits.Clear();

        return true;

    }


    public void ExchangeRoomIDsForExits(Tile_Map ourPreviousRoom, Tile_Map currentRoom)
    {
        int nLatestExit = (ourPreviousRoom.m_Exits.Count - 1);

        if (ourPreviousRoom.m_RoomType != TMBuildType.Junction)
        {
            //ourPreviousRoom.m_Exits[0].OtherRoomID = 
            TileMapExit tme = new TileMapExit();
            tme.Direction = ourPreviousRoom.m_Exits[nLatestExit].Direction;
            tme.Offset = ourPreviousRoom.m_Exits[nLatestExit].Offset;
            tme.OtherRoomID = currentRoom.m_RoomID;
            ourPreviousRoom.m_Exits.Remove(ourPreviousRoom.m_Exits[nLatestExit]);
            ourPreviousRoom.m_Exits.Add(tme);
        }
        nLatestExit = (currentRoom.m_Exits.Count - 1);
        TileMapExit tme2 = new TileMapExit();
        tme2.Direction = currentRoom.m_Exits[nLatestExit].Direction;
        tme2.Offset = currentRoom.m_Exits[nLatestExit].Offset;
        tme2.OtherRoomID = ourPreviousRoom.m_RoomID;
        currentRoom.m_Exits.Remove(currentRoom.m_Exits[nLatestExit]);
        currentRoom.m_Exits.Add(tme2);
    }

    bool TestNextRoom(int nOffset, Direction eDirection, Tile_Map previousRoom, RoomExit connectingExit, int nRangeMin, int nRangeMax, int nMaxExits)
    {
        //Debug.Log("Room");
        //Debug.Log ("Room#: " + m_nCurrentRooms);
        //find what our room size would be first so we know how much size to test against
        int nTestX = 0;
        int nTestY = 0;
        nTestX = Random.Range(nRangeMin, nRangeMax) * 2 + 1;
        nTestY = Random.Range(nRangeMin, nRangeMax) * 2 + 1;

        Tile_Map roomToAdd = null;
        //Add Exits
        List<Direction> currentExits = new List<Direction>();

        switch (eDirection)
        {
            case Direction.Right:
                {
                    //need the center to be in line with the center of the opening
                    Vector3 vCurrentCenter = new Vector3(((nTestX - 1) * m_fTileSize * .5f), 0f, 0f);
                    vCurrentCenter += connectingExit.WorldPosition;

                    //Offset for the Entrance
                    int nMaxOffsetY = 0;
                    int nEntranceOffsetY = 0;
                    nMaxOffsetY = CalculateMaxOffset(nTestX, nTestY, Direction.Left);

                    nEntranceOffsetY = Random.Range(-nMaxOffsetY, nMaxOffsetY);
                    vCurrentCenter.y += (nEntranceOffsetY * m_fTileSize);
                    //at this point we have the size and position of the room we're trying to add, need to test all rooms in this area and not do this if we're colliding
                    if (TestRoomPlacement(nTestX, nTestY, vCurrentCenter, connectingExit.RoomID))
                    {
                        if (connectingExit.BuildType == TMBuildType.Hallway)
                            m_UnusedHallwayExits.Remove(connectingExit);
                        else if (connectingExit.BuildType == TMBuildType.Junction)
                            m_UnusedJunctionExits.Remove(connectingExit);
                        return false;
                    }

                    //Safe to Open Previous Room now
                    OpenPreviousRoom(previousRoom, connectingExit, eDirection);

                    roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), vCurrentCenter, Quaternion.identity) as Tile_Map;

                    roomToAdd.Initialization(nTestX, nTestY);
                    //roomToAdd.CreateTileMap (m_TileMapLocations [0]);
                    roomToAdd.CreateTileMap(m_TerrainTextures[0]);
                    roomToAdd.SetCorners();
                    roomToAdd.m_RoomType = TMBuildType.Basic_Room;
                    m_Rooms.Add(roomToAdd);

                    m_nCurrentRooms++;

                    roomToAdd.m_RoomID = m_Rooms.Count - 1;

                    //Open Entrance
                    roomToAdd.OpenRoom(-nEntranceOffsetY, Direction.Left);

                    //previous room exit needs to know room ID of this room, this room's exit needs to know room ID of previous room
                    ExchangeRoomIDsForExits(previousRoom, roomToAdd);

                    //Exits that are available
                    currentExits.Add(Direction.Right);
                    currentExits.Add(Direction.Up);
                    currentExits.Add(Direction.Down);
                    break;
                }
            case Direction.Left:
                {
                    //need the center to be in line with the center of the opening
                    Vector3 vCurrentCenter = new Vector3(((nTestX - 1) * -m_fTileSize * .5f), 0f, 0f);
                    vCurrentCenter += connectingExit.WorldPosition;

                    //Offset for the Entrance
                    int nMaxOffsetY = 0;
                    int nEntranceOffsetY = 0;
                    nMaxOffsetY = CalculateMaxOffset(nTestX, nTestY, Direction.Right);

                    nEntranceOffsetY = Random.Range(-nMaxOffsetY, nMaxOffsetY);
                    vCurrentCenter.y += (nEntranceOffsetY * m_fTileSize);
                    //at this point we have the size and position of the room we're trying to add, need to test all rooms in this area and not do this if we're colliding
                    if (TestRoomPlacement(nTestX, nTestY, vCurrentCenter, connectingExit.RoomID))
                    {
                        if (connectingExit.BuildType == TMBuildType.Hallway)
                            m_UnusedHallwayExits.Remove(connectingExit);
                        else if (connectingExit.BuildType == TMBuildType.Junction)
                            m_UnusedJunctionExits.Remove(connectingExit);
                        return false;
                    }
                    //Safe to Open Previous Room now
                    OpenPreviousRoom(previousRoom, connectingExit, eDirection);

                    roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), vCurrentCenter, Quaternion.identity) as Tile_Map;

                    roomToAdd.Initialization(nTestX, nTestY);
                    //roomToAdd.CreateTileMap (m_TileMapLocations [0]);
                    roomToAdd.CreateTileMap(m_TerrainTextures[0]);
                    roomToAdd.SetCorners();
                    roomToAdd.m_RoomType = TMBuildType.Basic_Room;
                    m_Rooms.Add(roomToAdd);

                    m_nCurrentRooms++;

                    roomToAdd.m_RoomID = m_Rooms.Count - 1;

                    //Open Entrance
                    roomToAdd.OpenRoom(-nEntranceOffsetY, Direction.Right);

                    //previous room exit needs to know room ID of this room, this room's exit needs to know room ID of previous room
                    ExchangeRoomIDsForExits(previousRoom, roomToAdd);

                    //Exits that are available
                    currentExits.Add(Direction.Left);
                    currentExits.Add(Direction.Up);
                    currentExits.Add(Direction.Down);
                    break;
                }

            case Direction.Up:
                {
                    //need the center to be in line with the center of the opening
                    Vector3 vCurrentCenter = new Vector3(0f, ((nTestY - 1) * m_fTileSize * .5f), 0f);
                    vCurrentCenter += connectingExit.WorldPosition;

                    //Offset for the Entrance
                    int nMaxOffsetX = 0;
                    int nEntranceOffsetX = 0;
                    nMaxOffsetX = CalculateMaxOffset(nTestX, nTestY, Direction.Down);

                    nEntranceOffsetX = Random.Range(-nMaxOffsetX, nMaxOffsetX);
                    vCurrentCenter.x += (nEntranceOffsetX * m_fTileSize);
                    //at this point we have the size and position of the room we're trying to add, need to test all rooms in this area and not do this if we're colliding
                    if (TestRoomPlacement(nTestX, nTestY, vCurrentCenter, connectingExit.RoomID))
                    {
                        if (connectingExit.BuildType == TMBuildType.Hallway)
                            m_UnusedHallwayExits.Remove(connectingExit);
                        else if (connectingExit.BuildType == TMBuildType.Junction)
                            m_UnusedJunctionExits.Remove(connectingExit);
                        return false;
                    }
                    //Safe to Open Previous Room now
                    OpenPreviousRoom(previousRoom, connectingExit, eDirection);

                    roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), vCurrentCenter, Quaternion.identity) as Tile_Map;

                    roomToAdd.Initialization(nTestX, nTestY);
                    //roomToAdd.CreateTileMap (m_TileMapLocations [0]);
                    roomToAdd.CreateTileMap(m_TerrainTextures[0]);
                    roomToAdd.SetCorners();
                    roomToAdd.m_RoomType = TMBuildType.Basic_Room;
                    m_Rooms.Add(roomToAdd);

                    m_nCurrentRooms++;


                    roomToAdd.m_RoomID = m_Rooms.Count - 1;

                    //Find our entrance offset
                    //Open Entrance
                    roomToAdd.OpenRoom(-nEntranceOffsetX, Direction.Down);

                    //previous room exit needs to know room ID of this room, this room's exit needs to know room ID of previous room
                    ExchangeRoomIDsForExits(previousRoom, roomToAdd);

                    //Exits that are available
                    currentExits.Add(Direction.Left);
                    currentExits.Add(Direction.Up);
                    currentExits.Add(Direction.Right);
                    break;
                }

            case Direction.Down:
                {
                    //need the center to be in line with the center of the opening
                    Vector3 vCurrentCenter = new Vector3(0f, ((nTestY - 1) * -m_fTileSize * .5f), 0f);
                    vCurrentCenter += connectingExit.WorldPosition;

                    //Offset for the Entrance
                    int nMaxOffsetX = 0;
                    int nEntranceOffsetX = 0;
                    nMaxOffsetX = CalculateMaxOffset(nTestX, nTestY, Direction.Up);

                    nEntranceOffsetX = Random.Range(-nMaxOffsetX, nMaxOffsetX);
                    vCurrentCenter.x += (nEntranceOffsetX * m_fTileSize);
                    //at this point we have the size and position of the room we're trying to add, need to test all rooms in this area and not do this if we're colliding
                    if (TestRoomPlacement(nTestX, nTestY, vCurrentCenter, connectingExit.RoomID))
                    {
                        if (connectingExit.BuildType == TMBuildType.Hallway)
                            m_UnusedHallwayExits.Remove(connectingExit);
                        else if (connectingExit.BuildType == TMBuildType.Junction)
                            m_UnusedJunctionExits.Remove(connectingExit);
                        return false;
                    }
                    //Safe to Open Previous Room now
                    OpenPreviousRoom(previousRoom, connectingExit, eDirection);

                    roomToAdd = Instantiate(Resources.Load("Prefabs/Tile_Map", typeof(Tile_Map)), vCurrentCenter, Quaternion.identity) as Tile_Map;

                    roomToAdd.Initialization(nTestX, nTestY);
                    //roomToAdd.CreateTileMap (m_TileMapLocations [0]);
                    roomToAdd.CreateTileMap(m_TerrainTextures[0]);
                    roomToAdd.SetCorners();
                    roomToAdd.m_RoomType = TMBuildType.Basic_Room;
                    m_Rooms.Add(roomToAdd);

                    m_nCurrentRooms++;

                    roomToAdd.m_RoomID = m_Rooms.Count - 1;

                    //Find our entrance offset
                    roomToAdd.OpenRoom(-nEntranceOffsetX, Direction.Up);

                    //previous room exit needs to know room ID of this room, this room's exit needs to know room ID of previous room
                    ExchangeRoomIDsForExits(previousRoom, roomToAdd);

                    //Exits that are available
                    currentExits.Add(Direction.Left);
                    currentExits.Add(Direction.Down);
                    currentExits.Add(Direction.Right);
                    break;
                }
        }

        //Update Graph
        //Update Graph
        m_LevelGraph.AddNode(roomToAdd);
        m_LevelGraph.AddUndirectedEdge(m_LevelGraph.NodeList[connectingExit.RoomID], m_LevelGraph.NodeList[m_Rooms.Count - 1], (int)((m_LevelGraph.NodeList[connectingExit.RoomID].Node.transform.position - m_LevelGraph.NodeList[m_Rooms.Count - 1].Node.transform.position).sqrMagnitude) * 100);

        //CreateUnusedExit(List<Direction> exits, xCoord, yCoord, Tile_Map roomToAdd);

        //				int nMaxOffset = 0;
        //				//Debug.Log ("Room Size: " + "Width: " + roomToAdd.m_fWidthInTiles + " , " + "Height: " + roomToAdd.m_fHeightInTiles);	
        //				//Debug.Log ("Exits");
        CreateUnusedExit(currentExits, nTestX, nTestY, roomToAdd, 0, TMBuildType.Basic_Room);
        //				for (int nUnusedExit = 0; nUnusedExit < currentExits.Count; nUnusedExit++) {
        //						RoomExit exitToAdd = new RoomExit ();
        //						exitToAdd.RoomID = m_Rooms.Count - 1;
        //						exitToAdd.BuildType = TMBuildType.Basic_Room;
        //						exitToAdd.ExitDirection = currentExits [nUnusedExit];
        //			
        //						nMaxOffset = CalculateMaxOffset (nTestX, nTestY, exitToAdd.ExitDirection);
        //			
        //						exitToAdd.OffSet = Random.Range (-nMaxOffset, nMaxOffset);//need to calculate other offsets
        //						exitToAdd.WorldPosition = roomToAdd.GetExitPoint (exitToAdd.OffSet, exitToAdd.ExitDirection);
        //						m_UnusedExits.Add (exitToAdd);
        //				}
        currentExits.Clear();
        return true;
    }

    bool TestRoomPlacement(Tile_Map roomToTest)
    {
        int nStuff = 0;
        for (nStuff = 0; nStuff < m_Rooms.Count; nStuff++)
        {
            if (m_Rooms[nStuff].RectCollisionTest(roomToTest))
                return true;
        }
        return false;
    }

    bool TestRoomPlacement(int nWidth, int nHeight, Vector3 vCenter)
    {
        Rect roomRect = new Rect();

        roomRect.width = (float)(nWidth) * m_fTileSize;
        roomRect.height = (float)(nHeight) * m_fTileSize;
        roomRect.center = new Vector2(vCenter.x, vCenter.y);

        //Debug.Log ("Test Rect: Center: " + roomRect.center + " , Width: " + roomRect.width + " , Height: " + roomRect.height);
        //for each room in rooms, if this rect overlaps, return true
        for (int nRoom = 0; nRoom < m_Rooms.Count; nRoom++)
        {
            if (m_Rooms[nRoom].RectCollisionTest(roomRect))
            {
                //Debug.Log ("Add Feature: [Room] : No space, Rerolling...");
                return true;
            }
        }

        return false;
    }

    bool TestRoomPlacement(int nWidth, int nHeight, Vector3 vCenter, int nPreviousRoomID)
    {
        //TODO:1/7/2016 replace this with raycasting: 5 rays that would cover the tiles just outside the tilemap. All along the borders, plus one that runs through the middle
        //distance should be height + 2 tiles and width + 2 tiles 1 additional tile on each side for the outer border
        float fX = (float)((nWidth + 1) * .5f) * m_fTileSize;
        float fY = (float)((nHeight + 1) * .5f) * m_fTileSize;
        Vector2 vRayPosition = Vector2.zero;
        Vector2 vDirection = Vector2.zero;
        float fSearchDistance = 0f; //width or height + 1 tile

        //BottomLeft - UP
        vRayPosition.x = vCenter.x - fX;
        vRayPosition.y = vCenter.y - fY;
        vDirection = Vector2.up;
        fSearchDistance = (nHeight + 1) * m_fTileSize;

        RaycastHit2D hit = Physics2D.Raycast(vRayPosition, vDirection, fSearchDistance, (1 << LayerMask.NameToLayer("Walls")));

        if (hit.collider != null)
        {
            return true;
        }

        //BottomRight - LEFT
        vRayPosition.x = vCenter.x + fX;
        vRayPosition.y = vCenter.y - fY;
        vDirection = Vector2.left;
        fSearchDistance = (nWidth + 1) * m_fTileSize;

        hit = Physics2D.Raycast(vRayPosition, vDirection, fSearchDistance, (1 << LayerMask.NameToLayer("Walls")));

        if (hit.collider != null)
        {
            return true;
        }

        //TopLeft - RIGHT
        vRayPosition.x = vCenter.x - fX;
        vRayPosition.y = vCenter.y + fY;
        vDirection = Vector2.right;
        fSearchDistance = (nWidth + 1) * m_fTileSize;

        hit = Physics2D.Raycast(vRayPosition, vDirection, fSearchDistance, (1 << LayerMask.NameToLayer("Walls")));

        if (hit.collider != null)
        {
            return true;
        }

        //TopRight - DOWN
        vRayPosition.x = vCenter.x + fX;
        vRayPosition.y = vCenter.y + fY;
        vDirection = Vector2.down;
        fSearchDistance = (nWidth + 1) * m_fTileSize;

        hit = Physics2D.Raycast(vRayPosition, vDirection, fSearchDistance, (1 << LayerMask.NameToLayer("Walls")));

        if (hit.collider != null)
        {
            return true;
        }

        //CenterBottom - UP
        vRayPosition.x = vCenter.x;
        vRayPosition.y = vCenter.y - fY;
        vDirection = Vector2.up;
        fSearchDistance = (nWidth + 1) * m_fTileSize;

        hit = Physics2D.Raycast(vRayPosition, vDirection, fSearchDistance, (1 << LayerMask.NameToLayer("Walls")));

        if (hit.collider != null)
        {
            return true;
        }

        //Rect roomRect = new Rect();

        //roomRect.width = (float)(nWidth) * m_fTileSize;
        //roomRect.height = (float)(nHeight) * m_fTileSize;
        //roomRect.center = new Vector2(vCenter.x, vCenter.y);

        ////Debug.Log ("Test Rect: Center: " + roomRect.center + " , Width: " + roomRect.width + " , Height: " + roomRect.height);
        ////for each room in rooms, if this rect overlaps, return true
        //for (int nRoom = 0; nRoom < m_Rooms.Count; nRoom++)
        //{
        //    if (nRoom == nPreviousRoomID)
        //        continue;
        //    if (m_Rooms[nRoom].RectCollisionTest(roomRect))
        //    {
        //        //Debug.Log ("Add Feature: [Room] : No space, Rerolling...");
        //        return true;
        //    }
        //}

        return false;
    }

    int GenerateExits(int nMin, int nMax)
    {
        return Random.Range(nMin, nMax);
    }

    //		void SetAvailableExits (List<Direction> exitsToSet, TMBuildType roomType, Direction entranceDirection)
    //		{
    //		
    //				switch (roomType) {
    //				case TMBuildType.Basic_Room:
    //						{
    //								switch (entranceDirection) {
    //								case Direction.Right:
    //										{
    //												exitsToSet.Add (Direction.Right);
    //												exitsToSet.Add (Direction.Up);
    //												exitsToSet.Add (Direction.Down);
    //												break;
    //										}
    //								case Direction.Left:
    //										{
    //												exitsToSet.Add (Direction.Left);
    //												exitsToSet.Add (Direction.Up);
    //												exitsToSet.Add (Direction.Down);
    //												break;
    //										}
    //				
    //								case Direction.Up:
    //										{
    //												exitsToSet.Add (Direction.Left);
    //												exitsToSet.Add (Direction.Up);
    //												exitsToSet.Add (Direction.Right);
    //												break;
    //										}
    //								case Direction.Down:
    //										{
    //												exitsToSet.Add (Direction.Left);
    //												exitsToSet.Add (Direction.Down);
    //												exitsToSet.Add (Direction.Right);
    //												break;
    //										}
    //				
    //				
    //								}
    //								break;
    //						}
    //				case TMBuildType.Hallway_Horizontal:
    //						{
    //								switch (entranceDirection) {
    //								case Direction.Right:
    //										{
    //												exitsToSet.Add (Direction.Right);
    //												break;
    //										}
    //								case Direction.Left:
    //										{
    //												exitsToSet.Add (Direction.Left);
    //												break;
    //										}
    //				
    //								//								case Direction.Up:
    //								//										{
    //								//												
    //								//												break;
    //								//										}
    //								//								case Direction.Down:
    //								//										{
    //								//								
    //								//												break;
    //								//										}
    //								//				
    //								//				
    //								}
    //			
    //								break;
    //						}
    //				case TMBuildType.Hallway_Vertical:
    //						{
    //								switch (entranceDirection) {
    //								//								case Direction.Right:
    //								//										{
    //								//												
    //								//												break;
    //								//										}
    //								//								case Direction.Left:
    //								//										{
    //								//												break;
    //								//										}
    //				
    //								case Direction.Up:
    //										{
    //												exitsToSet.Add (Direction.Up);	
    //												break;
    //										}
    //								case Direction.Down:
    //										{
    //												exitsToSet.Add (Direction.Down);
    //												break;
    //										}
    //				
    //				
    //								}
    //			
    //								break;
    //						}
    //				case TMBuildType.Junction:
    //						{
    //								switch (entranceDirection) {
    //								case Direction.Right:
    //										{
    //												exitsToSet.Add (Direction.Right);
    //												exitsToSet.Add (Direction.Up);
    //												exitsToSet.Add (Direction.Down);
    //												break;
    //										}
    //								case Direction.Left:
    //										{
    //												exitsToSet.Add (Direction.Left);
    //												exitsToSet.Add (Direction.Up);
    //												exitsToSet.Add (Direction.Down);
    //												break;
    //										}
    //				
    //								case Direction.Up:
    //										{
    //												exitsToSet.Add (Direction.Left);
    //												exitsToSet.Add (Direction.Up);
    //												exitsToSet.Add (Direction.Right);
    //												break;
    //										}
    //								case Direction.Down:
    //										{
    //												exitsToSet.Add (Direction.Left);
    //												exitsToSet.Add (Direction.Down);
    //												exitsToSet.Add (Direction.Right);
    //												break;
    //										}
    //				
    //								}
    //								break;
    //						}
    //				}
    //		}
}
