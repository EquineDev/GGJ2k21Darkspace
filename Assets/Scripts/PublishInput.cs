using UnityEngine;
public class PublishInput : MonoBehaviour
{
    public delegate void EmitStringCommand (string command); 
    public static event EmitStringCommand EmitPublisher;

    public void EmitString(string command)
    {
        if (EmitPublisher != null)
        {
            EmitPublisher(command);
        }
    }
}