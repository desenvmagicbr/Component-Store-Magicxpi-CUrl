/***
 * (C) 2021, Magic Software BR
 * Classe de "configuração" (armazenamento das propriedades) do componente CUrl para Magic xpi 4.5 ou >
 * 
 * /--- BEGIN LICENSE WARRANTY ---/
 * Código livre (free) pode ser totalmente alterado
 * sem nenhum prévio aviso
 * Não possui direitos autorais
 * 
 * Visite: https://www.magicsoftware.com/pt-br/integration-platform/xpi/
 * /--- END LICENSE WARRANTY   ---/
 * 
 * Histórico de versões
 * 
 * 2021-03 : v1.0
 */

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.ComponentModel.Composition;

//
using MagicSoftware.Integration.Sdk;
using MagicSoftware.Integration.Sdk.Step;
using System.Diagnostics;

namespace com.magicbr.curl {

    /// <summary>
    /// Description of StepRT1.
    /// </summary>
    [Export(typeof(IStep))]
    public class runtime : IStep {

        /// <summary>
        /// 
        /// </summary>
        private readonly string CURL_RSRC_KEY = "CUrl";
        private readonly int CURL_ERROR_CODE = 56000;

        private readonly double HTTP_DEFAULT_ERROR_CODE = 500.00;
        private readonly string HTTP_DEFAULT_ERROR_TEXT = "Unknown HTTP access error";

        private readonly string ENV_RSRCNAME_KEY = "RESOURCE_NAME";
        private readonly string ENV_COMPPATH_KEY = "COMPONENT_LOCATION";

        private string _attachedResourceName;
        private string _attachedResourceUrlValue;
        private string _logFileName;
        private string _magicxpiTempDir;
        private string _curlProgram;
        private bool _debugIsOn;
        private long _fsid;
        private long _randValue;

        /// <summary>
        /// Lista de propriedades INPUT da configuração
        /// </summary>
        private enum CUrlInputArgs {
            httpVerb,
            urlDynAddition,
            headersToSend,
            dataToSend,
            additionalCurlFlags,
            keepWorkingFiles
        }

        /// <summary>
        /// Lista de propriedades OUTPUT da configuração
        /// </summary>
        private enum CUrlOutputArgs {
            finalSiteUrl,
            finalCUrlCmd,
            outputHeaders,
            outputData,
            httpResultCode,
            httpResultText,
            outputCUrl,
            exitCodeCUrl
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string enumStr(CUrlInputArgs value) {
            string result = value.ToString();

            //
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string enumStr(CUrlOutputArgs value) {
            string result = value.ToString();

            //
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="data"></param>
        private void writeAllBytes(string file, byte[] data) {

            if (!(data == null)) {
                File.WriteAllBytes(file, data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private byte[] readAllBytes(string file) {
            byte[] result = null; // default return value

            if (File.Exists(file)) {
                result = File.ReadAllBytes(file);
            }
            //
            return (result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string read1stLine(string file, IFormatProvider format) {
            string result = "HTTP/1.0 " + HTTP_DEFAULT_ERROR_CODE.ToString("0.00", format) + " " + HTTP_DEFAULT_ERROR_TEXT; // default return value

            if (File.Exists(file)) {
                string[] lines = File.ReadAllLines(file);
                int lineIdx = 0;
                foreach (string line in lines) {
                    if (line.Trim() == string.Empty) {
                        // as CURL might return multiples HTTP status code,
                        // every empty line resets the logic
                        lineIdx = 0;
                    } else {
                        lineIdx++;
                    }

                    if (lineIdx == 1) {
                        // This is the target line
                        result = line.Trim();
                        break;
                    }
                }
            }
            //
            return (result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private double extractHttpCode(string text, IFormatProvider format) {
            double result = HTTP_DEFAULT_ERROR_CODE;

            if (!(text == null) && !(text.Trim() == string.Empty)) {
                text = text.Trim();
                while (text.IndexOf("  ") >= 0) {
                    text = text.Replace("  ", " ");
                }
                if (text.IndexOf(" ") >= 0) {
                    string[] tokens = text.Split(new char[] { ' ' });
                    int tokenCnt = 0;
                    foreach (string token in tokens) {
                        tokenCnt++;
                        if (tokenCnt == 2) {
                            // Queremos o 2o token
                            Double.TryParse(token, NumberStyles.Float, format, out result);
                            break;
                        }
                    }
                }
            }

            //
            return (result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exitCode"></param>
        /// <param name="screenOutput"></param>
        /// <returns></returns>
        private string getCurlErrorMessage(long exitCode, string screenOutput) {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(screenOutput)) {
                string[] lines = screenOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                string matchToken = string.Format("curl: ({0})", exitCode);
                foreach (string line in lines) {
                    if (line.Trim().ToLower().StartsWith(matchToken.ToLower())) {
                        // Pega a 1a linha
                        result = line.Trim();
                        break;
                    }
                }
            }

            //
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void appendLogMessage(string message) {

            try {
                message = "\r\n" + string.Format("{0:yyyy'-'dd'-'MM HH':'mm':'ss'.'ffffffK}", DateTime.Now) + " CUrl log [FSID:" + _fsid.ToString() + "/Rsrc:" + _attachedResourceName + "/ExecID:" + _randValue.ToString() + "] " + message;
                File.AppendAllText(_logFileName, message);

            } catch { };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpVerb"></param>
        /// <param name="urlDynAddition"></param>
        /// <param name="headersToSend"></param>
        /// <param name="dataToSend"></param>
        /// <param name="additionalCurlFlags"></param>
        /// <param name="keepWorkingFiles"></param>
        /// <param name="out_finalSiteUrl"></param>
        /// <param name="out_finalCUrlCmd"></param>
        /// <param name="out_outputHeaders"></param>
        /// <param name="out_outputData"></param>
        /// <param name="out_httpResultCode"></param>
        /// <param name="out_httpResultText"></param>
        /// <param name="out_outputCUrl"></param>
        /// <param name="out_exitCodeCUrl"></param>
        private void execCurlCommand(string httpVerb, string urlDynAddition, byte[] headersToSend, byte[] dataToSend, string additionalCurlFlags, bool keepWorkingFiles, ref string out_finalSiteUrl, ref string out_finalCUrlCmd, ref byte[] out_outputHeaders, ref byte[] out_outputData, ref double out_httpResultCode, ref string out_httpResultText, ref byte[] out_outputCUrl, ref long out_exitCodeCUrl) {

            IFormatProvider format = new CultureInfo("en-US");
            string simpleDateFormat = string.Format("{0:yyyyddMMHHmmssffffff}", DateTime.Now);
            string clientID = simpleDateFormat + "_" + Guid.NewGuid().ToString("N").ToUpper();

            // Arquivos de trabalho
            string rqstHFName = _magicxpiTempDir + Path.DirectorySeparatorChar + "curl_HEADER_RQST_" + clientID + ".log";
            string rqstDFName = _magicxpiTempDir + Path.DirectorySeparatorChar + "curl_DATA_RQST_" + clientID + ".log";
            string respHFName = _magicxpiTempDir + Path.DirectorySeparatorChar + "curl_HEADER_RESP_" + clientID + ".log";
            string respDFName = _magicxpiTempDir + Path.DirectorySeparatorChar + "curl_DATA_RESP_" + clientID + ".log";
            string outputFName = _magicxpiTempDir + Path.DirectorySeparatorChar + "curl_OUTPUT_" + clientID + ".log";

            try {

                // Flags
                bool haveHeadersTosend = (!(headersToSend == null)) && (headersToSend.Length > 0);
                bool haveDataTosend = (!(dataToSend == null)) && (dataToSend.Length > 0);

                // Url de acesso
                out_finalSiteUrl = _attachedResourceUrlValue;
                if (!string.IsNullOrEmpty(urlDynAddition)) {
                    if (out_finalSiteUrl.EndsWith("/") || urlDynAddition.StartsWith("/")) {

                        out_finalSiteUrl += urlDynAddition;
                    } else {

                        out_finalSiteUrl += ("/" + urlDynAddition);
                    }
                }

                // Dados de envio
                if (haveHeadersTosend) {
                    writeAllBytes(rqstHFName, headersToSend);
                }
                if (haveDataTosend) {
                    writeAllBytes(rqstDFName, dataToSend);
                }

                // Comando CURL (completo)
                string curlCmdLineArgs = " -v -L -X " + httpVerb + ((haveHeadersTosend) ? " -H \"@" + rqstHFName + "\"" : string.Empty) + ((haveDataTosend) ? " --data-binary \"@" + rqstDFName + "\"" : string.Empty) + " -D \"" + respHFName + "\"" + " -o \"" + respDFName + "\"" + ((additionalCurlFlags == string.Empty) ? string.Empty : " " + additionalCurlFlags.Trim()) + " --compressed \"" + out_finalSiteUrl.Trim() + "\"";
                out_finalCUrlCmd = "\"" + _curlProgram + "\"" + curlCmdLineArgs;

                if (_debugIsOn) {
                    // DEBUG
                    appendLogMessage("httpVerb                  : " + httpVerb);
                    appendLogMessage("fullSiteUrl               : " + out_finalSiteUrl);
                    appendLogMessage("haveHeadersTosend         : " + ((haveHeadersTosend) ? "yes (" + headersToSend.Length + " bytes)" : "no"));
                    appendLogMessage("haveDataTosend            : " + ((haveDataTosend) ? "yes (" + dataToSend.Length + " bytes)" : "no"));
                    appendLogMessage("tempDir                   : " + _magicxpiTempDir);
                    appendLogMessage("clientID                  : " + clientID);
                    appendLogMessage("keep working files on disc: " + ((!keepWorkingFiles) ? "no" : "yes"));
                    appendLogMessage("command                   : " + out_finalCUrlCmd);
                }

                // Executa o "curl"
                Stopwatch elapTime = new Stopwatch();
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.FileName = "\"" + _curlProgram + "\"";
                p.StartInfo.Arguments = curlCmdLineArgs;

                elapTime.Start();
                p.Start();
                string progOutput = p.StandardError.ReadToEnd();
                p.WaitForExit();
                elapTime.Stop();

                // Pega os resultas da operação
                out_exitCodeCUrl = p.ExitCode;
                out_outputCUrl = (string.IsNullOrEmpty(progOutput)) ? null : Encoding.Default.GetBytes(progOutput);
                out_outputHeaders = readAllBytes(respHFName);
                out_outputData = readAllBytes(respDFName);
                out_httpResultText = read1stLine(respHFName, format);
                out_httpResultCode = extractHttpCode(out_httpResultText, format);

                if(out_exitCodeCUrl > 0 && out_httpResultCode > 299) {
                    string curlAdditionalMessage = getCurlErrorMessage(out_exitCodeCUrl, progOutput);
                    if( !string.IsNullOrEmpty(curlAdditionalMessage)) {

                        out_httpResultText += " {" + curlAdditionalMessage.Trim() + "}";
                    }
                }

                if (_debugIsOn) {
                    // DEBUG
                    appendLogMessage("elapsed time              : " + elapTime.ElapsedMilliseconds.ToString() + "ms");
                    appendLogMessage("exit code                 : " + out_exitCodeCUrl.ToString());
                    appendLogMessage("HTTP result               : " + out_httpResultText);
                }

                writeAllBytes(outputFName, out_outputCUrl);

            } finally {

                if (!keepWorkingFiles) {

                    try {

                        if (File.Exists(rqstHFName))
                            File.Delete(rqstHFName);

                        if (File.Exists(rqstDFName))
                            File.Delete(rqstDFName);

                        if (File.Exists(respHFName))
                            File.Delete(respHFName);

                        if (File.Exists(respDFName))
                            File.Delete(respDFName);

                        if (File.Exists(outputFName))
                            File.Delete(outputFName);

                    } catch { };

                }

            }

        }

        /// <summary>
        /// Construtor da classe
        /// </summary>
        public runtime() {
        }

        #region IStep implementation
        /// <summary>
        /// Invocado pelo Magic xpi Server
        /// </summary>
        /// <param name="stepParams"></param>
        public void invoke(StepGeneralParams stepParams) {
            try {

                #region
                // Propriedades [In]
                UserProperty httpVerbProperty = stepParams.getUserProperty(enumStr(CUrlInputArgs.httpVerb)); // string
                UserProperty urlDynAdditionProperty = stepParams.getUserProperty(enumStr(CUrlInputArgs.urlDynAddition)); // string
                UserProperty headersToSendProperty = stepParams.getUserProperty(enumStr(CUrlInputArgs.headersToSend)); // byte[]
                UserProperty dataToSendProperty = stepParams.getUserProperty(enumStr(CUrlInputArgs.dataToSend)); // byte[]
                UserProperty additionalCurlFlagsProperty = stepParams.getUserProperty(enumStr(CUrlInputArgs.additionalCurlFlags)); // string
                UserProperty keepWorkingFilesProperty = stepParams.getUserProperty(enumStr(CUrlInputArgs.keepWorkingFiles)); // bool

                // Propriedades [Out]
                UserProperty finalSiteUrlProperty = stepParams.getUserProperty(enumStr(CUrlOutputArgs.finalSiteUrl)); // ALPHA
                UserProperty finalCUrlCmdProperty = stepParams.getUserProperty(enumStr(CUrlOutputArgs.finalCUrlCmd)); // ALPHA
                UserProperty outputHeadersProperty = stepParams.getUserProperty(enumStr(CUrlOutputArgs.outputHeaders)); // BLOB
                UserProperty outputDataProperty = stepParams.getUserProperty(enumStr(CUrlOutputArgs.outputData)); // BLOB
                UserProperty httpResultCodeProperty = stepParams.getUserProperty(enumStr(CUrlOutputArgs.httpResultCode));  // NUMERIC
                UserProperty httpResultTextProperty = stepParams.getUserProperty(enumStr(CUrlOutputArgs.httpResultText)); // ALPHA
                UserProperty outputCUrlProperty = stepParams.getUserProperty(enumStr(CUrlOutputArgs.outputCUrl)); // BLOB
                UserProperty exitCodeCUrlProperty = stepParams.getUserProperty(enumStr(CUrlOutputArgs.exitCodeCUrl)); // NUMERIC

                _randValue = (new Random()).Next(1, Int32.MaxValue);
                _fsid = stepParams.FSID;
                _attachedResourceName = stepParams.EnviromentSettings[ENV_RSRCNAME_KEY];
                _attachedResourceUrlValue = stepParams.ResourceObject[CURL_RSRC_KEY];
                _logFileName = stepParams.EnviromentSettings[ENV_COMPPATH_KEY].Trim() + "\\..\\..\\logs\\CUrl_SDKComponent_" + string.Format("{0:yyyy'-'dd'-'MM}", DateTime.Now) + ".log";
                _curlProgram = stepParams.EnviromentSettings[ENV_COMPPATH_KEY].Trim() + "\\program\\bin\\curl.exe";
                _magicxpiTempDir = stepParams.EnviromentSettings[ENV_COMPPATH_KEY].Trim() + "\\..\\..\\temp";

                string[] dmLines = new string[0];
                if (!(stepParams.PayloadOBject == null)) {

                    dmLines = Encoding.UTF8.GetString(stepParams.PayloadOBject).Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                }

                // Analisa a 2a linha do FF
                _debugIsOn = dmLines.Length > 1 && (dmLines[1].ToUpper() == "Y" || dmLines[1].ToUpper() == "S" || dmLines[1].ToUpper() == "1");
                #endregion

                #region
                // Validar parâmetros de entrada "obrigatórios"
                // No mínimo um 'GET' de algum site tem de ter
                if ((httpVerbProperty.getValue() == null) || string.IsNullOrEmpty((string)httpVerbProperty.getValue())) {

                    throw new ArgumentNullException(enumStr(CUrlInputArgs.httpVerb));
                } else if (string.IsNullOrEmpty(_attachedResourceName)) {

                    throw new ArgumentNullException("attached resource name");
                } else if (string.IsNullOrEmpty(_attachedResourceUrlValue)) {

                    throw new ArgumentNullException("resource [ " + _attachedResourceName.Trim() + " ] value");
                }

                // Extrai os argumentos de entrada
                string httpVerb = httpVerbProperty.getValue().ToString().Trim().ToUpper();
                string urlDynAddition = (urlDynAdditionProperty.getValue() == null) ? string.Empty : urlDynAdditionProperty.getValue().ToString().Trim();
                byte[] headersToSend = (headersToSendProperty.getValue() == null) ? null : (byte[])headersToSendProperty.getValue();
                byte[] dataToSend = (dataToSendProperty.getValue() == null) ? null : (byte[])dataToSendProperty.getValue();
                string additionalCurlFlags = (additionalCurlFlagsProperty.getValue() == null) ? string.Empty : additionalCurlFlagsProperty.getValue().ToString().Trim();
                bool keepWorkingFiles = (keepWorkingFilesProperty.getValue() == null) ? false : (bool)keepWorkingFilesProperty.getValue();

                // Cria os argumentos de saída
                string finalSiteUrl = string.Empty;
                string finalCUrlCmd = string.Empty;
                byte[] outputHeaders = null;
                byte[] outputData = null;
                double httpResultCode = HTTP_DEFAULT_ERROR_CODE;
                string httpResultText = HTTP_DEFAULT_ERROR_TEXT;
                byte[] outputCUrl = null;
                long exitCodeCUrl = 0;
                #endregion

                #region
                ///
                /// Chama o programa "curl" por linha de comando
                ///
                if (_debugIsOn) {
                    appendLogMessage("[CUrl] SDK Component from MagicBR, (C) 2021 -- No warranty or support, of any kind, is provided");
                }

                execCurlCommand(httpVerb, urlDynAddition, headersToSend, dataToSend, additionalCurlFlags, keepWorkingFiles, ref finalSiteUrl, ref finalCUrlCmd, ref outputHeaders, ref outputData, ref httpResultCode, ref httpResultText, ref outputCUrl, ref exitCodeCUrl);
                #endregion

                #region
                // Atualiza as propriedades de saída [Out]
                finalSiteUrlProperty.setAlpha(finalSiteUrl);
                finalCUrlCmdProperty.setAlpha(finalCUrlCmd);
                outputHeadersProperty.setBlob(outputHeaders);
                outputDataProperty.setBlob(outputData);
                httpResultCodeProperty.setNumeric(httpResultCode);
                httpResultTextProperty.setAlpha(httpResultText);
                outputCUrlProperty.setBlob(outputCUrl);
                exitCodeCUrlProperty.setNumeric((double)exitCodeCUrl);
                #endregion

            } catch (Exception e) {

                // retorna qqer problema como uma exceção "conhecida", de código "conhecido", para poder ser tratada corretamente pelo Error Handling do Magi xpi
                throw new XPI_SDK.SDKException(CURL_ERROR_CODE, "[CUrl] SDK Component Error: " + e.GetType().ToString() + ": " + e.Message.Replace(Environment.NewLine, " "));
            }
        }
        #endregion
    }
}

// eof