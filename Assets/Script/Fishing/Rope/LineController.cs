using UnityEngine;

public class LineController : MonoBehaviour
{
    [Header("References")]
    public Transform rodTip;      // Olta ucu transformu
    public Transform bait;        // Yem transformu
    
    [Header("Settings")]
    public float lineThickness = 0.02f;  // İp kalınlığı
    public Color lineColor = new Color(1, 1, 1, 0.8f); // İp rengi
    public int segmentCount = 5;         // Eğrilik için segment sayısı
    
    [Header("Physics")]
    public float tension = 2f;            // Gerilim (sarkma azaltır)
    public float gravityEffect = 0.5f;    // Yerçekimi etkisi (sarkma artırır)
    
    private LineRenderer lineRenderer;
    private Vector3[] points;

    void Start()
    {
        // LineRenderer bileşenini ekle ve ayarla
        lineRenderer = GetComponent<LineRenderer>();
        
        lineRenderer.startWidth = lineThickness;
        lineRenderer.endWidth = lineThickness;
        lineRenderer.loop = false;
        // trailing özelliği kaldırıldı - eski Unity sürümlerinde yok
        
        // Gradient ayarla (düzeltilmiş)
        SetLineGradient();
        
        // Nokta dizisini oluştur
        points = new Vector3[segmentCount + 2];
    }

    void SetLineGradient()
    {
        // Gradient'i düzgün şekilde ayarla
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(lineColor, 0f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1f, 0f) 
            }
        );
        lineRenderer.colorGradient = gradient;
    }

    void Update()
    {
        if (rodTip == null || bait == null) return;
        
        CalculateLinePoints();
        UpdateLineRenderer();
    }

    void CalculateLinePoints()
    {
        points[0] = rodTip.position;  // Başlangıç: Olta ucu
        points[segmentCount + 1] = bait.position;  // Bitiş: Yem
        
        // Aradaki noktaları hesapla (eğri/sarkma efekti)
        for (int i = 1; i <= segmentCount; i++)
        {
            float t = i / (float)(segmentCount + 1);
            
            // Doğrusal interpolasyon
            Vector3 linearPoint = Vector3.Lerp(rodTip.position, bait.position, t);
            
            // Sarkma efekti için aşağı doğru kaydır
            float sagAmount = tension * Mathf.Sin(t * Mathf.PI) - gravityEffect;
            points[i] = linearPoint + (Vector3.down * sagAmount);
        }
    }

    void UpdateLineRenderer()
    {
        lineRenderer.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }

    // İp kalınlığını dinamik olarak değiştirme
    public void SetThickness(float thickness)
    {
        lineThickness = thickness;
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = thickness;
            lineRenderer.endWidth = thickness;
        }
    }

    // İp rengini dinamik olarak değiştirme
    public void SetColor(Color color)
    {
        lineColor = color;
        if (lineRenderer != null)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(color, 0f) 
                },
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(1f, 0f) 
                }
            );
            lineRenderer.colorGradient = gradient;
        }
    }

    // İpi gizle/göster
    public void SetVisible(bool visible)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = visible;
        }
    }
}
