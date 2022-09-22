using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmPatchManager : MonoBehaviour
{
    public FarmConfiguration Config;
    public List<FarmPatch> Patches;

    private void Start()
    {
        foreach (FarmPatch patch in Patches)
        {
            patch.SetManager(this);
        }
        this.SpawnPumpkins();
    }

    protected void SpawnPumpkins()
    {
        int pumpkinCount = Random.Range(Config.MinPumpkinSpawns, Config.MaxPumpkinSpawns);
        while (pumpkinCount > 0)
        {
            // Get a random patch / tile for each spawn.
            FarmTile selectedTile = this.getRandomTileInPatches();

            if (!selectedTile.Occupied)
            {
                selectedTile.SpawnPumpkin(this.getNewPumpkinType(), PumpkinState.Grown);
                pumpkinCount--;
            }
        }
    }

    public void RequestNewSapling(FarmTile tileToSpawnOn_)
    {
        tileToSpawnOn_.SpawnPumpkin(this.getNewPumpkinType(), PumpkinState.Sapling);
    }

    protected FarmTile getRandomTileInPatches()
    {
        int patchIndex = Random.Range(0, this.Patches.Count);
        FarmPatch selectedPatch = this.Patches[patchIndex];
        int tileIndex = Random.Range(0, selectedPatch.Tiles.Count);
        return selectedPatch.Tiles[tileIndex];
    }
    
    protected PumpkinType getNewPumpkinType()
    {
        int pumpkinIndex = Random.Range(0, this.Config.PumpkinTypes.Length);
        return this.Config.PumpkinTypes[pumpkinIndex];
    }
}
