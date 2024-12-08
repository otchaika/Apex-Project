using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PaintArea : MonoBehaviour
{
    public XRController leftController;   // Left controller reference
    public XRController rightController;  // Right controller reference
    public Texture2D maskTexture;         // Mask texture to paint on
    public Color paintColor = Color.red;  // Default paint color (red)
    public int brushSize = 10;          // Default brush size
    public Material mat;
    private XRRayInteractor leftRayInteractor;   // Left controller ray interactor
    private XRRayInteractor rightRayInteractor;  // Right controller ray interactor
    private Renderer meshRenderer;
    public PaintingManager manager;
    private void Awake()
    {

        // Initialize the ray interactors from controllers
        leftRayInteractor = leftController.GetComponent<XRRayInteractor>();
        rightRayInteractor = rightController.GetComponent<XRRayInteractor>();

        // Get the mesh renderer of the object you're painting on
        meshRenderer = GetComponent<Renderer>();
        maskTexture = CreateMaskTexture(1024, 1024);

    }
    Texture2D CreateMaskTexture(int width, int height)
    {
        Texture2D maskTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color(0, 0, 0, 0);
            
        }
        maskTexture.SetPixels(pixels);
        maskTexture.Apply();
        return maskTexture;
    }

    [SerializeField] private InputActionReference leftRef;
    [SerializeField] private InputActionReference rightRef;
    private void Update()
    {
            // Try to get the hit information for each controller
            if (leftRef.action.ReadValue<float>() > 0.5f)
            {
                HandlePainting(leftController);
            }
            else if (rightRef.action.ReadValue<float>() > 0.5f)
            {
                HandlePainting(rightController);
            }

        UpdateMaterial();
    }

    private void HandlePainting(XRBaseController controller)
    {
        Transform controllerTransform = controller.transform;
        if (controllerTransform != null)
        {
            Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    PaintOnTexture(hit.point);
                    
                }
            }
        }

        paintColor = manager.curColor;
    }
    

    private void PaintOnTexture(Vector3 hitPoint)
    {
        // Convert world hit point to UV coordinates
        Vector2 uv = GetUVCoordinates(hitPoint);

        // Convert UV coordinates to texture coordinates
        int textureX = Mathf.FloorToInt(uv.x * maskTexture.width);
        int textureY = Mathf.FloorToInt(uv.y * maskTexture.height);

        // Paint on the texture with the brush size
        for (int x = -brushSize; x <= brushSize; x++)
        {
            for (int y = -brushSize; y <= brushSize; y++)
            {
                int pixelX = Mathf.Clamp(textureX + x, 0, maskTexture.width - 1);
                int pixelY = Mathf.Clamp(textureY + y, 0, maskTexture.height - 1);

                // Apply circular brush size effect
                if (Vector2.Distance(Vector2.zero, new Vector2(x, y)) <= brushSize)
                {
                    maskTexture.SetPixel(pixelX, pixelY, paintColor);
                }
            }
        }

        // Apply changes to the mask texture
        maskTexture.Apply();
    }

    private Vector2 GetUVCoordinates(Vector3 hitPoint)
    {
        // Get mesh UV coordinates based on hit point
        Mesh mesh = meshRenderer.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uv = mesh.uv;
        //Vector3[] normals = mesh.normals;

        Vector3 closestPoint = Vector3.zero;
        float closestDistance = float.MaxValue;
        int closestTriangleIndex = -1;

        // Iterate through the triangles in the mesh
        for (int i = 0; i < vertices.Length-2; i += 3)
        {
            Vector3 v0 = transform.TransformPoint(vertices[i]);
            Vector3 v1 = transform.TransformPoint(vertices[i + 1]);
            Vector3 v2 = transform.TransformPoint(vertices[i + 2]);

            float distance = PointToTriangleDistance(hitPoint, v0, v1, v2);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTriangleIndex = i;
                closestPoint = hitPoint;
            }
        }

        Vector2 uv0 = uv[closestTriangleIndex];
        Vector2 uv1 = uv[closestTriangleIndex + 1];
        Vector2 uv2 = uv[closestTriangleIndex + 2];

        // Interpolate UV coordinates for precision
        return uv0;
    }

    private float PointToTriangleDistance(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(point, v0);  // Placeholder distance (could be improved)
    }

    // Function to change the paint color
    public void ChangePaintColor(Color newColor)
    {
        paintColor = newColor;
    }

    // Function to change the brush size
    public void ChangeBrushSize(int newSize)
    {
        brushSize = newSize;
    }

    void UpdateMaterial()
    {
        mat.SetTexture("_MaskTex", maskTexture);
    }
}
