/***
 * (C) 2021, Magic Software BR
 * Form de edição da "configurações" do componente CUrl para Magic xpi 4.5 ou >
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
//
using MagicSoftware.Integration.UserComponents;
using MagicSoftware.Integration.UserComponents.Interfaces;
//
using com.magicbr.curl;

/// <summary>
/// 
/// </summary>
namespace com.magicbr.curl {

    /// <summary>
    /// Classe WinForm que faz a interface de configuração com o usuário
    /// </summary>
    public partial class uiForm : Form {

        private DataTable _editTable;
        private CUrlConfigData _cfgData;
        private CUrlConfigDataDetail[] _cfgDataDetails;
        private ISDKStudioUtils _utils;
        private IReadOnlyResourceConfiguration _resourceData;
        private int _initialSelectedRow = -1;
        private bool _isDesigning = true;
        private bool _configChanged = false;

        private const int COLUMN_ID = 0;
        private const int COLUMN_NAME = 1;
        private const int COLUMN_DESCRIPTION = 2;
        private const int COLUMN_TYPE = 3;
        private const int COLUMN_DIRECTION = 4;
        private const int COLUMN_VALUE = 5;
        private const int COLUMN_BUTTON = 6;
        private const int COLUMN_MAX = 6;

        /// <summary>
        /// Flag (Propriedade) se mudou ou não algo nas edições
        /// </summary>
        public bool ConfigurationChanged {
            get { 
                return _configChanged; 
            }
            set {
                _configChanged = value;
                lblChangeAlert.Visible = _configChanged;
            } 
        }

        /// <summary>
        /// Construtor da classe (WinForm)
        /// </summary>
        public uiForm() {

            InitializeComponent();
            //
            ConfigurationChanged = false;
            _editTable = new DataTable("CUrlConfigProperties");
            this.Text = CUrlConfigData.CURL_CONFIG_WIN_TITLE;
        }

        /// <summary>
        /// Form concluiu seu carregamento e criação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiForm_Load(object sender, EventArgs e) {
            // Form foi criado (instaciando)
        }

        /// <summary>
        /// Obtém os deatalhes de uma propriedade, com base no seu Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private CUrlConfigDataDetail? GetPropertyDetails(int Id) {
            CUrlConfigDataDetail? result = null;

            if (!(_cfgDataDetails == null)) {
                foreach (CUrlConfigDataDetail det in _cfgDataDetails) {
                    if (det.PropertyId == Id) {
                        result = det;
                        break;
                    }
                }
            }

            //
            return result;
        }

        /// <summary>
        /// Atualizar a propriedade que sofreu a alteração
        /// </summary>
        /// <param name="propDet"></param>
        /// <param name="xpiExpNew"></param>
        /// <param name="xpiVarNew"></param>
        private void HandlePropertyValueChanged(CUrlConfigDataDetail? propDet, Expression xpiExpNew, Variable xpiVarNew) {
            bool propChanged = true;
            string newValue = string.Empty;

            //
            // Aqui, o bom seria atualizar por reflexão ( Runtime.Reflection ). Mas ... (o desenvolvedor não domina tanto)  :|
            //
            if (propDet.Value.PropertyName.ToUpper() == "httpVerb".ToUpper()) {
                // [In]

                _cfgData.httpVerb = xpiExpNew;
            } else if (propDet.Value.PropertyName.ToUpper() == "urlDynAddition".ToUpper()) {
                // [In]

                _cfgData.urlDynAddition = xpiExpNew;
            } else if (propDet.Value.PropertyName.ToUpper() == "headersToSend".ToUpper()) {
                // [In]

                _cfgData.headersToSend = xpiExpNew;
            } else if (propDet.Value.PropertyName.ToUpper() == "dataToSend".ToUpper()) {
                // [In]

                _cfgData.dataToSend = xpiExpNew;
            } else if (propDet.Value.PropertyName.ToUpper() == "additionalCurlFlags".ToUpper()) {
                // [In]

                _cfgData.additionalCurlFlags = xpiExpNew;
            } else if (propDet.Value.PropertyName.ToUpper() == "keepWorkingFiles".ToUpper()) {
                // [In]

                _cfgData.keepWorkingFiles = xpiExpNew;
            } else if (propDet.Value.PropertyName.ToUpper() == "finalSiteUrl".ToUpper()) {
                // [Out]

                _cfgData.finalSiteUrl = xpiVarNew;
            } else if (propDet.Value.PropertyName.ToUpper() == "finalCUrlCmd".ToUpper()) {
                // [Out]

                _cfgData.finalCUrlCmd = xpiVarNew;
            } else if (propDet.Value.PropertyName.ToUpper() == "outputHeaders".ToUpper()) {
                // [Out]

                _cfgData.outputHeaders = xpiVarNew;
            } else if (propDet.Value.PropertyName.ToUpper() == "outputData".ToUpper()) {
                // [Out]

                _cfgData.outputData = xpiVarNew;
            } else if (propDet.Value.PropertyName.ToUpper() == "httpResultCode".ToUpper()) {
                // [Out]

                _cfgData.httpResultCode = xpiVarNew;
            } else if (propDet.Value.PropertyName.ToUpper() == "httpResultText".ToUpper()) {
                // [Out]

                _cfgData.httpResultText = xpiVarNew;
            } else if (propDet.Value.PropertyName.ToUpper() == "outputCUrl".ToUpper()) {
                // [Out]

                _cfgData.outputCUrl = xpiVarNew;
            } else if (propDet.Value.PropertyName.ToUpper() == "exitCodeCUrl".ToUpper()) {
                // [Out]

                _cfgData.exitCodeCUrl = xpiVarNew;
            } else {

                propChanged = false;
            }

            if (propChanged) {

                ConfigurationChanged = true;
                _cfgDataDetails = _cfgData.GetDetails();

                //
                // Atualizar o DataTable para refletir a mudança (visualmente)
                //
                if (xpiExpNew == null) {

                    newValue = xpiVarNew.GetValue().ToString();
                } else {

                    newValue = xpiExpNew.GetValue().ToString();
                }
                _editTable.Rows[propDet.Value.PropertyId - 1]["Value"] = newValue;
            }
        }

        /// <summary>
        /// Trata o "click" no botão de ZOOM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseButtonClick(object sender, DataGridViewCellEventArgs e) {
            CUrlConfigDataDetail? propDet = GetPropertyDetails(e.RowIndex + 1);
            DataGridView dgv = (DataGridView)sender;
            int row = dgv.SelectedCells[0].RowIndex;
            int col = dgv.SelectedCells[0].ColumnIndex;

            dgv.CurrentCell = dgv.Rows[row].Cells[COLUMN_VALUE + 1];

            if (!(propDet == null)) {

                if (_isDesigning) {

                    if (propDet.Value.PropertyAcceptExpressionEditor) {
                        // Editor de EXPRESSÕES

                        Expression xpiExpCurrent = (Expression)propDet.Value.PropertyValue;
                        string propOldValue = xpiExpCurrent.GetValue().Trim();
                        Expression xpiExpNew = _utils.OpenExpressionEditor(xpiExpCurrent, propDet.Value.PropertyDataType, propDet.Value.PropertyDisplayText);
                        string propNewValue = xpiExpNew.GetValue().Trim();
                        if (propOldValue != propNewValue) {
                            // A edição alterou o valor da propriedade (por menor que seja a mudança)

                            if (string.IsNullOrEmpty(propNewValue) && propDet.Value.PropertyIsMandatory) {

                                _utils.OpenErrorBox("This is a 'mandatory' propoerty and must have an expression assigned to it", false);
                            } else {
                                HandlePropertyValueChanged(propDet, xpiExpNew, null);
                            }
                        };
                    } else if (propDet.Value.PropertyAcceptVariablesSelector) {
                        // Seletor de VARIÁVEIS

                        Variable xpiVarCurrent = (Variable)propDet.Value.PropertyValue;
                        string propOldValue = xpiVarCurrent.GetValue().Trim();
                        Variable xpiVarNew = _utils.OpenVariablePicklist(xpiVarCurrent, VariableFilter.ALLVariables, propDet.Value.PropertyDataType);
                        string propNewValue = xpiVarNew.GetValue().Trim();
                        if (propOldValue != propNewValue) {
                            // A edição alterou o valor da propriedade (por menor que seja a mudança)

                            if (string.IsNullOrEmpty(propNewValue) && propDet.Value.PropertyIsMandatory) {

                                _utils.OpenErrorBox("This is a 'mandatory' propoerty and must have a variable assigned to it", false);
                            } else {

                                HandlePropertyValueChanged(propDet, null, xpiVarNew);
                            }
                        };
                    }
                } else {

                    _utils.OpenErrorBox("Property editors can be opnned only when in 'designing' mode", false);
                }
            }
        }

        /// <summary>
        /// Usuário fez "click" em alguma célula
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewEditProperties_CellClick(object sender, DataGridViewCellEventArgs e) {

            if (e.RowIndex >= 0 && e.ColumnIndex == 0) {
                // clicou no botão (à direita) do ZOOM
                HandleMouseButtonClick(sender, e);
            }
        }

        /// <summary>
        /// Usuário fez "double click" em alguma célula
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewEditProperties_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {

            if (e.RowIndex >= 0 && (e.ColumnIndex == -1 || e.ColumnIndex == (COLUMN_VALUE + 1))) {
                // clicou no botão (à esquerda) do seletor
                DataGridViewCellEventArgs e1 = new DataGridViewCellEventArgs(0, e.RowIndex);
                HandleMouseButtonClick(sender, e1);
            }
        }

        /// <summary>
        /// Mostrar toolTipo para alguma célula
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewEditProperties_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e) {

            if (e.RowIndex >= 0 && e.ColumnIndex == (COLUMN_NAME + 1)) {
                // Mouse sobre a coluna "Property"
                CUrlConfigDataDetail? propDet = GetPropertyDetails(e.RowIndex + 1);
                e.ToolTipText = (propDet == null) ? string.Empty : propDet.Value.PropertyHint;
            } else if (e.RowIndex >= 0 && e.ColumnIndex == (COLUMN_ID + 1)) {
                // Mouse sobre a coluna "Id"
                CUrlConfigDataDetail? propDet = GetPropertyDetails(e.RowIndex + 1);
                e.ToolTipText = (propDet == null) || !propDet.Value.PropertyIsMandatory ? string.Empty : "Mandatory";
            } else if (e.RowIndex >= 0 && e.ColumnIndex == (COLUMN_VALUE + 1)) {
                // Mouse sobre a coluna "Value"
                CUrlConfigDataDetail? propDet = GetPropertyDetails(e.RowIndex + 1);
                e.ToolTipText = (propDet == null) ? string.Empty : propDet.Value.PropertyValue.ToString();
            } else if (e.RowIndex >= 0 && e.ColumnIndex == 0) {
                // Mouse sobre a coluna "BUTTON"
                CUrlConfigDataDetail? propDet = GetPropertyDetails(e.RowIndex + 1);
                e.ToolTipText = ((propDet == null) ? string.Empty : ((propDet.Value.PropertyAcceptExpressionEditor) ? "Click to open 'Expression Editor'" : ((propDet.Value.PropertyAcceptVariablesSelector) ? "Click to open 'Variable Selector'" : string.Empty)));
            }
        }

        /// <summary>
        /// Célula em foco mudou
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewEditProperties_SelectionChanged(object sender, EventArgs e) {
            DataGridView dgv = (DataGridView)sender;

            if (dgv.SelectedCells.Count > 0) {
                int row = dgv.SelectedCells[0].RowIndex;
                int col = dgv.SelectedCells[0].ColumnIndex;

                if (row >= 0 && col == (COLUMN_VALUE + 1)) {
                    // Selecionou a célula do "VALOR"
                } else if (row >= 0 && col == 0) {
                    // Selecionou a célula do "BOTÃO"
                } else if (row >= 0 && dgv.RowCount > row && dgv.ColumnCount > (COLUMN_BUTTON)) {
                    // Selecionou outra célula

                    dgv.CurrentCell = dgv.Rows[row].Cells[COLUMN_VALUE + 1];
                }
            }
        }

        /// <summary>
        /// Está "desenhando" cada célula do GRID. Aqui é o local de se alterar o padrão visual de alguma célula em específico
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewEditProperties_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) {

                if (e.Value != null) {
                    CUrlConfigDataDetail? propDet = GetPropertyDetails(e.RowIndex + 1);
                    DataGridView dgv = (DataGridView)sender;

                    if (propDet.Value.PropertyIsMandatory && e.ColumnIndex <= COLUMN_VALUE) {
                        // Mostrar em negrito as colunas de propriedades "orbigatórias"

                        e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                    } else if (propDet.Value.PropertyIsOutput && e.ColumnIndex == (COLUMN_VALUE + 1)) {
                        // Mostrar em itálico as variáveis [Out]

                        e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Italic);
                    }
                }
            }
        }

        /// <summary>
        /// Janela foi exibia inicialmente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiForm_Shown(object sender, EventArgs e) {

            if ((_initialSelectedRow >= 0) && (_initialSelectedRow < _editTable.Rows.Count)) {
                int row = _initialSelectedRow;
                _initialSelectedRow = -1;

                // Posicionar numa linha específica (logo de início)
                if (_isDesigning) {
                    DataGridView dgv = dataGridViewEditProperties;
                    dgv.CurrentCell = dgv.Rows[row].Cells[COLUMN_VALUE + 1];

                    // Simular duplo click no botão
                    DataGridViewCellEventArgs e1 = new DataGridViewCellEventArgs(0, row);
                    HandleMouseButtonClick(dgv, e1);
                }
            }
        }

        /// <summary>
        /// Clicou no hyperLink do label central
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelMagicBR_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            //
            System.Diagnostics.Process.Start(@"http://blog.magicsoftware.com.br/connector-builder-o-canivete-suico-do-magic-xpi-parte-i/");
        }

        /// <summary>
        /// Liga o DBLBuffer do Grid, para otimizar o desenho dele na tela ...
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="setting"></param>
        private void DoubleBufferedDataGridView(DataGridView dgv, bool setting) {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        /// <summary>
        /// Lê o arquivo de recursos e extrai a informação definida
        /// </summary>
        /// <param name="rsrcName"></param>
        /// <param name="rsrcFullPathName"></param>
        /// <returns></returns>
        private string GetResourceValue(string rsrcName, string rsrcFullPathName) {
            string result = string.Empty;

            try {

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(rsrcFullPathName);

                bool nameMatch = false;
                foreach (XmlNode n1 in xDoc.DocumentElement) {
                    if (n1.LocalName == "Resource_Element") {
                        foreach (XmlNode n2 in n1.ChildNodes) {
                            if (n2.LocalName == "Main_Resource_Defintion") {
                                foreach (XmlNode n3 in n2.ChildNodes) {
                                    nameMatch = nameMatch || (n3.LocalName == "Resource_Type_Name" && n3.InnerText == rsrcName);
                                    if (n3.LocalName == "Resource_Properties") {
                                        foreach (XmlNode n4 in n3.ChildNodes) {
                                            if (n4.LocalName == "ExpressionValue") {
                                                if(nameMatch) {
                                                    // É este

                                                    result = " : " + n4.InnerText;
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(result))
                                                break;
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(result))
                                        break;
                                }
                            }
                            if (!string.IsNullOrEmpty(result))
                                break;
                        }
                    }
                    if (!string.IsNullOrEmpty(result))
                        break;
                }

            } catch { }

            //
            return result;
        }

        #region Public methods
        /// <summary>
        /// Inicializa o "form" com os dados das propriedades que devem ser exibidas/editadas
        /// E exibe-o, para permitir a edição
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="utils"></param>
        /// <param name="resourceData"></param>
        /// <param name="navigateTo"></param>
        public DialogResult Configure(ref CUrlConfigData cfgData, ISDKStudioUtils utils, IReadOnlyResourceConfiguration resourceData, object navigateTo) {
            DialogResult result = DialogResult.Cancel;
            string sysPropStudioDesignMode = utils.GetSystemProperty("IsStudioInDesignMode");

            _cfgData = cfgData;
            _cfgDataDetails = _cfgData.GetDetails();
            _utils = utils;
            _resourceData = resourceData;
            _isDesigning = sysPropStudioDesignMode.ToUpper().StartsWith("Y") || sysPropStudioDesignMode.ToUpper().StartsWith("T") || sysPropStudioDesignMode.ToUpper().StartsWith("1");

            // Adiciona as colunas (6: Id, Property, Description, Type, Direction, Value)
            DoubleBufferedDataGridView(dataGridViewEditProperties, true);
            dataGridViewEditProperties.Columns.Clear();
            dataGridViewEditProperties.ColumnHeadersDefaultCellStyle.BackColor = Color.FromKnownColor(KnownColor.Control);
            dataGridViewEditProperties.EnableHeadersVisualStyles = false;
            for (int i = 0; i <= COLUMN_MAX; i++) {

                if (!(i == COLUMN_BUTTON)) {
                    DataColumn column = new DataColumn();

                    column.DataType = System.Type.GetType("System.String");
                    column.AutoIncrement = false;
                    column.ReadOnly = false;

                    // Coluna da tabela
                    if (i == COLUMN_ID) {
                        // Coluna #1

                        column.ColumnName = "Id";
                        column.Caption = column.ColumnName;
                        column.AutoIncrement = false;
                        column.Unique = true;
                    } else if (i == COLUMN_NAME) {
                        // Coluna #2

                        column.ColumnName = "Property";
                        column.Caption = column.ColumnName;
                        column.AutoIncrement = false;
                        column.Unique = true;
                    } else if (i == COLUMN_DESCRIPTION) {
                        // Coluna #3

                        column.ColumnName = "Description";
                        column.Caption = column.ColumnName;
                        column.AutoIncrement = false;
                        column.Unique = false;
                    } else if (i == COLUMN_TYPE) {
                        // Coluna #4

                        column.ColumnName = "Type";
                        column.Caption = column.ColumnName;
                        column.AutoIncrement = false;
                        column.Unique = false;
                    } else if (i == COLUMN_DIRECTION) {
                        // Coluna #5

                        column.ColumnName = "Direction";
                        column.Caption = column.ColumnName;
                        column.AutoIncrement = false;
                        column.Unique = false;
                    } else if (i == COLUMN_BUTTON) {
                        // Coluna #7

                    } else {
                        // Coluna #6

                        column.ColumnName = "Value";
                        column.Caption = column.ColumnName;
                        column.AutoIncrement = false;
                        column.Unique = false;
                    }

                    //
                    _editTable.Columns.Add(column);
                }
            }

            // "Liga" a tabela ao Grid
            dataGridViewEditProperties.DataSource = _editTable;

            // Ajusta a aparência e comportamento (default) das colunas
            for (int i = 0; i <= COLUMN_MAX; i++) {
                DataGridViewColumn gCol = dataGridViewEditProperties.Columns[i];

                // Coluna da tabela
                if (i == COLUMN_ID) {
                    // Coluna #1

                    gCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                    gCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    gCol.ReadOnly = true;
                    gCol.Frozen = true;
                    gCol.Width = 30;
                } else if (i == COLUMN_NAME) {
                    // Coluna #2

                    gCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                    gCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    gCol.ReadOnly = true;
                    gCol.Frozen = true;
                    gCol.Width = 170;

                } else if (i == COLUMN_DESCRIPTION) {
                    // Coluna #3

                    gCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                    gCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    gCol.ReadOnly = true;
                    gCol.Frozen = true;
                    gCol.Width = 300;
                } else if (i == COLUMN_TYPE) {
                    // Coluna #4

                    gCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                    gCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    gCol.ReadOnly = true;
                    gCol.Frozen = true;
                    gCol.Width = 80;
                } else if (i == COLUMN_DIRECTION) {
                    // Coluna #5

                    gCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                    gCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    gCol.ReadOnly = true;
                    gCol.Frozen = true;
                    gCol.Width = 60;
                } else if (i == COLUMN_BUTTON) {
                    // Coluna #7

                    gCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                    gCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    gCol.ReadOnly = true;
                    gCol.Frozen = true;
                    gCol.Width = 30;
                    gCol.HeaderText = string.Empty;
                } else {
                    // Coluna #6

                    gCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                    gCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    gCol.ReadOnly = true;
                    gCol.Frozen = true;
                    gCol.Width = 410;

                    // Adiciona o botão (7a coluna)
                    DataGridViewButtonColumn gNewCol = new DataGridViewButtonColumn();
                    gNewCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                    gNewCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    gNewCol.ReadOnly = true;
                    gNewCol.Frozen = true;
                    gNewCol.Width = 30;
                    gNewCol.Text = "...";
                    gNewCol.HeaderText = string.Empty;
                    gNewCol.UseColumnTextForButtonValue = true;
                    dataGridViewEditProperties.Columns.Insert(i + 1, gNewCol);
                }
            }

            // Adiciona as linhas (Uma para cada propriedade)
            // São 6 colunas: Id, Property, Description, Type, Direction e Value
            foreach (CUrlConfigDataDetail propDet in _cfgDataDetails) {

                if (propDet.PropertyAcceptExpressionEditor || propDet.PropertyAcceptVariablesSelector) {
                    DataRow row = _editTable.NewRow();
                    row[COLUMN_ID] = propDet.PropertyId.ToString();
                    row[COLUMN_NAME] = propDet.PropertyDisplayText;
                    row[COLUMN_DESCRIPTION] = propDet.PropertyHint;
                    row[COLUMN_TYPE] = propDet.PropertyMagicType.ToUpper();
                    row[COLUMN_DIRECTION] = (propDet.PropertyIsInput && propDet.PropertyIsOutput) ? "[In/Out]" : ((propDet.PropertyIsInput) ? "[In]" : ((propDet.PropertyIsOutput) ? "[Out]" : string.Empty));
                    row[COLUMN_VALUE] = (propDet.PropertyValue == null) ? string.Empty : propDet.PropertyValue.ToString();

                    //
                    _editTable.Rows.Add(row);
                }
            }

            // Completar algumas informações
            string iconFile = _utils.GetSystemProperty("ConnectorPath") + "\\icons\\curl.gif";
            if (File.Exists(iconFile)) {
                pictureBoxLogo.ImageLocation = iconFile;
            }
            new ToolTip().SetToolTip(pictureBoxLogo, iconFile);

            // Cehcar se a ação é "normal" ou veio de um "MoveTo"
            if (!(navigateTo == null)) {
                if (Int32.TryParse(navigateTo.ToString(), out _initialSelectedRow)) {
                    _initialSelectedRow--;
                    lblNavigateTo.Text = "Direct \"Move To\" to property id: #" + navigateTo.ToString();
                }
            }
            lblRsrcInfo.Text = resourceData.ResourceName + GetResourceValue(resourceData.ResourceName, Path.Combine(_utils.GetSystemProperty("ProjectPath"), "Resources.xml"));

            // Só permite confirmar a edição, qdo o Studio está em "Design Mode"
            btnOk.Enabled = _isDesigning;

            // Exibir o FORM
            result = this.ShowDialog();

            //
            return (result);
        }
        #endregion
    }
}

// eof