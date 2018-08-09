using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EmmySpel
{
    class InputManager
    {
        public InputMode InputMode { get; set; }

        public InputManager(InputMode inputMode)
        {
            InputMode = inputMode;
        }

        public Vector2 GetMovementDirection()
        {
            switch (InputMode)
            {
                case InputMode.KeyBoard:
                    return GetKeyBoardDirection(Keyboard.GetState());

                case InputMode.Gamepad:
                    return GetGamepadDirection(GamePad.GetState(PlayerIndex.One));

                default:
                    throw new ArgumentException("unkown input mode");
            }
        }

        private Vector2 GetGamepadDirection(GamePadState gamePadState)
        {
            var direction = gamePadState.ThumbSticks.Left;
            direction.Y = -direction.Y; //the thumbstick is inverted in the y axis
            return direction;
        }

        private Vector2 GetKeyBoardDirection(KeyboardState keyState)
        {
            Vector2 movement = Vector2.Zero;

            if (keyState.IsKeyDown(Keys.W))
            {
                movement.Y -= 1;
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                movement.Y += 1;
            }
            if (keyState.IsKeyDown(Keys.A))
            {
                movement.X -= 1;
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                movement.X += 1;
            }
            return movement;
        }

        public bool IsKeyDown(Keys keyBoardKey, Buttons gamePadButton)
        {
            switch (InputMode)
            {
                case InputMode.KeyBoard:
                    return Keyboard.GetState().IsKeyDown(keyBoardKey);

                case InputMode.Gamepad:
                    return GamePad.GetState(PlayerIndex.One).IsButtonDown(gamePadButton);

                default:
                    throw new ArgumentException("unkown input mode");
            }
        }
    }
}
