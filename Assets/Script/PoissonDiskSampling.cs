
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Random = Unity.Mathematics.Random;
[BurstCompile]
public class PoissonDiskSampling
{
    private readonly float radius;
    private readonly float cellSize;
    private readonly int width, height;
    private readonly int gridWidth, gridHeight;
    private NativeArray<float2> grid;
    private readonly NativeList<float2> samples = new(Allocator.Persistent);
    private readonly NativeList<float2> activeList = new(Allocator.Persistent);
    private readonly Random random;

    // k: 1点から候補を生成する試行回数（通常は20くらい）
    private readonly int k = 30;

    public PoissonDiskSampling(int width, int height, float radius, uint seed = 0)
    {
        this.width = width;
        this.height = height;
        this.radius = radius;
        this.cellSize = radius / (float)Math.Sqrt(2);

        this.gridWidth = (int)Math.Ceiling(width / cellSize);
        this.gridHeight = (int)Math.Ceiling(height / cellSize);
        this.grid = new NativeArray<float2>(width * height, Allocator.Persistent);

        this.random = (seed == 0) ? new Random() : new Random(seed);
    }

    public NativeList<float2> Generate()
    {
        // 初期点をランダムに生成
        var initialPoint = new float2(
            (float)random.NextDouble() * width,
            (float)random.NextDouble() * height
        );

        samples.Add(initialPoint);
        activeList.Add(initialPoint);
        PlaceInGrid(initialPoint);

        // アクティブリストが空になるまで繰り返し
        while (activeList.Length > 0)
        {
            int index = random.NextInt(activeList.Length);
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
                activeList.RemoveAt(index);
            }
        }

        return samples;
    }

    private float2 GenerateCandidate(float2 point)
    {
        float angle = (float)(2 * Math.PI * random.NextDouble());
        float r = radius * (1 + (float)random.NextDouble());

        return new float2(
            point.x + r * (float)Math.Cos(angle),
            point.y + r * (float)Math.Sin(angle)
        );
    }

    private bool IsValid(float2 point)
    {
        if (point.x < 0 || point.y < 0 || point.x >= width || point.y >= height)
            return false;

        int gx = (int)(point.x / cellSize);
        int gy = (int)(point.y / cellSize);

        int startX = math.max(gx - 2, 0);
        int endX = math.min(gx + 2, gridWidth - 1);
        int startY = math.max(gy - 2, 0);
        int endY = math.min(gy + 2, gridHeight - 1);

        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                var index = Util.Int2ToInt(x, y, width);
                float dist = Util.Distance(grid[index], point);
                if (dist < radius)
                    return false;
            }
        }

        return true;
    }

    private void PlaceInGrid(float2 point)
    {
        int gx = (int)(point.x / cellSize);
        int gy = (int)(point.y / cellSize);
        var index = Util.Int2ToInt(gx, gy, width);
        grid[index] = point;
    }
}

