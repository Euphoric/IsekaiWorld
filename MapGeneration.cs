using System;
using System.Diagnostics;
using DeBroglie;
using DeBroglie.Models;
using DeBroglie.Rot;
using DeBroglie.Topo;

namespace IsekaiWorld
{
    public class MapGeneration
    {
        public static byte[,] GenerateMap(int width, int height)
        {
            //var output = UseWaveFunctionCollapse(width, height);
            var output = UseNoise(width, height);

            return output;
        }

        private static byte[,] UseNoise(int width, int height)
        {
            var noise = new Simplex.Noise();
            noise.Seed = 1234;
            var output = new byte[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var noiseValue = noise.CalcPixel2D(x, y, 1/16f) / 256;
                    output[x, y] = (byte)(noiseValue < 0.5 ? 0 : 1);
                }
            }

            return output;
        }

        private static byte[,] UseWaveFunctionCollapse(int width, int height)
        {
            var stopwatch = Stopwatch.StartNew();
            // Define some sample data
            //var model = AdjacentModel();
            var model = CreateOverlappingModel();
            // Set the output dimensions
            var topology = new GridTopology(width, height, periodic: false);
            // Actually run the algorithm
            var propagator = new TilePropagator(model, topology);
            var status = propagator.Run();
            if (status != Resolution.Decided)
                throw new Exception("Undecided");
            var output = propagator.ToValueArray<byte>().ToArray2d();

            Debug.WriteLine($"Map generation took: {stopwatch.Elapsed}");
            return output;
        }

        private static OverlappingModel CreateOverlappingModel()
        {
            var model = new OverlappingModel(2, 2, 0);
            var grass = new Tile((byte)0);
            var dirt0 = new Tile((byte)1);
            var wall0 = new Tile((byte)3);
            ITopoArray<Tile> sample1 = TopoArray.Create(new[]
            {
                new[] { grass, grass, grass },
                new[] { grass, grass, grass },
                new[] { grass, grass, grass },
            }, periodic: false);
            ITopoArray<Tile> sample2 = TopoArray.Create(new[]
            {
                new[] { dirt0, dirt0 },
                new[] { dirt0, dirt0 },
            }, periodic: false);
            ITopoArray<Tile> sample3 = TopoArray.Create(new[]
            {
                new[] { grass, grass, grass, grass, grass },
                new[] { grass, dirt0, dirt0, dirt0, grass },
                new[] { grass, dirt0, dirt0, dirt0, grass },
                new[] { grass, dirt0, dirt0, dirt0, grass },
                new[] { grass, grass, grass, grass, grass },
            }, periodic: false);
            
            model.AddSample(sample1);
            model.AddSample(sample2);
            model.AddSample(sample3);
            
            return model;
        }

        private static AdjacentModel AdjacentModel()
        {
            var model = new AdjacentModel();
            var grass = new Tile((byte)0);
            var dirt0 = new Tile((byte)1);
            var wall0 = new Tile((byte)3);
            ITopoArray<Tile> sample1 = TopoArray.Create(new[]
            {
                new[] { grass, grass, grass },
                new[] { grass, grass, grass },
                new[] { grass, grass, grass },
            }, periodic: false);

            ITopoArray<Tile> sample2 = TopoArray.Create(new[]
            {
                new[] { dirt0, dirt0 },
                new[] { dirt0, dirt0 },
            }, periodic: false);

            ITopoArray<Tile> sample3 = TopoArray.Create(new[]
            {
                new[] { dirt0, grass },
                new[] { grass, dirt0 },
            }, periodic: true);

            ITopoArray<Tile> sample4 = TopoArray.Create(new[]
            {
                new[] { grass, grass, grass, grass, grass, grass },
                new[] { grass, wall0, wall0, wall0, wall0, grass },
                new[] { grass, wall0, grass, grass, wall0, grass },
                new[] { grass, grass, grass, grass, wall0, grass },
                new[] { grass, grass, wall0, wall0, wall0, grass },
                new[] { grass, grass, grass, grass, grass, grass },
            }, periodic: false);

            model.SetDirections(DirectionSet.Cartesian2d);
            model.AddSample(sample1);
            model.AddSample(sample2);
            model.AddSample(sample3);
            model.AddSample(sample4);
            return model;
        }
    }
}