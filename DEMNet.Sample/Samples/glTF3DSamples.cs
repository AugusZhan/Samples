﻿//
// glTF3DSamples.cs
//
// Author:
//       Xavier Fischer
//
// Copyright (c) 2019 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using DEM.Net.Core;
using DEM.Net.glTF.SharpglTF;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DEM.Net.Core.Imagery;

namespace SampleApp
{
    /// <summary>
    /// Extracts a DEM from a bbox and generates a 3D export in glTF format
    /// </summary>
    public class glTF3DSamples
    {
        private readonly ILogger<glTF3DSamples> _logger;
        private readonly IElevationService _elevationService;
<<<<<<< HEAD
        private readonly IglTFService _glTFService;
        private readonly IImageryService _imageryService;


        public glTF3DSamples(ILogger<glTF3DSamples> logger
                , IElevationService elevationService
                , IglTFService glTFService
                , IImageryService imageryService)
        {
            _logger = logger;
            _elevationService = elevationService;
            _glTFService = glTFService;
            _imageryService = imageryService;
=======
        private readonly SharpGltfService _sharpGltfService;

        public glTF3DSamples(ILogger<glTF3DSamples> logger
                , IElevationService elevationService
                , SharpGltfService sharpGltfService)
        {
            _logger = logger;
            _elevationService = elevationService;
            _sharpGltfService = sharpGltfService;


>>>>>>> 104765ffe0f1757308e176bada4b37c91ea116d9
        }
        public void Run(DEMDataSet dataset, bool withTexture = true)
        {
            try
            {


                Stopwatch sw = Stopwatch.StartNew();
                string modelName = $"Montagne Sainte Victoire {dataset.Name}";
                string outputDir = Directory.GetCurrentDirectory();
                
                ImageryProvider provider = ImageryProvider.MapBoxSatelliteStreet;// new TileDebugProvider(new GeoPoint(43.5,5.5));

                // You can get your boox from https://geojson.net/ (save as WKT)
                //string bboxWKT = "POLYGON((5.54888 43.519525, 5.61209 43.519525, 5.61209 43.565225, 5.54888 43.565225, 5.54888 43.519525))";
//                string bboxWKT =
//                    "POLYGON((5.594457381483949 43.545276557046044,5.652135604140199 43.545276557046044,5.652135604140199 43.52038635099936,5.594457381483949 43.52038635099936,5.594457381483949 43.545276557046044))";
//                _logger.LogInformation($"Processing model {modelName}...");
//
//
//                _logger.LogInformation($"Getting bounding box geometry...");
//                var bbox = GeometryService.GetBoundingBox(bboxWKT);

                var bbox = new BoundingBox(5.5613898348431485,5.597185285307553,43.49372969433046,43.50939068558466);
                _logger.LogInformation($"Getting height map data...");
<<<<<<< HEAD
                var heightMap = _elevationService.GetHeightMap(bbox, dataset);
               bbox = heightMap.BoundingBox;
                
//                var refPoint = new GeoPoint(43.5, 5.5);
//                heightMap = heightMap.BakeCoordinates();
//                var hMapRefPoint = heightMap.Coordinates.OrderBy(c => c.DistanceSquaredTo(refPoint)).First();
//                hMapRefPoint.Elevation += 100;
                
=======
                var heightMap = _elevationService.GetHeightMap(ref bbox, dataset);

>>>>>>> 104765ffe0f1757308e176bada4b37c91ea116d9
                _logger.LogInformation($"Processing height map data ({heightMap.Count} coordinates)...");
                heightMap = heightMap
                    .ReprojectGeodeticToCartesian() // Reproject to 3857 (useful to get coordinates in meters)
                    .ZScale(2f);                    // Elevation exageration

                //=======================
                // Textures
                //
                PBRTexture pbrTexture = null;
                if (withTexture)
                {


                    Console.WriteLine("Download image tiles...");
                    TileRange tiles = _imageryService.DownloadTiles(bbox, provider, 8);
                    string fileName = Path.Combine(outputDir, "Texture.jpg");

                    Console.WriteLine("Construct texture...");
                    TextureInfo texInfo = _imageryService.ConstructTexture(tiles, bbox, fileName, TextureImageFormat.image_jpeg);

                    //
                    //=======================

                    //=======================
                    // Normal map
                    Console.WriteLine("Height map...");
                    //float Z_FACTOR = 0.00002f;

                    //hMap = hMap.CenterOnOrigin().ZScale(Z_FACTOR);
                    var normalMap = _imageryService.GenerateNormalMap(heightMap, outputDir);
    
                    pbrTexture = PBRTexture.Create(texInfo, normalMap);

                    //hMap = hMap.CenterOnOrigin(Z_FACTOR);
                    //
                    //=======================
                }
                // Triangulate height map
                // and add base and sides
                _logger.LogInformation($"Triangulating height map and generating 3D mesh...");
<<<<<<< HEAD
                var mesh = _glTFService.GenerateTriangleMesh(heightMap, null, pbrTexture);
=======
>>>>>>> 104765ffe0f1757308e176bada4b37c91ea116d9

                var model = _sharpGltfService.CreateTerrainMesh(heightMap);
                model.SaveGLB(Path.Combine(Directory.GetCurrentDirectory(), modelName + ".glb"));

<<<<<<< HEAD
                _logger.LogInformation($"Exporting glTF model...");
                _glTFService.Export(model, outputDir, modelName, exportglTF: false, exportGLB: true);
=======
                model = _sharpGltfService.CreateTerrainMesh(heightMap, GenOptions.Normals | GenOptions.BoxedBaseElevationMin);
                model.SaveGLB(Path.Combine(Directory.GetCurrentDirectory(), modelName + "_normalsBox.glb"));
>>>>>>> 104765ffe0f1757308e176bada4b37c91ea116d9

                _logger.LogInformation($"Model exported as {Path.Combine(Directory.GetCurrentDirectory(), modelName + ".gltf")} and .glb");

                _logger.LogInformation($"Done in {sw.Elapsed:g}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
