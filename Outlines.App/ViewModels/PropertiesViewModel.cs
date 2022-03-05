﻿using System;
using System.ComponentModel;
using Outlines.Core;

namespace Outlines.App.ViewModels
{
    public class PropertiesViewModel : INotifyPropertyChanged
    {
        protected IOutlinesService OutlinesService { get; set; }

        private ElementProperties elementProperties;
        public ElementProperties ElementProperties
        {
            get => elementProperties;
            private set
            {
                if (value != elementProperties)
                {
                    elementProperties = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ElementProperties)));
                }
            }
        }

        private TextProperties textProperties;
        public TextProperties TextProperties
        {
            get => textProperties;
            private set
            {
                if (value != textProperties)
                {
                    textProperties = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextProperties)));
                }
            }
        }

        private ProcessProperties processProperties;
        public ProcessProperties ProcessProperties
        {
            get => processProperties;
            private set
            {
                if (value != processProperties)
                {
                    processProperties = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessProperties)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PropertiesViewModel(IOutlinesService outlinesService)
        {
            if (outlinesService == null)
            {
                throw new ArgumentNullException(nameof(outlinesService));
            }
            OutlinesService = outlinesService;
            OutlinesService.SelectedElementChanged += OnSelectedElementChanged;

            ElementProperties = OutlinesService.SelectedElementProperties;
            TextProperties = OutlinesService.SelectedElementProperties?.TextProperties;
            ProcessProperties = OutlinesService.SelectedElementProperties?.ProcessProperties;
        }

        protected void OnSelectedElementChanged()
        {
            ElementProperties = OutlinesService.SelectedElementProperties;
            TextProperties = OutlinesService.SelectedElementProperties?.TextProperties;
            ProcessProperties = OutlinesService.SelectedElementProperties?.ProcessProperties;
        }
    }
}
