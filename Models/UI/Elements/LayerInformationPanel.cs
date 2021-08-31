using HunterCombatMR.Builders.Animation;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace HunterCombatMR.Models.UI.Elements
{
    public class LayerInformationPanel
        : CollapsingUIPanel
    {
        private LayerTextInfo _infoFlags;
        private IEnumerable<LayerInfoTextBox> _textBoxes;
        private LayerBuilder layerRef;

        public LayerInformationPanel(bool startCollapsed)
            : base(startCollapsed)
        {
            InformationList = new UIList()
            {
                Width = new StyleDimension(0f, 1f),
                Height = new StyleDimension(0f, 1f)
            };
            Append(InformationList);

            var scrollbar = new UIScrollbar()
            {
                HAlign = 1f,
                Height = new StyleDimension(-20f, 1f),
                Top = new StyleDimension(10f, 0f)
            };
            scrollbar.SetView(100f, 1000f);
            Append(scrollbar);
            InformationList.SetScrollbar(scrollbar);
        }

        public UIList InformationList { get; }
        public LayerBuilder LayerRef { get => layerRef; set { layerRef = value; ResetInformationDisplay(); PopulateBoxes(); } }

        internal LayerTextInfo VisableInformation
        {
            get
            {
                return _infoFlags;
            }
            set
            {
                _infoFlags = value;
                ResetInformationDisplay();
            }
        }

        public static LayerInformationPanel Default(StyleDimension left,
            StyleDimension top)
        {
            LayerInformationPanel panel = new LayerInformationPanel(true)
            {
                Left = left,
                Top = top,
                Width = new StyleDimension(0f, 0.15f),
                MinWidth = new StyleDimension(50f, 0f),
                Height = new StyleDimension(0f, 0.15f),
                MinHeight = new StyleDimension(150f, 0f),
                VisableInformation = LayerTextInfo.All
            };

            return panel;
        }

        public override void OnInitialize()
        {
            ResetInformationDisplay();
        }

        public void ResetInformationDisplay()
        {
            InformationList.Clear();

            var boxes = new List<LayerInfoTextBox>();

            var infoBlocks = _infoFlags.GetFlags();

            foreach (LayerTextInfo block in infoBlocks.Where(x => !x.Equals(LayerTextInfo.None)))
            {
                InformationList.Add(new LayerText(LayerRef, block));
                if (block == LayerTextInfo.Coordinates)
                {
                    var box1 = new LayerInfoTextBox("0", block, 4, false, false, null)
                    { TextColor = Color.White, PositiveIntegersOnly = false, ZeroNotAllowed = false };
                    boxes.Add(box1);
                    InformationList.Add(box1);
                    var box2 = new LayerInfoTextBox("0", block, 4, false, false, null, 1)
                    { TextColor = Color.White, PositiveIntegersOnly = false, ZeroNotAllowed = false };
                    boxes.Add(box2);
                    InformationList.Add(box2);
                }
            }
            _textBoxes = boxes;
        }

        private void PopulateBoxes()
        {
            foreach (var box in _textBoxes)
            {
                box.LayerRef = LayerRef;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (IsCollapsed || VisableInformation.Equals(LayerTextInfo.None))
                return;

            base.Update(gameTime);
        }
    }
}