// CrewJournal — importacao dos PNGs fatiados: sprite single, sem compressao,
// 9-slice em botoes e paineis. Reimporta sozinho ao abrir o projeto.
using UnityEngine;
using UnityEditor;

public class ArtPost : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!assetPath.Contains("/Resources/Art/")) return;
        TextureImporter t = (TextureImporter)assetImporter;
        t.textureType = TextureImporterType.Sprite;
        t.spriteImportMode = SpriteImportMode.Single;
        t.filterMode = FilterMode.Bilinear;
        t.textureCompression = TextureImporterCompression.Uncompressed;
        t.maxTextureSize = 512;
        string f = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        if (f.StartsWith("btn_")) t.spriteBorder = new Vector4(20, 12, 20, 12);
        else if (f.StartsWith("panel_")) t.spriteBorder = new Vector4(26, 26, 26, 26);
    }
}
