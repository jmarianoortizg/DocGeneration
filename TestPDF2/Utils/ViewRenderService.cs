using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace TestPDF2.Utils
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
        SmtpClient createSMTPClient();
        Attachment cretateAttachmentImage(string file, string contentID);
    }

    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpContext _http;

        public ViewRenderService(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, IHttpContextAccessor ctx)
        {
            _razorViewEngine = razorViewEngine; _tempDataProvider = tempDataProvider; _serviceProvider = serviceProvider; _http = ctx.HttpContext;
        }

        public async Task<string> RenderToStringAsync(string viewName, object model)
        {
            try
            {
                var actionContext = new ActionContext(_http, new Microsoft.AspNetCore.Routing.RouteData(), new ActionDescriptor());

                using (var sw = new StringWriter())
                {
                    var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };

                    var viewResult = _razorViewEngine.GetView(viewName, viewName, false);
                    //var viewResult = _razorViewEngine.GetView(_env.WebRootPath, viewName, false); // For views outside the usual Views folder
                    if (viewResult.View == null)
                    {
                        throw new ArgumentNullException($"{viewName} does not match any available view");
                    }
                    var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = model
                    };
                    var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary, new TempDataDictionary(_http, _tempDataProvider), sw, new HtmlHelperOptions());
                    viewContext.RouteData = _http.GetRouteData();
                    await viewResult.View.RenderAsync(viewContext);
                    return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SmtpClient createSMTPClient()
        {
            try
            {
                var client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.UseDefaultCredentials = true;
                client.Credentials = new System.Net.NetworkCredential("no-reply@accrueme.com", "SlazyMotto!33");
                client.EnableSsl = true;
                return client;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Attachment cretateAttachmentImage(string file, string contentID)
        {
            var inlineLogo = new Attachment(file);
            inlineLogo.ContentId = contentID;
            inlineLogo.ContentDisposition.Inline = true;
            inlineLogo.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
            return inlineLogo;
        }

    }
}
