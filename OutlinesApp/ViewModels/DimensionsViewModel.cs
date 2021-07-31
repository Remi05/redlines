﻿using System;
using System.Windows;
using Outlines;
using OutlinesApp.Services;

namespace OutlinesApp.ViewModels
{
    public enum DimensionsTextPlacement { Below, Above, Inside };

    public class DimensionsViewModel
    {
        private const int ContainerRectWidth = 100;
        private const int ContainerRectHeight = 35;

        private ElementProperties ElementProperties { get; set; }
        private ICoordinateConverter CoordinateConverter { get; set; }
        private IScreenHelper ScreenHelper { get; set; }

        public string DimensionsText => $"{ElementProperties.BoundingRect.Width} x {ElementProperties.BoundingRect.Height}";

        public DimensionsTextPlacement TextPlacement { get; private set; }

        public Rect ContainerRect { get; private set; }

        public DimensionsViewModel(ElementProperties elementProperties, ICoordinateConverter coordinateConverter, IScreenHelper screenHelper)
        {
            if (elementProperties == null || coordinateConverter == null)
            {
                throw new ArgumentNullException(elementProperties == null ? nameof(elementProperties) : nameof(coordinateConverter));
            }
            ElementProperties = elementProperties;
            CoordinateConverter = coordinateConverter;
            ScreenHelper = screenHelper;
            InitializeContainerRect();
        }

        private void InitializeContainerRect()
        {
            Rect localElementRect = CoordinateConverter.RectFromScreen(ElementProperties.BoundingRect);
            double localElementCenterX = localElementRect.Left + localElementRect.Width / 2;
            Point containerRectTopLeft = new Point(localElementCenterX - ContainerRectWidth / 2, localElementRect.Bottom);
            Point containerRectBottomCenter = new Point(localElementCenterX, containerRectTopLeft.Y + ContainerRectHeight);
            TextPlacement = DimensionsTextPlacement.Below;

            Rect monitorRect = ScreenHelper.GetDisplayRect(localElementRect.TopLeft);
            Rect localMonitorRect = CoordinateConverter.RectFromScreen(monitorRect);
            if (!localMonitorRect.Contains(containerRectBottomCenter))
            {
                // If the text is outside the screen when shown below, try above the outline.
                containerRectTopLeft.Y = localElementRect.Top - ContainerRectHeight;
                TextPlacement = DimensionsTextPlacement.Above;

                if (!localMonitorRect.Contains(containerRectTopLeft))
                {
                    // Fallback to a centered rectangle at the bottom of the outline, but inside of it, if it can't be shown above or below.
                    containerRectTopLeft.Y = localElementRect.Bottom - ContainerRectHeight;
                    TextPlacement = DimensionsTextPlacement.Inside;
                }
            }

            ContainerRect = new Rect(containerRectTopLeft, new Size(ContainerRectWidth, ContainerRectHeight));
        }
    }
}
