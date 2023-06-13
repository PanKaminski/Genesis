using Genesis.App.Contract.Models.Forms;

namespace Genesis.App.Implementation.Forms
{
    public abstract class FormBuilder<T>
    {
        public Form BuildForm(T model)
        {
            PrepareModel(model);
            var controls = CreateFormControls(model);
            foreach (var control in controls)
            {
                control.Value = GetControlValue(control, model);
                if (control.Type == ControlType.Select)
                    control.Items = GetComboItems(control.EntityType, model);
            }

            return new Form
            {
                Controls = controls,
                Tabs = BuildFormTabs(model),
                ButtonTypes = GetButtonTypes(model),
            };
        }

        protected virtual void PrepareModel(T model) { }

        protected abstract List<FormTab> BuildFormTabs(T model);

        protected abstract IEnumerable<Control> CreateFormControls(T model);

        protected abstract object GetControlValue(Control control, T model);

        protected abstract List<SelectItem> GetComboItems(EntityType itemCode, T model);

        protected abstract List<ButtonType> GetButtonTypes(T model);
    }
}
