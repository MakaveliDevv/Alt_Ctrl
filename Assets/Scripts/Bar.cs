using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] private Gradient gradient;
    [SerializeField] private bool useGradient;
    private GameObject obj;
    public Slider slider;
    public Image fill;

    void Awake() 
    {
        obj = gameObject;
        slider = obj.GetComponent<Slider>();
    }

    public void SetMaxValue(float health) 
    {
        slider.maxValue = health;
        slider.value = health;
        if(useGradient) fill.color = gradient.Evaluate(1f);
    } 

    public void SetValue(float health) 
    {
        slider.value = health;
        if(useGradient) fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
