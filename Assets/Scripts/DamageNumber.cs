using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DamageNumber : MonoBehaviour
{
    public int Number
    {
        get
        {
            return m_Number;
        }

        set
        {
            m_Number = value;

            #if UNITY_EDITOR

            if (Application.isPlaying)
            {
                CreateNumber();
            }

            #else
            
            CreateNumber();

            #endif
        }
    }

    int m_Number = 1;

    [Header("Settings")]
    [SerializeField, Tooltip("Amount that digits overlap eachother")] int m_SpriteOverlap = 38;
    [SerializeField, Tooltip("Variance in height between digits")] int m_HeightVariance = 25;
    [SerializeField] float m_Duration = 1;
    [SerializeField] float m_UpSpeed = 50;
    [SerializeField, Range(0, 1)] float m_FadeStart = 0.5f;

    [SerializeField] float m_FirstNumberHeightDifference = 20;
    [SerializeField] float m_FirstNumberScale = 1.15f;
    [SerializeField] int m_FirstNumberSpriteOverlap = 60;


    [Header("Sprites")]
    [SerializeField] Sprite[] m_Sprites = new Sprite[10];

    [Header("References")]
    [SerializeField] RectTransform m_RectTransform = null;
    [SerializeField] Image m_Image = null;
    [SerializeField] Image m_ImagePrefab = null;
    [SerializeField] RenderTexture m_RenderTexture = null;
    [SerializeField] DamageNumberRenderer m_RendererPrefab = null;

    static List<Image> m_ImagePool = new List<Image>();

    static DamageNumberRenderer m_Renderer;

    float m_CurrentDuration;
    float m_CurrentYOffset;

    public Vector3 WorldPosition { get; private set; }

    public float StackOffset { get; private set; }

    void Reset()
    {
        m_RectTransform = transform as RectTransform;
        m_Image = GetComponent<Image>();
    }

    void Start()
    {
        StartCoroutine(TimedLife());
    }

    IEnumerator TimedLife()
    {
        m_CurrentDuration = m_Duration;
        m_CurrentYOffset = 0;

        while (m_CurrentDuration > 0)
        {
            m_CurrentDuration -= Time.deltaTime;

            m_CurrentYOffset += m_UpSpeed * Time.deltaTime;

            m_RectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(WorldPosition) + new Vector3(0, m_CurrentYOffset + StackOffset, 0);

            if (m_CurrentDuration / m_Duration <= m_FadeStart)
            {
                m_Image.color = new Color
                (
                    m_Image.color.r, m_Image.color.g, m_Image.color.b, (m_CurrentDuration / m_Duration) / m_FadeStart);
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    public void CreateNumber()
    {
        if (m_Renderer == null)
        {
            m_Renderer = Instantiate(m_RendererPrefab);
        }

        if (!Application.isPlaying ||
            m_Renderer.Camera == null ||
            m_Renderer.ImageParent == null)
        {
            return;
        }

        int[] digits = DigitsIn(m_Number);
        
        if (digits.Length == 0)
        {
            m_Image.sprite = null;
            return;
        }

        while (m_ImagePool.Count < digits.Length)
        {
            m_ImagePool.Add(Instantiate(m_ImagePrefab, m_Renderer.ImageParent));
        }

        float currentX = 0;

        for (int i = digits.Length; i < m_ImagePool.Count; i++)
        {
            m_ImagePool[i].gameObject.SetActive(false);
        }

        m_ImagePool[0].gameObject.SetActive(true);

        m_ImagePool[0].sprite = m_Sprites[digits[0]];
        m_ImagePool[0].rectTransform.anchoredPosition = new Vector2
        (
            currentX,
            0
        );

        currentX += m_Sprites[digits[0]].rect.width * m_FirstNumberScale - m_FirstNumberSpriteOverlap;

        m_ImagePool[0].rectTransform.localScale = new Vector3(m_FirstNumberScale, m_FirstNumberScale, m_FirstNumberScale);

        bool heightOffsetToggle = true;

        for (int i = 1; i < digits.Length; i++)
        {
            m_ImagePool[i].gameObject.SetActive(true);

            m_ImagePool[i].sprite = m_Sprites[digits[i]];
            m_ImagePool[i].rectTransform.anchoredPosition = new Vector2
            (
                currentX,
                (heightOffsetToggle ? -m_HeightVariance : 0) - m_FirstNumberHeightDifference
            );

            currentX += m_Sprites[digits[i]].rect.width - m_SpriteOverlap;
            heightOffsetToggle = !heightOffsetToggle;
        }

        float textureWidth = currentX + m_SpriteOverlap + 4;

        float textureHeight = m_Sprites[digits[0]].texture.height * m_FirstNumberScale;

        for (int i = 1; i < digits.Length; i++)
        {
            float height = m_Sprites[digits[i]].texture.height + m_FirstNumberHeightDifference + m_HeightVariance;

            if (height > textureHeight)
            {
                textureHeight = height;
            }
        }

        textureHeight += m_HeightVariance;

        RenderTexture.active = m_RenderTexture;

        m_Renderer.Camera.Render();

        Texture2D texture = new Texture2D((int)textureWidth, (int)textureHeight);

        texture.ReadPixels(new Rect
        (
            0,
            0,
            textureWidth,
            textureHeight
        ), 0, 0);

        texture.Apply();

        m_Image.sprite = Sprite.Create(texture, new Rect(0, 0, (int)textureWidth, (int)textureHeight), new Vector2(0, 0), 100);
    }

    int[] DigitsIn(int value)
    {
        if (value == 0)
        {
            return new int[0];
        }

        List<int> digits = new List<int>(DigitsIn(value / 10));

        digits.Add(value % 10);

        return digits.ToArray();
    }
}
