using UnityEngine;

public class animationEvents : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public void StartLanding()
    {
        playerController.OnLandStarted();
    }

    public void EndLanding()
    {
        playerController.OnLandEnded();
    }
}