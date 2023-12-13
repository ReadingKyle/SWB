using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RetroReadingRacing.Screens;
using static System.TimeZoneInfo;
using SWB.Screens;
using SWB.StateManagement;

namespace SWB.Screens
{
    // Base class for screens that contain a menu of options. The user can
    // move up and down to select an entry, or cancel to back out of the screen.
    public class TitleScreen : GameScreen
    {
        private ContentManager _content;

        private InputAction _playGame;
        private InputAction _quitGame;
        private InputAction _credits;

        private Texture2D _bg;

        private int _animationFrame;
        private double _animationTimer;

        public override void Activate()
        {
            base.Activate();

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _playGame = new InputAction(
                new[] { Keys.Space }, true);
            _quitGame = new InputAction(
                new[] { Keys.Escape }, true);

            if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _bg = _content.Load<Texture2D>("fieldpng");
        }

        // Responds to user input, changing the selected entry and accepting or cancelling the menu.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            if (_playGame.Occurred(input, ControllingPlayer, out playerIndex))
            {
                LoadingScreen.Load(ScreenManager, true, new MenuScreen());
            }
            else if (_quitGame.Occurred(input, ControllingPlayer, out playerIndex))
            {
                ScreenManager.Game.Exit();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _animationTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (_animationTimer < 0)
            {
                _animationFrame++;
                if (_animationFrame > 2)
                {
                    _animationFrame = 0;
                }
                _animationTimer = 0.2;
            }
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(
            _bg,
            new Vector2(0, 0),
            new Rectangle(_animationFrame * 256, 0, 256, 224),
                Color.White,
                0,
                new Vector2(0, 0),
                2f,
                SpriteEffects.None,
                0
            );

            spriteBatch.DrawString(ScreenManager.Font, "Press Space to Play", new Vector2(25, 300), Color.Purple);
            spriteBatch.DrawString(ScreenManager.Font, "Press ESC to Quit", new Vector2(25, 350), Color.White);
            spriteBatch.End();
        }
    }
}
