using UnityEngine;

[AddComponentMenu("Sensor/FOV Collider")]
[RequireComponent(typeof(MeshCollider))]
[ExecuteInEditMode]
public class FOVCollider : MonoBehaviour 
{
    #region Fields

    [Tooltip("The length of the field of view cone in world units.")]
    public float length = 5f;

    [Tooltip("The size of the field of view cones base in world units.")]
    public float size = 0.5f;

    [Range(1f, 180f), Tooltip("The arc angle of the fov cone.")]
    public float fovAngle = 90f;

    [Range(1f, 180f), Tooltip("The elevation angle of the cone.")]
    public float elevationAngle = 90f;

    [Range(0, 8), Tooltip("The number of vertices used to approximate the arc of the fov cone. Ideally this should be as low as possible.")]
    public int resolution = 0;

    // Returns the generated collider mesh so that it can be rendered.
    public Mesh Mesh => m_mesh;

    private Mesh m_mesh;
    private MeshCollider m_collider;
    private Vector3[] m_points;
    private int[] m_triangles;

    #endregion

    #region Methods

    private void Awake()
    {
        m_collider = GetComponent<MeshCollider>();
        CreateCollider();
    }

    private void OnValidate()
    {
        length = Mathf.Max(0f, length);
        size = Mathf.Max(0f, size);
        if (m_collider != null) 
        {
            CreateCollider();
        }
    }

    public void CreateCollider()
    {
        m_points = new Vector3[4 + (2 + resolution) * (2 + resolution)];

        // There are 2 triangles on the base
        var baseTriangleIndices = 2 * 3;

        // The arc is (Resolution+2) vertices to each side, making (Resolution+1)*(Resolution+1) boxes of 2 tris each
        var arcTriangleIndices = (resolution + 1) * (resolution + 1) * 2 * 3;

        // There are 4 sides to the cone, and each side has Resolution+2 triangles
        var sideTriangleIndices = (resolution + 2) * 3;
        m_triangles = new int[baseTriangleIndices + arcTriangleIndices + sideTriangleIndices * 4];

        // Base points
        m_points[0] = new Vector3(-size / 2f, -size / 2f, 0f); // Bottom Left
        m_points[1] = new Vector3(size / 2f, -size / 2f, 0f);  // Bottom Right
        m_points[2] = new Vector3(size / 2f, size / 2f, 0f);   // Top Right
        m_points[3] = new Vector3(-size / 2f, size / 2f, 0f);  // Top Left
        m_triangles[0] = 2; m_triangles[1] = 1; m_triangles[2] = 0; m_triangles[3] = 3; m_triangles[4] = 2; m_triangles[5] = 0;

        for (int y = 0; y < 2 + resolution; y++)
        {
            for (int x = 0; x < 2 + resolution; x++)
            {
                int i = 4 + y * (2 + resolution) + x;
                float ay = Mathf.Lerp(-fovAngle / 2f, fovAngle / 2f, x / (float)(resolution + 1));
                float ax = Mathf.Lerp(-elevationAngle / 2f, elevationAngle / 2f, y / (float)(resolution + 1));
                Vector3 p = Quaternion.Euler(ax, ay, 0f) * Vector3.forward * length;
                m_points[i] = p;

                if (x < (1 + resolution) && y < (1 + resolution))
                {
                    var ti = baseTriangleIndices + (y * (resolution + 1) + x) * 3 * 2;
                    m_triangles[ti] = i + 1 + (2 + resolution); // top right
                    m_triangles[ti + 1] = i + 1; // bottom right
                    m_triangles[ti + 2] = i; // bottom left
                    m_triangles[ti + 3] = i + (2 + resolution); // top left
                    m_triangles[ti + 4] = i + (2 + resolution) + 1; // top right
                    m_triangles[ti + 5] = i; // bottom left
                }
            }
        }

        // Top and bottom side triangles
        for (int x = 0; x < 2 + resolution; x++)
        {
            var iTop = 4 + x;
            var iBottom = 4 + (1 + resolution) * (2 + resolution) + x;

            var tiTop = baseTriangleIndices + arcTriangleIndices + x*3;
            var tiBottom = tiTop + sideTriangleIndices;
            if (x == 0)
            {
                m_triangles[tiTop] = 2;
                m_triangles[tiTop+1] = 3;
                m_triangles[tiTop + 2] = iTop;

                m_triangles[tiBottom] = 0;
                m_triangles[tiBottom + 1] = 1;
                m_triangles[tiBottom + 2] = iBottom;
            }
            else
            {
                m_triangles[tiTop] = iTop;
                m_triangles[tiTop + 1] = 2;
                m_triangles[tiTop + 2] = iTop-1;

                m_triangles[tiBottom] = 1;
                m_triangles[tiBottom + 1] = iBottom;
                m_triangles[tiBottom + 2] = iBottom-1;
            }
        }

        // Left and right side triangles
        var yIncr = 2 + resolution;
        for (int y = 0; y < 2 + resolution; y++)
        {
            var iLeft = 4 + y * (2 + resolution);
            var iRight = iLeft + (1 + resolution);

            var tiLeft = baseTriangleIndices + arcTriangleIndices + sideTriangleIndices * 2 + y * 3;
            var tiRight = tiLeft + sideTriangleIndices;
            if (y == 0)
            {
                m_triangles[tiLeft] = 3;
                m_triangles[tiLeft + 1] = 0;
                m_triangles[tiLeft + 2] = iLeft;

                m_triangles[tiRight] = 1;
                m_triangles[tiRight + 1] = 2;
                m_triangles[tiRight + 2] = iRight;
            }
            else
            {
                m_triangles[tiLeft] = 0;
                m_triangles[tiLeft + 1] = iLeft;
                m_triangles[tiLeft + 2] = iLeft - yIncr;

                m_triangles[tiRight] = iRight;
                m_triangles[tiRight + 1] = 1;
                m_triangles[tiRight + 2] = iRight - yIncr;
            }
        }

        ReleaseMesh();

        m_mesh = new Mesh();
        m_mesh.vertices = m_points;
        m_mesh.triangles = m_triangles;
        m_mesh.name = "ColliderPoints";

        m_collider.sharedMesh = m_mesh;
        m_collider.convex = true;
        m_collider.isTrigger = true;
    }

    private void ReleaseMesh()
    {
        if (m_collider.sharedMesh != null && m_collider.sharedMesh == m_mesh)
        {
            DestroyImmediate(m_collider.sharedMesh, true);
        }
    }

    #endregion
}