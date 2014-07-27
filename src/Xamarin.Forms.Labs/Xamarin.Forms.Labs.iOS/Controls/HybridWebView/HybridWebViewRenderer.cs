using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.Labs.Controls;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]

namespace Xamarin.Forms.Labs.Controls
{
    /// <summary>
    /// The hybrid web view renderer.
    /// </summary>
    public partial class HybridWebViewRenderer : ViewRenderer<HybridWebView, UIWebView>
    {
        private UIWebView webView;

        /// <summary>
        /// The on element changed callback.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (this.webView == null)
            {
                this.webView = new UIWebView();

                this.webView.LoadFinished += LoadFinished;
                this.webView.ShouldStartLoad += this.HandleStartLoad;
                this.InjectNativeFunctionScript();
                this.SetNativeControl(this.webView);
            }

            this.Unbind(e.OldElement);
            this.Bind();
        }

        void LoadFinished(object sender, EventArgs e)
        {
            this.Element.OnLoadFinished(sender, e);
        }

        private bool HandleStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            return !this.CheckRequest(request.Url.RelativeString);
        }

        partial void Inject(string script)
        {
            this.webView.EvaluateJavascript(script);
        }

        partial void Load(Uri uri)
        {
            if (uri != null)
            {
                this.webView.LoadRequest(new NSUrlRequest(new NSUrl(uri.AbsoluteUri)));
            }
        }

        partial void LoadFromContent(object sender, string contentFullName)
        {
            this.Element.Uri = new Uri(NSBundle.MainBundle.BundlePath + "/" + contentFullName);
            //string homePageUrl = NSBundle.MainBundle.BundlePath + "/" + contentFullName;
            //this.webView.LoadRequest(new NSUrlRequest(new NSUrl(homePageUrl, false)));
        }

        partial void LoadFromHtmlString(object sender, string htmlString)
        {
            this.webView.LoadHtmlString(htmlString, NSBundle.MainBundle.BundleUrl);
        }
    }
}