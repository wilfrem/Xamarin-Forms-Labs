﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xamarin.Forms;
using Xamarin.Forms.Labs.Controls;
//using System.Windows.Controls;
using Xamarin.Forms.Labs.WP8.Controls;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(CheckBox), typeof(CheckBoxRenderer))]

namespace Xamarin.Forms.Labs.WP8.Controls
{
    using NativeCheckBox = System.Windows.Controls.CheckBox;
    public class CheckBoxRenderer : ViewRenderer<CheckBox, System.Windows.Controls.CheckBox>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.CheckedChanged -= CheckedChanged;
            }

            if (this.Control == null)
            {
                var checkBox = new NativeCheckBox();
                checkBox.Checked += (s, args) => this.Element.Checked = true;
                checkBox.Unchecked += (s, args) => this.Element.Checked = false;

                this.SetNativeControl(checkBox);
            }

            this.Control.Content = e.NewElement.Text;
            this.Control.IsChecked = e.NewElement.Checked;

            this.Element.CheckedChanged += CheckedChanged;
            this.Element.PropertyChanged += ElementOnPropertyChanged;
        }

        private void ElementOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "Checked":
                    this.Control.IsChecked = this.Element.Checked;
                    break;
                case "TextColor":

                    break;
                case "CheckedText":
                case "UncheckedText":
                    this.Control.Content = Element.Text;
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Property change for {0} has not been implemented.", propertyChangedEventArgs.PropertyName);
                    break;
            }
        }

        private void CheckedChanged(object sender, EventArgs<bool> eventArgs)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.Control.Content = this.Element.Text;
                this.Control.IsChecked = eventArgs.Value;
            });
        }
    }
}
