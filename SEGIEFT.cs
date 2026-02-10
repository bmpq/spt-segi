using EFT.Weather;
using UnityEngine;

using DistantShadow_CachedParameters = DistantShadow.Class613;

namespace tarkin.SEGI.Bep
{
    public class SEGIEFT : MonoBehaviour
    {
        SEGIRenderer segi;

        void Start()
        {
            segi = GetComponent<SEGIRenderer>();
        }

        void Update()
        {
            // Main Configuration
            segi.voxelResolution = SEGIConfig.voxelResolution.Value;
            segi.voxelAA = SEGIConfig.voxelAA.Value;
            segi.innerOcclusionLayers = SEGIConfig.innerOcclusionLayers.Value;
            segi.gaussianMipFilter = SEGIConfig.gaussianMipFilter.Value;
            segi.voxelSpaceSize = SEGIConfig.voxelSpaceSize.Value;
            segi.shadowSpaceSize = SEGIConfig.shadowSpaceSize.Value;
            segi.updateGI = SEGIConfig.updateGI.Value;
            segi.infiniteBounces = SEGIConfig.infiniteBounces.Value;

            // Environment Properties
            segi.softSunlight = SEGIConfig.softSunlight.Value;
            segi.skyColor = SEGIConfig.skyColor.Value;
            segi.skyIntensity = SEGIConfig.skyIntensity.Value;
            segi.sphericalSkylight = SEGIConfig.sphericalSkylight.Value;

            // Tracing Properties
            segi.temporalBlendWeight = SEGIConfig.temporalBlendWeight.Value;
            segi.useBilateralFiltering = SEGIConfig.useBilateralFiltering.Value;
            segi.halfResolution = SEGIConfig.halfResolution.Value;
            segi.stochasticSampling = SEGIConfig.stochasticSampling.Value;
            segi.cones = SEGIConfig.cones.Value;
            segi.coneTraceSteps = SEGIConfig.coneTraceSteps.Value;
            segi.coneLength = SEGIConfig.coneLength.Value;
            segi.coneWidth = SEGIConfig.coneWidth.Value;
            segi.coneTraceBias = SEGIConfig.coneTraceBias.Value;
            segi.occlusionStrength = SEGIConfig.occlusionStrength.Value;
            segi.nearOcclusionStrength = SEGIConfig.nearOcclusionStrength.Value;
            segi.farOcclusionStrength = SEGIConfig.farOcclusionStrength.Value;
            segi.farthestOcclusionStrength = SEGIConfig.farthestOcclusionStrength.Value;
            segi.occlusionPower = SEGIConfig.occlusionPower.Value;
            segi.nearLightGain = SEGIConfig.nearLightGain.Value;
            segi.giGain = SEGIConfig.giGain.Value;
            segi.secondaryBounceGain = SEGIConfig.secondaryBounceGain.Value;
            segi.secondaryCones = SEGIConfig.secondaryCones.Value;
            segi.secondaryOcclusionStrength = SEGIConfig.secondaryOcclusionStrength.Value;

            // Reflection Properties
            segi.doReflections = SEGIConfig.doReflections.Value;
            segi.reflectionSteps = SEGIConfig.reflectionSteps.Value;
            segi.reflectionOcclusionPower = SEGIConfig.reflectionOcclusionPower.Value;
            segi.skyReflectionIntensity = SEGIConfig.skyReflectionIntensity.Value;

            // Debug Tools
            segi.visualizeSunDepthTexture = SEGIConfig.visualizeSunDepthTexture.Value;
            segi.visualizeGI = SEGIConfig.visualizeGI.Value;
            segi.visualizeVoxels = SEGIConfig.visualizeVoxels.Value;
        }
    }
}