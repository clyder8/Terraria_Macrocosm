﻿using Macrocosm.Common.Config;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.UI.Navigation.Info
{
    public abstract class ValueUnitSpecialInfoElement : InfoElement
    {
        protected LocalizedText formattedLocalizedText;

        protected string infoElementKey => GetType().Name.Replace("InfoElement", "");

        private const string specialValuesPath = "Mods.Macrocosm.UI.Rocket.Special.";

        public ValueUnitSpecialInfoElement(string specialValueKey) : base(specialValueKey)
        {
            formattedLocalizedText = Language.GetText(specialValuesPath + base.specialValueKey);
        }

        public ValueUnitSpecialInfoElement(float value, string specialValueKey = "") : base(value, specialValueKey)
        {
            // The formatted localization texts that have units are updated
            // whenever the measure system configuration is updated
            ClientConfig.Instance.OnConfigChanged += OnConfigChanged;

            FormatValueUnitText(value, specialValueKey);
        }

        // Regenerate the formatted localized text whenever config is changed
        private void OnConfigChanged(object sender, EventArgs e)
        {
            FormatValueUnitText(value, specialValueKey);
        }

        private void FormatValueUnitText(float value, string specialValueKey = "")
        {
            bool hasSpecial = specialValueKey != "";

            // Parantheses are passed as format parameters into the localization
            // when the special value is existent alongside the numeric value  
            string extra1 = hasSpecial ? "(" : "";
            string extra2 = hasSpecial ? ")" : "";

            // Get the extra info and display it between parantheses if a special value key is provided alongside the numeric value
            LocalizedText extraInfo = hasSpecial ? Language.GetOrRegister(specialValuesPath + base.specialValueKey, () => Utility.PrettyPrintName(base.specialValueKey)) : LocalizedText.Empty;

            // Each InfoElement gets the localized text associated with the value based on the current configuration, and may update the value as well
            formattedLocalizedText = GetLocalizedValueUnitText(ref value).WithFormatArgs(value, extra1, extraInfo, extra2);
        }

        /// <summary> 
        /// Provides the localized text generated by this value, allows you to determine
        /// the units used based on the current configuration and update the actual value 
        /// </summary>
        protected abstract LocalizedText GetLocalizedValueUnitText(ref float value);

        protected override LocalizedColorScaleText GetText() => new(formattedLocalizedText, scale: 0.9f);

        protected override LocalizedText GetHoverText() => Language.GetText("Mods.Macrocosm.UI.Rocket.Navigation." + infoElementKey + ".Name");

        protected override Asset<Texture2D> GetIcon() => ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Textures/WorldInfo/" + infoElementKey);

    }
}
