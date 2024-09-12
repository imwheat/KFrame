//****************** 代码文件申明 ***********************
//* 文件：KGUIStyles
//* 作者：wheat
//* 创建时间：2024/05/28 18:57:25 星期二
//* 描述：包含一些绘制编辑器GUI时使用的GUIStyle
//*******************************************************

using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.ComponentModel;

namespace KFrame.Editor
{
    /// <summary>
    /// 包含一些绘制编辑器GUI时使用的GUIStyle
    /// </summary>
    public static class KGUIStyles
    {
        private static readonly Type GeneralDrawerConfig_Type;

        private static readonly PropertyInfo GeneralDrawerConfig_Instance_Prop;

        private static readonly PropertyInfo GeneralDrawerConfig_ListItemColorEvenDarkSkin_Prop;

        private static readonly PropertyInfo GeneralDrawerConfig_ListItemColorEvenLightSkin_Prop;

        private static readonly PropertyInfo GeneralDrawerConfig_ListItemColorOddDarkSkin_Prop;

        private static readonly PropertyInfo GeneralDrawerConfig_ListItemColorOddLightSkin_Prop;

        //
        // 摘要:
        //     Validator Green
        public static Color ValidatorGreen;

        //
        // 摘要:
        //     Inspector Orange
        public static Color InspectorOrange;

        //
        // 摘要:
        //     Serializer Yellow
        public static Color SerializerYellow;

        //
        // 摘要:
        //     Green valid color
        public static Color GreenValidColor;

        //
        // 摘要:
        //     Red error color
        public static Color RedErrorColor;

        //
        // 摘要:
        //     Yellow warning color
        public static Color YellowWarningColor;

        //
        // 摘要:
        //     Border color.
        public static readonly Color BorderColor;

        //
        // 摘要:
        //     Box background color.
        public static readonly Color BoxBackgroundColor;

        //
        // 摘要:
        //     Dark editor background color.
        public static readonly Color DarkEditorBackground;

        //
        // 摘要:
        //     Editor window background color.
        public static readonly Color EditorWindowBackgroundColor;

        //
        // 摘要:
        //     Menu background color.
        public static readonly Color MenuBackgroundColor;

        //
        // 摘要:
        //     Header box background color.
        public static readonly Color HeaderBoxBackgroundColor;

        //
        // 摘要:
        //     Highlighted Button Color.
        public static readonly Color HighlightedButtonColor;

        //
        // 摘要:
        //     Highlight text color.
        public static readonly Color HighlightedTextColor;

        //
        // 摘要:
        //     Highlight property color.
        public static readonly Color HighlightPropertyColor;

        //
        // 摘要:
        //     List item hover color for every other item.
        public static readonly Color ListItemColorHoverEven;

        //
        // 摘要:
        //     List item hover color for every other item.
        public static readonly Color ListItemColorHoverOdd;

        //
        // 摘要:
        //     List item drag background color.
        public static readonly Color ListItemDragBg;

        //
        // 摘要:
        //     List item drag background color.
        public static readonly Color ListItemDragBgColor;

        //
        // 摘要:
        //     Column title background colors.
        public static readonly Color ColumnTitleBg;

        //
        // 摘要:
        //     The default background color for when a menu item is selected.
        public static readonly Color DefaultSelectedMenuTreeColorDarkSkin;

        //
        // 摘要:
        //     The default background color for when a menu item is selected.
        public static readonly Color DefaultSelectedInactiveMenuTreeColorDarkSkin;

        //
        // 摘要:
        //     The default background color for when a menu item is selected.
        public static readonly Color DefaultSelectedMenuTreeColorLightSkin;

        //
        // 摘要:
        //     The default background color for when a menu item is selected.
        public static readonly Color DefaultSelectedInactiveMenuTreeColorLightSkin;

        //
        // 摘要:
        //     A mouse over background overlay color.
        public static readonly Color MouseOverBgOverlayColor;

        private static Color? listItemColorEven;

        private static Color? listItemColorOdd;

        //
        // 摘要:
        //     Menu button active background color.
        public static readonly Color MenuButtonActiveBgColor;

        //
        // 摘要:
        //     Menu button border color.
        public static readonly Color MenuButtonBorderColor;

        //
        // 摘要:
        //     Menu button color.
        public static readonly Color MenuButtonColor;

        //
        // 摘要:
        //     Menu button hover color.
        public static readonly Color MenuButtonHoverColor;

        //
        // 摘要:
        //     A light border color.
        public static readonly Color LightBorderColor;

        //
        // 摘要:
        //     Bold label style.
        public static GUIStyle Temporary;

        private static GUIStyle boldLabel;

        private static GUIStyle boldLabelCentered;

        private static GUIStyle boxContainer;

        private static GUIStyle boxHeaderStyle;

        private static GUIStyle button;

        private static GUIStyle buttonLeft;

        private static GUIStyle buttonMid;

        private static GUIStyle buttonRight;

        private static GUIStyle miniButton;

        private static GUIStyle colorFieldBackground;

        private static GUIStyle foldout;

        private static GUIStyle iconButton;

        private static GUIStyle label;

        private static GUIStyle labelCentered;

        private static GUIStyle whiteLabelCentered;

        private static GUIStyle leftAlignedGreyLabel;

        private static GUIStyle leftAlignedGreyMiniLabel;

        private static GUIStyle leftAlignedWhiteMiniLabel;

        private static GUIStyle listItem;

        private static GUIStyle menuButtonBackground;

        private static GUIStyle none;

        private static GUIStyle paddingLessBox;

        private static GUIStyle contentPadding;

        private static GUIStyle propertyPadding;

        private static GUIStyle propertyMargin;

        private static GUIStyle richTextLabel;

        private static GUIStyle rightAlignedGreyMiniLabel;

        private static GUIStyle rightAlignedWhiteMiniLabel;

        private static GUIStyle sectionHeader;

        private static GUIStyle sectionHeaderCentered;

        private static GUIStyle toggleGroupBackground;

        private static GUIStyle toggleGroupCheckbox;

        private static GUIStyle toggleGroupPadding;

        private static GUIStyle toggleGroupTitleBg;

        private static GUIStyle toolbarBackground;

        private static GUIStyle toolbarButton;

        private static GUIStyle toolbarButtonSelected;

        private static GUIStyle toolbarSeachCancelButton;

        private static GUIStyle toolbarSeachTextField;

        private static GUIStyle toolbarTab;

        private static GUIStyle title;

        private static GUIStyle boldTitle;

        private static GUIStyle subtitle;

        private static GUIStyle titleRight;

        private static GUIStyle titleCentered;

        private static GUIStyle boldTitleRight;

        private static GUIStyle boldTitleCentered;

        private static GUIStyle subtitleCentered;

        private static GUIStyle subtitleRight;

        private static GUIStyle messageBox;

        private static GUIStyle detailedMessageBox;

        private static GUIStyle multiLineLabel;

        private static GUIStyle odinEditorWrapper;

        private static GUIStyle whiteLabel;

        private static GUIStyle blackLabel;

        private static GUIStyle miniButtonRightSelected;

        private static GUIStyle miniButtonRight;

        private static GUIStyle miniButtonLeftSelected;

        private static GUIStyle miniButtonLeft;

        private static GUIStyle miniButtonSelected;

        private static GUIStyle miniButtonMid;

        private static GUIStyle miniButtonMidSelected;

        private static GUIStyle centeredTextField;

        private static GUIStyle tagButton;

        private static GUIStyle centeredGreyMiniLabel;

        private static GUIStyle leftAlignedCenteredLabel;

        private static GUIStyle popup;

        private static GUIStyle multiLineCenteredLabel;

        private static GUIStyle dropDownMiniutton;

        private static GUIStyle bottomBoxPadding;

        private static GUIStyle paneOptions;

        private static GUIStyle containerOuterShadow;

        private static GUIStyle moduleHeader;

        private static GUIStyle containerOuterShadowGlow;

        private static GUIStyle cardStyle;

        private static GUIStyle centeredWhiteMiniLabel;

        private static GUIStyle centeredBlackMiniLabel;

        private static GUIStyle miniLabelCentered;

        //
        // 摘要:
        //     SDFIconButton Label.
        public static GUIStyle SDFIconButtonLabelStyle;

        //
        // 摘要:
        //     The default background color for when a menu item is selected.
        public static Color DefaultSelectedMenuTreeColor
        {
            get
            {
                if (!EditorGUIUtility.isProSkin)
                {
                    return DefaultSelectedMenuTreeColorLightSkin;
                }

                return DefaultSelectedMenuTreeColorDarkSkin;
            }
        }

        //
        // 摘要:
        //     The default background color for when a menu item is selected.
        public static Color DefaultSelectedMenuTreeInactiveColor
        {
            get
            {
                if (!EditorGUIUtility.isProSkin)
                {
                    return DefaultSelectedInactiveMenuTreeColorLightSkin;
                }

                return DefaultSelectedInactiveMenuTreeColorDarkSkin;
            }
        }

        //
        // 摘要:
        //     List item background color for every other item. OBSOLETE: Use ListItemColorEven
        //     instead.
        [Obsolete("Use ListItemColorEven instead.", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Color ListItemEven => ListItemColorEven;

        //
        // 摘要:
        //     List item background color for every other item. OBSOLETE: Use ListItemColorOdd
        //     instead.
        [Obsolete("Use ListItemColorOdd instead.", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Color ListItemOdd => ListItemColorOdd;

        //
        // 摘要:
        //     List item color for every other item.
        public static Color ListItemColorEven
        {
            get
            {
                if (!listItemColorEven.HasValue)
                {
                    listItemColorEven = (EditorGUIUtility.isProSkin ? GetGeneralConfigDefaultColor(GeneralDrawerConfig_ListItemColorEvenDarkSkin_Prop, new Color(0.235f, 0.235f, 0.235f, 1f)) : GetGeneralConfigDefaultColor(GeneralDrawerConfig_ListItemColorEvenLightSkin_Prop, new Color(0.838f, 0.838f, 0.838f, 1f)));
                }

                return listItemColorEven.Value;
            }
            set
            {
                listItemColorEven = value;
            }
        }

        //
        // 摘要:
        //     List item color for every other item.
        public static Color ListItemColorOdd
        {
            get
            {
                if (!listItemColorOdd.HasValue)
                {
                    listItemColorOdd = (EditorGUIUtility.isProSkin ? GetGeneralConfigDefaultColor(GeneralDrawerConfig_ListItemColorOddDarkSkin_Prop, new Color(0.235f, 0.235f, 0.235f, 1f)) : GetGeneralConfigDefaultColor(GeneralDrawerConfig_ListItemColorOddLightSkin_Prop, new Color(0.788f, 0.788f, 0.788f, 1f)));
                }

                return listItemColorOdd.Value;
            }
            set
            {
                listItemColorOdd = value;
            }
        }

        //
        // 摘要:
        //     Tag Button style.
        public static GUIStyle TagButton
        {
            get
            {
                if (tagButton == null)
                {
                    tagButton = new GUIStyle("MiniToolbarButton")
                    {
                        alignment = TextAnchor.MiddleCenter,
                        padding = new RectOffset(),
                        margin = new RectOffset(),
                        contentOffset = new Vector2(0f, 0f),
                        fontSize = 9,
                        font = EditorStyles.standardFont
                    };
                }

                return tagButton;
            }
        }

        //
        // 摘要:
        //     Bold label style.
        public static GUIStyle BoldLabel
        {
            get
            {
                if (boldLabel == null)
                {
                    boldLabel = new GUIStyle(EditorStyles.boldLabel)
                    {
                        contentOffset = new Vector2(0f, 0f),
                        margin = new RectOffset(0, 0, 0, 0)
                    };
                }

                return boldLabel;
            }
        }

        //
        // 摘要:
        //     Centered bold label style.
        public static GUIStyle BoldLabelCentered
        {
            get
            {
                if (boldLabelCentered == null)
                {
                    boldLabelCentered = new GUIStyle(BoldLabel)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }

                return boldLabelCentered;
            }
        }

        //
        // 摘要:
        //     Box container style.
        public static GUIStyle BoxContainer
        {
            get
            {
                if (boxContainer == null)
                {
                    boxContainer = new GUIStyle(EditorStyles.helpBox)
                    {
                        margin = new RectOffset(0, 0, 0, 2)
                    };
                }

                return boxContainer;
            }
        }

        //
        // 摘要:
        //     Popup style.
        public static GUIStyle Popup
        {
            get
            {
                if (popup == null)
                {
                    popup = new GUIStyle(EditorStyles.miniButton)
                    {
                        alignment = TextAnchor.MiddleLeft
                    };
                }

                return popup;
            }
        }

        //
        // 摘要:
        //     Box header style.
        public static GUIStyle BoxHeaderStyle
        {
            get
            {
                if (boxHeaderStyle == null)
                {
                    boxHeaderStyle = new GUIStyle(None)
                    {
                        margin = new RectOffset(0, 0, 0, 2)
                    };
                }

                return boxHeaderStyle;
            }
        }

        //
        // 摘要:
        //     Button style.
        public static GUIStyle Button
        {
            get
            {
                if (button == null)
                {
                    button = new GUIStyle("Button");
                    button.clipping = TextClipping.Clip;
                }

                return button;
            }
        }

        //
        // 摘要:
        //     Button selected style.
        [Obsolete("Use Button and draw its selected state.", false)]
        public static GUIStyle ButtonSelected => Button;

        //
        // 摘要:
        //     Left button style.
        public static GUIStyle ButtonLeft
        {
            get
            {
                if (buttonLeft == null)
                {
                    buttonLeft = new GUIStyle("ButtonLeft");
                    buttonLeft.normal = buttonLeft.onNormal;
                }

                return buttonLeft;
            }
        }

        //
        // 摘要:
        //     Left button selected style.
        [Obsolete("Use ButtonLeft and draw its selected state.", false)]
        public static GUIStyle ButtonLeftSelected => ButtonLeft;

        //
        // 摘要:
        //     Mid button style.
        public static GUIStyle ButtonMid
        {
            get
            {
                if (buttonMid == null)
                {
                    buttonMid = new GUIStyle("ButtonMid");
                    buttonMid.normal = buttonMid.onNormal;
                }

                return buttonMid;
            }
        }

        //
        // 摘要:
        //     Mid button selected style.
        [Obsolete("Use ButtonMid and draw its selected state.", false)]
        public static GUIStyle ButtonMidSelected => ButtonMid;

        //
        // 摘要:
        //     Right button style.
        public static GUIStyle ButtonRight
        {
            get
            {
                if (buttonRight == null)
                {
                    buttonRight = new GUIStyle("ButtonRight");
                }

                return buttonRight;
            }
        }

        //
        // 摘要:
        //     Right button selected style.
        [Obsolete("Use ButtonRight and draw its selected state.", false)]
        public static GUIStyle ButtonRightSelected => ButtonRight;

        //
        // 摘要:
        //     Pane Options Button
        public static GUIStyle DropDownMiniButton
        {
            get
            {
                if (dropDownMiniutton == null)
                {
                    dropDownMiniutton = new GUIStyle(EditorStyles.popup);
                }

                return dropDownMiniutton;
            }
        }

        //
        // 摘要:
        //     Left button style.
        public static GUIStyle MiniButton
        {
            get
            {
                if (miniButton == null)
                {
                    miniButton = new GUIStyle(EditorStyles.miniButton);
                    miniButton.normal = miniButton.onNormal;
                }

                return miniButton;
            }
        }

        //
        // 摘要:
        //     Left button selected style.
        [Obsolete("Use MiniButton and draw its selected state.", false)]
        public static GUIStyle MiniButtonSelected => MiniButton;

        //
        // 摘要:
        //     Left button style.
        public static GUIStyle MiniButtonLeft
        {
            get
            {
                if (miniButtonLeft == null)
                {
                    miniButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
                    miniButtonLeft.normal = miniButtonLeft.onNormal;
                }

                return miniButtonLeft;
            }
        }

        //
        // 摘要:
        //     Left button selected style.
        [Obsolete("Use MiniButtonLeft and draw its selected state.", false)]
        public static GUIStyle MiniButtonLeftSelected => MiniButtonLeft;

        //
        // 摘要:
        //     Mid button style.
        public static GUIStyle MiniButtonMid
        {
            get
            {
                if (miniButtonMid == null)
                {
                    miniButtonMid = new GUIStyle(EditorStyles.miniButtonMid);
                    miniButtonMid.normal = miniButtonMid.onNormal;
                }

                return miniButtonMid;
            }
        }

        //
        // 摘要:
        //     Mid button selected style.
        [Obsolete("Use MiniButtonMid and draw its selected state.", false)]
        public static GUIStyle MiniButtonMidSelected => MiniButtonMid;

        //
        // 摘要:
        //     Right button style.
        public static GUIStyle MiniButtonRight
        {
            get
            {
                if (miniButtonRight == null)
                {
                    miniButtonRight = new GUIStyle(EditorStyles.miniButtonRight);
                    miniButtonRight.normal = miniButtonRight.onNormal;
                }

                return miniButtonRight;
            }
        }

        //
        // 摘要:
        //     Right button selected style.
        [Obsolete("Use iniButtonRight and draw its selected state.", false)]
        public static GUIStyle MiniButtonRightSelected => MiniButtonRight;

        //
        // 摘要:
        //     Color field background style.
        public static GUIStyle ColorFieldBackground
        {
            get
            {
                if (colorFieldBackground == null)
                {
                    colorFieldBackground = new GUIStyle("ShurikenEffectBg");
                }

                return colorFieldBackground;
            }
        }

        //
        // 摘要:
        //     Foldout style.
        public static GUIStyle Foldout
        {
            get
            {
                if (foldout == null)
                {
                    foldout = new GUIStyle(EditorStyles.foldout)
                    {
                        fixedWidth = 0f,
                        fixedHeight = 0f,
                        stretchHeight = false,
                        stretchWidth = true
                    };
                    if (UnityVersion.IsVersionOrGreater(2019, 3))
                    {
                        foldout.margin = new RectOffset
                        {
                            left = foldout.margin.left,
                            right = foldout.margin.right,
                            top = 1,
                            bottom = 1
                        };
                    }
                }

                return foldout;
            }
        }

        //
        // 摘要:
        //     Icon button style.
        public static GUIStyle IconButton
        {
            get
            {
                if (iconButton == null)
                {
                    iconButton = new GUIStyle(GUIStyle.none)
                    {
                        padding = new RectOffset(1, 1, 1, 1)
                    };
                    GUIStyle gUIStyle = iconButton;
                    GUIStyle gUIStyle2 = iconButton;
                    GUIStyle gUIStyle3 = iconButton;
                    GUIStyle gUIStyle4 = iconButton;
                    GUIStyle gUIStyle5 = iconButton;
                    GUIStyle gUIStyle6 = iconButton;
                    GUIStyle gUIStyle7 = iconButton;
                    GUIStyleState gUIStyleState = (iconButton.onFocused = Button.normal);
                    GUIStyleState gUIStyleState3 = (gUIStyle7.focused = gUIStyleState);
                    GUIStyleState gUIStyleState5 = (gUIStyle6.onActive = gUIStyleState3);
                    GUIStyleState gUIStyleState7 = (gUIStyle5.onHover = gUIStyleState5);
                    GUIStyleState gUIStyleState9 = (gUIStyle4.onNormal = gUIStyleState7);
                    GUIStyleState gUIStyleState11 = (gUIStyle3.active = gUIStyleState9);
                    GUIStyleState gUIStyleState14 = (gUIStyle.normal = (gUIStyle2.hover = gUIStyleState11));
                    Color textColor = iconButton.normal.textColor;
                    textColor.a *= 0.5f;
                    iconButton.normal.textColor = textColor;
                    iconButton.hover.textColor = textColor;
                    iconButton.active.textColor = textColor;
                    iconButton.onNormal.textColor = textColor;
                    iconButton.onHover.textColor = textColor;
                    iconButton.onActive.textColor = textColor;
                    iconButton.focused.textColor = textColor;
                    iconButton.onFocused.textColor = textColor;
                    GUIStyleState hover = iconButton.hover;
                    GUIStyleState onHover = iconButton.onHover;
                    GUIStyleState active = iconButton.active;
                    Color color = (iconButton.hover.textColor = HighlightedTextColor);
                    Color color3 = (active.textColor = color);
                    Color color6 = (hover.textColor = (onHover.textColor = color3));
                }

                return iconButton;
            }
        }

        //
        // 摘要:
        //     Label style.
        public static GUIStyle Label
        {
            get
            {
                if (label == null)
                {
                    label = new GUIStyle(EditorStyles.label)
                    {
                        margin = new RectOffset(0, 0, 0, 0)
                    };
                }

                return label;
            }
        }

        //
        // 摘要:
        //     White label style.
        public static GUIStyle WhiteLabel
        {
            get
            {
                if (whiteLabel == null)
                {
                    whiteLabel = new GUIStyle(EditorStyles.label)
                    {
                        margin = new RectOffset(0, 0, 0, 0)
                    };
                    whiteLabel.normal.textColor = Color.white;
                    whiteLabel.onNormal.textColor = Color.white;
                    whiteLabel.active.textColor = Color.white;
                    whiteLabel.onActive.textColor = Color.white;
                    whiteLabel.hover.textColor = Color.white;
                    whiteLabel.onHover.textColor = Color.white;
                }

                return whiteLabel;
            }
        }

        //
        // 摘要:
        //     Black label style.
        public static GUIStyle BlackLabel
        {
            get
            {
                if (blackLabel == null)
                {
                    blackLabel = new GUIStyle(EditorStyles.label)
                    {
                        margin = new RectOffset(0, 0, 0, 0)
                    };
                    blackLabel.normal.textColor = Color.black;
                    blackLabel.onNormal.textColor = Color.black;
                }

                return blackLabel;
            }
        }

        //
        // 摘要:
        //     Centered label style.
        public static GUIStyle LabelCentered
        {
            get
            {
                if (labelCentered == null)
                {
                    labelCentered = new GUIStyle(Label)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        margin = new RectOffset(0, 0, 0, 0),
                        clipping = TextClipping.Clip
                    };
                }

                return labelCentered;
            }
        }

        //
        // 摘要:
        //     White centered label style.
        public static GUIStyle WhiteLabelCentered
        {
            get
            {
                if (whiteLabelCentered == null)
                {
                    whiteLabelCentered = new GUIStyle(WhiteLabel)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }

                return whiteLabelCentered;
            }
        }

        //
        // 摘要:
        //     Centered mini label style.
        public static GUIStyle MiniLabelCentered
        {
            get
            {
                if (miniLabelCentered == null)
                {
                    miniLabelCentered = new GUIStyle(EditorStyles.miniLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        margin = new RectOffset(0, 0, 0, 0)
                    };
                }

                return miniLabelCentered;
            }
        }

        //
        // 摘要:
        //     Left Aligned Centered Label
        public static GUIStyle LeftAlignedCenteredLabel
        {
            get
            {
                if (leftAlignedCenteredLabel == null)
                {
                    leftAlignedCenteredLabel = new GUIStyle(Label)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        margin = new RectOffset(0, 0, 0, 0)
                    };
                }

                return leftAlignedCenteredLabel;
            }
        }

        //
        // 摘要:
        //     Left aligned grey mini label style.
        public static GUIStyle LeftAlignedGreyMiniLabel
        {
            get
            {
                if (leftAlignedGreyMiniLabel == null)
                {
                    leftAlignedGreyMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        clipping = TextClipping.Clip
                    };
                    if (UnityVersion.IsVersionOrGreater(2019, 3))
                    {
                        leftAlignedGreyMiniLabel.margin = new RectOffset(4, 4, 4, 4);
                    }
                }

                return leftAlignedGreyMiniLabel;
            }
        }

        //
        // 摘要:
        //     Left aligned grey label style.
        public static GUIStyle LeftAlignedGreyLabel
        {
            get
            {
                if (leftAlignedGreyLabel == null)
                {
                    leftAlignedGreyLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        clipping = TextClipping.Clip,
                        fontSize = Label.fontSize
                    };
                    if (UnityVersion.IsVersionOrGreater(2019, 3))
                    {
                        leftAlignedGreyLabel.margin = new RectOffset(4, 4, 4, 4);
                    }
                }

                return leftAlignedGreyLabel;
            }
        }

        //
        // 摘要:
        //     Centered grey mini label
        public static GUIStyle CenteredGreyMiniLabel
        {
            get
            {
                if (centeredGreyMiniLabel == null)
                {
                    centeredGreyMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        clipping = TextClipping.Clip
                    };
                    if (UnityVersion.IsVersionOrGreater(2019, 3))
                    {
                        centeredGreyMiniLabel.margin = new RectOffset(4, 4, 4, 4);
                    }
                }

                return centeredGreyMiniLabel;
            }
        }

        //
        // 摘要:
        //     Left right aligned white mini label style.
        public static GUIStyle LeftAlignedWhiteMiniLabel
        {
            get
            {
                if (leftAlignedWhiteMiniLabel == null)
                {
                    leftAlignedWhiteMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        clipping = TextClipping.Clip,
                        normal = new GUIStyleState
                        {
                            textColor = Color.white
                        }
                    };
                    if (UnityVersion.IsVersionOrGreater(2019, 3))
                    {
                        leftAlignedWhiteMiniLabel.margin = new RectOffset(4, 4, 4, 4);
                    }
                }

                return leftAlignedWhiteMiniLabel;
            }
        }

        //
        // 摘要:
        //     Centered white mini label style.
        public static GUIStyle CenteredWhiteMiniLabel
        {
            get
            {
                if (centeredWhiteMiniLabel == null)
                {
                    centeredWhiteMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        clipping = TextClipping.Clip,
                        normal = new GUIStyleState
                        {
                            textColor = Color.white
                        }
                    };
                    if (UnityVersion.IsVersionOrGreater(2019, 3))
                    {
                        centeredWhiteMiniLabel.margin = new RectOffset(4, 4, 4, 4);
                    }
                }

                return centeredWhiteMiniLabel;
            }
        }

        //
        // 摘要:
        //     Centered black mini label style.
        public static GUIStyle CenteredBlackMiniLabel
        {
            get
            {
                if (centeredBlackMiniLabel == null)
                {
                    centeredBlackMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        clipping = TextClipping.Clip,
                        normal = new GUIStyleState
                        {
                            textColor = Color.black
                        }
                    };
                    if (UnityVersion.IsVersionOrGreater(2019, 3))
                    {
                        centeredBlackMiniLabel.margin = new RectOffset(4, 4, 4, 4);
                    }
                }

                return centeredBlackMiniLabel;
            }
        }

        //
        // 摘要:
        //     List item style.
        public static GUIStyle ListItem
        {
            get
            {
                if (listItem == null)
                {
                    listItem = new GUIStyle(None)
                    {
                        padding = new RectOffset(0, 0, 3, 3)
                    };
                }

                return listItem;
            }
        }

        //
        // 摘要:
        //     Menu button background style.
        public static GUIStyle MenuButtonBackground
        {
            get
            {
                if (menuButtonBackground == null)
                {
                    menuButtonBackground = new GUIStyle
                    {
                        margin = new RectOffset(0, 1, 0, 0),
                        padding = new RectOffset(0, 0, 4, 4),
                        border = new RectOffset(0, 0, 0, 0)
                    };
                }

                return menuButtonBackground;
            }
        }

        //
        // 摘要:
        //     No style.
        public static GUIStyle None
        {
            get
            {
                if (none == null)
                {
                    none = new GUIStyle
                    {
                        margin = new RectOffset(0, 0, 0, 0),
                        padding = new RectOffset(0, 0, 0, 0),
                        border = new RectOffset(0, 0, 0, 0)
                    };
                }

                return none;
            }
        }

        //
        // 摘要:
        //     Odin Editor Wrapper.
        public static GUIStyle OdinEditorWrapper
        {
            get
            {
                if (odinEditorWrapper == null)
                {
                    odinEditorWrapper = new GUIStyle
                    {
                        padding = new RectOffset(4, 4, 0, 0)
                    };
                }

                return odinEditorWrapper;
            }
        }

        //
        // 摘要:
        //     Padding less box style.
        public static GUIStyle PaddingLessBox
        {
            get
            {
                if (paddingLessBox == null)
                {
                    paddingLessBox = new GUIStyle("box")
                    {
                        padding = new RectOffset(1, 1, 0, 0)
                    };
                }

                return paddingLessBox;
            }
        }

        //
        // 摘要:
        //     Content Padding
        public static GUIStyle ContentPadding
        {
            get
            {
                if (contentPadding == null)
                {
                    contentPadding = new GUIStyle
                    {
                        padding = new RectOffset(3, 3, 3, 3)
                    };
                }

                return contentPadding;
            }
        }

        //
        // 摘要:
        //     Property padding.
        public static GUIStyle PropertyPadding
        {
            get
            {
                if (propertyPadding == null)
                {
                    propertyPadding = new GUIStyle(GUIStyle.none)
                    {
                        padding = new RectOffset(0, 0, 0, 3),
                        margin = new RectOffset(0, 0, 0, 0)
                    };
                }

                return propertyPadding;
            }
        }

        //
        // 摘要:
        //     Property margin.
        public static GUIStyle PropertyMargin
        {
            get
            {
                if (propertyMargin == null)
                {
                    RectOffset margin = EditorStyles.objectField.margin;
                    margin = new RectOffset(margin.left, margin.right, margin.top, margin.bottom);
                    propertyMargin = new GUIStyle(GUIStyle.none)
                    {
                        margin = margin
                    };
                }

                return propertyMargin;
            }
        }

        //
        // 摘要:
        //     Rich text label style.
        public static GUIStyle RichTextLabel
        {
            get
            {
                if (richTextLabel == null)
                {
                    richTextLabel = new GUIStyle(EditorStyles.label)
                    {
                        richText = true,
                        wordWrap = true
                    };
                }

                return richTextLabel;
            }
        }

        //
        // 摘要:
        //     Right aligned grey mini label style.
        public static GUIStyle RightAlignedGreyMiniLabel
        {
            get
            {
                if (rightAlignedGreyMiniLabel == null)
                {
                    rightAlignedGreyMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        alignment = TextAnchor.MiddleRight,
                        clipping = TextClipping.Overflow
                    };
                    if (UnityVersion.IsVersionOrGreater(2019, 3))
                    {
                        rightAlignedGreyMiniLabel.margin = new RectOffset(4, 4, 4, 4);
                    }
                }

                return rightAlignedGreyMiniLabel;
            }
        }

        //
        // 摘要:
        //     Right aligned white mini label style.
        public static GUIStyle RightAlignedWhiteMiniLabel
        {
            get
            {
                if (rightAlignedWhiteMiniLabel == null)
                {
                    rightAlignedWhiteMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        alignment = TextAnchor.MiddleRight,
                        clipping = TextClipping.Overflow,
                        normal = new GUIStyleState
                        {
                            textColor = Color.white
                        }
                    };
                    if (UnityVersion.IsVersionOrGreater(2019, 3))
                    {
                        rightAlignedWhiteMiniLabel.margin = new RectOffset(4, 4, 4, 4);
                    }
                }

                return rightAlignedWhiteMiniLabel;
            }
        }

        //
        // 摘要:
        //     Section header style.
        public static GUIStyle SectionHeader
        {
            get
            {
                if (sectionHeader == null)
                {
                    sectionHeader = new GUIStyle(EditorStyles.largeLabel)
                    {
                        fontSize = 22,
                        margin = new RectOffset(0, 0, 5, 0),
                        fontStyle = FontStyle.Normal,
                        wordWrap = true,
                        font = EditorStyles.centeredGreyMiniLabel.font,
                        overflow = new RectOffset(0, 0, 0, 0)
                    };
                }

                return sectionHeader;
            }
        }

        //
        // 摘要:
        //     Section header style.
        public static GUIStyle SectionHeaderCentered
        {
            get
            {
                if (sectionHeaderCentered == null)
                {
                    sectionHeaderCentered = new GUIStyle(SectionHeader)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }

                return sectionHeaderCentered;
            }
        }

        //
        // 摘要:
        //     Toggle group background style.
        public static GUIStyle ToggleGroupBackground
        {
            get
            {
                if (toggleGroupBackground == null)
                {
                    toggleGroupBackground = new GUIStyle(EditorStyles.helpBox)
                    {
                        overflow = new RectOffset(0, 0, 0, 0),
                        margin = new RectOffset(0, 0, 0, 0),
                        padding = new RectOffset(0, 0, 0, 0)
                    };
                }

                return toggleGroupBackground;
            }
        }

        //
        // 摘要:
        //     Toggle group checkbox style.
        public static GUIStyle ToggleGroupCheckbox
        {
            get
            {
                if (toggleGroupCheckbox == null)
                {
                    toggleGroupCheckbox = new GUIStyle("ShurikenCheckMark");
                }

                return toggleGroupCheckbox;
            }
        }

        //
        // 摘要:
        //     Toggle group padding style.
        public static GUIStyle ToggleGroupPadding
        {
            get
            {
                if (toggleGroupPadding == null)
                {
                    toggleGroupPadding = new GUIStyle(GUIStyle.none)
                    {
                        padding = new RectOffset(5, 5, 5, 5)
                    };
                }

                return toggleGroupPadding;
            }
        }

        //
        // 摘要:
        //     Toggle group title background style.
        public static GUIStyle ToggleGroupTitleBg
        {
            get
            {
                if (toggleGroupTitleBg == null)
                {
                    toggleGroupTitleBg = new GUIStyle("ShurikenModuleTitle")
                    {
                        font = new GUIStyle("Label").font,
                        border = new RectOffset(15, 7, 4, 4),
                        fixedHeight = 22f,
                        contentOffset = new Vector2(20f, -2f),
                        margin = new RectOffset(0, 0, 3, 0)
                    };
                }

                return toggleGroupTitleBg;
            }
        }

        //
        // 摘要:
        //     Toolbar background style.
        public static GUIStyle ToolbarBackground
        {
            get
            {
                if (toolbarBackground == null)
                {
                    toolbarBackground = new GUIStyle(EditorStyles.toolbar)
                    {
                        padding = new RectOffset(0, 1, 0, 0),
                        stretchHeight = true,
                        stretchWidth = true,
                        fixedHeight = 0f
                    };
                    if (UnityVersion.IsVersionOrGreater(2019, 3))
                    {
                        toolbarBackground.padding = new RectOffset(0, 0, 0, 0);
                    }
                }

                return toolbarBackground;
            }
            set
            {
                toolbarBackground = value;
            }
        }

        //
        // 摘要:
        //     Toolbar button style.
        public static GUIStyle ToolbarButton
        {
            get
            {
                if (toolbarButton == null)
                {
                    toolbarButton = new GUIStyle(EditorStyles.toolbarButton)
                    {
                        fixedHeight = 0f,
                        alignment = TextAnchor.MiddleCenter,
                        stretchHeight = true,
                        stretchWidth = false,
                        margin = new RectOffset(0, 0, 0, 0)
                    };
                }

                return toolbarButton;
            }
        }

        //
        // 摘要:
        //     Toolbar button selected style.
        public static GUIStyle ToolbarButtonSelected
        {
            get
            {
                if (toolbarButtonSelected == null)
                {
                    toolbarButtonSelected = new GUIStyle(ToolbarButton)
                    {
                        normal = new GUIStyle(ToolbarButton).onNormal
                    };
                }

                return toolbarButtonSelected;
            }
        }

        //
        // 摘要:
        //     Toolbar search cancel button style.
        public static GUIStyle ToolbarSearchCancelButton
        {
            get
            {
                if (toolbarSeachCancelButton == null)
                {
                    toolbarSeachCancelButton = GUI.skin.FindStyle("ToolbarSeachCancelButton") ?? GUI.skin.FindStyle("ToolbarSearchCancelButton");
                }

                return toolbarSeachCancelButton;
            }
        }

        //
        // 摘要:
        //     Toolbar search field style.
        public static GUIStyle ToolbarSearchTextField
        {
            get
            {
                if (toolbarSeachTextField == null)
                {
                    toolbarSeachTextField = GUI.skin.FindStyle("ToolbarSeachTextField") ?? GUI.skin.FindStyle("ToolbarSearchTextField");
                }

                return toolbarSeachTextField;
            }
        }

        //
        // 摘要:
        //     Toolbar tab style.
        public static GUIStyle ToolbarTab
        {
            get
            {
                if (toolbarTab == null)
                {
                    toolbarTab = new GUIStyle(EditorStyles.toolbarButton)
                    {
                        fixedHeight = 0f,
                        stretchHeight = true,
                        stretchWidth = true
                    };
                }

                return toolbarTab;
            }
        }

        //
        // 摘要:
        //     Title style.
        public static GUIStyle Title
        {
            get
            {
                if (title == null)
                {
                    title = new GUIStyle(EditorStyles.label);
                }

                return title;
            }
        }

        //
        // 摘要:
        //     Bold title style.
        public static GUIStyle BoldTitle
        {
            get
            {
                if (boldTitle == null)
                {
                    boldTitle = new GUIStyle(Title)
                    {
                        fontStyle = FontStyle.Bold,
                        padding = new RectOffset(0, 0, 0, 0)
                    };
                }

                return boldTitle;
            }
        }

        //
        // 摘要:
        //     Centered bold title style.
        public static GUIStyle BoldTitleCentered
        {
            get
            {
                if (boldTitleCentered == null)
                {
                    boldTitleCentered = new GUIStyle(BoldTitle)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }

                return boldTitleCentered;
            }
        }

        //
        // 摘要:
        //     Right aligned bold title style.
        public static GUIStyle BoldTitleRight
        {
            get
            {
                if (boldTitleRight == null)
                {
                    boldTitleRight = new GUIStyle(BoldTitle)
                    {
                        alignment = TextAnchor.MiddleRight
                    };
                }

                return boldTitleRight;
            }
        }

        //
        // 摘要:
        //     Centered title style.
        public static GUIStyle TitleCentered
        {
            get
            {
                if (titleCentered == null)
                {
                    titleCentered = new GUIStyle(Title)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }

                return titleCentered;
            }
        }

        //
        // 摘要:
        //     Right aligned title style.
        public static GUIStyle TitleRight
        {
            get
            {
                if (titleRight == null)
                {
                    titleRight = new GUIStyle(Title)
                    {
                        alignment = TextAnchor.MiddleRight
                    };
                }

                return titleRight;
            }
        }

        //
        // 摘要:
        //     Subtitle style.
        public static GUIStyle Subtitle
        {
            get
            {
                if (subtitle == null)
                {
                    subtitle = new GUIStyle(Title)
                    {
                        font = GUI.skin.button.font,
                        fontSize = 10,
                        contentOffset = new Vector2(0f, -3f),
                        fixedHeight = 16f
                    };
                    Color textColor = subtitle.normal.textColor;
                    textColor.a *= 0.7f;
                    subtitle.normal.textColor = textColor;
                }

                return subtitle;
            }
        }

        //
        // 摘要:
        //     Centered sub-title style.
        public static GUIStyle SubtitleCentered
        {
            get
            {
                if (subtitleCentered == null)
                {
                    subtitleCentered = new GUIStyle(Subtitle)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }

                return subtitleCentered;
            }
        }

        //
        // 摘要:
        //     Right aligned sub-title style.
        public static GUIStyle SubtitleRight
        {
            get
            {
                if (subtitleRight == null)
                {
                    subtitleRight = new GUIStyle(Subtitle)
                    {
                        alignment = TextAnchor.MiddleRight
                    };
                }

                return subtitleRight;
            }
        }

        //
        // 摘要:
        //     Message box style.
        public static GUIStyle MessageBox
        {
            get
            {
                if (messageBox == null)
                {
                    messageBox = new GUIStyle("HelpBox")
                    {
                        margin = new RectOffset(4, 4, 2, 2),
                        fontSize = 10,
                        richText = true
                    };
                }

                return messageBox;
            }
        }

        //
        // 摘要:
        //     Detailed Message box style.
        public static GUIStyle DetailedMessageBox
        {
            get
            {
                if (detailedMessageBox == null)
                {
                    detailedMessageBox = new GUIStyle(MessageBox);
                    detailedMessageBox.padding.right += 18;
                }

                return detailedMessageBox;
            }
        }

        //
        // 摘要:
        //     Multiline Label
        public static GUIStyle MultiLineLabel
        {
            get
            {
                if (multiLineLabel == null)
                {
                    multiLineLabel = new GUIStyle(EditorStyles.label)
                    {
                        richText = true,
                        stretchWidth = true,
                        wordWrap = true
                    };
                }

                return multiLineLabel;
            }
        }

        //
        // 摘要:
        //     Centered Multiline Label
        public static GUIStyle MultiLineCenteredLabel
        {
            get
            {
                if (multiLineCenteredLabel == null)
                {
                    multiLineCenteredLabel = new GUIStyle(MultiLineLabel)
                    {
                        stretchWidth = true,
                        alignment = TextAnchor.MiddleCenter
                    };
                }

                return multiLineCenteredLabel;
            }
        }

        //
        // 摘要:
        //     Centered Text Field
        public static GUIStyle CenteredTextField
        {
            get
            {
                if (centeredTextField == null)
                {
                    centeredTextField = new GUIStyle(EditorStyles.textField)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }

                return centeredTextField;
            }
        }

        //
        // 摘要:
        //     Gets the bottom box padding.
        public static GUIStyle BottomBoxPadding
        {
            get
            {
                if (bottomBoxPadding == null)
                {
                    bottomBoxPadding = new GUIStyle
                    {
                        padding = new RectOffset(0, 0, 3, 5),
                        margin = new RectOffset(0, 0, 7, 0)
                    };
                }

                return bottomBoxPadding;
            }
        }

        //
        // 摘要:
        //     Unitys PaneOptions GUIStyle.
        public static GUIStyle PaneOptions
        {
            get
            {
                if (paneOptions == null)
                {
                    paneOptions = new GUIStyle("PaneOptions");
                }

                return paneOptions;
            }
        }

        //
        // 摘要:
        //     Unitys ProjectBrowserTextureIconDropShadow GUIStyle.
        public static GUIStyle ContainerOuterShadow
        {
            get
            {
                if (containerOuterShadow == null)
                {
                    containerOuterShadow = new GUIStyle("ProjectBrowserTextureIconDropShadow");
                }

                return containerOuterShadow;
            }
        }

        //
        // 摘要:
        //     Unitys TL SelectionButton PreDropGlow GUIStyle.
        public static GUIStyle ContainerOuterShadowGlow
        {
            get
            {
                if (containerOuterShadowGlow == null)
                {
                    containerOuterShadowGlow = new GUIStyle("TL SelectionButton PreDropGlow");
                }

                return containerOuterShadowGlow;
            }
        }

        //
        // 摘要:
        //     Unitys ShurikenModuleTitle GUIStyle.
        public static GUIStyle ModuleHeader
        {
            get
            {
                if (moduleHeader == null)
                {
                    moduleHeader = new GUIStyle("ShurikenModuleTitle");
                }

                return moduleHeader;
            }
        }

        //
        // 摘要:
        //     Draw this one manually with: new Color(1, 1, 1, EditorGUIUtility.isProSkin ?
        //     0.25f : 0.45f)
        public static GUIStyle CardStyle
        {
            get
            {
                if (cardStyle == null)
                {
                    cardStyle = new GUIStyle("sv_iconselector_labelselection")
                    {
                        padding = new RectOffset(15, 15, 15, 15),
                        margin = new RectOffset(0, 0, 0, 0),
                        stretchHeight = false
                    };
                }

                return cardStyle;
            }
        }

        static KGUIStyles()
        {
            ValidatorGreen = new Color(0.224f, 0.71f, 0.29f, 1f);
            InspectorOrange = new Color(0.949f, 0.384f, 0.247f, 1f);
            SerializerYellow = new Color(1f, 0.682f, 0f, 1f);
            GreenValidColor = new Color(0.2f, 193f / 255f, 7f / 255f);
            RedErrorColor = (EditorGUIUtility.isProSkin ? new Color(1f, 83f / 255f, 74f / 255f) : new Color(59f / 85f, 4f / 85f, 4f / 85f, 1f));
            YellowWarningColor = (EditorGUIUtility.isProSkin ? new Color(1f, 193f / 255f, 7f / 255f) : new Color(67f / 85f, 151f / 255f, 0f, 1f));
            BorderColor = (EditorGUIUtility.isProSkin ? new Color(0.11f, 0.11f, 0.11f, 0.8f) : new Color(0.38f, 0.38f, 0.38f, 0.6f));
            BoxBackgroundColor = (EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.05f) : new Color(1f, 1f, 1f, 0.2f));
            DarkEditorBackground = (EditorGUIUtility.isProSkin ? new Color(0.192f, 0.192f, 0.192f, 1f) : new Color(0f, 0f, 0f, 0f));
            EditorWindowBackgroundColor = (EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f, 1f) : new Color(0.76f, 0.76f, 0.76f, 1f));
            MenuBackgroundColor = (EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.035f) : new Color(0.87f, 0.87f, 0.87f, 1f));
            HeaderBoxBackgroundColor = (EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.06f) : new Color(1f, 1f, 1f, 0.26f));
            HighlightedButtonColor = (EditorGUIUtility.isProSkin ? new Color(0f, 1f, 0f, 1f) : new Color(0f, 1f, 0f, 1f));
            HighlightedTextColor = (EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 1f) : new Color(0f, 0f, 0f, 1f));
            HighlightPropertyColor = (EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.6f) : new Color(0f, 0f, 0f, 0.6f));
            ListItemColorHoverEven = (EditorGUIUtility.isProSkin ? new Color(0.223200008f, 0.223200008f, 0.223200008f, 1f) : new Color(0.89f, 0.89f, 0.89f, 1f));
            ListItemColorHoverOdd = (EditorGUIUtility.isProSkin ? new Color(0.2472f, 0.2472f, 0.2472f, 1f) : new Color(0.904f, 0.904f, 0.904f, 1f));
            ListItemDragBg = new Color(0.1f, 0.1f, 0.1f, 1f);
            ListItemDragBgColor = (EditorGUIUtility.isProSkin ? new Color(0.1f, 0.1f, 0.1f, 1f) : new Color(0.338f, 0.338f, 0.338f, 1f));
            ColumnTitleBg = (EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.019f) : new Color(1f, 1f, 1f, 0.019f));
            DefaultSelectedMenuTreeColorDarkSkin = new Color(0.243f, 0.373f, 0.588f, 1f);
            DefaultSelectedInactiveMenuTreeColorDarkSkin = new Color(0.838f, 0.838f, 0.838f, 0.134f);
            DefaultSelectedMenuTreeColorLightSkin = new Color(0.243f, 0.49f, 0.9f, 1f);
            DefaultSelectedInactiveMenuTreeColorLightSkin = new Color(0.5f, 0.5f, 0.5f, 1f);
            MouseOverBgOverlayColor = (EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.028f) : new Color(1f, 1f, 1f, 0.3f));
            MenuButtonActiveBgColor = (EditorGUIUtility.isProSkin ? new Color(0.243f, 0.373f, 0.588f, 1f) : new Color(0.243f, 0.49f, 0.9f, 1f));
            MenuButtonBorderColor = new Color(EditorWindowBackgroundColor.r * 0.8f, EditorWindowBackgroundColor.g * 0.8f, EditorWindowBackgroundColor.b * 0.8f);
            MenuButtonColor = new Color(0f, 0f, 0f, 0f);
            MenuButtonHoverColor = new Color(1f, 1f, 1f, 0.08f);
            LightBorderColor = new Color32(90, 90, 90, byte.MaxValue);
            Temporary = new GUIStyle();
            SDFIconButtonLabelStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(0, 0, 0, 0)
            };
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly[] array = assemblies;
            foreach (Assembly assembly in array)
            {
                try
                {
                    if (!(assembly.GetName().Name == "Sirenix.OdinInspector.Editor"))
                    {
                        continue;
                    }

                    GeneralDrawerConfig_Type = assembly.GetType("Sirenix.OdinInspector.Editor.GeneralDrawerConfig");
                    if (GeneralDrawerConfig_Type != null)
                    {
                        GeneralDrawerConfig_Instance_Prop = GeneralDrawerConfig_Type.GetProperty("Instance", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
                        if (GeneralDrawerConfig_Instance_Prop != null)
                        {
                            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public;
                            GeneralDrawerConfig_ListItemColorEvenDarkSkin_Prop = GeneralDrawerConfig_Type.GetProperty("ListItemColorEvenDarkSkin", bindingAttr);
                            GeneralDrawerConfig_ListItemColorEvenLightSkin_Prop = GeneralDrawerConfig_Type.GetProperty("ListItemColorEvenLightSkin", bindingAttr);
                            GeneralDrawerConfig_ListItemColorOddDarkSkin_Prop = GeneralDrawerConfig_Type.GetProperty("ListItemColorOddDarkSkin", bindingAttr);
                            GeneralDrawerConfig_ListItemColorOddLightSkin_Prop = GeneralDrawerConfig_Type.GetProperty("ListItemColorOddLightSkin", bindingAttr);
                        }
                    }

                    return;
                }
                catch
                {
                }
            }
        }

        private static Color GetGeneralConfigDefaultColor(PropertyInfo colorProp, Color defaultColor)
        {
            if (colorProp == null)
            {
                return defaultColor;
            }

            object value = GeneralDrawerConfig_Instance_Prop.GetValue(null, null);
            return (Color)colorProp.GetValue(value, null);
        }
    }
}

