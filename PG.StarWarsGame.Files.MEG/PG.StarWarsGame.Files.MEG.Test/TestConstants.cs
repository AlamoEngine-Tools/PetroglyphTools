// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Test;

namespace PG.StarWarsGame.Files.MEG.Test
{
    internal static class TestConstants
    {
        internal const string FILE_NAME_GAMEOBJECTFILES = "gameobjectfiles.xml";

        internal const string CONTENT_GAMEOBJECTFILES =
            "<?xml version=\"1.0\" ?>\n\n<Game_Object_Files>\n\n\t<File>TransportUnits.xml</File>\n\n\t<File>TechBuilding.xml</File>\t\t<!-- AJA 10/12/2005 - Moved to the top so that tech upgrades always show up on the left of the build bar. -->\n\t<File>GroundBases.xml</File>\n\t<File>GroundStructures.xml</File>\n\t<File>GroundTurrets.xml</File>\n\t<File>GroundInfantry.xml</File>\n\t<File>GroundVehicles.xml</File>\n\t<File>GroundCompaniesRebel.xml</File>\n\t<File>GroundCompaniesEmpire.xml</File>\n\t<File>GroundIndigenous.xml</File>\n\t<File>GroundCompaniesIndigenous.xml</File>\n\t<File>GroundBuildablesRebel.xml</File>\n\t<File>GroundBuildablesEmpire.xml</File>\n\t<File>GroundBuildablesPirate.xml</File>\n\t<File>GroundBuildablesSkirmish.xml</File>\n\n\t<File>SpaceProps.xml</File>\n\t<File>SpaceUnitsCapital.xml</File>\n\t<File>SpaceUnitsFighters.xml</File>\n\t<File>SpaceUnitsCorvettes.xml</File>\n\t<File>SpaceUnitsFrigates.xml</File>\n\t<File>SpaceUnitsSupers.xml</File>\n\t<File>SpacePrimarySkydomes.xml</File>\n\t<File>SpaceSecondarySkydomes.xml</File>\n\t<File>SpaceBuildablesSkirmish.xml</File>\n\t\n\t<File>NamedHeroUnits.xml</File>\n\t<File>GenericHeroUnits.xml</File>\n\t<File>HeroCompanies.xml</File>\n\n\t<File>LandPrimarySkydomes.xml</File>\n\t<File>LandSecondarySkydomes.xml</File>\n\n\t<File>LandBombingRunUnits.xml</File>\n\n\t<File>SpecialStructures.xml</File>\n\t<File>StarBases.xml</File>\n\n\t<File>Squadrons.xml</File>\n\n\t<File>Planets.xml</File>\n\n\t<File>Projectiles.xml</File>\n\n\t<File>SpecialEffects.xml</File>\n\t<File>Particles.xml</File>\n\t<File>Markers.xml</File>\n\t<File>UniqueUnits.xml</File>\n\t<File>Containers.xml</File>\n\t<File>ScriptMarkers.xml</File>\n\n\t<File>MiscObjects.xml</File>\n\t<File>UpgradeObjects.xml</File>\n\t<File>SecondaryStructures.xml</File>\n\n\t<File>Props_Temperate.xml</File>\n\t<File>Props_Swamp.xml</File>\n\t<File>Props_Volcanic.xml</File>\n\t<File>Props_Forest.xml</File>\n\t<File>Props_Snow.xml</File>\n\t<File>Props_Desert.xml</File>\n\t<File>Props_Urban.xml</File>\n\t<File>Props_Generic.xml</File>\n\t<File>Props_Story.xml</File>\t\n\n\t<File>CIN_SpaceUnitsCapital.xml</File>\n\t<File>CIN_SpaceUnitsFighters.xml</File>\n\t<File>CIN_SpaceUnitsCorvettes.xml</File>\n\t<File>CIN_SpaceUnitsFrigates.xml</File>\n\t<File>CIN_GroundInfantry.xml</File>\n\t<File>CIN_GroundVehicles.xml</File>\n\t<File>CIN_SpaceProps.xml</File>\n\t<File>CIN_TransportUnits.xml</File>\n\t<File>CIN_Projectiles.xml</File>\n\t<File>CIN_GroundProps.xml</File>\n\t<File>CIN_GroundTurrets.xml</File>\n\n\t<File>MOV_Cinematics.xml</File>\n\n\n\n\n\n\n\t<!-- Expansion Units: Empire -->\n\n\t<File>Units_Hero_Empire_Thrawn.xml</File>\n\n\t<File>Units_Space_Empire_TIE_Defender.xml</File>\n\t<File>Units_Space_Empire_TIE_Interceptor.xml</File>\n\t<File>Units_Space_Empire_TIE_Phantom.xml</File>\n\t<File>Units_Space_Empire_Executor.xml</File>\n\t\n\t<File>Units_Land_Empire_DarkTroopers.xml</File>\n\t<File>Units_Land_Empire_Lancet.xml</File>\n\t<File>Units_Land_Empire_Noghri.xml</File>\n\t<File>Units_Land_Empire_Juggernaut.xml</File>\n\n\n\n\t<!-- Expansion Units: Rebel -->\n\t\n\t<File>Units_Hero_Rebel_Yoda.xml</File>\n\t<File>Units_Hero_Rebel_LukeSkywalker.xml</File>\n\t<File>Units_Hero_Rebel_Gargantuan.xml</File>\n\t<File>Units_Hero_Rebel_Rogue_Squadron.xml</File>\n\n\t<File>Units_Land_Rebel_Gallofree_HTT.xml</File>\n\n\t<File>Units_Space_Rebel_BWing.xml</File>\t\n\t<File>Units_Space_Rebel_MC30_Frigate.xml</File>\n\n\n\n\t<!-- Expansion Units: Underworld -->\n\n\t<File>Units_Hero_Underworld_IG88.xml</File>\n\t<File>Units_Hero_Underworld_Bossk.xml</File>\n\t<File>Units_Hero_Underworld_Tyber_Zann.xml</File>\n\t<File>Units_Hero_Underworld_Silri.xml</File>\n\t<File>Units_Hero_Underworld_Urai_Fen.xml</File>\n\n\t<File>Units_Land_Underworld_MercenaryInf.xml</File>\n\t<File>Units_Land_Underworld_DisruptorInf.xml</File>\n\t<File>Units_Land_Underworld_DestroyerDroids.xml</File>\n\t<File>Units_Land_Underworld_Vornskr.xml</File>\n\t<File>Units_Land_Underworld_F9TZ_Transport.xml</File>\n\t<File>Units_Land_Underworld_MAL.xml</File>\n\t<File>Units_Land_Underworld_PulseTank.xml</File>\n\t<File>Units_Land_Underworld_Canderous_Tank.xml</File>\n\t<File>Units_Land_Underworld_ScavengerDroids.xml</File>\n\t<File>Units_Land_Underworld_NightSister.xml</File>\n\t<File>Units_Land_Underworld_Ewok_Bomber.xml</File>\n\n\t<File>Units_Space_Underworld_StarViper.xml</File>\n\t<File>Units_Space_Underworld_Skipray.xml</File>\n\t<File>Units_Space_Underworld_Crusader_Gunship.xml</File>\n\t<File>Units_Space_Underworld_Interceptor4.xml</File>\n\t<File>Units_Space_Underworld_Vengeance.xml</File>\t\n\t<File>Units_Space_Underworld_Kadalbe_Battleship.xml</File>\n\t<File>Units_Space_Underworld_Krayt_Destroyer.xml</File>\n\t<File>Units_Space_Underworld_Buzz_Droids.xml</File>\n\n\t<File>SpecialStructures_Underworld.xml</File>\n\t<File>StarBases_Underworld.xml</File>\n\n\n\n\t<!-- Expansion Units: Neutral -->\n\n\t<File>Units_Space_Neutral_Eclipse.xml</File>\n\t<File>Hutt_Faction_Units.xml</File>\n\t<File>GroundBuildablesHutts.xml</File>\n\n\n\t<!-- Expansion General Files -->\n\t\n\t<File>SpaceProps_Underworld.xml</File>\n\t<File>Underworld_TransportUnits.xml</File>\n\t<File>Minor_Heroes_Expansion.xml</File>\n\t<File>Props_Felucia.xml</File>\n\t<File>GroundBuildablesUnderworld.xml</File>\n\t<File>GroundStructures_Underworld.xml</File>\n\t<File>Mobile_Defense_Units.XML</File>\n\t<File>Multiplayer_Structure_Markers.XML</File>\n\t<File>UpgradeObjects_Underworld.XML</File>\n\t<File>Corruption_Special_Units.XML</File>\n\n\n\n</Game_Object_Files>";

        internal const string FILE_NAME_CAMPAIGNFILES = "campaignfiles.xml";

        internal const string CONTENT_CAMPAIGNFILES =
            "<?xml version='1.0' encoding='utf-8'?>\n<Campaign_Files>\n\t<File>core/gc/campaign/campaigns_underworld_gc.xml</File>\n\t<File>core/gc/campaign/campaigns_underworld_story.xml</File>\n\t<File>core/gc/campaign/campaigns_multiplayer.xml</File>\n\t<File>core/gc/campaign/campaigns_tutorial.xml</File>\n\t<File>core/gc/campaign/campaigns_underworld_tutorial.xml</File>\n</Campaign_Files>";

        internal const string FILE_NAME_MEG_FILE = "testfile.meg";

        internal static readonly string FILE_PATH_GAMEOBJECTFILES = TestUtility.IsWindows()
            ? @"c:\mod\data\xml\gameobjectfiles.xml"
            : @"/mnt/c/mod/data/xml/gameobjectfiles.xml";

        internal static readonly string FILE_PATH_CAMPAIGNFILES = TestUtility.IsWindows()
            ? @"c:\mod\data\xml\campaignfiles.xml"
            : @"/mnt/c/mod/data/xml/campaignfiles.xml";

        internal static readonly string BASE_PATH = TestUtility.IsWindows() ? @"c:\mod" : @"/mnt/c/mod";

        internal static readonly string FILE_PATH_MEG_FILE = TestUtility.IsWindows()
            ? @"c:\mod\data\testfile.meg"
            : @"/mnt/c/mod/data/testfile.meg";

        internal static readonly byte[] CONTENT_MEG_FILE =
        {
            2, 0, 0, 0, 2, 0, 0, 0, 28, 0, 68, 65, 84, 65, 47, 88, 77, 76, 47, 71, 65, 77, 69, 79, 66, 74, 69, 67, 84,
            70, 73, 76, 69, 83, 46, 88, 77, 76, 26, 0, 68, 65, 84, 65, 47, 88, 77, 76, 47, 67, 65, 77, 80, 65, 73, 71,
            78, 70, 73, 76, 69, 83, 46, 88, 77, 76, 146, 206, 127, 126, 0, 0, 0, 0, 158, 20, 0, 0, 106, 0, 0, 0, 0, 0,
            0, 0, 252, 90, 98, 183, 1, 0, 0, 0, 114, 1, 0, 0, 8, 21, 0, 0, 1, 0, 0, 0
        };
    }
}
