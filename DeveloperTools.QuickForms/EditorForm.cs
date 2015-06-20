using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DeveloperTools.QuickForms
{
    public class EditorForm<T> : Form
        where T : class
    {
        const int WIDTH = 480;
        const int HEIGHT = 80;
        const String BUTTON_OK = "OK";
        const String BUTTON_CANCEL = "Cancel";

        Type entityType;
        List<FieldMetadata> fields;
        PropertyInfo[] properties;
        MethodInfo[] methods;

        public EditorForm()
        {
            this.entityType = typeof(T);
            this.properties = this.entityType.GetProperties();
            this.methods = this.entityType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            this.fields = new List<FieldMetadata>();

            Initialization();
        }

        public void Bind(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            foreach (FieldMetadata field in this.fields)
            {
                field.Bind(entity);
            }
        }

        public void UnBind(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            foreach (FieldMetadata field in this.fields)
            {
                field.Unbind(entity);
            }
        }

        private void Initialization()
        {
            int row = 0;

            // form initialization
            this.Text = entityType.FullName;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoSize = true;
            this.MinimumSize = new System.Drawing.Size(WIDTH, HEIGHT);
            this.Size = new System.Drawing.Size(WIDTH, HEIGHT);

            // editor controls initialization
            TableLayoutPanel panelForEditors = new TableLayoutPanel();
            panelForEditors.Dock = DockStyle.Fill;
            panelForEditors.AutoSize = true;

            for (int i = 0; i < this.properties.Length; ++row, ++i)
            {
                FieldMetadata fieldMetadata = new FieldMetadata
                {
                    editControl = null,
                    editorAttribute = GetEditorAttribute(this.properties[i]),
                    propertyInfo = this.properties[i]
                };

                GenerateEditControl(panelForEditors, fieldMetadata, row);

                this.fields.Add(fieldMetadata);
            }

            // hack - stretchable controls
            panelForEditors.Controls.Add(new Control(), 0, row);
            panelForEditors.Controls.Add(new Control(), 1, row);
            this.Controls.Add(panelForEditors);

            // generate action handlers
            FlowLayoutPanel panelForButtons = new FlowLayoutPanel();
            panelForButtons.Dock = DockStyle.Bottom;
            panelForButtons.FlowDirection = FlowDirection.LeftToRight;
            panelForButtons.AutoSize = true;
            this.Controls.Add(panelForButtons);

            // generate OK button
            panelForButtons.Controls.Add(GenerateButtonControl(BUTTON_OK, DialogResult.OK));

            // generate cancel button
            panelForButtons.Controls.Add(GenerateButtonControl(BUTTON_CANCEL, DialogResult.Cancel));
        }

        private void GenerateEditControl(TableLayoutPanel parent, FieldMetadata fieldMetadata, int row)
        {
            // TODO: nullable type
            // TODO: validation
            // TODO: file & directory
            // TODO: simplification & rationalization

            Type propertyType = fieldMetadata.propertyInfo.PropertyType;

            String propertyName = fieldMetadata.propertyInfo.Name;
            String displayName = (fieldMetadata.editorAttribute == null ? fieldMetadata.propertyInfo.Name : fieldMetadata.editorAttribute.DisplayName);

            if (String.IsNullOrWhiteSpace(displayName))
            {
                displayName = propertyName;
            }

            Label label = GenerateLabelControl(displayName);

            if (propertyType.IsEnum)
            {
                ComboBox control = new ComboBox();
                control.Dock = DockStyle.Fill;
                control.DropDownStyle = ComboBoxStyle.DropDownList;
                control.Name = "edit" + propertyName;

                String[] names = Enum.GetNames(propertyType);

                foreach (String name in names)
                {
                    control.Items.Add(name);
                }

                parent.Controls.Add(label, 0, row);
                parent.Controls.Add(control, 1, row);

                fieldMetadata.editControl = control;
            }
            else if (propertyType == typeof(String))
            {
                if (fieldMetadata.editorAttribute == null || fieldMetadata.editorAttribute.EditorType == EditorFeatureType.MultiLine)
                {
                    TextBox control = new TextBox();
                    control.Dock = DockStyle.Fill;
                    control.Name = "edit" + propertyName;

                    if (fieldMetadata.editorAttribute != null)
                    {
                        if (fieldMetadata.editorAttribute.EditorType == EditorFeatureType.MultiLine)
                        {
                            control.Multiline = true;
                            control.MinimumSize = new System.Drawing.Size(50, 100);
                        }
                    }

                    parent.Controls.Add(label, 0, row);
                    parent.Controls.Add(control, 1, row);

                    fieldMetadata.editControl = control;
                }
                else if (fieldMetadata.editorAttribute != null && fieldMetadata.editorAttribute.EditorType == EditorFeatureType.OpenFile)
                {
                    TextBoxWithButton control = new TextBoxWithButton();
                    control.Dock = DockStyle.Fill;
                    control.Name = "edit" + propertyName;

                    control.ButtonClick += delegate (object sender, EventArgs ea)
                    {
                        OpenFileDialog dialog = new OpenFileDialog();
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            control.Value = dialog.FileName;
                        }
                    };

                    parent.Controls.Add(label, 0, row);
                    parent.Controls.Add(control, 1, row);

                    fieldMetadata.editControl = control;
                }
                else if (fieldMetadata.editorAttribute != null && fieldMetadata.editorAttribute.EditorType == EditorFeatureType.OpenDirectory)
                {
                    TextBoxWithButton control = new TextBoxWithButton();
                    control.Dock = DockStyle.Fill;
                    control.Name = "edit" + propertyName;

                    control.ButtonClick += delegate (object sender, EventArgs ea)
                    {
                        FolderBrowserDialog dialog = new FolderBrowserDialog();
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            control.Value = dialog.SelectedPath;
                        }
                    };

                    parent.Controls.Add(label, 0, row);
                    parent.Controls.Add(control, 1, row);

                    fieldMetadata.editControl = control;
                }
            }
            else if (propertyType == typeof(Boolean))
            {
                CheckBox control = new CheckBox();
                control.Dock = DockStyle.Fill;
                control.Name = "edit" + propertyName;

                control.Text = displayName;

                // parent.Controls.Add(label, 0, row);
                parent.Controls.Add(control, 1, row);

                fieldMetadata.editControl = control;
            }
            else if (propertyType == typeof(Decimal)
                || propertyType == typeof(Double)
                || propertyType == typeof(Single)
                || propertyType == typeof(Int16)
                || propertyType == typeof(Int32)
                || propertyType == typeof(Int64)
                || propertyType == typeof(UInt16)
                || propertyType == typeof(UInt32)
                || propertyType == typeof(UInt64))
            {
                NumericUpDown control = new NumericUpDown();
                control.Dock = DockStyle.Fill;
                control.Name = "edit" + propertyName;

                parent.Controls.Add(label, 0, row);
                parent.Controls.Add(control, 1, row);

                fieldMetadata.editControl = control;
            }
            else if (propertyType == typeof(DateTime))
            {
                DateTimePicker control = new DateTimePicker();
                control.Dock = DockStyle.Fill;
                control.Name = "edit" + propertyName;
                control.Format = DateTimePickerFormat.Custom;
                control.CustomFormat = "yyyy.MM.dd hh:mm:ss";
                control.MinDate = DateTime.MinValue;
                control.MaxDate = DateTime.MaxValue;

                parent.Controls.Add(label, 0, row);
                parent.Controls.Add(control, 1, row);

                fieldMetadata.editControl = control;
            }
            else if (propertyType == typeof(TimeSpan))
            {
                DateTimePicker control = new DateTimePicker();
                control.Dock = DockStyle.Fill;
                control.Name = "edit" + propertyName;
                control.Format = DateTimePickerFormat.Custom;
                control.CustomFormat = "hh:mm:ss";
                control.ShowUpDown = true;

                parent.Controls.Add(label, 0, row);
                parent.Controls.Add(control, 1, row);

                fieldMetadata.editControl = control;
            }
            else
            {
                throw new NotSupportedException(propertyType.FullName);
            }
        }

        private static Label GenerateLabelControl(String text)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Dock = DockStyle.Fill;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            label.Text = text;

            return label;
        }

        private static Button GenerateButtonControl(String text, DialogResult dialogResult)
        {
            Button button = new Button();
            button.Text = text;
            button.DialogResult = dialogResult;

            return button;
        }

        private static EditorAttribute GetEditorAttribute(PropertyInfo propertyInfo)
        {
            object[] attributes = propertyInfo.GetCustomAttributes(typeof(EditorAttribute), true);
            if (attributes != null && attributes.Length > 0)
            {
                return (EditorAttribute)attributes[0];
            }

            return null;
        }

        public static bool ShowDialog(T entity)
        {
            EditorForm<T> editor = new EditorForm<T>();
            editor.Bind(entity);

            if (editor.ShowDialog() == DialogResult.OK)
            {
                editor.UnBind(entity);
                return true;
            }

            return false;
        }

        class FieldMetadata
        {
            public PropertyInfo propertyInfo;
            public EditorAttribute editorAttribute;
            public Control editControl;

            public void Bind(object entity)
            {
                if (editControl == null)
                {
                    return;
                }

                if (entity == null)
                {
                    return;
                }

                Type propertyType = this.propertyInfo.PropertyType;
                object value = this.propertyInfo.GetValue(entity, null);

                if (propertyType.IsEnum)
                {
                    ComboBox control = (ComboBox)editControl;

                    control.SelectedIndex = (int)value;
                }
                else if (propertyType == typeof(String))
                {
                    if (this.editorAttribute == null || this.editorAttribute.EditorType == EditorFeatureType.MultiLine)
                    {
                        TextBox control = (TextBox)editControl;

                        control.Text = Convert.ToString(value);
                    }
                    else if (this.editorAttribute != null && this.editorAttribute.EditorType == EditorFeatureType.OpenFile)
                    {
                        TextBoxWithButton control = (TextBoxWithButton)editControl;

                        control.Value = Convert.ToString(value);
                    }
                }
                else if (propertyType == typeof(Decimal)
                    || propertyType == typeof(Double)
                    || propertyType == typeof(Single)
                    || propertyType == typeof(Int16)
                    || propertyType == typeof(Int32)
                    || propertyType == typeof(Int64)
                    || propertyType == typeof(UInt16)
                    || propertyType == typeof(UInt32)
                    || propertyType == typeof(UInt64))
                {
                    NumericUpDown control = (NumericUpDown)editControl;

                    control.Value = Convert.ToDecimal(value);
                }
                else if (propertyType == typeof(DateTime))
                {
                    DateTimePicker control = (DateTimePicker)editControl;

                    DateTime dateTime = (DateTime)value;
                    if (dateTime < control.MinDate)
                    {
                        dateTime = DateTime.Today;
                    }

                    if (dateTime > control.MaxDate)
                    {
                        dateTime = control.MaxDate;
                    }

                    control.Value = dateTime;
                }
                else if (propertyType == typeof(TimeSpan))
                {
                    DateTimePicker control = (DateTimePicker)editControl;

                    control.Value = DateTime.Today.Add((TimeSpan)value);
                }
                else if (propertyType == typeof(Boolean))
                {
                    CheckBox control = (CheckBox)editControl;

                    control.Checked = Convert.ToBoolean(value);
                }
            }

            public void Unbind(object entity)
            {
                if (editControl == null)
                {
                    return;
                }

                if (entity == null)
                {
                    return;
                }

                Type propertyType = this.propertyInfo.PropertyType;

                if (propertyType.IsEnum)
                {
                    ComboBox control = (ComboBox)editControl;
                    propertyInfo.SetValue(entity, Enum.Parse(propertyType, control.SelectedItem.ToString()), null);
                }
                else if (propertyType == typeof(String))
                {
                    if (this.editorAttribute == null || this.editorAttribute.EditorType == EditorFeatureType.MultiLine)
                    {
                        TextBox control = (TextBox)editControl;
                        propertyInfo.SetValue(entity, control.Text, null);
                    }
                    else if (this.editorAttribute != null && this.editorAttribute.EditorType == EditorFeatureType.OpenFile)
                    {
                        TextBoxWithButton control = (TextBoxWithButton)editControl;
                        propertyInfo.SetValue(entity, control.Value, null);
                    }
                }
                else if (propertyType == typeof(Decimal))
                {
                    NumericUpDown control = (NumericUpDown)editControl;
                    propertyInfo.SetValue(entity, Convert.ToDecimal(control.Value), null);
                }
                else if (propertyType == typeof(Double))
                {
                    NumericUpDown control = (NumericUpDown)editControl;
                    propertyInfo.SetValue(entity, Convert.ToDouble(control.Value), null);
                }
                else if (propertyType == typeof(Single))
                {
                    NumericUpDown control = (NumericUpDown)editControl;
                    propertyInfo.SetValue(entity, Convert.ToSingle(control.Value), null);
                }
                else if (propertyType == typeof(Int16))
                {
                    NumericUpDown control = (NumericUpDown)editControl;
                    propertyInfo.SetValue(entity, Convert.ToInt16(control.Value), null);
                }
                else if (propertyType == typeof(Int32))
                {
                    NumericUpDown control = (NumericUpDown)editControl;
                    propertyInfo.SetValue(entity, Convert.ToInt32(control.Value), null);
                }
                else if (propertyType == typeof(Int64))
                {
                    NumericUpDown control = (NumericUpDown)editControl;
                    propertyInfo.SetValue(entity, Convert.ToInt64(control.Value), null);
                }
                else if (propertyType == typeof(UInt16))
                {
                    NumericUpDown control = (NumericUpDown)editControl;
                    propertyInfo.SetValue(entity, Convert.ToUInt16(control.Value), null);
                }
                else if (propertyType == typeof(UInt32))
                {
                    NumericUpDown control = (NumericUpDown)editControl;
                    propertyInfo.SetValue(entity, Convert.ToUInt32(control.Value), null);
                }
                else if (propertyType == typeof(UInt64))
                {
                    NumericUpDown control = (NumericUpDown)editControl;
                    propertyInfo.SetValue(entity, Convert.ToUInt64(control.Value), null);
                }
                else if (propertyType == typeof(DateTime))
                {
                    DateTimePicker control = (DateTimePicker)editControl;
                    propertyInfo.SetValue(entity, control.Value, null);
                }
                else if (propertyType == typeof(TimeSpan))
                {
                    DateTimePicker control = (DateTimePicker)editControl;
                    propertyInfo.SetValue(entity, control.Value.TimeOfDay, null);
                }
                else if (propertyType == typeof(Boolean))
                {
                    CheckBox control = (CheckBox)editControl;
                    propertyInfo.SetValue(entity, control.Checked, null);
                }
            }
        }

        class TextBoxWithButton : Panel
        {
            TableLayoutPanel layout;
            TextBox textBox;
            Button button;

            public TextBoxWithButton()
            {
                this.textBox = new TextBox();
                this.textBox.Dock = DockStyle.Fill;
                this.textBox.Margin = new Padding(3, 5, 3, 3);

                this.button = new Button();
                this.button.Text = "...";
                this.button.AutoSize = true;
                this.button.Size = new System.Drawing.Size(29, 23);
                this.button.Click += Button_Click;

                this.layout = new TableLayoutPanel();
                this.layout.Dock = DockStyle.Fill;
                this.layout.ColumnCount = 2;
                this.layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                this.layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35F));
                this.layout.RowCount = 1;
                this.layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                this.layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));

                this.layout.Controls.Add(this.button, 1, 0);
                this.layout.Controls.Add(this.textBox, 0, 0);

                this.AutoSize = true;
                this.MinimumSize = new System.Drawing.Size(0, 35);
                this.Controls.Add(this.layout);
            }

            public event EventHandler ButtonClick;

            public String Value
            {
                get { return this.textBox.Text; }
                set { this.textBox.Text = value; }
            }

            private void Button_Click(object sender, EventArgs e)
            {
                if (this.ButtonClick != null)
                {
                    this.ButtonClick(this, EventArgs.Empty);
                }
            }
        }
    }
}
