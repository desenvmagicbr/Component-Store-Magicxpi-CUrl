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

using MagicSoftware.Integration.UserComponents;

using System;
using System.ComponentModel;
using System.Reflection;

/// <summary>
/// 
/// </summary>
namespace com.magicbr.curl {

    /// <summary>
    /// Caracter´tsicas indivuduas de cada propriedade da class
    /// </summary>
    public struct CUrlConfigDataDetail {
        public string PropertyName;
        public string PropertyType;
        public bool PropertyIsMandatory;
        public bool PropertyIsNotSet;
        public int PropertyId;
        public string PropertyDisplayText;
        public string PropertyHint;
        public bool PropertyCanBeNotSet;
        public bool PropertyIsInput;
        public bool PropertyIsOutput;
        public string PropertyMagicType;
        public bool PropertyAcceptExpressionEditor;
        public bool PropertyAcceptVariablesSelector;
        public object PropertyValue;
        public MagicSoftware.Integration.UserComponents.DataType PropertyDataType;
        public MagicSoftware.Integration.UserComponents.Id AttrId;
        public MagicSoftware.Integration.UserComponents.PrimitiveDataTypes AttrPrimitiveDataType;
        public MagicSoftware.Integration.UserComponents.DisplayPropertyName AttrDisplayPropertyName;
        public System.ComponentModel.DescriptionAttribute AttrDescription;
        public MagicSoftware.Integration.UserComponents.AllowEmptyExpression AttrAllowEmptyExpression;
        public MagicSoftware.Integration.UserComponents.In AttrIn;
        public MagicSoftware.Integration.UserComponents.Out AttrOut;
    }

    /// <summary>
    /// Dados de configuração para edição (Studio) e execução (Server)
    /// </summary>
    public class CUrlConfigData {

        /// <summary>
        /// Nome da propriedade do recurso (CUrl)
        /// </summary>
        public static readonly string CURL_RSRC_KEY_NAME = @"CUrl";
        public static readonly string CURL_CONFIG_WIN_TITLE = @"CUrl Component Configuration - (C) 2021, Magic Software Brasil";

        //
        // Arumentos IN
        //

        /// <summary>
        /// {Obrigatório} Verbo HTTP {GET, POST, PUT, PATCH, HEAD ou DELETE} a usar na chamada
        /// </summary>
        [Id(1)]
        [PrimitiveDataTypes(DataType.Alpha)]
        [DisplayPropertyName("HTTP Verb")]
        [Description("HTTP verb to use on this call")]
        [In]
        public Expression httpVerb { get; set; }

        /// <summary>
        /// {Opcional} Complemento da Url a usar com a Url do recurso, na chamada
        /// </summary>
        [Id(2)]
        [PrimitiveDataTypes(DataType.Alpha)]
        [DisplayPropertyName("Url Dynamic Addition")]
        [Description("Additional informartion to add to resource's url")]
        [AllowEmptyExpression]
        [In]
        public Expression urlDynAddition { get; set; }

        /// <summary>
        /// {Opcional} Cabeçalhos HTTP a usar na chamada (separados por CR_LF)
        /// </summary>
        [Id(3)]
        [PrimitiveDataTypes(DataType.Blob)]
        [DisplayPropertyName("Headers to Send")]
        [Description("HTTP headers to send (separated by CR_LF)")]
        [AllowEmptyExpression]
        [In]
        public Expression headersToSend { get; set; }

        /// <summary>
        /// {Opcional} Dados a usar na chamada { GET é um exemplo }
        /// </summary>
        [Id(4)]
        [PrimitiveDataTypes(DataType.Blob)]
        [DisplayPropertyName("Data to Send")]
        [Description("Data (content) to send")]
        [AllowEmptyExpression]
        [In]
        public Expression dataToSend { get; set; }

        /// <summary>
        /// {Opcional} Flags para controlar o funcionamento do comando CURL
        /// </summary>
        [Id(5)]
        [PrimitiveDataTypes(DataType.Alpha)]
        [DisplayPropertyName("CUrl Additional Flags to Use")]
        [Description("Additional CUrl flags for behavior control and tunning")]
        [AllowEmptyExpression]
        [In]
        public Expression additionalCurlFlags { get; set; }

        /// <summary>
        /// {Obrigatório} TRUE/FALSE se deve deixar os arquivos de trbalho no disco, ou apa´gá-los ao final da execução
        /// </summary>
        [Id(6)]
        [PrimitiveDataTypes(DataType.Logical)]
        [DisplayPropertyName("Keep Working Files on Disc")]
        [Description("Keep or delete temporary working files")]
        [In]
        public Expression keepWorkingFiles { get; set; }

        //
        // Arumentos OUT
        //

        /// <summary>
        /// {Opcional} conterá a Url final usada no acesso, que pode ser a combinação do "recurso" + o "parâmetro #2"
        /// </summary>
        [Id(7)]
        [PrimitiveDataTypes(DataType.Alpha)]
        [DisplayPropertyName("Final Site Url")]
        [Description("Final Url/Uri effectively used during this call")]
        [AllowEmptyExpression]
        [Out]
        public Variable finalSiteUrl { get; set; }

        /// <summary>
        /// {Opcional} conterá a linha completa do comando CURL, usada na chamada
        /// </summary>
        [Id(8)]
        [PrimitiveDataTypes(DataType.Alpha)]
        [DisplayPropertyName("Final CUrl Command")]
        [Description("Full CUrl command line built for this call")]
        [AllowEmptyExpression]
        [Out]
        public Variable finalCUrlCmd { get; set; }

        /// <summary>
        /// {Obrigatório} conterá (se possível) os cabeçalho HTTP retornados pelo site chamado
        /// </summary>
        [Id(9)]
        [PrimitiveDataTypes(DataType.Blob)]
        [DisplayPropertyName("Output Headers")]
        [Description("HTTP headers received from remote url/uri")]
        [AllowEmptyExpression]
        [Out]
        public Variable outputHeaders { get; set; }

        /// <summary>
        /// {Obrigatório} conterá (se possível) os dados retornados pelo site chamado
        /// </summary>
        [Id(10)]
        [PrimitiveDataTypes(DataType.Blob)]
        [DisplayPropertyName("Output Data")]
        [Description("Data (contents) received from remote url/uri")]
        [Out]
        public Variable outputData { get; set; }

        /// <summary>
        /// {Obrigatório} conterá (se possível) o código HTTP retornados pelo site chamado
        /// </summary>
        [Id(11)]
        [PrimitiveDataTypes(DataType.Numeric)]
        [DisplayPropertyName("HTTP Result Code")]
        [Description("HTTP code received from remote url/uri")]
        [Out]
        public Variable httpResultCode { get; set; }

        /// <summary>
        /// {Opcional} conterá (se possível) a mensagem HTTP retornada pelo site chamado
        /// </summary>
        [Id(12)]
        [PrimitiveDataTypes(DataType.Alpha)]
        [DisplayPropertyName("HTTP Result Text")]
        [Description("HTTP message received from remote url/uri")]
        [AllowEmptyExpression]
        [Out]
        public Variable httpResultText { get; set; }

        /// <summary>
        /// {Obrigatório} conterá a "tela de saída" do comando CURL, durante a sua execução
        /// </summary>
        [Id(13)]
        [PrimitiveDataTypes(DataType.Blob)]
        [DisplayPropertyName("CUrl Output Log")]
        [Description("CUrl window output during this call")]
        [AllowEmptyExpression]
        [Out]
        public Variable outputCUrl { get; set; }

        [Id(14)]
        [PrimitiveDataTypes(DataType.Numeric)]
        [DisplayPropertyName("CUrl Output Code")]
        [Description("Command exit code during this call (0=Success)")]
        [AllowEmptyExpression]
        [Out]
        public Variable exitCodeCUrl { get; set; }

        [Id(90)]
        [PrimitiveDataTypes(DataType.Logical)]
        [DisplayPropertyName("Configured")]
        [Description("TRUE or FALSE if current component is already configured")]
        [ExcludeFromRuntime]
        [ExcludeFromTextSearch]
        public Logical isConfigured { get; set; }

        /// <summary>
        /// Inicializador das propriedades
        /// </summary>
        private void InitClassMembers() {

            // Vamos inicializar todas as propriedades, para evitar futuros problema com PROP == null :)
            // E para definir alguns valores DEFAULT também

            //IN (6)
            #region
            httpVerb = new Expression();
            httpVerb.SetValue("'GET'"); // Verbo padrão de uso nos acessos HTTP

            urlDynAddition = new Expression();
            urlDynAddition.SetValue(string.Empty);

            headersToSend = new Expression();
            headersToSend.SetValue("'Accept-Encoding: gzip, deflate' & ASCIIChr (13) & ASCIIChr (10) & 'Connection: keep-alive'"); // Cabeçalhos básicos de acesso

            dataToSend = new Expression();
            dataToSend.SetValue(string.Empty);

            additionalCurlFlags = new Expression();
            additionalCurlFlags.SetValue("'--ipv4 --connect-timeout 10 --max-time 290'"); // Define um timeOut total de 5min (300seg)

            keepWorkingFiles = new Expression();
            keepWorkingFiles.SetValue("'FALSE'LOG"); // Não manter em disco os arquivos temporários de trabalho
            #endregion

            // OUT (8)
            #region
            finalSiteUrl = new Variable();
            finalSiteUrl.SetValue(string.Empty);

            finalCUrlCmd = new Variable();
            finalCUrlCmd.SetValue(string.Empty);

            outputHeaders = new Variable();
            outputHeaders.SetValue(string.Empty);

            outputData = new Variable();
            outputData.SetValue("C.UserBlob"); // Guardar os dados de retorno do site, na C.UserBlob

            httpResultCode = new Variable();
            httpResultCode.SetValue("C.UserCode"); // Guardar o código HTTP retornado do site, na C.UserCose

            httpResultText = new Variable();
            httpResultText.SetValue("C.UserString"); // Guardar a mensagem HTTP retornado do site, na C.UserCose

            outputCUrl = new Variable();
            outputCUrl.SetValue(string.Empty);

            exitCodeCUrl = new Variable();
            exitCodeCUrl.SetValue(string.Empty);
            #endregion

            // Internas (1)
            isConfigured = new Logical();
            isConfigured.SetLogical(false);

        }

        /// <summary>
        /// Construtor da Classe
        /// </summary>
        public CUrlConfigData() {
            InitClassMembers();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CUrlConfigDataDetail[] GetDetails() {
            CUrlConfigDataDetail[] result;

            Type t = this.GetType();
            PropertyInfo[] props = t.GetProperties();

            result = new CUrlConfigDataDetail[props.Length];
            int idx = 0;

            foreach (PropertyInfo prp in props) {
                Type tP = prp.GetType();
                object vP = prp.GetValue(this, new object[] { });
                CUrlConfigDataDetail decP = new CUrlConfigDataDetail();

                #region
                decP.PropertyName = prp.Name;
                decP.PropertyType = prp.PropertyType.Name;
                decP.PropertyValue = vP;
                decP.PropertyIsNotSet = (decP.PropertyValue == null) || string.IsNullOrEmpty(decP.PropertyValue.ToString());
                decP.AttrId = (Id)Attribute.GetCustomAttribute(prp, typeof(MagicSoftware.Integration.UserComponents.Id));
                decP.AttrPrimitiveDataType = (PrimitiveDataTypes)Attribute.GetCustomAttribute(prp, typeof(MagicSoftware.Integration.UserComponents.PrimitiveDataTypes));
                decP.AttrDisplayPropertyName = (DisplayPropertyName)Attribute.GetCustomAttribute(prp, typeof(MagicSoftware.Integration.UserComponents.DisplayPropertyName));
                decP.AttrDescription = (DescriptionAttribute)Attribute.GetCustomAttribute(prp, typeof(System.ComponentModel.DescriptionAttribute));
                decP.AttrAllowEmptyExpression = (AllowEmptyExpression)Attribute.GetCustomAttribute(prp, typeof(MagicSoftware.Integration.UserComponents.AllowEmptyExpression));
                decP.AttrIn = (In)Attribute.GetCustomAttribute(prp, typeof(MagicSoftware.Integration.UserComponents.In));
                decP.AttrOut = (Out)Attribute.GetCustomAttribute(prp, typeof(MagicSoftware.Integration.UserComponents.Out));
                decP.PropertyIsMandatory = decP.AttrAllowEmptyExpression == null;
                decP.PropertyId = ((decP.AttrId == null) ? 0 : (int)decP.AttrId.ID);
                decP.PropertyHint = ((decP.AttrDescription == null) ? string.Empty : decP.AttrDescription.Description);
                decP.PropertyDisplayText = ((decP.AttrDisplayPropertyName == null) ? string.Empty : decP.AttrDisplayPropertyName.DisplayName);
                decP.PropertyCanBeNotSet = !(decP.AttrAllowEmptyExpression == null);
                decP.PropertyIsInput = (!(decP.AttrIn == null) && decP.AttrIn.IsInbound);
                decP.PropertyIsOutput = (!(decP.AttrOut == null) && decP.AttrOut.IsOutbound);
                decP.PropertyMagicType = decP.AttrPrimitiveDataType.DataType.ToString();
                decP.PropertyAcceptExpressionEditor = decP.PropertyType.ToUpper().StartsWith(@"EXPRESSION");
                decP.PropertyAcceptVariablesSelector = decP.PropertyType.ToUpper().StartsWith(@"VARIABLE");
                if (decP.PropertyMagicType.ToUpper().StartsWith(@"BLOB")) {
                    decP.PropertyDataType = DataType.Blob;
                } else if (decP.PropertyMagicType.ToUpper().StartsWith(@"LOGICAL")) {
                    decP.PropertyDataType = DataType.Logical;
                } else if (decP.PropertyMagicType.ToUpper().StartsWith(@"NUMERIC")) {
                    decP.PropertyDataType = DataType.Numeric;
                } else if (decP.PropertyMagicType.ToUpper().StartsWith(@"DATE")) {
                    decP.PropertyDataType = DataType.Date;
                } else if (decP.PropertyMagicType.ToUpper().StartsWith(@"TIME")) {
                    decP.PropertyDataType = DataType.Time;
                } else {
                    decP.PropertyDataType = DataType.Alpha;
                }
                #endregion

                result[idx] = decP;
                //
                idx++;
            }

            //
            return result;
        }


    }

}

// eof