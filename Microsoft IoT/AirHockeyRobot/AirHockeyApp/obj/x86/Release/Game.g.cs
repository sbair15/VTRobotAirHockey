﻿#pragma checksum "C:\Users\VTAirHockeyRobot\Source\Repos\VTRobotAirHockey2\Microsoft IoT\AirHockeyRobot\AirHockeyApp\Game.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D380FF2A8348DFCB2D0006F72113A35C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AirHockeyApp
{
    partial class DiagnosticsPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.mainCanvas = (global::Windows.UI.Xaml.Controls.Canvas)(target);
                }
                break;
            case 2:
                {
                    this.gameCanvas = (global::Windows.UI.Xaml.Controls.Canvas)(target);
                }
                break;
            case 3:
                {
                    this.popUpCanvas = (global::Windows.UI.Xaml.Controls.Canvas)(target);
                }
                break;
            case 4:
                {
                    this.homeButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 44 "..\..\..\Game.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.homeButton).Click += this.homeButton_Click;
                    #line default
                }
                break;
            case 5:
                {
                    this.statImage = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 6:
                {
                    this.popUpTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 7:
                {
                    this.gameOutputTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 8:
                {
                    this.scoreCanvas = (global::Windows.UI.Xaml.Controls.Canvas)(target);
                }
                break;
            case 9:
                {
                    this.textBlock1 = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 10:
                {
                    this.textBlock1_Copy = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 11:
                {
                    this.humanScoreTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 12:
                {
                    this.robotScoreTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 13:
                {
                    this.centerOfMass = (global::Windows.UI.Xaml.Shapes.Ellipse)(target);
                }
                break;
            case 14:
                {
                    this.puck = (global::Windows.UI.Xaml.Shapes.Ellipse)(target);
                }
                break;
            case 15:
                {
                    this.slider = (global::Windows.UI.Xaml.Controls.Slider)(target);
                    #line 16 "..\..\..\Game.xaml"
                    ((global::Windows.UI.Xaml.Controls.Slider)this.slider).ValueChanged += this.slider_ValueChanged;
                    #line default
                }
                break;
            case 16:
                {
                    this.brightnessTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 17:
                {
                    this.mallet = (global::Windows.UI.Xaml.Shapes.Ellipse)(target);
                }
                break;
            case 18:
                {
                    this.moveMalletButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 19 "..\..\..\Game.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.moveMalletButton).Click += this.moveMalletButton_Click;
                    #line default
                }
                break;
            case 19:
                {
                    this.mirrorButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 20 "..\..\..\Game.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.mirrorButton).Click += this.mirrorButton_Click;
                    #line default
                }
                break;
            case 20:
                {
                    this.textCanvas = (global::Windows.UI.Xaml.Controls.Canvas)(target);
                }
                break;
            case 21:
                {
                    this.hideTextButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 30 "..\..\..\Game.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.hideTextButton).Click += this.hideTextButton_Click;
                    #line default
                }
                break;
            case 22:
                {
                    this.outputTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 23:
                {
                    this.translatedTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 24:
                {
                    this.statusTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 25:
                {
                    this.malletLocationTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 26:
                {
                    this.centerOfMassTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 27:
                {
                    this.textBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 28:
                {
                    this.scoreTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

