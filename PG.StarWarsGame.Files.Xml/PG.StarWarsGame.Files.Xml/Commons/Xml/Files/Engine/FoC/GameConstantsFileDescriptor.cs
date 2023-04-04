// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.Xml.Commons.Xml.Tags.Engine.FoC;

namespace PG.StarWarsGame.Files.Xml.Commons.Xml.Files.Engine.FoC
{
    public sealed class GameConstantsFileDescriptor : AXmlFileDescriptor
    {
        public DefaultDefenseAdjustXmlTagDescriptor DefaultDefenseAdjust { get; } =
            new DefaultDefenseAdjustXmlTagDescriptor();

        public StartingGalacticCameraPositionXmlTagDescriptor StartingGalacticCameraPosition { get; } =
            new StartingGalacticCameraPositionXmlTagDescriptor();

        public CameraStopLeftXmlTagDescriptor CameraStopLeft { get; } = new CameraStopLeftXmlTagDescriptor();
        public CameraStopRightXmlTagDescriptor CameraStopRight { get; } = new CameraStopRightXmlTagDescriptor();
        public CameraZPositionXmlTagDescriptor CameraZPosition { get; } = new CameraZPositionXmlTagDescriptor();

        public FleetMovementLineTextureNameXmlTagDescriptor FleetMovementLineTextureName { get; } =
            new FleetMovementLineTextureNameXmlTagDescriptor();

        public FleetHyperspaceBandTextureNameXmlTagDescriptor FleetHyperspaceBandTextureName { get; } =
            new FleetHyperspaceBandTextureNameXmlTagDescriptor();

        public LoopWaypointLineTextureNameXmlTagDescriptor LoopWaypointLineTextureName { get; } =
            new LoopWaypointLineTextureNameXmlTagDescriptor();

        public WaypointLineTextureNameXmlTagDescriptor WaypointLineTextureName { get; } =
            new WaypointLineTextureNameXmlTagDescriptor();

        public WaypointFlagModelNameXmlTagDescriptor WaypointFlagModelName { get; } =
            new WaypointFlagModelNameXmlTagDescriptor();

        public FleetMaintenanceUpdateDelaySecondsXmlTagDescriptor FleetMaintenanceUpdateDelaySeconds { get; } =
            new FleetMaintenanceUpdateDelaySecondsXmlTagDescriptor();

        public PayAsYouGoXmlTagDescriptor PayAsYouGo { get; } = new PayAsYouGoXmlTagDescriptor();
        public UseNeutralUIColorXmlTagDescriptor UseNeutralUIColor { get; } = new UseNeutralUIColorXmlTagDescriptor();
        public NeutralUIColorXmlTagDescriptor NeutralUIColor { get; } = new NeutralUIColorXmlTagDescriptor();
        public PlayerColorXmlTagDescriptor PlayerColor { get; } = new PlayerColorXmlTagDescriptor();
        public EnemyColorXmlTagDescriptor EnemyColor { get; } = new EnemyColorXmlTagDescriptor();

        public SpaceAutoResolveDelaySecondsXmlTagDescriptor SpaceAutoResolveDelaySeconds { get; } =
            new SpaceAutoResolveDelaySecondsXmlTagDescriptor();

        public LandAutoResolveDelaySecondsXmlTagDescriptor LandAutoResolveDelaySeconds { get; } =
            new LandAutoResolveDelaySecondsXmlTagDescriptor();

        public ProductionSpeedFactorXmlTagDescriptor ProductionSpeedFactor { get; } =
            new ProductionSpeedFactorXmlTagDescriptor();

        public PoliticalControlChangeTimeSecondsXmlTagDescriptor PoliticalControlChangeTimeSeconds { get; } =
            new PoliticalControlChangeTimeSecondsXmlTagDescriptor();

        public PoliticalIncomeCurveXmlTagDescriptor PoliticalIncomeCurve { get; } =
            new PoliticalIncomeCurveXmlTagDescriptor();

        public ProgressiveTaxationXmlTagDescriptor ProgressiveTaxation { get; } =
            new ProgressiveTaxationXmlTagDescriptor();

        public IncomeRedistributionXmlTagDescriptor IncomeRedistribution { get; } =
            new IncomeRedistributionXmlTagDescriptor();

        public CreditCapPerPlanetXmlTagDescriptor CreditCapPerPlanet { get; } =
            new CreditCapPerPlanetXmlTagDescriptor();

        public FiscalCycleTimeInSecsXmlTagDescriptor FiscalCycleTimeInSecs { get; } =
            new FiscalCycleTimeInSecsXmlTagDescriptor();

        public MediumCoinStackSizeXmlTagDescriptor MediumCoinStackSize { get; } =
            new MediumCoinStackSizeXmlTagDescriptor();

        public LargeCoinStackSizeXmlTagDescriptor LargeCoinStackSize { get; } =
            new LargeCoinStackSizeXmlTagDescriptor();

        public BlackMarketIncomeMultMinXmlTagDescriptor BlackMarketIncomeMultMin { get; } =
            new BlackMarketIncomeMultMinXmlTagDescriptor();

        public BlackMarketIncomeMultMaxXmlTagDescriptor BlackMarketIncomeMultMax { get; } =
            new BlackMarketIncomeMultMaxXmlTagDescriptor();

        public NumStructuresForMediumPlanetNameXmlTagDescriptor NumStructuresForMediumPlanetName { get; } =
            new NumStructuresForMediumPlanetNameXmlTagDescriptor();

        public NumStructuresForLargePlanetNameXmlTagDescriptor NumStructuresForLargePlanetName { get; } =
            new NumStructuresForLargePlanetNameXmlTagDescriptor();

        public MaximumPoliticalControlXmlTagDescriptor MaximumPoliticalControl { get; } =
            new MaximumPoliticalControlXmlTagDescriptor();

        public MaximumStarbaseLevelXmlTagDescriptor MaximumStarbaseLevel { get; } =
            new MaximumStarbaseLevelXmlTagDescriptor();

        public MaximumGroundbaseLevelXmlTagDescriptor MaximumGroundbaseLevel { get; } =
            new MaximumGroundbaseLevelXmlTagDescriptor();

        public MaximumSpecialStructuresXmlTagDescriptor MaximumSpecialStructures { get; } =
            new MaximumSpecialStructuresXmlTagDescriptor();

        public MaximumSpecialStructuresLandXmlTagDescriptor MaximumSpecialStructuresLand { get; } =
            new MaximumSpecialStructuresLandXmlTagDescriptor();

        public MaximumSpecialStructuresSpaceXmlTagDescriptor MaximumSpecialStructuresSpace { get; } =
            new MaximumSpecialStructuresSpaceXmlTagDescriptor();

        public MaximumFleetMovementDistanceXmlTagDescriptor MaximumFleetMovementDistance { get; } =
            new MaximumFleetMovementDistanceXmlTagDescriptor();

        public ShipNameTextFilesXmlTagDescriptor ShipNameTextFiles { get; } = new ShipNameTextFilesXmlTagDescriptor();

        public TradeRouteMovementFactorXmlTagDescriptor TradeRouteMovementFactor { get; } =
            new TradeRouteMovementFactorXmlTagDescriptor();

        public GMCInitialPitchAngleDegreesXmlTagDescriptor GMCInitialPitchAngleDegrees { get; } =
            new GMCInitialPitchAngleDegreesXmlTagDescriptor();

        public GMCZoomedPitchAngleDegreesXmlTagDescriptor GMCZoomedPitchAngleDegrees { get; } =
            new GMCZoomedPitchAngleDegreesXmlTagDescriptor();

        public GMCInitialPullbackDistanceXmlTagDescriptor GMCInitialPullbackDistance { get; } =
            new GMCInitialPullbackDistanceXmlTagDescriptor();

        public GMCZoomedPullbackPlanetRadiusFractionXmlTagDescriptor GMCZoomedPullbackPlanetRadiusFraction { get; } =
            new GMCZoomedPullbackPlanetRadiusFractionXmlTagDescriptor();

        public GMCZoomedPositionOffsetPlanetRadiusFractionsXmlTagDescriptor
            GMCZoomedPositionOffsetPlanetRadiusFractions { get; } =
            new GMCZoomedPositionOffsetPlanetRadiusFractionsXmlTagDescriptor();

        public GMCZoomTimeXmlTagDescriptor GMCZoomTime { get; } = new GMCZoomTimeXmlTagDescriptor();
        public GMCBattleZoomTimeXmlTagDescriptor GMCBattleZoomTime { get; } = new GMCBattleZoomTimeXmlTagDescriptor();
        public GMCBattleFadeTimeXmlTagDescriptor GMCBattleFadeTime { get; } = new GMCBattleFadeTimeXmlTagDescriptor();

        public GalacticRightButtonScrollSpeedFactorXmlTagDescriptor GalacticRightButtonScrollSpeedFactor { get; } =
            new GalacticRightButtonScrollSpeedFactorXmlTagDescriptor();

        public GalacticScrollPlaneXmlTagDescriptor GalacticScrollPlane { get; } =
            new GalacticScrollPlaneXmlTagDescriptor();

        public PushScrollSpeedModifierXmlTagDescriptor PushScrollSpeedModifier { get; } =
            new PushScrollSpeedModifierXmlTagDescriptor();

        public ScrollDecelerationFactorXmlTagDescriptor ScrollDecelerationFactor { get; } =
            new ScrollDecelerationFactorXmlTagDescriptor();

        public ScrollAccelerationFactorXmlTagDescriptor ScrollAccelerationFactor { get; } =
            new ScrollAccelerationFactorXmlTagDescriptor();

        public GUIMoveCommandAckEffectXmlTagDescriptor GUIMoveCommandAckEffect { get; } =
            new GUIMoveCommandAckEffectXmlTagDescriptor();

        public GUIDoubleClickMoveCommandAckEffectXmlTagDescriptor GUIDoubleClickMoveCommandAckEffect { get; } =
            new GUIDoubleClickMoveCommandAckEffectXmlTagDescriptor();

        public GUIAttackMoveCommandAckEffectXmlTagDescriptor GUIAttackMoveCommandAckEffect { get; } =
            new GUIAttackMoveCommandAckEffectXmlTagDescriptor();

        public GUIGuardMoveCommandAckEffectXmlTagDescriptor GUIGuardMoveCommandAckEffect { get; } =
            new GUIGuardMoveCommandAckEffectXmlTagDescriptor();

        public GUIAttackMovementClickRadarEventNameXmlTagDescriptor GUIAttackMovementClickRadarEventName { get; } =
            new GUIAttackMovementClickRadarEventNameXmlTagDescriptor();

        public GUIMovementClickRadarEventNameXmlTagDescriptor GUIMovementClickRadarEventName { get; } =
            new GUIMovementClickRadarEventNameXmlTagDescriptor();

        public GUIMovementDoubleClickRadarEventNameXmlTagDescriptor GUIMovementDoubleClickRadarEventName { get; } =
            new GUIMovementDoubleClickRadarEventNameXmlTagDescriptor();

        public GUIMoveAcknowledgeScaleLandXmlTagDescriptor GUIMoveAcknowledgeScaleLand { get; } =
            new GUIMoveAcknowledgeScaleLandXmlTagDescriptor();

        public GUIMoveAcknowledgeScaleSpaceXmlTagDescriptor GUIMoveAcknowledgeScaleSpace { get; } =
            new GUIMoveAcknowledgeScaleSpaceXmlTagDescriptor();

        public TooltipDelayXmlTagDescriptor TooltipDelay { get; } = new TooltipDelayXmlTagDescriptor();
        public TextRevealRateXmlTagDescriptor TextRevealRate { get; } = new TextRevealRateXmlTagDescriptor();
        public EncyclopediaDelayXmlTagDescriptor EncyclopediaDelay { get; } = new EncyclopediaDelayXmlTagDescriptor();

        public LongEncyclopediaDelayXmlTagDescriptor LongEncyclopediaDelay { get; } =
            new LongEncyclopediaDelayXmlTagDescriptor();

        public MapPreviewImageSizeXmlTagDescriptor MapPreviewImageSize { get; } =
            new MapPreviewImageSizeXmlTagDescriptor();

        public JapaneseSTLinePercentXmlTagDescriptor JapaneseSTLinePercent { get; } =
            new JapaneseSTLinePercentXmlTagDescriptor();

        public ElevatedVulnerabilityDurationXmlTagDescriptor ElevatedVulnerabilityDuration { get; } =
            new ElevatedVulnerabilityDurationXmlTagDescriptor();

        public ElevatedVulnerabilityFactorXmlTagDescriptor ElevatedVulnerabilityFactor { get; } =
            new ElevatedVulnerabilityFactorXmlTagDescriptor();

        public SpaceElevatedVulnerabilityDurationXmlTagDescriptor SpaceElevatedVulnerabilityDuration { get; } =
            new SpaceElevatedVulnerabilityDurationXmlTagDescriptor();

        public SpaceElevatedVulnerabilityFactorXmlTagDescriptor SpaceElevatedVulnerabilityFactor { get; } =
            new SpaceElevatedVulnerabilityFactorXmlTagDescriptor();

        public TelekinesisHoverHeightXmlTagDescriptor TelekinesisHoverHeight { get; } =
            new TelekinesisHoverHeightXmlTagDescriptor();

        public TelekinesisTransitionTimeXmlTagDescriptor TelekinesisTransitionTime { get; } =
            new TelekinesisTransitionTimeXmlTagDescriptor();

        public TelekinesisWobbleCycleTimeXmlTagDescriptor TelekinesisWobbleCycleTime { get; } =
            new TelekinesisWobbleCycleTimeXmlTagDescriptor();

        public TelekinesisWobbleFadeTimeXmlTagDescriptor TelekinesisWobbleFadeTime { get; } =
            new TelekinesisWobbleFadeTimeXmlTagDescriptor();

        public TelekinesisMaxWobbleAngleXmlTagDescriptor TelekinesisMaxWobbleAngle { get; } =
            new TelekinesisMaxWobbleAngleXmlTagDescriptor();

        public TelekinesisMaxBobHeightXmlTagDescriptor TelekinesisMaxBobHeight { get; } =
            new TelekinesisMaxBobHeightXmlTagDescriptor();

        public EarthquakeTransitionTimeXmlTagDescriptor EarthquakeTransitionTime { get; } =
            new EarthquakeTransitionTimeXmlTagDescriptor();

        public EarthquakeShakeSpeedXmlTagDescriptor EarthquakeShakeSpeed { get; } =
            new EarthquakeShakeSpeedXmlTagDescriptor();

        public EarthquakeShakeMagnitudeXmlTagDescriptor EarthquakeShakeMagnitude { get; } =
            new EarthquakeShakeMagnitudeXmlTagDescriptor();

        public AIUsesFogOfWarGalacticXmlTagDescriptor AIUsesFogOfWarGalactic { get; } =
            new AIUsesFogOfWarGalacticXmlTagDescriptor();

        public AIUsesFogOfWarSpaceXmlTagDescriptor AIUsesFogOfWarSpace { get; } =
            new AIUsesFogOfWarSpaceXmlTagDescriptor();

        public AIUsesFogOfWarLandXmlTagDescriptor AIUsesFogOfWarLand { get; } =
            new AIUsesFogOfWarLandXmlTagDescriptor();

        public SetupPhaseEnabledXmlTagDescriptor SetupPhaseEnabled { get; } = new SetupPhaseEnabledXmlTagDescriptor();

        public AlwaysBypassAutoResolveXmlTagDescriptor AlwaysBypassAutoResolve { get; } =
            new AlwaysBypassAutoResolveXmlTagDescriptor();

        public AutomaticAutoResolveXmlTagDescriptor AutomaticAutoResolve { get; } =
            new AutomaticAutoResolveXmlTagDescriptor();

        public AutoResolveVoteDefaultToTacticalXmlTagDescriptor AutoResolveVoteDefaultToTactical { get; } =
            new AutoResolveVoteDefaultToTacticalXmlTagDescriptor();

        public AutoResolveVoteDefaultTimeOutXmlTagDescriptor AutoResolveVoteDefaultTimeOut { get; } =
            new AutoResolveVoteDefaultTimeOutXmlTagDescriptor();

        public PlayModeSwitchMoviesXmlTagDescriptor PlayModeSwitchMovies { get; } =
            new PlayModeSwitchMoviesXmlTagDescriptor();

        public ShieldRechargeIntervalInSecsXmlTagDescriptor ShieldRechargeIntervalInSecs { get; } =
            new ShieldRechargeIntervalInSecsXmlTagDescriptor();

        public EnergyRechargeIntervalInSecsXmlTagDescriptor EnergyRechargeIntervalInSecs { get; } =
            new EnergyRechargeIntervalInSecsXmlTagDescriptor();

        public EnergyToShieldExchangeRateXmlTagDescriptor EnergyToShieldExchangeRate { get; } =
            new EnergyToShieldExchangeRateXmlTagDescriptor();

        public MaxInfluenceTransitionAlignmentBonusXmlTagDescriptor MaxInfluenceTransitionAlignmentBonus { get; } =
            new MaxInfluenceTransitionAlignmentBonusXmlTagDescriptor();

        public MaxInfluenceTransitionAlignmentPenaltyXmlTagDescriptor MaxInfluenceTransitionAlignmentPenalty { get; } =
            new MaxInfluenceTransitionAlignmentPenaltyXmlTagDescriptor();

        public MaxCreditIncomeAlignmentBonusXmlTagDescriptor MaxCreditIncomeAlignmentBonus { get; } =
            new MaxCreditIncomeAlignmentBonusXmlTagDescriptor();

        public MaxCreditIncomeAlignmentPenaltyXmlTagDescriptor MaxCreditIncomeAlignmentPenalty { get; } =
            new MaxCreditIncomeAlignmentPenaltyXmlTagDescriptor();

        public MaxCombatAccuracyAlignmentBonusXmlTagDescriptor MaxCombatAccuracyAlignmentBonus { get; } =
            new MaxCombatAccuracyAlignmentBonusXmlTagDescriptor();

        public MaxCombatDamageAlignmentBonusXmlTagDescriptor MaxCombatDamageAlignmentBonus { get; } =
            new MaxCombatDamageAlignmentBonusXmlTagDescriptor();

        public MaxCombatSensorRangeAlignmentBonusXmlTagDescriptor MaxCombatSensorRangeAlignmentBonus { get; } =
            new MaxCombatSensorRangeAlignmentBonusXmlTagDescriptor();

        public MaxGalacticZoomDistanceXmlTagDescriptor MaxGalacticZoomDistance { get; } =
            new MaxGalacticZoomDistanceXmlTagDescriptor();

        public MinGalacticZoomSpeedXmlTagDescriptor MinGalacticZoomSpeed { get; } =
            new MinGalacticZoomSpeedXmlTagDescriptor();

        public MaxGalacticZoomSpeedXmlTagDescriptor MaxGalacticZoomSpeed { get; } =
            new MaxGalacticZoomSpeedXmlTagDescriptor();

        public GalacticZoomAccelerationXmlTagDescriptor GalacticZoomAcceleration { get; } =
            new GalacticZoomAccelerationXmlTagDescriptor();

        public GalacticZoomLightLevelXmlTagDescriptor GalacticZoomLightLevel { get; } =
            new GalacticZoomLightLevelXmlTagDescriptor();

        public GalacticZoomInLightAngleXmlTagDescriptor GalacticZoomInLightAngle { get; } =
            new GalacticZoomInLightAngleXmlTagDescriptor();

        public GalacticZoomOutLightAngleXmlTagDescriptor GalacticZoomOutLightAngle { get; } =
            new GalacticZoomOutLightAngleXmlTagDescriptor();

        public GalacticZoomInStationOffsetXmlTagDescriptor GalacticZoomInStationOffset { get; } =
            new GalacticZoomInStationOffsetXmlTagDescriptor();

        public GalacticZoomInStationRotationXmlTagDescriptor GalacticZoomInStationRotation { get; } =
            new GalacticZoomInStationRotationXmlTagDescriptor();

        public RandomStoryTriggersXmlTagDescriptor RandomStoryTriggers { get; } =
            new RandomStoryTriggersXmlTagDescriptor();

        public RandomStoryMaxTriggersXmlTagDescriptor RandomStoryMaxTriggers { get; } =
            new RandomStoryMaxTriggersXmlTagDescriptor();

        public RandomStoryRebelConstructionXmlTagDescriptor RandomStoryRebelConstruction { get; } =
            new RandomStoryRebelConstructionXmlTagDescriptor();

        public RandomStoryEmpireConstructionXmlTagDescriptor RandomStoryEmpireConstruction { get; } =
            new RandomStoryEmpireConstructionXmlTagDescriptor();

        public RandomStoryRebelDestroyXmlTagDescriptor RandomStoryRebelDestroy { get; } =
            new RandomStoryRebelDestroyXmlTagDescriptor();

        public RandomStoryEmpireDestroyXmlTagDescriptor RandomStoryEmpireDestroy { get; } =
            new RandomStoryEmpireDestroyXmlTagDescriptor();

        public RandomStoryRewardsXmlTagDescriptor RandomStoryRewards { get; } =
            new RandomStoryRewardsXmlTagDescriptor();

        public RandomStoryRewardRebelBuildableXmlTagDescriptor RandomStoryRewardRebelBuildable { get; } =
            new RandomStoryRewardRebelBuildableXmlTagDescriptor();

        public RandomStoryRewardEmpireBuildableXmlTagDescriptor RandomStoryRewardEmpireBuildable { get; } =
            new RandomStoryRewardEmpireBuildableXmlTagDescriptor();

        public RandomStoryRewardRebelUnitXmlTagDescriptor RandomStoryRewardRebelUnit { get; } =
            new RandomStoryRewardRebelUnitXmlTagDescriptor();

        public RandomStoryRewardEmpireUnitXmlTagDescriptor RandomStoryRewardEmpireUnit { get; } =
            new RandomStoryRewardEmpireUnitXmlTagDescriptor();

        public MaxGroundForcesOnPlanetXmlTagDescriptor MaxGroundForcesOnPlanet { get; } =
            new MaxGroundForcesOnPlanetXmlTagDescriptor();

        public AllowReinforcementPercentageNormalizedXmlTagDescriptor AllowReinforcementPercentageNormalized { get; } =
            new AllowReinforcementPercentageNormalizedXmlTagDescriptor();

        public TacticalEdgeScrollRegionXmlTagDescriptor TacticalEdgeScrollRegion { get; } =
            new TacticalEdgeScrollRegionXmlTagDescriptor();

        public TacticalMaxScrollSpeedXmlTagDescriptor TacticalMaxScrollSpeed { get; } =
            new TacticalMaxScrollSpeedXmlTagDescriptor();

        public TacticalMinScrollSpeedXmlTagDescriptor TacticalMinScrollSpeed { get; } =
            new TacticalMinScrollSpeedXmlTagDescriptor();

        public TacticalOffscreenScrollRegionXmlTagDescriptor TacticalOffscreenScrollRegion { get; } =
            new TacticalOffscreenScrollRegionXmlTagDescriptor();

        public StrategicEdgeScrollRegionXmlTagDescriptor StrategicEdgeScrollRegion { get; } =
            new StrategicEdgeScrollRegionXmlTagDescriptor();

        public StrategicMaxScrollSpeedXmlTagDescriptor StrategicMaxScrollSpeed { get; } =
            new StrategicMaxScrollSpeedXmlTagDescriptor();

        public StrategicMinScrollSpeedXmlTagDescriptor StrategicMinScrollSpeed { get; } =
            new StrategicMinScrollSpeedXmlTagDescriptor();

        public StrategicOffscreenScrollRegionXmlTagDescriptor StrategicOffscreenScrollRegion { get; } =
            new StrategicOffscreenScrollRegionXmlTagDescriptor();

        public HardPointTargetReticleEnemyScreenSizeXmlTagDescriptor HardPointTargetReticleEnemyScreenSize { get; } =
            new HardPointTargetReticleEnemyScreenSizeXmlTagDescriptor();

        public HardPointTargetReticleFriendlyScreenSizeXmlTagDescriptor
            HardPointTargetReticleFriendlyScreenSize { get; } =
            new HardPointTargetReticleFriendlyScreenSizeXmlTagDescriptor();

        public HardPointTargetReticleEnemyTextureXmlTagDescriptor HardPointTargetReticleEnemyTexture { get; } =
            new HardPointTargetReticleEnemyTextureXmlTagDescriptor();

        public HardPointTargetReticleEnemyTrackedTextureXmlTagDescriptor
            HardPointTargetReticleEnemyTrackedTexture { get; } =
            new HardPointTargetReticleEnemyTrackedTextureXmlTagDescriptor();

        public HardPointTargetReticleFriendlyTextureXmlTagDescriptor HardPointTargetReticleFriendlyTexture { get; } =
            new HardPointTargetReticleFriendlyTextureXmlTagDescriptor();

        public HardPointTargetReticleFriendlyTrackedTextureXmlTagDescriptor
            HardPointTargetReticleFriendlyTrackedTexture { get; } =
            new HardPointTargetReticleFriendlyTrackedTextureXmlTagDescriptor();

        public HardPointTargetReticleFriendlyRepairingTextureXmlTagDescriptor
            HardPointTargetReticleFriendlyRepairingTexture { get; } =
            new HardPointTargetReticleFriendlyRepairingTextureXmlTagDescriptor();

        public HardPointTargetReticleFriendlyDisabledTextureXmlTagDescriptor
            HardPointTargetReticleFriendlyDisabledTexture { get; } =
            new HardPointTargetReticleFriendlyDisabledTextureXmlTagDescriptor();

        public HardPointTargetReticleFriendlyDisabledTrackedTextureXmlTagDescriptor
            HardPointTargetReticleFriendlyDisabledTrackedTexture { get; } =
            new HardPointTargetReticleFriendlyDisabledTrackedTextureXmlTagDescriptor();

        public DebugHotKeyLoadMapXmlTagDescriptor DebugHotKeyLoadMap { get; } =
            new DebugHotKeyLoadMapXmlTagDescriptor();

        public DebugHotKeyLoadMapScriptXmlTagDescriptor DebugHotKeyLoadMapScript { get; } =
            new DebugHotKeyLoadMapScriptXmlTagDescriptor();

        public DebugHotKeyLoadCampaignXmlTagDescriptor DebugHotKeyLoadCampaign { get; } =
            new DebugHotKeyLoadCampaignXmlTagDescriptor();

        public GameScoringScriptNameXmlTagDescriptor GameScoringScriptName { get; } =
            new GameScoringScriptNameXmlTagDescriptor();

        public ShowUnitAIPlanAttachmentXmlTagDescriptor ShowUnitAIPlanAttachment { get; } =
            new ShowUnitAIPlanAttachmentXmlTagDescriptor();

        public AISpaceEvaluatorRegionSizeXmlTagDescriptor AISpaceEvaluatorRegionSize { get; } =
            new AISpaceEvaluatorRegionSizeXmlTagDescriptor();

        public AILandEvaluatorRegionSizeXmlTagDescriptor AILandEvaluatorRegionSize { get; } =
            new AILandEvaluatorRegionSizeXmlTagDescriptor();

        public AISpaceThreatDistanceFactorXmlTagDescriptor AISpaceThreatDistanceFactor { get; } =
            new AISpaceThreatDistanceFactorXmlTagDescriptor();

        public AILandThreatDistanceFactorXmlTagDescriptor AILandThreatDistanceFactor { get; } =
            new AILandThreatDistanceFactorXmlTagDescriptor();

        public AISpaceThreatTurnRateFactorXmlTagDescriptor AISpaceThreatTurnRateFactor { get; } =
            new AISpaceThreatTurnRateFactorXmlTagDescriptor();

        public AILandThreatTurnRateFactorXmlTagDescriptor AILandThreatTurnRateFactor { get; } =
            new AILandThreatTurnRateFactorXmlTagDescriptor();

        public AISpaceThreatLookAheadTimeXmlTagDescriptor AISpaceThreatLookAheadTime { get; } =
            new AISpaceThreatLookAheadTimeXmlTagDescriptor();

        public AILandThreatLookAheadTimeXmlTagDescriptor AILandThreatLookAheadTime { get; } =
            new AILandThreatLookAheadTimeXmlTagDescriptor();

        public AISpaceAreaThreatScaleFactorXmlTagDescriptor AISpaceAreaThreatScaleFactor { get; } =
            new AISpaceAreaThreatScaleFactorXmlTagDescriptor();

        public AILandAreaThreatScaleFactorXmlTagDescriptor AILandAreaThreatScaleFactor { get; } =
            new AILandAreaThreatScaleFactorXmlTagDescriptor();

        public AIFogCellsPerThreatCellXmlTagDescriptor AIFogCellsPerThreatCell { get; } =
            new AIFogCellsPerThreatCellXmlTagDescriptor();

        public AISpaceThreatDecayStepXmlTagDescriptor AISpaceThreatDecayStep { get; } =
            new AISpaceThreatDecayStepXmlTagDescriptor();

        public AIBuildTaskReservationSecondsXmlTagDescriptor AIBuildTaskReservationSeconds { get; } =
            new AIBuildTaskReservationSecondsXmlTagDescriptor();

        public AILandThreatRangeCapXmlTagDescriptor AILandThreatRangeCap { get; } =
            new AILandThreatRangeCapXmlTagDescriptor();

        public AISpaceThreatRangeCapXmlTagDescriptor AISpaceThreatRangeCap { get; } =
            new AISpaceThreatRangeCapXmlTagDescriptor();

        public SpaceFOWColorXmlTagDescriptor SpaceFOWColor { get; } = new SpaceFOWColorXmlTagDescriptor();
        public LandFOWColorXmlTagDescriptor LandFOWColor { get; } = new LandFOWColorXmlTagDescriptor();

        public SpaceReinforceFOWColorXmlTagDescriptor SpaceReinforceFOWColor { get; } =
            new SpaceReinforceFOWColorXmlTagDescriptor();

        public SetupPhaseFOWColorXmlTagDescriptor SetupPhaseFOWColor { get; } =
            new SetupPhaseFOWColorXmlTagDescriptor();

        public SetupPhaseInvalidDragColorXmlTagDescriptor SetupPhaseInvalidDragColor { get; } =
            new SetupPhaseInvalidDragColorXmlTagDescriptor();

        public SetupPhaseCountdownSecondsXmlTagDescriptor SetupPhaseCountdownSeconds { get; } =
            new SetupPhaseCountdownSecondsXmlTagDescriptor();

        public SpaceFOWHeightXmlTagDescriptor SpaceFOWHeight { get; } = new SpaceFOWHeightXmlTagDescriptor();

        public GoodGroundColorTintXmlTagDescriptor GoodGroundColorTint { get; } =
            new GoodGroundColorTintXmlTagDescriptor();

        public RetreatColorTintXmlTagDescriptor RetreatColorTint { get; } = new RetreatColorTintXmlTagDescriptor();

        public HighGroundColorTintXmlTagDescriptor HighGroundColorTint { get; } =
            new HighGroundColorTintXmlTagDescriptor();

        public SlowGroundColorTintXmlTagDescriptor SlowGroundColorTint { get; } =
            new SlowGroundColorTintXmlTagDescriptor();

        public LavaGroundColorTintXmlTagDescriptor LavaGroundColorTint { get; } =
            new LavaGroundColorTintXmlTagDescriptor();

        public InfantryGroundColorTintXmlTagDescriptor InfantryGroundColorTint { get; } =
            new InfantryGroundColorTintXmlTagDescriptor();

        public DesiredLandFOWCellSizeXmlTagDescriptor DesiredLandFOWCellSize { get; } =
            new DesiredLandFOWCellSizeXmlTagDescriptor();

        public DesiredSpaceFOWCellSizeXmlTagDescriptor DesiredSpaceFOWCellSize { get; } =
            new DesiredSpaceFOWCellSizeXmlTagDescriptor();

        public LandFOWRegrowTimeXmlTagDescriptor LandFOWRegrowTime { get; } = new LandFOWRegrowTimeXmlTagDescriptor();

        public SpaceFOWRegrowTimeXmlTagDescriptor SpaceFOWRegrowTime { get; } =
            new SpaceFOWRegrowTimeXmlTagDescriptor();

        public SpaceReinforceFeedbackOnlyWhileDraggingXmlTagDescriptor SpaceReinforceFeedbackOnlyWhileDragging { get; }
            = new SpaceReinforceFeedbackOnlyWhileDraggingXmlTagDescriptor();

        public WaterRenderTargetResolutionXmlTagDescriptor WaterRenderTargetResolution { get; } =
            new WaterRenderTargetResolutionXmlTagDescriptor();

        public WaterClipPlaneOffsetXmlTagDescriptor WaterClipPlaneOffset { get; } =
            new WaterClipPlaneOffsetXmlTagDescriptor();

        public OcclusionSilhouettesEnabledXmlTagDescriptor OcclusionSilhouettesEnabled { get; } =
            new OcclusionSilhouettesEnabledXmlTagDescriptor();

        public LaserBeamZScaleFactorXmlTagDescriptor LaserBeamZScaleFactor { get; } =
            new LaserBeamZScaleFactorXmlTagDescriptor();

        public LaserKiteZScaleFactorXmlTagDescriptor LaserKiteZScaleFactor { get; } =
            new LaserKiteZScaleFactorXmlTagDescriptor();

        public MouseOverHighlightScaleXmlTagDescriptor MouseOverHighlightScale { get; } =
            new MouseOverHighlightScaleXmlTagDescriptor();

        public DefaultHeroRespawnTimeXmlTagDescriptor DefaultHeroRespawnTime { get; } =
            new DefaultHeroRespawnTimeXmlTagDescriptor();

        public MinimumDragDistanceXmlTagDescriptor MinimumDragDistance { get; } =
            new MinimumDragDistanceXmlTagDescriptor();

        public MinimumDragSelectDistanceXmlTagDescriptor MinimumDragSelectDistance { get; } =
            new MinimumDragSelectDistanceXmlTagDescriptor();

        public SpacePathfindMaxExpansionsXmlTagDescriptor SpacePathfindMaxExpansions { get; } =
            new SpacePathfindMaxExpansionsXmlTagDescriptor();

        public CurrentPathCostCoefficientSpaceXmlTagDescriptor CurrentPathCostCoefficientSpace { get; } =
            new CurrentPathCostCoefficientSpaceXmlTagDescriptor();

        public SpacePathfindFrameDelayDeltaXmlTagDescriptor SpacePathfindFrameDelayDelta { get; } =
            new SpacePathfindFrameDelayDeltaXmlTagDescriptor();

        public SpacePathFailureDistanceCutoffCoefficientXmlTagDescriptor
            SpacePathFailureDistanceCutoffCoefficient { get; } =
            new SpacePathFailureDistanceCutoffCoefficientXmlTagDescriptor();

        public SpacePathFailureMaxExpansionsCoefficientXmlTagDescriptor
            SpacePathFailureMaxExpansionsCoefficient { get; } =
            new SpacePathFailureMaxExpansionsCoefficientXmlTagDescriptor();

        public SpacePathFailureRotationExpansionIncrementXmlTagDescriptor
            SpacePathFailureRotationExpansionIncrement { get; } =
            new SpacePathFailureRotationExpansionIncrementXmlTagDescriptor();

        public SpacePathFailureForwardExpansionIncrementXmlTagDescriptor
            SpacePathFailureForwardExpansionIncrement { get; } =
            new SpacePathFailureForwardExpansionIncrementXmlTagDescriptor();

        public SpacePathingTriesXmlTagDescriptor SpacePathingTries { get; } = new SpacePathingTriesXmlTagDescriptor();

        public SpaceStaticObstacleAvoidanceBonusDistanceXmlTagDescriptor
            SpaceStaticObstacleAvoidanceBonusDistance { get; } =
            new SpaceStaticObstacleAvoidanceBonusDistanceXmlTagDescriptor();

        public MinObstacleCostSpaceXmlTagDescriptor MinObstacleCostSpace { get; } =
            new MinObstacleCostSpaceXmlTagDescriptor();

        public MaxObstacleCostSpaceXmlTagDescriptor MaxObstacleCostSpace { get; } =
            new MaxObstacleCostSpaceXmlTagDescriptor();

        public ObstacleAreaOverlapForMaxSpaceXmlTagDescriptor ObstacleAreaOverlapForMaxSpace { get; } =
            new ObstacleAreaOverlapForMaxSpaceXmlTagDescriptor();

        public XYExpansionDistanceSpaceXmlTagDescriptor XYExpansionDistanceSpace { get; } =
            new XYExpansionDistanceSpaceXmlTagDescriptor();

        public MaxRotationsSpaceXmlTagDescriptor MaxRotationsSpace { get; } = new MaxRotationsSpaceXmlTagDescriptor();

        public MatchFacingDeltaSpaceXmlTagDescriptor MatchFacingDeltaSpace { get; } =
            new MatchFacingDeltaSpaceXmlTagDescriptor();

        public OccupationRadiusCoefficientSpaceXmlTagDescriptor OccupationRadiusCoefficientSpace { get; } =
            new OccupationRadiusCoefficientSpaceXmlTagDescriptor();

        public DestinationSearchRadiusIncrementSpaceXmlTagDescriptor DestinationSearchRadiusIncrementSpace { get; } =
            new DestinationSearchRadiusIncrementSpaceXmlTagDescriptor();

        public UseLinearCollisionChecksXmlTagDescriptor UseLinearCollisionChecks { get; } =
            new UseLinearCollisionChecksXmlTagDescriptor();

        public WaitOperatorCostCoefficientXmlTagDescriptor WaitOperatorCostCoefficient { get; } =
            new WaitOperatorCostCoefficientXmlTagDescriptor();

        public WaitOperatorBaseFrameTimeXmlTagDescriptor WaitOperatorBaseFrameTime { get; } =
            new WaitOperatorBaseFrameTimeXmlTagDescriptor();

        public WaitOperatorSpeedCoefficientXmlTagDescriptor WaitOperatorSpeedCoefficient { get; } =
            new WaitOperatorSpeedCoefficientXmlTagDescriptor();

        public LandWaitOperatorSpeedCoefficientXmlTagDescriptor LandWaitOperatorSpeedCoefficient { get; } =
            new LandWaitOperatorSpeedCoefficientXmlTagDescriptor();

        public SpaceLocomotorFacingLookaheadAccXmlTagDescriptor SpaceLocomotorFacingLookaheadAcc { get; } =
            new SpaceLocomotorFacingLookaheadAccXmlTagDescriptor();

        public FinalFacing180PenaltyXmlTagDescriptor FinalFacing180Penalty { get; } =
            new FinalFacing180PenaltyXmlTagDescriptor();

        public SpecialAlignedOperatorBonusXmlTagDescriptor SpecialAlignedOperatorBonus { get; } =
            new SpecialAlignedOperatorBonusXmlTagDescriptor();

        public ThreatExpansionDistanceXmlTagDescriptor ThreatExpansionDistance { get; } =
            new ThreatExpansionDistanceXmlTagDescriptor();

        public OffMapCostPenaltyXmlTagDescriptor OffMapCostPenalty { get; } = new OffMapCostPenaltyXmlTagDescriptor();

        public MaxLandFormationFormupFramesXmlTagDescriptor MaxLandFormationFormupFrames { get; } =
            new MaxLandFormationFormupFramesXmlTagDescriptor();

        public MovementReevaluationFrameCountXmlTagDescriptor MovementReevaluationFrameCount { get; } =
            new MovementReevaluationFrameCountXmlTagDescriptor();

        public AsteroidFieldDamageXmlTagDescriptor AsteroidFieldDamage { get; } =
            new AsteroidFieldDamageXmlTagDescriptor();

        public AsteroidFieldDamageRateXmlTagDescriptor AsteroidFieldDamageRate { get; } =
            new AsteroidFieldDamageRateXmlTagDescriptor();

        public GUIFlashLevelXmlTagDescriptor GUIFlashLevel { get; } = new GUIFlashLevelXmlTagDescriptor();
        public GUIFlashDurationXmlTagDescriptor GUIFlashDuration { get; } = new GUIFlashDurationXmlTagDescriptor();

        public GUIRapidFlashDurationXmlTagDescriptor GUIRapidFlashDuration { get; } =
            new GUIRapidFlashDurationXmlTagDescriptor();

        public GUICycleSpeedXmlTagDescriptor GUICycleSpeed { get; } = new GUICycleSpeedXmlTagDescriptor();
        public GUIDarkenLevelXmlTagDescriptor GUIDarkenLevel { get; } = new GUIDarkenLevelXmlTagDescriptor();
        public GUICycleColorXmlTagDescriptor GUICycleColor { get; } = new GUICycleColorXmlTagDescriptor();
        public GUIHiliteLevelXmlTagDescriptor GUIHiliteLevel { get; } = new GUIHiliteLevelXmlTagDescriptor();

        public GUIPlanetFlashLevelXmlTagDescriptor GUIPlanetFlashLevel { get; } =
            new GUIPlanetFlashLevelXmlTagDescriptor();

        public GUIPlanetFadeDurationXmlTagDescriptor GUIPlanetFadeDuration { get; } =
            new GUIPlanetFadeDurationXmlTagDescriptor();

        public CBFlashDurationXmlTagDescriptor CBFlashDuration { get; } = new CBFlashDurationXmlTagDescriptor();
        public CBFlashCountXmlTagDescriptor CBFlashCount { get; } = new CBFlashCountXmlTagDescriptor();
        public IconsPerColumnXmlTagDescriptor IconsPerColumn { get; } = new IconsPerColumnXmlTagDescriptor();
        public HintTextColorXmlTagDescriptor HintTextColor { get; } = new HintTextColorXmlTagDescriptor();
        public SystemTextColorXmlTagDescriptor SystemTextColor { get; } = new SystemTextColorXmlTagDescriptor();
        public TaskTextColorXmlTagDescriptor TaskTextColor { get; } = new TaskTextColorXmlTagDescriptor();
        public SpeechTextColorXmlTagDescriptor SpeechTextColor { get; } = new SpeechTextColorXmlTagDescriptor();
        public DroidTextColorXmlTagDescriptor DroidTextColor { get; } = new DroidTextColorXmlTagDescriptor();
        public DroidDateColorXmlTagDescriptor DroidDateColor { get; } = new DroidDateColorXmlTagDescriptor();

        public DroidSeperatorColorXmlTagDescriptor DroidSeperatorColor { get; } =
            new DroidSeperatorColorXmlTagDescriptor();

        public CBMovieOffsetXmlTagDescriptor CBMovieOffset { get; } = new CBMovieOffsetXmlTagDescriptor();
        public CBMovieColorXmlTagDescriptor CBMovieColor { get; } = new CBMovieColorXmlTagDescriptor();
        public GoodSideNameXmlTagDescriptor GoodSideName { get; } = new GoodSideNameXmlTagDescriptor();
        public EvilSideNameXmlTagDescriptor EvilSideName { get; } = new EvilSideNameXmlTagDescriptor();
        public CorruptSideNameXmlTagDescriptor CorruptSideName { get; } = new CorruptSideNameXmlTagDescriptor();

        public GoodSideLeaderNameXmlTagDescriptor GoodSideLeaderName { get; } =
            new GoodSideLeaderNameXmlTagDescriptor();

        public EvilSideLeaderNameXmlTagDescriptor EvilSideLeaderName { get; } =
            new EvilSideLeaderNameXmlTagDescriptor();

        public CorruptSideLeaderNameXmlTagDescriptor CorruptSideLeaderName { get; } =
            new CorruptSideLeaderNameXmlTagDescriptor();

        public EncyclopediaPopulationOffsetXmlTagDescriptor EncyclopediaPopulationOffset { get; } =
            new EncyclopediaPopulationOffsetXmlTagDescriptor();

        public EncyclopediaNameOffsetXmlTagDescriptor EncyclopediaNameOffset { get; } =
            new EncyclopediaNameOffsetXmlTagDescriptor();

        public EncyclopediaCostOffsetXmlTagDescriptor EncyclopediaCostOffset { get; } =
            new EncyclopediaCostOffsetXmlTagDescriptor();

        public EncyclopediaIconXOffsetXmlTagDescriptor EncyclopediaIconXOffset { get; } =
            new EncyclopediaIconXOffsetXmlTagDescriptor();

        public EncyclopediaIconYOffsetXmlTagDescriptor EncyclopediaIconYOffset { get; } =
            new EncyclopediaIconYOffsetXmlTagDescriptor();

        public EncyclopediaClassYOffsetXmlTagDescriptor EncyclopediaClassYOffset { get; } =
            new EncyclopediaClassYOffsetXmlTagDescriptor();

        public EncyclopediaFadeRateXmlTagDescriptor EncyclopediaFadeRate { get; } =
            new EncyclopediaFadeRateXmlTagDescriptor();

        public EncyclopediaMinDisplayTimeXmlTagDescriptor EncyclopediaMinDisplayTime { get; } =
            new EncyclopediaMinDisplayTimeXmlTagDescriptor();

        public MinAccuracyForIconXmlTagDescriptor MinAccuracyForIcon { get; } =
            new MinAccuracyForIconXmlTagDescriptor();

        public MinSightRangeForIconXmlTagDescriptor MinSightRangeForIcon { get; } =
            new MinSightRangeForIconXmlTagDescriptor();

        public LeftQueueTintXmlTagDescriptor LeftQueueTint { get; } = new LeftQueueTintXmlTagDescriptor();
        public RightQueueTintXmlTagDescriptor RightQueueTint { get; } = new RightQueueTintXmlTagDescriptor();

        public GUITacticalCountdownTimersScreenXXmlTagDescriptor GUITacticalCountdownTimersScreenX { get; } =
            new GUITacticalCountdownTimersScreenXXmlTagDescriptor();

        public GUITacticalCountdownTimersScreenYXmlTagDescriptor GUITacticalCountdownTimersScreenY { get; } =
            new GUITacticalCountdownTimersScreenYXmlTagDescriptor();

        public GUITacticalCountdownTimersScreenSpacingXmlTagDescriptor GUITacticalCountdownTimersScreenSpacing { get; }
            = new GUITacticalCountdownTimersScreenSpacingXmlTagDescriptor();

        public GUIStrategicCountdownTimersScreenXXmlTagDescriptor GUIStrategicCountdownTimersScreenX { get; } =
            new GUIStrategicCountdownTimersScreenXXmlTagDescriptor();

        public GUIStrategicCountdownTimersScreenYXmlTagDescriptor GUIStrategicCountdownTimersScreenY { get; } =
            new GUIStrategicCountdownTimersScreenYXmlTagDescriptor();

        public GUIStrategicCountdownTimersScreenSpacingXmlTagDescriptor
            GUIStrategicCountdownTimersScreenSpacing { get; } =
            new GUIStrategicCountdownTimersScreenSpacingXmlTagDescriptor();

        public BeaconPlaceDelayXmlTagDescriptor BeaconPlaceDelay { get; } = new BeaconPlaceDelayXmlTagDescriptor();

        public WaypointLineLandDashLengthXmlTagDescriptor WaypointLineLandDashLength { get; } =
            new WaypointLineLandDashLengthXmlTagDescriptor();

        public WaypointLineLandGapLengthXmlTagDescriptor WaypointLineLandGapLength { get; } =
            new WaypointLineLandGapLengthXmlTagDescriptor();

        public WaypointLineLandDashVelocityXmlTagDescriptor WaypointLineLandDashVelocity { get; } =
            new WaypointLineLandDashVelocityXmlTagDescriptor();

        public MaxWaypointsPerPathXmlTagDescriptor MaxWaypointsPerPath { get; } =
            new MaxWaypointsPerPathXmlTagDescriptor();

        public IdleWalkBlendTimeXmlTagDescriptor IdleWalkBlendTime { get; } = new IdleWalkBlendTimeXmlTagDescriptor();

        public CrouchIdleWalkBlendTimeXmlTagDescriptor CrouchIdleWalkBlendTime { get; } =
            new CrouchIdleWalkBlendTimeXmlTagDescriptor();

        public MoveBlendTimeXmlTagDescriptor MoveBlendTime { get; } = new MoveBlendTimeXmlTagDescriptor();

        public CrouchMoveBlendTimeXmlTagDescriptor CrouchMoveBlendTime { get; } =
            new CrouchMoveBlendTimeXmlTagDescriptor();

        public TeamMoveBlendTimeXmlTagDescriptor TeamMoveBlendTime { get; } = new TeamMoveBlendTimeXmlTagDescriptor();

        public TeamCrouchMoveBlendTimeXmlTagDescriptor TeamCrouchMoveBlendTime { get; } =
            new TeamCrouchMoveBlendTimeXmlTagDescriptor();

        public InfantryTurnBlendTimeXmlTagDescriptor InfantryTurnBlendTime { get; } =
            new InfantryTurnBlendTimeXmlTagDescriptor();

        public SpaceObjectTrackingIntervalXmlTagDescriptor SpaceObjectTrackingInterval { get; } =
            new SpaceObjectTrackingIntervalXmlTagDescriptor();

        public LandObjectTrackingIntervalXmlTagDescriptor LandObjectTrackingInterval { get; } =
            new LandObjectTrackingIntervalXmlTagDescriptor();

        public SpaceObjectTrackingTreeCountXmlTagDescriptor SpaceObjectTrackingTreeCount { get; } =
            new SpaceObjectTrackingTreeCountXmlTagDescriptor();

        public LandObjectTrackingTreeCountXmlTagDescriptor LandObjectTrackingTreeCount { get; } =
            new LandObjectTrackingTreeCountXmlTagDescriptor();

        public MinObstacleCostLandXmlTagDescriptor MinObstacleCostLand { get; } =
            new MinObstacleCostLandXmlTagDescriptor();

        public MaxObstacleCostLandXmlTagDescriptor MaxObstacleCostLand { get; } =
            new MaxObstacleCostLandXmlTagDescriptor();

        public XYExpansionDistanceLandXmlTagDescriptor XYExpansionDistanceLand { get; } =
            new XYExpansionDistanceLandXmlTagDescriptor();

        public ShouldDisplayPredictionPathsXmlTagDescriptor ShouldDisplayPredictionPaths { get; } =
            new ShouldDisplayPredictionPathsXmlTagDescriptor();

        public ShouldDisplaySyncedPathsXmlTagDescriptor ShouldDisplaySyncedPaths { get; } =
            new ShouldDisplaySyncedPathsXmlTagDescriptor();

        public SyncedFrameIntervalXmlTagDescriptor SyncedFrameInterval { get; } =
            new SyncedFrameIntervalXmlTagDescriptor();

        public LandPredictionTimeIntervalXmlTagDescriptor LandPredictionTimeInterval { get; } =
            new LandPredictionTimeIntervalXmlTagDescriptor();

        public RepushDistanceXmlTagDescriptor RepushDistance { get; } = new RepushDistanceXmlTagDescriptor();

        public MinLandPredictionDistanceXmlTagDescriptor MinLandPredictionDistance { get; } =
            new MinLandPredictionDistanceXmlTagDescriptor();

        public ShouldSkipLandFormupXmlTagDescriptor ShouldSkipLandFormup { get; } =
            new ShouldSkipLandFormupXmlTagDescriptor();

        public ShouldInfantryTeamsSplitAcrossFormationsXmlTagDescriptor
            ShouldInfantryTeamsSplitAcrossFormations { get; } =
            new ShouldInfantryTeamsSplitAcrossFormationsXmlTagDescriptor();

        public VehicleFormationRecruitmentDistanceXmlTagDescriptor VehicleFormationRecruitmentDistance { get; } =
            new VehicleFormationRecruitmentDistanceXmlTagDescriptor();

        public InfantryFormationRecruitmentDistanceXmlTagDescriptor InfantryFormationRecruitmentDistance { get; } =
            new InfantryFormationRecruitmentDistanceXmlTagDescriptor();

        public FramesPerCollisionCheckXmlTagDescriptor FramesPerCollisionCheck { get; } =
            new FramesPerCollisionCheckXmlTagDescriptor();

        public CloseEnoughAngleForMoveStartXmlTagDescriptor CloseEnoughAngleForMoveStart { get; } =
            new CloseEnoughAngleForMoveStartXmlTagDescriptor();

        public DynamicObstacleOverlapPenaltyXmlTagDescriptor DynamicObstacleOverlapPenalty { get; } =
            new DynamicObstacleOverlapPenaltyXmlTagDescriptor();

        public DynamicAvoidanceRectangleBoundXmlTagDescriptor DynamicAvoidanceRectangleBound { get; } =
            new DynamicAvoidanceRectangleBoundXmlTagDescriptor();

        public ShouldDisplayPotentialPathXmlTagDescriptor ShouldDisplayPotentialPath { get; } =
            new ShouldDisplayPotentialPathXmlTagDescriptor();

        public TurnInPlaceSlowdownCorvetteXmlTagDescriptor TurnInPlaceSlowdownCorvette { get; } =
            new TurnInPlaceSlowdownCorvetteXmlTagDescriptor();

        public TurnInPlaceSlowdownFrigateXmlTagDescriptor TurnInPlaceSlowdownFrigate { get; } =
            new TurnInPlaceSlowdownFrigateXmlTagDescriptor();

        public TurnInPlaceSlowdownCapitalXmlTagDescriptor TurnInPlaceSlowdownCapital { get; } =
            new TurnInPlaceSlowdownCapitalXmlTagDescriptor();

        public FormationMinimumSideErrorXmlTagDescriptor FormationMinimumSideError { get; } =
            new FormationMinimumSideErrorXmlTagDescriptor();

        public FormationMaximumSideErrorXmlTagDescriptor FormationMaximumSideError { get; } =
            new FormationMaximumSideErrorXmlTagDescriptor();

        public ApproximationSmoothCosAngleXmlTagDescriptor ApproximationSmoothCosAngle { get; } =
            new ApproximationSmoothCosAngleXmlTagDescriptor();

        public ApproximationForwardDistanceXmlTagDescriptor ApproximationForwardDistance { get; } =
            new ApproximationForwardDistanceXmlTagDescriptor();

        public MinimumStoppedVsStoppedOverlapCoefficientXmlTagDescriptor
            MinimumStoppedVsStoppedOverlapCoefficient { get; } =
            new MinimumStoppedVsStoppedOverlapCoefficientXmlTagDescriptor();

        public MovingVsMovingLookAheadTimeXmlTagDescriptor MovingVsMovingLookAheadTime { get; } =
            new MovingVsMovingLookAheadTimeXmlTagDescriptor();

        public LandDestinationProximityXmlTagDescriptor LandDestinationProximity { get; } =
            new LandDestinationProximityXmlTagDescriptor();

        public LandTemporaryDestinationProximityXmlTagDescriptor LandTemporaryDestinationProximity { get; } =
            new LandTemporaryDestinationProximityXmlTagDescriptor();

        public BetweenFormationSpacingXmlTagDescriptor BetweenFormationSpacing { get; } =
            new BetweenFormationSpacingXmlTagDescriptor();

        public PreferredPathfinderTypesXmlTagDescriptor PreferredPathfinderTypes { get; } =
            new PreferredPathfinderTypesXmlTagDescriptor();

        public DestinationCollisionQueryExtensionXmlTagDescriptor DestinationCollisionQueryExtension { get; } =
            new DestinationCollisionQueryExtensionXmlTagDescriptor();

        public FramesPerPositionApproximationRebuildXmlTagDescriptor FramesPerPositionApproximationRebuild { get; } =
            new FramesPerPositionApproximationRebuildXmlTagDescriptor();

        public DynamicLandComplexityQuotaXmlTagDescriptor DynamicLandComplexityQuota { get; } =
            new DynamicLandComplexityQuotaXmlTagDescriptor();

        public DynamicLandQuotaResetIntervalXmlTagDescriptor DynamicLandQuotaResetInterval { get; } =
            new DynamicLandQuotaResetIntervalXmlTagDescriptor();

        public FinalFormationFacingMinimumAngleXmlTagDescriptor FinalFormationFacingMinimumAngle { get; } =
            new FinalFormationFacingMinimumAngleXmlTagDescriptor();

        public FinalFormationFacingDeltaCoefficientXmlTagDescriptor FinalFormationFacingDeltaCoefficient { get; } =
            new FinalFormationFacingDeltaCoefficientXmlTagDescriptor();

        public WalkAnimationCutoffXmlTagDescriptor WalkAnimationCutoff { get; } =
            new WalkAnimationCutoffXmlTagDescriptor();

        public DoubleClickMoveMaxSpeedRatioXmlTagDescriptor DoubleClickMoveMaxSpeedRatio { get; } =
            new DoubleClickMoveMaxSpeedRatioXmlTagDescriptor();

        public FleeingInfantrySpeedBonusXmlTagDescriptor FleeingInfantrySpeedBonus { get; } =
            new FleeingInfantrySpeedBonusXmlTagDescriptor();

        public ShieldFlashScaleXmlTagDescriptor ShieldFlashScale { get; } = new ShieldFlashScaleXmlTagDescriptor();

        public ShieldFlashDurationXmlTagDescriptor ShieldFlashDuration { get; } =
            new ShieldFlashDurationXmlTagDescriptor();

        public HullVsHardPointsHealthConstraintXmlTagDescriptor HullVsHardPointsHealthConstraint { get; } =
            new HullVsHardPointsHealthConstraintXmlTagDescriptor();

        public LowThreatReachabilityToleranceXmlTagDescriptor LowThreatReachabilityTolerance { get; } =
            new LowThreatReachabilityToleranceXmlTagDescriptor();

        public MediumThreatReachabilityToleranceXmlTagDescriptor MediumThreatReachabilityTolerance { get; } =
            new MediumThreatReachabilityToleranceXmlTagDescriptor();

        public HighThreatReachabilityToleranceXmlTagDescriptor HighThreatReachabilityTolerance { get; } =
            new HighThreatReachabilityToleranceXmlTagDescriptor();

        public OverrunAutoResolveMultipleXmlTagDescriptor OverrunAutoResolveMultiple { get; } =
            new OverrunAutoResolveMultipleXmlTagDescriptor();

        public AutoResolveAttritionAllowanceFactorXmlTagDescriptor AutoResolveAttritionAllowanceFactor { get; } =
            new AutoResolveAttritionAllowanceFactorXmlTagDescriptor();

        public AutoResolveTransportLossesXmlTagDescriptor AutoResolveTransportLosses { get; } =
            new AutoResolveTransportLossesXmlTagDescriptor();

        public AutoResolveDisplayTimeXmlTagDescriptor AutoResolveDisplayTime { get; } =
            new AutoResolveDisplayTimeXmlTagDescriptor();

        public RetreatAutoResolveLoserAttritionXmlTagDescriptor RetreatAutoResolveLoserAttrition { get; } =
            new RetreatAutoResolveLoserAttritionXmlTagDescriptor();

        public RetreatAutoResolveWinnerAttritionXmlTagDescriptor RetreatAutoResolveWinnerAttrition { get; } =
            new RetreatAutoResolveWinnerAttritionXmlTagDescriptor();

        public AutoResolveLoserAttritionXmlTagDescriptor AutoResolveLoserAttrition { get; } =
            new AutoResolveLoserAttritionXmlTagDescriptor();

        public AutoResolveWinnerAttritionXmlTagDescriptor AutoResolveWinnerAttrition { get; } =
            new AutoResolveWinnerAttritionXmlTagDescriptor();

        public MinimumTacticalOverrunTimeInSecsXmlTagDescriptor MinimumTacticalOverrunTimeInSecs { get; } =
            new MinimumTacticalOverrunTimeInSecsXmlTagDescriptor();

        public TacticalOverrunMultipleXmlTagDescriptor TacticalOverrunMultiple { get; } =
            new TacticalOverrunMultipleXmlTagDescriptor();

        public ObjectMaxSpeedMultiplierGalacticXmlTagDescriptor ObjectMaxSpeedMultiplierGalactic { get; } =
            new ObjectMaxSpeedMultiplierGalacticXmlTagDescriptor();

        public ObjectMaxSpeedMultiplierSpaceXmlTagDescriptor ObjectMaxSpeedMultiplierSpace { get; } =
            new ObjectMaxSpeedMultiplierSpaceXmlTagDescriptor();

        public ObjectMaxSpeedMultiplierLandXmlTagDescriptor ObjectMaxSpeedMultiplierLand { get; } =
            new ObjectMaxSpeedMultiplierLandXmlTagDescriptor();

        public ObjectMaxHealthMultiplierSpaceXmlTagDescriptor ObjectMaxHealthMultiplierSpace { get; } =
            new ObjectMaxHealthMultiplierSpaceXmlTagDescriptor();

        public ObjectMaxHealthMultiplierLandXmlTagDescriptor ObjectMaxHealthMultiplierLand { get; } =
            new ObjectMaxHealthMultiplierLandXmlTagDescriptor();

        public AutoRotateForSpaceTargetingXmlTagDescriptor AutoRotateForSpaceTargeting { get; } =
            new AutoRotateForSpaceTargetingXmlTagDescriptor();

        public AutoAdjustMoveForSpaceTargetingXmlTagDescriptor AutoAdjustMoveForSpaceTargeting { get; } =
            new AutoAdjustMoveForSpaceTargetingXmlTagDescriptor();

        public InGameCinematicsXmlTagDescriptor InGameCinematics { get; } = new InGameCinematicsXmlTagDescriptor();

        public ProductionSpeedModBaseVsTech0XmlTagDescriptor ProductionSpeedModBaseVsTech0 { get; } =
            new ProductionSpeedModBaseVsTech0XmlTagDescriptor();

        public ProductionSpeedModBaseVsTech1XmlTagDescriptor ProductionSpeedModBaseVsTech1 { get; } =
            new ProductionSpeedModBaseVsTech1XmlTagDescriptor();

        public ProductionSpeedModBaseVsTech2XmlTagDescriptor ProductionSpeedModBaseVsTech2 { get; } =
            new ProductionSpeedModBaseVsTech2XmlTagDescriptor();

        public ProductionSpeedModBaseVsTech3XmlTagDescriptor ProductionSpeedModBaseVsTech3 { get; } =
            new ProductionSpeedModBaseVsTech3XmlTagDescriptor();

        public ProductionSpeedModBaseVsTech4XmlTagDescriptor ProductionSpeedModBaseVsTech4 { get; } =
            new ProductionSpeedModBaseVsTech4XmlTagDescriptor();

        public EnginesDisabledSpeedModifierXmlTagDescriptor EnginesDisabledSpeedModifier { get; } =
            new EnginesDisabledSpeedModifierXmlTagDescriptor();

        public CrouchMoveFireAngleCutoffXmlTagDescriptor CrouchMoveFireAngleCutoff { get; } =
            new CrouchMoveFireAngleCutoffXmlTagDescriptor();

        public MaxMoveFrameDelayXmlTagDescriptor MaxMoveFrameDelay { get; } = new MaxMoveFrameDelayXmlTagDescriptor();

        public SpreadOutSpacingCoefficientXmlTagDescriptor SpreadOutSpacingCoefficient { get; } =
            new SpreadOutSpacingCoefficientXmlTagDescriptor();

        public MaxFormationAreaXmlTagDescriptor MaxFormationArea { get; } = new MaxFormationAreaXmlTagDescriptor();

        public ShortRangeAttackFormationCoefficientXmlTagDescriptor ShortRangeAttackFormationCoefficient { get; } =
            new ShortRangeAttackFormationCoefficientXmlTagDescriptor();

        public SoloAttackRangeXmlTagDescriptor SoloAttackRange { get; } = new SoloAttackRangeXmlTagDescriptor();

        public BaseLandTargetingArcAngleCoefficientXmlTagDescriptor BaseLandTargetingArcAngleCoefficient { get; } =
            new BaseLandTargetingArcAngleCoefficientXmlTagDescriptor();

        public RotateFormationFacingMovesXmlTagDescriptor RotateFormationFacingMoves { get; } =
            new RotateFormationFacingMovesXmlTagDescriptor();

        public ShouldUseSpaceIdleMovementXmlTagDescriptor ShouldUseSpaceIdleMovement { get; } =
            new ShouldUseSpaceIdleMovementXmlTagDescriptor();

        public SpaceIdleMovementSpeedXmlTagDescriptor SpaceIdleMovementSpeed { get; } =
            new SpaceIdleMovementSpeedXmlTagDescriptor();

        public SpaceIdlePathCullCoefficientXmlTagDescriptor SpaceIdlePathCullCoefficient { get; } =
            new SpaceIdlePathCullCoefficientXmlTagDescriptor();

        public IdleMovementFramesXmlTagDescriptor IdleMovementFrames { get; } =
            new IdleMovementFramesXmlTagDescriptor();

        public GripperCombatGridSnapDistanceXmlTagDescriptor GripperCombatGridSnapDistance { get; } =
            new GripperCombatGridSnapDistanceXmlTagDescriptor();

        public DamageTypesXmlTagDescriptor DamageTypes { get; } = new DamageTypesXmlTagDescriptor();
        public ArmorTypesXmlTagDescriptor ArmorTypes { get; } = new ArmorTypesXmlTagDescriptor();
        public DamageToArmorModXmlTagDescriptor DamageToArmorMod { get; } = new DamageToArmorModXmlTagDescriptor();

        public UnitCommandRankingsByCategoryXmlTagDescriptor UnitCommandRankingsByCategory { get; } =
            new UnitCommandRankingsByCategoryXmlTagDescriptor();

        public PlanetAbilityIconNamesXmlTagDescriptor PlanetAbilityIconNames { get; } =
            new PlanetAbilityIconNamesXmlTagDescriptor();

        public PlanetAbilityTextIDsXmlTagDescriptor PlanetAbilityTextIDs { get; } =
            new PlanetAbilityTextIDsXmlTagDescriptor();

        public PlanetAbilityRGBsXmlTagDescriptor PlanetAbilityRGBs { get; } = new PlanetAbilityRGBsXmlTagDescriptor();

        public DroidEncyclopediaOffsetXmlTagDescriptor DroidEncyclopediaOffset { get; } =
            new DroidEncyclopediaOffsetXmlTagDescriptor();

        public ReinforcementOverlayGoodColorXmlTagDescriptor ReinforcementOverlayGoodColor { get; } =
            new ReinforcementOverlayGoodColorXmlTagDescriptor();

        public ReinforcementOverlayBadColorXmlTagDescriptor ReinforcementOverlayBadColor { get; } =
            new ReinforcementOverlayBadColorXmlTagDescriptor();

        public DisplayBinkMovieFramesXmlTagDescriptor DisplayBinkMovieFrames { get; } =
            new DisplayBinkMovieFramesXmlTagDescriptor();

        public MinSkirmishCreditsXmlTagDescriptor MinSkirmishCredits { get; } =
            new MinSkirmishCreditsXmlTagDescriptor();

        public MaxSkirmishCreditsXmlTagDescriptor MaxSkirmishCredits { get; } =
            new MaxSkirmishCreditsXmlTagDescriptor();

        public TacticalBuildTimeMultiplierXmlTagDescriptor TacticalBuildTimeMultiplier { get; } =
            new TacticalBuildTimeMultiplierXmlTagDescriptor();

        public SpaceRetreatAttritionFactorXmlTagDescriptor SpaceRetreatAttritionFactor { get; } =
            new SpaceRetreatAttritionFactorXmlTagDescriptor();

        public LandRetreatAttritionFactorXmlTagDescriptor LandRetreatAttritionFactor { get; } =
            new LandRetreatAttritionFactorXmlTagDescriptor();

        public BlockadeRunAttritionFactorXmlTagDescriptor BlockadeRunAttritionFactor { get; } =
            new BlockadeRunAttritionFactorXmlTagDescriptor();

        public TransportRefillThresholdXmlTagDescriptor TransportRefillThreshold { get; } =
            new TransportRefillThresholdXmlTagDescriptor();

        public SquadronRefillThresholdXmlTagDescriptor SquadronRefillThreshold { get; } =
            new SquadronRefillThresholdXmlTagDescriptor();

        public SpaceTacticalCameraLockedXmlTagDescriptor SpaceTacticalCameraLocked { get; } =
            new SpaceTacticalCameraLockedXmlTagDescriptor();

        public LandTacticalCameraLockedXmlTagDescriptor LandTacticalCameraLocked { get; } =
            new LandTacticalCameraLockedXmlTagDescriptor();

        public SpaceCollidableGridCullSizeXmlTagDescriptor SpaceCollidableGridCullSize { get; } =
            new SpaceCollidableGridCullSizeXmlTagDescriptor();

        public LandCollidableGridCullSizeXmlTagDescriptor LandCollidableGridCullSize { get; } =
            new LandCollidableGridCullSizeXmlTagDescriptor();

        public SpaceLargeShipGridCullSizeXmlTagDescriptor SpaceLargeShipGridCullSize { get; } =
            new SpaceLargeShipGridCullSizeXmlTagDescriptor();

        public RaidForceFreeObjectCategoryMaskXmlTagDescriptor RaidForceFreeObjectCategoryMask { get; } =
            new RaidForceFreeObjectCategoryMaskXmlTagDescriptor();

        public RaidForceLimitedObjectCategoryMaskXmlTagDescriptor RaidForceLimitedObjectCategoryMask { get; } =
            new RaidForceLimitedObjectCategoryMaskXmlTagDescriptor();

        public RaidForceMaxLimitedObjectsXmlTagDescriptor RaidForceMaxLimitedObjects { get; } =
            new RaidForceMaxLimitedObjectsXmlTagDescriptor();

        public RaidForceMaxHerosXmlTagDescriptor RaidForceMaxHeros { get; } = new RaidForceMaxHerosXmlTagDescriptor();

        public RaidForceRequiredFactionXmlTagDescriptor RaidForceRequiredFaction { get; } =
            new RaidForceRequiredFactionXmlTagDescriptor();

        public WinMessageColorXmlTagDescriptor WinMessageColor { get; } = new WinMessageColorXmlTagDescriptor();
        public LoseMessageColorXmlTagDescriptor LoseMessageColor { get; } = new LoseMessageColorXmlTagDescriptor();

        public WinLoseMessageFontXmlTagDescriptor WinLoseMessageFont { get; } =
            new WinLoseMessageFontXmlTagDescriptor();

        public WinLoseMessageFontSizeXmlTagDescriptor WinLoseMessageFontSize { get; } =
            new WinLoseMessageFontSizeXmlTagDescriptor();

        public GamePlayUICreditFontNameXmlTagDescriptor GamePlayUICreditFontName { get; } =
            new GamePlayUICreditFontNameXmlTagDescriptor();

        public GamePlayUICreditFontSizeXmlTagDescriptor GamePlayUICreditFontSize { get; } =
            new GamePlayUICreditFontSizeXmlTagDescriptor();

        public GamePlayUICountdownFontNameXmlTagDescriptor GamePlayUICountdownFontName { get; } =
            new GamePlayUICountdownFontNameXmlTagDescriptor();

        public GamePlayUICountdownFontSizeXmlTagDescriptor GamePlayUICountdownFontSize { get; } =
            new GamePlayUICountdownFontSizeXmlTagDescriptor();

        public InGameMessageDefaultFontNameXmlTagDescriptor InGameMessageDefaultFontName { get; } =
            new InGameMessageDefaultFontNameXmlTagDescriptor();

        public InGameMessageDefaultFontSizeXmlTagDescriptor InGameMessageDefaultFontSize { get; } =
            new InGameMessageDefaultFontSizeXmlTagDescriptor();

        public EventMessageDefaultFontNameXmlTagDescriptor EventMessageDefaultFontName { get; } =
            new EventMessageDefaultFontNameXmlTagDescriptor();

        public EventMessageDefaultFontSizeXmlTagDescriptor EventMessageDefaultFontSize { get; } =
            new EventMessageDefaultFontSizeXmlTagDescriptor();

        public BinkPlayerCaptionFontNameXmlTagDescriptor BinkPlayerCaptionFontName { get; } =
            new BinkPlayerCaptionFontNameXmlTagDescriptor();

        public BinkPlayerCaptionFontSizeXmlTagDescriptor BinkPlayerCaptionFontSize { get; } =
            new BinkPlayerCaptionFontSizeXmlTagDescriptor();

        public ToolTipFontNameXmlTagDescriptor ToolTipFontName { get; } = new ToolTipFontNameXmlTagDescriptor();
        public ToolTipFontSizeXmlTagDescriptor ToolTipFontSize { get; } = new ToolTipFontSizeXmlTagDescriptor();

        public ToolTipSmallFontNameXmlTagDescriptor ToolTipSmallFontName { get; } =
            new ToolTipSmallFontNameXmlTagDescriptor();

        public ToolTipSmallFontSizeXmlTagDescriptor ToolTipSmallFontSize { get; } =
            new ToolTipSmallFontSizeXmlTagDescriptor();

        public CommandBarDefaultFontNameXmlTagDescriptor CommandBarDefaultFontName { get; } =
            new CommandBarDefaultFontNameXmlTagDescriptor();

        public CommandBarDefaultFontSizeXmlTagDescriptor CommandBarDefaultFontSize { get; } =
            new CommandBarDefaultFontSizeXmlTagDescriptor();

        public TextButtonDefaultFontNameXmlTagDescriptor TextButtonDefaultFontName { get; } =
            new TextButtonDefaultFontNameXmlTagDescriptor();

        public TextButtonDefaultFontSizeXmlTagDescriptor TextButtonDefaultFontSize { get; } =
            new TextButtonDefaultFontSizeXmlTagDescriptor();

        public GameObjectNameFontNameXmlTagDescriptor GameObjectNameFontName { get; } =
            new GameObjectNameFontNameXmlTagDescriptor();

        public GameObjectNameFontSizeXmlTagDescriptor GameObjectNameFontSize { get; } =
            new GameObjectNameFontSizeXmlTagDescriptor();

        public BattlePendingMessageColorXmlTagDescriptor BattlePendingMessageColor { get; } =
            new BattlePendingMessageColorXmlTagDescriptor();

        public BattlePendingMessageFontXmlTagDescriptor BattlePendingMessageFont { get; } =
            new BattlePendingMessageFontXmlTagDescriptor();

        public BattlePendingMessageFontSizeXmlTagDescriptor BattlePendingMessageFontSize { get; } =
            new BattlePendingMessageFontSizeXmlTagDescriptor();

        public BattlePendingMessagePosXXmlTagDescriptor BattlePendingMessagePosX { get; } =
            new BattlePendingMessagePosXXmlTagDescriptor();

        public BattlePendingMessagePosYXmlTagDescriptor BattlePendingMessagePosY { get; } =
            new BattlePendingMessagePosYXmlTagDescriptor();

        public MpDefaultCreditsXmlTagDescriptor MPDefaultCredits { get; } = new MpDefaultCreditsXmlTagDescriptor();

        public MpDefaultStartTechLevelXmlTagDescriptor MPDefaultStartTechLevel { get; } =
            new MpDefaultStartTechLevelXmlTagDescriptor();

        public MpDefaultMaxTechLevelXmlTagDescriptor MPDefaultMaxTechLevel { get; } =
            new MpDefaultMaxTechLevelXmlTagDescriptor();

        public MpDefaultAllowAutoResolveXmlTagDescriptor MPDefaultAllowAutoResolve { get; } =
            new MpDefaultAllowAutoResolveXmlTagDescriptor();

        public MpDefaultGameTimerXmlTagDescriptor MPDefaultGameTimer { get; } =
            new MpDefaultGameTimerXmlTagDescriptor();

        public MpDefaultWinConditionXmlTagDescriptor MPDefaultWinCondition { get; } =
            new MpDefaultWinConditionXmlTagDescriptor();

        public MpDefaultWinConditionIntParamXmlTagDescriptor MPDefaultWinConditionIntParam { get; } =
            new MpDefaultWinConditionIntParamXmlTagDescriptor();

        public MpDefaultWinConditionFloatParamXmlTagDescriptor MPDefaultWinConditionFloatParam { get; } =
            new MpDefaultWinConditionFloatParamXmlTagDescriptor();

        public MpDefaultAllowHeroesXmlTagDescriptor MPDefaultAllowHeroes { get; } =
            new MpDefaultAllowHeroesXmlTagDescriptor();

        public MpDefaultAllowSuperWeaponsXmlTagDescriptor MPDefaultAllowSuperWeapons { get; } =
            new MpDefaultAllowSuperWeaponsXmlTagDescriptor();

        public MpDefaultPreBuiltBaseXmlTagDescriptor MPDefaultPreBuiltBase { get; } =
            new MpDefaultPreBuiltBaseXmlTagDescriptor();

        public MpDefaultAllowRandomEventsXmlTagDescriptor MPDefaultAllowRandomEvents { get; } =
            new MpDefaultAllowRandomEventsXmlTagDescriptor();

        public MpDefaultFreeStartingUnitsXmlTagDescriptor MPDefaultFreeStartingUnits { get; } =
            new MpDefaultFreeStartingUnitsXmlTagDescriptor();

        public MpDefaultLandTacticalWinConditionXmlTagDescriptor MPDefaultLandTacticalWinCondition { get; } =
            new MpDefaultLandTacticalWinConditionXmlTagDescriptor();

        public MpDefaultSpaceTacticalWinConditionXmlTagDescriptor MPDefaultSpaceTacticalWinCondition { get; } =
            new MpDefaultSpaceTacticalWinConditionXmlTagDescriptor();

        public MultiplayerLosingTeamBonusCreditPercentageXmlTagDescriptor
            MultiplayerLosingTeamBonusCreditPercentage { get; } =
            new MultiplayerLosingTeamBonusCreditPercentageXmlTagDescriptor();

        public MaxBuildQueueXmlTagDescriptor MaxBuildQueue { get; } = new MaxBuildQueueXmlTagDescriptor();

        public SpaceTacticalUnitCapXmlTagDescriptor SpaceTacticalUnitCap { get; } =
            new SpaceTacticalUnitCapXmlTagDescriptor();

        public TerrainResurfaceRandXmlTagDescriptor TerrainResurfaceRand { get; } =
            new TerrainResurfaceRandXmlTagDescriptor();

        public TerrainResurfaceToleranceXmlTagDescriptor TerrainResurfaceTolerance { get; } =
            new TerrainResurfaceToleranceXmlTagDescriptor();

        public BaseShieldDelayTimeXmlTagDescriptor BaseShieldDelayTime { get; } =
            new BaseShieldDelayTimeXmlTagDescriptor();

        public MinGameSpeedXmlTagDescriptor MinGameSpeed { get; } = new MinGameSpeedXmlTagDescriptor();
        public MaxGameSpeedXmlTagDescriptor MaxGameSpeed { get; } = new MaxGameSpeedXmlTagDescriptor();

        public GameSpeedTrackbarStepsXmlTagDescriptor GameSpeedTrackbarSteps { get; } =
            new GameSpeedTrackbarStepsXmlTagDescriptor();

        public ObjectImportanceDecayFactorXmlTagDescriptor ObjectImportanceDecayFactor { get; } =
            new ObjectImportanceDecayFactorXmlTagDescriptor();

        public UnitPresenceRelaxationTimeXmlTagDescriptor UnitPresenceRelaxationTime { get; } =
            new UnitPresenceRelaxationTimeXmlTagDescriptor();

        public DemoAttractMapsXmlTagDescriptor DemoAttractMaps { get; } = new DemoAttractMapsXmlTagDescriptor();

        public DemoAttractStartTimeoutSecondsXmlTagDescriptor DemoAttractStartTimeoutSeconds { get; } =
            new DemoAttractStartTimeoutSecondsXmlTagDescriptor();

        public DemoAttractMapCycleDelaySecondsXmlTagDescriptor DemoAttractMapCycleDelaySeconds { get; } =
            new DemoAttractMapCycleDelaySecondsXmlTagDescriptor();

        public BattlePendingTimeoutSecondsXmlTagDescriptor BattlePendingTimeoutSeconds { get; } =
            new BattlePendingTimeoutSecondsXmlTagDescriptor();

        public UnderConstructionDamageMultiplierXmlTagDescriptor UnderConstructionDamageMultiplier { get; } =
            new UnderConstructionDamageMultiplierXmlTagDescriptor();

        public SkirmishBuyCreditsXmlTagDescriptor SkirmishBuyCredits { get; } =
            new SkirmishBuyCreditsXmlTagDescriptor();

        public SkirmishReinforcementDelayFramesXmlTagDescriptor SkirmishReinforcementDelayFrames { get; } =
            new SkirmishReinforcementDelayFramesXmlTagDescriptor();

        public DistributeCreditQuantumXmlTagDescriptor DistributeCreditQuantum { get; } =
            new DistributeCreditQuantumXmlTagDescriptor();

        public DiminishingFirepowerXmlTagDescriptor DiminishingFirepower { get; } =
            new DiminishingFirepowerXmlTagDescriptor();

        public IonStormShieldDisableTimeXmlTagDescriptor IonStormShieldDisableTime { get; } =
            new IonStormShieldDisableTimeXmlTagDescriptor();

        public NebulaAbilityDisableTimeXmlTagDescriptor NebulaAbilityDisableTime { get; } =
            new NebulaAbilityDisableTimeXmlTagDescriptor();

        public ForceAbilityDisableTimeXmlTagDescriptor ForceAbilityDisableTime { get; } =
            new ForceAbilityDisableTimeXmlTagDescriptor();

        public DepletedShieldDisableTimeXmlTagDescriptor DepletedShieldDisableTime { get; } =
            new DepletedShieldDisableTimeXmlTagDescriptor();

        public DepletedShieldDamageIncrementXmlTagDescriptor DepletedShieldDamageIncrement { get; } =
            new DepletedShieldDamageIncrementXmlTagDescriptor();

        public DepletedShieldRegenCapXmlTagDescriptor DepletedShieldRegenCap { get; } =
            new DepletedShieldRegenCapXmlTagDescriptor();

        public NebulaEffectColorXmlTagDescriptor NebulaEffectColor { get; } = new NebulaEffectColorXmlTagDescriptor();

        public BaseShieldSpeedModifierXmlTagDescriptor BaseShieldSpeedModifier { get; } =
            new BaseShieldSpeedModifierXmlTagDescriptor();

        public BaseShieldVulnerabilityModifierXmlTagDescriptor BaseShieldVulnerabilityModifier { get; } =
            new BaseShieldVulnerabilityModifierXmlTagDescriptor();

        public HardpointRechargeCutoffForOpportunityFireXmlTagDescriptor
            HardpointRechargeCutoffForOpportunityFire { get; } =
            new HardpointRechargeCutoffForOpportunityFireXmlTagDescriptor();

        public BattleLoadPlanetViewportXmlTagDescriptor BattleLoadPlanetViewport { get; } =
            new BattleLoadPlanetViewportXmlTagDescriptor();

        public SaliencySizeXmlTagDescriptor SaliencySize { get; } = new SaliencySizeXmlTagDescriptor();
        public SaliencyPowerXmlTagDescriptor SaliencyPower { get; } = new SaliencyPowerXmlTagDescriptor();
        public SaliencyXXmlTagDescriptor SaliencyX { get; } = new SaliencyXXmlTagDescriptor();
        public SaliencyYXmlTagDescriptor SaliencyY { get; } = new SaliencyYXmlTagDescriptor();
        public SaliencyHealthXmlTagDescriptor SaliencyHealth { get; } = new SaliencyHealthXmlTagDescriptor();
        public SaliencyTargetsXmlTagDescriptor SaliencyTargets { get; } = new SaliencyTargetsXmlTagDescriptor();
        public SaliencySpeedXmlTagDescriptor SaliencySpeed { get; } = new SaliencySpeedXmlTagDescriptor();

        public PlanetRevealDelayTimeXmlTagDescriptor PlanetRevealDelayTime { get; } =
            new PlanetRevealDelayTimeXmlTagDescriptor();

        public BattleLoadPlanetDirectionXmlTagDescriptor BattleLoadPlanetDirection { get; } =
            new BattleLoadPlanetDirectionXmlTagDescriptor();

        public BattleLoadPlanetAmbientXmlTagDescriptor BattleLoadPlanetAmbient { get; } =
            new BattleLoadPlanetAmbientXmlTagDescriptor();

        public ActivatedBlackMarketAbilityNamesXmlTagDescriptor ActivatedBlackMarketAbilityNames { get; } =
            new ActivatedBlackMarketAbilityNamesXmlTagDescriptor();

        public ActivatedSliceAbilityNamesXmlTagDescriptor ActivatedSliceAbilityNames { get; } =
            new ActivatedSliceAbilityNamesXmlTagDescriptor();

        public ActivatedSabotageAbilityNamesXmlTagDescriptor ActivatedSabotageAbilityNames { get; } =
            new ActivatedSabotageAbilityNamesXmlTagDescriptor();

        public ActivatedNeutralizeHeroAbilityNamesXmlTagDescriptor ActivatedNeutralizeHeroAbilityNames { get; } =
            new ActivatedNeutralizeHeroAbilityNamesXmlTagDescriptor();

        public ActivatedSiphonCreditsAbilityNamesXmlTagDescriptor ActivatedSiphonCreditsAbilityNames { get; } =
            new ActivatedSiphonCreditsAbilityNamesXmlTagDescriptor();

        public ActivatedSystemSpyAbilityNamesXmlTagDescriptor ActivatedSystemSpyAbilityNames { get; } =
            new ActivatedSystemSpyAbilityNamesXmlTagDescriptor();

        public ActivatedDestroyPlanetAbilityNamesXmlTagDescriptor ActivatedDestroyPlanetAbilityNames { get; } =
            new ActivatedDestroyPlanetAbilityNamesXmlTagDescriptor();

        public ActivatedCorruptPlanetAbilityNamesXmlTagDescriptor ActivatedCorruptPlanetAbilityNames { get; } =
            new ActivatedCorruptPlanetAbilityNamesXmlTagDescriptor();

        public ActivatedRemoveCorruptionAbilityNamesXmlTagDescriptor ActivatedRemoveCorruptionAbilityNames { get; } =
            new ActivatedRemoveCorruptionAbilityNamesXmlTagDescriptor();

        public ActivatedHackSuperWeaponAbilityNamesXmlTagDescriptor ActivatedHackSuperWeaponAbilityNames { get; } =
            new ActivatedHackSuperWeaponAbilityNamesXmlTagDescriptor();

        public UseReinforcementPointsXmlTagDescriptor UseReinforcementPoints { get; } =
            new UseReinforcementPointsXmlTagDescriptor();

        public StrategicQueueTacticalBattlesXmlTagDescriptor StrategicQueueTacticalBattles { get; } =
            new StrategicQueueTacticalBattlesXmlTagDescriptor();

        public HealthBarScaleXmlTagDescriptor HealthBarScale { get; } = new HealthBarScaleXmlTagDescriptor();

        public LandHealthBarScaleXmlTagDescriptor LandHealthBarScale { get; } =
            new LandHealthBarScaleXmlTagDescriptor();

        public MinHealthBarScaleXmlTagDescriptor MinHealthBarScale { get; } = new MinHealthBarScaleXmlTagDescriptor();
        public CreditsSpacingXmlTagDescriptor CreditsSpacing { get; } = new CreditsSpacingXmlTagDescriptor();
        public CreditsScrollRateXmlTagDescriptor CreditsScrollRate { get; } = new CreditsScrollRateXmlTagDescriptor();
        public CreditsFontXmlTagDescriptor CreditsFont { get; } = new CreditsFontXmlTagDescriptor();
        public CreditsFontSizeXmlTagDescriptor CreditsFontSize { get; } = new CreditsFontSizeXmlTagDescriptor();
        public CreditsRowsXmlTagDescriptor CreditsRows { get; } = new CreditsRowsXmlTagDescriptor();
        public CreditsTopColorXmlTagDescriptor CreditsTopColor { get; } = new CreditsTopColorXmlTagDescriptor();

        public CreditsBottomColorXmlTagDescriptor CreditsBottomColor { get; } =
            new CreditsBottomColorXmlTagDescriptor();

        public CreditsHeaderTopColorXmlTagDescriptor CreditsHeaderTopColor { get; } =
            new CreditsHeaderTopColorXmlTagDescriptor();

        public CreditsHeaderBottomColorXmlTagDescriptor CreditsHeaderBottomColor { get; } =
            new CreditsHeaderBottomColorXmlTagDescriptor();

        public CreditsMarginXmlTagDescriptor CreditsMargin { get; } = new CreditsMarginXmlTagDescriptor();
        public CreditsLogo1NameXmlTagDescriptor CreditsLogo1Name { get; } = new CreditsLogo1NameXmlTagDescriptor();
        public CreditsLogo1WidthXmlTagDescriptor CreditsLogo1Width { get; } = new CreditsLogo1WidthXmlTagDescriptor();

        public CreditsLogo1HeightXmlTagDescriptor CreditsLogo1Height { get; } =
            new CreditsLogo1HeightXmlTagDescriptor();

        public CreditsLogo1YOffsetXmlTagDescriptor CreditsLogo1YOffset { get; } =
            new CreditsLogo1YOffsetXmlTagDescriptor();

        public CreditsLogo2NameXmlTagDescriptor CreditsLogo2Name { get; } = new CreditsLogo2NameXmlTagDescriptor();
        public CreditsLogo2WidthXmlTagDescriptor CreditsLogo2Width { get; } = new CreditsLogo2WidthXmlTagDescriptor();

        public CreditsLogo2HeightXmlTagDescriptor CreditsLogo2Height { get; } =
            new CreditsLogo2HeightXmlTagDescriptor();

        public CreditsLogo2YOffsetXmlTagDescriptor CreditsLogo2YOffset { get; } =
            new CreditsLogo2YOffsetXmlTagDescriptor();

        public CreditsLogo3NameXmlTagDescriptor CreditsLogo3Name { get; } = new CreditsLogo3NameXmlTagDescriptor();
        public CreditsLogo3WidthXmlTagDescriptor CreditsLogo3Width { get; } = new CreditsLogo3WidthXmlTagDescriptor();

        public CreditsLogo3HeightXmlTagDescriptor CreditsLogo3Height { get; } =
            new CreditsLogo3HeightXmlTagDescriptor();

        public CreditsLogo3YOffsetXmlTagDescriptor CreditsLogo3YOffset { get; } =
            new CreditsLogo3YOffsetXmlTagDescriptor();

        public TractorBeamWidthXmlTagDescriptor TractorBeamWidth { get; } = new TractorBeamWidthXmlTagDescriptor();

        public TractorBeamTextureXmlTagDescriptor TractorBeamTexture { get; } =
            new TractorBeamTextureXmlTagDescriptor();

        public TractorBeamFramesXmlTagDescriptor TractorBeamFrames { get; } = new TractorBeamFramesXmlTagDescriptor();
        public TractorBeamColorXmlTagDescriptor TractorBeamColor { get; } = new TractorBeamColorXmlTagDescriptor();
        public EnergyBeamWidthXmlTagDescriptor EnergyBeamWidth { get; } = new EnergyBeamWidthXmlTagDescriptor();
        public EnergyBeamTextureXmlTagDescriptor EnergyBeamTexture { get; } = new EnergyBeamTextureXmlTagDescriptor();
        public EnergyBeamFramesXmlTagDescriptor EnergyBeamFrames { get; } = new EnergyBeamFramesXmlTagDescriptor();
        public EnergyBeamColorXmlTagDescriptor EnergyBeamColor { get; } = new EnergyBeamColorXmlTagDescriptor();

        public TeamHealthbarOffsetXmlTagDescriptor TeamHealthbarOffset { get; } =
            new TeamHealthbarOffsetXmlTagDescriptor();

        public ObjectVisualStatusParticleAttachBoneNamesXmlTagDescriptor
            ObjectVisualStatusParticleAttachBoneNames { get; } =
            new ObjectVisualStatusParticleAttachBoneNamesXmlTagDescriptor();

        public MessageOfTheDayURLXmlTagDescriptor MessageOfTheDayURL { get; } =
            new MessageOfTheDayURLXmlTagDescriptor();

        public ControlPointDominationVictoryTimeInSecsXmlTagDescriptor ControlPointDominationVictoryTimeInSecs { get; }
            = new ControlPointDominationVictoryTimeInSecsXmlTagDescriptor();

        public CameraFXManagerLetterboxHeightXmlTagDescriptor CameraFXManagerLetterboxHeight { get; } =
            new CameraFXManagerLetterboxHeightXmlTagDescriptor();

        public HealthLowPercentThresholdXmlTagDescriptor HealthLowPercentThreshold { get; } =
            new HealthLowPercentThresholdXmlTagDescriptor();

        public HealthCriticalPercentThresholdXmlTagDescriptor HealthCriticalPercentThreshold { get; } =
            new HealthCriticalPercentThresholdXmlTagDescriptor();

        public MpColorBlueXmlTagDescriptor MPColorBlue { get; } = new MpColorBlueXmlTagDescriptor();
        public MpColorRedXmlTagDescriptor MPColorRed { get; } = new MpColorRedXmlTagDescriptor();
        public MpColorGreenXmlTagDescriptor MPColorGreen { get; } = new MpColorGreenXmlTagDescriptor();
        public MpColorOrangeXmlTagDescriptor MPColorOrange { get; } = new MpColorOrangeXmlTagDescriptor();
        public MpColorCyanXmlTagDescriptor MPColorCyan { get; } = new MpColorCyanXmlTagDescriptor();
        public MpColorPurpleXmlTagDescriptor MPColorPurple { get; } = new MpColorPurpleXmlTagDescriptor();
        public MpColorYellowXmlTagDescriptor MPColorYellow { get; } = new MpColorYellowXmlTagDescriptor();
        public MpColorGrayXmlTagDescriptor MPColorGray { get; } = new MpColorGrayXmlTagDescriptor();
        public MpColorEightXmlTagDescriptor MPColorEight { get; } = new MpColorEightXmlTagDescriptor();
        public MeleeCutoffRangeXmlTagDescriptor MeleeCutoffRange { get; } = new MeleeCutoffRangeXmlTagDescriptor();

        public SpaceRetreatAllowedCountdownSecondsXmlTagDescriptor SpaceRetreatAllowedCountdownSeconds { get; } =
            new SpaceRetreatAllowedCountdownSecondsXmlTagDescriptor();

        public LandRetreatAllowedCountdownSecondsXmlTagDescriptor LandRetreatAllowedCountdownSeconds { get; } =
            new LandRetreatAllowedCountdownSecondsXmlTagDescriptor();

        public LocalizedSplashScreenXmlTagDescriptor LocalizedSplashScreen { get; } =
            new LocalizedSplashScreenXmlTagDescriptor();

        public LocalizedMenuOverlayXmlTagDescriptor LocalizedMenuOverlay { get; } =
            new LocalizedMenuOverlayXmlTagDescriptor();

        public LocalizedUKEnglishSplashScreenXmlTagDescriptor LocalizedUKEnglishSplashScreen { get; } =
            new LocalizedUKEnglishSplashScreenXmlTagDescriptor();

        public MainMenuDemoAttractModeXmlTagDescriptor MainMenuDemoAttractMode { get; } =
            new MainMenuDemoAttractModeXmlTagDescriptor();

        public IndigenousSpawnDestructionRewardXmlTagDescriptor IndigenousSpawnDestructionReward { get; } =
            new IndigenousSpawnDestructionRewardXmlTagDescriptor();

        public AdvisorHintIntervalXmlTagDescriptor AdvisorHintInterval { get; } =
            new AdvisorHintIntervalXmlTagDescriptor();

        public AdvisorHintDurationXmlTagDescriptor AdvisorHintDuration { get; } =
            new AdvisorHintDurationXmlTagDescriptor();

        public RadarColorizeSelectedUnitsXmlTagDescriptor RadarColorizeSelectedUnits { get; } =
            new RadarColorizeSelectedUnitsXmlTagDescriptor();

        public RadarSelectedUnitsColorXmlTagDescriptor RadarSelectedUnitsColor { get; } =
            new RadarSelectedUnitsColorXmlTagDescriptor();

        public RadarColorizeMultiplayerEnemyXmlTagDescriptor RadarColorizeMultiplayerEnemy { get; } =
            new RadarColorizeMultiplayerEnemyXmlTagDescriptor();

        public RadarMultiplayerEnemyColorXmlTagDescriptor RadarMultiplayerEnemyColor { get; } =
            new RadarMultiplayerEnemyColorXmlTagDescriptor();

        public AnimateDuringGalacticModePauseXmlTagDescriptor AnimateDuringGalacticModePause { get; } =
            new AnimateDuringGalacticModePauseXmlTagDescriptor();

        public LandBaseDestructionForcesRetreatXmlTagDescriptor LandBaseDestructionForcesRetreat { get; } =
            new LandBaseDestructionForcesRetreatXmlTagDescriptor();

        public SpaceStationDestructionForcesRetreatXmlTagDescriptor SpaceStationDestructionForcesRetreat { get; } =
            new SpaceStationDestructionForcesRetreatXmlTagDescriptor();

        public LandCaptureAllowedCountdownSecondsXmlTagDescriptor LandCaptureAllowedCountdownSeconds { get; } =
            new LandCaptureAllowedCountdownSecondsXmlTagDescriptor();

        public SpaceCaptureAllowedCountdownSecondsXmlTagDescriptor SpaceCaptureAllowedCountdownSeconds { get; } =
            new SpaceCaptureAllowedCountdownSecondsXmlTagDescriptor();

        public SpaceReinforcementCollisionCheckDistanceXmlTagDescriptor
            SpaceReinforcementCollisionCheckDistance { get; } =
            new SpaceReinforcementCollisionCheckDistanceXmlTagDescriptor();

        public HealthBarSpacingXmlTagDescriptor HealthBarSpacing { get; } = new HealthBarSpacingXmlTagDescriptor();

        public MaxBombingRunIntervalSecondsXmlTagDescriptor MaxBombingRunIntervalSeconds { get; } =
            new MaxBombingRunIntervalSecondsXmlTagDescriptor();

        public MinBombingRunIntervalSecondsXmlTagDescriptor MinBombingRunIntervalSeconds { get; } =
            new MinBombingRunIntervalSecondsXmlTagDescriptor();

        public BombingRunReductionPerSquadronPercentXmlTagDescriptor BombingRunReductionPerSquadronPercent { get; } =
            new BombingRunReductionPerSquadronPercentXmlTagDescriptor();

        public MaxBombardIntervalSecondsXmlTagDescriptor MaxBombardIntervalSeconds { get; } =
            new MaxBombardIntervalSecondsXmlTagDescriptor();

        public MinBombardIntervalSecondsXmlTagDescriptor MinBombardIntervalSeconds { get; } =
            new MinBombardIntervalSecondsXmlTagDescriptor();

        public BombardmentOffsetXmlTagDescriptor BombardmentOffset { get; } = new BombardmentOffsetXmlTagDescriptor();

        public BombardmentDistributionXmlTagDescriptor BombardmentDistribution { get; } =
            new BombardmentDistributionXmlTagDescriptor();

        public AIUsesGalacticPopCapXmlTagDescriptor AIUsesGalacticPopCap { get; } =
            new AIUsesGalacticPopCapXmlTagDescriptor();

        public SpaceGuardRangeXmlTagDescriptor SpaceGuardRange { get; } = new SpaceGuardRangeXmlTagDescriptor();
        public LandGuardRangeXmlTagDescriptor LandGuardRange { get; } = new LandGuardRangeXmlTagDescriptor();

        public OverrideDeathPersistenceDurationXmlTagDescriptor OverrideDeathPersistenceDuration { get; } =
            new OverrideDeathPersistenceDurationXmlTagDescriptor();

        public AutoResolveTacticalMultiplierXmlTagDescriptor AutoResolveTacticalMultiplier { get; } =
            new AutoResolveTacticalMultiplierXmlTagDescriptor();

        public QuickmatchMapExclusionListXmlTagDescriptor QuickmatchMapExclusionList { get; } =
            new QuickmatchMapExclusionListXmlTagDescriptor();

        public CorruptionParticleNameXmlTagDescriptor CorruptionParticleName { get; } =
            new CorruptionParticleNameXmlTagDescriptor();

        public CorruptionParticleLineNameXmlTagDescriptor CorruptionParticleLineName { get; } =
            new CorruptionParticleLineNameXmlTagDescriptor();

        public ParticleBrightnessPerCorruptionLevelXmlTagDescriptor ParticleBrightnessPerCorruptionLevel { get; } =
            new ParticleBrightnessPerCorruptionLevelXmlTagDescriptor();

        public ParticleScalePerCorruptionLevelXmlTagDescriptor ParticleScalePerCorruptionLevel { get; } =
            new ParticleScalePerCorruptionLevelXmlTagDescriptor();

        public ParticleEnergyPerCorruptionLevelXmlTagDescriptor ParticleEnergyPerCorruptionLevel { get; } =
            new ParticleEnergyPerCorruptionLevelXmlTagDescriptor();

        public CorruptionLineRadiusXmlTagDescriptor CorruptionLineRadius { get; } =
            new CorruptionLineRadiusXmlTagDescriptor();

        public CorruptionLineStartEndOffsetXmlTagDescriptor CorruptionLineStartEndOffset { get; } =
            new CorruptionLineStartEndOffsetXmlTagDescriptor();

        public CorruptionLineGrowSecondsXmlTagDescriptor CorruptionLineGrowSeconds { get; } =
            new CorruptionLineGrowSecondsXmlTagDescriptor();

        public CorruptionPathColorXmlTagDescriptor CorruptionPathColor { get; } =
            new CorruptionPathColorXmlTagDescriptor();

        public CorruptionPathWidthXmlTagDescriptor CorruptionPathWidth { get; } =
            new CorruptionPathWidthXmlTagDescriptor();

        public CorruptionPathOffsetXmlTagDescriptor CorruptionPathOffset { get; } =
            new CorruptionPathOffsetXmlTagDescriptor();

        public BriberyFleetRevealRangeXmlTagDescriptor BriberyFleetRevealRange { get; } =
            new BriberyFleetRevealRangeXmlTagDescriptor();

        public SabotageParticleEffectXmlTagDescriptor SabotageParticleEffect { get; } =
            new SabotageParticleEffectXmlTagDescriptor();

        public HackSuperWeaponParticleEffectXmlTagDescriptor HackSuperWeaponParticleEffect { get; } =
            new HackSuperWeaponParticleEffectXmlTagDescriptor();

        public HackSuperWeaponRequiredTypeXmlTagDescriptor HackSuperWeaponRequiredType { get; } =
            new HackSuperWeaponRequiredTypeXmlTagDescriptor();

        public SensorJammingTimeXmlTagDescriptor SensorJammingTime { get; } = new SensorJammingTimeXmlTagDescriptor();

        public StealthDetectionTimeXmlTagDescriptor StealthDetectionTime { get; } =
            new StealthDetectionTimeXmlTagDescriptor();

        public FirstStrikeExtraDamagePercentXmlTagDescriptor FirstStrikeExtraDamagePercent { get; } =
            new FirstStrikeExtraDamagePercentXmlTagDescriptor();

        public FirstStrikeParticleXmlTagDescriptor FirstStrikeParticle { get; } =
            new FirstStrikeParticleXmlTagDescriptor();

        public GarrisonedMaxAttackDistanceMultiplierXmlTagDescriptor GarrisonedMaxAttackDistanceMultiplier { get; } =
            new GarrisonedMaxAttackDistanceMultiplierXmlTagDescriptor();

        public PathfinderSlotIndexXmlTagDescriptor PathfinderSlotIndex { get; } =
            new PathfinderSlotIndexXmlTagDescriptor();

        public DefaultBountyByCategorySPXmlTagDescriptor DefaultBountyByCategorySP { get; } =
            new DefaultBountyByCategorySPXmlTagDescriptor();

        public DefaultBountyByCategoryMpXmlTagDescriptor DefaultBountyByCategoryMP { get; } =
            new DefaultBountyByCategoryMpXmlTagDescriptor();

        public MaxRemoteBombsPerPlayerXmlTagDescriptor MaxRemoteBombsPerPlayer { get; } =
            new MaxRemoteBombsPerPlayerXmlTagDescriptor();

        public CorruptionHyperspaceBonusXmlTagDescriptor CorruptionHyperspaceBonus { get; } =
            new CorruptionHyperspaceBonusXmlTagDescriptor();

        public CorruptionChoiceIconNameXmlTagDescriptor CorruptionChoiceIconName { get; } =
            new CorruptionChoiceIconNameXmlTagDescriptor();

        public CorruptionChoiceNameXmlTagDescriptor CorruptionChoiceName { get; } =
            new CorruptionChoiceNameXmlTagDescriptor();

        public CorruptionChoiceBenefitXmlTagDescriptor CorruptionChoiceBenefit { get; } =
            new CorruptionChoiceBenefitXmlTagDescriptor();

        public CorruptionMissionRequirementIconNameXmlTagDescriptor CorruptionMissionRequirementIconName { get; } =
            new CorruptionMissionRequirementIconNameXmlTagDescriptor();

        public CorruptionChoiceEncyclopediaXmlTagDescriptor CorruptionChoiceEncyclopedia { get; } =
            new CorruptionChoiceEncyclopediaXmlTagDescriptor();

        public CorruptionChoiceIncomePercentageXmlTagDescriptor CorruptionChoiceIncomePercentage { get; } =
            new CorruptionChoiceIncomePercentageXmlTagDescriptor();

        public CorruptionPlanetIconXmlTagDescriptor CorruptionPlanetIcon { get; } =
            new CorruptionPlanetIconXmlTagDescriptor();

        public CorruptionPlanetIconEncyclopediaNameXmlTagDescriptor CorruptionPlanetIconEncyclopediaName { get; } =
            new CorruptionPlanetIconEncyclopediaNameXmlTagDescriptor();

        public CorruptionPlanetIconEncyclopediaDescXmlTagDescriptor CorruptionPlanetIconEncyclopediaDesc { get; } =
            new CorruptionPlanetIconEncyclopediaDescXmlTagDescriptor();

        public HackSuperWeaponCostXmlTagDescriptor HackSuperWeaponCost { get; } =
            new HackSuperWeaponCostXmlTagDescriptor();

        public CorruptionEncyclopediaBackdropXmlTagDescriptor CorruptionEncyclopediaBackdrop { get; } =
            new CorruptionEncyclopediaBackdropXmlTagDescriptor();

        public CorruptionEncyclopediaHeaderXmlTagDescriptor CorruptionEncyclopediaHeader { get; } =
            new CorruptionEncyclopediaHeaderXmlTagDescriptor();

        public CorruptionEncyclopediaCompleteXmlTagDescriptor CorruptionEncyclopediaComplete { get; } =
            new CorruptionEncyclopediaCompleteXmlTagDescriptor();

        public CorruptionEncyclopediaIncompleteXmlTagDescriptor CorruptionEncyclopediaIncomplete { get; } =
            new CorruptionEncyclopediaIncompleteXmlTagDescriptor();

        public CorruptionEncyclopediaMoneyIconXmlTagDescriptor CorruptionEncyclopediaMoneyIcon { get; } =
            new CorruptionEncyclopediaMoneyIconXmlTagDescriptor();

        public CorruptionEncyclopediaLeftEdgeXmlTagDescriptor CorruptionEncyclopediaLeftEdge { get; } =
            new CorruptionEncyclopediaLeftEdgeXmlTagDescriptor();

        public CorruptionEncyclopediaSpacingXmlTagDescriptor CorruptionEncyclopediaSpacing { get; } =
            new CorruptionEncyclopediaSpacingXmlTagDescriptor();
    }
}
