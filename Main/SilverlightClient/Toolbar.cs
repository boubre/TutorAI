﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Printing;
using DynamicGeometry;

namespace LiveGeometry
{
    public partial class Page : UserControl
    {
        Command CommandNew;
        Command CommandOpen;
        Command CommandSave;
        Command CommandPrint;
        Command CommandSamples;
        Command CommandSettings;
        Command CommandHomepage;
        Command CommandParse;
        Command CommandParseOptions;
        Command CommandProblemCharacteristics;
        Command CommandManageGivens;
        Command CommandEnterSolution;
        Command CommandMakeBookProblem;
        Command CommandSynthProblem;
        Command CommandStartRegionShading;
        Command CommandClearRegionShading;

        public void InitializeToolbar()
        {
            ShowToolbarButtonText = IsolatedStorage.GetSetting("ShowToolbarText", true);

            CommandNew = new Command(New, GetImageFromResource("New.png"), "New", "Drawing");
            CommandOpen = new Command(Open, GetImageFromResource("Open.png"), "Open", "Drawing");
            CommandSave = new Command(Save, GetImageFromResource("Save.png"), "Save", "Drawing");
            CommandPrint = new Command(Print, GetImageFromResource("Print.png"), "Print", "Drawing");
            CommandSamples = new Command(Samples, GetImageFromResource("Samples.png"), "Samples", "Drawing");
            CommandSettings = new Command(OpenSettings, GetImageFromResource("Repair.png"), "Settings", "Drawing");
            CommandHomepage = new Command(Homepage, GetImageFromResource("Homepage.png"), "Homepage", "Drawing");
            CommandParse = new Command(ParseToAst, GetImageFromResource("Parse.png"), "Parse", "Parsing");
            CommandParseOptions = new Command(DisplayParseOptions, GetImageFromResource("ParseOptions.png"), "Def and Theorem Options", "Parsing");
            CommandProblemCharacteristics = new Command(DisplayProblemCharacteristics, GetImageFromResource("ParseOptions.png"), "Desired Problem Characteristics", "Parsing");
            CommandEnterSolution = new Command(DisplayEnterSolution, GetImageFromResource("Parse.png"), "Enter Solution", "Parsing");
            CommandManageGivens = new Command(DisplayManageGivens, GetImageFromResource("ParseOptions.png"), "Manage Givens", "Parsing");
            CommandStartRegionShading = new Command(StartRegionShading, GetImageFromResource("Play.png"), "Start Shading", "Regions");
            CommandMakeBookProblem = new Command(DrawBookProblem, GetImageFromResource("Draw.png"), "Book Problem", "Parsing");
            CommandSynthProblem = new Command(SynthesizeProblem, GetImageFromResource("Draw.png"), "Synthesize Problem", "Parsing");
            CommandClearRegionShading = new Command(ClearRegionShading, GetImageFromResource("Trash.png"), "Clear Shading", "Regions");
            CommandClearRegionShading.Icon.Opacity = 0.2;

            drawingHost.DrawingControl.CommandUndo.Icon = GetImageFromResource("Undo.png");
            drawingHost.DrawingControl.CommandRedo.Icon = GetImageFromResource("Redo.png");

            drawingHost.AddToolbarButton(drawingHost.CommandToggleGrid);
            drawingHost.AddToolbarButton(CommandNew);
            drawingHost.AddToolbarButton(CommandOpen);
            drawingHost.AddToolbarButton(CommandSave);
            drawingHost.AddToolbarButton(CommandPrint);
            drawingHost.AddToolbarButton(drawingHost.DrawingControl.CommandUndo);
            drawingHost.AddToolbarButton(drawingHost.DrawingControl.CommandRedo);
            drawingHost.AddToolbarButton(CommandSamples);
            drawingHost.AddToolbarButton(CommandSettings);
            drawingHost.AddToolbarButton(drawingHost.CommandShowFigureExplorer);
            drawingHost.AddToolbarButton(CommandHomepage);
            drawingHost.AddToolbarButton(drawingHost.CommandToggleOrtho);
            drawingHost.AddToolbarButton(drawingHost.CommandToggleSnapToGrid);
            drawingHost.AddToolbarButton(drawingHost.CommandToggleSnapToPoint);
            drawingHost.AddToolbarButton(drawingHost.CommandToggleSnapToCenter);
            drawingHost.AddToolbarButton(drawingHost.CommandToggleLabelNewPoints);
            drawingHost.AddToolbarButton(drawingHost.CommandTogglePolar);

            drawingHost.AddToolbarButton(CommandParse);
            drawingHost.AddToolbarButton(drawingHost.CommandShowAIDebugWindow);
            drawingHost.AddToolbarButton(CommandParseOptions);
            //drawingHost.AddToolbarButton(CommandProblemCharacteristics);
            drawingHost.AddToolbarButton(CommandManageGivens);
            drawingHost.AddToolbarButton(CommandEnterSolution);
            drawingHost.AddToolbarButton(CommandMakeBookProblem);
            drawingHost.AddToolbarButton(CommandSynthProblem);
            drawingHost.AddToolbarButton(CommandStartRegionShading);
            drawingHost.AddToolbarButton(CommandClearRegionShading);

            drawingHost.Ribbon.GetPanel("Drawing").HeaderContent.Icon = GetImageFromResource("SaveFormDesign.png");
        }

        public static Image GetImageFromResource(string resourceName)
        {
            resourceName = string.Format("LiveGeometry;component/{0}", resourceName);
            var uri = new Uri(resourceName, UriKind.Relative);
            var streamInfo = Application.GetResourceStream(uri);
            BitmapImage source = new BitmapImage();
            source.SetSource(streamInfo.Stream);
            return new Image() { Source = source, Stretch = Stretch.None };
        }

        private bool mShowToolbarButtonText = true;
        public bool ShowToolbarButtonText
        {
            get
            {
                return mShowToolbarButtonText;
            }
            set
            {
                mShowToolbarButtonText = value;
                IsolatedStorage.SaveSetting("ShowToolbarText", value);
            }
        }

        private bool showToolbar = true;
        public bool ShowToolbar
        {
            get
            {
                return showToolbar;
            }
            set
            {
                showToolbar = value;
                drawingHost.Ribbon.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                IsolatedStorage.SaveSetting("ShowToolbar", value);
            }
        }

        private void ShowToolbarText_Checked(object sender, RoutedEventArgs e)
        {
            ShowToolbarButtonText = !ShowToolbarButtonText;
        }

        private void Print()
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += PrintPageHandler;
            printDocument.Print("Live Geometry Document");
        }

        private void PrintPageHandler(object sender, PrintPageEventArgs e)
        {
            var printCanvas = new DrawingControl();
            printCanvas.Height = e.PrintableArea.Height;
            printCanvas.Width = e.PrintableArea.Width;
            printCanvas.LoadDrawing(DrawingSerializer.SaveDrawing(drawingHost.CurrentDrawing));

            var savedScaleSetting = DynamicGeometry.Settings.ScaleTextWithDrawing;
            DynamicGeometry.Settings.ScaleTextWithDrawing = true;   // Must be true for text to appear as expected.
            printCanvas.Drawing.Recalculate();
            DynamicGeometry.Settings.ScaleTextWithDrawing = savedScaleSetting;

            e.PageVisual = printCanvas;
        }
    }
}