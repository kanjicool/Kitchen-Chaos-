using UnityEngine;
using UnityEngine.UI;
public class ProcessBar : MonoBehaviour
{
    [SerializeField] private CuttingCounter cuttingCounter;
    [SerializeField] private Image barImage;

    private void Start()
    {
        barImage.fillAmount = 0f;
    }

    public void CuttingCounter_OnProcessChanged(float progress) 
    {
        barImage.fillAmount = progress;
    }

}
