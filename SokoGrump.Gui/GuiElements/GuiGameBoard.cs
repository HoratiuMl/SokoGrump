﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.GuiElements;
using NuciXNA.Primitives;

using SokoGrump.GameLogic;
using SokoGrump.Gui.SpriteEffects;
using SokoGrump.Models;
using SokoGrump.Settings;

namespace SokoGrump.Gui.GuiElements
{
    /// <summary>
    /// World map GUI element.
    /// </summary>
    public class GuiGameBoard : GuiElement
    {
        /// <summary>
        /// Gets the selected province identifier.
        /// </summary>
        /// <value>The selected province identifier.</value>
        public string SelectedProvinceId { get; private set; }

        GameEngine game;

        TileSpriteSheetEffect tileEffect;
        Dictionary<int, TextureSprite> terrainSprites;
        TextureSprite playerSprite;

        public GuiGameBoard(GameEngine game)
        {
            this.game = game;
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        public override void LoadContent()
        {
            terrainSprites = new Dictionary<int, TextureSprite>();
            playerSprite = new TextureSprite
            {
                ContentFile = "Tiles/player/player",
                SourceRectangle = new Rectangle2D(0, 0, GameDefines.MapTileSize, GameDefines.MapTileSize)
            };

            tileEffect = new TileSpriteSheetEffect(game);

            foreach (Tile tile in game.GetTiles())
            {
                TextureSprite tileSprite = new TextureSprite
                {
                    ContentFile = tile.SpriteSheet,
                    SourceRectangle = new Rectangle2D(0, 0, GameDefines.MapTileSize, GameDefines.MapTileSize),
                    SpriteSheetEffect = tileEffect,
                    Active = true
                };

                tileSprite.LoadContent();
                tileSprite.SpriteSheetEffect.Activate();

                terrainSprites.Add(tile.Id, tileSprite);
            }

            playerSprite.LoadContent();
            base.LoadContent();
        }

        /// <summary>
        /// Unloads the content.
        /// </summary>
        public override void UnloadContent()
        {
            terrainSprites.Values.ToList().ForEach(x => x.UnloadContent());
            playerSprite.UnloadContent();

            terrainSprites.Clear();

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Player player = game.GetPlayer();

            playerSprite.Location = new Point2D(
                player.Location.X * GameDefines.MapTileSize,
                player.Location.Y * GameDefines.MapTileSize);
        }

        /// <summary>
        /// Draw the content on the specified spriteBatch.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < GameDefines.BoardHeight; y++)
            {
                for (int x = 0; x < GameDefines.BoardWidth; x++)
                {
                    Tile tile = game.GetTile(x, y);
                    
                    TextureSprite terrainSprite = terrainSprites[tile.Id];
                    terrainSprite.Location = new Point2D(x * GameDefines.MapTileSize, y * GameDefines.MapTileSize);

                    // TODO: This is temporary
                    if (tile.Id == 0)
                    {
                        tileEffect.TileLocation = new Point2D(x, y);
                        tileEffect.TilesWith = new List<int> { 0, 2, 3, 5 };
                        tileEffect.UpdateFrame(null);

                        terrainSprite.SourceRectangle = new Rectangle2D(
                            tileEffect.CurrentFrame.X * GameDefines.MapTileSize,
                            tileEffect.CurrentFrame.Y * GameDefines.MapTileSize,
                            GameDefines.MapTileSize,
                            GameDefines.MapTileSize);
                    }

                    terrainSprite.Draw(spriteBatch);
                }
            }
            playerSprite.Draw(spriteBatch);

            base.Draw(spriteBatch);
        }
    }
}
