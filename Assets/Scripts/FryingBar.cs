using UnityEngine;
using UnityEngine.UI;
public class FryingBar : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private Image barImage;

    private void Start()
    {
        barImage.fillAmount = 0f;
    }

    public void FryingCounter_OnProcessChanged(float progress)
    {
        barImage.fillAmount = progress;
    }

}
