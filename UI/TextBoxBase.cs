﻿using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Chat;

namespace HunterCombatMR.UI
{
    /// <summary>
    /// Shamelessly ripped from UIInputTextField, defines a UI text box that can be manipulated onscreen without triggering the chat.
    /// </summary>
    public abstract class TextBoxBase
        : UIElement
    {
        #region Protected Fields

        protected readonly string _hintText;
        protected string _currentString = string.Empty;
        protected Color _currentTextColor;
        protected int _textBlinkerCount;
        protected Color _textColor;

        #endregion Protected Fields

        #region Public Constructors

        public TextBoxBase(string hintText,
            int maxLength = 0,
            bool isLarge = false,
            bool startHidden = false,
            string defaultText = null)
        {
            _hintText = hintText;
            MaxLength = maxLength;
            IsLarge = isLarge;
            Hidden = startHidden;
            DefaultText = defaultText;
            HintTextColor = new Color(200f, 200f, 200f, 50f);
            OnMouseOver += ((x, y) => { (y as TextBoxBase)._currentTextColor = HoverColor; });
            OnMouseOut += ((x, y) => { (y as TextBoxBase)._currentTextColor = _textColor; });
            OnClick += delegate { StartInteracting(); };
        }

        #endregion Public Constructors

        #region Public Delegates

        public delegate void EventHandler(object sender, EventArgs e);

        #endregion Public Delegates

        #region Public Events

        public event EventHandler OnTextChange;

        #endregion Public Events

        #region Public Properties

        public string DefaultText { get; set; }

        public bool Hidden { get; set; }
        public Color HintTextColor { get; set; }
        public Color HoverColor { get; set; } = Color.Aqua;
        public bool Interacting { get; protected set; }
        public bool IsLarge { get; set; }

        public int MaxLength { get; set; }

        public string Text
        {
            get
            {
                return _currentString;
            }
            set
            {
                if (_currentString != value)
                {
                    UpdateString(value);
                }
            }
        }

        public Color TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                _textColor = value;
                if (HoverColor == null || _currentTextColor != HoverColor)
                    _currentTextColor = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public virtual void StartInteracting()
        {
            Interacting = true;
        }

        public virtual void StopInteracting()
        {
            Interacting = false;
        }

        #endregion Public Methods

        #region Protected Methods

        protected abstract void DrawDetails(string newString);

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Hidden)
                return;

            if (Interacting)
            {
                Main.chatRelease = false;
                Main.drawingPlayerChat = false;
                PlayerInput.WritingText = true;
                Main.instance.HandleIME();
                string newString = Main.GetInputText(_currentString);

                if (newString.Length > MaxLength)
                {
                    newString = newString.Substring(0, MaxLength);
                }

                DrawDetails(newString);

                if (Main.inputTextEnter || Main.inputTextEscape)
                    Interacting = false;
            }
            string displayString = _currentString;
            if (++_textBlinkerCount / 20 % 2 == 0 && Interacting)
            {
                displayString += "|";
            }
            CalculatedStyle space = GetDimensions();
            if (_currentString.Length == 0)
            {
                var sizeFont = Main.fontMouseText;
                if (IsLarge)
                {
                    sizeFont = Main.fontDeathText;
                    Utils.DrawBorderStringBig(spriteBatch, _hintText, new Vector2(space.X, space.Y), HintTextColor);
                    if (displayString.Length > 0)
                        Utils.DrawBorderStringBig(spriteBatch, displayString, new Vector2(space.X, space.Y), _currentTextColor);
                }
                else
                {
                    Utils.DrawBorderString(spriteBatch, _hintText, new Vector2(space.X, space.Y), HintTextColor);
                    if (displayString.Length > 0)
                        Utils.DrawBorderString(spriteBatch, displayString, new Vector2(space.X, space.Y), _currentTextColor);
                }
                Vector2 textSize = ChatManager.GetStringSize(sizeFont, _hintText, new Vector2(1f));
                MinWidth.Set(textSize.X + PaddingLeft + PaddingRight, 0f);
                MinHeight.Set(textSize.Y + PaddingTop + PaddingBottom, 0f);
            }
            else
            {
                if (IsLarge)
                    Utils.DrawBorderStringBig(spriteBatch, displayString, new Vector2(space.X, space.Y), _currentTextColor);
                else
                    Utils.DrawBorderString(spriteBatch, displayString, new Vector2(space.X, space.Y), _currentTextColor);
            }

            if (!Interacting && _currentString.Length <= 0 && !string.IsNullOrWhiteSpace(DefaultText))
            {
                _currentString = DefaultText;
            }
        }

        #endregion Protected Methods

        #region protected Methods

        protected void UpdateString(string newString)
        {
            _currentString = newString;
            OnTextChange?.Invoke(this, EventArgs.Empty);
            Vector2 textSize = ChatManager.GetStringSize(IsLarge ? Main.fontDeathText : Main.fontMouseText, _currentString, new Vector2(1f));
            MinWidth.Set(textSize.X + PaddingLeft + PaddingRight, 0f);
            MinHeight.Set(textSize.Y + PaddingTop + PaddingBottom, 0f);
        }

        #endregion protected Methods
    }
}