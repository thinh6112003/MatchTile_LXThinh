using UnityEngine;

public class Btn : MonoBehaviour
{
    [SerializeField] private GameObject onBg;
    [SerializeField] private GameObject offSwitch;
    [SerializeField] private GameObject offTextImage;
    [SerializeField] private GameObject onSwitch;
    [SerializeField] private GameObject onTextImage;
    internal bool status = false;
    private void Start()
    {
        status = onSwitch.active;
    }
    public void changeStatus()
    {
        status = !status;
        onBg.SetActive(status);
        onSwitch.SetActive(status);
        onTextImage.SetActive(status);
        offSwitch.SetActive(!status);
        offTextImage.SetActive(!status);
    }
}
