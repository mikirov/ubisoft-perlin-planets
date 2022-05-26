using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NFTController))]
public class NFTControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Update Metadata"))
        {
            NFTController.Instance.UpdateMetadata();
        }

        if (GUILayout.Button("Generate NFTs"))
        {
            NFTController.Instance.GenerateNFTs();
        }
        if (GUILayout.Button("Generate Planet"))
        {
            //TODO: get seed:
            NFTController.Instance.GeneratePlanet(880713271);
        }
    }
}
