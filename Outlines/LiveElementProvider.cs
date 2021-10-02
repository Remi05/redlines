﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;

namespace Outlines
{
    public class LiveElementProvider : IElementProvider
    {
        protected IElementPropertiesProvider PropertiesProvider { get; set; }
        protected Condition FilterCondition { get; set; }

        public LiveElementProvider(IElementPropertiesProvider propertiesProvider)
        {
            PropertiesProvider = propertiesProvider;
            FilterCondition = new AndCondition(new NotCondition(new AndCondition(new PropertyCondition(AutomationElement.NameProperty, "Outlines", PropertyConditionFlags.IgnoreCase), 
                                                                                 new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window))),
                                               new PropertyCondition(AutomationElement.IsOffscreenProperty, false));
        }

        public virtual ElementProperties TryGetElementFromPoint(Point point)
        {
            var containingElement = GetContainingElement(AutomationElement.RootElement, point);
            return PropertiesProvider.GetElementProperties(containingElement);
        }

        protected AutomationElement GetContainingElement(AutomationElement rootElement, Point point)
        {
            if (rootElement == null)
            {
                return null;
            }

            try
            {
                Rect elementBounds = rootElement.Current.BoundingRectangle;

                if (!elementBounds.Contains(point))
                {
                    return null;
                }

                var children = rootElement.FindAll(TreeScope.Children, FilterCondition);
                foreach (AutomationElement child in children)
                {
                    try
                    {
                        var containingElement = GetContainingElement(child, point);
                        if (containingElement != null)
                        {
                            return containingElement;
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }

            return rootElement;
        }
    }
}
