using System.Windows;
using System.Windows.Controls;

namespace Office365.User.Loader.Controls
{
    public class ModalControl : ContentControl
    {
        public bool IsActive
        {
            get { return (bool) GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public new static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsActive", typeof (bool), typeof (ModalControl),
                new PropertyMetadata(OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            VisualStateManager.GoToState((ModalControl) dpo, (bool) args.NewValue ? "On" : "Off", false);
        }

        public override void OnApplyTemplate()
        {
            VisualStateManager.GoToState(this, IsActive ? "On" : "Off", false);
            base.OnApplyTemplate();
        }

    }
}