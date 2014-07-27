using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xamarin.Forms.Labs.Controls
{
    public partial class HybridWebViewRenderer
    {

        private const string Format = "^(file|http|https)://(local|LOCAL)/Action(=|%3D)(?<Action>[\\w]+)/";
        private static readonly Regex Expression = new Regex(Format);

        private void InjectNativeFunctionScript()
        {
            var builder = new StringBuilder();
            builder.Append("function Native(action, data){ ");
#if WINDOWS_PHONE
            builder.Append("window.external.notify(");
#else
            builder.Append("window.location = \"//LOCAL/Action=\" + ");
#endif
            builder.Append("action + \"/\"");
            builder.Append(" + ((typeof data == \"object\") ? JSON.stringify(data) : data)");
#if WINDOWS_PHONE
            builder.Append(")");
#endif
            builder.Append(" ;}");

            this.Inject(builder.ToString());
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Uri")
            {
                this.Load(this.Element.Uri);
            }
        }

        private void Bind()
        {
            this.Element.PropertyChanged += this.Model_PropertyChanged;
            if (this.Element.Uri != null)
            {
                this.Load (this.Element.Uri);
            }

            this.Element.JavaScriptLoadRequested += OnInjectRequest;
            this.Element.LoadFromContentRequested += LoadFromContent;
            this.Element.LoadFromHtmlStringRequested += LoadFromHtmlString;
        }

        private void Unbind(HybridWebView oldElement)
        {
            if (oldElement != null)
            {
                oldElement.PropertyChanged -= this.Model_PropertyChanged;
                oldElement.JavaScriptLoadRequested -= OnInjectRequest;
                oldElement.LoadFromContentRequested -= LoadFromContent;
                this.Element.LoadFromHtmlStringRequested -= LoadFromHtmlString;
            }
        }

        private void OnInjectRequest(object sender, string script)
        {
            this.Inject(script);
        }

        partial void Inject(string script);

        partial void Load(Uri uri);

        partial void LoadFromContent(object sender, string contentFullName);

        partial void LoadFromHtmlString(object sender, string htmlString);

        private bool CheckRequest(string request)
        {
            var m = Expression.Match(request);

            if (m.Success)
            {
                Action<string> action;
                var name = m.Groups["Action"].Value;

                if (this.Element.TryGetAction (name, out action))
                {
                    var data = Uri.UnescapeDataString (request.Remove (m.Index, m.Length));
                    action.Invoke (data);
                } 
                else
                {
                    System.Diagnostics.Debug.WriteLine ("Unhandled callback {0} was called from JavaScript", name);
                }
            }

            return m.Success;
        }
    }
}
