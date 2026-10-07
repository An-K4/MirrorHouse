using UnityEngine;

public class RoleC_AIWaypoints
{
    public static void SetupWaypoints()
    {
        Debug.Log("Setting up AI waypoints...");

        GameObject ai = GameObject.Find("AI");
        if (ai == null)
        {
            ai = new GameObject("AI");
        }

        GameObject waypointsGroup = new GameObject("Waypoints");
        waypointsGroup.transform.SetParent(ai.transform);
        waypointsGroup.transform.position = Vector3.zero;

        // 7 Waypoints for patrol route
        CreateWaypoint(waypointsGroup, "WP_1_MainHall", new Vector3(-3, 0.5f, -12));
        CreateWaypoint(waypointsGroup, "WP_2_Kitchen", new Vector3(-8, 0.5f, -12));
        CreateWaypoint(waypointsGroup, "WP_3_Hallway", new Vector3(10, 0.5f, 0));
        CreateWaypoint(waypointsGroup, "WP_4_Bathroom", new Vector3(10, 0.5f, -6));
        CreateWaypoint(waypointsGroup, "WP_5_Staircase", new Vector3(2, 0.5f, -8));
        CreateWaypoint(waypointsGroup, "WP_6_Bedroom", new Vector3(0, 0.5f, 0));
        CreateWaypoint(waypointsGroup, "WP_7_Return", new Vector3(0, 0.5f, -12));

        Debug.Log("✓ 7 Waypoints created!");
    }

    static void CreateWaypoint(GameObject parent, string name, Vector3 pos)
    {
        GameObject waypoint = new GameObject(name);
        waypoint.transform.SetParent(parent.transform);
        waypoint.transform.position = pos;

        MeshFilter mf = waypoint.AddComponent<MeshFilter>();
        mf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

        MeshRenderer mr = waypoint.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0f, 1f, 0f, 0.5f);
        mr.material = mat;

        waypoint.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        waypoint.tag = "Waypoint";
    }
}