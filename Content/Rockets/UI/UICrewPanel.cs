﻿using Macrocosm.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
    public class UICrewPanel : UIListScrollablePanel, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; }

        private Connector connector;
		private Player commander;
		private Player prevCommander = Main.LocalPlayer;
		private List<Player> crew = new();
		private List<Player> prevCrew = new();

		public UICrewPanel() : base(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Crew"), scale: 1.2f))
        {
			connector = new(0);
		}

        public override void OnInitialize()
        {
            base.OnInitialize();
            BorderColor = new Color(89, 116, 213, 255);
            BackgroundColor = new Color(53, 72, 135);

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Add(new UIPlayerInfoElement(Main.LocalPlayer));
            } 
        }


		protected override void DrawChildren(SpriteBatch spriteBatch)
		{
			base.DrawChildren(spriteBatch);
		}

		public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Main.netMode == NetmodeID.MultiplayerClient || true)
            {
				crew.Clear();

				for (int i = 0; i < Main.maxPlayers; i++)
                {
                    var player = Main.player[i];

                    if (!player.active)
                        continue;

                    var rocketPlayer = player.GetModPlayer<RocketPlayer>();

                    if (rocketPlayer.InRocket && rocketPlayer.RocketID == Rocket.WhoAmI)
                    {
                        if (rocketPlayer.IsCommander)
                            commander = player;
                        else
                             crew.Add(player);
                    }
                }

				crew.Add(new());
				crew.Add(new());
				crew.Add(new());

				if (!commander.Equals(prevCommander) || !crew.SequenceEqual(prevCrew))
				{
					Deactivate();
					ClearList();

					connector = new(crew.Count) { };
					Add(connector);

					Add(new UIPlayerInfoElement(commander));

					foreach(var player in crew)
                    {
                         Add(new UIPlayerInfoElement(player));
                    }

					prevCommander = commander;
					prevCrew = crew;

					Activate();
				}
			}
        }

        private class Connector : UIElement
        {
			private int count;

			public Connector(int count)
			{
				this.count = count;
			}

			/*
			public override void OnInitialize()
			{
				base.OnInitialize();
			}
			*/

			public override void Draw(SpriteBatch spriteBatch)
			{
				base.Draw(spriteBatch);

				var dimensions = Parent.GetDimensions();
				Rectangle rect = new((int)(dimensions.X + dimensions.Width * 0.1f), (int)(dimensions.Y + dimensions.Height * 0.35f), 20, 14 + 48 * count);
				spriteBatch.Draw(TextureAssets.BlackTile.Value, rect, Color.White);

				for (int i = 0; i < count; i++)
				{
					rect = new((int)(dimensions.X + dimensions.Width * 0.1f), (int)(dimensions.Y + dimensions.Height * 0.438f + 64 * i), 68, 20);
					spriteBatch.Draw(TextureAssets.BlackTile.Value, rect, Color.White);
				}
			}
		}
    }
}
