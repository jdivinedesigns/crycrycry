using UnityEngine;

public class ToggleActiveAction : MonoBehaviour
{
    public GameObject target;
    public void Activate() { if (target) target.SetActive(true); }
    public void Deactivate() { if (target) target.SetActive(false); }
}
