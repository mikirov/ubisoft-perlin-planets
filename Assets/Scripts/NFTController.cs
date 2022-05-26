using UnityEngine;
using System.IO;
using System;
using Random = UnityEngine.Random;

using System.Text;
[Serializable]
class NFTAttribute
{
    public string trait_type;
    public string value;
}

[Serializable]
class NFTFiles
{
    public string uri;
    public string type;
    public bool cdn;
}
[Serializable]
class NFTCreator
{
    public string address;
    public int share;
}

[Serializable]
class NFTProperties
{
    public NFTFiles[] files;
    public string category;
    public NFTCreator[] creators;
}
[Serializable]
class NFTCollection
{
    public string name;
    public string family;
}
[Serializable]
class NFTMetadata
{
    public string name;
    public string description;
    public string image;
    public string symbol;
    public int seller_fee_basis_points;
    public string animation_url;
    public string external_url;
    public NFTAttribute[] attributes;
    public NFTCollection collection;
    public NFTProperties properties;
}

[ExecuteInEditMode]
public class NFTController : MonoBehaviour
{

    private static NFTController _instance;
    public static NFTController Instance { get { return _instance; } }

    public int StartIndex = 1225;
    public int Count = 500;
    public Camera RenderingCamera;

    [SerializeField]
    private Planet Planet;

    [SerializeField]
    private Gradient[] gradients;

    [SerializeField]
    private Material[] SkyboxMaterials;

    private RenderTexture image;

    private int gradientIndex;
    private int gradientMultiplier;
    private int skyIndex;
    private int resourceIndex;
    private static string[] SkyboxTypes = new string[] { "Basic", "Gas", "Poison", "Sea", "Wise", "Calm", "Weird", "Passion", "Danger", "War" };
    private static string[] ResourceTypes = new string[] { "Magma", "Ruby", "Citrine", "Emerald", "Uranium", "Aquamarine", "Green quartz", "Copper", "Sulfur", "Turquoise" };

    private string mintHash;

    public void SetMintHash(string mintHashToSet)
    {
        mintHash = mintHashToSet;
    }

    public string GetMintHash()
    {
        return mintHash;
    }

    private int seed;

    public void SetSeed(int seedToSet)
    {
        seed = seedToSet;
    }

    public int GetSeed() { return seed; }

    private string ownerPublicKey;
    public string GetOwnerPublicKey()
    {
        return ownerPublicKey;
    }

    public void SetOwnerPublicKey(string ownerPublicKeyToSet)
    {
        ownerPublicKey = ownerPublicKeyToSet;
    }

    private string signedMessage;

    public string GetSignedMessage()
    {
        return signedMessage;
    }

    public void SetSignedMessage(string signedMessageToSet)
    {
        signedMessage = signedMessageToSet;
    }

    private string message;

    public void SetMessage(string messageToSet)
    {
        message = messageToSet;
    }

    public string GetMessage()
    {
        return message;
    }

    public float GetMaxPoint()
    {
        return Planet.colourGenerator.GetMaxElevation();
    }

    public void UpdateMetadata()
    {
        for (int i = 0; i < 5555; i++)
        {
            string jsonString = File.ReadAllText(Application.dataPath + "../../../generated-photos/" + i + ".json");
            NFTMetadata metadata = JsonUtility.FromJson<NFTMetadata>(jsonString);
            UpdateJsonMetadata(i, int.Parse(metadata.attributes[0].value));
        }
    }

    public void GenerateNFTs()
    {
        for (int i = StartIndex; i < StartIndex + Count; i++)
        {
            Random.InitState(Random.Range(int.MinValue, int.MaxValue));
            int seed = Random.seed;

            GeneratePlanet(seed);
            //ExportGLTF(i);
            CaptureScreenShot(i);
            CreateJsonMetadata(i, seed);

        }
    }

    public string RetrieveTexturePath(UnityEngine.Texture texture)
    {
        return texture.name;
    }
    //void ExportGLTF(int number)
    //{
    //    GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
    //    Transform[] transforms = new Transform[rootGameObjects.Length];
    //    for (int i = 0; i < rootGameObjects.Length; i++)
    //    {
    //        transforms[i] = rootGameObjects[i].transform;
    //    }
    //    var exporter = new GLTFSceneExporter(transforms, RetrieveTexturePath);
    //    string path = Path.Combine(Application.dataPath + "../../../gltf/", number.ToString());
    //    if(!Directory.Exists(path))
    //    {
    //        Directory.CreateDirectory(path);
    //    }
    //    exporter.SaveGLTFandBin(path, "TestScene");
    //}

    void UpdateJsonMetadata(int number, int seed)
    {
        GenerateSettings(seed);

        NFTMetadata metadata = new NFTMetadata();
        //TODO: generate random star names
        metadata.name = "Planet #" + number;
        metadata.description = "Welcome to Solaniverse - a collection of 5555 planets with procedurally generated terrain on the Solana blockchain.\n Explore, gather resources, play and earn on your own multiplayer planet.\n Follow us on Twitter: https://twitter.com/solaniverse \n And join the Discord: https://discord.gg/AWnP7ZkaYk \n This planet gives you access to the viewer and game https://solaniverse.online/planet/" + seed;
        metadata.symbol = "SPLNT";
        metadata.seller_fee_basis_points = 200;
        //TODO: upload generated images to arweave
        metadata.image = "";
        //TODO: make rotating gif animation and upload it to arweave
        metadata.animation_url = "";
        metadata.external_url = "https://www.solaniverse.online/";
        metadata.attributes = new NFTAttribute[5];

        metadata.attributes[0] = new NFTAttribute();
        metadata.attributes[0].trait_type = "Seed";
        metadata.attributes[0].value = seed.ToString();

        metadata.attributes[1] = new NFTAttribute();
        metadata.attributes[1].trait_type = "Size";
        metadata.attributes[1].value = Planet.shapeSettings.planetRadius <= 40.0f ? "Small" : Planet.shapeSettings.planetRadius <= 50.0f ? "Meduim" : "Large";

        metadata.attributes[2] = new NFTAttribute();
        metadata.attributes[2].trait_type = "Resource quantity";
        metadata.attributes[2].value = gradientIndex % 3 == 1 ? "Scarce" : gradientIndex % 3 == 2 ? "Plentiful" : "Limited";

        metadata.attributes[3] = new NFTAttribute();
        metadata.attributes[3].trait_type = "Sky type";
        metadata.attributes[3].value = SkyboxTypes[skyIndex];

        metadata.attributes[4] = new NFTAttribute();
        metadata.attributes[4].trait_type = "Resource type";
        metadata.attributes[4].value = ResourceTypes[(gradientIndex - 1) / 3];

        metadata.properties = new NFTProperties();
        metadata.properties.files = new NFTFiles[1];
        metadata.properties.files[0] = new NFTFiles();
        metadata.properties.files[0].uri = number + ".png";
        metadata.properties.files[0].type = "image/png";
        metadata.properties.files[0].cdn = false;
        metadata.properties.category = "image";
        metadata.properties.creators = new NFTCreator[1];
        metadata.properties.creators[0] = new NFTCreator();
        metadata.properties.creators[0].share = 100;
        metadata.properties.creators[0].address = "Gw9TR8S1pripPSXCk8zvWVE42ddqa71ZFBNJZg8cJ1Qo";

        metadata.collection = new NFTCollection();
        metadata.collection.name = "Solaniverse Planets";
        metadata.collection.family = "Solaniverse";

        String metadataJson = JsonUtility.ToJson(metadata);
        File.WriteAllBytes(Application.dataPath + "../../../generated-photos/" + number + ".json", Encoding.ASCII.GetBytes(metadataJson));
    }

    void CreateJsonMetadata(int number, int seed)
    {
        NFTMetadata metadata = new NFTMetadata();
        //TODO: generate random star names
        metadata.name = "Planet #" + number;
        metadata.description = "Procedurally generated planets expanding the solainverse. Expole, collect and gather resources on the planets, craft using the materials, communicate and earn on these unique planets living on the Solana blockchain";

        //TODO: upload generated images to arweave
        metadata.image = "";
        //TODO: make rotating gif animation and upload it to arweave
        metadata.animation_url = "";
        metadata.external_url = "";
        metadata.attributes = new NFTAttribute[5];

        metadata.attributes[0] = new NFTAttribute();
        metadata.attributes[0].trait_type = "Seed";
        metadata.attributes[0].value = seed.ToString();

        metadata.attributes[1] = new NFTAttribute();
        metadata.attributes[1].trait_type = "Size";
        metadata.attributes[1].value = Planet.shapeSettings.planetRadius <= 40.0f ? "small" : Planet.shapeSettings.planetRadius <= 50.0f ? "meduim" : "large";

        metadata.attributes[2] = new NFTAttribute();
        metadata.attributes[2].trait_type = "Resource quantity";
        metadata.attributes[2].value = gradientIndex % 3 == 1 ? "plentiful" : gradientIndex % 3 == 2 ? "scarce" : "limited";

        metadata.attributes[3] = new NFTAttribute();
        metadata.attributes[3].trait_type = "Sky type";
        metadata.attributes[3].value = SkyboxTypes[skyIndex];

        metadata.attributes[4] = new NFTAttribute();
        metadata.attributes[4].trait_type = "Resource type";
        metadata.attributes[4].value = ResourceTypes[(gradientIndex - 1) / 3];

        metadata.properties = new NFTProperties();
        metadata.properties.files = new NFTFiles[1];
        metadata.properties.files[0] = new NFTFiles();
        metadata.properties.files[0].uri = "";
        metadata.properties.files[0].type = "images/png";
        metadata.properties.files[0].cdn = false;

        //TODO: map gradient index to resource type and quantity 1-3

        String metadataJson = JsonUtility.ToJson(metadata);
        File.WriteAllBytes(Application.dataPath + "../../../generated-photos/" + number + ".json", Encoding.ASCII.GetBytes(metadataJson));
    }
    public void GeneratePlanet(int seed)
    {
        GenerateSettings(seed);
        //patch gradient bug
        Planet.colourSettings.gradient = gradients[(((gradientIndex / gradientMultiplier) - 1) * gradientMultiplier)];
        Planet.GeneratePlanet();
    }

    private void GenerateSettings(int seed)
    {
        Random.InitState(seed);

        int skyRandomSeed = Random.Range(0, 101);
        skyIndex = skyRandomSeed <= 20 ? 0 : skyRandomSeed <= 35 ? 1 : skyRandomSeed <= 50 ? 2 : skyRandomSeed <= 65 ? 3 : skyRandomSeed <= 75 ? 4 : skyRandomSeed <= 85 ? 5 : skyRandomSeed <= 90 ? 6 : skyRandomSeed <= 95 ? 7 : skyRandomSeed <= 97.5 ? 8 : 9;
        RenderSettings.skybox = SkyboxMaterials[skyIndex];
        Planet.shapeSettings.planetRadius = Random.Range(40.0f, 60.0f);

        ShapeSettings.NoiseLayer[] noiseLayers = new ShapeSettings.NoiseLayer[2];
        noiseLayers[0] = new ShapeSettings.NoiseLayer();
        noiseLayers[1] = new ShapeSettings.NoiseLayer();

        noiseLayers[0].enabled = true;
        noiseLayers[0].useFirstLayerAsMask = false;

        NoiseSettings noiseSettings = new NoiseSettings();
        noiseSettings.simpleNoiseSettings = new NoiseSettings.SimpleNoiseSettings();
        noiseSettings.filterType = NoiseSettings.FilterType.Simple;
        noiseSettings.simpleNoiseSettings.strength = Random.Range(0.05f, 0.13f);
        //noiseSettings.simpleNoiseSettings.numLayers = Random.Range(4, 8);
        noiseSettings.simpleNoiseSettings.numLayers = 8;
        noiseSettings.simpleNoiseSettings.baseRoughness = Random.Range(2.0f, 3.0f);
        noiseSettings.simpleNoiseSettings.roughness = Random.Range(1.5f, 2.5f);
        noiseSettings.simpleNoiseSettings.persistence = Random.Range(0.4f, 0.5f);

        noiseLayers[0].noiseSettings = noiseSettings;

        noiseLayers[1].enabled = true;
        noiseLayers[1].useFirstLayerAsMask = true;

        NoiseSettings secondNoiseSettings = new NoiseSettings();
        secondNoiseSettings.filterType = NoiseSettings.FilterType.Ridgid;

        secondNoiseSettings.ridgidNoiseSettings = new NoiseSettings.RidgidNoiseSettings();
        secondNoiseSettings.ridgidNoiseSettings.strength = Random.Range(1f, 2f);
        //secondNoiseSettings.ridgidNoiseSettings.numLayers = Random.Range(4, 8);
        secondNoiseSettings.ridgidNoiseSettings.numLayers = 8;
        secondNoiseSettings.ridgidNoiseSettings.baseRoughness = Random.Range(1.5f, 3.0f);
        secondNoiseSettings.ridgidNoiseSettings.roughness = Random.Range(0.5f, 3f);
        secondNoiseSettings.ridgidNoiseSettings.persistence = Random.Range(0.5f, 1f);
        secondNoiseSettings.ridgidNoiseSettings.minValue = Random.Range(0.2f, 0.8f);

        noiseLayers[1].noiseSettings = noiseSettings;

        Planet.shapeSettings.noiseLayers = noiseLayers;

        int gradientIndexSeed = Random.Range(0, 101);
        gradientIndex = gradientIndexSeed <= 20 ? 10 : gradientIndexSeed <= 35 ? 9 : gradientIndexSeed <= 50 ? 8 : gradientIndexSeed <= 65 ? 7 : gradientIndexSeed <= 75 ? 6 : gradientIndexSeed <= 85 ? 5 : gradientIndexSeed <= 90 ? 4 : gradientIndexSeed <= 95 ? 3 : gradientIndexSeed <= 97.5 ? 2 : 1;

        //for different resource scarcities
        gradientMultiplier = (int)Random.Range(1, 4);
        gradientIndex *= gradientMultiplier;
        Planet.colourSettings.gradient = gradients[gradientIndex - 1];
    }
    private void CaptureScreenShot(int nubmer)
    {
        RenderTexture currentRT = RenderTexture.active;

        RenderTexture.active = image;
        RenderingCamera.targetTexture = image;

        RenderingCamera.Render();

        Texture2D Image = new Texture2D(RenderingCamera.targetTexture.width, RenderingCamera.targetTexture.height, TextureFormat.RGB24, false, true);
        Image.ReadPixels(new Rect(0, 0, RenderingCamera.targetTexture.width, RenderingCamera.targetTexture.height), 0, 0);
        Image.Apply();

        var Bytes = Image.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "../../../generated-photos/" + nubmer + ".png", Bytes);
        Destroy(Image);

        RenderTexture.active = currentRT;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        image = new RenderTexture(3840, 2160, 32);
        image.name = "RenderTexture";
        image.enableRandomWrite = true;
        image.Create();

    }

}
