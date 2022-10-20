//
// Change the FPC walker type (from standard grounded walker to fly mode)
// Pressing the 'f' key you can change betwen them.
//

using UnityEngine;

public class ChangeWalker : MonoBehaviour
{
    [Tooltip("Drag here the standard walker (it can walk, run and jump)")]
    public Transform FPCStandard;
    [Tooltip("Drag here the fly walker (it just fly)")]
    public Transform FPCFly;

    [Header("Runtime variable watcher")]
    [SerializeField]
    private bool flyMode = false; //Are we using FPC fly mode?"

    void Start()
    {
        flyMode = false;    // Always start using the Standard Walker (you are grounded and can walk/run/jump).
        FPCStandard.position = FPCFly.position; // Both walkers start in the same 3d position in the scene.

        // Make sure the fly walker root is active and ready to be used.
        if (!FPCFly.root.gameObject.activeSelf)
            FPCFly.root.gameObject.SetActive (true);

        UpdateWalkers();    // Enable the Standard walker and disbale the fly walker.
    }

    // Check if 'y' is pressed. If so, change betwen both walkers.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            flyMode = !flyMode;

            if (flyMode)
            {
                FPCFly.position = FPCStandard.position;
            }
            else
            {
                FPCStandard.position = FPCFly.position;
            }

            UpdateWalkers();
        }
    }

    // Enable/disable walkers
    private void UpdateWalkers()
    {
        FPCStandard.gameObject.SetActive(!flyMode);
        FPCFly.gameObject.SetActive(flyMode);
    }
}