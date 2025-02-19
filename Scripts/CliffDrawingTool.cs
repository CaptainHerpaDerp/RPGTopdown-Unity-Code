using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CliffDrawingTool : EditorWindow
{
    private enum DrawState
    {
        Drawing,
        Erasing,
        None
    }

    private DrawState drawState = DrawState.None;

    private int sizeX = 3;
    private int sizeY = 3;

    [SerializeField] private Tilemap targetTilemap;
    [SerializeField] private TileBase tileToDraw;

    private bool isDrawing = false;
    private Tool previousTool;

    [MenuItem("Window/Cliff Drawing Tool")]
    public static void ShowWindow()
    {
        GetWindow<CliffDrawingTool>("Cliff Drawing Tool");
    }

    private void OnGUI()
    {
        // Create a button to add a new tilemap
        if (GUILayout.Button("Add New Tilemap"))
        {
            // Set the position the the center of the scene view
            Vector3 scenePos = SceneView.lastActiveSceneView.camera.transform.position;

            GameObject tilemapObject = new GameObject("Tilemap");
            tilemapObject.transform.position = new Vector3(scenePos.x, scenePos.y, 0);
            tilemapObject.AddComponent<Tilemap>();
            tilemapObject.AddComponent<TilemapRenderer>();
            tilemapObject.transform.SetParent(FindObjectOfType<Grid>().transform.GetChild(0));

            tilemapObject.GetComponent<TilemapRenderer>().sortingLayerName = "Actor";
            tilemapObject.GetComponent<TilemapRenderer>().sortingOrder = 1;
            targetTilemap = tilemapObject.GetComponent<Tilemap>();
            
        }

        GUILayout.Label("Rectangle Settings", EditorStyles.boldLabel);

        sizeX = EditorGUILayout.IntField("Width", sizeX);
        sizeY = EditorGUILayout.IntField("Height", sizeY);

        targetTilemap = (Tilemap)EditorGUILayout.ObjectField("Target Tilemap", targetTilemap, typeof(Tilemap), true);
        tileToDraw = (TileBase)EditorGUILayout.ObjectField("Tile to Draw", tileToDraw, typeof(TileBase), true);

        GUILayout.Space(20);

        if (GUILayout.Button(isDrawing ? "Stop Drawing" : "Start Drawing"))
        {
            ToggleDrawing();
            SceneView.RepaintAll();
        }
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (isDrawing)
        {
            if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
            {
                drawState = DrawState.Drawing;

                Vector3 mouseWorldPosition = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
                Vector3Int cellPosition = targetTilemap.WorldToCell(mouseWorldPosition);

                DrawRectangle(cellPosition);
                e.Use(); // Consume the event to prevent other actions (like selecting objects)
            }
            else if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 1)
            {
                drawState = DrawState.Erasing;

                Vector3 mouseWorldPosition = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
                Vector3Int cellPosition = targetTilemap.WorldToCell(mouseWorldPosition);

                EraseRectangle(cellPosition);
                e.Use(); // Consume the event to prevent other actions (like selecting objects)
            }
            else
            {
                drawState = DrawState.None;
            }

            DrawHighlightedArea(sceneView, Color.white);
        }

        // Check for pressing the "C" key to clear to toggle the drawing
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.C && e.keyCode == KeyCode.LeftControl)
        {
            ToggleDrawing();
            SceneView.RepaintAll();

            // repaint the inspector window
            Repaint();
        }
    }

    private void DrawRectangle(Vector3Int cellPos)
    {
        // Calculate the starting position for the rectangle
        int startX = cellPos.x - sizeX / 2;
        int startY = cellPos.y - sizeY / 2;

        // Loop through the tiles in the rectangle and set them
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Vector3Int tilePos = new Vector3Int(startX + x, startY + y, 0);
                targetTilemap.SetTile(tilePos, tileToDraw);
            }
        }

        // Request repaint to update the scene view
        SceneView.RepaintAll();
    }

    private void EraseRectangle(Vector3Int cellPos)
    {
        // Calculate the starting position for the rectangle
        int startX = cellPos.x - sizeX / 2;
        int startY = cellPos.y - sizeY / 2;

        // Loop through the tiles in the rectangle and erase them
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Vector3Int tilePos = new Vector3Int(startX + x, startY + y, 0);
                targetTilemap.SetTile(tilePos, null); // Set the tile to null to erase
            }
        }

        // Request repaint to update the scene view
        SceneView.RepaintAll();
    }

    private void ToggleDrawing()
    {
        isDrawing = !isDrawing;

        // Restore previous tool when drawing is stopped
        if (!isDrawing)
        {
            Tools.current = Tool.Custom;
        }
    }

    private void DrawHighlightedArea(SceneView sceneView, Color color)
    {
        // Get mouse position
        Event e = Event.current;
        Vector3 mouseWorldPosition = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
        Vector3Int cellPosition = targetTilemap.WorldToCell(mouseWorldPosition);

        // Calculate the starting position for the rectangle
        int startX = cellPosition.x - sizeX / 2;
        int startY = cellPosition.y - sizeY / 2;

        // Draw the highlighted area
        Vector3 topLeft = targetTilemap.CellToWorld(new Vector3Int(startX, startY, 0));
        Vector3 topRight = targetTilemap.CellToWorld(new Vector3Int(startX + sizeX, startY, 0));
        Vector3 bottomLeft = targetTilemap.CellToWorld(new Vector3Int(startX, startY + sizeY, 0));
        Vector3 bottomRight = targetTilemap.CellToWorld(new Vector3Int(startX + sizeX, startY + sizeY, 0));

        Handles.DrawSolidRectangleWithOutline(
            new Vector3[] { topLeft, topRight, bottomRight, bottomLeft },

            Color.clear, // Fill color
            color // Outline color
        );
    }

}
