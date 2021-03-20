/***
 * (C) 2021, Magic Software BR
 * Classe de "configuração" (edição das propriedades) do componente CUrl para Magic xpi 4.5 ou >
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
using System.Collections.Generic;
//
using MagicSoftware.Integration.UserComponents.Interfaces;
using MagicSoftware.Integration.UserComponents;
//
using com.magicbr.curl;
//
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace com.magicbr.curl {

    /// <summary>
    /// 
    /// </summary>
    [Export(typeof(IUserComponent))]
    public class ui : IUserComponent {

        private ISDKStudioUtils _utils;
        private IReadOnlyResourceConfiguration _rsrcData;

        /// <summary>
        /// Construtor da classe
        /// </summary>
        public ui() {
        }

        #region IUserComponent implementation

        /// <summary>
        /// Cria uma instância do objeto que guarda as configurações
        /// </summary>
        /// <returns></returns>
        public object CreateDataObject() {
            return new CUrlConfigData();
        }

        /// <summary>
        /// Chamado pelo Studio quando é necessário "configurar" o componente
        /// </summary>
        /// <param name="dataObject">Deve ser atualizado com uma instância do objetoi de "configuração"</param>
        /// <param name="utils">Recebe um objeto do xpi Studio com diversas ferramentas disponíveis para uso nesta interface (UI)</param>
        /// <param name="resourceData">Recebe um objeto do xpi Studio com dados do recurso associado ao componente</param>
        /// <param name="navigateTo">Quando <> de NULL, indica que foi ativado pelo "Finder" do xpi Studio</param>
        /// <param name="configurationChanged">Deve ser atualizado com TRUE/FALSE para avisar se alguma configuração foi alterada</param>
        /// <returns></returns>
        public bool? Configure(ref object dataObject, ISDKStudioUtils utils, IReadOnlyResourceConfiguration resourceData, object navigateTo, out bool configurationChanged) {

            bool? confirmed = new bool?(false);

            // Guarda a instância destes utilitários/ferramentas, para uso em outros métodos (se necessário)
            _utils = utils;
            _rsrcData = resourceData;

            // Handling dataObject null situation (1st config for example)
            if (!(dataObject is CUrlConfigData)) {

                dataObject = CreateDataObject();
            }

            CUrlConfigData adaptorData = dataObject as CUrlConfigData;

            // Cópia de trabalho do objeto
            CUrlConfigData adaptorDataWorkObject = new CUrlConfigData {
                additionalCurlFlags = adaptorData.additionalCurlFlags,
                dataToSend = adaptorData.dataToSend,
                exitCodeCUrl = adaptorData.exitCodeCUrl,
                finalCUrlCmd = adaptorData.finalCUrlCmd,
                finalSiteUrl = adaptorData.finalSiteUrl,
                headersToSend = adaptorData.headersToSend,
                httpResultCode = adaptorData.httpResultCode,
                httpResultText = adaptorData.httpResultText,
                httpVerb = adaptorData.httpVerb,
                keepWorkingFiles = adaptorData.keepWorkingFiles,
                outputCUrl = adaptorData.outputCUrl,
                outputData = adaptorData.outputData,
                outputHeaders = adaptorData.outputHeaders,
                urlDynAddition = adaptorData.urlDynAddition,
                isConfigured = adaptorData.isConfigured
            };

            configurationChanged = false; //Indicate a change in configuration resulting in dirty state

            // Abre e exibe a tela de configuração
            uiForm frm = new uiForm();
            frm.ConfigurationChanged = !adaptorDataWorkObject.isConfigured.GetLogical(); // Se nunca fez a configuração antes, a 1a vez é considerada: configuração alterada
            confirmed = new bool?(frm.Configure(ref adaptorDataWorkObject, utils, resourceData, navigateTo) == DialogResult.OK);

            if (confirmed.HasValue && confirmed.Value) {

                // Fechou pelo botão OK
                configurationChanged = frm.ConfigurationChanged;

                if (configurationChanged) {
                    // Atualiza o objeto de configuração de retorno

                    #region
                    adaptorData.additionalCurlFlags = adaptorDataWorkObject.additionalCurlFlags;
                    adaptorData.dataToSend = adaptorDataWorkObject.dataToSend;
                    adaptorData.exitCodeCUrl = adaptorDataWorkObject.exitCodeCUrl;
                    adaptorData.finalCUrlCmd = adaptorDataWorkObject.finalCUrlCmd;
                    adaptorData.finalSiteUrl = adaptorDataWorkObject.finalSiteUrl;
                    adaptorData.headersToSend = adaptorDataWorkObject.headersToSend;
                    adaptorData.httpResultCode = adaptorDataWorkObject.httpResultCode;
                    adaptorData.httpResultText = adaptorDataWorkObject.httpResultText;
                    adaptorData.httpVerb = adaptorDataWorkObject.httpVerb;
                    adaptorData.keepWorkingFiles = adaptorDataWorkObject.keepWorkingFiles;
                    adaptorData.outputCUrl = adaptorDataWorkObject.outputCUrl;
                    adaptorData.outputData = adaptorDataWorkObject.outputData;
                    adaptorData.outputHeaders = adaptorDataWorkObject.outputHeaders;
                    adaptorData.urlDynAddition = adaptorDataWorkObject.urlDynAddition;
                    adaptorData.isConfigured.SetLogical(true);
                    #endregion

                    //
                    dataObject = adaptorData;
                }
            }

            //
            return confirmed; // confirmed; //se retornar TRUE, o Studio abre o "DataMapper" interno para continuar a configuração. Se retornar FALSE, não abre.
        }

        /// <summary>
        /// Usaremos, mas para exercitar somente, a opção de uso do DataMapper interno de configuração do componente
        /// Aqui, tem retornar uma instância de GetXMLSchemaConfiguration() ou GetJSonSchemaConfiguration() ou GetFFSchemaConfiguration()
        /// </summary>
        /// <returns></returns>
        public SchemaInfo GetSchema() // returns the schema object created during configurations
        {
            SchemaInfo resultSchema = GetFFSchemaConfiguration();

            //
            return resultSchema;
        }

        /// <summary>
        /// Não usaremos, mas aqui podemos adicionar código extra para usar no "Checker", quando ele estiver validando o nosso componente
        /// Retornando NULL, o checker terá o comportamnto padrão
        /// </summary>
        /// <param name="data"></param>
        /// <param name="resourceData"></param>
        /// <returns></returns>
        public ICheckerResult Check(ref object data, IReadOnlyResourceConfiguration resourceData) {

            //
            return null; // can be used to return additional results o the builds in checker mechanism	
        }

        /// <summary>
        /// Botão "Validate" clicado, este código é chamado em resposta ao "click"
        /// </summary>
        /// <param name="resourceData">Dados do recurso</param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool ValidateResource(IReadOnlyResourceConfiguration resourceData, out string errorMsg) {

            //errorMsg should be set to null in case there is no error
            errorMsg = null;

            //This method receives read-only copy of the Resource.
            //String myProperty = serviceData.GetPropertyValue("ServicePropertyName");
            //You should open your own dialog here with the success/failure message
            String cUrlRemoteSite = resourceData.GetPropertyValue(CUrlConfigData.CURL_RSRC_KEY_NAME);

            // Validação básica, para fins didáticos apenas
            if (string.IsNullOrEmpty(cUrlRemoteSite)) {

                errorMsg = @"Site Url/Uri must be set";
            } else if (cUrlRemoteSite.Trim().StartsWith(@"/") || cUrlRemoteSite.Trim().EndsWith(@"/")) {

                errorMsg = "Site Url/Uri must not start or end with \"/\"";
            }

            if (string.IsNullOrEmpty(errorMsg))
                MessageBox.Show("Resource Url is OK!", "Resource Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //Method should return true for success and false for failure
            return string.IsNullOrEmpty(errorMsg);
        }

        /// <summary>
        /// Não usaremos, mas se o nosso "Recurso" tivesse "botões customizaddos", este código seria chamado em resposta aos "clicks"
        /// </summary>
        /// <param name="helperId">ID do botão clicado na tela de recursos</param>
        /// <param name="resourceData">Dados do recurso</param>
        public void InvokeResourceHelper(string helperId, IResourceConfiguration resourceData) {

            // This method receives an updateable Resource object and the name of the helper button that was pressed.
            // In order to get a property from the Resource:
            // string myProperty = resourceData.GetPropertyValue("ResourcePropertyName");

            // You should open your own configuration dialogs

            // MessageBox.Show("Define your configuration dialogs and update the Resource if required");

            // In order to set a properties of the Resource:

            /*
                using (IResourceConfigurationWriter writer =  resourceData.BeginEditing())
                {
                    writer.SetPropertyValue("property1","Val1");
                    writer.SetPropertyValue("property2","Val2");
                    writer.AcceptChanges();
                }
            */
        }
        #endregion

        #region Internal DM Schema Definitions
        /// <summary>
        /// Não usaremos, mas este método é para retornar um SchemInfo() do tipo "XML", para uso pelo DataMapper do configurador
        /// </summary>
        /// <returns></returns>
        public SchemaInfo GetXMLSchemaConfiguration() {

            // Create the schema info based on the user entered data...
            XMLSchemaInfo xmlSchemaInfoLocal = new XMLSchemaInfo {
                SchemaName = "CUrlConnector_Xml_Schema",
                AlwayCreateNodes = true,
                AppendData = false,
                Description = "Schema for 'XML Style' mapping of CUrl component",
                RecursionDepth = 3,
                XMLEncoding = XMLSchemaInfo.UTF_8,
                XMLValidation = false,
                XSDSchemaFilePath = _utils.GetSystemProperty("ConnectorPath") + "\\ui\\lib\\StepUi.xsd"
            };

            //
            return xmlSchemaInfoLocal;
        }

        /// <summary>
        /// Não usaremos, mas este método é para retornar um SchemInfo() do tipo "JSon", para uso pelo DataMapper do configurador
        /// </summary>
        /// <returns></returns>
        public SchemaInfo GetJSonSchemaConfiguration() {

            // Create the schema info based on the user entered data...
            JSonSchemaInfo jSonSchemaInfoLocal = new JSonSchemaInfo {
                SchemaName = "CUrlConnector_JSon_Schema",
                AlwayCreateNodes = true,
                Description = "Schema for 'JSon Style' mapping of CUrl component",
                JSonEncoding = XMLSchemaInfo.ANSI,
                JSonSchemaFilePath = _utils.GetSystemProperty("ConnectorPath") + "\\ui\\lib\\StepUi.JSon"
            };

            //
            return jSonSchemaInfoLocal;
        }

        /// <summary>
        /// Usaremos, e definiremos apenas um campo de entrada
        /// </summary>
        /// <returns></returns>
        public SchemaInfo GetFFSchemaConfiguration() {

            // Create the schema info based on the user entered data...
            return new FlatFileSchemaInfo {
                SchemaName = "CUrlConnector_FlatFile_Schema",
                CreateHeaderLine = true,
                DelimitedPositional = 1,
                Delimiter = ",",
                Lines = new List<FlatFileRecordArguments>
                {
                    new FlatFileRecordArguments
                    {
                        Name = "ComponentDebugOn", Type = DataType.Alpha, Picture = "1", Lenght = 1
                    }
                }
            };
        }
        #endregion

    }
}

// eof