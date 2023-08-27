﻿using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation.NavigationInfo
{
    public class WorldInfo : ILoadable
	{
        private static Dictionary<string, List<InfoElement>> worldInfoStorage;

        public void Load(Mod mod)
        {
			worldInfoStorage = new Dictionary<string, List<InfoElement>>();
			LoadData();
        }

        public void Unload()
        {
            worldInfoStorage.Clear();
            worldInfoStorage = null;
        }

		public static void Add(string key, params InfoElement[] infoElements)
            => worldInfoStorage.Add(key, infoElements.ToList());

        public static List<InfoElement> GetInfoElements(string key) => worldInfoStorage[key];
   
        public static bool TryGetValue(string key, out List<InfoElement> info) 
            => worldInfoStorage.TryGetValue(key, out info);

		public static UIListScrollablePanel ProvideUI(string subworldName)
		{
			string localizationPath = "Mods.Macrocosm.Subworlds.";

		    UIListScrollablePanel listPanel = new(new LocalizedColorScaleText(Language.GetText(localizationPath + subworldName + ".DisplayName"), scale: 1.2f))
			{
				Width = new(0, 0.31f),
				Height = new(0, 0.62f),
				Left = new(0, 0.01f),
				Top = new(0, 0.365f),
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			listPanel.SetPadding(0f);

			LocalizedText flavorText = Utility.GetLocalizedTextOrEmpty(localizationPath + subworldName + ".FlavorText");
			if (flavorText != LocalizedText.Empty && flavorText.Value != "default")
			{
				listPanel.Add(new UIDynamicTextPanel(new LocalizedColorScaleText(flavorText, Color.White, scale: 0.85f)));
				listPanel.AddSeparator();
			}

			List<InfoElement> elements = worldInfoStorage[subworldName];

			if (elements is not null)
			{
				bool foundHazards = false;
				foreach (InfoElement element in elements)
				{
					if (!foundHazards && element is HazardInfoElement)
					{
						listPanel.AddSeparator();
						foundHazards = true;
					}

					listPanel.Add(element.ProvideUI());
				}
			}

			return listPanel;
		}

		/// <summary>
		/// Populates the World Info data.
		/// <br> <br>
		/// Supports: 
		/// <list type="bullet">
		///     <item> <see cref="GravityInfoElement"/>, <see cref="RadiusInfoElement"/>, <see cref="DayPeriodInfoElement"/>: general info elements, specify numeric and/or string value (e.g. TidallyLocked) </item>
		///     <item> <see cref="ThreatLevelInfoElement"/>: the threat level info element, pick from ThreatLevel enum. </item>
		///     <item> <see cref="HazardInfoElement"/>: the hazards specific to this world (e.g. MeteorStorms). </item>
		/// </list>
		///          
		///     <br> Icons are loaded based on the name, from Rockets/Textures/Icons. </br>
		///     <br> Text is loaded from the localization files. </br>
		/// </br> </br>
		/// </summary>
		private static void LoadData()
        {
			Add("Sun",
                new GravityInfoElement(28f),
                new RadiusInfoElement(695700f),

                new ThreatLevelInfoElement(ThreatLevel.Apollyon, Color.Red)
            );

            Add("Vulcan",
                new DayPeriodInfoElement("TidallyLocked")
            );

            Add("Mercury",
                new GravityInfoElement(0.38f),
                new RadiusInfoElement(2439.7f),
                new DayPeriodInfoElement(58f)
            );

            Add("Venus",
                new GravityInfoElement(0.9f),
                new RadiusInfoElement(6051.8f),
                new DayPeriodInfoElement(116f)
            );

            Add("Earth",  
                new GravityInfoElement(1f),
                new RadiusInfoElement(6371f),
                new DayPeriodInfoElement(1f),

                new ThreatLevelInfoElement(ThreatLevel.Harmless, Color.White)
            );

            Add("Moon",  
                new GravityInfoElement(0.125f),
                new RadiusInfoElement(1737.4f),
                new DayPeriodInfoElement(8f, "TidallyLocked"),

                new ThreatLevelInfoElement(ThreatLevel.Arduous, Color.White),

                new HazardInfoElement("MeteorStorms"),
                new HazardInfoElement("SolarStorms")
            );

            Add("Mars",  
                new GravityInfoElement(0.38f),
                new RadiusInfoElement(3389.5f),
                new DayPeriodInfoElement(24.62f)
            );

            Add("Phobos", 
                 new GravityInfoElement("Unstable")
            );

            Add("Deimos" 
 
            );

            Add("Ceres"
 
            );

            Add("Jupiter",  
                new GravityInfoElement(2.52f),
                new RadiusInfoElement(69911f),
                new DayPeriodInfoElement(0.3f)
            );

            Add("Io"
 
            );

            Add("Europa"

            );

            Add("Saturn",
                new GravityInfoElement(1.065f ),
                new RadiusInfoElement(58232f),
                new DayPeriodInfoElement(0.43f)
            );

            Add("Titan"

            );

            Add("Ouranos",
                new GravityInfoElement(0.89f),
                new RadiusInfoElement(25362f),
                new DayPeriodInfoElement(0.718f)
            );

            Add("Miranda"

            );

            Add("Neptune",
                new GravityInfoElement(1.14f),
                new RadiusInfoElement(24622f),
                new DayPeriodInfoElement(0.671f)
            );

            Add("Triton"

            );

            Add("Pluto",
                new GravityInfoElement(0.064f),
                new RadiusInfoElement(1188.3f),
                new DayPeriodInfoElement(153.3f)
            );

            Add("Charon"

            );

            Add("Eris"

            );

            Add("YanoMoore"

            );
        }
    }
}