
using System;
using Unity.Mathematics;
using Unity.Collections;
using Random = Unity.Mathematics.Random;

public class PoissonDiskSampling : IDisposable
{
    private readonly float radius;
    private readonly float cellSize;
    private readonly int width, height;
    private readonly int gridWidth, gridHeight;

    private NativeArray<float2> grid;
    private NativeArray<int> gridOccupied;

    private NativeList<float2> samples;
    private NativeList<float2> activeList;

    private Random random;
    private readonly int k = 50;

    public PoissonDiskSampling(int width, int height, float radius, uint seed = 1)
    {
        this.width = width;
        this.height = height;
        this.radius = radius;
        this.cellSize = radius / (float)math.sqrt(2);

        this.gridWidth = (int)math.ceil(width / cellSize);
        this.gridHeight = (int)math.ceil(height / cellSize);

        this.grid = new NativeArray<float2>(gridWidth * gridHeight, Allocator.Persistent);
        this.gridOccupied = new NativeArray<int>(gridWidth * gridHeight, Allocator.Persistent);

        this.samples = new NativeList<float2>(Allocator.Persistent);
        this.activeList = new NativeList<float2>(Allocator.Persistent);

        if (seed == 0) seed = 1;
        this.random = new Random(seed);
    }

    public void Dispose()
    {
        if (grid.IsCreated) grid.Dispose();
        if (gridOccupied.IsCreated) gridOccupied.Dispose();
        if (samples.IsCreated) samples.Dispose();
        if (activeList.IsCreated) activeList.Dispose();
    }

    public NativeList<float2> Generate()
    {
        samples.Clear();
        activeList.Clear();
        for (int i = 0; i < gridOccupied.Length; i++) gridOccupied[i] = 0;

        var initialPoint = new float2(
            random.NextFloat(0f, width),
            random.NextFloat(0f, height)
        );

        samples.Add(initialPoint);
        activeList.Add(initialPoint);
        PlaceInGrid(initialPoint);

        while (activeList.Length > 0)
        {
            int index = random.NextInt(0, activeList.Length);
            float2 point = activeList[index];

            bool found = false;
            for (int i = 0; i < k; i++)
            {
                float2 candidate = GenerateCandidate(point);

                if (IsValid(candidate))
                {
                    samples.Add(candidate);
                    activeList.Add(candidate);
                    PlaceInGrid(candidate);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                activeList.RemoveAtSwapBack(index);
            }
        }

        return samples;
    }

    private float2 GenerateCandidate(float2 point)
    {
        float angle = random.NextFloat(0f, math.PI * 2f);
        float r = radius * (1f + random.NextFloat());

        return new float2(
            point.x + r * math.cos(angle),
            point.y + r * math.sin(angle)
        );
    }

    private bool IsValid(float2 point)
    {
        if (point.x < radius || point.y < radius || point.x >= width - radius || point.y >= height - radius)
            return false;

        int gx = (int)math.floor(point.x / cellSize);
        int gy = (int)math.floor(point.y / cellSize);

        int startX = math.max(gx - 2, 0);
        int endX = math.min(gx + 2, gridWidth - 1);
        int startY = math.max(gy - 2, 0);
        int endY = math.min(gy + 2, gridHeight - 1);

        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                int idx = Util.Int2ToInt(x, y, gridWidth);
                if (gridOccupied[idx] == 0) continue;
                float dist = Util.Distance(grid[idx], point);
                if (dist < radius) return false;
            }
        }

        return true;
    }

    private void PlaceInGrid(float2 point)
    {
        int gx = (int)math.floor(point.x / cellSize);
        int gy = (int)math.floor(point.y / cellSize);
        int idx = Util.Int2ToInt(gx, gy, gridWidth);
        grid[idx] = point;
        gridOccupied[idx] = 1;
    }
}

