using Microsoft.AspNetCore.Mvc.Infrastructure;
using Rotativa.AspNetCore.Options;
using System;
using System.Threading.Tasks;

namespace Core.Utilities
{
    public class HTMLToPDF
    {
        #region Fields

        private string _viewPath;
        private string _fileName;
        private string _customSwitches;
        private Size _sizeCore;
        private object _model;
        private IActionContextAccessor _actionContextAccessor;

        #endregion Fields

        #region Constructor

        public HTMLToPDF(string view, string fileName, IActionContextAccessor actionContextAccessor, object model = null, string size = "A4", string switches = null, int delay = 800)
        {
            _viewPath = view;

            _model = model ?? null;

            _fileName = fileName;

            _actionContextAccessor = actionContextAccessor;

            switch (size)
            {
                case "A3":
                    _sizeCore = Size.A3;
                    break;

                case "A2":
                    _sizeCore = Size.A2;
                    break;

                case "A1":
                    _sizeCore = Size.A1;
                    break;

                default:
                    _sizeCore = Size.A4;
                    break;
            }

            if (switches != null)
            {
                _customSwitches = string.Format("{0} --disable-internal-links --enable-internal-links --disable-external-links  " +
                    "--enable-javascript --debug-javascript --no-stop-slow-scripts --javascript-delay {1}", switches, delay);
            }
            else
            {
                _customSwitches = string.Format("--disable-internal-links --enable-internal-links --disable-external-links  " +
                    "--enable-javascript --debug-javascript --no-stop-slow-scripts --javascript-delay {0}", delay);
            }
        }

        #endregion Constructor

        #region Methods

        public Rotativa.AspNetCore.ViewAsPdf GeneratePdf()
        {
            try
            {
                if (_model != null)
                {
                    return new Rotativa.AspNetCore.ViewAsPdf(_viewPath, _model)
                    {
                        PageSize = _sizeCore,
                        CustomSwitches = _customSwitches,
                        FileName = _fileName
                    };
                }

                return new Rotativa.AspNetCore.ViewAsPdf(_viewPath)
                {
                    PageSize = _sizeCore,
                    CustomSwitches = _customSwitches,
                    FileName = _fileName
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<byte[]> GetBytePdf()
        {
            try
            {
                Rotativa.AspNetCore.ViewAsPdf pdfDoc = null;

                if (_model != null)
                {
                    pdfDoc = new Rotativa.AspNetCore.ViewAsPdf(_viewPath, _model)
                    {
                        PageSize = _sizeCore,
                        CustomSwitches = _customSwitches,
                        FileName = _fileName
                    };

                    return await pdfDoc.BuildFile(_actionContextAccessor.ActionContext);
                }

                pdfDoc = new Rotativa.AspNetCore.ViewAsPdf(_viewPath)
                {
                    PageSize = _sizeCore,
                    CustomSwitches = _customSwitches,
                    FileName = _fileName
                };

                return await pdfDoc.BuildFile(_actionContextAccessor.ActionContext);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #endregion Methods
    }
}
