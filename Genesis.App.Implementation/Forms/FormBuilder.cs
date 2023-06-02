using Genesis.App.Contract.Models.Forms;

namespace Genesis.App.Implementation.Forms
{
    public abstract class FormBuilder<T>
    {
        protected abstract List<FormTab> FormTabs { get; }

        public Form BuildForm(T model)
        {
            PrepareModel(model);
            var controls = CreateFormControls();
            foreach (var control in controls)
            {
                control.Value = GetControlValue(control, model);
                if (control.Type == ControlType.Select)
                    control.Items = GetComboItems(control.EntityType, model);
            }

            return new Form
            {
                Controls = controls,
                Tabs = FormTabs,
                ButtonTypes = GetButtonTypes(model),
            };
        }

        protected virtual void PrepareModel(T model)
        {

        }

        protected abstract IEnumerable<Control> CreateFormControls();

        protected abstract object GetControlValue(Control control, T model);

        protected abstract List<SelectItem> GetComboItems(ControlEntityType itemCode, T model);

        protected abstract List<ButtonType> GetButtonTypes(T model);
    }
}
