using UnityEngine;
using UnityEngine.AI;

public class RoleC_LevelBuilder
{
    public static void BuildCompleteLevelC()
    {
        Debug.Log("Building Level C complete scene...");

        ClearOldGeometry();

        BuildBedroomWalls();
        BuildHallway1Walls();
        BuildBathroomWalls();
        BuildStaircaseWalls();
        BuildMainHallWalls();
        BuildKitchenWalls();

        BuildBedroomFurniture();
        BuildHallwayFurniture();
        BuildBathroomFurniture();
        BuildMainHallFurniture();
        BuildKitchenFurniture();

        Debug.Log("✅ LEVEL_C BUILD COMPLETE!");
    }

    static void ClearOldGeometry()
    {
        Transform geom = GameObject.Find("Geometry")?.transform;
        if (geom != null)
        {
            foreach (Transform child in geom)
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }
    }

    static void BuildBedroomWalls()
    {
        GameObject rooms = GameObject.Find("Geometry/Rooms");
        GameObject bedroom = new GameObject("Bedroom");
        bedroom.transform.SetParent(rooms.transform);
        bedroom.transform.position = Vector3.zero;

        CreateWall(bedroom, "Floor", 8, 0.2f, 6, 0, -0.1f, 0, new Color(0.8f, 0.8f, 0.8f));
        CreateWall(bedroom, "WallNorth", 8, 3, 0.2f, 0, 1.5f, 3, Color.white);
        CreateWall(bedroom, "WallSouth", 8, 3, 0.2f, 0, 1.5f, -3, Color.white);
        CreateWall(bedroom, "WallEast", 0.2f, 3, 6, 4, 1.5f, 0, Color.white);
        CreateWall(bedroom, "WallWest", 0.2f, 3, 6, -4, 1.5f, 0, Color.white);
    }

    static void BuildHallway1Walls()
    {
        GameObject rooms = GameObject.Find("Geometry/Rooms");
        GameObject hallway = new GameObject("Hallway1");
        hallway.transform.SetParent(rooms.transform);
        hallway.transform.position = new Vector3(10, 0, 0);

        CreateWall(hallway, "Floor", 15, 0.2f, 3, 0, -0.1f, 0, new Color(0.8f, 0.8f, 0.8f));
        CreateWall(hallway, "WallNorth", 15, 3, 0.2f, 0, 1.5f, 1.5f, Color.white);
        CreateWall(hallway, "WallSouth", 15, 3, 0.2f, 0, 1.5f, -1.5f, Color.white);
        CreateWall(hallway, "WallEnd", 0.2f, 3, 3, 7.5f, 1.5f, 0, Color.white);
    }

    static void BuildBathroomWalls()
    {
        GameObject rooms = GameObject.Find("Geometry/Rooms");
        GameObject bathroom = new GameObject("Bathroom");
        bathroom.transform.SetParent(rooms.transform);
        bathroom.transform.position = new Vector3(10, 0, -6);

        CreateWall(bathroom, "Floor", 5, 0.2f, 4, 0, -0.1f, 0, new Color(0.8f, 0.8f, 0.8f));
        CreateWall(bathroom, "WallNorth", 5, 3, 0.2f, 0, 1.5f, 2, Color.white);
        CreateWall(bathroom, "WallSouth", 5, 3, 0.2f, 0, 1.5f, -2, Color.white);
        CreateWall(bathroom, "WallEast", 0.2f, 3, 4, 2.5f, 1.5f, 0, Color.white);
        CreateWall(bathroom, "WallWest", 0.2f, 3, 4, -2.5f, 1.5f, 0, Color.white);
    }

    static void BuildStaircaseWalls()
    {
        GameObject rooms = GameObject.Find("Geometry/Rooms");
        GameObject staircase = new GameObject("Staircase");
        staircase.transform.SetParent(rooms.transform);
        staircase.transform.position = new Vector3(2, 0, -8);

        CreateWall(staircase, "Floor", 4, 0.2f, 4, 0, -0.1f, 0, new Color(0.8f, 0.8f, 0.8f));
        CreateWall(staircase, "WallNorth", 4, 3.5f, 0.2f, 0, 1.75f, 2, Color.white);
        CreateWall(staircase, "WallSouth", 4, 3.5f, 0.2f, 0, 1.75f, -2, Color.white);
        CreateWall(staircase, "WallEast", 0.2f, 3.5f, 4, 2, 1.75f, 0, Color.white);
        CreateWall(staircase, "WallWest", 0.2f, 3.5f, 4, -2, 1.75f, 0, Color.white);
    }

    static void BuildMainHallWalls()
    {
        GameObject rooms = GameObject.Find("Geometry/Rooms");
        GameObject mainhall = new GameObject("MainHall");
        mainhall.transform.SetParent(rooms.transform);
        mainhall.transform.position = new Vector3(0, 0, -12);

        CreateWall(mainhall, "Floor", 10, 0.2f, 8, 0, -0.1f, 0, new Color(0.8f, 0.8f, 0.8f));
        CreateWall(mainhall, "WallNorth", 10, 3, 0.2f, 0, 1.5f, 4, Color.white);
        CreateWall(mainhall, "WallSouth", 10, 3, 0.2f, 0, 1.5f, -4, Color.white);
        CreateWall(mainhall, "WallEast", 0.2f, 3, 8, 5, 1.5f, 0, Color.white);
        CreateWall(mainhall, "WallWest", 0.2f, 3, 8, -5, 1.5f, 0, Color.white);
    }

    static void BuildKitchenWalls()
    {
        GameObject rooms = GameObject.Find("Geometry/Rooms");
        GameObject kitchen = new GameObject("Kitchen");
        kitchen.transform.SetParent(rooms.transform);
        kitchen.transform.position = new Vector3(-8, 0, -12);

        CreateWall(kitchen, "Floor", 7, 0.2f, 5, 0, -0.1f, 0, new Color(0.8f, 0.8f, 0.8f));
        CreateWall(kitchen, "WallNorth", 7, 3, 0.2f, 0, 1.5f, 2.5f, Color.white);
        CreateWall(kitchen, "WallSouth", 7, 3, 0.2f, 0, 1.5f, -2.5f, Color.white);
        CreateWall(kitchen, "WallEast", 0.2f, 3, 5, 3.5f, 1.5f, 0, Color.white);
        CreateWall(kitchen, "WallWest", 0.2f, 3, 5, -3.5f, 1.5f, 0, Color.white);
    }

    static void BuildBedroomFurniture()
    {
        GameObject furniture = GameObject.Find("Furniture");
        GameObject bedroomFurn = new GameObject("Bedroom");
        bedroomFurn.transform.SetParent(furniture.transform);
        bedroomFurn.transform.position = Vector3.zero;

        CreateFurniture(bedroomFurn, "Bed", 2, 1, 1, 0, 0.5f, 0);
        CreateFurniture(bedroomFurn, "Dresser", 1, 0.5f, 0.5f, 3, 0.25f, 2);
        CreateFurniture(bedroomFurn, "Nightstand", 0.5f, 0.5f, 0.5f, -3, 0.25f, 0);
        CreateFurniture(bedroomFurn, "Lamp", 0.3f, 0.3f, 0.3f, -3, 0.3f, 1);
        CreateFurniture(bedroomFurn, "Picture", 1.5f, 0.1f, 1, 0, 2, 3.8f);
        CreateFurniture(bedroomFurn, "Rug", 3, 0.1f, 2, 0, 0.05f, -2);
    }

    static void BuildHallwayFurniture()
    {
        GameObject furniture = GameObject.Find("Furniture");
        GameObject hallwayFurn = new GameObject("Hallway");
        hallwayFurn.transform.SetParent(furniture.transform);
        hallwayFurn.transform.position = new Vector3(10, 0, 0);

        CreateFurniture(hallwayFurn, "Bookshelf", 1, 2, 0.5f, 0, 1, 1.4f);
        CreateFurniture(hallwayFurn, "Plant", 0.5f, 1, 0.5f, 3, 0.5f, 0);
        CreateFurniture(hallwayFurn, "Chair", 0.6f, 0.9f, 0.6f, -2, 0.45f, -1);
        CreateFurniture(hallwayFurn, "SmallTable", 1, 0.7f, 0.8f, 6, 0.35f, 0);
        CreateFurniture(hallwayFurn, "Picture", 1.5f, 0.1f, 1, 5, 2, 1.4f);
    }

    static void BuildBathroomFurniture()
    {
        GameObject furniture = GameObject.Find("Furniture");
        GameObject bathroomFurn = new GameObject("Bathroom");
        bathroomFurn.transform.SetParent(furniture.transform);
        bathroomFurn.transform.position = new Vector3(10, 0, -6);

        CreateFurniture(bathroomFurn, "Sink", 1, 0.8f, 0.5f, 0, 0.4f, 1.8f);
        CreateFurniture(bathroomFurn, "Toilet", 0.4f, 0.6f, 0.4f, -1.5f, 0.3f, 1.8f);
        CreateFurniture(bathroomFurn, "Bathtub", 1.5f, 0.7f, 0.8f, 0, 0.35f, -1.5f);
        CreateFurniture(bathroomFurn, "Mirror", 0.8f, 0.1f, 0.8f, 0, 1.5f, 2.3f);
        CreateFurniture(bathroomFurn, "TowelRack", 0.3f, 1.5f, 0.1f, 2, 0.75f, 0);
    }

    static void BuildMainHallFurniture()
    {
        GameObject furniture = GameObject.Find("Furniture");
        GameObject mainhallFurn = new GameObject("MainHall");
        mainhallFurn.transform.SetParent(furniture.transform);
        mainhallFurn.transform.position = new Vector3(0, 0, -12);

        CreateFurniture(mainhallFurn, "Sofa", 2.5f, 0.9f, 1, -2, 0.45f, 0);
        CreateFurniture(mainhallFurn, "Table", 1.5f, 0.6f, 1, 0, 0.3f, 0);
        CreateFurniture(mainhallFurn, "ChairLeft", 0.7f, 0.9f, 0.7f, 1, 0.45f, -1.5f);
        CreateFurniture(mainhallFurn, "ChairRight", 0.7f, 0.9f, 0.7f, 1, 0.45f, 1.5f);
        CreateFurniture(mainhallFurn, "Lamp", 0.4f, 1.5f, 0.4f, -3, 0.75f, 2);
        CreateFurniture(mainhallFurn, "Rug", 4, 0.1f, 2.5f, 0, 0.05f, -1);
    }

    static void BuildKitchenFurniture()
    {
        GameObject furniture = GameObject.Find("Furniture");
        GameObject kitchenFurn = new GameObject("Kitchen");
        kitchenFurn.transform.SetParent(furniture.transform);
        kitchenFurn.transform.position = new Vector3(-8, 0, -12);

        CreateFurniture(kitchenFurn, "Counter", 3, 0.9f, 0.6f, 0, 0.45f, 1.8f);
        CreateFurniture(kitchenFurn, "Stove", 0.8f, 0.9f, 0.7f, -1.5f, 0.45f, 0);
        CreateFurniture(kitchenFurn, "Fridge", 0.8f, 1.8f, 0.7f, 1.5f, 0.9f, 0);
        CreateFurniture(kitchenFurn, "Sink", 1, 0.8f, 0.6f, 0, 0.4f, -1.5f);
        CreateFurniture(kitchenFurn, "Table", 1.2f, 0.7f, 1, -2, 0.35f, -1);
        CreateFurniture(kitchenFurn, "ChairL", 0.5f, 0.9f, 0.5f, -2, 0.45f, -2);
        CreateFurniture(kitchenFurn, "Shelves", 2, 1.5f, 0.3f, 0, 0.75f, -2.3f);
    }

    static void CreateWall(GameObject parent, string name, float x, float y, float z, float posX, float posY, float posZ, Color color)
    {
        GameObject wall = new GameObject(name);
        wall.transform.SetParent(parent.transform);
        wall.transform.position = new Vector3(posX, posY, posZ);

        MeshFilter mf = wall.AddComponent<MeshFilter>();
        mf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

        MeshRenderer mr = wall.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        mr.material = mat;

        BoxCollider bc = wall.AddComponent<BoxCollider>();
        bc.size = new Vector3(x, y, z);

        wall.tag = "NavigationStatic";
        wall.layer = LayerMask.NameToLayer("Environment");

        wall.transform.localScale = new Vector3(x, y, z);
    }

    static void CreateFurniture(GameObject parent, string name, float x, float y, float z, float posX, float posY, float posZ)
    {
        GameObject furn = new GameObject(name);
        furn.transform.SetParent(parent.transform);
        furn.transform.position = new Vector3(posX, posY, posZ);

        MeshFilter mf = furn.AddComponent<MeshFilter>();
        mf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

        MeshRenderer mr = furn.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.6f, 0.4f, 0.2f);
        mr.material = mat;

        BoxCollider bc = furn.AddComponent<BoxCollider>();
        bc.size = new Vector3(x, y, z);

        furn.tag = "FurnitureItem";
        furn.layer = LayerMask.NameToLayer("Furniture");

        furn.transform.localScale = new Vector3(x, y, z);
    }
}