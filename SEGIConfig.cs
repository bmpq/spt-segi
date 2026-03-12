using BepInEx.Configuration;
using UnityEngine;

namespace tarkin.SEGI.Bep
{
    internal class SEGIConfig
    {
        private enum Preset
        {
            Low,
            High
        }
        internal ConfigEntry<string> presetDummy;

        // Main Configuration
        internal ConfigEntry<SEGIRenderer.VoxelResolution> voxelResolution;
        internal ConfigEntry<bool> voxelAA;
        internal ConfigEntry<int> innerOcclusionLayers;
        internal ConfigEntry<bool> gaussianMipFilter;
        internal ConfigEntry<float> voxelSpaceSize;
        internal ConfigEntry<float> shadowSpaceSize;
        internal ConfigEntry<bool> updateGI;
        internal ConfigEntry<bool> infiniteBounces;

        // Environment Properties
        internal ConfigEntry<float> softSunlight;
        internal ConfigEntry<Color> skyColor;
        internal ConfigEntry<float> skyIntensity;
        internal ConfigEntry<bool> sphericalSkylight;

        // Tracing Properties
        internal ConfigEntry<float> temporalBlendWeight;
        internal ConfigEntry<bool> useBilateralFiltering;
        internal ConfigEntry<bool> halfResolution;
        internal ConfigEntry<bool> stochasticSampling;
        internal ConfigEntry<int> cones;
        internal ConfigEntry<int> coneTraceSteps;
        internal ConfigEntry<float> coneLength;
        internal ConfigEntry<float> coneWidth;
        internal ConfigEntry<float> coneTraceBias;
        internal ConfigEntry<float> edgeFadeSize;
        internal ConfigEntry<float> occlusionStrength;
        internal ConfigEntry<float> nearOcclusionStrength;
        internal ConfigEntry<float> farOcclusionStrength;
        internal ConfigEntry<float> farthestOcclusionStrength;
        internal ConfigEntry<float> occlusionPower;
        internal ConfigEntry<float> nearLightGain;
        internal ConfigEntry<float> giGain;
        internal ConfigEntry<float> secondaryBounceGain;
        internal ConfigEntry<int> secondaryCones;
        internal ConfigEntry<float> secondaryOcclusionStrength;

        // Reflection Properties
        internal ConfigEntry<bool> doReflections;
        internal ConfigEntry<int> reflectionSteps;
        internal ConfigEntry<float> reflectionOcclusionPower;
        internal ConfigEntry<float> skyReflectionIntensity;

        // Debug Tools
        internal ConfigEntry<bool> visualizeSunDepthTexture;
        internal ConfigEntry<bool> visualizeGI;
        internal ConfigEntry<bool> visualizeVoxels;

        public void Bind(ConfigFile config)
        {
            // dummy entry to host the custom drawer
            presetDummy = config.Bind("0. Presets", "Presets", "",
                new ConfigDescription("", null,
                    new ConfigurationManagerAttributes
                    {
                        CustomDrawer = DrawPresetButtons,
                        HideDefaultButton = true,
                        HideSettingName = true
                    }));

            // Main Configuration
            const string secMain = "1. Main Configuration";

            voxelResolution = config.Bind(secMain, "Voxel Resolution", SEGIRenderer.VoxelResolution.high,
                "Significant performance impact. Reduces performance load heavily if lowered. Controls the resolution of the volume used to store scene data.");

            voxelAA = config.Bind(secMain, "Voxel AA", false,
                "Enables 8x supersampling during voxelization. Improves the look of moving or small objects at the cost of performance.");

            innerOcclusionLayers = config.Bind(secMain, "Inner Occlusion Layers", 0,
                new ConfigDescription("Prevents light leaking artifacts, but may cause issues with thin or small objects. Adds black opaque layers to the backside of geometry.",
                new AcceptableValueRange<int>(0, 2)));

            gaussianMipFilter = config.Bind(secMain, "Gaussian Mip Filter", false,
                "Improves smoothness and consistency of lighting and reflections. Has a significant performance impact at High voxel resolution, but minor impact at Low resolution.");

            voxelSpaceSize = config.Bind(secMain, "Voxel Space Size", 25.0f,
                new ConfigDescription("The extents of the voxel volume. Higher values allow farther objects to contribute to GI, but increases the size of individual voxels (reducing detail).",
                new AcceptableValueRange<float>(0, 100f)));

            shadowSpaceSize = config.Bind(secMain, "Shadow Space Size", 30.0f,
                new ConfigDescription("Size of the shadow map used for sunlight injection. Recommended to keep this similar to Voxel Space Size.",
                new AcceptableValueRange<float>(0, 100f)));

            updateGI = config.Bind(secMain, "Update GI", true,
                "If disabled, GI calculations stop and the scene uses cached data from the last frame. Disable to save performance in scenes.");

            infiniteBounces = config.Bind(secMain, "Infinite Bounces", false,
                "Experimental. Enables a feedback loop for unlimited light bounces. Performance cost depends on 'Secondary Cones' setting.");


            // Environment Properties
            const string secEnv = "2. Environment";

            softSunlight = config.Bind(secEnv, "Soft Sunlight", 0.0f,
                new ConfigDescription("Adds cone-traced soft sunlight. Useful for cloudy scenes, haze, or sunset lighting.",
                new AcceptableValueRange<float>(0.0f, 16.0f)));

            skyColor = config.Bind(secEnv, "Sky Color", Color.white, 
                new ConfigDescription("The average color of the sky light added to the scene.", 
                tags: new ConfigurationManagerAttributes() { IsAdvanced = true }));

            skyIntensity = config.Bind(secEnv, "Sky Intensity", 0f,
                new ConfigDescription("Tarkov already does fake sky lighting, this setting will just light on top and look wrong.",
                new AcceptableValueRange<float>(0.0f, 8.0f),
                tags: new ConfigurationManagerAttributes() { IsAdvanced = true }));

            sphericalSkylight = config.Bind(secEnv, "Spherical Skylight", false,
                new ConfigDescription("If enabled, sky light comes from all directions. If disabled, it only comes from the top hemisphere.",
                tags: new ConfigurationManagerAttributes() { IsAdvanced = true }));


            // Tracing Properties
            const string secTrace = "3. Tracing";

            temporalBlendWeight = config.Bind(secTrace, "Temporal Blend Weight", 0.1f,
                new ConfigDescription("Controls how much previous frames contribute to the current frame. Lower values are smoother and less grainy but update 'lazily'.",
                new AcceptableValueRange<float>(0.01f, 1.0f)));

            useBilateralFiltering = config.Bind(secTrace, "Bilateral Filtering", false,
                "Blurs GI to smooth out graininess. Best used when Stochastic Sampling is enabled.");

            halfResolution = config.Bind(secTrace, "Half Resolution", true,
                "Highly recommended for performance. Renders GI at half the screen resolution. Visual drawbacks are usually minimal.");

            stochasticSampling = config.Bind(secTrace, "Stochastic Sampling", true,
                "Randomizes trace cones per-pixel. Trades banding artifacts for noise (which can be smoothed with Temporal Blend).");

            cones = config.Bind(secTrace, "Cones", 6,
                new ConfigDescription("Number of diffuse cones to trace. Higher values reduce noise/banding but cost performance. Lower values work well if Gaussian Mip Filter is on.",
                new AcceptableValueRange<int>(1, 128)));

            coneTraceSteps = config.Bind(secTrace, "Cone Trace Steps", 14,
                new ConfigDescription("Samples taken per cone. Higher values reduce banding but darken indirect shadows. High values cost performance.",
                new AcceptableValueRange<int>(1, 32)));

            coneLength = config.Bind(secTrace, "Cone Length", 1.0f,
                new ConfigDescription("Adjusts the 'GI Radius'. Determines how far away objects can be to contribute to lighting.",
                new AcceptableValueRange<float>(0.1f, 2.0f)));

            coneWidth = config.Bind(secTrace, "Cone Width", 5.5f,
                new ConfigDescription("Wider cones are smoother (less noise) but can cause over-occlusion or self-bleeding artifacts.",
                new AcceptableValueRange<float>(0.5f, 6.0f)));

            coneTraceBias = config.Bind(secTrace, "Cone Trace Bias", 1.0f,
                new ConfigDescription("Offset from surface to begin tracing. Increase this if you see 'voxel acne' (self-occlusion artifacts).",
                new AcceptableValueRange<float>(0.0f, 4.0f)));

            occlusionStrength = config.Bind(secTrace, "Occlusion Strength", 1.0f,
                new ConfigDescription("Global modifier for how much light is blocked by objects. Affects both near and far shadows.",
                new AcceptableValueRange<float>(0.0f, 4.0f)));

            nearOcclusionStrength = config.Bind(secTrace, "Near Occlusion Strength", 0.5f,
                new ConfigDescription("Strength of shadows from very close objects. Reduce this to fix close-proximity artifacts.",
                new AcceptableValueRange<float>(0.0f, 4.0f)));

            farOcclusionStrength = config.Bind(secTrace, "Far Occlusion Strength", 1.0f,
                new ConfigDescription("Light blocking strength for distant objects proportional to distance.",
                new AcceptableValueRange<float>(0.1f, 4.0f)));

            farthestOcclusionStrength = config.Bind(secTrace, "Farthest Occlusion Strength", 1.0f,
                new ConfigDescription("Light blocking strength for the farthest objects proportional to squared distance.",
                new AcceptableValueRange<float>(0.1f, 4.0f)));

            occlusionPower = config.Bind(secTrace, "Occlusion Power", 1.5f,
                new ConfigDescription("Higher values cause indirect shadows to 'fill out' more.",
                new AcceptableValueRange<float>(0.001f, 4.0f)));

            nearLightGain = config.Bind(secTrace, "Near Light Gain", 1.0f,
                new ConfigDescription("Controls close-proximity light intensity. Lower values result in a cleaner, less noisy look.",
                new AcceptableValueRange<float>(0.0f, 4.0f)));

            giGain = config.Bind(secTrace, "GI Gain", 4.0f,
                new ConfigDescription("The overall brightness of the indirect light.",
                new AcceptableValueRange<float>(0.0f, 10.0f)));

            secondaryBounceGain = config.Bind(secTrace, "Secondary Bounce Gain", 1.0f,
                new ConfigDescription("Strength of infinite bounces. Values > 1 can cause runaway light flooding.",
                new AcceptableValueRange<float>(0.0f, 4.0f)));

            secondaryCones = config.Bind(secTrace, "Secondary Cones", 6,
                new ConfigDescription("Number of cones for infinite bounces. Improves accuracy but costs performance based on scene complexity.",
                new AcceptableValueRange<int>(3, 16)));

            secondaryOcclusionStrength = config.Bind(secTrace, "Secondary Occlusion Strength", 1.0f,
                new ConfigDescription("Light blocking during infinite bounces. Too low values causes runaway light flooding.",
                new AcceptableValueRange<float>(0.1f, 4.0f)));


            // Reflection Properties
            const string secRefl = "4. Reflections";

            doReflections = config.Bind(secRefl, "Enable Reflections", true,
                "If enabled, renders cone-traced reflections.");

            reflectionSteps = config.Bind(secRefl, "Reflection Steps", 64,
                new ConfigDescription("Higher values reduce tracing artifacts but cost more performance.",
                new AcceptableValueRange<int>(12, 128)));

            reflectionOcclusionPower = config.Bind(secRefl, "Reflection Occlusion Power", 1.0f,
                new ConfigDescription("Higher values can reduce light leaking in reflections.",
                new AcceptableValueRange<float>(0.001f, 4.0f)));

            skyReflectionIntensity = config.Bind(secRefl, "Sky Reflection Intensity", 0.0f,
                new ConfigDescription("Brightness of sky reflections.",
                new AcceptableValueRange<float>(0.0f, 1.0f),
                tags: new ConfigurationManagerAttributes() { IsAdvanced = true }));


            // Debug Tools
            const string secDebug = "5. Debug";

            visualizeSunDepthTexture = config.Bind(secDebug, "Visualize Sun Depth", false,
                "Displays the texture used to inject sunlight/shadows. Useful for checking accuracy.");

            visualizeGI = config.Bind(secDebug, "Visualize GI", false,
                "Displays only the indirect lighting result.");

            visualizeVoxels = config.Bind(secDebug, "Visualize Voxels", false,
                "Displays the direct voxel data. Useful for checking if objects are being voxelized correctly.");
        }

        private void DrawPresetButtons(ConfigEntryBase entry)
        {
            GUILayout.BeginHorizontal();

            foreach (Preset preset in System.Enum.GetValues(typeof(Preset)))
            {
                if (GUILayout.Button(preset.ToString(), GUILayout.ExpandWidth(true)))
                    ApplyPreset(preset);
            }

            GUILayout.EndHorizontal();
        }

        private void ApplyPreset(Preset preset)
        {
            updateGI.Value = true;

            softSunlight.Value = 0f;
            skyIntensity.Value = 0f;
            sphericalSkylight.Value = false;
            skyReflectionIntensity.Value = 0f;

            visualizeSunDepthTexture.Value = false;
            visualizeGI.Value = false;
            visualizeVoxels.Value = false;

            switch (preset)
            {
                case Preset.Low:
                    voxelResolution.Value = SEGIRenderer.VoxelResolution.low;
                    voxelAA.Value = false;
                    innerOcclusionLayers.Value = 0;
                    gaussianMipFilter.Value = false;
                    voxelSpaceSize.Value = 25f;
                    shadowSpaceSize.Value = 30f;
                    infiniteBounces.Value = false;
                    temporalBlendWeight.Value = 0.15f;
                    useBilateralFiltering.Value = true;
                    halfResolution.Value = true;
                    stochasticSampling.Value = true;
                    cones.Value = 3;
                    coneTraceSteps.Value = 8;
                    coneLength.Value = 1f;
                    coneWidth.Value = 5.5f;
                    coneTraceBias.Value = 1f;
                    occlusionStrength.Value = 1f;
                    nearOcclusionStrength.Value = 0.5f;
                    farOcclusionStrength.Value = 1f;
                    farthestOcclusionStrength.Value = 1f;
                    occlusionPower.Value = 1.5f;
                    nearLightGain.Value = 1f;
                    giGain.Value = 4f;

                    secondaryBounceGain.Value = 1f;
                    secondaryCones.Value = 3;
                    secondaryOcclusionStrength.Value = 1f;

                    doReflections.Value = false;
                    reflectionSteps.Value = 32;
                    reflectionOcclusionPower.Value = 1f;

                    break;
                case Preset.High:
                    voxelResolution.Value = SEGIRenderer.VoxelResolution.high;
                    voxelAA.Value = false;
                    innerOcclusionLayers.Value = 0;
                    gaussianMipFilter.Value = false;
                    voxelSpaceSize.Value = 25f;
                    shadowSpaceSize.Value = 30f;
                    infiniteBounces.Value = false;
                    temporalBlendWeight.Value = 0.1f;
                    useBilateralFiltering.Value = false;
                    halfResolution.Value = true;
                    stochasticSampling.Value = true;
                    cones.Value = 6;
                    coneTraceSteps.Value = 14;
                    coneLength.Value = 1f;
                    coneWidth.Value = 5.5f;
                    coneTraceBias.Value = 1f;
                    occlusionStrength.Value = 1f;
                    nearOcclusionStrength.Value = 0.5f;
                    farOcclusionStrength.Value = 1f;
                    farthestOcclusionStrength.Value = 1f;
                    occlusionPower.Value = 1.5f;
                    nearLightGain.Value = 1f;
                    giGain.Value = 4f;

                    secondaryBounceGain.Value = 1f;
                    secondaryCones.Value = 6;
                    secondaryOcclusionStrength.Value = 1f;

                    doReflections.Value = true;
                    reflectionSteps.Value = 64;
                    reflectionOcclusionPower.Value = 1f;

                    break;
            }
        }

        public void Apply(SEGIRenderer segi)
        {
            // Main Configuration
            segi.voxelResolution = voxelResolution.Value;
            segi.voxelAA = voxelAA.Value;
            segi.innerOcclusionLayers = innerOcclusionLayers.Value;
            segi.gaussianMipFilter = gaussianMipFilter.Value;
            segi.voxelSpaceSize = voxelSpaceSize.Value;
            segi.shadowSpaceSize = shadowSpaceSize.Value;
            segi.updateGI = updateGI.Value;
            segi.infiniteBounces = infiniteBounces.Value;

            // Environment Properties
            segi.softSunlight = softSunlight.Value;
            segi.skyColor = skyColor.Value;
            segi.skyIntensity = skyIntensity.Value;
            segi.sphericalSkylight = sphericalSkylight.Value;

            // Tracing Properties
            segi.temporalBlendWeight = temporalBlendWeight.Value;
            segi.useBilateralFiltering = useBilateralFiltering.Value;
            segi.halfResolution = halfResolution.Value;
            segi.stochasticSampling = stochasticSampling.Value;
            segi.cones = cones.Value;
            segi.coneTraceSteps = coneTraceSteps.Value;
            segi.coneLength = coneLength.Value;
            segi.coneWidth = coneWidth.Value;
            segi.coneTraceBias = coneTraceBias.Value;
            segi.occlusionStrength = occlusionStrength.Value;
            segi.nearOcclusionStrength = nearOcclusionStrength.Value;
            segi.farOcclusionStrength = farOcclusionStrength.Value;
            segi.farthestOcclusionStrength = farthestOcclusionStrength.Value;
            segi.occlusionPower = occlusionPower.Value;
            segi.nearLightGain = nearLightGain.Value;
            segi.giGain = giGain.Value;
            segi.secondaryBounceGain = secondaryBounceGain.Value;
            segi.secondaryCones = secondaryCones.Value;
            segi.secondaryOcclusionStrength = secondaryOcclusionStrength.Value;

            // Reflection Properties
            segi.doReflections = doReflections.Value;
            segi.reflectionSteps = reflectionSteps.Value;
            segi.reflectionOcclusionPower = reflectionOcclusionPower.Value;
            segi.skyReflectionIntensity = skyReflectionIntensity.Value;

            // Debug Tools
            segi.visualizeSunDepthTexture = visualizeSunDepthTexture.Value;
            segi.visualizeGI = visualizeGI.Value;
            segi.visualizeVoxels = visualizeVoxels.Value;
        }
    }
}
