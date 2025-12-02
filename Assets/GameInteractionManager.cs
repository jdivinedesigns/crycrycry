using UnityEngine;

public class GameInteractionManager : MonoBehaviour
{
    public static GameInteractionManager instance;

    [Header("Interaction States")]
    public bool lightInteractionActive = false;
    public bool sittingInteractionActive = false;
    public bool phonePickedUp = false;

    void Awake()
    {
        // simple singleton
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // returns true if ANY main interaction is happening
    public bool IsBusy()
    {
        return lightInteractionActive || sittingInteractionActive || phonePickedUp;
    }
}
