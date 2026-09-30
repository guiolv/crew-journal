// CrewJournal — contrato do layout estrutural de ilhas (SPEC-ISLAND-CJ).
// Estado RED: so assinatura + tipos. Implementacao real e fase posterior.
// NAO tocar IslandRenderer.cs ate a fase de adocao (DoD do spec).
// Puro C#, sem UnityEngine (mesma regra dos demais arquivos Logic/).
using System;

namespace CrewJournal.Logic
{
    public enum IslandTile { Ocean, Sand, Grass }

    public class IslandLayout
    {
        public int width;
        public int height;
        public int seed;
        public IslandTile[,] map;
        public int spawnX;
        public int spawnY;

        public static IslandLayout Generate(int width, int height, int seed)
        {
            throw new NotImplementedException("SPEC-ISLAND-CJ RED: layout ainda nao implementado");
        }
    }
}
