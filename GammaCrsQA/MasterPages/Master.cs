using GammaCrsQA.TXManager;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace GammaCrsQA.MasterPages
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:GammaCrsQA"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:GammaCrsQA;assembly=GammaCrsQA"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:Master/>
    ///
    /// </summary>
    /// 
    public class Master : Control
    {
        static Master()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Master), new FrameworkPropertyMetadata(typeof(Master)));
        }

        public static readonly DependencyProperty ToolsProperty = DependencyProperty.Register("Tools", typeof(object), typeof(Master), new UIPropertyMetadata());
        public object Tools
        {
            get { return (object)GetValue(ToolsProperty); }
            set { SetValue(ToolsProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var tb = Template.FindName("ToolsTB", this) as TextBox;
            SynchronizationContext sc = SynchronizationContext.Current;
            //GetTemplateChild("toolsRTB") as RichTextBox;
            if (tb != null)
            {
                GammaClientTXManager.GetInstance().OnResultComesBack += (s, e) => {
                    sc.Post((obj) => {
                        if (e.TRANSACTION.TX_TYPE == GammaTransactionType.QATOOLS)
                        {
                            tb.AppendText(string.Format("\r\n{0}: {1}\r\n", e.TRANSACTION.Machine, e.TRANSACTION.TX_RESULT));
                        }
                    }, null);
                    
                };
            }
        }
    }
}
