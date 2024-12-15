using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuickLevelGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/Platforming Test Level")]
    static void CreateTestLevel()
    {
        // Ground
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.localScale = new Vector3(10, 1, 10);
        ground.name = "Ground";

        // Platforms
        CreatePlatform(new Vector3(5, 2, 0), new Vector3(3, 0.5f, 3));
        CreatePlatform(new Vector3(8, 4, 3), new Vector3(3, 0.5f, 3));
        CreatePlatform(new Vector3(12, 6, 0), new Vector3(3, 0.5f, 3));

        // Ramps
        CreateRamp(new Vector3(-5, 1, 0), new Vector3(6, 0.5f, 3), 20);
        CreateRamp(new Vector3(0, 2, -8), new Vector3(3, 0.5f, 3), 35);

        // Player
        var player = new GameObject("Player");
        player.AddComponent<CharacterController>();
        player.GetComponent<CharacterController>().height = 2f;
        player.GetComponent<CharacterController>().radius = 0.5f;
        player.GetComponent<CharacterController>().center = Vector3.up;

        var camera = new GameObject("Main Camera");
        camera.AddComponent<Camera>();
        camera.transform.SetParent(player.transform);
        camera.transform.localPosition = new Vector3(0, 1.6f, 0);

        player.AddComponent<PlatformingController>().cameraTransform = camera.transform;
    }

    static void CreatePlatform(Vector3 position, Vector3 scale)
    {
        var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.transform.position = position;
        platform.transform.localScale = scale;
        platform.name = "Platform";
    }

    static void CreateRamp(Vector3 position, Vector3 scale, float angle)
    {
        var ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ramp.transform.position = position;
        ramp.transform.localScale = scale;
        ramp.transform.rotation = Quaternion.Euler(-angle, 0, 0);
        ramp.name = "Ramp";
    }
#endif
}
