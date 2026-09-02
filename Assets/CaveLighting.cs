using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class CaveLighting : MonoBehaviour
{
    [Header("Player")]
    private Transform player; // Stores a reference to the player so lighting can follow them
    private PlayerInventory playerInventory; // Stores reference to the inventory (script) to know if battery was picked up or not

    [HideInInspector]
    public bool xRayActive = false; // Controls if X ray is showing cave or not

    [Header("Darkness")]
    [Range(0f, 255f)]
    public float darknessAlpha = 230f; // Controls how dark the screen is outside the torch radius

    [Header("Light")]
    public float innerRadius = 1.6f; // Keeps the area closest to the Player at full brightness
    public float lightRadius = 5f; //Sets the normal maximum distance the Player can see 
    public float batteryLightRadius = 7f; // Increases radius of light if battery is picked up

    [Header("Raycasting")]
    public int rayCount = 360; // Uses rays around the player to make a smooth boundary
    public LayerMask wallLayer; // Picks which objects count as walls

    [Header("Texture")]
    public int textureHeight = 144;

    private RawImage darknessImage; // Stores the CaveDarkness UI that shows the generated darkness

    private Texture2D darknessTexture; // Stores the texture that is continuously updated to create the lighting effect

    private Color32[] pixels;// Stores the colour and transparency of each pixel in the darkness texture

    private Camera cam; // Stores a reference to the Main Camera so world positions can be converted to screen positions

    private int textureWidth; // Stores the width of the generated texture based on the screen's aspect ratio

    private int lastScreenWidth; // Stores the previous screen width so the texture can be recreated if the screen size changes

    private int lastScreenHeight; // Stores the previous screen height so the texture can be recreated if the screen size changes

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");


        if (playerObject != null)
        {
            player = playerObject.transform; // Gets the Player's Transform so its position can be followed

            playerInventory = playerObject.GetComponent<PlayerInventory>(); //Getting inventory to check for battery
        }

        cam = Camera.main; // Gets main camera to calculate screen positios

        darknessImage = FindDarknessImage(); // Finds the UI object used to display the Cave darkness

        CreateTexture(); // Creates the initial darkness texture
    }

    void LateUpdate()
    {
        if (player == null) // Checks whether the player reference is missing
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); // Searches for the player again

            if (playerObject != null) // Checks if Player is found, and sets positions and Inventory
            {
                player = playerObject.transform;

                playerInventory = playerObject.GetComponent<PlayerInventory>();
            }

            return;
        }

        if (playerInventory == null) // Checks whether the inventory reference is missing and sets it
        {
            playerInventory = player.GetComponent<PlayerInventory>();
        }

        if (cam == null) // Camera reference and sets it
        {
            cam = Camera.main; 
        }

        if (darknessImage == null) // darkness UI reference sets it
        {
            darknessImage = FindDarknessImage();
        }

        if (darknessImage == null || cam == null) // Stops the lighting calculations if an important variable is missing
        {
            return;
        }

        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight) // Checks whether the screen size has changed
        {
            CreateTexture(); // Recreates the texture so it still fits the screen correctly
        }

        UpdateDarkness(); // Recalculates the visible area around the Player
    }

    RawImage FindDarknessImage()
    {
        GameObject darknessObject = GameObject.Find("CaveDarkness"); // Finds the UI object used as the darkness overlay

        if (darknessObject == null) // Checks whether the darkness object could not be found
        {
            return null;
        }

        return darknessObject.GetComponent<RawImage>(); // Gets the RawImage component so the generated texture can be displayed
    }

    void CreateTexture()
    {
        lastScreenWidth = Screen.width; // Records the current screen width
        lastScreenHeight = Screen.height;//and height

        float aspectRatio = Screen.width / (float)Screen.height; // Calculates the screen's width-to-height ratio

        textureWidth = Mathf.RoundToInt(textureHeight * aspectRatio); // Creates a texture with the same aspect ratio as the screen

        if (darknessTexture != null)
        {
            Destroy(darknessTexture); // Gets rid of the previous texture before a new one takes over
        }

        darknessTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false); // Makes the texture used for the Cave darkness

        darknessTexture.filterMode = FilterMode.Bilinear; // Smooths the texture as it goes out so it doesnt look chunky/pixelated

        darknessTexture.wrapMode = TextureWrapMode.Clamp; // Stops the texture from repeating outside its boundaries

        pixels = new Color32[textureWidth * textureHeight]; // Creates space to store the colour and transparency of every pixel

        darknessImage.texture = darknessTexture; // Gives the generated texture to the CaveDarkness UI

        darknessImage.color = Color.white; // Keeps the UI colour neutral so the texture controls the darkness
    }

    void UpdateDarkness()
    {
        if (xRayActive) // Checks whether the X-ray goggles are currently being used
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color32(0, 0, 0, 0); // Makes every pixel transparent so the entire Cave becomes visible
            }

            darknessTexture.SetPixels32(pixels); // Applies the transparent pixels to the darkness texture (essentially making it gone)

            darknessTexture.Apply(false, false); // Updates the texture shown on screen


            return; // Skips all lighting calculations while Xray is active
        }

        float currentLightRadius = lightRadius; // Starts with the normal visibility range

        if (playerInventory != null && playerInventory.hasBattery) // Checks whether the Player has collected the Battery
        {
            currentLightRadius = batteryLightRadius; // Increases the visibility range when the Battery has been collected
        }

        byte darkAlpha = (byte)Mathf.Clamp(darknessAlpha, 0f, 255f); // Keeps the darkness value within the valid alpha range

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color32(0, 0, 0, darkAlpha); // Starts by making the entire Cave dark
        }

        Vector3 playerScreen = cam.WorldToScreenPoint(player.position); // Converts the Player's world position into a screen position

        Vector2 center = ScreenToTexture(playerScreen); // Converts the Player's screen position into the generated texture's coordinates

        Vector2[] rayPoints = new Vector2[rayCount]; // Stores the end position of every visibility ray

        for (int i = 0; i < rayCount; i++)
        {
            float angle = (360f / rayCount) * i; // Spreads the rays evenly around the Player

            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad),Mathf.Sin(angle * Mathf.Deg2Rad)); // Converts the angle into a 2D direction

            RaycastHit2D hit = Physics2D.Raycast(player.position, direction, currentLightRadius, wallLayer); // Checks whether a wall blocks the Player's view in this direction

            Vector2 worldPoint;

            if (hit.collider != null) // Checks whether the ray hit a wall
            {
                worldPoint = hit.point; // Stops the visible area at the wall so light cannot pass through it
            }
            else
            {
                worldPoint =(Vector2)player.position + direction * currentLightRadius; // Extends the visible area to the maximum light range when no wall is hit
            }

            Vector3 screenPoint = cam.WorldToScreenPoint(worldPoint); // Converts the visibility boundary from world space to screen space

            rayPoints[i] = ScreenToTexture(screenPoint); // Stores the boundary point in texture coordinates
        }

        for (int i = 0; i < rayCount; i++)
        {
            int next = (i + 1) % rayCount; // Gets the next ray and wraps back to the first ray at the end


            DrawTriangle(center, rayPoints[i], rayPoints[next], currentLightRadius); // Builds the visible area from the Player to two neighbouring ray points
        }

        darknessTexture.SetPixels32(pixels); // Applies the newly calculated darkness values to the texture

        darknessTexture.Apply(false, false); // Updates the texture shown on screen
    }

    Vector2 ScreenToTexture(Vector3 screenPoint)
    {
        float x = screenPoint.x /  Screen.width * textureWidth; // Converts the screen X coordinate into a texture X coordinate

        float y = screenPoint.y / Screen.height * textureHeight; // Converts the screen Y coordinate into a texture Y coordinate

        return new Vector2(x, y); // Returns the corresponding position on the generated texture
    
}

    void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, float currentLightRadius)
    {
        int minX = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(a.x, Mathf.Min(b.x, c.x))), 0, textureWidth - 1); // Finds the leftmost pixel that could be inside the triangle

        int maxX = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(a.x, Mathf.Max(b.x, c.x))), 0, textureWidth - 1); // Rightmost

        int minY = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(a.y, Mathf.Min(b.y, c.y))), 0, textureHeight - 1); // Lowest

        int maxY = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(a.y, Mathf.Max(b.y, c.y))), 0, textureHeight - 1); // Highest

        float worldPerPixel = (cam.orthographicSize * 2f) / Screen.height; // Calculates how much world space each screen pixel represents

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                Vector2 point = new Vector2(x + 0.5f, y + 0.5f); // Uses the centre of the current pixel for the triangle test

                if (!PointInTriangle(point, a, b, c)) // Checks whether the current pixel belongs to this visibility triangle
                {
                    continue;
                }

                float dx = (point.x - a.x) * (Screen.width / (float)textureWidth); // Converts the pixel's horizontal distance into screen distance

                float dy = (point.y - a.y) * (Screen.height / (float)textureHeight);// Converts the pixel's vertical distance into screen distance

                float distance = Mathf.Sqrt(dx * dx + dy * dy) * worldPerPixel; // Calculates the world-space distance from the Player to the pixel

                float lightAmount;

                if (distance <= innerRadius) // Keeps the area closest to the Player fully illuminated
                {
                    lightAmount = 1f;
                }
                else if (distance >= currentLightRadius) // Makes the area outside the visibility range completely dark
                {
                    lightAmount = 0f;
                }
                else
                {
                    float t = Mathf.InverseLerp(currentLightRadius, innerRadius, distance); // Converts the distance into a value between the outer and inner radius

                    lightAmount = Mathf.SmoothStep(0f, 1f, t); // Smooths the change so the lighting fades gradually instead of changing suddenly
                }

                byte alpha = (byte)Mathf.Clamp(darknessAlpha * (1f - lightAmount), 0f, 255f); // Converts the light amount into the darkness transparency


                int index = y * textureWidth + x; // Finds the position of the current pixel in the pixel array

                if (alpha < pixels[index].a) // Keeps the lighter result when multiple triangles affect the same pixel
                {
                    pixels[index] = new Color32(0, 0, 0, alpha); // Updates the pixel with the calculated darkness
                }
            }
        }
    }

    bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float d1 = Sign(p, a, b); // Checks the point's position relative to the first triangle edge

        float d2 = Sign(p, b, c); // Checks the point's position relative to the second triangle edge

        float d3 = Sign(p, c, a); // Checks the point's position relative to the third triangle edge

        bool hasNegative = d1 < 0f || d2 < 0f || d3 < 0f; // checks if any result is negative

        bool hasPositive = d1 > 0f || d2 > 0f || d3 > 0f; // checks if any result in positive

        return !(hasNegative && hasPositive); // Returns true when the point lies inside the triangle
    }

    float Sign( Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);// Uses the positions of three points to help determine which side of an edge a point lies on
    }
}
