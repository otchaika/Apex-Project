using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;

public class PaintArea : MonoBehaviour
{
    public XRController leftController;
    public XRController rightController;
    public GameObject leftFinger;
    public GameObject rightFinger;
    public Texture2D maskTexture;
    public Color paintColor = Color.red;
    public int brushSize = 5;
    public Material mat;
    public PaintingManager manager;
    public Animator anim;
    [SerializeField] private InputActionReference leftTriggerRef;
    [SerializeField] private InputActionReference rightTriggerRef;
    private Color curColor;
    private Renderer meshRenderer;
    public Renderer skinRenderer;

    private void Awake()
    {
        manager.animatePaintings += StartAnimating;
        // Initialize the mask texture
        maskTexture = CreateMaskTexture(1024, 1024);
        meshRenderer = GetComponent<Renderer>();
    }
    private void StartAnimating()
    {
        if (anim != null)
        {
            skinRenderer.enabled = true;
            meshRenderer.enabled = false;
            anim.SetBool("isAnimating", true);
        }
        
    }
    private Texture2D CreateMaskTexture(int width, int height)
    {
        Texture2D maskTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color(0, 0, 0, 0);
        maskTexture.SetPixels(pixels);
        maskTexture.Apply();
        return maskTexture;
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if the other object is the controller
        if (other.gameObject.CompareTag("PaintingFinger") && other.gameObject.GetComponent<Finger>())
        {
            Debug.Log("finger is painting");
            HandlePaintingOnCollision(other);
            UpdateMaterial();
        }
        //Check if the other object is the brush
        else if (other.gameObject.CompareTag("PaintingFinger") && other.transform.parent.name == "brush_hair")
        {
            Debug.Log(other.transform.parent.name);
            Debug.Log("brush_hair is interacting");
            HandleBrushHairCollision(other);
            UpdateMaterial();
        }
    }
    float strength = 0f;
    private void HandlePaintingOnCollision(Collider controllerCollider)
    {
        // Get the InputActionReference for the corresponding controller
        InputActionReference triggerRef = (controllerCollider.gameObject == leftFinger)
            ? leftTriggerRef
            : rightTriggerRef
            ;
        Debug.Log(triggerRef.ToString());
        // Check if the trigger is pressed
        if (triggerRef.action.ReadValue<float>() > 0.5f)
        {
            if (triggerRef == leftTriggerRef)
            {
                curColor = manager.curColorLeft;
            }
                if (triggerRef == rightTriggerRef)
                {
                    curColor = manager.curColorRight;
                }
            strength = triggerRef.action.ReadValue<float>();
            // Get the collision contact point
            Vector3 contactPoint = controllerCollider.ClosestPointOnBounds(transform.position);
            PaintOnTexture(contactPoint);
        }
    }
    //Brush collision
    private void HandleBrushHairCollision(Collider brushCollider)
    {
        strength = 1f; 
        curColor = manager.curColorBrush;
        Vector3 contactPoint = brushCollider.ClosestPointOnBounds(transform.position);
        PaintOnTexture(contactPoint);
    }

    private void PaintOnTexture(Vector3 hitPoint)
    {
        // Convert world point to UV coordinates
        Vector2 uv = GetUVCoordinates(hitPoint);

        // Convert UV to texture coordinates
        int textureX = Mathf.FloorToInt(uv.x * maskTexture.width);
        int textureY = Mathf.FloorToInt(uv.y * maskTexture.height);

        // Paint with a circular brush
        PaintAtTexturePoint(textureX, textureY);
    }

    private void PaintAtTexturePoint(int x, int y)
    {
        int baseBrush = brushSize;
        brushSize = (int)(brushSize * strength * strength);
        for (int dx = -brushSize; dx <= brushSize; dx++)
        {
            for (int dy = -brushSize; dy <= brushSize; dy++)
            {
                int pixelX = Mathf.Clamp(x + dx, 0, maskTexture.width - 1);
                int pixelY = Mathf.Clamp(y + dy, 0, maskTexture.height - 1);

                float distance = Vector2.Distance(Vector2.zero, new Vector2(dx, dy));
                if (distance <= brushSize)
                {
                    float falloff = Mathf.Clamp01(1.0f - (distance / brushSize));
                    Color existingColor = maskTexture.GetPixel(pixelX, pixelY);
                    paintColor = curColor;

                    Color blendedColor = Color.Lerp(existingColor, paintColor, falloff * paintColor.a);
                    maskTexture.SetPixel(pixelX, pixelY, blendedColor);
                }
            }
        }
        brushSize = baseBrush;

        maskTexture.Apply();
    }

    private Vector2 GetUVCoordinates(Vector3 hitPoint)
    {
        Mesh mesh = meshRenderer.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uv = mesh.uv;

        Vector3 closestPoint = Vector3.zero;
        float closestDistance = float.MaxValue;
        int closestTriangleIndex = -1;

        for (int i = 0; i < vertices.Length - 2; i += 3)
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

        return uv0;
    }

    private float PointToTriangleDistance(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(point, v0); // Placeholder
    }

    private void UpdateMaterial()
    {
        mat.SetTexture("_MaskTex", maskTexture);
    }
}
