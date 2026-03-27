using UnityEngine;

public class ComputerOpener : MonoBehaviour
{
    public ComputerManager computer;

    public void OpenComputer()
    {
        computer.Show();
    }
}