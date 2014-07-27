using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Labs.Services;
using Xamarin.Forms.Labs.Services.Serialization;

namespace Xamarin.Forms.Labs.Controls
{
    /// <summary>
    /// The hybrid web view.
    /// </summary>
    public class HybridWebView : WebView
    {
        /// <summary>
        /// The inject lock.
        /// </summary>
        private readonly object injectLock = new object();

        /// <summary>
        /// The JSON serializer.
        /// </summary>
        private readonly IStringSerializer jsonSerializer;

        /// <summary>
        /// The registered actions.
        /// </summary>
        private readonly Dictionary<string, Action<string>> registeredActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="HybridWebView"/> class.
        /// </summary>
        /// <remarks>HybridWebView will use either <see cref="IJsonSerializer"/> configured
        /// with IoC or if missing it will use <see cref="SystemJsonSerializer"/> by default.</remarks>
        public HybridWebView()
        {
            if (!Resolver.IsSet || (this.jsonSerializer = Resolver.Resolve<IJsonSerializer>()) == null)
            {
                this.jsonSerializer = new SystemJsonSerializer();
            }

            this.registeredActions = new Dictionary<string, Action<string>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HybridWebView"/> class.
        /// </summary>
        /// <param name="jsonSerializer">
        /// The JSON serializer.
        /// </param>
        public HybridWebView(IJsonSerializer jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
            this.registeredActions = new Dictionary<string, Action<string>>();
        }

        /// <summary>
        /// The uri property.
        /// </summary>
        public static readonly BindableProperty UriProperty = BindableProperty.Create<HybridWebView, Uri>(p => p.Uri, default(Uri));

        /// <summary>
        /// Gets or sets the uri.
        /// </summary>
        public Uri Uri
        {
            get { return (Uri)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        /// <summary>
        /// Registers a native callback.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public void RegisterCallback(string name, Action<string> action)
        {
            this.registeredActions.Add(name, action);
        }

        public bool RemoveCallback(string name)
        {
            return this.registeredActions.Remove(name);
        }

        public void LoadFromContent(string contentFullName)
        {
            var handler = this.LoadFromContentRequested;
            if (handler != null)
            {
                handler(this, contentFullName);
            }
        }

        public void LoadFromHtmlString(string htmlString)
        {
            var handler = this.LoadFromHtmlStringRequested;
            if (handler != null)
            {
                handler(this, htmlString);
            }
        }

        public void InjectJavaScript(string script)
        {
            lock (this.injectLock)
            {
                var handler = this.JavaScriptLoadRequested;
                if (handler != null)
                {
                    handler(this, script);
                }
            }
        }

        public bool TryGetAction(string name, out Action<string> action)
        {
            return this.registeredActions.TryGetValue(name, out action);
        }

        public void OnLoadFinished(object sender, EventArgs e)
        {
            var handler = this.LoadFinished;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void CallJsFunction(string funcName, params object[] parameters)
        {
            var builder = new StringBuilder();

            builder.Append(funcName);
            builder.Append("(");

            for (var n = 0; n < parameters.Length; n++)
            {
                builder.Append(this.jsonSerializer.Serialize(parameters[n]));
                if (n < parameters.Length - 1)
                {
                    builder.Append(", ");
                }
            }

            builder.Append(");");

            this.InjectJavaScript(builder.ToString());
        }

        public EventHandler<string> JavaScriptLoadRequested;
        public EventHandler<string> LoadFromContentRequested;
        public EventHandler<string> LoadFromHtmlStringRequested;
        public EventHandler LoadFinished;
    }
}
